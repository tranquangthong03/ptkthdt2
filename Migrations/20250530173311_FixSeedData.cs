using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UTEScholarshipSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(2382), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(2804) });

            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3676), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3678) });

            migrationBuilder.UpdateData(
                table: "Scholarships",
                keyColumn: "ScholarshipId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3691), new DateTime(2025, 5, 31, 0, 31, 0, 662, DateTimeKind.Local).AddTicks(3692) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(4109), new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(4591) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(5077), new DateTime(2025, 5, 31, 0, 31, 0, 664, DateTimeKind.Local).AddTicks(5079) });
        }
    }
}
