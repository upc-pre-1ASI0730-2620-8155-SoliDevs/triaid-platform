using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public static class ModelBuilderExtensions
{
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Table name: fall back to the CLR type name when the table
            // has not been explicitly configured yet.
            var tableName = entity.GetTableName();
            entity.SetTableName(string.IsNullOrEmpty(tableName)
                ? entity.ClrType.Name.ToSnakeCase()
                : tableName.ToSnakeCase());

            // Column names
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                    property.SetColumnName(columnName.ToSnakeCase());
            }

            // Key names
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                    key.SetName(keyName.ToSnakeCase());
            }

            // Foreign key constraint names
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var constraintName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                    foreignKey.SetConstraintName(constraintName.ToSnakeCase());
            }

            // Index names
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                    index.SetDatabaseName(indexName.ToSnakeCase());
            }
        }
    }
}
