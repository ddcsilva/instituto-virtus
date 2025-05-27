using Virtus.Application.DTOs.Aluno;
using Virtus.Application.DTOs.Turma;
using Virtus.Application.DTOs.Matricula;
using Virtus.Application.DTOs.Pagamento;
using Virtus.Application.DTOs.Professor;
using Virtus.Application.DTOs.Responsavel;
using Virtus.Domain.Entities;
using AutoMapper;

namespace Virtus.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapeamentos de Aluno
        CreateMap<Aluno, AlunoDTO>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Pessoa.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Pessoa.Email.Endereco))
            .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Pessoa.Telefone))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Pessoa, ResponsavelDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Endereco));

        CreateMap<Aluno, AlunoListaDTO>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Pessoa.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Pessoa.Email.Endereco))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.Responsavel != null ? src.Responsavel.Nome : null))
            .ForMember(dest => dest.QuantidadeTurmas, opt => opt.MapFrom(src => src.Matriculas.Count));

        // Mapeamentos de Turma
        CreateMap<Turma, TurmaDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.VagasDisponiveis, opt => opt.Ignore())
            .ForMember(dest => dest.AlunosMatriculados, opt => opt.Ignore());

        CreateMap<Professor, ProfessorDTO>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Pessoa.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Pessoa.Email.Endereco));

        CreateMap<Turma, TurmaListaDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.NomeProfessor, opt => opt.MapFrom(src => src.Professor.Pessoa.Nome))
            .ForMember(dest => dest.StatusVagas, opt => opt.Ignore())
            .ForMember(dest => dest.TemVagas, opt => opt.Ignore());

        // Mapeamentos de Matrícula
        CreateMap<Matricula, MatriculaDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Aluno, AlunoMatriculaDTO>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Pessoa.Nome))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Pessoa.Email.Endereco));

        CreateMap<Turma, TurmaMatriculaDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()));

        // Mapeamentos de Pagamento
        CreateMap<Pagamento, PagamentoDTO>()
            .ForMember(dest => dest.EstaBalanceado, opt => opt.Ignore())
            .ForMember(dest => dest.Alunos, opt => opt.MapFrom(src => src.PagamentoAlunos));

        CreateMap<Pessoa, PagadorDTO>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Endereco));

        CreateMap<PagamentoAluno, AlunoPagamentoDTO>()
            .ForMember(dest => dest.NomeAluno, opt => opt.MapFrom(src => src.Aluno.Pessoa.Nome));
    }
}
