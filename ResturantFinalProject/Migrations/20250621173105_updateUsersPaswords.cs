using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResturantFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class updateUsersPaswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c128ce0f-9942-43d8-8166-ae2e7448212e", "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "chef-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "94a45fb0-cac0-468d-a938-945d09027906", "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "customer-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "38f3b2aa-cb6d-4cac-a1ab-5b770d660f25", "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "waiter-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "35970672-d173-412e-8147-93b346a261f9", "AQAAAAIAAYagAAAAEJoXvY9a7Yc1z4z5gK8qN3rXh2mW9pL8vT6uR5wZ2kM7nP4bQ9cF3jH6mY8vL2xK9Q==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0cb85a35-8fd6-483e-81e7-b078df77d6b0", "AQAAAAIAAYagAAAAEKZPJ8QbQlFJ9KpG6fJn4O2VJlGmUpYU8LYq5cNrRyF3HJxLgZy4/MpJnY3XgD7P0A==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "chef-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "79821bb6-600b-4f70-a0f2-da3a66279513", "AQAAAAIAAYagAAAAEKZPJ8QbQlFJ9KpG6fJn4O2VJlGmUpYU8LYq5cNrRyF3HJxLgZy4/MpJnY3XgD7P0B==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "customer-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "54c2945f-c580-4bef-a402-faa32ce14aff", "AQAAAAIAAYagAAAAEKZPJ8QbQlFJ9KpG6fJn4O2VJlGmUpYU8LYq5cNrRyF3HJxLgZy4/MpJnY3XgD7P0D==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "waiter-1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "34ebc438-67ee-4697-a1f9-b2420de50d0a", "AQAAAAIAAYagAAAAEKZPJ8QbQlFJ9KpG6fJn4O2VJlGmUpYU8LYq5cNrRyF3HJxLgZy4/MpJnY3XgD7P0C==" });
        }
    }
}
