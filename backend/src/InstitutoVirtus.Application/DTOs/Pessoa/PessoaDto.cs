using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Application.DTOs.Turma;

namespace InstitutoVirtus.Application.DTOs.Pessoa;

public class PessoaDto
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime DataNascimento { get; set; }
    public string TipoPessoa { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; }
    public int Idade { get; set; }
}

public class AlunoDto : PessoaDto
{
    public List<ResponsavelResumoDto> Responsaveis { get; set; } = new();
    public List<MatriculaResumoDto> Matriculas { get; set; } = new();
}

public class ProfessorDto : PessoaDto
{
    public string? Especialidade { get; set; }
    public List<TurmaResumoDto> Turmas { get; set; } = new();
}

public class ResponsavelDto : PessoaDto
{
    public List<AlunoResumoDto> Alunos { get; set; } = new();
    public decimal SaldoCredito { get; set; }
}

public class ResponsavelResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Parentesco { get; set; } = string.Empty;
}

public class AlunoResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}
