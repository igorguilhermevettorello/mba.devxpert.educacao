# PlataformaEducacional

## 📋 Sobre o Projeto

**Plataforma Educacional Enterprise** é uma solução de microsserviços desenvolvida em .NET 8 para gerenciamento de uma plataforma educacional completa. O projeto segue os princípios de arquitetura distribuída, implementando padrões como DDD (Domain-Driven Design), CQRS e mensageria assíncrona.

Desenvolvido pelo **Grupo 5 - MBA DevIO** como projeto do Módulo 4.

---

## 🏗️ Arquitetura da Solução

A solução está organizada em camadas bem definidas, seguindo princípios SOLID e Clean Architecture:

```
PlataformaEducacional/
├── src/
│   ├── buildingBlocks/          # Componentes compartilhados
│   └── services/                # Microsserviços
```

### 🧱 Building Blocks (Componentes Compartilhados)

Conjunto de bibliotecas reutilizáveis que fornecem funcionalidades comuns para todos os microsserviços:

| Projeto | Descrição |
|---------|-----------|
| **PlataformaEducacional.Core** | Componentes base, entidades, value objects, abstrações de domínio e mensagens de integração |
| **PlataformaEducacional.MessageBus** | Implementação de mensageria com RabbitMQ usando EasyNetQ e políticas de resiliência com Polly |
| **PlataformaEducacional.WebApi.Core** | Configurações compartilhadas de API, filtros, middleware, autenticação JWT e Swagger |

---

## 🚀 Microsserviços

### 1. **Auth API** - Serviço de Autenticação e Autorização

**Responsável**: Cleber

**Descrição**: Gerencia autenticação, autorização e identidade de usuários.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- ASP.NET Core Identity
- JWT Bearer Authentication
- Entity Framework Core 8.0
- SQL Server / LocalDB
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Auth.Api` - API REST

**Porta**: A ser configurada

---

### 2. **Conteúdo API** - Serviço de Gerenciamento de Conteúdo

**Responsável**: Igor

**Descrição**: Gerencia conteúdos educacionais, cursos, aulas e materiais didáticos.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Conteudo.Api` - API REST
- `PlataformaEducacional.Conteudo.Application` - Casos de uso e lógica de aplicação
- `PlataformaEducacional.Conteudo.Data` - Acesso a dados e contexto
- `PlataformaEducacional.Conteudo.Domain` - Entidades e regras de negócio

**Porta**: A ser configurada

---

### 3. **Alunos API** - Serviço de Gerenciamento de Alunos

**Responsável**: Gustavo

**Descrição**: Gerencia informações de alunos, matrículas e progresso acadêmico.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB
- MediatR (CQRS)
- RabbitMQ (Mensageria)
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Alunos.Api` - API REST

**Porta**: A ser configurada

---

### 4. **Pagamentos API** - Serviço de Gestão de Pagamentos

**Responsáveis**: Lucas / Rafael

**Descrição**: Gerencia transações financeiras, pagamentos e cobranças.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Pagamentos.Api` - API REST
- `PlataformaEducacional.Pagamentos.Application` - Casos de uso e lógica de aplicação
- `PlataformaEducacional.Pagamentos.Data` - Acesso a dados e contexto
- `PlataformaEducacional.Pagamentos.Domain` - Entidades e regras de negócio

**Porta**: A ser configurada

---

### 5. **BFF API** - Backend for Frontend

**Descrição**: Agrega e orquestra chamadas aos microsserviços, servindo como gateway para o frontend.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Bff.Api` - API REST

**Porta**: A ser configurada

---

## 🛠️ Tecnologias e Ferramentas

### Core Stack
- **.NET 8.0** - Framework principal
- **C# 12** - Linguagem de programação
- **ASP.NET Core** - Framework Web API

### Banco de Dados
- **SQL Server** - Banco de dados principal
- **LocalDB** - Para desenvolvimento local
- **Entity Framework Core 8.0** - ORM

### Mensageria
- **RabbitMQ** - Message Broker
- **EasyNetQ** - Cliente RabbitMQ simplificado
- **Polly** - Biblioteca de resiliência e retry policies

### Segurança
- **ASP.NET Core Identity** - Gerenciamento de identidade
- **JWT Bearer** - Autenticação baseada em tokens

### Padrões e Bibliotecas
- **MediatR** - Implementação de CQRS e mediator pattern
- **Swagger/OpenAPI** - Documentação de API
- **AutoMapper** - Mapeamento de objetos (quando aplicável)

### Arquitetura
- **Microsserviços** - Arquitetura distribuída
- **DDD** - Domain-Driven Design
- **CQRS** - Command Query Responsibility Segregation
- **Event-Driven** - Comunicação assíncrona baseada em eventos

---

## ⚙️ Configuração do Ambiente

### Pré-requisitos

Certifique-se de ter os seguintes componentes instalados:

- [ ] [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (versão 8.0 ou superior)
- [ ] [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8+) ou [Visual Studio Code](https://code.visualstudio.com/)
- [ ] [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) ou SQL Server LocalDB
- [ ] [RabbitMQ](https://www.rabbitmq.com/download.html) (para mensageria)
- [ ] [Git](https://git-scm.com/downloads)

### Passo a Passo para Configuração

#### 1. Clone o Repositório

```bash
git clone https://github.com/igorguilhermevettorello/mba.devxpert.educacao.git
cd mba.devxpert.educacao
```

#### 2. Restaurar Dependências

```bash
dotnet restore
```

#### 3. Configurar Banco de Dados

Cada microsserviço possui seu próprio banco de dados. Configure as connection strings nos arquivos `appsettings.json` ou `appsettings.Development.json` de cada API:

**Auth API** - `src/services/auth/PlataformaEducacional.Auth.Api/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PeIdentidade;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**Alunos API** - `src/services/alunos/PlataformaEducacional.Alunos.Api/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PeAlunos;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

> **Nota**: Ajuste as connection strings conforme seu ambiente (LocalDB, SQL Server Express ou SQL Server completo).

#### 4. Configurar RabbitMQ

**Instalação do RabbitMQ (Windows)**:
```bash
# Usando Chocolatey
choco install rabbitmq

