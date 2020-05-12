using Microsoft.EntityFrameworkCore.Migrations;

namespace Archivator_desktop_WPF_WTS.Migrations
{
    public partial class AlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Items_Category_InternalId_SubCategory",
                table: "Items",
                columns: new[] { "Category", "InternalId", "SubCategory" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Items_Category_InternalId_SubCategory",
                table: "Items");
        }
    }
}
