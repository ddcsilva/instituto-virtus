namespace Virtus.Application.DTOs.Common;

public class ResultadoDTO<T>
{
    public bool Sucesso { get; set; }
    public T? Dados { get; set; }
    public List<string> Erros { get; set; } = [];
}

public static class ResultadoDTO
{
    public static ResultadoDTO<T> ComSucesso<T>(T dados)
    {
        return new ResultadoDTO<T>
        {
            Sucesso = true,
            Dados = dados
        };
    }

    public static ResultadoDTO<T> ComErro<T>(string erro)
    {
        return new ResultadoDTO<T>
        {
            Sucesso = false,
            Erros = new List<string> { erro }
        };
    }

    public static ResultadoDTO<T> ComErros<T>(List<string> erros)
    {
        return new ResultadoDTO<T>
        {
            Sucesso = false,
            Erros = erros
        };
    }
}
