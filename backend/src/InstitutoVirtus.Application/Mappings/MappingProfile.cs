namespace InstitutoVirtus.Application.Mappings;

using AutoMapper;
using InstitutoVirtus.Application.DTOs.Aula;
using InstitutoVirtus.Application.DTOs.Avaliacao;
using InstitutoVirtus.Application.DTOs.Curso;
using InstitutoVirtus.Application.DTOs.Matricula;
using InstitutoVirtus.Application.DTOs.Mensalidade;
using InstitutoVirtus.Application.DTOs.Nota;
using InstitutoVirtus.Application.DTOs.Pagamento;
using InstitutoVirtus.Application.DTOs.Pessoa;
using InstitutoVirtus.Application.DTOs.Presenca;
using InstitutoVirtus.Application.DTOs.Turma;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Pessoa mappings
        CreateMap<Pessoa, PessoaDto>()
            .ForMember(d => d.Telefone, opt => opt.MapFrom(s => s.Telefone.NumeroFormatado()))
            .ForMember(d => d.Cpf, opt => opt.MapFrom(s => s.Cpf != null ? s.Cpf.Numero : null))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email != null ? s.Email.Endereco : null))
            .ForMember(d => d.TipoPessoa, opt => opt.MapFrom(s => s.TipoPessoa.ToString()))
            .ForMember(d => d.Idade, opt => opt.MapFrom(s =>
                (DateTime.Today.Year - s.DataNascimento.Year) -
                ((DateTime.Today.Month < s.DataNascimento.Month ||
                  (DateTime.Today.Month == s.DataNascimento.Month && DateTime.Today.Day < s.DataNascimento.Day)) ? 1 : 0)
            ));

        CreateMap<Aluno, AlunoDto>()
            .IncludeBase<Pessoa, PessoaDto>()
            .ForMember(d => d.Responsaveis, opt => opt.MapFrom(s => s.Responsaveis))
            .ForMember(d => d.Matriculas, opt => opt.MapFrom(s => s.Matriculas));

        CreateMap<ResponsavelAluno, ResponsavelResumoDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ResponsavelId))
            .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.Responsavel != null ? s.Responsavel.NomeCompleto : string.Empty))
            .ForMember(d => d.Telefone, opt => opt.MapFrom(s => s.Responsavel != null ? s.Responsavel.Telefone.NumeroFormatado() : string.Empty))
            .ForMember(d => d.Parentesco, opt => opt.MapFrom(s => s.Parentesco.ToString()));

        CreateMap<Professor, ProfessorDto>()
            .IncludeBase<Pessoa, PessoaDto>();

        CreateMap<Responsavel, ResponsavelDto>()
            .IncludeBase<Pessoa, PessoaDto>()
            .ForMember(d => d.Alunos, opt => opt.MapFrom(s => s.Alunos))
            .ForMember(d => d.SaldoCredito, opt => opt.MapFrom(s => s.SaldoCredito));

        CreateMap<ResponsavelAluno, AlunoResumoDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.AlunoId))
            .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.Aluno != null ? s.Aluno.NomeCompleto : string.Empty))
            .ForMember(d => d.Idade, opt => opt.MapFrom(s => s.Aluno != null ?
                (DateTime.Today.Year - s.Aluno.DataNascimento.Year) -
                ((DateTime.Today.Month < s.Aluno.DataNascimento.Month ||
                  (DateTime.Today.Month == s.Aluno.DataNascimento.Month && DateTime.Today.Day < s.Aluno.DataNascimento.Day)) ? 1 : 0)
                : 0));

        // Curso mappings
        CreateMap<Curso, CursoDto>()
            .ForMember(d => d.TotalTurmas, opt => opt.MapFrom(s => s.Turmas.Count));

        CreateMap<Curso, CursoResumoDto>();

        // Turma mappings
        CreateMap<Turma, TurmaDto>()
            .ForMember(d => d.CursoNome, opt => opt.MapFrom(s => s.Curso != null ? s.Curso.Nome : string.Empty))
            .ForMember(d => d.ProfessorNome, opt => opt.MapFrom(s => s.Professor != null ? s.Professor.NomeCompleto : string.Empty))
            .ForMember(d => d.DiaSemana, opt => opt.MapFrom(s => s.DiaSemana.ToString()))
            .ForMember(d => d.HorarioInicio, opt => opt.MapFrom(s => s.Horario.HoraInicio.ToString("hh\\:mm")))
            .ForMember(d => d.HorarioFim, opt => opt.MapFrom(s => s.Horario.HoraFim.ToString("hh\\:mm")))
            .ForMember(d => d.AlunosMatriculados, opt => opt.MapFrom(s => s.Matriculas.Count(m => m.Status == StatusMatricula.Ativa)))
            .ForMember(d => d.VagasDisponiveis, opt => opt.MapFrom(s => s.VagasDisponiveis()));

        CreateMap<Turma, TurmaResumoDto>()
            .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.ObterNome()))
            .ForMember(d => d.Horario, opt => opt.MapFrom(s => s.Horario.FormatoString()))
            .ForMember(d => d.VagasDisponiveis, opt => opt.MapFrom(s => s.VagasDisponiveis()));

        // Matrícula mappings
        CreateMap<Matricula, MatriculaDto>()
            .ForMember(d => d.AlunoNome, opt => opt.MapFrom(s => s.Aluno != null ? s.Aluno.NomeCompleto : string.Empty))
            .ForMember(d => d.TurmaNome, opt => opt.MapFrom(s => s.Turma != null ? s.Turma.ObterNome() : string.Empty))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Matricula, MatriculaResumoDto>()
            .ForMember(d => d.TurmaNome, opt => opt.MapFrom(s => s.Turma != null ? s.Turma.ObterNome() : string.Empty))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // Mensalidade mappings
        CreateMap<Mensalidade, MensalidadeDto>()
            .ForMember(d => d.AlunoNome, opt => opt.MapFrom(s => s.Matricula != null && s.Matricula.Aluno != null ? s.Matricula.Aluno.NomeCompleto : string.Empty))
            .ForMember(d => d.CursoNome, opt => opt.MapFrom(s => s.Matricula != null && s.Matricula.Turma != null && s.Matricula.Turma.Curso != null ? s.Matricula.Turma.Curso.Nome : string.Empty))
            .ForMember(d => d.Competencia, opt => opt.MapFrom(s => s.Competencia.FormatoString()))
            .ForMember(d => d.Valor, opt => opt.MapFrom(s => s.Valor.Valor))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.FormaPagamento, opt => opt.MapFrom(s => s.FormaPagamento != null ? s.FormaPagamento.ToString() : null))
            .ForMember(d => d.EstaVencida, opt => opt.MapFrom(s => s.EstaVencida()));

        // Pagamento mappings
        CreateMap<Pagamento, PagamentoDto>()
            .ForMember(d => d.ResponsavelNome, opt => opt.MapFrom(s => s.Responsavel != null ? s.Responsavel.NomeCompleto : string.Empty))
            .ForMember(d => d.ValorTotal, opt => opt.MapFrom(s => s.ValorTotal.Valor))
            .ForMember(d => d.MeioPagamento, opt => opt.MapFrom(s => s.MeioPagamento.ToString()))
            .ForMember(d => d.ValorDisponivel, opt => opt.MapFrom(s => s.ObterValorDisponivel().Valor));

        CreateMap<PagamentoParcela, ParcelaDto>()
            .ForMember(d => d.Competencia, opt => opt.MapFrom(s => s.Mensalidade != null ? s.Mensalidade.Competencia.FormatoString() : string.Empty))
            .ForMember(d => d.AlunoNome, opt => opt.MapFrom(s => s.Mensalidade != null && s.Mensalidade.Matricula != null && s.Mensalidade.Matricula.Aluno != null ? s.Mensalidade.Matricula.Aluno.NomeCompleto : string.Empty))
            .ForMember(d => d.ValorAlocado, opt => opt.MapFrom(s => s.ValorAlocado.Valor));

        // Aula mappings
        CreateMap<Aula, AulaDto>()
            .ForMember(d => d.TurmaNome, opt => opt.MapFrom(s => s.Turma != null ? s.Turma.ObterNome() : string.Empty))
            .ForMember(d => d.TotalPresentes, opt => opt.MapFrom(s => s.Presencas.Count(p => p.Status == StatusPresenca.Presente || p.Status == StatusPresenca.Justificada)))
            .ForMember(d => d.TotalFaltas, opt => opt.MapFrom(s => s.Presencas.Count(p => p.Status == StatusPresenca.Falta)))
            .ForMember(d => d.PercentualPresenca, opt => opt.MapFrom(s => s.CalcularPercentualPresenca()));

        // Presença mappings
        CreateMap<Presenca, PresencaDto>()
            .ForMember(d => d.AlunoNome, opt => opt.MapFrom(s => s.Aluno != null ? s.Aluno.NomeCompleto : string.Empty))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // Avaliação mappings
        CreateMap<Avaliacao, AvaliacaoDto>()
            .ForMember(d => d.TurmaNome, opt => opt.MapFrom(s => s.Turma != null ? s.Turma.ObterNome() : string.Empty));

        // Nota mappings
        CreateMap<Nota, NotaDto>()
            .ForMember(d => d.AvaliacaoNome, opt => opt.MapFrom(s => s.Avaliacao != null ? s.Avaliacao.Nome : string.Empty))
            .ForMember(d => d.AlunoNome, opt => opt.MapFrom(s => s.Aluno != null ? s.Aluno.NomeCompleto : string.Empty));
    }
}
