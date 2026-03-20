using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedShop.Infrastructure.Migrations
{
    public partial class UsersProductsConfigured : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UsersProducts",
                columns: new[] { "ProductId", "UserId" },
                values: new object[,]
                {
                    { 9, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { 10, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { 11, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { 12, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" },
                    { 5, "89159c08-2f95-456f-91ea-75136c030b7b" },
                    { 6, "89159c08-2f95-456f-91ea-75136c030b7b" },
                    { 7, "89159c08-2f95-456f-91ea-75136c030b7b" },
                    { 8, "89159c08-2f95-456f-91ea-75136c030b7b" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 9, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 10, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 11, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 12, "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 5, "89159c08-2f95-456f-91ea-75136c030b7b" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 6, "89159c08-2f95-456f-91ea-75136c030b7b" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 7, "89159c08-2f95-456f-91ea-75136c030b7b" });

            migrationBuilder.DeleteData(
                table: "UsersProducts",
                keyColumns: new[] { "ProductId", "UserId" },
                keyValues: new object[] { 8, "89159c08-2f95-456f-91ea-75136c030b7b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4c263e6-d49a-4320-bc96-3a8c34110d29", "AQAAAAEAACcQAAAAEMr89YJZv0O4qMt8dpQCekMXbA6XW1Pkf68fwGuhLLgCGtpiY1pbcdAaQoojqSeVpg==", "17ca22af-0f32-4833-9751-467d5469e873" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "21023afa-e7e7-4d4b-b3b1-fceb3990d465", "AQAAAAEAACcQAAAAEMWHm70xhFWIHfHiVNyrB9SJBhW3pJB3A5YWdvd/eC/bZhASgjQoZZCbfLovkxIeeQ==", "5aede226-dadc-4564-89a6-d6f96fdeeaac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d3b64b7-f3e8-406d-b2ca-5e0647c894cc", "AQAAAAEAACcQAAAAEMgKENEO50tC3QJKUScsS1qqqT8NxRgRTWjw1R1Ue+jfqO0/Z/yBwTmr4XiFC2HYwg==", "bffe309b-2339-4c1f-bc51-82a19e2beea9" });
        }
    }
}
