using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentingSystem2025.Data.Migrations
{
    public partial class AddUserIdToCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId",
                table: "Customers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId");
        }
    }
}
