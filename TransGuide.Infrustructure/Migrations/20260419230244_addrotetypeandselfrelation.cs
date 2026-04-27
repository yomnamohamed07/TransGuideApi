using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TransGuide.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class addrotetypeandselfrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentRouteId",
                table: "Routes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteTypeId",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RouteTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "RouteTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "باص" },
                    { 2, "مترو" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ParentRouteId",
                table: "Routes",
                column: "ParentRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteTypeId",
                table: "Routes",
                column: "RouteTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_RouteTypes_RouteTypeId",
                table: "Routes",
                column: "RouteTypeId",
                principalTable: "RouteTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Routes_ParentRouteId",
                table: "Routes",
                column: "ParentRouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_RouteTypes_RouteTypeId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Routes_ParentRouteId",
                table: "Routes");

            migrationBuilder.DropTable(
                name: "RouteTypes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_ParentRouteId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_RouteTypeId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ParentRouteId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "RouteTypeId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "type",
                table: "Routes");
        }
    }
}
