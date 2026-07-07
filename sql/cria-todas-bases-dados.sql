-- Criando base de dados para Indentity - ApiAuth
print 'Criando base de dados para Indentity - ApiAuth';
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'PeIdentidade'
)
BEGIN
    CREATE DATABASE PeIdentidade;
    
END
GO

use PeIdentidade
Go

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END
Go

IF OBJECT_ID(N'[AspNetRoles]') IS NULL
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );

    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );

    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
END
Go

print 'sleep 5 segundos ...'
WAITFOR DELAY '00:00:05'; 
print 'Continuando Seed Api Auth...'

IF NOT EXISTS(select 1 from [AspNetRoles])
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260623234903_IdentityStart', N'8.0.25');    

    -- INSERT DATA SEED
    -- ROLES
    INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('73495ED3-3E4A-4E92-99C6-E724C5156754','Administrador', 'ADMINISTRADOR', '73495ED3-3E4A-4E92-99C6-E724C5156754');
    INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('9EF38777-247B-4043-B8A5-C17505BCF637','Aluno', 'ALUNO', '9EF38777-247B-4043-B8A5-C17505BCF637');

    -- USERS
    -- Admin
    INSERT INTO [dbo].[AspNetUsers]
           ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
     VALUES ('fd0ab48e-b395-40c5-ab78-aa68470f9c73', 'admin@educa.com', 'admin@educa.com', 'ADMIN@EDUCA.COM', 'ADMIN@EDUCA.COM', 1, 'Admin@123', NewId(), NewId(), null, 0, 0, null, 1, 0)

    INSERT INTO [dbo].[AspNetUserRoles] ([UserId] , [RoleId]) VALUES ('fd0ab48e-b395-40c5-ab78-aa68470f9c73', '73495ED3-3E4A-4E92-99C6-E724C5156754');

    -- Aluno 1
    INSERT INTO [dbo].[AspNetUsers]
           ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
     VALUES ('a2b73c37-c6a9-4582-a397-f0bc020d2fbd', 'joao.estudioso@educa.com', 'João Estudioso da Silva', 'joao.estudioso@educa.com', 'JOAO.ESTUDIOSO@EDUCA.COM', 1, '"Aluno@123', NewId(), NewId(), null, 0, 0, null, 1, 0)
    
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId] , [RoleId]) VALUES ('a2b73c37-c6a9-4582-a397-f0bc020d2fbd', '9EF38777-247B-4043-B8A5-C17505BCF637');

    -- Aluno 2
    INSERT INTO [dbo].[AspNetUsers]
           ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
     VALUES ('9eec2f27-d99d-4624-8d2e-c98e5e5861f8', 'marcio.iniciante@educa.com', 'Mario Iniciante dos Santos', 'admin@educa.com', 'MARCIO.INICIANTE@EDUCA.COM', 1, '"Aluno@123', NewId(), NewId(), null, 0, 0, null, 1, 0)

    INSERT INTO [dbo].[AspNetUserRoles] ([UserId] , [RoleId]) VALUES ('9eec2f27-d99d-4624-8d2e-c98e5e5861f8', '9EF38777-247B-4043-B8A5-C17505BCF637');
END;
GO


-- Criando base de dados para Aluno - ApiAluno
print 'Criando base de dados para Aluno - ApiAluno';
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'PeAlunos'
)
BEGIN
    Create Database PeAlunos
END
GO

