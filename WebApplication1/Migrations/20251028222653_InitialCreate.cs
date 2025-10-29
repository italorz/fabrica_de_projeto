using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "condutor",
                columns: table => new
                {
                    id_condutor = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    cpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    cpf_uf = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    cnh = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    categoria_cnh = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_condutor", x => x.id_condutor);
                });

            migrationBuilder.CreateTable(
                name: "tipo_multa",
                columns: table => new
                {
                    id_tipo_multa = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    codigo_multa = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    valor_multa = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    gravidade = table.Column<string>(type: "TEXT", nullable: false),
                    pontos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_multa", x => x.id_tipo_multa);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    cpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "TEXT", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculos",
                columns: table => new
                {
                    id_veiculo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    placa = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    marca = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    modelo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    proprietario = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_veiculos", x => x.id_veiculo);
                });

            migrationBuilder.CreateTable(
                name: "multas",
                columns: table => new
                {
                    id_multas = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_veiculo = table.Column<int>(type: "INTEGER", nullable: false),
                    id_usuario = table.Column<int>(type: "INTEGER", nullable: false),
                    condutor_id = table.Column<int>(type: "INTEGER", nullable: true),
                    data_hora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    endereco = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    descricao = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    id_tipo_multa = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_multas", x => x.id_multas);
                    table.ForeignKey(
                        name: "FK_multas_condutor_condutor_id",
                        column: x => x.condutor_id,
                        principalTable: "condutor",
                        principalColumn: "id_condutor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_multas_tipo_multa_id_tipo_multa",
                        column: x => x.id_tipo_multa,
                        principalTable: "tipo_multa",
                        principalColumn: "id_tipo_multa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_multas_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_multas_veiculos_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculos",
                        principalColumn: "id_veiculo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_multas_condutor_id",
                table: "multas",
                column: "condutor_id");

            migrationBuilder.CreateIndex(
                name: "IX_multas_id_tipo_multa",
                table: "multas",
                column: "id_tipo_multa");

            migrationBuilder.CreateIndex(
                name: "IX_multas_id_usuario",
                table: "multas",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_multas_id_veiculo",
                table: "multas",
                column: "id_veiculo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "multas");

            migrationBuilder.DropTable(
                name: "condutor");

            migrationBuilder.DropTable(
                name: "tipo_multa");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "veiculos");
        }
    }
}
