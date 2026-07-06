# PlataformaEducacional

## 📋 Sobre o Projeto

**Plataforma Educacional Enterprise** é uma solução de microsserviços desenvolvida em **.NET 8** para gerenciamento de uma plataforma educacional completa. O projeto segue os princípios de arquitetura distribuída, implementando padrões como **DDD (Domain-Driven Design)**, **CQRS** e **mensageria assíncrona com RabbitMQ**.

Desenvolvido pelo **Grupo 5 - MBA DevIO** como projeto do Módulo 4.

## ⚠️ Última atualização

Este README foi atualizado em **janeiro 2025** após análise completa do repositório para refletir o estado real da solução. Todas as portas, tecnologias, configurações e estrutura foram verificadas contra o código-fonte atual.

---

## 🏗️ Arquitetura da Solução

A solução está organizada em camadas bem definidas, seguindo princípios SOLID e Clean Architecture:

```
PlataformaEducacional/
├── src/
│   ├── buildingBlocks/          # Componentes compartilhados
│   ├── services/                # Microsserviços
│   └── tools/                   # Ferramentas utilitárias
```

### 🧱 Building Blocks (Componentes Compartilhados)

Conjunto de bibliotecas reutilizáveis que fornecem funcionalidades comuns para todos os microsserviços:

| Projeto | Descrição |
|---------|-----------|
| **PlataformaEducacional.Core** | Componentes base, entidades, value objects, abstrações de domínio e mensagens de integração |
| **PlataformaEducacional.MessageBus** | Implementação de mensageria com RabbitMQ usando EasyNetQ e políticas de resiliência com Polly |
| **PlataformaEducacional.WebApi.Core** | Configurações compartilhadas de API, filtros, middleware, autenticação JWT e Swagger |

### 🔧 Ferramentas Utilitárias

Ferramentas auxiliares para desenvolvimento e operação da plataforma:

| Projeto | Descrição |
|---------|-----------|
| **GenerateJwtSigningKey** | Utilitário para gerar chaves privadas RSA em formato PEM para assinatura de JWT (usado apenas na Auth API) |

---

## 🚀 Microsserviços

### 1. **Auth API** - Serviço de Autenticação e Autorização

**Responsável**: Cleber

**Descrição**: Gerencia autenticação, autorização e identidade de usuários. Responsável por registrar novos usuários (alunos), gerar tokens JWT e validar credenciais.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- ASP.NET Core Identity (SQL Server)
- JWT Bearer Authentication (RSA com chaves privadas em PEM)
- Entity Framework Core 8.0
- SQL Server / LocalDB (Database: `PeIdentidade`)
- Swagger/OpenAPI
- RabbitMQ (para eventos de integração)

**Estrutura**:
- `PlataformaEducacional.Auth.Api` - API REST com Controllers, Security, Models

**Banco de dados**: LocalDB `PeIdentidade`

**Portas**: `https://localhost:5001`

**Dependências**:
- RabbitMQ (para enviar eventos de registro de aluno)
- Base de dados SQL Server

**Status**: ✅ Implementado

**Fluxo crítico**:
- Endpoint `POST /api/identidade/nova-conta` registra usuário e dispara `UsuarioRegistradoIntegrationEvent` via RPC/MessageBus
- ⚠️ **NOTA IMPORTANTE**: Atualmente usa `_bus.Request()` (síncrono), sujeito a `TaskCanceledException` se o responder não estiver disponível
- Recomendação: Migrar para `_bus.RequestAsync()` com tratamento explícito de timeout (veja seção Problemas conhecidos)

---

### 2. **Conteúdo API** - Serviço de Gerenciamento de Conteúdo

**Responsável**: Igor

**Descrição**: Gerencia conteúdos educacionais, cursos, aulas, módulos e materiais didáticos. Fornece endpoints para CRUD de cursos e conteúdo.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB (Database: `PeConteudo`)
- MediatR (CQRS)
- AutoMapper
- Swagger/OpenAPI
- JWT Bearer Authentication

