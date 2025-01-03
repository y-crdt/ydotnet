namespace YDotNet.Server.EntityFramework.Migrations;

using FluentMigrator;

[Migration(20250103143219)]
#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1649 // File name should match first type name
public class InitialCreate : Migration
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore MA0048 // File name must match type name
{
    public override void Up()
    {
        Create.Table("YDotNetDocuments")
            .WithColumn(nameof(DocumentEntity.Id)).AsString().PrimaryKey().Identity()
            .WithColumn(nameof(DocumentEntity.Expiration)).AsDateTime().Nullable()
            .WithColumn(nameof(DocumentEntity.Data)).AsBinary().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("YDotNetDocuments");
    }
}
