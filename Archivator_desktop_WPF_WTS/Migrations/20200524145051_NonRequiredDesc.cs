using Microsoft.EntityFrameworkCore.Migrations;

namespace Archivator_desktop_WPF_WTS.Migrations
{
    public partial class NonRequiredDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                maxLength: 32500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 32500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                type: "nvarchar(max)",
                maxLength: 32500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32500,
                oldNullable: true);
        }
    }
}
