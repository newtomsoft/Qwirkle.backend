using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwirkle.Infra.Repository.Migrations.Sqlite
{
    public partial class AddLastTurnSkipped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LastTurnSkipped",
                table: "Player",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTurnSkipped",
                table: "Player");
        }
    }
}