use PeAlunos
Go

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );

    CREATE TABLE [Alunos] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] varchar(200) NOT NULL,
        [Email] varchar(254) NOT NULL,
        [Cpf] varchar(11) NOT NULL,
        [Excluido] bit NOT NULL,
        CONSTRAINT [PK_Alunos] PRIMARY KEY ([Id])
    );
    
    CREATE TABLE [Enderecos] (
        [Id] uniqueidentifier NOT NULL,
        [Logradouro] varchar(200) NOT NULL,
        [Numero] varchar(50) NOT NULL,
        [Complemento] varchar(250) NOT NULL,
        [Bairro] varchar(100) NOT NULL,
        [Cep] varchar(20) NOT NULL,
        [Cidade] varchar(100) NOT NULL,
        [Estado] varchar(50) NOT NULL,
        [AlunoId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Enderecos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Enderecos_Alunos_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Alunos] ([Id])
    );

    CREATE TABLE [Matriculas] (
        [Id] uniqueidentifier NOT NULL,
        [AlunoId] uniqueidentifier NOT NULL,
        [CursoId] uniqueidentifier NOT NULL,
        [DataMatricula] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Matriculas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Matriculas_Alunos_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Alunos] ([Id])
    );
    
    CREATE TABLE [Certificados] (
        [Id] uniqueidentifier NOT NULL,
        [MatriculaId] uniqueidentifier NOT NULL,
        [CodigoValidacao] uniqueidentifier NOT NULL,
        [DataEmissao] datetime2 NOT NULL,
        CONSTRAINT [PK_Certificados] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Certificados_Matriculas_MatriculaId] FOREIGN KEY ([MatriculaId]) REFERENCES [Matriculas] ([Id])
    );

    CREATE TABLE [ProgressoAulas] (
        [Id] uniqueidentifier NOT NULL,
        [MatriculaId] uniqueidentifier NOT NULL,
        [AulaId] uniqueidentifier NOT NULL,
        [DataConclusao] datetime2 NOT NULL,
        CONSTRAINT [PK_ProgressoAulas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProgressoAulas_Matriculas_MatriculaId] FOREIGN KEY ([MatriculaId]) REFERENCES [Matriculas] ([Id])
    );

    CREATE UNIQUE INDEX [IX_Certificados_MatriculaId] ON [Certificados] ([MatriculaId]);

    CREATE UNIQUE INDEX [IX_Enderecos_AlunoId] ON [Enderecos] ([AlunoId]);

    CREATE INDEX [IX_Matriculas_AlunoId] ON [Matriculas] ([AlunoId]);

    CREATE INDEX [IX_ProgressoAulas_MatriculaId] ON [ProgressoAulas] ([MatriculaId]);

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260614205919_AlunoStart', N'8.0.25');
END
GO

print 'sleep 5 segundos ...'
WAITFOR DELAY '00:00:05'; 
print 'Continuando Seed Api Aluno...'

IF NOT EXISTS(select 1 from [Alunos])
BEGIN
    -- Aluno 1
    INSERT INTO [dbo].[Alunos] ([Id], [Nome], [Email], [Cpf], [Excluido])
     VALUES ('a2b73c37-c6a9-4582-a397-f0bc020d2fbd', 'João Estudioso da Silva', 'joao.estudioso@educa.com', '26337499093', 0)

    -- Aluno 1
    INSERT INTO [dbo].[Alunos] ([Id], [Nome], [Email], [Cpf], [Excluido])
     VALUES ('9eec2f27-d99d-4624-8d2e-c98e5e5861f8', 'Mario Iniciante dos Santos', 'marcio.iniciante@educa.com', '11793917051', 0)

END;
GO

-- Criando base de dados para Conteudo - ApiConteudo
print 'Criando base de dados para Conteudo - ApiConteudo';
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'PeConteudo'
)
BEGIN
    CREATE DATABASE PeConteudo;
END
GO

use PeConteudo
Go

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );

    CREATE TABLE [Cursos] (
        [Id] uniqueidentifier NOT NULL,
        [Titulo] varchar(200) NOT NULL,
        [Descricao] varchar(1000) NOT NULL,
        [Instrutor] varchar(150) NOT NULL,
        [Nivel] int NOT NULL,
        [Valor] decimal(15,2) NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [Ativo] bit NOT NULL,
        [Ementa] varchar(2000) NOT NULL,
        [Objetivo] varchar(1000) NOT NULL,
        [Bibliografia] varchar(2000) NOT NULL,
        [MaterialUrl] varchar(500) NOT NULL,
        CONSTRAINT [PK_Cursos] PRIMARY KEY ([Id])
    );

    CREATE TABLE [Aulas] (
        [Id] uniqueidentifier NOT NULL,
        [Titulo] varchar(200) NOT NULL,
        [Descricao] varchar(1000) NOT NULL,
        [DuracaoMinutos] int NOT NULL,
        [Ordem] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [Ativa] bit NOT NULL,
        [CursoId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Aulas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Aulas_Cursos_CursoId] FOREIGN KEY ([CursoId]) REFERENCES [Cursos] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Aulas_CursoId] ON [Aulas] ([CursoId]);
    
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260618032230_ConteudoStart', N'8.0.25');

END
Go

