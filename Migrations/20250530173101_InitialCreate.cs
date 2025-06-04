using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UTEScholarshipSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scholarships",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MinGPA = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ApplicationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    AdditionalRequirements = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarships", x => x.ScholarshipId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    Major = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GPA = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipApplications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ScholarshipId = table.Column<int>(type: "int", nullable: false),
                    ApplicationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Achievements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FamilyEconomicStatus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GPAAtApplication = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_ScholarshipApplications_Scholarships_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarships",
                        principalColumn: "ScholarshipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipApplications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Scholarships",
                columns: new[] { "ScholarshipId", "AcademicYear", "AdditionalRequirements", "Amount", "ApplicationEndDate", "ApplicationStartDate", "CreatedAt", "Description", "MinGPA", "Name", "Quantity", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2024, null, 2000000m, new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(2382), "Học bổng dành cho sinh viên có kết quả học tập xuất sắc", 3.2m, "Học bổng Khuyến khích học tập", 50, 1, new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(2804) },
                    { 2, 2024, "Có giấy xác nhận hộ nghèo hoặc cận nghèo", 1500000m, new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3676), "Học bổng dành cho sinh viên có hoàn cảnh khó khăn", 2.5m, "Học bổng Hỗ trợ sinh viên khó khăn", 100, 1, new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3678) },
                    { 3, 2024, "Có giải thưởng cấp quốc gia hoặc quốc tế", 5000000m, new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3691), "Học bổng dành cho sinh viên có tài năng đặc biệt", 3.7m, "Học bổng Tài năng", 10, 1, new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3692) }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "AcademicYear", "Address", "Class", "CreatedAt", "DateOfBirth", "Email", "FullName", "GPA", "Major", "PhoneNumber", "StudentCode", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2021, "123 Võ Văn Ngân, Thủ Đức, TP.HCM", "ĐHCNTT21A", new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(4109), new DateTime(2003, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "20110001@student.hcmute.edu.vn", "Nguyễn Văn An", 3.5m, "Công nghệ thông tin", "0123456789", "20110001", new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(4591) },
                    { 2, 2021, "456 Nguyễn Văn Cừ, Quận 5, TP.HCM", "ĐHKĐT21B", new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(5077), new DateTime(2003, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "20110002@student.hcmute.edu.vn", "Trần Thị Bình", 3.8m, "Kỹ thuật điện tử", "0987654321", "20110002", new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(5079) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipApplications_ScholarshipId",
                table: "ScholarshipApplications",
                column: "ScholarshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipApplications_StudentId_ScholarshipId",
                table: "ScholarshipApplications",
                columns: new[] { "StudentId", "ScholarshipId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentCode",
                table: "Students",
                column: "StudentCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScholarshipApplications");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