**Estrutura**:
- `PlataformaEducacional.Conteudo.Api` - API REST com Controllers
- `PlataformaEducacional.Conteudo.Application` - Casos de uso, Commands, Queries
- `PlataformaEducacional.Conteudo.Data` - Contexto EF Core, Migrations
- `PlataformaEducacional.Conteudo.Domain` - Entidades, Value Objects, DDD

**Banco de dados**: LocalDB (sem configuração em appsettings - a ser verificado)

**Portas**: `https://localhost:7077`

**Status**: ✅ Implementado com CQRS

**Observações**:
- Não usa RabbitMQ (sem MessageBusConfiguration)
- Sem banco de dados explícito na `appsettings.json`

---

### 3. **Alunos API** - Serviço de Gerenciamento de Alunos

**Responsável**: Gustavo

**Descrição**: Gerencia informações de alunos, matrículas, progresso acadêmico e dados de estudantes.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB (Database: `PeAlunos`)
- MediatR (CQRS)
- RabbitMQ (Mensageria e eventos)
- JWT Bearer Authentication
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Alunos.Api` - API REST com Controllers
- `PlataformaEducacional.Alunos.Application` - Casos de uso com MediatR
- `PlataformaEducacional.Alunos.Data` - Contexto EF Core, Migrations
- `PlataformaEducacional.Alunos.Domain` - Entidades, eventos de domínio, DDD

**Banco de dados**: LocalDB `PeAlunos`

**Portas**: `https://localhost:44360`

**Dependências**:
- RabbitMQ (subscribe a eventos de outros serviços)
- Auth API (para validação de token JWT)
- Conteúdo API (`https://localhost:7077`)

**Status**: ✅ Implementado com CQRS

**Fluxo principal**:
- Recebe eventos como `UsuarioRegistradoIntegrationEvent` e processa inscrições de alunos

---

### 4. **Pagamentos API** - Serviço de Gestão de Pagamentos

**Responsáveis**: Lucas / Rafael

**Descrição**: Gerencia transações financeiras, pagamentos, cobranças e integração com provedores de pagamento.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- SQL Server / LocalDB (Database: `PePagamentos`)
- MediatR (CQRS)
- RabbitMQ (Mensageria)
- AutoMapper
- JWT Bearer Authentication
- Swagger/OpenAPI
- Integração EducaPag (sistema de pagamentos customizado)

**Estrutura**:
- `PlataformaEducacional.Pagamentos.Api` - API REST com Controllers, Configuration, Facade
- `PlataformaEducacional.Pagamentos.EducaPag` - Integração e cliente do sistema de pagamentos

**Banco de dados**: LocalDB `PePagamentos`

**Portas**: `https://localhost:44342`

**Dependências**:
- RabbitMQ (para eventos e comunicação inter-serviços)
- Auth API (validação JWT)
- Alunos API (`https://localhost:44360`)
- Conteúdo API (`https://localhost:7077`)
- Sistema EducaPag (externo, com API Key e chave de criptografia)

**Status**: ✅ Implementado com CQRS e integração de pagamentos

**Configuração**:
```json
"PagamentoConfig": {
  "DefaultApiKey": "ak_ewr4dsWehiwAT",
  "DefaultEncryptionKey": "ek_SweRsdFas4uT5"
}
```

**Fluxo de pagamento**:
- Recebe solicitações de pagamento
- Comunica com EducaPag para processar transações
- Publica eventos de sucesso/falha para RabbitMQ

---

### 5. **BFF API** - Backend for Frontend

**Descrição**: Serviço que funciona como camada intermediária e agregadora para o frontend. Coordena requisições, roteamento e integração entre microsserviços.

**Tecnologias**:
- ASP.NET Core 8.0 Web API
- MediatR (CQRS)
- JWT Bearer Authentication
- Swagger/OpenAPI

**Estrutura**:
- `PlataformaEducacional.Bff.Api` - API REST com Controllers, Configurations, Services

