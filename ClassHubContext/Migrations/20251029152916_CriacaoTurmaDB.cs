using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassHub.ClassHubContext.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoTurmaDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    DtInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DtFim = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdProfessor = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turmas_Usuarios_IdProfessor",
                        column: x => x.IdProfessor,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlunoTurmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdAluno = table.Column<int>(type: "INTEGER", nullable: false),
                    IdTurma = table.Column<int>(type: "INTEGER", nullable: false),
                    DtMatricula = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlunoTurmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlunoTurmas_Turmas_IdTurma",
                        column: x => x.IdTurma,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlunoTurmas_Usuarios_IdAluno",
                        column: x => x.IdAluno,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlunoTurmas_IdAluno_IdTurma",
                table: "AlunoTurmas",
                columns: new[] { "IdAluno", "IdTurma" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlunoTurmas_IdTurma",
                table: "AlunoTurmas",
                column: "IdTurma");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_IdProfessor",
                table: "Turmas",
                column: "IdProfessor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlunoTurmas");

            migrationBuilder.DropTable(
                name: "Turmas");
        }
    }
}
