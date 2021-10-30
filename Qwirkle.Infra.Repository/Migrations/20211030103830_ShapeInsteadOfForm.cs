using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwirkle.Infra.Repository.Migrations
{
    public partial class ShapeInsteadOfForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Form",
                table: "Tile",
                newName: "Shape");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Shape",
                table: "Tile",
                newName: "Form");
        }
    }
}
