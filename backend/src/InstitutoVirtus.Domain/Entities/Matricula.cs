using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;
using InstitutoVirtus.Domain.ValueObjects;

namespace InstitutoVirtus.Domain.Entities;

public class Matricula : AuditableEntity
{
    private readonly List<Mensalidade> _mensalidades = new();

    public Guid AlunoId { get; private set; }
    public Guid TurmaId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public StatusMatricula Status { get; private set; }
    public DateTime? DataTrancamento { get; private set; }
    public DateTime? DataConclusao { get; private set; }
    public string? MotivoSaida { get; private set; }

    public virtual Aluno? Aluno { get; private set; }
    public virtual Turma? Turma { get; private set; }
    public IReadOnlyCollection<Mensalidade> Mensalidades => _mensalidades;

    protected Matricula() { }

    public Matricula(Guid alunoId, Guid turmaId)
    {
        AlunoId = alunoId;
        TurmaId = turmaId;
        DataMatricula = DateTime.UtcNow;
        Status = StatusMatricula.Ativa;
    }

    public void Trancar(string motivo)
    {
        if (Status != StatusMatricula.Ativa)
            throw new BusinessRuleValidationException("Apenas matrículas ativas podem ser trancadas");

        Status = StatusMatricula.Trancada;
        DataTrancamento = DateTime.UtcNow;
        MotivoSaida = motivo;
    }

    public void Reativar()
    {
        if (Status != StatusMatricula.Trancada)
            throw new BusinessRuleValidationException("Apenas matrículas trancadas podem ser reativadas");

        Status = StatusMatricula.Ativa;
        DataTrancamento = null;
        MotivoSaida = null;
    }

    public void Concluir()
    {
        if (Status != StatusMatricula.Ativa)
            throw new BusinessRuleValidationException("Apenas matrículas ativas podem ser concluídas");

        Status = StatusMatricula.Concluida;
        DataConclusao = DateTime.UtcNow;
    }

    public void Cancelar(string motivo)
    {
        Status = StatusMatricula.Cancelada;
        MotivoSaida = motivo;
    }

    public void GerarMensalidades(int mesesQuantidade, decimal valorMensal, int diaVencimento = 10)
    {
        var dataBase = DateTime.Today;

        for (int i = 0; i < mesesQuantidade; i++)
        {
            var competencia = new Competencia(dataBase.Year, dataBase.Month);
            var vencimento = new DateTime(dataBase.Year, dataBase.Month, diaVencimento);

            var mensalidade = new Mensalidade(
                this.Id,
                competencia,
                new Dinheiro(valorMensal),
                vencimento);

            _mensalidades.Add(mensalidade);

            dataBase = dataBase.AddMonths(1);
        }
    }
}