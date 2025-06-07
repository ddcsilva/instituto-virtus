using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Infrastructure.Data.SeedData;

public static class DevelopmentSeedData
{
  public static async Task SeedAsync(VirtusDbContext context)
  {
    // Se já existem dados, não faz seed
    if (await context.Pessoas.AnyAsync())
      return;

    // Pessoas - Professores
    var profCarlos = new Pessoa("Carlos Silva", TipoPessoa.Professor, "carlos.silva@institutovirtus.com.br", "(11) 98765-4321");
    var profMaria = new Pessoa("Maria Santos", TipoPessoa.Professor, "maria.santos@institutovirtus.com.br", "(11) 98765-4322");
    var profJose = new Pessoa("José Oliveira", TipoPessoa.Professor, "jose.oliveira@institutovirtus.com.br", "(11) 98765-4323");
    var profAna = new Pessoa("Ana Costa", TipoPessoa.Professor, "ana.costa@institutovirtus.com.br", "(11) 98765-4324");

    // Pessoas - Responsáveis
    var respPedro = new Pessoa("Pedro Almeida", TipoPessoa.Responsavel, "pedro.almeida@email.com", "(11) 91234-5678", "123.456.789-00");
    var respLucia = new Pessoa("Lúcia Ferreira", TipoPessoa.Responsavel, "lucia.ferreira@email.com", "(11) 91234-5679", "987.654.321-00");

    // Pessoas - Alunos
    var alunoJoao = new Pessoa("João Pedro", TipoPessoa.Aluno, "joao.pedro@email.com", "(11) 99876-5432");
    var alunoMariana = new Pessoa("Mariana Silva", TipoPessoa.Aluno, "mariana.silva@email.com", "(11) 99876-5433");
    var alunoLucas = new Pessoa("Lucas Almeida", TipoPessoa.Aluno, "lucas.almeida@email.com", "(11) 99876-5434");
    var alunoJulia = new Pessoa("Júlia Ferreira", TipoPessoa.Aluno, "julia.ferreira@email.com", "(11) 99876-5435");
    var alunoRafael = new Pessoa("Rafael Santos", TipoPessoa.Aluno, "rafael.santos@email.com", "(11) 99876-5436");
    var alunoBeatriz = new Pessoa("Beatriz Costa", TipoPessoa.Aluno, "beatriz.costa@email.com", "(11) 99876-5437");

    await context.Pessoas.AddRangeAsync(
      profCarlos, profMaria, profJose, profAna,
      respPedro, respLucia,
      alunoJoao, alunoMariana, alunoLucas, alunoJulia, alunoRafael, alunoBeatriz
    );

    await context.SaveChangesAsync();

    // Professores
    var professorCarlos = new Professor(profCarlos, "Violão");
    var professorMaria = new Professor(profMaria, "Teclado e Piano");
    var professorJose = new Professor(profJose, "Canto e Técnica Vocal");
    var professorAna = new Professor(profAna, "Teologia");

    await context.Professores.AddRangeAsync(professorCarlos, professorMaria, professorJose, professorAna);
    await context.SaveChangesAsync();

    // Alunos
    var aluno1 = new Aluno(alunoJoao);
    var aluno2 = new Aluno(alunoMariana);
    var aluno3 = new Aluno(alunoLucas, respPedro); // Lucas é filho de Pedro
    var aluno4 = new Aluno(alunoJulia, respLucia); // Júlia é filha de Lúcia
    var aluno5 = new Aluno(alunoRafael);
    var aluno6 = new Aluno(alunoBeatriz);

    await context.Alunos.AddRangeAsync(aluno1, aluno2, aluno3, aluno4, aluno5, aluno6);
    await context.SaveChangesAsync();

    // Turmas
    var turmaViolaoBasico = new Turma("Violão Básico", 10, TipoCurso.Violao, professorCarlos, "19:00", "Segunda", DateTime.UtcNow);
    var turmaViolaoIntermediario = new Turma("Violão Intermediário", 8, TipoCurso.Violao, professorCarlos, "20:00", "Quarta", DateTime.UtcNow);
    var turmaTecladoBasico = new Turma("Teclado Básico", 6, TipoCurso.Teclado, professorMaria, "18:00", "Terça", DateTime.UtcNow);
    var turmaCantoBasico = new Turma("Canto Básico", 12, TipoCurso.Canto, professorJose, "19:00", "Quinta", DateTime.UtcNow);
    var turmaTeologia = new Turma("Teologia Fundamental", 20, TipoCurso.Teologia, professorAna, "19:30", "Sexta", DateTime.UtcNow);

    await context.Turmas.AddRangeAsync(turmaViolaoBasico, turmaViolaoIntermediario, turmaTecladoBasico, turmaCantoBasico, turmaTeologia);
    await context.SaveChangesAsync();

    // Matrículas
    var matricula1 = new Matricula(aluno1, turmaViolaoBasico);
    var matricula2 = new Matricula(aluno2, turmaViolaoBasico);
    var matricula3 = new Matricula(aluno3, turmaTecladoBasico);
    var matricula4 = new Matricula(aluno4, turmaCantoBasico);
    var matricula5 = new Matricula(aluno5, turmaTeologia);
    var matricula6 = new Matricula(aluno6, turmaTeologia);

    await context.Matriculas.AddRangeAsync(matricula1, matricula2, matricula3, matricula4, matricula5, matricula6);
    await context.SaveChangesAsync();

    // Pagamentos
    var pagamento1 = new Pagamento(150.00m, DateTime.UtcNow.AddDays(5), respPedro, "PIX");
    pagamento1.AdicionarAluno(aluno3, 150.00m); // Pedro paga por Lucas
    pagamento1.ConfirmarPagamento("PIX123456");

    var pagamento2 = new Pagamento(150.00m, DateTime.UtcNow.AddDays(5), respLucia, "Transferência");
    pagamento2.AdicionarAluno(aluno4, 150.00m); // Lúcia paga por Júlia

    var pagamento3 = new Pagamento(150.00m, DateTime.UtcNow.AddDays(-10), alunoJoao, "PIX");
    pagamento3.AdicionarAluno(aluno1, 150.00m); // João paga por si mesmo
    pagamento3.MarcarComoAtrasado();

    await context.Pagamentos.AddRangeAsync(pagamento1, pagamento2, pagamento3);
    await context.SaveChangesAsync();
  }
}