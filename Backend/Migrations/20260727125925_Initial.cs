using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GPA = table.Column<double>(type: "float", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrollDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "City", "Course", "Email", "EnrollDate", "GPA", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Lahore", "BSIT", "abdullah@student.com", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.9500000000000002, true, "Abdullah Imran" },
                    { 2, "Karachi", "BSCS", "ali@student.com", new DateTime(2024, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.7000000000000002, true, "Ali Hassan" },
                    { 3, "Lahore", "BSIT", "sara@student.com", new DateTime(2024, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.8500000000000001, true, "Sara Khan" },
                    { 4, "Islamabad", "BBA", "usman@student.com", new DateTime(2024, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.6000000000000001, true, "Usman Malik" },
                    { 5, "Karachi", "BSCS", "ayesha@student.com", new DateTime(2024, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.8999999999999999, true, "Ayesha Siddiqui" },
                    { 6, "Multan", "BSIT", "bilal@student.com", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.3999999999999999, true, "Bilal Ahmed" },
                    { 7, "Faisalabad", "BSCS", "fatima@student.com", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.5499999999999998, true, "Fatima Noor" },
                    { 8, "Rawalpindi", "BSIT", "hamza@student.com", new DateTime(2024, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.7999999999999998, true, "Hamza Ali" },
                    { 9, "Peshawar", "BBA", "zara@student.com", new DateTime(2024, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.1000000000000001, true, "Zara Ahmed" },
                    { 10, "Quetta", "BSIT", "omar@student.com", new DateTime(2024, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.3999999999999999, true, "Omar Farooq" },
                    { 11, "Lahore", "BSCS", "hina@student.com", new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.8999999999999999, true, "Hina Rizvi" },
                    { 12, "Karachi", "BSIT", "rayan@student.com", new DateTime(2024, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.2000000000000002, true, "Rayan Khan" },
                    { 13, "Islamabad", "BBA", "mahnoor@student.com", new DateTime(2024, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.1000000000000001, true, "Mahnoor Sheikh" },
                    { 14, "Multan", "BSCS", "zain@student.com", new DateTime(2024, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.2999999999999998, true, "Zain Abbas" },
                    { 15, "Faisalabad", "BSIT", "sana@student.com", new DateTime(2024, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.7000000000000002, true, "Sana Tariq" },
                    { 16, "Rawalpindi", "BBA", "adeel@student.com", new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.5800000000000001, true, "Adeel Hussain" },
                    { 17, "Peshawar", "BSIT", "nadia@student.com", new DateTime(2024, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.5, true, "Nadia Jamil" },
                    { 18, "Quetta", "BSCS", "shahid@student.com", new DateTime(2024, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.8999999999999999, true, "Shahid Afridi" },
                    { 19, "Lahore", "BBA", "alishba@student.com", new DateTime(2024, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.0, true, "Alishba Akhtar" },
                    { 20, "Karachi", "BSIT", "haris@student.com", new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.2999999999999998, true, "Haris Malik" },
                    { 21, "Islamabad", "BSCS", "eman@student.com", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.1499999999999999, true, "Eman Fatima" },
                    { 22, "Multan", "BSIT", "raza@student.com", new DateTime(2024, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.8500000000000001, true, "Raza Haider" },
                    { 23, "Faisalabad", "BBA", "iqra@student.com", new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.25, true, "Iqra Nasir" },
                    { 24, "Rawalpindi", "BSCS", "danish@student.com", new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.0, true, "Danish Raza" },
                    { 25, "Peshawar", "BSIT", "hira@student.com", new DateTime(2024, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.0499999999999998, true, "Hira Batool" },
                    { 26, "Quetta", "BBA", "mohsin@student.com", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.4500000000000002, true, "Mohsin Ali" },
                    { 27, "Lahore", "BSCS", "sadia@student.com", new DateTime(2024, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.3500000000000001, true, "Sadia Anwar" },
                    { 28, "Karachi", "BSIT", "sufyan@student.com", new DateTime(2024, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.1499999999999999, true, "Sufyan Ahmed" },
                    { 29, "Islamabad", "BBA", "aleena@student.com", new DateTime(2024, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.75, true, "Aleena Khan" },
                    { 30, "Multan", "BSCS", "hassanr@student.com", new DateTime(2024, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.8, true, "Hassan Raza" },
                    { 31, "Faisalabad", "BSIT", "mariam@student.com", new DateTime(2024, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.5, true, "Mariam Asif" },
                    { 32, "Rawalpindi", "BBA", "taha@student.com", new DateTime(2024, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.2000000000000002, true, "Taha Ahmed" },
                    { 33, "Peshawar", "BSCS", "mehak@student.com", new DateTime(2024, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.9500000000000002, true, "Mehak Aslam" },
                    { 34, "Quetta", "BSIT", "usamac@student.com", new DateTime(2024, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2.6000000000000001, true, "Usama Chaudhry" },
                    { 35, "Lahore", "BBA", "hania@student.com", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.6000000000000001, true, "Hania Aamir" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", "Admin", "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG", "Admin" },
                    { 2, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "abdullah@gmail.com", "abdullah", "$2b$11$Glz.i7DzU/P8b4/9n/Hwc.EndWg4drs7VofW3FEM4/5GghBZrYdwG", "admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students1");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
