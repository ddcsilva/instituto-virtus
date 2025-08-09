using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.ValueObjects;

namespace InstitutoVirtus.Domain.Entities;

public class Professor : Pessoa
{
    private readonly List<Turma> _turmas = new();

    public IReadOnlyCollection<Turma> Turmas => _turmas;
    public string? Especialidade { get; private set; }

    protected Professor() { }

    public Professor(
        string nomeCompleto,
        Telefone telefone,
        Email email,
        DateTime dataNascimento,
        string? especialidade = null,
        string? observacoes = null)
        : base(nomeCompleto, telefone, email, dataNascimento, TipoPessoa.Professor, observacoes)
    {
        Especialidade = especialidade;
    }

    public void AtualizarEspecialidade(string especialidade)
    {
        Especialidade = especialidade;
    }

    public bool EstaDisponivelNoHorario(DiaSemana dia, TimeSpan horaInicio)
    {
        return !_turmas.Any(t =>
            t.Ativo &&
            t.DiaSemana == dia &&
            t.Horario.HoraInicio == horaInicio);
    }
}
