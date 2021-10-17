using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwirkle.Infra.Repository.Migrations.Sqlite
{
    public partial class LastTurnPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "LastTurnPoints",
                table: "Player",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTurnPoints",
                table: "Player");
        }
    }
}
