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
END;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
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
    
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260614200446_IdentityStartSql', N'8.0.25');
    
END;
GO

COMMIT;
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
END;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'[Alunos]') IS NULL
BEGIN
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

END;
GO

COMMIT;
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
END;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
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

END;
GO

COMMIT;
GO

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
END;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
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

COMMIT;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeIdentidade')
BEGIN
    print 'Database PeIdentidade criado com sucesso !'
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeAlunos')
BEGIN
    print 'Database PeAlunos criado com sucesso !'
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PeConteudo')
BEGIN
    print 'Database PeConteudo criado com sucesso !'
END
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PePagamentos')
BEGIN
    print 'Database PePagamentos criado com sucesso !'
END
GO