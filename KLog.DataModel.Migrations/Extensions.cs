using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KLog.DataModel.Migrations
{
    public static class Extensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax WithId(
            this ICreateTableWithColumnSyntax table,
            string columnName
        )
        {
            return table
                .WithColumn(columnName)
                .AsInt32()
                .NotNullable()
                .PrimaryKey()
                .Identity();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithKLogBase(
            this ICreateTableWithColumnSyntax table
        )
        {
            return table
                .WithColumn("Created").AsDateTimeOffset().NotNullable()
                .WithColumn("Modified").AsDateTimeOffset().NotNullable()
                .WithColumn("IsDeleted").AsBoolean().NotNullable();
        }

        public static IServiceCollection AddMigrations(
            this IServiceCollection services,
            string connectionString,
            List<Type> assemblyTypes
        )
        {
            var assemblies = assemblyTypes
                .Select(t => t.Assembly)
                .ToArray();

            return services.AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                    rb.AddPostgres()
                    //rb.AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(assemblies).For.Migrations()
                )
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .Configure<FluentMigratorLoggerOptions>(options =>
                {
                    options.ShowSql = true;
                    options.ShowElapsedTime = true;
                });
        }
    }
}
