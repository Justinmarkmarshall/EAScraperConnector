using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EAScraperConnector.Migrations
{
    public partial class AddUpperCaseDateToAuditAndRemoveNonUniqueResultsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultsCount",
                table: "Audit");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Audit",
                newName: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Audit",
                newName: "date");

            migrationBuilder.AddColumn<int>(
                name: "ResultsCount",
                table: "Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
