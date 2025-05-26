# Instituto Virtus 🎵📖

## Sistema de Gestão Acadêmica

Plataforma completa para gestão de cursos de música e teologia!

[.NET](https://img.shields.io/badge/.NET-8-512BD4?style=flat-square&logo=dotnet)![Angular](https://img.shields.io/badge/Angular-18-DD0031?style=flat-square&logo=angular)![License](https://img.shields.io/badge/License-Private-red?style=flat-square)

---

## 📋 Sobre o Projeto

O **Instituto Virtus** é um sistema de gestão acadêmica desenvolvido
especificamente para instituições de ensino de música e teologia. A
plataforma oferece controle completo sobre alunos, turmas, matrículas e
pagamentos.

### ✨ Principais Funcionalidades

- 👥 **Gestão de Alunos** - Cadastro e controle completo de estudantes
- 🎓 **Gestão de Turmas** - Organização e administração de classes
- 📝 **Sistema de Matrículas** - Processo automatizado de inscrições
- ⏰ **Lista de Espera** - Gerenciamento inteligente de vagas
- 💰 **Controle Financeiro** - Acompanhamento de pagamentos e mensalidades

---

## 🛠️ Tecnologias Utilizadas

### Backend

- **Framework:** .NET 8
- **Arquitetura:** Clean Architecture
- **ORM:** Entity Framework Core
- **Banco de Dados:**
  - SQLite (desenvolvimento)
  - PostgreSQL (produção)

### Frontend

- **Framework:** Angular 18
- **Estilização:** TailwindCSS
- **Linguagem:** TypeScript

---

## 🚀 Começando

### 📋 Pré-requisitos

Certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Angular CLI 18](https://angular.io/cli)

### ⚡ Instalação Rápida

1. **Clone o repositório**

   ```bash
   git clone https://github.com/seu-usuario/instituto-virtus.git
   cd instituto-virtus
   ```

2. **Configure o Backend**

   ```bash
   cd backend
   dotnet restore
   dotnet build
   ```

3. **Configure o Frontend**

   ```bash
   cd frontend
   npm install
   ```

4. **Execute o projeto**

   ```bash
   # Terminal 1 - Backend
   cd backend
   dotnet run

   # Terminal 2 - Frontend
   cd frontend
   ng serve
   ```

5. **Acesse a aplicação**
   - Frontend: `http://localhost:4200`
   - API: `http://localhost:5000`

---

## 📁 Estrutura do Projeto

```text
instituto-virtus/
├── 📂 backend/                 # API .NET 8
│   ├── 📂 src/
│   │   ├── 📂 Core/           # Domínio e aplicação
│   │   ├── 📂 Infrastructure/ # Infraestrutura e dados
│   │   └── 📂 WebAPI/         # Controladores e endpoints
│   └── 📂 tests/              # Testes unitários
├── 📂 frontend/               # Aplicação Angular 18
│   ├── 📂 src/
│   │   ├── 📂 app/           # Componentes e módulos
│   │   ├── 📂 assets/        # Recursos estáticos
│   │   └── 📂 environments/  # Configurações de ambiente
└── 📂 docs/                   # Documentação completa
```

---

## 🎯 Roadmap

### 🚧 Fase 1 - MVP (Em desenvolvimento)

- [x] Estrutura base do projeto
- [x] Configuração do ambiente
- [ ] Cadastro de alunos
- [ ] Gestão básica de turmas
- [ ] Sistema de matrículas

### 🔮 Fase 2 - Funcionalidades Avançadas

- [ ] Sistema de pagamentos
- [ ] Relatórios e dashboards
- [ ] Notificações automáticas
- [ ] Lista de espera inteligente

### 🌟 Fase 3 - Melhorias

- [ ] Interface mobile responsiva
- [ ] Sistema de backup automático
- [ ] Integração com sistemas de pagamento
- [ ] API pública para integrações

---

## 📚 Documentação

Para documentação detalhada, acesse a pasta [`docs/`](./docs/):

- [📖 Visão Geral](./docs/visao-geral.md)
- [🏗️ Arquitetura](./docs/arquitetura.md)
- [🔌 Documentação da API](./docs/api.md)
- [🗄️ Banco de Dados](./docs/banco-dados.md)
- [🚀 Deploy](./docs/deploy.md)

---

## 🤝 Contribuição

Este é um projeto privado do Instituto Virtus. Para sugestões ou melhorias,
entre em contato com a equipe de desenvolvimento.

---

## 👨‍💻 Equipe

**Danilo Silva** - _Desenvolvedor Full Stack_

---

## 📄 Licença

Este projeto é propriedade privada do **Instituto Virtus**.
Todos os direitos reservados.

---

## ❤️ Agradecimentos

Feito com ❤️ para o Instituto Virtus

© 2025 Instituto Virtus. Todos os direitos reservados.
