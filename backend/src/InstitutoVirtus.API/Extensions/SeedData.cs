using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.API.Extensions;

public static class SeedData
{
    public static async Task InitializeAsync(VirtusDbContext context)
    {
        // Verificar se já existem dados
        if (await context.Pessoas.AnyAsync())
            return;

        // === 1. CRIAR CURSOS ===
        var cursos = new[]
        {
            new Curso("Teclado", "Curso de teclado para iniciantes e avançados", 150, 40),
            new Curso("Violão", "Curso de violão popular e clássico", 120, 40),
            new Curso("Bateria", "Curso de bateria", 180, 40),
            new Curso("Canto", "Curso de técnica vocal", 100, 30),
            new Curso("Teoria Musical", "Teoria e percepção musical", 80, 20),
            new Curso("Teologia Básica", "Introdução à teologia", 100, 60),
            new Curso("Teologia Avançada", "Estudos aprofundados em teologia", 150, 80)
        };

        await context.Cursos.AddRangeAsync(cursos);

        // === 2. CRIAR USUÁRIOS DO SISTEMA ===

        // Admin/Coordenação
        var admin = new Pessoa(
            "Administrador Sistema",
            new Telefone("11999999999"),
            new Email("admin@institutovirtus.com.br"),
            new DateTime(1980, 1, 1),
            TipoPessoa.Coordenador,
            "Usuário administrador do sistema",
            senha: "Admin@123"
        );

        var coordenacao = new Pessoa(
            "Tairiny Oliveira",
            new Telefone("11988887777"),
            new Email("tairiny@institutovirtus.com.br"),
            new DateTime(1985, 5, 15),
            TipoPessoa.Coordenador,
            "Coordenadora pedagógica",
            senha: "Coord@123"
        );

        // === 3. CRIAR PROFESSORES ===
        var prof1 = new Professor(
            "João Silva",
            new Telefone("11999888777"),
            new Email("joao.silva@institutovirtus.com.br"),
            new DateTime(1985, 5, 15),
            "Teclado e Piano",
            "Professor de teclado com 15 anos de experiência"
        );
        prof1.DefinirSenha("Prof@123");

        var prof2 = new Professor(
            "Maria Santos",
            new Telefone("11999777666"),
            new Email("maria.santos@institutovirtus.com.br"),
            new DateTime(1990, 8, 22),
            "Violão e Guitarra",
            "Especialista em violão clássico e popular"
        );
        prof2.DefinirSenha("Prof@123");

        var prof3 = new Professor(
            "Pedro Costa",
            new Telefone("11999666555"),
            new Email("pedro.costa@institutovirtus.com.br"),
            new DateTime(1988, 3, 10),
            "Bateria e Percussão",
            "Baterista profissional"
        );
        prof3.DefinirSenha("Prof@123");

        var prof4 = new Professor(
            "Ana Paula Rodrigues",
            new Telefone("11999555444"),
            new Email("ana.rodrigues@institutovirtus.com.br"),
            new DateTime(1987, 7, 20),
            "Canto e Técnica Vocal",
            "Cantora lírica e professora de canto"
        );
        prof4.DefinirSenha("Prof@123");

        var prof5 = new Professor(
            "Carlos Mendes",
            new Telefone("11999444333"),
            new Email("carlos.mendes@institutovirtus.com.br"),
            new DateTime(1975, 11, 30),
            "Teoria Musical",
            "Maestro e teórico musical"
        );
        prof5.DefinirSenha("Prof@123");

        var prof6 = new Professor(
            "Pastor Roberto Lima",
            new Telefone("11999333222"),
            new Email("roberto.lima@institutovirtus.com.br"),
            new DateTime(1970, 2, 14),
            "Teologia",
            "Pastor e teólogo"
        );
        prof6.DefinirSenha("Prof@123");

        // === 4. CRIAR RESPONSÁVEIS ===
        var resp1 = new Responsavel(
            "José Oliveira Pai",
            new Telefone("11998765432"),
            new Email("jose.oliveira@gmail.com"),
            new DateTime(1975, 10, 20),
            "Responsável família Oliveira - Senha: Resp@123"
        );

        var resp2 = new Responsavel(
            "Maria Silva Mãe",
            new Telefone("11997654321"),
            new Email("maria.silva@gmail.com"),
            new DateTime(1978, 3, 15),
            "Responsável família Silva - Senha: Resp@123"
        );

        var resp3 = new Responsavel(
            "Ana Costa",
            new Telefone("11996543210"),
            new Email("ana.costa@gmail.com"),
            new DateTime(1980, 6, 25),
            "Responsável família Costa - Senha: Resp@123"
        );

        // === 5. CRIAR ALUNOS ===
        var aluno1 = new Aluno(
            "Lucas Oliveira",
            new Telefone("11998765433"),
            null, // Menor de idade, email opcional
            new DateTime(2010, 4, 15),
            "Aluno de teclado"
        );

        var aluno2 = new Aluno(
            "Julia Oliveira",
            new Telefone("11998765434"),
            null,
            new DateTime(2012, 8, 22),
            "Aluna de violão"
        );

        var aluno3 = new Aluno(
            "Pedro Silva",
            new Telefone("11997654322"),
            null,
            new DateTime(2011, 1, 10),
            "Aluno de bateria"
        );

        var aluno4 = new Aluno(
            "Mariana Silva",
            new Telefone("11997654323"),
            null,
            new DateTime(2013, 5, 30),
            "Aluna de canto"
        );

        var aluno5 = new Aluno(
            "Gabriel Costa",
            new Telefone("11996543211"),
            new Email("gabriel.costa@gmail.com"), // Maior de idade
            new DateTime(2005, 11, 15),
            "Aluno de teologia"
        );
        aluno5.DefinirSenha("Aluno@123");

        var aluno6 = new Aluno(
            "Beatriz Santos",
            new Telefone("11995432100"),
            new Email("beatriz.santos@gmail.com"), // Maior de idade
            new DateTime(2004, 9, 8),
            "Aluna de teoria musical"
        );
        aluno6.DefinirSenha("Aluno@123");

        // Salvar pessoas
        await context.Pessoas.AddRangeAsync(
            admin, coordenacao,
            prof1, prof2, prof3, prof4, prof5, prof6,
            resp1, resp2, resp3,
            aluno1, aluno2, aluno3, aluno4, aluno5, aluno6
        );

        await context.SaveChangesAsync();

        // === 6. VINCULAR RESPONSÁVEIS AOS ALUNOS ===
        var vinculos = new[]
        {
            new ResponsavelAluno(resp1.Id, aluno1.Id, Parentesco.Pai, true),
            new ResponsavelAluno(resp1.Id, aluno2.Id, Parentesco.Pai, true),
            new ResponsavelAluno(resp2.Id, aluno3.Id, Parentesco.Mae, true),
            new ResponsavelAluno(resp2.Id, aluno4.Id, Parentesco.Mae, true),
            new ResponsavelAluno(resp3.Id, aluno5.Id, Parentesco.Mae, true)
        };

        await context.ResponsaveisAlunos.AddRangeAsync(vinculos);

        // === 7. CRIAR TURMAS ===
        var turmas = new[]
        {
            // Teclado
            new Turma(
                cursos[0].Id, prof1.Id,
                DiaSemana.Segunda,
                new HorarioAula(new TimeSpan(14, 0, 0), new TimeSpan(14, 50, 0)),
                15, 2025, 1, "Sala 1"
            ),
            new Turma(
                cursos[0].Id, prof1.Id,
                DiaSemana.Quarta,
                new HorarioAula(new TimeSpan(15, 0, 0), new TimeSpan(15, 50, 0)),
                15, 2025, 1, "Sala 1"
            ),

            // Violão
            new Turma(
                cursos[1].Id, prof2.Id,
                DiaSemana.Terca,
                new HorarioAula(new TimeSpan(14, 0, 0), new TimeSpan(14, 50, 0)),
                12, 2025, 1, "Sala 2"
            ),
            new Turma(
                cursos[1].Id, prof2.Id,
                DiaSemana.Quinta,
                new HorarioAula(new TimeSpan(16, 0, 0), new TimeSpan(16, 50, 0)),
                12, 2025, 1, "Sala 2"
            ),

            // Bateria
            new Turma(
                cursos[2].Id, prof3.Id,
                DiaSemana.Segunda,
                new HorarioAula(new TimeSpan(16, 0, 0), new TimeSpan(16, 50, 0)),
                8, 2025, 1, "Sala 3"
            ),

            // Canto
            new Turma(
                cursos[3].Id, prof4.Id,
                DiaSemana.Sexta,
                new HorarioAula(new TimeSpan(14, 0, 0), new TimeSpan(14, 50, 0)),
                10, 2025, 1, "Sala 4"
            ),

            // Teoria Musical
            new Turma(
                cursos[4].Id, prof5.Id,
                DiaSemana.Sabado,
                new HorarioAula(new TimeSpan(9, 0, 0), new TimeSpan(9, 50, 0)),
                20, 2025, 1, "Sala 5"
            ),

            // Teologia Básica
            new Turma(
                cursos[5].Id, prof6.Id,
                DiaSemana.Sabado,
                new HorarioAula(new TimeSpan(10, 0, 0), new TimeSpan(10, 50, 0)),
                30, 2025, 1, "Auditório"
            )
        };

        await context.Turmas.AddRangeAsync(turmas);
        await context.SaveChangesAsync();

        // === 8. CRIAR MATRÍCULAS ===
        var matriculas = new[]
        {
            new Matricula(aluno1.Id, turmas[0].Id), // Lucas - Teclado
            new Matricula(aluno2.Id, turmas[2].Id), // Julia - Violão
            new Matricula(aluno3.Id, turmas[4].Id), // Pedro - Bateria
            new Matricula(aluno4.Id, turmas[5].Id), // Mariana - Canto
            new Matricula(aluno5.Id, turmas[7].Id), // Gabriel - Teologia
            new Matricula(aluno6.Id, turmas[6].Id), // Beatriz - Teoria Musical
        };

        // Gerar mensalidades para cada matrícula
        foreach (var matricula in matriculas)
        {
            var turma = turmas.First(t => t.Id == matricula.TurmaId);
            var curso = cursos.First(c => c.Id == turma.CursoId);
            matricula.GerarMensalidades(12, curso.ValorMensalidade, 10);
        }

        await context.Matriculas.AddRangeAsync(matriculas);
        await context.SaveChangesAsync();

        // === 9. CRIAR ALGUMAS AULAS E PRESENÇAS ===
        var aulas = new List<Aula>();
        var dataBase = new DateTime(2025, 2, 3); // Primeira segunda-feira de fevereiro

        // Criar 4 aulas para cada turma
        foreach (var turma in turmas.Take(4)) // Primeiras 4 turmas apenas
        {
            for (int i = 0; i < 4; i++)
            {
                var dataAula = ObterProximaDataAula(dataBase.AddDays(i * 7), turma.DiaSemana);
                var aula = new Aula(turma.Id, dataAula, $"Aula {i + 1}");
                aula.MarcarComoRealizada();
                aulas.Add(aula);
            }
        }

        await context.Aulas.AddRangeAsync(aulas);
        await context.SaveChangesAsync();

        // Registrar presenças
        var presencas = new List<Presenca>();
        var random = new Random();

        foreach (var aula in aulas)
        {
            var matriculasTurma = matriculas.Where(m => m.TurmaId == aula.TurmaId);

            foreach (var matricula in matriculasTurma)
            {
                var status = random.Next(10) < 8 ? StatusPresenca.Presente : StatusPresenca.Falta;
                var presenca = new Presenca(aula.Id, matricula.AlunoId, status);
                presencas.Add(presenca);
            }
        }

        await context.Presencas.AddRangeAsync(presencas);

        // === 10. CRIAR AVALIAÇÕES E NOTAS ===
        var avaliacoes = new List<Avaliacao>();

        foreach (var turma in turmas.Take(4))
        {
            var av1 = new Avaliacao(turma.Id, "Avaliação 1", 1, new DateTime(2025, 3, 15));
            var av2 = new Avaliacao(turma.Id, "Avaliação 2", 1, new DateTime(2025, 5, 15));
            avaliacoes.Add(av1);
            avaliacoes.Add(av2);
        }

        await context.Avaliacoes.AddRangeAsync(avaliacoes);
        await context.SaveChangesAsync();

        // Lançar notas
        var notas = new List<Nota>();

        foreach (var avaliacao in avaliacoes)
        {
            var matriculasTurma = matriculas.Where(m => m.TurmaId == avaliacao.TurmaId);

            foreach (var matricula in matriculasTurma)
            {
                var valorNota = (decimal)(random.NextDouble() * 4 + 6); // Notas entre 6 e 10
                var nota = new Nota(avaliacao.Id, matricula.AlunoId, Math.Round(valorNota, 1));
                notas.Add(nota);
            }
        }

        await context.Notas.AddRangeAsync(notas);

        // === 11. SIMULAR ALGUNS PAGAMENTOS ===
        var pagamentos = new List<Pagamento>();

        // Pagamento do responsável 1 (José)
        var pagamento1 = new Pagamento(
            resp1.Id,
            new Dinheiro(300), // 2 mensalidades
            MeioPagamento.Pix,
            DateTime.Today.AddDays(-5),
            $"PIX{Guid.NewGuid().ToString()[..8]}",
            null
        );

        // Alocar para mensalidades dos filhos
        var mensalidadesResp1 = await context.Mensalidades
            .Where(m => m.Matricula.AlunoId == aluno1.Id || m.Matricula.AlunoId == aluno2.Id)
            .OrderBy(m => m.DataVencimento)
            .Take(2)
            .ToListAsync();

        foreach (var mensalidade in mensalidadesResp1)
        {
            pagamento1.AlocarParaMensalidade(mensalidade.Id, new Dinheiro(150));
            mensalidade.RegistrarPagamento(MeioPagamento.Pix);
        }

        pagamentos.Add(pagamento1);

        // Pagamento do responsável 2 (Maria)
        var pagamento2 = new Pagamento(
            resp2.Id,
            new Dinheiro(200),
            MeioPagamento.Dinheiro,
            DateTime.Today.AddDays(-10)
        );

        var mensalidadesResp2 = await context.Mensalidades
            .Where(m => m.Matricula.AlunoId == aluno3.Id)
            .OrderBy(m => m.DataVencimento)
            .Take(1)
            .ToListAsync();

        foreach (var mensalidade in mensalidadesResp2)
        {
            pagamento2.AlocarParaMensalidade(mensalidade.Id, new Dinheiro(180));
            mensalidade.RegistrarPagamento(MeioPagamento.Dinheiro);
        }

        // Sobra de 20 reais fica como crédito
        resp2.AdicionarCredito(20);

        pagamentos.Add(pagamento2);

        await context.Pagamentos.AddRangeAsync(pagamentos);
        await context.SaveChangesAsync();

        Console.WriteLine("====================================");
        Console.WriteLine("🌱 SEED DATA CRIADO COM SUCESSO!");
        Console.WriteLine("====================================");
        Console.WriteLine("📧 CREDENCIAIS DE TESTE:");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("ADMIN:");
        Console.WriteLine("  Email: admin@institutovirtus.com.br");
        Console.WriteLine("  Senha: Admin@123");
        Console.WriteLine("");
        Console.WriteLine("COORDENAÇÃO:");
        Console.WriteLine("  Email: tairiny@institutovirtus.com.br");
        Console.WriteLine("  Senha: Coord@123");
        Console.WriteLine("");
        Console.WriteLine("PROFESSOR:");
        Console.WriteLine("  Email: joao.silva@institutovirtus.com.br");
        Console.WriteLine("  Senha: Prof@123");
        Console.WriteLine("");
        Console.WriteLine("RESPONSÁVEL:");
        Console.WriteLine("  Email: jose.oliveira@gmail.com");
        Console.WriteLine("  Senha: Resp@123");
        Console.WriteLine("");
        Console.WriteLine("ALUNO (maior de idade):");
        Console.WriteLine("  Email: gabriel.costa@gmail.com");
        Console.WriteLine("  Senha: Aluno@123");
        Console.WriteLine("====================================");
    }

    private static DateTime ObterProximaDataAula(DateTime dataBase, DiaSemana diaSemana)
    {
        var diasSemana = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                 DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday,
                                 DayOfWeek.Sunday };

        var targetDayOfWeek = diasSemana[(int)diaSemana - 1];

        while (dataBase.DayOfWeek != targetDayOfWeek)
        {
            dataBase = dataBase.AddDays(1);
        }

        return dataBase;
    }
}