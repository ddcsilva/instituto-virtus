using InstitutoVirtus.Domain.Common;

namespace InstitutoVirtus.Domain.Entities;

public class Avaliacao : AuditableEntity
{
    private readonly List<Nota> _notas = new();

    public Guid TurmaId { get; private set; }
    public string Nome { get; private set; }
    public decimal Peso { get; private set; }
    public DateTime? DataAplicacao { get; private set; }
    public string? Descricao { get; private set; }

    public virtual Turma? Turma { get; private set; }
    public IReadOnlyCollection<Nota> Notas => _notas;

#pragma warning disable CS8618
    protected Avaliacao() { }
#pragma warning restore CS8618

    public Avaliacao(Guid turmaId, string nome, decimal peso = 1, DateTime? dataAplicacao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da avaliação é obrigatório");

        if (peso <= 0)
            throw new ArgumentException("Peso deve ser maior que zero");

        TurmaId = turmaId;
        Nome = nome;
        Peso = peso;
        DataAplicacao = dataAplicacao;
    }

    public void LancarNota(Guid alunoId, decimal valor)
    {
        if (valor < 0 || valor > 10)
            throw new ArgumentException("Nota deve estar entre 0 e 10");

        var notaExistente = _notas.FirstOrDefault(n => n.AlunoId == alunoId);

        if (notaExistente != null)
        {
            notaExistente.AtualizarValor(valor);
        }
        else
        {
            var nota = new Nota(this.Id, alunoId, valor);
            _notas.Add(nota);
        }
    }
}