# Ou baixe diretamente do site oficial
```

**Configuração nos microsserviços** - Nos arquivos `appsettings.json`:
```json
"MessageQueueConnection": {
  "MessageBus": "host=localhost:5672;publisherConfirms=true;timeout=10"
}
```

**Iniciar RabbitMQ**:
```bash
# Windows (como serviço)
rabbitmq-service start

# Ou via linha de comando
rabbitmq-server
```

**Acessar o Management Console**:
- URL: http://localhost:15672
- Usuário padrão: `guest`
- Senha padrão: `guest`

#### 5. Executar Migrations

Para cada microsserviço que possui camada de dados, execute as migrations:

**Auth API**:
```bash
cd src/services/auth/PlataformaEducacional.Auth.Api
dotnet ef database update
```

**Alunos API**:
```bash
cd src/services/alunos/PlataformaEducacional.Alunos.Api
dotnet ef database update
```

> **Nota**: Repita o processo para outros microsserviços conforme necessário.

#### 6. Configurar JWT

Configure as chaves JWT nos arquivos `appsettings.json`:

```json
"JwtSettings": {
  "Secret": "2632D324-BC04-4382-A61D-19C1C7311187",
  "ExpiracaoHoras": 1,
  "Emissor": "MBA.PlataformaEducacional",
  "Audiencia": "https://localhost"
}
```

> ⚠️ **Importante**: Em produção, utilize segredos seguros e armazene em Azure Key Vault ou similar.

#### 7. Executar os Microsserviços

Você pode executar cada microsserviço individualmente:

```bash
# Auth API
cd src/services/auth/PlataformaEducacional.Auth.Api
dotnet run

# Alunos API
cd src/services/alunos/PlataformaEducacional.Alunos.Api
dotnet run

# Conteúdo API
cd src/services/conteudo/PlataformaEducacional.Conteudo.Api
dotnet run

# Pagamentos API
cd src/services/pagamentos/PlataformaEducacional.Pagamentos.Api
dotnet run

# BFF API
cd src/services/bff/PlataformaEducacional.Bff.Api
dotnet run
```

Ou usar o Visual Studio para executar múltiplos projetos de inicialização simultaneamente.

#### 8. Acessar a Documentação Swagger

Após iniciar cada API, acesse a documentação Swagger:

- **Auth API**: `https://localhost:{porta}/swagger`
- **Alunos API**: `https://localhost:{porta}/swagger`
- **Conteúdo API**: `https://localhost:{porta}/swagger`
- **Pagamentos API**: `https://localhost:{porta}/swagger`
- **BFF API**: `https://localhost:{porta}/swagger`

---

## 📦 Build e Deploy

### Build da Solução

```bash
dotnet build
```

### Build em modo Release

```bash
dotnet build --configuration Release
```

### Publicar uma API

```bash
cd src/services/{nome-do-servico}
dotnet publish --configuration Release --output ./publish
```

---

## 🔐 Segurança

- Todos os microsserviços (exceto Auth) utilizam autenticação JWT
- Tokens são validados com chave simétrica configurada
- HTTPS habilitado em todos os endpoints
- Validação de requisições com Data Annotations e FluentValidation

---

## 📚 Documentação Adicional

### Endpoints Principais

A documentação completa dos endpoints está disponível via Swagger em cada microsserviço.

### Padrões de Comunicação

- **Síncrona**: HTTP/REST entre BFF e microsserviços
- **Assíncrona**: RabbitMQ para eventos de integração

---

## 👥 Equipe

**Grupo 5 - MBA DevIO**

| Membro | Responsabilidade |
|--------|------------------|
| Cleber | Auth API |
| Igor | Conteúdo API |
| Gustavo | Alunos API |
| Lucas | Pagamentos API |
| Rafael | Pagamentos API |

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 📞 Contato

- **Email**: grupo5@mbaDev.io
- **Repositório**: [GitHub](https://github.com/igorguilhermevettorello/mba.devxpert.educacao)

---

## 🚧 Status do Projeto

🟢 **Em Desenvolvimento Ativo**

---

## 📌 Notas de Versão

### Versão 1.0.0 (Em desenvolvimento)
- Implementação inicial dos microsserviços
- Integração com RabbitMQ
- Autenticação JWT
- Documentação Swagger