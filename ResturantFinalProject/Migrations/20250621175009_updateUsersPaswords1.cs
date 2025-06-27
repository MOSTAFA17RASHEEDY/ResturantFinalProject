using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResturantFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class updateUsersPaswords1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                column: "ConcurrencyStamp",
                value: "01f17b5b-b359-4c6a-96aa-796c907b49f3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "chef-1",
                column: "ConcurrencyStamp",
                value: "3223039b-ba62-4275-bb34-581d6f80ec2a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "customer-1",
                column: "ConcurrencyStamp",
                value: "96997a10-75f3-4ac6-a3e8-f7d68d81ee8d");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "waiter-1",
                column: "ConcurrencyStamp",
                value: "bd0fa027-a0e8-44e3-a8c5-a3690828b6b7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                column: "ConcurrencyStamp",
                value: "c128ce0f-9942-43d8-8166-ae2e7448212e");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "chef-1",
                column: "ConcurrencyStamp",
                value: "94a45fb0-cac0-468d-a938-945d09027906");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "customer-1",
                column: "ConcurrencyStamp",
                value: "38f3b2aa-cb6d-4cac-a1ab-5b770d660f25");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "waiter-1",
                column: "ConcurrencyStamp",
                value: "35970672-d173-412e-8147-93b346a261f9");
        }
    }
}
