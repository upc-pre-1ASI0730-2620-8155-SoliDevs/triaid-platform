using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Incidents.Infrastructure.Persistence.EntityFrameworkCore.Schema;

/// <summary>
/// Upgrades the physical schema of the Incidents bounded context on startup.
/// </summary>
/// <remarks>
/// The platform creates its schema with <c>EnsureCreated()</c> instead of EF migrations, so an
/// existing deployment still has the alerting table under its legacy name and without the audit
/// columns of <see cref="Domain.Model.Aggregates.Incident"/>. This upgrade renames <c>alert</c>
/// to <c>incidents</c> and adds any missing column once, preserving and backfilling every stored
/// row. It is a no-op on fresh or already upgraded databases.
/// </remarks>
public static class IncidentsSchema
{
    private const string LegacyTable = "alert";
    private const string CurrentTable = "incidents";

    /// <summary>
    /// Audit columns added by the incidents aggregate that the legacy alert table lacks, with
    /// their definition and the backfill expression applied to existing rows.
    /// </summary>
    private static readonly (string Column, string Definition, string Backfill)[] AddedColumns =
    [
        ("created_at", "DATETIME NULL", "raised_at"),
        ("updated_at", "DATETIME NULL", "COALESCE(escalated_at, acknowledged_at, raised_at)")
    ];

    /// <summary>
    /// Brings the incidents table to the current schema when the upgrade has not run yet.
    /// </summary>
    /// <remarks>
    /// Must run before <c>EnsureCreated()</c>: once the table carries its new name and columns,
    /// EF finds it as is and neither recreates nor ignores it. Every step is idempotent.
    /// </remarks>
    /// <param name="services">The application service provider used to resolve the database context.</param>
    public static async Task UpgradeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IncidentsSchema));

        DbConnection connection = context.Database.GetDbConnection();
        var wasOpen = connection.State == ConnectionState.Open;
        if (!wasOpen) await connection.OpenAsync();

        try
        {
            if (!await TableExistsAsync(connection, CurrentTable))
            {
                if (!await TableExistsAsync(connection, LegacyTable))
                {
                    logger.LogDebug("No incidents table yet; EnsureCreated will create it with the current schema");
                    return;
                }

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"RENAME TABLE `{LegacyTable}` TO `{CurrentTable}`";
                    await command.ExecuteNonQueryAsync();
                }

                logger.LogInformation("Renamed the legacy {Legacy} table to {Current}; existing rows were preserved",
                    LegacyTable, CurrentTable);
            }

            foreach (var (column, definition, backfill) in AddedColumns)
            {
                if (await ColumnExistsAsync(connection, CurrentTable, column)) continue;

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER TABLE `{CurrentTable}` ADD COLUMN `{column}` {definition}";
                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE `{CurrentTable}` SET `{column}` = {backfill} WHERE `{column}` IS NULL";
                    await command.ExecuteNonQueryAsync();
                }

                logger.LogInformation("Added the {Column} column to {Current} and backfilled existing rows", column, CurrentTable);
            }
        }
        finally
        {
            if (!wasOpen) await connection.CloseAsync();
        }
    }

    private static DbParameter Parameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        return parameter;
    }

    private static async Task<bool> ColumnExistsAsync(DbConnection connection, string table, string column)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*) FROM information_schema.columns
            WHERE table_schema = DATABASE() AND table_name = @table AND column_name = @column
            """;
        command.Parameters.Add(Parameter(command, "@table", table));
        command.Parameters.Add(Parameter(command, "@column", column));

        var result = await command.ExecuteScalarAsync();
        return result is not null && Convert.ToInt64(result) > 0;
    }


    private static async Task<bool> TableExistsAsync(DbConnection connection, string name)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*) FROM information_schema.tables
            WHERE table_schema = DATABASE() AND table_name = @name
            """;
        command.Parameters.Add(Parameter(command, "@name", name));

        var result = await command.ExecuteScalarAsync();
        return result is not null && Convert.ToInt64(result) > 0;
    }
}