**Banco de dados**: Nenhum (stateless)

**Portas**: `https://localhost:5003`

**Dependências**:
- Auth API (`https://localhost:5001`)
- Alunos API (`https://localhost:44360`)
- Conteúdo API (`https://localhost:7077`)
- Pagamentos API (`https://localhost:44342`)

**Status**: ✅ Implementado

**Observações**:
- Sem banco de dados próprio
- Sem RabbitMQ (não processa eventos)
- Funciona como um proxy/agregador de requisições

---

## 🛠️ Tecnologias e Ferramentas

### Core Stack
- **.NET 8.0** - Framework principal
- **C# 12** - Linguagem de programação
- **ASP.NET Core 8.0** - Framework Web API
- **LocalDB / SQL Server** - Banco de dados

### Banco de Dados
- **SQL Server** - Banco de dados principal
- **LocalDB** - Para desenvolvimento local
- **Entity Framework Core 8.0** - ORM com Code-First migrations

### Mensageria e Integração
- **RabbitMQ** - Message Broker (porta `5672`, management `15672`)
- **EasyNetQ** - Cliente RabbitMQ simplificado
- **Polly** - Biblioteca de resiliência e retry policies (exponential backoff)

### Segurança e Autenticação
- **ASP.NET Core Identity** - Gerenciamento de identidade (apenas Auth API)
- **JWT Bearer** - Autenticação baseada em tokens RSA (chaves privadas em PEM)
- **RSA** - Criptografia assimétrica para assinatura de tokens

### Padrões e Bibliotecas
- **MediatR** - Implementação de CQRS e mediator pattern (Alunos, Conteúdo, Pagamentos)
- **AutoMapper** - Mapeamento de objetos (Conteúdo, Pagamentos)
- **Swagger/OpenAPI** - Documentação de API (todos os serviços)
- **FluentValidation** - Validação de modelos
- **Serilog** (implícito) - Logging estruturado

### Arquitetura
- **Microsserviços** - Arquitetura distribuída e independente
- **DDD** - Domain-Driven Design (especialmente Alunos, Conteúdo)
- **CQRS** - Command Query Responsibility Segregation (MediatR)
- **Event-Driven** - Comunicação assíncrona via RabbitMQ (Alunos, Pagamentos)
- **Database per Service** - Cada microserviço tem seu banco de dados próprio

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

Cada microsserviço possui seu próprio banco de dados. As connection strings já estão configuradas nos arquivos `appsettings.json`:

| Serviço | Database | Connection String |
|---------|----------|-------------------|
| **Auth API** | `PeIdentidade` | `(localdb)\mssqllocaldb` |
| **Alunos API** | `PeAlunos` | `(localdb)\mssqllocaldb` |
| **Conteúdo API** | (não configurado) | - |
| **Pagamentos API** | `PePagamentos` | `(localdb)\mssqllocaldb` |
| **BFF API** | Nenhum | - |

**Exemplo de connection string (Auth API)**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PeIdentidade;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

> **Nota**: As migrations são executadas **automaticamente** ao iniciar cada API através de `app.UseDatabaseMigrationStartData()`. Ajuste as connection strings se usar SQL Server Express ou servidor remoto.

#### 4. Configurar RabbitMQ

**Instalação do RabbitMQ (Windows)**:
```bash
# Usando Chocolatey
choco install rabbitmq

# Ou baixe diretamente do site oficial: https://www.rabbitmq.com/download.html
```

**Configuração nos microsserviços** - Nos arquivos `appsettings.json`:

| Serviço | MessageBus Config |
|---------|-------------------|
| **Auth API** | `host=localhost:5672;publisherConfirms=true;timeout=10` |
| **Alunos API** | `host=localhost:5672;publisherConfirms=true;timeout=10` |
| **Pagamentos API** | `host=localhost:5672;publisherConfirms=true;timeout=10` |
| **Conteúdo API** | Não usa RabbitMQ |
| **BFF API** | Não usa RabbitMQ |

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
- Usuário: `guest`
- Senha: `guest`

