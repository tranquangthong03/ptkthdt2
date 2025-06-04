using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UTEScholarshipSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressStepsToScholarshipApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProgressSteps",
                table: "ScholarshipApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressSteps",
                table: "ScholarshipApplications");
        }
    }
}
