using System.Text;
using InstitutoVirtus.Application.Common;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using MediatR;

namespace InstitutoVirtus.Application.Queries.Relatorios;

public class ExportarAlunosQuery : IRequest<Result<byte[]>>
{
    public string Formato { get; set; } = "excel";
    public bool? Ativos { get; set; }
}

public class ExportarAlunosQueryHandler : IRequestHandler<ExportarAlunosQuery, Result<byte[]>>
{
    private readonly IPessoaRepository _pessoaRepository;

    public ExportarAlunosQueryHandler(IPessoaRepository pessoaRepository)
    {
        _pessoaRepository = pessoaRepository;
    }

    public async Task<Result<byte[]>> Handle(ExportarAlunosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var alunos = await _pessoaRepository.GetByTipoAsync(TipoPessoa.Aluno, cancellationToken);

            if (request.Ativos.HasValue)
            {
                alunos = alunos.Where(a => a.Ativo == request.Ativos.Value);
            }

            byte[] fileContent;

            if (request.Formato.ToLower() == "csv")
            {
                fileContent = GenerateCsv(alunos);
            }
            else
            {
                fileContent = GenerateExcel(alunos);
            }

            return Result<byte[]>.Success(fileContent);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failure($"Erro ao exportar alunos: {ex.Message}");
        }
    }

    private byte[] GenerateCsv(IEnumerable<Pessoa> alunos)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Nome,Telefone,Email,Data Nascimento,Idade,Status");

        foreach (var aluno in alunos)
        {
            csv.AppendLine($"{aluno.NomeCompleto},{aluno.Telefone.NumeroFormatado()},{aluno.Email?.Endereco ?? ""},{aluno.DataNascimento:dd/MM/yyyy},{aluno.CalcularIdade()},{(aluno.Ativo ? "Ativo" : "Inativo")}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private byte[] GenerateExcel(IEnumerable<Pessoa> alunos)
    {
        // TODO: Implementar geração de Excel usando biblioteca como ClosedXML ou EPPlus
        // Por enquanto, retorna CSV como fallback
        return GenerateCsv(alunos);
    }
}