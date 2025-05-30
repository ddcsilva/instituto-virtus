using Virtus.Domain.Enums;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa uma turma.
/// </summary>
public class Turma : BaseEntity
{
    public string Nome { get; private set; } = default!;
    public int Capacidade { get; private set; }
    public TipoCurso Tipo { get; private set; }
    public int ProfessorId { get; private set; }
    public Professor Professor { get; private set; } = default!;
    public bool Ativa { get; private set; } = true;

    private readonly List<Matricula> _matriculas = [];
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();

    private Turma() { }

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
    }

    /// <summary>
    /// Valida o nome da turma.
    /// </summary>
    /// <param name="nome">O nome da turma.</param>
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

    /// <summary>
    /// Valida a capacidade da turma.
    /// </summary>
    /// <param name="capacidade">A capacidade da turma.</param>
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

    /// <summary>
    /// Verifica se a turma tem vagas disponíveis.
    /// </summary>
    /// <returns>true se a turma tem vagas disponíveis, false caso contrário.</returns>
    public bool TemVagasDisponiveis()
    {
        var matriculasAtivas = _matriculas.Count(m => m.Status == Enums.StatusMatricula.Ativa);
        return matriculasAtivas < Capacidade;
    }

    /// <summary>
    /// Obtém a quantidade de vagas disponíveis na turma.
    /// </summary>
    /// <returns>A quantidade de vagas disponíveis.</returns>
    public int ObterQuantidadeVagasDisponiveis()
    {
        var matriculasAtivas = _matriculas.Count(m => m.Status == StatusMatricula.Ativa);
        return Capacidade - matriculasAtivas;
    }

    /// <summary>
    /// Ativa a turma.
    /// </summary>
    public void Ativar()
    {
        Ativa = true;
        AtualizarData();
    }

    /// <summary>
    /// Inativa a turma.
    /// </summary>
    public void Inativar()
    {
        Ativa = false;
        AtualizarData();
    }

    /// <summary>
    /// Atualiza os dados da turma.
    /// </summary>
    /// <param name="nome">O novo nome da turma.</param>
    /// <param name="capacidade">A nova capacidade da turma.</param>
    /// <param name="professor">O novo professor da turma.</param>
    public void AtualizarDados(string nome, int capacidade, Professor professor)
    {
        ValidarNome(nome);
        ValidarCapacidade(capacidade);

        Nome = nome;
        Capacidade = capacidade;
        Professor = professor ?? throw new ArgumentNullException(nameof(professor));
        ProfessorId = professor.Id;
        AtualizarData();
    }
}
