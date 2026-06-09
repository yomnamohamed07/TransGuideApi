using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransGuide.Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class addsession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "Routes");

            migrationBuilder.CreateTable(
                name: "SignSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastFrameTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignSessions", x => x.SessionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignSessions");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
