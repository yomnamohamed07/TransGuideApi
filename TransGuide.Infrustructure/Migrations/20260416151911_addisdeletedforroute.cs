using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransGuide.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class addisdeletedforroute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Routes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Routes");
        }
    }
}
