using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedShop.Infrastructure.Migrations
{
    public partial class UserActiveStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "IsActive", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eade2a63-45c0-4935-95ab-8fc2b7d17ac0", true, "AQAAAAEAACcQAAAAEIpk3E+ISygoGa6Lag9yIOWx6zY7sIL0Hy/8QBsNQaDo9snUG8CWLbzC+3Dpto/CRQ==", "777b5a56-7ceb-4947-b44a-4dd2ea29e85d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "IsActive", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f148428-2a07-4cc5-bd4d-1714053a38c0", true, "AQAAAAEAACcQAAAAEGu+klWZ+b9sc6J3ejCZtctbvRWv5SWBHpKmzwylB+oQyUzEHG0xMbVe7C5kfTiGsQ==", "b885dca5-8946-46fc-9e5c-908ddc169785" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "IsActive", "PasswordHash", "SecurityStamp" },
                values: new object[] { "471eeb4d-ee8f-477e-b413-b9d2ca8a62b2", true, "AQAAAAEAACcQAAAAENbGjG2apBZ2gXgxXM7vx0DxIsvts21NMILKEBW7c9xjxbJWMrjWETWlxzLyL/OsUw==", "11ea21be-e830-4b6b-8ade-1c5eb9e7b36f" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23a9e6ba-beb5-4215-a6a3-91a598abc6b9", "AQAAAAEAACcQAAAAEAmeYRYm9+l0O65FXIeUZriZPQoMCqpqeEdhKgQylqMNSTn3zE5hRxwHAtBK8AMFwQ==", "20b9cd23-e3ad-4fa7-ae3d-07616f82222f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0749268a-5356-4d70-8f27-6d00ad46e1f5", "AQAAAAEAACcQAAAAECQv7EBcCk6yzA6/XI3j6QXzVzuM/OWxz61XyD6LTYdk7Gk1G7jXnwv8tbLu8zD56Q==", "c72fd83f-c68e-4f08-aae5-ca55aede3f6f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "479cd3a4-8145-474a-a822-62d112132259", "AQAAAAEAACcQAAAAEA58S2+z0lpZmg0gCOVXd5MAJ5FbAxjkO+uiFXJmiVfGNy1v5h64iXBOULZO9b0+ww==", "ea685ed6-2c52-4963-9ea0-e27b6ba0fb33" });
        }
    }
}
