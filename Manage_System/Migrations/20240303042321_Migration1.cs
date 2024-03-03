using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facultie__3213E83FA3774240", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Magazine",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    startYear = table.Column<DateTime>(type: "date", nullable: true),
                    closeYear = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Magazine__3213E83FD5FB45EE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3213E83F752E2C11", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    fullName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    roleId = table.Column<int>(type: "int", nullable: true),
                    facultyId = table.Column<int>(type: "int", nullable: true),
                    createDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    avatar = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3213E83F56046F46", x => x.id);
                    table.ForeignKey(
                        name: "FK__Users__facultyId__5AEE82B9",
                        column: x => x.facultyId,
                        principalTable: "Faculties",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Users__roleId__5BE2A6F2",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    submissionDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    lastModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<bool>(type: "bit", unicode: false, maxLength: 50, nullable: true),
                    publics = table.Column<bool>(type: "bit", unicode: false, maxLength: 50, nullable: true),
                    magazineId = table.Column<int>(type: "int", nullable: true),
                    shortDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contribu__3213E83F4A1504BF", x => x.id);
                    table.ForeignKey(
                        name: "FK__Contribut__magaz__5812160E",
                        column: x => x.magazineId,
                        principalTable: "Magazine",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Contribut__userI__59063A47",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contributionId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true),
                    commentText = table.Column<string>(type: "text", nullable: true),
                    commentDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__3213E83FD20A86B9", x => x.id);
                    table.ForeignKey(
                        name: "FK__Comments__contri__5629CD9C",
                        column: x => x.contributionId,
                        principalTable: "Contributions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__Comments__userId__571DF1D5",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "imgFile",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stype = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    url = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    contributionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__imgFile__3213E83F67F825DA", x => x.id);
                    table.ForeignKey(
                        name: "FK__imgFile__contrib__59FA5E80",
                        column: x => x.contributionId,
                        principalTable: "Contributions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_contributionId",
                table: "Comments",
                column: "contributionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_userId",
                table: "Comments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_magazineId",
                table: "Contributions",
                column: "magazineId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_userId",
                table: "Contributions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_imgFile_contributionId",
                table: "imgFile",
                column: "contributionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_facultyId",
                table: "Users",
                column: "facultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_roleId",
                table: "Users",
                column: "roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "imgFile");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "Magazine");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
