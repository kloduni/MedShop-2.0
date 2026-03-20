using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedShop.Infrastructure.Migrations
{
    public partial class UpdatedUserPswds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fc405068-588b-4d30-9781-a323d4f4ed0a", "AQAAAAEAACcQAAAAECNZX7gyhAkg9OL891TaCO4w434Or7458BVjEjwlCZnEbc5OPIOT+avChzLm9PTzEw==", "ba0a462d-a374-4e93-aad9-43688b63bdf2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6012423-8f3d-4a4f-9eb1-457df49c37ff", "AQAAAAEAACcQAAAAEFmkKJJ9m9WTWaH4PLi73spdRDDofGFhi3VHoMIcIHSFMm4MomgWbFWp8Z7MFBTEXA==", "ddfb0eee-6405-4aa5-b54d-4808d7e62ef5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "263d5341-3373-4732-8b51-362bad3316bb", "AQAAAAEAACcQAAAAECaUMsX4iC79Tl/8wgS6kBx/CLB+0MNSPQ9aJihv4NcLdoiwaM/gM9x/LX9s8+wqcQ==", "5764458d-cad5-431e-a52a-f0deb7093c1e" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9f01f7b1-c9bb-49bc-b3b8-0b6426a0d06c", "AQAAAAEAACcQAAAAEGDheZhG+w64XhuDN5TyLDB+MaOVQ6wKRNqu07VdX2BFaP+anVw1K8nXWlozodGb7w==", "ca35b258-abd3-448f-a2d8-0220de56d2f5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a99ad296-8f6a-4ce1-a244-ddc890762443", "AQAAAAEAACcQAAAAENwAXBJx97YePzCmHJE8jktkR5KAJxFGnNGe2+7951l4JfzmaJlTynO5rlXGDABLJw==", "fcaf218e-42d1-4957-b028-dd02ca7e1256" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d650ef0a-5f7c-4baf-86ec-45a549764939", "AQAAAAEAACcQAAAAEAXwcga7EbyP5K+4+NYkBgSJa7PBd+U/Y3D3aE+A/SyMHt6D5C6j9fQ+DaiVIWqyJg==", "9910c6c9-d766-48c8-92bc-2086afd45051" });
        }
    }
}
