using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Schema;

/// <summary>
/// Upgrades the physical schema of the Patient Registration bounded context on startup.
/// </summary>
/// <remarks>
/// The platform creates its schema with <c>EnsureCreated()</c> instead of EF migrations, so an
/// existing deployment lacks the tenancy column on patients and episodes. This upgrade adds it
/// once and is a no-op on fresh or already upgraded databases. Rows created before the column
/// existed get an empty institution until an administrator tags them.
/// </remarks>
public static class RegistrationSchema
{
    private static readonly string[] Tables = ["patients", "triage_episodes"];

    /// <summary>Adds the <c>institution</c> column to patients and episodes when missing.</summary>
    /// <param name="services">The application service provider used to resolve the database context.</param>
    public static async Task UpgradeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(RegistrationSchema));

        DbConnection connection = context.Database.GetDbConnection();
        var wasOpen = connection.State == ConnectionState.Open;
        if (!wasOpen) await connection.OpenAsync();
        try
        {
            foreach (var table in Tables)
            {
                if (await ColumnExistsAsync(connection, table, "institution")) continue;

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER TABLE `{table}` ADD COLUMN `institution` VARCHAR(150) NULL";
                    await command.ExecuteNonQueryAsync();
                }

                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE `{table}` SET `institution` = '' WHERE `institution` IS NULL; ALTER TABLE `{table}` MODIFY `institution` VARCHAR(150) NOT NULL DEFAULT ''";
                    await command.ExecuteNonQueryAsync();
                }

                logger.LogInformation("Added the institution column to {Table}", table);
            }
        }
        finally
        {
            if (!wasOpen) await connection.CloseAsync();
        }
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

    private static DbParameter Parameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        return parameter;
    }
}
