using AutoMapper;
using Virtus.Application.DTOs.Matricula;
using Virtus.Application.DTOs.Common;
using Virtus.Application.Interfaces;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Domain.Entities;

namespace Virtus.Application.Services;

public class MatriculaService : IMatriculaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MatriculaService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultadoDTO<MatriculaDTO>> CriarAsync(CriarMatriculaDTO dto)
    {
        var aluno = await _unitOfWork.Alunos.ObterComPessoaAsync(dto.AlunoId);
        if (aluno == null)
        {
            return ResultadoDTO.ComErro<MatriculaDTO>("Aluno não encontrado");
        }

        var turma = await _unitOfWork.Turmas.ObterComMatriculasAsync(dto.TurmaId);
        if (turma == null)
        {
            return ResultadoDTO.ComErro<MatriculaDTO>("Turma não encontrada");
        }

        if (!turma.Ativa)
        {
            return ResultadoDTO.ComErro<MatriculaDTO>("Turma não está ativa");
        }

        if (await _unitOfWork.Matriculas.AlunoJaMatriculadoAsync(dto.AlunoId, dto.TurmaId))
        {
            return ResultadoDTO.ComErro<MatriculaDTO>("Aluno já está matriculado nesta turma");
        }

        if (!aluno.PodeMatricular(turma))
        {
            return ResultadoDTO.ComErro<MatriculaDTO>("Aluno não pode ser matriculado nesta turma");
        }

        if (!turma.TemVagasDisponiveis())
        {
            aluno.AdicionarNaListaDeEspera();
            await _unitOfWork.SalvarAsync();
            return ResultadoDTO.ComErro<MatriculaDTO>("Turma sem vagas disponíveis. Aluno adicionado à lista de espera");
        }

        var matricula = new Matricula(aluno, turma);
        await _unitOfWork.Matriculas.AdicionarAsync(matricula);
        await _unitOfWork.SalvarAsync();

        var resultado = _mapper.Map<MatriculaDTO>(matricula);
        return ResultadoDTO.ComSucesso<MatriculaDTO>(resultado);
    }

    public async Task<ResultadoDTO<bool>> CancelarAsync(int id)
    {
        var matricula = await _unitOfWork.Matriculas.ObterPorIdAsync(id);

        if (matricula == null)
        {
            return ResultadoDTO.ComErro<bool>("Matrícula não encontrada");
        }

        try
        {
            matricula.Cancelar();
            await _unitOfWork.SalvarAsync();

            await VerificarListaEsperaAsync(matricula.TurmaId);

            return ResultadoDTO.ComSucesso<bool>(true);
        }
        catch (InvalidOperationException ex)
        {
            return ResultadoDTO.ComErro<bool>(ex.Message);
        }
    }

    public async Task<ResultadoDTO<bool>> TrancarAsync(int id)
    {
        var matricula = await _unitOfWork.Matriculas.ObterPorIdAsync(id);

        if (matricula == null)
        {
            return ResultadoDTO.ComErro<bool>("Matrícula não encontrada");
        }

        try
        {
            matricula.Trancar();
            await _unitOfWork.SalvarAsync();

            await VerificarListaEsperaAsync(matricula.TurmaId);

            return ResultadoDTO.ComSucesso<bool>(true);
        }
        catch (InvalidOperationException ex)
        {
            return ResultadoDTO.ComErro<bool>(ex.Message);
        }
    }

    public async Task<ResultadoDTO<bool>> ReativarAsync(int id)
    {
        var matricula = await _unitOfWork.Matriculas.ObterPorIdAsync(id);

        if (matricula == null)
        {
            return ResultadoDTO.ComErro<bool>("Matrícula não encontrada");
        }

        var turma = await _unitOfWork.Turmas.ObterComMatriculasAsync(matricula.TurmaId);

        if (!turma!.TemVagasDisponiveis())
        {
            return ResultadoDTO.ComErro<bool>("Turma não tem vagas disponíveis");
        }

        try
        {
            matricula.Reativar();
            await _unitOfWork.SalvarAsync();

            return ResultadoDTO.ComSucesso<bool>(true);
        }
        catch (InvalidOperationException ex)
        {
            return ResultadoDTO.ComErro<bool>(ex.Message);
        }
    }

    public async Task<IEnumerable<MatriculaDTO>> ObterPorAlunoAsync(int alunoId)
    {
        var matriculas = await _unitOfWork.Matriculas.ObterPorAlunoAsync(alunoId);
        return _mapper.Map<IEnumerable<MatriculaDTO>>(matriculas);
    }

    public async Task<IEnumerable<MatriculaDTO>> ObterPorTurmaAsync(int turmaId)
    {
        var matriculas = await _unitOfWork.Matriculas.ObterPorTurmaAsync(turmaId);
        return _mapper.Map<IEnumerable<MatriculaDTO>>(matriculas);
    }

    private async Task VerificarListaEsperaAsync(int turmaId)
    {
        var turma = await _unitOfWork.Turmas.ObterComMatriculasAsync(turmaId);

        if (turma!.TemVagasDisponiveis())
        {
            var alunosListaEspera = await _unitOfWork.Alunos.ObterPorStatusAsync(StatusAluno.ListaEspera);
            var primeiroAluno = alunosListaEspera.FirstOrDefault();

            if (primeiroAluno != null)
            {
                primeiroAluno.Reativar();
                await _unitOfWork.SalvarAsync();
            }
        }
    }
}
