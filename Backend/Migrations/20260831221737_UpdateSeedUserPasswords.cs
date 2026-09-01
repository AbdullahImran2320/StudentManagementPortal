using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2b$12$ozwluGM8jahSdFWdlXti3ejmtFyL2ImvmjmwMSBd3gYDwJHwLHhF6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash", "Role" },
                values: new object[] { "abdullah58rajput@gmail.com", "$2b$12$ozwluGM8jahSdFWdlXti3ejmtFyL2ImvmjmwMSBd3gYDwJHwLHhF6", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash", "Role" },
                values: new object[] { "abdullah@gmail.com", "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG", "admin" });
        }
    }
}
