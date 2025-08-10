using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Relatorios;

public class ObterRelatorioFinanceiroQuery : IRequest<Result<RelatorioFinanceiroDto>>
{
    public int Mes { get; set; }
    public int Ano { get; set; }
}

public class RelatorioFinanceiroDto
{
    public string Competencia { get; set; } = string.Empty;
    public decimal ReceitaPrevista { get; set; }
    public decimal ReceitaRealizada { get; set; }
    public decimal TotalEmAberto { get; set; }
    public decimal TotalVencido { get; set; }
    public int TotalMensalidades { get; set; }
    public int MensalidadesPagas { get; set; }
    public int MensalidadesEmAberto { get; set; }
    public int MensalidadesVencidas { get; set; }
    public double PercentualAdimplencia { get; set; }
}

public class ObterRelatorioFinanceiroQueryHandler : IRequestHandler<ObterRelatorioFinanceiroQuery, Result<RelatorioFinanceiroDto>>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;

    public ObterRelatorioFinanceiroQueryHandler(IMensalidadeRepository mensalidadeRepository)
    {
        _mensalidadeRepository = mensalidadeRepository;
    }

    public async Task<Result<RelatorioFinanceiroDto>> Handle(ObterRelatorioFinanceiroQuery request, CancellationToken cancellationToken)
    {
        var mensalidades = await _mensalidadeRepository.GetByCompetenciaAsync(request.Ano, request.Mes, cancellationToken);

        var relatorio = new RelatorioFinanceiroDto
        {
            Competencia = $"{request.Mes:00}/{request.Ano}",
            TotalMensalidades = mensalidades.Count(),
            MensalidadesPagas = mensalidades.Count(m => m.Status == StatusMensalidade.Pago),
            MensalidadesEmAberto = mensalidades.Count(m => m.Status == StatusMensalidade.EmAberto),
            MensalidadesVencidas = mensalidades.Count(m => m.Status == StatusMensalidade.Vencido),
            ReceitaPrevista = mensalidades.Sum(m => m.Valor.Valor),
            ReceitaRealizada = mensalidades.Where(m => m.Status == StatusMensalidade.Pago).Sum(m => m.Valor.Valor),
            TotalEmAberto = mensalidades.Where(m => m.Status == StatusMensalidade.EmAberto).Sum(m => m.Valor.Valor),
            TotalVencido = mensalidades.Where(m => m.Status == StatusMensalidade.Vencido).Sum(m => m.Valor.Valor)
        };

        relatorio.PercentualAdimplencia = relatorio.TotalMensalidades > 0
            ? (double)relatorio.MensalidadesPagas / relatorio.TotalMensalidades * 100
            : 100;

        return Result<RelatorioFinanceiroDto>.Success(relatorio);
    }
}