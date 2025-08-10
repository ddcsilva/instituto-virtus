using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.ValueObjects;
using InstitutoVirtus.Infrastructure.Data.Context;

namespace InstitutoVirtus.API.Extensions;

public static class SeedData
{
    public static async Task InitializeAsync(VirtusDbContext context)
    {
        // Verificar se já existem dados
        if (context.Cursos.Any())
            return;

        // Criar cursos
        var cursos = new[]
        {
            new Curso("Teclado", "Curso de teclado para iniciantes e avançados", 150, 40),
            new Curso("Violão", "Curso de violão popular e clássico", 120, 40),
            new Curso("Bateria", "Curso de bateria", 180, 40),
            new Curso("Canto", "Curso de técnica vocal", 100, 30),
            new Curso("Teoria Musical", "Teoria e percepção musical", 80, 20),
            new Curso("Teologia Básica", "Introdução à teologia", 100, 60)
        };

        await context.Cursos.AddRangeAsync(cursos);

        // Criar professores
        var professores = new[]
        {
            new Professor(
                "João Silva",
                new Telefone("11999888777"),
                new Email("joao.silva@institutovirtus.com.br"),
                new DateTime(1985, 5, 15),
                "Teclado e Piano"),
            new Professor(
                "Maria Santos",
                new Telefone("11999777666"),
                new Email("maria.santos@institutovirtus.com.br"),
                new DateTime(1990, 8, 22),
                "Violão e Guitarra"),
            new Professor(
                "Pedro Costa",
                new Telefone("11999666555"),
                new Email("pedro.costa@institutovirtus.com.br"),
                new DateTime(1988, 3, 10),
                "Bateria e Percussão")
        };

        await context.Professores.AddRangeAsync(professores);

        await context.SaveChangesAsync();
    }
}
