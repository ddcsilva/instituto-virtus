using InstitutoVirtus.Domain.Common;

namespace InstitutoVirtus.Domain.Entities;

public class Nota : AuditableEntity
{
    public Guid AvaliacaoId { get; private set; }
    public Guid AlunoId { get; private set; }
    public decimal Valor { get; private set; }
    public string? Observacoes { get; private set; }

    public virtual Avaliacao? Avaliacao { get; private set; }
    public virtual Aluno? Aluno { get; private set; }

    protected Nota() { }

    public Nota(Guid avaliacaoId, Guid alunoId, decimal valor, string? observacoes = null)
    {
        if (valor < 0 || valor > 10)
            throw new ArgumentException("Nota deve estar entre 0 e 10");

        AvaliacaoId = avaliacaoId;
        AlunoId = alunoId;
        Valor = valor;
        Observacoes = observacoes;
    }

    public void AtualizarValor(decimal valor)
    {
        if (valor < 0 || valor > 10)
            throw new ArgumentException("Nota deve estar entre 0 e 10");

        Valor = valor;
    }
}