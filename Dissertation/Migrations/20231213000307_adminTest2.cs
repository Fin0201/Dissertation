using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dissertation.Migrations
{
    /// <inheritdoc />
    public partial class adminTest2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83f97975-2d8c-438d-ae8d-06b82bde7c3d");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0ece0674-22c1-4564-9c22-0bca1b5bc76a", "a473aee9-03ea-4eab-8b15-8ac5eff16e83" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ece0674-22c1-4564-9c22-0bca1b5bc76a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a473aee9-03ea-4eab-8b15-8ac5eff16e83");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1c05f9ea-3101-437c-a0d4-b53da23869c3", "388c3bfd-1e49-439a-b4aa-aaab98fcc8f3", "Customer", "CUSTOMER" },
                    { "bb64277e-bc6a-4abe-b30e-800f51747959", "c9bcfdc3-7620-4a60-9f1c-5edcef1808bf", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "94862019-92e0-4337-9fc3-e5dfdc035f50", 0, "7b0b83db-d4e3-4ead-a676-0f3c5c1e7a10", "admin@admin.com", false, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEDXHdwkjdOtJABEAuj3js0NacbrhzQBDPZUYwdOHh1CuEF1gXtNLTJ0uGPNd+XfOAg==", null, false, "7bc47565-01f6-42c7-a115-6bea85366914", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "bb64277e-bc6a-4abe-b30e-800f51747959", "94862019-92e0-4337-9fc3-e5dfdc035f50" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c05f9ea-3101-437c-a0d4-b53da23869c3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "bb64277e-bc6a-4abe-b30e-800f51747959", "94862019-92e0-4337-9fc3-e5dfdc035f50" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bb64277e-bc6a-4abe-b30e-800f51747959");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "94862019-92e0-4337-9fc3-e5dfdc035f50");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0ece0674-22c1-4564-9c22-0bca1b5bc76a", "1744ef3a-a652-4fd9-ab16-ad8736958fc5", "Admin", "ADMIN" },
                    { "83f97975-2d8c-438d-ae8d-06b82bde7c3d", "ad39b48e-c043-4463-89b5-9da196f93379", "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a473aee9-03ea-4eab-8b15-8ac5eff16e83", 0, "2563d26e-afe9-48fb-8702-ed2f9f04fc36", "admin@admin.com", false, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEL61oq4nWjJ6YQrJ0KCr517tsIsPLnhP6e8zccPcxOJ18qYb614G7X+MihDsTyXRQA==", null, false, "0ab3081c-19bd-4d22-9fea-46f54e18fe86", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0ece0674-22c1-4564-9c22-0bca1b5bc76a", "a473aee9-03ea-4eab-8b15-8ac5eff16e83" });
        }
    }
}
