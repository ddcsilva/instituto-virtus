# Virtus.Domain.Tests 🧪

Projeto de testes unitários para o domínio do sistema Virtus, utilizando XUnit, FluentAssertions e outras ferramentas para garantir qualidade e robustez do código.

## 🗂️ Estrutura do Projeto

```
Virtus.Domain.Tests/
├── 📁 Builders/                    # Test Data Builders
│   └── TestBuilders.cs            # Builders para criação de objetos de teste
├── 📁 Entities/                   # Testes das entidades do domínio
│   ├── AlunoTests.cs              # Testes da entidade Aluno
│   ├── BaseEntityTests.cs         # Testes da classe base BaseEntity
│   ├── MatriculaTests.cs          # Testes da entidade Matricula
│   ├── PagamentoTests.cs          # Testes da entidade Pagamento
│   ├── PagamentoAlunoTests.cs     # Testes da entidade PagamentoAluno
│   ├── PessoaTests.cs             # Testes da entidade Pessoa
│   ├── ProfessorTests.cs          # Testes da entidade Professor
│   └── TurmaTests.cs              # Testes da entidade Turma
├── 📁 ValueObjects/               # Testes dos Value Objects
│   └── EmailTests.cs              # Testes do Value Object Email
├── 📁 Helpers/                    # Utilitários para testes
│   └── TestHelpers.cs             # Métodos auxiliares para testes
├── Virtus.Domain.Tests.csproj     # Arquivo de projeto
├── xunit.runner.json              # Configurações do XUnit
└── README.md                      # Esta documentação
```

## 🛠️ Tecnologias e Ferramentas

### Frameworks de Teste
- **XUnit** - Framework de testes principal
- **FluentAssertions** - Assertions mais legíveis e expressivas
- **AutoFixture** - Geração automática de dados de teste
- **Bogus** - Geração de dados fake realistas

### Coverage e Análise
- **Coverlet** - Análise de cobertura de código
- **Microsoft.NET.Test.Sdk** - SDK de testes do .NET

## 🚀 Como Executar os Testes

### Pré-requisitos
- .NET 9.0 SDK
- IDE com suporte a XUnit (Visual Studio, Rider, VS Code)

### Comandos Básicos

```bash
# Restaurar dependências
dotnet restore

# Executar todos os testes
dotnet test

# Executar testes com output detalhado
dotnet test --verbosity normal

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos por filtro
dotnet test --filter "FullyQualifiedName~PessoaTests"

# Executar testes de uma categoria específica
dotnet test --filter "Category=Entity"
```

### Coverage Report

```bash
# Gerar relatório de cobertura em HTML
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Instalar ferramenta de relatório (primeira vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Gerar relatório HTML
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/html" -reporttypes:Html
```

## 📝 Padrões de Teste

### Nomenclatura
```csharp
[Fact]
public void MetodoOuComportamento_DeveRetornarOuFazer_QuandoCondicao()
{
    // Arrange
    // Act  
    // Assert
}
```

### Estrutura AAA (Arrange-Act-Assert)
```csharp
[Fact]
public void Construtor_DeveInicializarCorretamente_QuandoParametrosValidos()
{
    // Arrange - Preparar dados e configurar cenário
    var nome = "João Silva";
    var email = Email.Criar("joao@email.com");
    
    // Act - Executar a ação sendo testada
    var pessoa = new Pessoa(nome, email, telefone, tipo);
    
    // Assert - Verificar o resultado
    pessoa.Nome.Should().Be(nome);
    pessoa.Email.Should().Be(email);
}
```

### Test Data Builders
Use os builders para criar objetos de teste de forma fluente:

```csharp
// Builder básico
var aluno = AlunoBuilder.Novo().Build();

// Builder com customizações
var aluno = AlunoBuilder.Novo()
    .ComStatus(StatusAluno.Ativo)
    .ComResponsavel(responsavel)
    .Build();

// Conversão implícita
Aluno aluno = AlunoBuilder.Novo().ComStatus(StatusAluno.Inativo);
```

## 🎯 Cobertura de Testes

### Cenários Cobertos

