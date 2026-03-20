using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2868ff8-e6b7-4a00-bf69-7ee4a66a1e8a", "AQAAAAIAAYagAAAAEJB9OkGYgADBwQDJcDPt/IUTtyPnO/fDaeHjGK9rVbuc7cyEMWLN50zXsRExiITBHw==", "1d5ef264-b52a-4fdf-9767-f584fdf6e64c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e95222c5-7f4c-47fc-8f6a-fbaeb0ccdaaf", "AQAAAAIAAYagAAAAEJKa30JvACzHXUvKYL463Ov4nPvw2uKouDeMxPQPe9V0JQmWnghIg7tkLioViIrpHQ==", "7b686259-873b-486d-b8de-fae7826359eb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c350119-9236-407d-a169-2fdf07e4d283", "AQAAAAIAAYagAAAAEEC93LkGA1IblokFJD/R69GtPP5iUOvhMRI0EeT257PPE5qr8DZdn4TnoBZcG+YPDA==", "0a5b8207-6bb3-4d2c-8ab5-f2d4e7de4eb0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
