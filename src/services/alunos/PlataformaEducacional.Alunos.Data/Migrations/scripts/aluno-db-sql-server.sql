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

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614205919_AlunoStart', N'8.0.25');

END;
GO

COMMIT;
GO

