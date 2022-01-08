using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qwirkle.Infra.Repository.Migrations
{
    public partial class BookmarkedOpponents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDaoUserDao",
                columns: table => new
                {
                    BookmarkedById = table.Column<int>(type: "int", nullable: false),
                    BookmarkedOpponentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDaoUserDao", x => new { x.BookmarkedById, x.BookmarkedOpponentsId });
                    table.ForeignKey(
                        name: "FK_UserDaoUserDao_AspNetUsers_BookmarkedById",
                        column: x => x.BookmarkedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDaoUserDao_AspNetUsers_BookmarkedOpponentsId",
                        column: x => x.BookmarkedOpponentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "743c446f-3a40-4879-a389-5f4120284133");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "4218c4ad-95c8-40c3-9521-957dc984ea9a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "01df5356-a8da-4813-9de8-0cf1ab29a64c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "1c5b787f-1852-4d3c-a484-ad08975c5e5e");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "11ea4ca2-ef23-4820-af8c-c34b34eca13c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "4a847d5e-d6cb-4ca2-b76a-34d320adfc12");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "a1285a56-ec5a-48d7-bf3a-50436ffc6f43");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "bbb32ebb-34a6-40d8-8e79-288475e81e73");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "0c458274-9b06-4522-8814-ceab1321eb72", "9F016EFFE03B49B28E1E20CA38EDCE3A" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "d7b8a386-96e7-4486-8f34-00d88ba43567", "CC952740949B42409175E35F0299D44C" });

            migrationBuilder.CreateIndex(
                name: "IX_UserDaoUserDao_BookmarkedOpponentsId",
                table: "UserDaoUserDao",
                column: "BookmarkedOpponentsId");
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
