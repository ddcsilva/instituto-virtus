namespace Virtus.Domain.Interfaces;

public interface IUnitOfWork
{
    public IAlunoRepository Alunos { get; }
    public ITurmaRepository Turmas { get; }
    public IPessoaRepository Pessoas { get; }
    public IMatriculaRepository Matriculas { get; }
    public IPagamentoRepository Pagamentos { get; }

    public Task<int> SalvarAsync();
    public Task IniciarTransacaoAsync();
    public Task ConfirmarTransacaoAsync();
    public Task ReverterTransacaoAsync();
}
