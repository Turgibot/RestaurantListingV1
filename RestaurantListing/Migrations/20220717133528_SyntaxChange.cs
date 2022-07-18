using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantListing.Migrations
{
    public partial class SyntaxChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VistedOn",
                table: "Restaurants",
                newName: "VisitedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitedOn",
                table: "Restaurants",
                newName: "VistedOn");
        }
    }
}
