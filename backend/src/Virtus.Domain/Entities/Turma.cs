using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

public class Turma
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public int Capacidade { get; set; }
    public TipoCurso Tipo { get; set; }
    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = default!;
    public bool Ativa { get; set; } = true;
    public DateTime DataCriacao { get; set; }

    private readonly List<Matricula> _matriculas = new();
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    protected Turma() { }

    public Turma(string nome, int capacidade, TipoCurso tipo, Professor professor)
    {
        ValidarNome(nome);
        ValidarCapacidade(capacidade);

        Nome = nome;
        Capacidade = capacidade;
        Tipo = tipo;
        Professor = professor ?? throw new ArgumentNullException(nameof(professor));
        ProfessorId = professor.Id;
        Ativa = true;
        DataCriacao = DateTime.UtcNow;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome da turma é obrigatório");
        }

        if (nome.Length < 3)
        {
            throw new ArgumentException("Nome da turma deve ter pelo menos 3 caracteres");
        }
    }

    private static void ValidarCapacidade(int capacidade)
    {
        if (capacidade < 1)
        {
            throw new ArgumentException("Capacidade deve ser maior que zero");
        }

        if (capacidade > 50)
        {
            throw new ArgumentException("Capacidade não pode ser maior que 50");
        }
    }

    public bool TemVagasDisponiveis()
    {
        var matriculasAtivas = _matriculas.Count(m => m.Status == Enums.StatusMatricula.Ativa);
        return matriculasAtivas < Capacidade;
    }

    public int ObterQuantidadeVagasDisponiveis()
    {
        var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
        return Capacidade - matriculasAtivas;
    }

    public void Ativar()
    {
        Ativa = true;
    }

    public void Inativar()
    {
        Ativa = false;
    }
}
