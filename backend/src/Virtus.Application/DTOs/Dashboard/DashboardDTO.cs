using System;

namespace Virtus.Application.DTOs.Dashboard;

public class DashboardDTO
{
    public int TotalAlunos { get; set; }
    public int AlunosAtivos { get; set; }
    public int AlunosListaEspera { get; set; }
    public int TotalTurmas { get; set; }
    public int TurmasAtivas { get; set; }
    public int TotalVagasDisponiveis { get; set; }
    public decimal ValorPagamentosUltimos30Dias { get; set; }
    public int QuantidadePagamentosUltimos30Dias { get; set; }
}
