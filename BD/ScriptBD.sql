/*================================================================================
  Script idempotente para crear la base de datos “TurnosTutorias” según los
  requisitos:
    - Estudiantes (PK = Matrícula)
    - Tutores (PK = NúmeroPersonal)
    - Carreras
    - Tutorías (sesiones con fecha, hora inicio/fin y duración)
    - Turnos (citas: matrícula, número personal, tutoriaId, estado, fechaCreación)
    - Estados de turno
    - Cancelaciones y abandonos con fecha, hora y razón
    - Evaluación final de la tutoría (estudiante, tutor, tutoría, comentario)
================================================================================*/

/* 0) Conéctate a master */
USE master;
GO

/* 1) Si existe, eliminar la BD para empezar de cero */
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'TurnosTutorias')
BEGIN
    ALTER DATABASE [TurnosTutorias] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [TurnosTutorias];
END
GO

/* 2) Crear la base de datos y cambiar a ella */
CREATE DATABASE [TurnosTutorias];
GO
USE [TurnosTutorias];
GO

/* 3) Limpiar tablas residuales */
DROP TABLE IF EXISTS dbo.EvaluacionFinal;
DROP TABLE IF EXISTS dbo.TurnoAbandonos;
DROP TABLE IF EXISTS dbo.TurnoCancelaciones;
DROP TABLE IF EXISTS dbo.Turnos;
DROP TABLE IF EXISTS dbo.Tutorias;
DROP TABLE IF EXISTS dbo.Tutores;
DROP TABLE IF EXISTS dbo.Usuarios;
DROP TABLE IF EXISTS dbo.Carreras;
DROP TABLE IF EXISTS dbo.TurnoEstados;
GO

/* 4) Catálogo: Estados de turno */
CREATE TABLE dbo.TurnoEstados (
    EstadoId   INT IDENTITY(1,1) PRIMARY KEY,
    EstadoName NVARCHAR(50) NOT NULL UNIQUE
);
INSERT dbo.TurnoEstados (EstadoName) VALUES
    (N'Pendiente'),
    (N'Atendido'),
    (N'NoAtendido'),
    (N'Cancelado');
GO

/* 5) Catálogo: Carreras */
CREATE TABLE dbo.Carreras (
    CarreraId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre    NVARCHAR(100) NOT NULL UNIQUE
);
GO

/* 6) Entidad: Estudiantes */
CREATE TABLE dbo.Usuarios (
    Matricula       NVARCHAR(20)   PRIMARY KEY,
    Nombre          NVARCHAR(100)  NOT NULL,
    ApellidoPaterno NVARCHAR(100)  NOT NULL,
    ApellidoMaterno NVARCHAR(100)  NOT NULL,
    Email           NVARCHAR(150)  NOT NULL UNIQUE,
    PasswordHash    VARBINARY(256) NOT NULL,
    CarreraId       INT            NULL
        CONSTRAINT FK_Usuarios_Carreras FOREIGN KEY REFERENCES dbo.Carreras(CarreraId),
    FechaRegistro   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/* 7) Entidad: Tutores */
CREATE TABLE dbo.Tutores (
    NumeroPersonal NVARCHAR(20)   PRIMARY KEY,
    Nombre         NVARCHAR(100)  NOT NULL,
    ApellidoPaterno NVARCHAR(100) NOT NULL,
    ApellidoMaterno NVARCHAR(100) NOT NULL,
    Telefono       NVARCHAR(15)   NULL,
    Email          NVARCHAR(150)  NOT NULL UNIQUE,
    PasswordHash   VARBINARY(256) NOT NULL
);
GO

/* 8) Entidad: Tutorías (sesiones) */
CREATE TABLE dbo.Tutorias (
    TutoriaId       INT IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100)     NOT NULL,
    Descripcion     NVARCHAR(250)     NULL,
    Fecha           DATE              NOT NULL,
    HoraInicio      TIME(0)           NOT NULL,
    HoraFin         TIME(0)           NOT NULL,
    DuracionMinutos INT               NOT NULL
);
GO

