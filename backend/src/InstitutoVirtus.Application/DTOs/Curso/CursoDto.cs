namespace InstitutoVirtus.Application.DTOs.Curso;

public class CursoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensalidade { get; set; }
    public int CargaHoraria { get; set; }
    public bool Ativo { get; set; }
    public int TotalTurmas { get; set; }
}

public class CursoResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorMensalidade { get; set; }
}