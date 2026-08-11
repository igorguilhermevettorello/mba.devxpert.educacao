# PlataformaEducacional

Plataforma Educacional Enterprise baseada em microsserviços com **.NET 8**, desenvolvida pelo **Grupo 5 - MBA DevIO (Módulo 4)**.

---

## 📋 Sobre o projeto

A solução segue arquitetura distribuída com:

- **Microsserviços**
- **DDD (Domain-Driven Design)**
- **CQRS (MediatR)**
- **Mensageria assíncrona com RabbitMQ**

Estrutura principal:

```text
PlataformaEducacional/
├── src/
│   ├── buildingBlocks/
│   ├── services/
│   └── tools/
```

---

## 🏗️ Arquitetura da solução

### Building Blocks (compartilhados)

- `PlataformaEducacional.Core`  
  Abstrações base, entidades, value objects e mensagens de integração.

- `PlataformaEducacional.MessageBus`  
  Integração com RabbitMQ (EasyNetQ) e resiliência com Polly.

- `PlataformaEducacional.WebApi.Core`  
  Configurações comuns de API, middleware, autenticação JWT e Swagger.

### Ferramenta utilitária

- `GenerateJwtSigningKey`  
  Gera chave privada RSA (`.pem`) para assinatura JWT (Auth API).

---

## 🚀 Microsserviços

### 1) Auth API

- Projeto: `src/services/auth/PlataformaEducacional.Auth.Api`
- URL: `https://localhost:5001`
- Responsabilidade: autenticação/autorização, registro de usuários e emissão de JWT
- Stack: ASP.NET Core Identity, EF Core, RabbitMQ
- Banco: `PeIdentidade`

### 2) Alunos API

- Projeto: `src/services/alunos/PlataformaEducacional.Alunos.Api`
- URL: `https://localhost:44360`
- Responsabilidade: gestão de alunos e dados acadêmicos
- Stack: EF Core, MediatR, RabbitMQ, JWT
- Banco: `PeAlunos`

### 3) Conteúdo API

- Projeto: `src/services/conteudo/PlataformaEducacional.Conteudo.Api`
- URL: `https://localhost:7077`
- Responsabilidade: cursos, módulos, aulas e materiais
- Stack: EF Core, MediatR, AutoMapper, JWT
- Banco: `PeConteudo`

### 4) Pagamentos API

- Projeto: `src/services/pagamentos/PlataformaEducacional.Pagamentos.Api`
- URL: `https://localhost:44342`
- Responsabilidade: transações e integração com EducaPag
- Stack: EF Core, MediatR, RabbitMQ, AutoMapper, JWT
- Banco: `PePagamentos`

### 5) BFF API

- Projeto: `src/services/bff/PlataformaEducacional.Bff.Api`
- URL: `https://localhost:5003`
- Responsabilidade: camada de agregação/proxy para frontend
- Stack: ASP.NET Core Web API, MediatR, JWT
- Banco: sem banco próprio

---

## 🛠️ Stack tecnológica

- `.NET 8` / `C# 12`
- `ASP.NET Core 8`
- `Entity Framework Core 8`
- `SQL Server / LocalDB`
- `RabbitMQ + EasyNetQ`
- `MediatR`
- `AutoMapper`
- `FluentValidation`
- `Swagger/OpenAPI`

---

## ⚙️ Configuração do ambiente local

### Pré-requisitos

- `.NET SDK 8+`
- `Visual Studio 2022 (17.8+)` ou VS Code
- `SQL Server` ou `LocalDB`
- `RabbitMQ`
- `Git`

### 1) Clonar e restaurar

```bash
git clone https://github.com/igorguilhermevettorello/mba.devxpert.educacao.git
cd mba.devxpert.educacao
dotnet restore
```

### 2) Banco de dados

Bancos usados em desenvolvimento:

- `PeIdentidade`
- `PeAlunos`
- `PeConteudo`
- `PePagamentos`

As APIs com camada de dados aplicam migration no startup com `UseDatabaseMigrationStartData()`.

### 3) RabbitMQ

Configuração padrão:

```json
"MessageQueueConnection": {
  "MessageBus": "host=localhost:5672;publisherConfirms=true;timeout=10"
}
```

Management:

- `http://localhost:15672`
- usuário/senha: `guest` / `guest`

### 4) JWT (RSA)

Gerar chave:

```bash
cd src/tools/GenerateJwtSigningKey
dotnet run
```

Caminho esperado (Auth API):

- `keys/educacao-private.pem`

Exemplo de configuração na Auth API:

```json
"JwtSettings": {
  "Authority": "https://localhost:5001",
  "ExpiracaoHoras": 1,
  "Emissor": "https://localhost:5001",
  "ValidoEm": "https://localhost",
  "SigningKeyPath": "keys/educacao-private.pem",
  "SigningKeyId": "dev-key-1"
}
```

---

## ▶️ Execução local

Ordem recomendada:

1. RabbitMQ
2. Auth API
3. Alunos API
4. Conteúdo API
5. Pagamentos API
6. BFF API

Execução manual:

```bash
cd src/services/auth/PlataformaEducacional.Auth.Api && dotnet run
cd src/services/alunos/PlataformaEducacional.Alunos.Api && dotnet run
cd src/services/conteudo/PlataformaEducacional.Conteudo.Api && dotnet run
cd src/services/pagamentos/PlataformaEducacional.Pagamentos.Api && dotnet run
cd src/services/bff/PlataformaEducacional.Bff.Api && dotnet run
```

Swagger:

- Auth: `https://localhost:5001/swagger`
- Alunos: `https://localhost:44360/swagger`
- Conteúdo: `https://localhost:7077/swagger`
- Pagamentos: `https://localhost:44342/swagger`
- BFF: `https://localhost:5003/swagger`

---

## 🐳 Docker

Se o arquivo `docker-compose-local.yml` estiver disponível:

```bash
docker-compose -f .\docker-compose-local.yml up --build
docker-compose -f .\docker-compose-local.yml down
```

---

## ⚠️ Problema conhecido

### `TaskCanceledException` no fluxo Auth → Alunos (RPC)

No registro de aluno, a Auth API usa requisição/resposta via RabbitMQ. Pode ocorrer timeout quando:

- Alunos API não está em execução
- RabbitMQ indisponível
- inicialização do consumer atrasada

Ações recomendadas:

1. Confirmar RabbitMQ ativo
2. Confirmar Alunos API iniciada e conectada ao broker
3. Preferir chamada assíncrona com timeout explícito e log estruturado

---

## 🔐 Segurança

- APIs usam autenticação JWT Bearer
- Assinatura JWT é **RSA** (chave privada PEM na Auth API)
- Não versionar chaves privadas no repositório
- Em produção, usar cofre de segredos (ex.: Azure Key Vault)

---

## 📦 Build e publish

```bash
dotnet build
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

---

## 👥 Equipe

- Cleber: Auth / Segurança
- Igor: Conteúdo / DDD / CQRS
- Gustavo: Alunos
- Lucas / Rafael: Pagamentos

---

## 📌 Próximos passos

- Health checks de banco, RabbitMQ e dependências
- Testes automatizados (unitário e integração)
- CI/CD
- Observabilidade
- Versionamento de API e rate limiting