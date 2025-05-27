using AutoMapper;
using Virtus.Application.DTOs.Aluno;
using Virtus.Application.DTOs.Common;
using Virtus.Application.Interfaces;
using Virtus.Domain.Interfaces;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.ValueObjects;

namespace Virtus.Application.Services;

public class AlunoService : IAlunoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AlunoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultadoDTO<AlunoDTO>> ObterPorIdAsync(int id)
    {
        var aluno = await _unitOfWork.Alunos.ObterComPessoaAsync(id);

        if (aluno == null)
        {
            return ResultadoDTO.ComErro<AlunoDTO>("Aluno não encontrado");
        }

        var dto = _mapper.Map<AlunoDTO>(aluno);
        return ResultadoDTO.ComSucesso(dto);
    }

    public async Task<RespostaPaginadaDTO<AlunoListaDTO>> ObterTodosAsync(int pagina = 1, int itensPorPagina = 20)
    {
        var alunos = await _unitOfWork.Alunos.ObterTodosComPessoaAsync();
        var totalItens = alunos.Count();

        var alunosPaginados = alunos
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToList();

        var dtos = _mapper.Map<List<AlunoListaDTO>>(alunosPaginados);

        return new RespostaPaginadaDTO<AlunoListaDTO>
        {
            Itens = dtos,
            TotalItens = totalItens,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)itensPorPagina)
        };
    }

    public async Task<ResultadoDTO<AlunoDTO>> CriarAsync(CriarAlunoDTO dto)
    {
        if (await _unitOfWork.Alunos.ExisteComEmailAsync(dto.Email))
        {
            return ResultadoDTO.ComErro<AlunoDTO>("Email já cadastrado");
        }

        var pessoa = new Pessoa(dto.Nome, Email.Criar(dto.Email), dto.Telefone ?? string.Empty, TipoPessoa.Aluno);
        await _unitOfWork.Pessoas.AdicionarAsync(pessoa);

        Pessoa? responsavel = null;
        if (dto.ResponsavelId.HasValue)
        {
            responsavel = await _unitOfWork.Pessoas.ObterPorIdAsync(dto.ResponsavelId.Value);
            if (responsavel == null)
            {
                return ResultadoDTO.ComErro<AlunoDTO>("Responsável não encontrado");
            }
        }

        var aluno = new Aluno(pessoa, responsavel);
        await _unitOfWork.Alunos.AdicionarAsync(aluno);

        await _unitOfWork.SalvarAsync();

        var alunoSalvo = await _unitOfWork.Alunos.ObterComPessoaAsync(aluno.Id);
        var resultado = _mapper.Map<AlunoDTO>(alunoSalvo);

        return ResultadoDTO.ComSucesso(resultado);
    }

    public async Task<ResultadoDTO<AlunoDTO>> AtualizarAsync(int id, AtualizarAlunoDTO dto)
    {
        var aluno = await _unitOfWork.Alunos.ObterComPessoaAsync(id);

        if (aluno == null)
        {
            return ResultadoDTO.ComErro<AlunoDTO>("Aluno não encontrado");
        }

        if (await _unitOfWork.Pessoas.EmailJaExisteAsync(dto.Email, aluno.PessoaId))
        {
            return ResultadoDTO.ComErro<AlunoDTO>("Email já cadastrado para outro usuário");
        }

        aluno.Pessoa.AtualizarDados(dto.Nome, Email.Criar(dto.Email), dto.Telefone ?? string.Empty);

        if (dto.ResponsavelId.HasValue && dto.ResponsavelId != aluno.ResponsavelId)
        {
            var responsavel = await _unitOfWork.Pessoas.ObterPorIdAsync(dto.ResponsavelId.Value);
            if (responsavel == null)
            {
                return ResultadoDTO.ComErro<AlunoDTO>("Responsável não encontrado");
            }

            aluno.AtualizarResponsavel(responsavel);
        }
        else if (!dto.ResponsavelId.HasValue && aluno.ResponsavelId.HasValue)
        {
            aluno.AtualizarResponsavel(null);
        }

        await _unitOfWork.SalvarAsync();

        var resultado = _mapper.Map<AlunoDTO>(aluno);
        return ResultadoDTO.ComSucesso(resultado);
    }

    public async Task<ResultadoDTO<bool>> InativarAsync(int id)
    {
        var aluno = await _unitOfWork.Alunos.ObterPorIdAsync(id);

        if (aluno == null)
        {
            return ResultadoDTO.ComErro<bool>("Aluno não encontrado");
        }

        aluno.Inativar();
        await _unitOfWork.SalvarAsync();

        return ResultadoDTO.ComSucesso(true);
    }

    public async Task<ResultadoDTO<bool>> ReativarAsync(int id)
    {
        var aluno = await _unitOfWork.Alunos.ObterPorIdAsync(id);

        if (aluno == null)
        {
            return ResultadoDTO.ComErro<bool>("Aluno não encontrado");
        }

        aluno.Reativar();
        await _unitOfWork.SalvarAsync();

        return ResultadoDTO.ComSucesso(true);
    }

    public async Task<IEnumerable<AlunoListaDTO>> ObterPorResponsavelAsync(int responsavelId)
    {
        var alunos = await _unitOfWork.Alunos.ObterPorResponsavelAsync(responsavelId);
        return _mapper.Map<IEnumerable<AlunoListaDTO>>(alunos);
    }

    public async Task<IEnumerable<AlunoListaDTO>> ObterListaEsperaAsync()
    {
        var alunos = await _unitOfWork.Alunos.ObterPorStatusAsync(StatusAluno.ListaEspera);
        return _mapper.Map<IEnumerable<AlunoListaDTO>>(alunos);
    }
}
