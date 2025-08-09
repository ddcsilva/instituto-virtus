namespace InstitutoVirtus.Application.DTOs.Relatorios;

public class InadimplenciaDto
{
    public Guid ResponsavelId { get; set; }
    public string ResponsavelNome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<MensalidadeVencidaDto> MensalidadesVencidas { get; set; } = new();
    public decimal TotalDevido { get; set; }
}

public class MensalidadeVencidaDto
{
    public string AlunoNome { get; set; } = string.Empty;
    public string Competencia { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DiasAtraso { get; set; }
}

public class BoletimDto
{
    public Guid AlunoId { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public Guid TurmaId { get; set; }
    public string TurmaNome { get; set; } = string.Empty;
    public List<NotaBoletimDto> Notas { get; set; } = new();
    public decimal MediaFinal { get; set; }
    public double Frequencia { get; set; }
    public bool Aprovado { get; set; }
    public string Situacao { get; set; } = string.Empty;
}

public class NotaBoletimDto
{
    public string Avaliacao { get; set; } = string.Empty;
    public decimal Nota { get; set; }
    public decimal Peso { get; set; }
}