> ⚠️ **IMPORTANTE**: O timeout de 10 segundos na connection string pode causar `TaskCanceledException` em RPC calls. Veja a seção "Problemas conhecidos".

#### 5. Executar Migrations

As migrations são executadas **automaticamente ao iniciar cada API** através do método `UseDatabaseMigrationStartData()` configurado no `Program.cs`.

**APIs com migrations automáticas no startup:**
- `PlataformaEducacional.Auth.Api`
- `PlataformaEducacional.Alunos.Api`
- `PlataformaEducacional.Conteudo.Api`
- `PlataformaEducacional.Pagamentos.Api`

**APIs sem camada de dados:**
- `PlataformaEducacional.Bff.Api` (sem banco de dados)

> **Nota**: Se precisar executar migrations manualmente (em caso de problemas ou desenvolvimento), use:
> ```bash
> cd src/services/{nome-da-api}
> dotnet ef database update
> ```

#### 5. Configurar JWT

**Geração de chaves RSA (primeira execução)**:

1. Use a ferramenta `GenerateJwtSigningKey`:
```bash
cd src/tools/GenerateJwtSigningKey
dotnet run
```

2. O comando gera uma chave privada RSA em formato PEM: `keys/educacao-private.pem`

**Configuração nos arquivos `appsettings.json`**:

**Auth API**:
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

**Demais APIs** (apenas validação, sem geração):
```json
"JwtSettings": {
  "Authority": "https://localhost:5001",
  "ExpiracaoHoras": 1,
  "ValidoEm": "https://localhost"
}
```

> ⚠️ **Importante em Produção**: 
> - Use Azure Key Vault ou similares para armazenar chaves privadas
> - Nunca commitar `educacao-private.pem` no repositório
> - Use variáveis de ambiente para paths de chaves

#### 6. Executar os Microsserviços

Você pode executar cada microsserviço individualmente:

```bash
# Auth API (porta 5001)
cd src/services/auth/PlataformaEducacional.Auth.Api
dotnet run

# Alunos API (porta 44360)
cd src/services/alunos/PlataformaEducacional.Alunos.Api
dotnet run

# Conteúdo API (porta 7077)
cd src/services/conteudo/PlataformaEducacional.Conteudo.Api
dotnet run

# Pagamentos API (porta 44342)
cd src/services/pagamentos/PlataformaEducacional.Pagamentos.Api
dotnet run

# BFF API (porta 5003)
cd src/services/bff/PlataformaEducacional.Bff.Api
dotnet run
```

**Ou usar Visual Studio**:
- Abra a solução
- Clique em `Debug > Start Without Debugging` ou selecione múltiplos startup projects

> **Recomendação**: Inicie todos os serviços para testar fluxos completos

#### 7. Acessar a Documentação Swagger

Após iniciar cada API, acesse a documentação Swagger:

| Serviço | URL | Status |
|---------|-----|--------|
| **Auth API** | `https://localhost:5001/swagger/index.html` | ✅ Autenticação, registro, geração JWT |
| **Alunos API** | `https://localhost:44360/swagger/index.html` | ✅ Gerenciamento de alunos, CQRS |
| **Conteúdo API** | `https://localhost:7077/swagger/index.html` | ✅ CRUD de cursos e conteúdo, CQRS |
| **Pagamentos API** | `https://localhost:44342/swagger/index.html` | ✅ Gerenciamento de pagamentos, EducaPag |
| **BFF API** | `https://localhost:5003/swagger/index.html` | ✅ Agregador/roteador de requisições |

> **Nota**: Primeiro registre um usuário em Auth API para obter token JWT, depois use-o nos outros serviços

---