/* 9) Transaccional: Turnos (citas programadas) */
CREATE TABLE dbo.Turnos (
    TurnoId         INT IDENTITY(1,1) PRIMARY KEY,
    Matricula       NVARCHAR(20)      NOT NULL,
    NumeroPersonal  NVARCHAR(20)      NOT NULL,
    TutoriaId       INT               NOT NULL,
    EstadoId        INT               NOT NULL,
    FechaCreacion   DATETIME2         NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Turnos_Estudiante FOREIGN KEY(Matricula)      REFERENCES dbo.Usuarios(Matricula),
    CONSTRAINT FK_Turnos_Tutor      FOREIGN KEY(NumeroPersonal) REFERENCES dbo.Tutores(NumeroPersonal),
    CONSTRAINT FK_Turnos_Tutoria    FOREIGN KEY(TutoriaId)      REFERENCES dbo.Tutorias(TutoriaId),
    CONSTRAINT FK_Turnos_Estado     FOREIGN KEY(EstadoId)       REFERENCES dbo.TurnoEstados(EstadoId)
);
GO

/* 10) Transaccional: Cancelaciones */
CREATE TABLE dbo.TurnoCancelaciones (
    CancelacionId    INT IDENTITY(1,1) PRIMARY KEY,
    TurnoId          INT               NOT NULL,
    FechaCancelacion DATE              NOT NULL,
    HoraCancelacion  TIME(0)           NOT NULL,
    Razon            NVARCHAR(200)     NOT NULL,
    CONSTRAINT FK_Cancel_Turnos FOREIGN KEY(TurnoId) REFERENCES dbo.Turnos(TurnoId) ON DELETE CASCADE
);
GO

/* 11) Transaccional: Abandonos */
CREATE TABLE dbo.TurnoAbandonos (
    AbandonoId     INT IDENTITY(1,1) PRIMARY KEY,
    TurnoId        INT               NOT NULL,
    FechaAbandono  DATE              NOT NULL,
    HoraAbandono   TIME(0)           NOT NULL,
    Razon          NVARCHAR(200)     NOT NULL,
    CONSTRAINT FK_Abandono_Turnos FOREIGN KEY(TurnoId) REFERENCES dbo.Turnos(TurnoId) ON DELETE CASCADE
);
GO

/* 12) Transaccional: Evaluación final */
CREATE TABLE dbo.EvaluacionFinal (
    EvaluacionId   INT IDENTITY(1,1) PRIMARY KEY,
    TutoriaId      INT               NOT NULL,
    Matricula      NVARCHAR(20)      NOT NULL,
    NumeroPersonal NVARCHAR(20)      NOT NULL,
    Comentario     NVARCHAR(500)     NULL,
    FechaEval      DATETIME2         NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Eval_Tutoria    FOREIGN KEY(TutoriaId)      REFERENCES dbo.Tutorias(TutoriaId),
    CONSTRAINT FK_Eval_Estudiante FOREIGN KEY(Matricula)      REFERENCES dbo.Usuarios(Matricula),
    CONSTRAINT FK_Eval_Tutor      FOREIGN KEY(NumeroPersonal) REFERENCES dbo.Tutores(NumeroPersonal)
);
GO

/* 13) Índices recomendados */
CREATE INDEX IX_Turnos_Estudiante  ON dbo.Turnos(Matricula);
CREATE INDEX IX_Turnos_Tutor       ON dbo.Turnos(NumeroPersonal);
CREATE INDEX IX_Turnos_Tutoria     ON dbo.Turnos(TutoriaId);
CREATE INDEX IX_Turnos_Estado      ON dbo.Turnos(EstadoId);
CREATE INDEX IX_Eval_Tutoria       ON dbo.EvaluacionFinal(TutoriaId);
GO
