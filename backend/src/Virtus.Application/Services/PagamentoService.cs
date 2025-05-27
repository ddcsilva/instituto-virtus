using AutoMapper;
using Virtus.Application.DTOs.Pagamento;
using Virtus.Application.DTOs.Common;
using Virtus.Application.Interfaces;
using Virtus.Domain.Interfaces;
using Virtus.Domain.Entities;

namespace Virtus.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PagamentoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultadoDTO<PagamentoDTO>> ObterPorIdAsync(int id)
    {
        var pagamento = await _unitOfWork.Pagamentos.ObterComAlunosAsync(id);

        if (pagamento == null)
        {
            return ResultadoDTO.ComErro<PagamentoDTO>("Pagamento não encontrado");
        }
        var dto = _mapper.Map<PagamentoDTO>(pagamento);
        dto.EstaBalanceado = pagamento.EstaBalanceado();

        return ResultadoDTO.ComSucesso<PagamentoDTO>(dto);
    }

    public async Task<ResultadoDTO<PagamentoDTO>> CriarAsync(CriarPagamentoDTO dto)
    {
        var pagador = await _unitOfWork.Pessoas.ObterPorIdAsync(dto.PagadorId);

        if (pagador == null)
        {
            return ResultadoDTO.ComErro<PagamentoDTO>("Pagador não encontrado");
        }

        var totalAlunos = dto.Alunos.Sum(a => a.Valor);

        if (Math.Abs(totalAlunos - dto.Valor) > 0.01m)
        {
            return ResultadoDTO.ComErro<PagamentoDTO>("Soma dos valores por aluno não corresponde ao valor total do pagamento");
        }

        var pagamento = new Pagamento(dto.Valor, dto.DataPagamento, pagador, dto.Observacao);

        foreach (var alunoDto in dto.Alunos)
        {
            var aluno = await _unitOfWork.Alunos.ObterPorIdAsync(alunoDto.AlunoId);

            if (aluno == null)
            {
                return ResultadoDTO.ComErro<PagamentoDTO>($"Aluno com ID {alunoDto.AlunoId} não encontrado");
            }

            pagamento.AdicionarAluno(aluno, alunoDto.Valor);
        }

        await _unitOfWork.Pagamentos.AdicionarAsync(pagamento);
        await _unitOfWork.SalvarAsync();

        var pagamentoSalvo = await _unitOfWork.Pagamentos.ObterComAlunosAsync(pagamento.Id);
        var resultado = _mapper.Map<PagamentoDTO>(pagamentoSalvo);
        resultado.EstaBalanceado = pagamentoSalvo!.EstaBalanceado();

        return ResultadoDTO.ComSucesso<PagamentoDTO>(resultado);
    }

    public async Task<RespostaPaginadaDTO<PagamentoDTO>> ObterPorPeriodoAsync(
        DateTime dataInicial,
        DateTime dataFinal,
        int pagina = 1,
        int itensPorPagina = 20)
    {
        var pagamentos = await _unitOfWork.Pagamentos.ObterPorPeriodoAsync(dataInicial, dataFinal);
        var totalItens = pagamentos.Count();

        var pagamentosPaginados = pagamentos
            .OrderByDescending(p => p.DataPagamento)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToList();

        var dtos = new List<PagamentoDTO>();

        foreach (var pagamento in pagamentosPaginados)
        {
            var pagamentoCompleto = await _unitOfWork.Pagamentos.ObterComAlunosAsync(pagamento.Id);
            var dto = _mapper.Map<PagamentoDTO>(pagamentoCompleto);
            dto.EstaBalanceado = pagamentoCompleto!.EstaBalanceado();
            dtos.Add(dto);
        }

        return new RespostaPaginadaDTO<PagamentoDTO>
        {
            Itens = dtos,
            TotalItens = totalItens,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)itensPorPagina)
        };
    }

    public async Task<IEnumerable<PagamentoDTO>> ObterPorAlunoAsync(int alunoId)
    {
        var pagamentos = await _unitOfWork.Pagamentos.ObterPorAlunoAsync(alunoId);
        var dtos = new List<PagamentoDTO>();

        foreach (var pagamento in pagamentos)
        {
            var pagamentoCompleto = await _unitOfWork.Pagamentos.ObterComAlunosAsync(pagamento.Id);
            var dto = _mapper.Map<PagamentoDTO>(pagamentoCompleto);
            dto.EstaBalanceado = pagamentoCompleto!.EstaBalanceado();
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<IEnumerable<PagamentoDTO>> ObterPorPagadorAsync(int pagadorId)
    {
        var pagamentos = await _unitOfWork.Pagamentos.ObterPorPagadorAsync(pagadorId);
        var dtos = new List<PagamentoDTO>();

        foreach (var pagamento in pagamentos)
        {
            var pagamentoCompleto = await _unitOfWork.Pagamentos.ObterComAlunosAsync(pagamento.Id);
            var dto = _mapper.Map<PagamentoDTO>(pagamentoCompleto);
            dto.EstaBalanceado = pagamentoCompleto!.EstaBalanceado();
            dtos.Add(dto);
        }

        return dtos;
    }
}
