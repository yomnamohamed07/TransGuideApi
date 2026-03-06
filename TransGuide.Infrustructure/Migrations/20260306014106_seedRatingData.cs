using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransGuide.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class seedRatingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rating",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Excellent" },
                    { 2, "Very Good" },
                    { 3, "Good" },
                    { 4, "Bad" },
                    { 5, "Very Bad" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rating",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rating",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rating",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rating",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rating",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
