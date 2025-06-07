using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CPF = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PessoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponsavelId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataMatricula = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alunos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alunos_Pessoas_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PagadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NumeroTransacao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FormaPagamento = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Pessoas_PagadorId",
                        column: x => x.PagadorId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Professores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PessoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Especialidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professores_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosAlunos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlunoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorProporcional = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosAlunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosAlunos_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagamentosAlunos_Pagamentos_PagamentoId",
                        column: x => x.PagamentoId,
                        principalTable: "Pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Capacidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Horario = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DiaSemana = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turmas_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlunoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurmaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataMatricula = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataCancelamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoCancelamento = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NumeroOrdemEspera = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matriculas_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_PessoaId",
                table: "Alunos",
                column: "PessoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_ResponsavelId",
                table: "Alunos",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_Status",
                table: "Alunos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_AlunoId_TurmaId_Status",
                table: "Matriculas",
                columns: new[] { "AlunoId", "TurmaId", "Status" },
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_Status",
                table: "Matriculas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_TurmaId",
                table: "Matriculas",
                column: "TurmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_TurmaId_NumeroOrdemEspera",
                table: "Matriculas",
                columns: new[] { "TurmaId", "NumeroOrdemEspera" },
                filter: "[NumeroOrdemEspera] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_DataVencimento",
                table: "Pagamentos",
                column: "DataVencimento");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_PagadorId",
                table: "Pagamentos",
                column: "PagadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_Status",
                table: "Pagamentos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_Status_DataVencimento",
                table: "Pagamentos",
                columns: new[] { "Status", "DataVencimento" });

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosAlunos_AlunoId",
                table: "PagamentosAlunos",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosAlunos_PagamentoId_AlunoId",
                table: "PagamentosAlunos",
                columns: new[] { "PagamentoId", "AlunoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_CPF",
                table: "Pessoas",
                column: "CPF",
                unique: true,
                filter: "[CPF] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Email",
                table: "Pessoas",
                column: "Email",
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Tipo",
                table: "Pessoas",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Professores_Ativo",
                table: "Professores",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Professores_Especialidade",
                table: "Professores",
                column: "Especialidade");

            migrationBuilder.CreateIndex(
                name: "IX_Professores_PessoaId",
                table: "Professores",
                column: "PessoaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_Ativa",
                table: "Turmas",
                column: "Ativa");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_DiaSemana_Horario",
                table: "Turmas",
                columns: new[] { "DiaSemana", "Horario" });

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_ProfessorId",
                table: "Turmas",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_Tipo",
                table: "Turmas",
                column: "Tipo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "PagamentosAlunos");

            migrationBuilder.DropTable(
                name: "Turmas");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Professores");

            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
