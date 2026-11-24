using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readex.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaObras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Autor = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Genero = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    CapaUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Destaque = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obras");
        }
    }
}
