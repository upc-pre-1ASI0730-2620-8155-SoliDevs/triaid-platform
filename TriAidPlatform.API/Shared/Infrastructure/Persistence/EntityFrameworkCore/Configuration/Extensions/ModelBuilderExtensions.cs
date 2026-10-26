using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            //Replace all table names
            entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
            //Replace column names from tables
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName()!.ToSnakeCase());
            }
            //Replace key names
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()!.ToSnakeCase());
            }
            //Replace foreign key names
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName()!.ToSnakeCase());
            }
            //Replace index names
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()!.ToSnakeCase());
            }
            
        }
    }
}