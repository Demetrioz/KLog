using FluentMigrator;

namespace KLog.DataModel.Migrations.Releases.Release_001
{
    [Migration(001)]
    public class Release_001 : Migration
    {
        public override void Up()
        {
            Create.Table("Application")
                .WithId("ApplicationId")
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Id").AsString(8).NotNullable()
                .WithColumn("Key").AsString(255).NotNullable()
                .WithKLogBase();

            Create.Table("Log")
                .WithId("LogId")
                .WithColumn("Timestamp").AsDateTimeOffset().NotNullable()
                .WithColumn("Level").AsInt32().NotNullable()
                .WithColumn("Source").AsString(255).NotNullable()
                .WithColumn("Subject").AsString(255).Nullable()
                .WithColumn("Component").AsString(255).Nullable()
                .WithColumn("Message").AsString(int.MaxValue).NotNullable()
                .WithColumn("Data").AsString(int.MaxValue).Nullable()
                .WithKLogBase();
        }

        public override void Down()
        {
            Delete.Table("Application");
            Delete.Table("Log");
        }
    }
}
