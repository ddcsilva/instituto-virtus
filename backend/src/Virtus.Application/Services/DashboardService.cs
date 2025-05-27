using Virtus.Application.DTOs.Dashboard;
using Virtus.Application.Interfaces;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;

namespace Virtus.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardDTO> ObterDadosDashboardAsync()
    {
        var dashboard = new DashboardDTO();

        // Dados de alunos
        var alunos = await _unitOfWork.Alunos.ObterTodosAsync();
        dashboard.TotalAlunos = alunos.Count();
        dashboard.AlunosAtivos = alunos.Count(a => a.Status == StatusAluno.Ativo);
        dashboard.AlunosListaEspera = alunos.Count(a => a.Status == StatusAluno.ListaEspera);

        // Dados de turmas
        var turmas = await _unitOfWork.Turmas.ObterTodosAsync();
        dashboard.TotalTurmas = turmas.Count();
        dashboard.TurmasAtivas = turmas.Count(t => t.Ativa);

        // Calcular vagas disponíveis
        var totalVagas = 0;
        foreach (var turma in turmas.Where(t => t.Ativa))
        {
            var turmaCompleta = await _unitOfWork.Turmas.ObterComMatriculasAsync(turma.Id);
            totalVagas += turmaCompleta!.ObterQuantidadeVagasDisponiveis();
        }
        dashboard.TotalVagasDisponiveis = totalVagas;

        // Dados financeiros
        var dataInicial = DateTime.UtcNow.AddDays(-30);
        var pagamentos = await _unitOfWork.Pagamentos.ObterPorPeriodoAsync(dataInicial, DateTime.UtcNow);
        dashboard.QuantidadePagamentosUltimos30Dias = pagamentos.Count();
        dashboard.ValorPagamentosUltimos30Dias = pagamentos.Sum(p => p.Valor);

        return dashboard;
    }
}
