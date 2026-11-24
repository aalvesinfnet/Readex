using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readex.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class CriarContasDeLeitores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leitores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeUsuario = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    NomeUsuarioNormalizado = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    EmailNormalizado = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    NomeExibicao = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    FotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leitores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Listas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeitorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listas_Leitores_LeitorId",
                        column: x => x.LeitorId,
                        principalTable: "Leitores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leitores_EmailNormalizado",
                table: "Leitores",
                column: "EmailNormalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leitores_NomeUsuarioNormalizado",
                table: "Leitores",
                column: "NomeUsuarioNormalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listas_LeitorId_Tipo",
                table: "Listas",
                columns: new[] { "LeitorId", "Tipo" },
                unique: true,
                filter: "\"Tipo\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Listas");

            migrationBuilder.DropTable(
                name: "Leitores");
        }
    }
}
