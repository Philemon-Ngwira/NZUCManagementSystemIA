using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NZUCManagementSystemIA.Server.Data.Migrations
{
    public partial class RolesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "23afd8d3-6c0d-4a7d-9597-9b9810fb9a42", "2539d7cf-3670-47b8-9a6d-13f8bb0cd340", "Reviewer", "REVIEWER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ba7e1c72-d4d3-4242-946b-22653deb6dcb", "98a7f878-8256-44a4-8439-d4e1f5c78b1c", "Authorizer", "AUTHORIZER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23afd8d3-6c0d-4a7d-9597-9b9810fb9a42");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba7e1c72-d4d3-4242-946b-22653deb6dcb");
        }
    }
}
