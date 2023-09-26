using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1330726f-d838-4e3f-a358-e0ca6bdcb18c", "fbc9e95d-b42d-4b44-833c-03ad5f8e47e5", "Administrator", "ADMINISTRATOR" },
                    { "af9eaaeb-3518-4a37-b1f6-1e6935854841", "cb2b234e-bd2e-4141-b7ae-288707981e50", "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1330726f-d838-4e3f-a358-e0ca6bdcb18c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "af9eaaeb-3518-4a37-b1f6-1e6935854841");
        }
    }
}
