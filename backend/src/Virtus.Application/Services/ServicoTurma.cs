using AutoMapper;
using Virtus.Application.DTOs.Turma;
using Virtus.Application.DTOs.Common;
using Virtus.Application.Interfaces;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Domain.Entities;

namespace Virtus.Application.Services;

public class ServicoTurma : IServicoTurma
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ServicoTurma(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultadoDTO<TurmaDTO>> ObterPorIdAsync(int id)
    {
        var turma = await _unitOfWork.Turmas.ObterComMatriculasAsync(id);

        if (turma == null)
        {
            return ResultadoDTO.ComErro<TurmaDTO>("Turma não encontrada");
        }

        var dto = _mapper.Map<TurmaDTO>(turma);
        dto.VagasDisponiveis = turma.ObterQuantidadeVagasDisponiveis();
        dto.AlunosMatriculados = turma.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);

        return ResultadoDTO.ComSucesso(dto);
    }

    public async Task<RespostaPaginadaDTO<TurmaListaDTO>> ObterTodasAsync(int pagina = 1, int itensPorPagina = 20)
    {
        var turmas = await _unitOfWork.Turmas.ObterTodosAsync();
        var totalItens = turmas.Count();

        var turmasPaginadas = turmas
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToList();

        var dtos = new List<TurmaListaDTO>();

        foreach (var turma in turmasPaginadas)
        {
            var turmaCompleta = await _unitOfWork.Turmas.ObterComMatriculasAsync(turma.Id);
            var dto = _mapper.Map<TurmaListaDTO>(turmaCompleta);

            var matriculasAtivas = turmaCompleta!.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            dto.StatusVagas = $"{matriculasAtivas}/{turmaCompleta.Capacidade}";
            dto.TemVagas = turmaCompleta.TemVagasDisponiveis();

            dtos.Add(dto);
        }

        return new RespostaPaginadaDTO<TurmaListaDTO>
        {
            Itens = dtos,
            TotalItens = totalItens,
            PaginaAtual = pagina,
            ItensPorPagina = itensPorPagina,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)itensPorPagina)
        };
    }

    public async Task<ResultadoDTO<TurmaDTO>> CriarAsync(CriarTurmaDTO dto)
    {
        var professor = await _unitOfWork.Pessoas.ObterPorIdAsync(dto.ProfessorId);

        if (professor == null || professor.Tipo != TipoPessoa.Professor)
        {
            return ResultadoDTO.ComErro<TurmaDTO>("Professor não encontrado");
        }

        var professorEntidade = new Professor(professor);

        var turma = new Turma(dto.Nome, dto.Capacidade, dto.Tipo, professorEntidade);
        await _unitOfWork.Turmas.AdicionarAsync(turma);
        await _unitOfWork.SalvarAsync();

        var turmaSalva = await _unitOfWork.Turmas.ObterComMatriculasAsync(turma.Id);
        var resultado = _mapper.Map<TurmaDTO>(turmaSalva);
        resultado.VagasDisponiveis = turmaSalva!.ObterQuantidadeVagasDisponiveis();
        resultado.AlunosMatriculados = 0;

        return ResultadoDTO.ComSucesso(resultado);
    }

    public async Task<ResultadoDTO<TurmaDTO>> AtualizarAsync(int id, AtualizarTurmaDTO dto)
    {
        var turma = await _unitOfWork.Turmas.ObterComMatriculasAsync(id);

        if (turma == null)
        {
            return ResultadoDTO.ComErro<TurmaDTO>("Turma não encontrada");
        }

        var matriculasAtivas = turma.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);

        if (dto.Capacidade < matriculasAtivas)
        {
            return ResultadoDTO.ComErro<TurmaDTO>($"Capacidade não pode ser menor que o número de alunos matriculados ({matriculasAtivas})");
        }

        var professor = await _unitOfWork.Pessoas.ObterPorIdAsync(dto.ProfessorId);

        if (professor == null || professor.Tipo != TipoPessoa.Professor)
        {
            return ResultadoDTO.ComErro<TurmaDTO>("Professor não encontrado");
        }

        var professorEntidade = new Professor(professor);

        turma.AtualizarDados(dto.Nome, dto.Capacidade, professorEntidade);

        await _unitOfWork.SalvarAsync();

        var resultado = _mapper.Map<TurmaDTO>(turma);
        resultado.VagasDisponiveis = turma.ObterQuantidadeVagasDisponiveis();
        resultado.AlunosMatriculados = matriculasAtivas;

        return ResultadoDTO.ComSucesso(resultado);
    }

    public async Task<ResultadoDTO<bool>> InativarAsync(int id)
    {
        var turma = await _unitOfWork.Turmas.ObterPorIdAsync(id);

        if (turma == null)
        {
            return ResultadoDTO.ComErro<bool>("Turma não encontrada");
        }

        turma.Inativar();
        await _unitOfWork.SalvarAsync();

        return ResultadoDTO.ComSucesso(true);
    }

    public async Task<ResultadoDTO<bool>> AtivarAsync(int id)
    {
        var turma = await _unitOfWork.Turmas.ObterPorIdAsync(id);

        if (turma == null)
        {
            return ResultadoDTO.ComErro<bool>("Turma não encontrada");
        }

        turma.Ativar();
        await _unitOfWork.SalvarAsync();

        return ResultadoDTO.ComSucesso(true);
    }

    public async Task<IEnumerable<TurmaListaDTO>> ObterPorTipoAsync(TipoCurso tipo)
    {
        var turmas = await _unitOfWork.Turmas.ObterPorTipoAsync(tipo);
        var dtos = new List<TurmaListaDTO>();

        foreach (var turma in turmas)
        {
            var turmaCompleta = await _unitOfWork.Turmas.ObterComMatriculasAsync(turma.Id);
            var dto = _mapper.Map<TurmaListaDTO>(turmaCompleta);

            var matriculasAtivas = turmaCompleta!.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            dto.StatusVagas = $"{matriculasAtivas}/{turmaCompleta.Capacidade}";
            dto.TemVagas = turmaCompleta.TemVagasDisponiveis();

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<IEnumerable<TurmaListaDTO>> ObterComVagasAsync()
    {
        var turmas = await _unitOfWork.Turmas.ObterComVagasDisponiveisAsync();
        var dtos = new List<TurmaListaDTO>();

        foreach (var turma in turmas)
        {
            var turmaCompleta = await _unitOfWork.Turmas.ObterComMatriculasAsync(turma.Id);
            var dto = _mapper.Map<TurmaListaDTO>(turmaCompleta);

            var matriculasAtivas = turmaCompleta!.Matriculas.Count(m => m.Status == StatusMatricula.Ativa);
            dto.StatusVagas = $"{matriculasAtivas}/{turmaCompleta.Capacidade}";
            dto.TemVagas = true;

            dtos.Add(dto);
        }

        return dtos;
    }
}
