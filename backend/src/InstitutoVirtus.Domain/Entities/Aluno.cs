using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;

namespace InstitutoVirtus.Domain.Entities;

public class Aluno : Pessoa
{
    private readonly List<ResponsavelAluno> _responsaveis = new();
    private readonly List<Matricula> _matriculas = new();
    private readonly List<Presenca> _presencas = new();
    private readonly List<Nota> _notas = new();

    public IReadOnlyCollection<ResponsavelAluno> Responsaveis => _responsaveis;
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas;
    public IReadOnlyCollection<Presenca> Presencas => _presencas;
    public IReadOnlyCollection<Nota> Notas => _notas;

    protected Aluno() { }

    public Aluno(
        string nomeCompleto,
        Cpf? cpf,
        Telefone telefone,
        Email? email,
        DateTime dataNascimento,
        string? observacoes = null)
        : base(nomeCompleto, cpf, telefone, email, dataNascimento, TipoPessoa.Aluno, observacoes)
    {
        if (EhMenorDeIdade() && email == null)
        {
            // Email é opcional para menores de 18 anos
        }
    }

    public void AdicionarResponsavel(Responsavel responsavel, Parentesco parentesco, bool principal = false)
    {
        if (!EhMenorDeIdade())
            throw new BusinessRuleValidationException("Aluno maior de idade não precisa de responsável");

        if (_responsaveis.Any(r => r.ResponsavelId == responsavel.Id))
            throw new BusinessRuleValidationException("Responsável já vinculado ao aluno");

        if (principal)
        {
            foreach (var r in _responsaveis)
            {
                if (r.Principal)
                {
                    r.DefinirComoNaoPrincipal();
                }
            }
        }

        var vinculo = new ResponsavelAluno(responsavel.Id, this.Id, parentesco, principal);
        _responsaveis.Add(vinculo);
    }

    public void RemoverResponsavel(Guid responsavelId)
    {
        var vinculo = _responsaveis.FirstOrDefault(r => r.ResponsavelId == responsavelId);
        if (vinculo != null)
            _responsaveis.Remove(vinculo);
    }

    public bool TemMatriculaAtiva()
    {
        return _matriculas.Any(m => m.Status == StatusMatricula.Ativa);
    }
}