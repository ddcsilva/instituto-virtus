using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoVirtus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ValorMensalidade = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    CargaHoraria = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoPessoa = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Especialidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SaldoCredito = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true, defaultValue: 0m),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResponsavelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MeioPagamento = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenciaExterna = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ComprovanteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Pessoas_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResponsaveisAlunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResponsavelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Parentesco = table.Column<string>(type: "TEXT", nullable: false),
                    Principal = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsaveisAlunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsaveisAlunos_Pessoas_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResponsaveisAlunos_Pessoas_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DiaSemana = table.Column<string>(type: "TEXT", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Capacidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Sala = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AnoLetivo = table.Column<int>(type: "INTEGER", nullable: false),
                    Periodo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turmas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turmas_Pessoas_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aulas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TurmaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataAula = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Conteudo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Realizada = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aulas_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TurmaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Peso = table.Column<decimal>(type: "TEXT", precision: 3, scale: 2, nullable: false, defaultValue: 1m),
                    DataAplicacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TurmaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataMatricula = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DataTrancamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoSaida = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Pessoas_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matriculas_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presencas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AulaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presencas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presencas_Aulas_AulaId",
                        column: x => x.AulaId,
                        principalTable: "Aulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presencas_Pessoas_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AvaliacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 4, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notas_Avaliacoes_AvaliacaoId",
                        column: x => x.AvaliacaoId,
                        principalTable: "Avaliacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notas_Pessoas_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mensalidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatriculaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompetenciaAno = table.Column<int>(type: "INTEGER", nullable: false),
                    CompetenciaMes = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FormaPagamento = table.Column<string>(type: "TEXT", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CriadoPor = table.Column<string>(type: "TEXT", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensalidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensalidades_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosParcelas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PagamentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MensalidadeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ValorAlocado = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosParcelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosParcelas_Mensalidades_MensalidadeId",
                        column: x => x.MensalidadeId,
                        principalTable: "Mensalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagamentosParcelas_Pagamentos_PagamentoId",
                        column: x => x.PagamentoId,
                        principalTable: "Pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aulas_Realizada",
                table: "Aulas",
                column: "Realizada");

            migrationBuilder.CreateIndex(
                name: "IX_Aulas_TurmaId_DataAula",
                table: "Aulas",
                columns: new[] { "TurmaId", "DataAula" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_TurmaId",
                table: "Avaliacoes",
                column: "TurmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Ativo",
                table: "Cursos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Nome",
                table: "Cursos",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_AlunoId_TurmaId",
                table: "Matriculas",
                columns: new[] { "AlunoId", "TurmaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_DataMatricula",
                table: "Matriculas",
                column: "DataMatricula");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_Status",
                table: "Matriculas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_TurmaId",
                table: "Matriculas",
                column: "TurmaId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalidades_CompetenciaAno_CompetenciaMes",
                table: "Mensalidades",
                columns: new[] { "CompetenciaAno", "CompetenciaMes" });

            migrationBuilder.CreateIndex(
                name: "IX_Mensalidades_DataVencimento",
                table: "Mensalidades",
                column: "DataVencimento");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalidades_MatriculaId",
                table: "Mensalidades",
                column: "MatriculaId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalidades_Status",
                table: "Mensalidades",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notas_AlunoId",
                table: "Notas",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notas_AvaliacaoId_AlunoId",
                table: "Notas",
                columns: new[] { "AvaliacaoId", "AlunoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_DataPagamento",
                table: "Pagamentos",
                column: "DataPagamento");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ReferenciaExterna",
                table: "Pagamentos",
                column: "ReferenciaExterna",
                unique: true,
                filter: "ReferenciaExterna IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ResponsavelId",
                table: "Pagamentos",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosParcelas_MensalidadeId",
                table: "PagamentosParcelas",
                column: "MensalidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosParcelas_PagamentoId_MensalidadeId",
                table: "PagamentosParcelas",
                columns: new[] { "PagamentoId", "MensalidadeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Ativo",
                table: "Pessoas",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Email",
                table: "Pessoas",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Telefone",
                table: "Pessoas",
                column: "Telefone");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_TipoPessoa",
                table: "Pessoas",
                column: "TipoPessoa");

            migrationBuilder.CreateIndex(
                name: "IX_Presencas_AlunoId",
                table: "Presencas",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Presencas_AulaId_AlunoId",
                table: "Presencas",
                columns: new[] { "AulaId", "AlunoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Presencas_Status",
                table: "Presencas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsaveisAlunos_AlunoId",
                table: "ResponsaveisAlunos",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsaveisAlunos_Principal",
                table: "ResponsaveisAlunos",
                column: "Principal");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsaveisAlunos_ResponsavelId_AlunoId",
                table: "ResponsaveisAlunos",
                columns: new[] { "ResponsavelId", "AlunoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_Ativo",
                table: "Turmas",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_CursoId",
                table: "Turmas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_ProfessorId_DiaSemana_AnoLetivo_Periodo",
                table: "Turmas",
                columns: new[] { "ProfessorId", "DiaSemana", "AnoLetivo", "Periodo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notas");

            migrationBuilder.DropTable(
                name: "PagamentosParcelas");

            migrationBuilder.DropTable(
                name: "Presencas");

            migrationBuilder.DropTable(
                name: "ResponsaveisAlunos");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "Mensalidades");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Aulas");

            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Turmas");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Pessoas");
        }
    }
}
