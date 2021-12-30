using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwirkle.Infra.Repository.Migrations
{
    public partial class IntPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatDate",
                table: "Game",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "Player",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Game",
                newName: "CreatDate");

            migrationBuilder.AlterColumn<byte>(
                name: "Points",
                table: "Player",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