-- Criando base de dados para Pagamento - ApiPagamento
print 'Criando base de dados para Pagamento - ApiPagamento';
IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = N'PePagamentos'
)
BEGIN
    CREATE DATABASE PePagamentos;    
END
GO

use PePagamentos
Go

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );

    CREATE TABLE [Pagamentos] (
        [Id] uniqueidentifier NOT NULL,
        [MatriculaId] uniqueidentifier NOT NULL,
        [TipoPagamento] int NOT NULL,
        [Valor] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Pagamentos] PRIMARY KEY ([Id])
    );
    
    CREATE TABLE [Transacoes] (
        [Id] uniqueidentifier NOT NULL,
        [CodigoAutorizacao] varchar(100) NOT NULL,
        [BandeiraCartao] varchar(100) NOT NULL,
        [DataTransacao] datetime2 NULL,
        [ValorTotal] decimal(18,2) NOT NULL,
        [CustoTransacao] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [TID] varchar(100) NOT NULL,
        [NSU] varchar(100) NOT NULL,
        [PagamentoId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Transacoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Transacoes_Pagamentos_PagamentoId] FOREIGN KEY ([PagamentoId]) REFERENCES [Pagamentos] ([Id])
    );
    
    CREATE INDEX [IX_Transacoes_PagamentoId] ON [Transacoes] ([PagamentoId]);
    
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260618035824_PagamtentoStart', N'8.0.25');

END;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeIdentidade')
BEGIN
    print 'Database PeIdentidade criado com sucesso !'
    USE PeIdentidade;

    print '   Tabelas';
    IF OBJECT_ID(N'[AspNetRoles]') IS NOT NULL
    BEGIN
        print '    - AspNetRoles - Ok';
    END
    IF OBJECT_ID(N'[AspNetUsers]') IS NOT NULL
    BEGIN
        print '    - AspNetUsers - Ok';
    END
    IF OBJECT_ID(N'[AspNetRoleClaims]') IS NOT NULL
    BEGIN
        print '    - AspNetRoleClaims - Ok';
    END
    IF OBJECT_ID(N'[AspNetUserClaims]') IS NOT NULL
    BEGIN
        print '    - AspNetUserClaims - Ok';
    END
    IF OBJECT_ID(N'[AspNetUserLogins]') IS NOT NULL
    BEGIN
        print '    - AspNetUserLogins - Ok';
    END
    IF OBJECT_ID(N'[AspNetUserRoles]') IS NOT NULL
    BEGIN
        print '    - AspNetUserRoles - Ok';
    END
    IF OBJECT_ID(N'[AspNetUserTokens]') IS NOT NULL
    BEGIN
        print '    - AspNetUserTokens - Ok';
    END    
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeAlunos')
BEGIN
    print 'Database PeAlunos criado com sucesso !'
    USE PeAlunos;

    print '   Tabelas';
    IF OBJECT_ID(N'[Alunos]') IS NOT NULL
    BEGIN
        print '    - Alunos - Ok';
    END
    IF OBJECT_ID(N'[Enderecos]') IS NOT NULL
    BEGIN
        print '    - Enderecos - Ok';
    END
    IF OBJECT_ID(N'[Matriculas]') IS NOT NULL
    BEGIN
        print '    - Matriculas - Ok';
    END
    IF OBJECT_ID(N'[Certificados]') IS NOT NULL
    BEGIN
        print '    - Certificados - Ok';
    END
    IF OBJECT_ID(N'[ProgressoAulas]') IS NOT NULL
    BEGIN
        print '    - ProgressoAulas - Ok';
    END
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeConteudo')
BEGIN
    print 'Database PeConteudo criado com sucesso !'
    USE PeConteudo;

    print '   Tabelas';
    IF OBJECT_ID(N'[Cursos]') IS NOT NULL
    BEGIN
        print '    - Cursos - Ok';
    END
    IF OBJECT_ID(N'[Aulas]') IS NOT NULL
    BEGIN
        print '    - Aulas - Ok';
    END    
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PePagamentos')
BEGIN
    print 'Database PePagamentos criado com sucesso !'
    USE PePagamentos;

    print '   Tabelas';
    IF OBJECT_ID(N'[Pagamentos]') IS NOT NULL
    BEGIN
        print '    - Pagamentos - Ok';
    END
    IF OBJECT_ID(N'[Transacoes]') IS NOT NULL
    BEGIN
        print '    - Transacoes - Ok';
    END
END
