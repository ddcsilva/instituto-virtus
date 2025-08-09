using InstitutoVirtus.Domain.Common;

namespace InstitutoVirtus.Domain.Entities;

public class Curso : AuditableEntity
{
    private readonly List<Turma> _turmas = new();

    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public decimal ValorMensalidade { get; private set; }
    public int CargaHoraria { get; private set; }
    public bool Ativo { get; private set; }

    public IReadOnlyCollection<Turma> Turmas => _turmas;

    protected Curso() { }

    public Curso(string nome, string? descricao, decimal valorMensalidade, int cargaHoraria = 0)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do curso é obrigatório");

        if (valorMensalidade < 0)
            throw new ArgumentException("Valor da mensalidade não pode ser negativo");

        Nome = nome;
        Descricao = descricao;
        ValorMensalidade = valorMensalidade;
        CargaHoraria = cargaHoraria;
        Ativo = true;
    }

    public void AtualizarDados(string nome, string? descricao, decimal valorMensalidade, int cargaHoraria)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do curso é obrigatório");

        Nome = nome;
        Descricao = descricao;
        ValorMensalidade = valorMensalidade;
        CargaHoraria = cargaHoraria;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
