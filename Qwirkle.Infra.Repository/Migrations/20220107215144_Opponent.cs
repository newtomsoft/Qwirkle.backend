using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwirkle.Infra.Repository.Migrations
{
    public partial class Opponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDaoUserDao",
                columns: table => new
                {
                    RegisteredOpponentById = table.Column<int>(type: "int", nullable: false),
                    RegisteredOpponentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDaoUserDao", x => new { x.RegisteredOpponentById, x.RegisteredOpponentsId });
                    table.ForeignKey(
                        name: "FK_UserDaoUserDao_AspNetUsers_RegisteredOpponentById",
                        column: x => x.RegisteredOpponentById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDaoUserDao_AspNetUsers_RegisteredOpponentsId",
                        column: x => x.RegisteredOpponentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "b1cc66bd-0af1-4a64-b2b2-cb53c36ded62");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "a9d7de2f-7b67-47a6-908c-6c14554296b0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "e47ef09e-69de-4ad5-8f66-5e7691f8ebf7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "e791442a-e1e3-42d6-bbbc-253df0a2b23b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "240021a3-bd7f-4ba8-83a2-5c5fc1ac3d33");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "d6687b01-d940-4a7e-9cda-7454a58b0128");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "87efbbaf-b90b-483d-8551-374da00c0b22");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "aed73f53-8103-4f96-ad8b-c5e9f4bf73b4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "f4f7a323-d344-45eb-a608-c3144da7368b", "B69E08FAF5F24E339FE9B41847EC3EDB" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "bd6221c7-6bc2-4d34-abe4-de4e0ae89388", "30386978C4FE4096B7EBF17EE0AE843B" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDaoUserDao_RegisteredOpponentsId",
                table: "UserDaoUserDao",
                column: "RegisteredOpponentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDaoUserDao");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "febc152d-ee52-4b71-9081-9b8e1858e9a2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "e0ead932-9416-4bfd-b28f-75a1cd7cbb75");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "646e5251-5476-44cf-a126-4572013a15c2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "24e14a03-6012-403a-811e-c6a8f0a3bd53");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "451df290-952a-4214-9993-6770d73bee4c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "4ecf87bf-eee0-47e6-9b84-daf284034f01");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "e394cc12-b66f-4de1-8b6a-3c76e4efe075");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "78b740a3-0d97-4588-badc-a951ef572c9a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "6e7923a6-bb51-4323-9659-1316945b5ff0", "79CFAB5831BB49588C2DF5CB197DB024" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "2bde0448-417f-4996-8e6e-7b0788ea061f", "CAA7069E4691479CBE84785ECC4EFA0D" });
        }
    }
}
