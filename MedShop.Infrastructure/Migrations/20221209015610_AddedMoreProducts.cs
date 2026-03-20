using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedShop.Infrastructure.Migrations
{
    public partial class AddedMoreProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description" },
                values: new object[] { 3, "Used for scalping." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description" },
                values: new object[] { 2, "Used for cutting." });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsActive", "Price", "ProductName", "Quantity" },
                values: new object[,]
                {
                    { 5, 4, "Tube used for various purposes.", "https://www.helmed.bg/media/t44s4/1883.webp", true, 1.73m, "Cannula", 10 },
                    { 6, 5, "Inspection instrument used to look deep into the body.", "https://www.msschippers.com/products/images/00/0010816/0010816_fotodtp_1_750x750_1459677832333.jpg", true, 105.20m, "Endoscope", 10 },
                    { 7, 7, "Storage and containment of gasses.", "https://www.amcaremed.com/wp-content/uploads/2020/01/steel-seamless-medical-gas-cylinder.jpg", true, 55m, "Gas cylinder", 10 },
                    { 8, 5, "Medical device used to look into ears.", "https://m.media-amazon.com/images/I/31W7wpCID4L.jpg", true, 13.20m, "Otoscope", 10 },
                    { 9, 1, "Used for auscultation - listening to inner sounds of the human body", "https://www.veterinarna-apteka.com/images/products/dc02e706cde3923b65404ac663791616.jpg", true, 20m, "Stethoscope", 10 },
                    { 10, 5, "Used for measuring temperature.", "https://tfa.bg/userfiles/productlargeimages/product_1820.jpg", true, 3.70m, "Thermometer", 10 },
                    { 11, 2, "Used for blood transfusion.", "https://www.smd-medical.com/wp-content/uploads/2017/01/7-Blood-Transfusion-Set-480x480.jpg", true, 12.30m, "Transfusion kit", 10 },
                    { 12, 5, "Used to deliver medicine in the form of a mist inhaled orally.", "https://medicaldepot.com.ph/wp-content/uploads/products/3b9a64f6-38fa-fdfc-cadb-74e041fda45f.jpg", true, 40.20m, "Nebulizer", 10 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f591e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eade2a63-45c0-4935-95ab-8fc2b7d17ac0", "AQAAAAEAACcQAAAAEIpk3E+ISygoGa6Lag9yIOWx6zY7sIL0Hy/8QBsNQaDo9snUG8CWLbzC+3Dpto/CRQ==", "777b5a56-7ceb-4947-b44a-4dd2ea29e85d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "89159c08-2f95-456f-91ea-75136c030b7b",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f148428-2a07-4cc5-bd4d-1714053a38c0", "AQAAAAEAACcQAAAAEGu+klWZ+b9sc6J3ejCZtctbvRWv5SWBHpKmzwylB+oQyUzEHG0xMbVe7C5kfTiGsQ==", "b885dca5-8946-46fc-9e5c-908ddc169785" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c198-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "471eeb4d-ee8f-477e-b413-b9d2ca8a62b2", "AQAAAAEAACcQAAAAENbGjG2apBZ2gXgxXM7vx0DxIsvts21NMILKEBW7c9xjxbJWMrjWETWlxzLyL/OsUw==", "11ea21be-e830-4b6b-8ade-1c5eb9e7b36f" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description" },
                values: new object[] { 7, "General instrument." });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description" },
                values: new object[] { 7, "General instrument." });
        }
    }
}
