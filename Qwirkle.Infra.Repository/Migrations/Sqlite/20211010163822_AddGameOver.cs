using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwirkle.Infra.Repository.Migrations.Sqlite
{
    public partial class AddGameOver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GameOver",
                table: "Game",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameOver",
                table: "Game");
        }
    }
}