#### ✅ Casos de Sucesso
- Construção válida de objetos
- Métodos funcionando corretamente
- Propriedades sendo definidas adequadamente
- Relacionamentos funcionais

#### ⚠️ Casos de Erro
- Parâmetros nulos
- Valores inválidos
- Estados inconsistentes
- Regras de negócio violadas

#### 🔄 Casos de Transição
- Mudanças de estado
- Atualizações de propriedades
- Operações de ativação/inativação

### Principais Validações

- **Validação de parâmetros** - Nulls, vazios, inválidos
- **Regras de negócio** - Lógicas específicas do domínio
- **Encapsulamento** - Propriedades private set, métodos protegidos
- **Herança** - Comportamento de BaseEntity
- **Imutabilidade** - Value objects e dados protegidos
- **Relacionamentos** - Associações entre entidades

## 🏃‍♂️ Execução Contínua

### Scripts Úteis

**run-tests.ps1** (PowerShell)
```powershell
# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --logger "console;verbosity=detailed"

# Gerar relatório HTML se covertura > 80%
$coverage = dotnet test --collect:"XPlat Code Coverage" | Select-String "Line coverage"
if ($coverage -match "(\d+\.\d+)%") {
    $percentage = [decimal]$matches[1]
    if ($percentage -ge 80) {
        Write-Host "Coverage: $percentage% ✅" -ForegroundColor Green
        reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/html" -reporttypes:Html
    } else {
        Write-Host "Coverage: $percentage% ❌ (mínimo: 80%)" -ForegroundColor Red
        exit 1
    }
}
```

**run-tests.sh** (Bash)
```bash
#!/bin/bash
echo "🧪 Executando testes do domínio..."

# Limpar resultados anteriores
rm -rf TestResults

# Executar testes
dotnet test --collect:"XPlat Code Coverage" --logger "console;verbosity=detailed"

# Verificar cobertura
echo "📊 Gerando relatório de cobertura..."
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/html" -reporttypes:Html

echo "✅ Testes concluídos! Relatório disponível em TestResults/html/index.html"
```

## 🎭 Mocks e Stubs

Para este projeto de domínio, focamos em **testes unitários puros** sem dependências externas:

- **Sem mocks de repositórios** - Testamos apenas a lógica de domínio
- **Builders ao invés de mocks** - Criação de objetos reais para teste
- **Foco no comportamento** - Testamos regras de negócio, não infraestrutura

## 📈 Métricas de Qualidade

### Objetivos de Cobertura
- **Mínimo:** 80% de cobertura de linha
- **Ideal:** 90%+ de cobertura de linha
- **Objetivo:** 95%+ de cobertura de branch

### Qualidade dos Testes
- Testes rápidos (< 1s cada)
- Testes independentes (sem side effects)
- Testes legíveis e bem nomeados
- Cobertura de casos edge

## 🚨 Troubleshooting

### Problemas Comuns

**Testes falhando por timing**
```csharp
// ❌ Problemático
Assert.Equal(DateTime.UtcNow, entidade.DataCriacao);

// ✅ Correto
entidade.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
```

**Reflection para propriedades privadas**
```csharp
// Use os helpers do TestHelpers.cs
TestHelpers.DefinirId(entidade, 123);
TestHelpers.SimularCarregamentoDoBanco(entidade, 123, DateTime.UtcNow.AddDays(-1));
```

**Builders não funcionando**
```csharp
// Verifique se está usando o namespace correto
using Virtus.Domain.Tests.Builders;

// E que o builder está sendo usado corretamente
var aluno = AlunoBuilder.Novo().Build(); // ✅
```

## 🔗 Links Úteis

- [XUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [AutoFixture Documentation](https://github.com/AutoFixture/AutoFixture)
- [Bogus Documentation](https://github.com/bchavez/Bogus)
- [Test Naming Guidelines](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

## 📞 Suporte

Para dúvidas sobre os testes ou problemas na execução:

1. Verifique se todas as dependências estão instaladas
2. Confira se o .NET 9.0 SDK está instalado
3. Execute `dotnet restore` no diretório do projeto de testes
4. Consulte os logs detalhados com `dotnet test --verbosity detailed`

**Lembre-se:** Testes são documentação viva do código! 📚✨
