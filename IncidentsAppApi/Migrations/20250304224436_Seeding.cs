using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentsAppApi.Migrations
{
    /// <inheritdoc />
    public partial class Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Incidents",
                columns: new[] { "Id", "Description", "Priority", "Status" },
                values: new object[,]
                {
                    { 1, "Poep op de stoep", "Hoog", "In behandeling" },
                    { 2, "Omgevallen boom", "Hoog", "In behandeling" },
                    { 3, "Graffiti op de muur in een tunnel", "Laag", "Gemeld" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Incidents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Incidents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Incidents",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
