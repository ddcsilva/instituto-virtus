using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Exceptions;

namespace InstitutoVirtus.Domain.Entities;

public class Aluno : Pessoa
{
    private readonly List<ResponsavelAluno> _responsaveis = [];
    private readonly List<Matricula> _matriculas = [];
    private readonly List<Presenca> _presencas = [];
    private readonly List<Nota> _notas = [];

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
            foreach (ResponsavelAluno r in _responsaveis)
            {
                if (r.Principal)
                {
                    r.DefinirComoNaoPrincipal();
                }
            }
        }

        ResponsavelAluno vinculo = new(responsavel.Id, this.Id, parentesco, principal);
        _responsaveis.Add(vinculo);
    }

    public void RemoverResponsavel(Guid responsavelId)
    {
        ResponsavelAluno? vinculo = _responsaveis.FirstOrDefault(r => r.ResponsavelId == responsavelId);
        if (vinculo != null)
            _responsaveis.Remove(vinculo);
    }

    public bool TemMatriculaAtiva()
    {
        return _matriculas.Any(m => m.Status == StatusMatricula.Ativa);
    }
}