# CleanStore

Uma aplicação de exemplo implementando **Clean Architecture** com **.NET 10** e **C# 14**, focando em boas práticas de design e padrões de desenvolvimento.

## 📋 Visão Geral

CleanStore é uma aplicação que demonstra a implementação de princípios de Clean Architecture, separando claramente as responsabilidades entre camadas e facilitando a manutenção, testabilidade e escalabilidade do código.

### Arquitetura em Camadas

```
┌─────────────────────────────────────┐
│    CleanStore.API                   │ ← Camada de Apresentação
│ (Endpoints, Configurações)          │
├─────────────────────────────────────┤
│    CleanStore.Application           │ ← Camada de Aplicação
│ (Use Cases, Commands, Queries)      │
├─────────────────────────────────────┤
│    CleanStore.Domain                │ ← Camada de Domínio
│ (Entities, Value Objects, Rules)    │
├─────────────────────────────────────┤
│    CleanStore.Infrastructure        │ ← Camada de Infraestrutura
│ (Data Access, Repositories)         │
└─────────────────────────────────────┘
```

## 🏗️ Estrutura do Projeto

### CleanStore.Domain
Contém as **regras de negócio** e especificações do domínio, independente de qualquer framework.

- **AccountContext/Entities**: Entidade `Account` com agregação de root
- **AccountContext/ValueObjects**: `Email` como value object imutável
- **AccountContext/Events**: Eventos de domínio como `OnAccountCreatedEvent`
- **AccountContext/Exceptions**: Exceções de negócio customizadas
- **SharedContext**: Abstrações compartilhadas (Entity, ValueObject, Events)

### CleanStore.Application
Orquestra as operações de negócio e implementa os padrões CQRS e MediatR.

- **AccountContext/UseCases**: Separação entre Commands e Queries
  - **Create**: Comando para criar nova conta
  - **Get**: Query para obter conta por ID
- **AccountContext/Repositories/Abstractions**: `IAccountRepository` (dependency inversion)
- **SharedContext/Results**: `Result<T>` e `Error` para tratamento de erros
- **SharedContext/Behaviors**: Middlewares de Pipeline (Logging, Validation)
- **SharedContext/Specifications**: Pattern para queries dinâmicas

### CleanStore.Infrastructure
Implementação de detalhes técnicos e integração com serviços externos.

- **AccountContext/Repositories**: `AccountRepository` com Entity Framework Core
- **SharedContext/Data**: `AppDbContext` para acesso aos dados
- **Migrations**: Versionamento do schema do banco de dados

### CleanStore.API
Camada de apresentação com endpoints REST.

- **Program.cs**: Configuração da aplicação e injeção de dependências
- **appsettings.json**: Configurações de ambiente
- **Properties/launchSettings.json**: Configurações de execução local

## 🚀 Tecnologias Utilizadas

| Tecnologia | Versão | Propósito |
|------------|--------|----------|
| .NET | 10.0 | Framework base |
| C# | 14.0 | Linguagem |
| Entity Framework Core | Latest | ORM e Data Access |
| MediatR | Latest | CQRS Pattern |
| PostgreSQL | Latest | Banco de dados |
| FluentValidation | Latest | Validação de dados |

## 🔧 Padrões de Projeto

### CQRS (Command Query Responsibility Segregation)
Separação clara entre operações de escrita (Commands) e leitura (Queries):

```csharp
// Command para criar conta
public class Command : ICommand<Response>
{
	public string Email { get; set; }
}

// Query para obter conta
public class Query : IQuery<Response>
{
	public Guid Id { get; set; }
}
```

### MediatR
Implementa o padrão de mediador para desacoplamento entre a API e os handlers:

```csharp
// No endpoint
var result = await sender.Send(new Query(accountId));
```

### Specification Pattern
Encapsula critérios de query de forma reutilizável:

```csharp
var account = await repository.GetByIdAsync(
	new GetByIdSpecification(request.Id)
);
```

### Result Pattern
Tratamento de erros sem exceções, retornando sempre um resultado:

```csharp
return Result.Success(new Response(...));
return Result.Failure<Response>(new Error("404", "Conta não encontrada"));
```

### Value Objects
Encapsulamento de conceitos do domínio com validação:

