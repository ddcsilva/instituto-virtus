using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Entities;

public class ResponsavelAluno : BaseEntity
{
    public Guid ResponsavelId { get; private set; }
    public Guid AlunoId { get; private set; }
    public Parentesco Parentesco { get; private set; }
    public bool Principal { get; private set; }

    public virtual Responsavel? Responsavel { get; private set; }
    public virtual Aluno? Aluno { get; private set; }

    protected ResponsavelAluno() { }

    public ResponsavelAluno(Guid responsavelId, Guid alunoId, Parentesco parentesco, bool principal = false)
    {
        ResponsavelId = responsavelId;
        AlunoId = alunoId;
        Parentesco = parentesco;
        Principal = principal;
    }

    public void DefinirComoPrincipal()
    {
        Principal = true;
    }
}