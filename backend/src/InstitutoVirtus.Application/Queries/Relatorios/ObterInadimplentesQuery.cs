using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Application.DTOs.Relatorios;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Relatorios;

public class ObterInadimplentesQuery : IRequest<Result<List<InadimplenciaDto>>>
{
    public int Mes { get; set; }
    public int Ano { get; set; }
}

public class ObterInadimplentesQueryHandler : IRequestHandler<ObterInadimplentesQuery, Result<List<InadimplenciaDto>>>
{
    private readonly IMensalidadeRepository _mensalidadeRepository;
    private readonly IPessoaRepository _pessoaRepository;

    public ObterInadimplentesQueryHandler(
        IMensalidadeRepository mensalidadeRepository,
        IPessoaRepository pessoaRepository)
    {
        _mensalidadeRepository = mensalidadeRepository;
        _pessoaRepository = pessoaRepository;
    }

    public async Task<Result<List<InadimplenciaDto>>> Handle(ObterInadimplentesQuery request, CancellationToken cancellationToken)
    {
        var mensalidadesVencidas = await _mensalidadeRepository.GetVencidasAsync(cancellationToken);

        var inadimplencias = new Dictionary<Guid, InadimplenciaDto>();

        foreach (var mensalidade in mensalidadesVencidas)
        {
            if (mensalidade.Matricula?.Aluno == null)
                continue;

            var responsaveis = mensalidade.Matricula.Aluno.Responsaveis;
            if (!responsaveis.Any())
                continue;

            var responsavelPrincipal = responsaveis.FirstOrDefault(r => r.Principal) ?? responsaveis.First();

            if (!inadimplencias.ContainsKey(responsavelPrincipal.ResponsavelId))
            {
                var responsavel = await _pessoaRepository.GetByIdAsync(responsavelPrincipal.ResponsavelId, cancellationToken);
                if (responsavel == null)
                    continue;

                inadimplencias[responsavelPrincipal.ResponsavelId] = new InadimplenciaDto
                {
                    ResponsavelId = responsavel.Id,
                    ResponsavelNome = responsavel.NomeCompleto,
                    Telefone = responsavel.Telefone.NumeroFormatado(),
                    Email = responsavel.Email?.Endereco ?? string.Empty
                };
            }

            var inadimplencia = inadimplencias[responsavelPrincipal.ResponsavelId];
            inadimplencia.MensalidadesVencidas.Add(new MensalidadeVencidaDto
            {
                AlunoNome = mensalidade.Matricula.Aluno.NomeCompleto,
                Competencia = mensalidade.Competencia.FormatoString(),
                Valor = mensalidade.Valor.Valor,
                DiasAtraso = (DateTime.Today - mensalidade.DataVencimento).Days
            });
            inadimplencia.TotalDevido += mensalidade.Valor.Valor;
        }

        return Result<List<InadimplenciaDto>>.Success(inadimplencias.Values.ToList());
    }
}
