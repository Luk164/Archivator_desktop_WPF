using Microsoft.EntityFrameworkCore.Migrations;

namespace Archivator_desktop_WPF_WTS.Migrations
{
    public partial class InternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Items",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "InternalId",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubCategory",
                table: "Items",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "InternalId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Items");
        }
    }
}
