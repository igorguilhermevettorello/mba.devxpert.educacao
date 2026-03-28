using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Conteudo.Data.Context;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Enums;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;

namespace PlataformaEducacional.Conteudo.Api.Configuration.Seed
{
    public static class DatabaseMigrationStartDataExtension
    {
        public static void UseDatabaseMigrationStartData(this WebApplication app)
        {
            EnsureSeedData(app).Wait();
        }

        private static async Task EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IWebHostEnvironment>();

            try
            {
                if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
                {
                    var context = services.GetRequiredService<CursoContext>();

                    await context.Database.MigrateAsync();

                    await EnsureSeedCursos(context);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task EnsureSeedCursos(CursoContext context)
        {
            // Verifica se já existem cursos cadastrados
            if (await context.Cursos.AnyAsync())
                return;

            var cursos = new List<Curso>();

            // ========================================
            // CURSO 1: C# Fundamentos
            // ========================================
            var cursoCSharp = new Curso(
                "C# Fundamentos - Do Básico ao Avançado",
                "Aprenda C# do zero com uma abordagem prática e completa. Ideal para iniciantes que desejam dominar a linguagem de programação mais utilizada no ecossistema .NET.",
                "Ana Silva",
                NivelCurso.Basico,
                299.90m
            );

            var conteudoCSharp = new ConteudoProgramatico(
                "Introdução ao C#, Tipos de dados, Estruturas de controle, POO, Coleções, LINQ, Async/Await, Exception Handling",
                "Capacitar o aluno a desenvolver aplicações robustas utilizando C# e .NET, compreendendo desde conceitos fundamentais até recursos avançados da linguagem.",
                "C# 12 in a Nutshell - Joseph Albahari; Pro C# 10 with .NET 6 - Andrew Troelsen",
                "https://docs.microsoft.com/pt-br/dotnet/csharp/"
            );

            cursoCSharp.AdicionarConteudoProgramatico(conteudoCSharp);

            var aulasCSharp = new List<Aula>
            {
                new Aula("Introdução ao C# e .NET", "Visão geral da linguagem C#, história do .NET e configuração do ambiente de desenvolvimento.", 45, 1),
                new Aula("Tipos de Dados e Variáveis", "Aprenda sobre tipos primitivos, conversões, constantes e variáveis em C#.", 60, 2),
                new Aula("Estruturas de Controle", "Dominando if/else, switch, loops (for, while, foreach) e controle de fluxo.", 55, 3),
                new Aula("Métodos e Funções", "Criação de métodos, parâmetros, retorno de valores, sobrecarga e métodos de extensão.", 50, 4),
                new Aula("Programação Orientada a Objetos - Classes", "Conceitos fundamentais de POO: classes, objetos, encapsulamento e propriedades.", 70, 5),
                new Aula("POO - Herança e Polimorfismo", "Trabalhando com herança, classes abstratas, interfaces e polimorfismo.", 65, 6),
                new Aula("Coleções e Genéricos", "Arrays, List, Dictionary, HashSet e trabalhando com tipos genéricos.", 60, 7),
                new Aula("LINQ - Language Integrated Query", "Consultas em coleções usando LINQ: where, select, orderby, join e agregações.", 75, 8),
                new Aula("Tratamento de Exceções", "Exception handling, try/catch/finally, exceções customizadas e best practices.", 50, 9),
                new Aula("Async/Await e Programação Assíncrona", "Dominando async/await, Task, operações assíncronas e paralelismo.", 80, 10)
            };

            foreach (var aula in aulasCSharp)
            {
                aula.AssociarCurso(cursoCSharp.Id);
                cursoCSharp.AdicionarAula(aula);
            }

            cursos.Add(cursoCSharp);

            // ========================================
            // CURSO 2: ASP.NET Core Web API
            // ========================================
            var cursoWebApi = new Curso(
                "ASP.NET Core Web API - RESTful APIs Profissionais",
                "Construa APIs REST escaláveis e seguras usando ASP.NET Core. Aprenda autenticação JWT, validação, documentação com Swagger e muito mais.",
                "Carlos Mendes",
                NivelCurso.Intermediario,
                399.90m
            );

            var conteudoWebApi = new ConteudoProgramatico(
                "REST, HTTP, ASP.NET Core fundamentals, Controllers, Routing, Model Binding, Validação, Autenticação JWT, Swagger/OpenAPI, Entity Framework Core, Repository Pattern, CQRS",
                "Formar desenvolvedores capazes de criar APIs REST de qualidade profissional, seguindo boas práticas de arquitetura, segurança e documentação.",
                "Building Web APIs with ASP.NET Core - Valerio De Sanctis; Pro ASP.NET Core 6 - Adam Freeman",
                "https://learn.microsoft.com/aspnet/core/web-api/"
            );

            cursoWebApi.AdicionarConteudoProgramatico(conteudoWebApi);

            var aulasWebApi = new List<Aula>
            {
                new Aula("Introdução a APIs REST e HTTP", "Fundamentos de REST, verbos HTTP, status codes e princípios RESTful.", 50, 1),
                new Aula("Criando seu Primeiro Controller", "Estrutura de projeto ASP.NET Core, criação de controllers e endpoints básicos.", 60, 2),
                new Aula("Routing e Model Binding", "Configuração de rotas, parâmetros de rota, query strings e model binding.", 55, 3),
                new Aula("Validação de Dados com Data Annotations", "Validando requisições usando Data Annotations e FluentValidation.", 50, 4),
                new Aula("DTOs e AutoMapper", "Separando modelos de domínio de DTOs e mapeamento automático.", 65, 5),
                new Aula("Entity Framework Core - Básico", "Configuração do EF Core, DbContext, migrations e operações CRUD.", 70, 6),
                new Aula("Repository Pattern e Unit of Work", "Implementando o padrão Repository para desacoplamento e testabilidade.", 60, 7),
                new Aula("Autenticação e Autorização com JWT", "Implementando autenticação baseada em tokens JWT e políticas de autorização.", 80, 8),
                new Aula("Documentação com Swagger/OpenAPI", "Configurando Swagger UI e documentando APIs automaticamente.", 45, 9),
                new Aula("Tratamento Global de Erros", "Middleware de exceções, problem details e padronização de erros.", 55, 10),
                new Aula("Versionamento de APIs", "Estratégias de versionamento: URL, header, query string.", 50, 11),
                new Aula("CQRS com MediatR", "Separando comandos de consultas usando o padrão CQRS e MediatR.", 70, 12)
            };

            foreach (var aula in aulasWebApi)
            {
                aula.AssociarCurso(cursoWebApi.Id);
                cursoWebApi.AdicionarAula(aula);
            }

            cursos.Add(cursoWebApi);

            // ========================================
            // CURSO 3: Clean Architecture e DDD
            // ========================================
            var cursoCleanArch = new Curso(
                "Clean Architecture e Domain-Driven Design",
                "Domine os conceitos de arquitetura limpa e DDD para criar aplicações enterprise escaláveis e manuteníveis.",
                "Roberto Oliveira",
                NivelCurso.Avancado,
                599.90m
            );

            var conteudoCleanArch = new ConteudoProgramatico(
                "Clean Architecture, DDD Tactical Patterns, Entities, Value Objects, Aggregates, Domain Events, Application Layer, Infrastructure Layer, Dependency Injection, Microservices",
                "Capacitar arquitetos e desenvolvedores sênior a projetar sistemas complexos usando Clean Architecture e DDD, promovendo código testável e desacoplado.",
                "Clean Architecture - Robert C. Martin; Domain-Driven Design - Eric Evans; Implementing DDD - Vaughn Vernon",
                "https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/"
            );

            cursoCleanArch.AdicionarConteudoProgramatico(conteudoCleanArch);

            var aulasCleanArch = new List<Aula>
            {
                new Aula("Fundamentos de Clean Architecture", "Princípios SOLID, separação de camadas e independência de frameworks.", 60, 1),
                new Aula("Domain-Driven Design - Visão Geral", "Strategic Design, Bounded Contexts, Ubiquitous Language.", 70, 2),
                new Aula("Entities e Value Objects", "Diferenças entre entidades e objetos de valor, identidade e imutabilidade.", 65, 3),
                new Aula("Aggregates e Aggregate Roots", "Modelando agregados, garantindo consistência transacional.", 75, 4),
                new Aula("Domain Events", "Comunicação entre agregados usando eventos de domínio.", 60, 5),
                new Aula("Repository Pattern Avançado", "Implementação de repositórios respeitando boundaries do DDD.", 55, 6),
                new Aula("Application Layer e Use Cases", "Orquestrando lógica de negócio com Application Services e Commands.", 70, 7),
                new Aula("CQRS e Event Sourcing", "Separação de leitura/escrita e armazenamento baseado em eventos.", 80, 8),
                new Aula("Infrastructure Layer", "Implementação de persistência, mensageria e serviços externos.", 65, 9),
                new Aula("Dependency Injection e IoC", "Inversão de controle e injeção de dependência em arquiteturas limpas.", 50, 10),
                new Aula("Testing Strategy em Clean Architecture", "Testes unitários, integração e arquitetura testável.", 70, 11),
                new Aula("Microservices com Clean Architecture", "Aplicando Clean Arch em arquiteturas distribuídas.", 75, 12)
            };

            foreach (var aula in aulasCleanArch)
            {
                aula.AssociarCurso(cursoCleanArch.Id);
                cursoCleanArch.AdicionarAula(aula);
            }

            cursos.Add(cursoCleanArch);

            // ========================================
            // CURSO 4: SQL Server e Entity Framework Core
            // ========================================
            var cursoBD = new Curso(
                "SQL Server e Entity Framework Core - Do Básico ao Avançado",
                "Domine banco de dados relacionais com SQL Server e aprenda a trabalhar com Entity Framework Core de forma profissional.",
                "Mariana Costa",
                NivelCurso.Intermediario,
                349.90m
            );

            var conteudoBD = new ConteudoProgramatico(
                "SQL Fundamentals, SELECT, JOIN, Agregações, Índices, Stored Procedures, Triggers, EF Core Fundamentals, Code First, Database First, Migrations, Lazy/Eager Loading, Performance",
                "Preparar desenvolvedores para trabalhar com banco de dados SQL Server e utilizar Entity Framework Core de forma eficiente e otimizada.",
                "SQL Server 2022 - Ben Forta; Programming Entity Framework Core - Julia Lerman",
                "https://learn.microsoft.com/ef/core/"
            );

            cursoBD.AdicionarConteudoProgramatico(conteudoBD);

            var aulasBD = new List<Aula>
            {
                new Aula("Introdução a Bancos de Dados Relacionais", "Conceitos fundamentais: tabelas, chaves, relacionamentos, normalização.", 50, 1),
                new Aula("SQL Básico - SELECT e Filtros", "Consultas básicas, WHERE, LIKE, IN, BETWEEN, ORDER BY.", 60, 2),
                new Aula("JOINs e Relacionamentos", "INNER JOIN, LEFT JOIN, RIGHT JOIN, FULL JOIN, self-join.", 70, 3),
                new Aula("Funções de Agregação e GROUP BY", "COUNT, SUM, AVG, MIN, MAX, HAVING.", 55, 4),
                new Aula("Subconsultas e CTEs", "Subqueries, Common Table Expressions e consultas complexas.", 65, 5),
                new Aula("Índices e Performance", "Criação de índices, clustered vs non-clustered, análise de planos de execução.", 60, 6),
                new Aula("Stored Procedures e Functions", "Criando procedures, parâmetros, funções escalares e table-valued.", 70, 7),
                new Aula("Entity Framework Core - Introdução", "DbContext, DbSet, configuração inicial e conexão.", 55, 8),
                new Aula("Code First e Migrations", "Criando modelos, conventions, migrations e seed data.", 75, 9),
                new Aula("Fluent API e Configurações Avançadas", "Mapeamento avançado, relacionamentos, índices via Fluent API.", 70, 10),
                new Aula("Lazy Loading vs Eager Loading", "Estratégias de carregamento e otimização de consultas.", 60, 11),
                new Aula("Performance no EF Core", "Tracking, AsNoTracking, compiled queries, batch operations.", 65, 12)
            };

            foreach (var aula in aulasBD)
            {
                aula.AssociarCurso(cursoBD.Id);
                cursoBD.AdicionarAula(aula);
            }

            cursos.Add(cursoBD);

            // ========================================
            // CURSO 5: Testes Automatizados com xUnit
            // ========================================
            var curseTestes = new Curso(
                "Testes Automatizados com xUnit, Moq e FluentAssertions",
                "Aprenda a criar testes unitários, de integração e automatizados usando as melhores ferramentas do ecossistema .NET.",
                "Pedro Santos",
                NivelCurso.Intermediario,
                279.90m
            );

            var conteudoTestes = new ConteudoProgramatico(
                "Fundamentos de Testes, TDD, xUnit, Moq, FluentAssertions, Test Doubles, Testes de Integração, Code Coverage, CI/CD com testes",
                "Formar desenvolvedores capazes de escrever testes de qualidade, promovendo código confiável e manutenível através de TDD e automação.",
                "The Art of Unit Testing - Roy Osherove; Test-Driven Development - Kent Beck",
                "https://xunit.net/"
            );

            curseTestes.AdicionarConteudoProgramatico(conteudoTestes);

            var aulasTestes = new List<Aula>
            {
                new Aula("Fundamentos de Testes de Software", "Tipos de testes, pirâmide de testes, benefícios da automação.", 45, 1),
                new Aula("Introdução ao xUnit", "Criando primeiro projeto de testes, Fact, Theory, assertions.", 55, 2),
                new Aula("Assertions com FluentAssertions", "Melhorando legibilidade dos testes com FluentAssertions.", 50, 3),
                new Aula("Test-Driven Development (TDD)", "Ciclo Red-Green-Refactor, escrevendo testes antes do código.", 70, 4),
                new Aula("Mocking com Moq", "Criando mocks, stubs e fakes para isolar dependências.", 65, 5),
                new Aula("Testando Controllers e APIs", "Testes de integração em ASP.NET Core usando WebApplicationFactory.", 75, 6),
                new Aula("Testando Repositórios e Banco de Dados", "In-Memory Database, DbContext em testes, fixtures.", 60, 7),
                new Aula("Code Coverage e Qualidade", "Medindo cobertura de código, análise de qualidade.", 50, 8),
                new Aula("Testes Parametrizados com Theory", "MemberData, ClassData, InlineData para testes parametrizados.", 55, 9),
                new Aula("CI/CD com Testes Automatizados", "Integração contínua, pipelines, execução automática de testes.", 60, 10)
            };

            foreach (var aula in aulasTestes)
            {
                aula.AssociarCurso(curseTestes.Id);
                curseTestes.AdicionarAula(aula);
            }

            cursos.Add(curseTestes);

            // Adiciona todos os cursos e salva
            context.Cursos.AddRange(cursos);
            await context.SaveChangesAsync();
        }
    }
}