```csharp
public sealed class Email : ValueObject
{
	public string Address { get; private set; }
	// Validação e regras de negócio encapsuladas
}
```

### Domain Events
Eventos que capturam mudanças significativas no domínio:

```csharp
account.RaiseDomainEvent(new OnAccountCreatedEvent(id, email));
```

## 📝 Endpoints da API

### Criar Conta
```http
POST /accounts
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Resposta (201 Created)**
```json
{
  "isSuccess": true,
  "value": {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"email": "user@example.com"
  }
}
```

### Obter Conta por ID
```http
GET /accounts/{id}
```

**Resposta (200 OK)**
```json
{
  "isSuccess": true,
  "value": {
	"id": "550e8400-e29b-41d4-a716-446655440000",
	"email": "user@example.com"
  }
}
```

**Resposta (404 Not Found)**
```json
{
  "isSuccess": false,
  "error": {
	"code": "404",
	"message": "Conta não encontrada"
  }
}
```

## 🚀 Como Executar

### Pré-requisitos
- .NET 10 SDK instalado
- PostgreSQL 14+ em execução
- Git

### Passos de Instalação

1. **Clone o repositório**
```bash
git clone https://github.com/rodolfoms/CleanStore.git
cd CleanStore
```

2. **Configure a string de conexão**
Edite `CleanStore.API/appsettings.json` ou `Program.cs`:
```
Host=localhost;Port=5432;Database=cleanstore;Username=postgres;Password=sua_senha
```

3. **Aplique as migrações**
```bash
cd CleanStore.API
dotnet ef database update
```

4. **Execute a aplicação**
```bash
dotnet run
```

A API estará disponível em `https://localhost:5000` (ou a porta configurada).

## 🧪 Testes

A solução segue as melhores práticas de teste com estrutura preparada para:

- **Unit Tests**: Testes das entidades de domínio e value objects
- **Integration Tests**: Testes dos repositories e banco de dados
- **Use Case Tests**: Testes dos handlers de commands e queries

_(Framework de testes a ser adicionado na próxima iteration)_

## 📚 Estrutura de Pastas

```
CleanStore/
├── CleanStore.Domain/
│   └── AccountContext/
│       ├── Entities/
│       ├── Events/
│       ├── ValueObjects/
│       └── Exceptions/
├── CleanStore.Application/
│   └── AccountContext/
│       ├── UseCases/
│       │   ├── Create/
│       │   └── Get/
│       └── Repositories/
├── CleanStore.Infrastructure/
│   └── AccountContext/
│       └── Repositories/
├── CleanStore.API/
│   ├── Program.cs
│   ├── Migrations/
│   └── appsettings.json
└── README.md
```

## 🔐 Segurança

- ✅ Validação de entrada em todos os endpoints
- ✅ Value Objects imutáveis
- ✅ Regras de negócio encapsuladas no domínio
- ✅ Tratamento estruturado de erros
- ✅ Sem exposição de detalhes técnicos

## 🎯 Princípios Aplicados

| Princípio | Implementação |
|-----------|--------------|
| SOLID | Separação de responsabilidades por camada |
| DRY | Abstrações compartilhadas no SharedContext |
| KISS | Código simples e focado |
| Clean Code | Nomes descritivos e estrutura clara |
| Domain-Driven Design | Linguagem ubíqua e domínio rico |

## 📖 Recursos Úteis

- [Microsoft - Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)

## 🤝 Contribuindo

Este é um projeto de estudo e referência. Sugestões de melhorias são bem-vindas!

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/NovaFeature`)
3. Commit suas mudanças (`git commit -m 'Add NovaFeature'`)
4. Push para a branch (`git push origin feature/NovaFeature`)
5. Abra um Pull Request

## 📄 Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais informações.

## 👨‍💻 Autor

**Rodolfo Sobreira**
- GitHub: [@rodolfoms](https://github.com/rodolfoms)
- Repositório: [CleanStore](https://github.com/rodolfoms/CleanStore)

---

## 📝 Changelog

### [1.0.0] - 2026-11-18
- ✨ Implementação inicial da arquitetura Clean
- ✨ Use case de criação de conta
- ✨ Use case de obtenção de conta por ID
- 🔧 Configuração de banco de dados PostgreSQL
- 📚 Documentação e estrutura base

---

**Desenvolvido com ❤️ usando .NET 10 e C# 14**