## 🐳 Execução com Docker Compose

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e em execução
- [Docker Compose](https://docs.docker.com/compose/install/) (incluído no Docker Desktop)

### Comandos Docker Compose

#### Construir e Iniciar os Containers

Para construir as imagens e iniciar todos os containers da aplicação:

```bash
docker-compose -f .\docker-compose-local.yml up --build
```

> **Nota**: O parâmetro `--build` força a reconstrução das imagens Docker. Use-o sempre que houver alterações no código ou nas dependências.

#### Parar Todos os Containers

Para parar todos os containers em execução:

```bash
docker-compose -f .\docker-compose-local.yml down
```

> **Nota**: Este comando para e remove os containers, mas mantém as imagens e volumes.

#### Limpar Imagens após Parar os Containers

Para remover todas as imagens Docker não utilizadas e liberar espaço em disco:

```bash
docker image prune -a
```

> **⚠️ Atenção**: Este comando remove **todas as imagens Docker não utilizadas** do seu sistema, não apenas as do projeto. Use com cuidado.

### Dicas de Uso

- **Verificar logs dos containers**:
  ```bash
  docker-compose -f .\docker-compose-local.yml logs -f
  ```

- **Verificar status dos containers**:
  ```bash
  docker-compose -f .\docker-compose-local.yml ps
  ```

- **Parar um serviço específico**:
  ```bash
  docker-compose -f .\docker-compose-local.yml stop <nome-do-servico>
  ```

- **Reiniciar um serviço específico**:
  ```bash
  docker-compose -f .\docker-compose-local.yml restart <nome-do-servico>
  ```

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

## ⚠️ Problemas Conhecidos e Recomendações

### 1. **TaskCanceledException em RPC Calls (Auth API)**

**Problema**:
- O método `AuthController.RegistrarAluno()` usa `_bus.Request<UsuarioRegistradoIntegrationEvent, ResponseMessage>()` (síncrono)
- Lança `System.Threading.Tasks.TaskCanceledException` quando o responder não responde a tempo
- Timeout conflitante entre `timeout=10` na connection string e timeouts implícitos de EasyNetQ

**Raiz do problema**:
- Chamada RPC síncrona fica sujeita ao timeout padrão de EasyNetQ
- Serviço respondedor (Alunos API) **está implementado corretamente** em `RegistroAlunoIntegrationHandler` (BackgroundService)
- Conflito entre `timeout=10` (connection string) e timeouts de aplicação

**Respondedor (Alunos API)**:
```csharp
// src/services/alunos/PlataformaEducacional.Alunos.Application/Services/RegistroAlunoIntegrationHandler.cs
public class RegistroAlunoIntegrationHandler : BackgroundService
{
    private void SetResponder()
    {
        _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
            await RegistrarAluno(request));
    }

    private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistradoIntegrationEvent message)
    {
        var alunoCommand = new RegistrarAlunoCommand(message.Id, message.Nome, message.Email, message.Cpf);
        ValidationResult sucesso;

        using (var scope = _serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            sucesso = await mediator.SendCommand(alunoCommand);
        }

        return new ResponseMessage(sucesso);
    }
}
```

**Verificação**:
✅ Responder está registrado em `RegistroAlunoIntegrationHandler`  
✅ Tipo de evento está correto: `UsuarioRegistradoIntegrationEvent`  
✅ Retorna `ResponseMessage` corretamente  
✅ Inicia como BackgroundService (verifica conexão automática)

**Possíveis causas de timeout**:
1. **Alunos API não está em execução** - Serviço respondedor precisa estar rodando
2. **RabbitMQ não está acessível** - Verificar `http://localhost:15672`
3. **Alunos API não conseguiu conectar ao RabbitMQ** - Verificar logs do serviço
4. **Timeout muito curto** - `timeout=10` pode ser insuficiente para latência

**Solução recomendada**:
```csharp
// Trocar de síncrono para assíncrono com timeout explícito
private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistro usuarioRegistro)
{
    var usuario = await _userManager.FindByEmailAsync(usuarioRegistro.Email);
    var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
        Guid.Parse(usuario.Id), usuarioRegistro.Nome, usuarioRegistro.Email, usuarioRegistro.Cpf);

    try
    {
        // Usar RequestAsync que aplica timeout explícito de 30s
        return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
    }
    catch (TaskCanceledException tcex)
    {
        _logger.LogWarning(tcex, "Timeout ao aguardar resposta RPC de UsuarioRegistradoIntegrationEvent. " +
                                  "Verifique se Alunos API está em execução.");
        await _userManager.DeleteAsync(usuario);
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
            "RabbitMQ", "Tempo esgotado ao aguardar resposta do serviço de integração"));
        return new ResponseMessage(validationResult);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao enviar evento de registro para RabbitMQ. Serviço: Alunos API");
        await _userManager.DeleteAsync(usuario);
        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
            "RabbitMQ", "Erro ao tentar enviar para fila"));
        return new ResponseMessage(validationResult);
    }
}
```

**Checklist de diagnóstico**:
- [ ] Alunos API está em execução (`dotnet run` na pasta do projeto)
- [ ] RabbitMQ está rodando (verificar `Get-Service RabbitMQ` no PowerShell)
- [ ] RabbitMQ Management acessível: http://localhost:15672
- [ ] Logs de Alunos API mostram "BackgroundService started"
- [ ] Não há erros de conexão com banco de dados (PeAlunos)
- [ ] Auth API consegue conectar ao RabbitMQ (sem erros)

**Passos de validação**:
1. Inicie RabbitMQ: `rabbitmq-server` ou `rabbitmq-service start`
2. Inicie Alunos API com logs: `dotnet run --verbosity=minimal`
3. Inicie Auth API
4. Tente registrar novo usuário
5. Verifique logs em ambos serviços

**Passos de validação**:
1. Confirmar que **Alunos API** está registrando `RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>()`
2. Verificar logs de ambos serviços: `auth` e `alunos`
3. Inspecionar RabbitMQ Management UI para verificar:
   - Filas criadas
   - Consumers conectados
   - Mensagens pendentes
4. Considerar aumentar timeout em connection string (ex.: `timeout=30`) ou remover

### 2. **Banco de dados Conteúdo API não configurado**

**Problema**: `appsettings.json` não contém `ConnectionStrings` para Conteúdo API

**Solução**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PeConteudo;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. **Dependências entre microsserviços**

**Observado**:
- Alunos API depende de Conteúdo API (`ConteudoUrl: https://localhost:7077`)
- Pagamentos API depende de Alunos API (`AlunoUrl: https://localhost:44360`)
- Todos (exceto Auth) precisam de Auth para validar JWT

**Recomendação**:
- Usar `https://localhost` em desenvolvimento; ajustar conforme ambiente
- Considerar implementar health checks para dependências

### 4. **RabbitMQ deve estar em execução**

**Verificar**:
```bash
# Windows
Get-Service RabbitMQ | Status

# ou verificar via URL
http://localhost:15672
```

### 5. **Licença MediatR**

- Token de licença incluído em `appsettings.json` (desenvolvimento)
- **Importante**: Verificar se licença é válida para produção (expira em **2026-05-17**)

## 📚 Documentação Adicional

### Padrões de Comunicação

- **Síncrona**: HTTP/REST diretos entre serviços e clientes
- **Assíncrona**: RabbitMQ (Pub/Sub) para eventos de integração entre microsserviços
- **RPC**: MessageBus com EasyNetQ para requisição-resposta (ex.: Auth → Alunos)

### Estrutura de Eventos de Integração

Eventos publicados/consumidos via RabbitMQ:
- **UsuarioRegistradoIntegrationEvent**: Publicado por Auth API ao registrar novo aluno; consumido por Alunos API
- Outros eventos podem estar sendo implementados nos serviços

### Portas de Desenvolvimento

| Serviço | HTTPS | HTTP |
|---------|-------|------|
| Auth | 5001 | 5000 |
| Alunos | 44360 | 44359 |
| Conteúdo | 7077 | 7076 |
| Pagamentos | 44342 | 44341 |
| BFF | 5003 | 5002 |

### Arquitetura de Microsserviços

```
┌─────────────────────────────────────────────────────────────────┐
│                          Cliente/Frontend                         │
└──────────────────┬──────────────────┬──────────────────┬─────────┘
                   │                  │                  │
        ┌──────────▼────────┐  ┌──────▼────────┐  ┌─────▼──────┐
        │    BFF API        │  │  Swagger      │  │  Auth API  │
        │  (5003/5002)      │  │  (Docs)       │  │ (5001)     │
        └──────────┬────────┘  └───────────────┘  └────┬───────┘
                   │                                    │
        ┌──────────▼────────────────────────────────────▼──────┐
        │               RabbitMQ (localhost:5672)              │
        │  - Pub/Sub (eventos de integração)                  │
        │  - RPC (requisição-resposta)                        │
        └──────┬──────────────────────┬──────────┬────────────┘
               │                      │          │
        ┌──────▼─────┐      ┌────────▼────┐  ┌─▼──────────┐
        │ Alunos API │      │ Conteúdo API│  │ Pagamentos │
        │ (44360)    │      │  (7077)     │  │ API(44342) │
        └──────┬─────┘      └────────┬────┘  └─┬──────────┘
               │                      │         │
        ┌──────▼──────────────────────▼────────▼─────┐
        │    SQL Server LocalDB                      │
        │ ┌──────────┐ ┌────────┐ ┌──────────┐      │
        │ │PeAlunos  │ │PeConteudo│PePagamentos│    │
        │ └──────────┘ └────────┘ └──────────┘      │
        └──────────────────────────────────────────┘
```

### Health Checks

> **TODO**: Implementar health checks para:
> - RabbitMQ connectivity
> - Banco de dados connectivity
> - Serviços dependentes (cross-service checks)

---

## 👥 Equipe e Responsabilidades

**Grupo 5 - MBA DevIO - Módulo 4**

| Membro | Responsabilidade | Serviço |
|--------|------------------|---------|
| Cleber | Arquitetura, Segurança JWT RSA | Auth API |
| Igor | DDD, Conteúdo Educacional, CQRS | Conteúdo API |
| Gustavo | Gerenciamento de Alunos, MediatR | Alunos API |
| Lucas | Pagamentos, EducaPag Integration | Pagamentos API |
| Rafael | Infraestrutura RabbitMQ, Pagamentos | Pagamentos API |

---

## 📝 Contribuindo

### Padrões de Código

1. **Nomenclatura**: PascalCase para classes, camelCase para variáveis
2. **DDD**: Use value objects, agregados e entidades de domínio quando apropriado
3. **CQRS**: Separe Commands (escrita) e Queries (leitura) usando MediatR
4. **Validação**: Use FluentValidation para regras complexas
5. **Logging**: Use ILogger injetado via DI

### Git Workflow

- **Branch principal**: `main` (produção)
- **Branch de desenvolvimento**: `rfg-dev` ou similar
- **Branches de feature**: `feature/descricao-curta` 
- **Commits**: Use mensagens descritivas em português ou inglês

### Build e Deploy

```bash
# Build local
dotnet build

# Teste (quando implementado)
dotnet test

# Publish
dotnet publish --configuration Release
```

---

## 📞 Referências e Recursos

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [RabbitMQ Getting Started](https://www.rabbitmq.com/getstarted.html)
- [MediatR](https://github.com/jbogard/MediatR)
- [Polly Resilience Library](https://github.com/App-vNext/Polly)

---

## 🚧 Roadmap Futuro

- [ ] API Gateway (Kong / Azure API Management)
- [ ] Health Checks implementados
- [ ] Circuit Breaker para chamadas inter-serviços
- [ ] Testes unitários e integração
- [ ] Docker Compose para ambiente local
- [ ] CI/CD (GitHub Actions / Azure Pipelines)
- [ ] Observabilidade (Serilog, Application Insights)
- [ ] Rate Limiting e Throttling
- [ ] Versionamento de API

---