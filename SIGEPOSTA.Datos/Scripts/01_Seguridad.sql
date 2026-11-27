IF DB_ID('SIGEPOSTA_DB') IS NULL
BEGIN
    CREATE DATABASE SIGEPOSTA_DB;
END;
GO

USE SIGEPOSTA_DB;
GO

-- Tabla de usuarios
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        IdPersonalSalud INT NULL,
        NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
        HashContrasena NVARCHAR(500) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        UltimoAcceso DATETIME2 NULL
    );
END;
GO

-- Tabla de roles
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        IdRol INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL UNIQUE,
        Descripcion NVARCHAR(200) NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
END;
GO

-- Tabla de permisos
IF OBJECT_ID('dbo.Permisos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permisos
    (
        IdPermiso INT IDENTITY(1,1) PRIMARY KEY,
        Codigo NVARCHAR(80) NOT NULL UNIQUE,
        Nombre NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
END;
GO

-- Relación entre usuarios y roles
IF OBJECT_ID('dbo.UsuarioRoles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioRoles
    (
        IdUsuario INT NOT NULL,
        IdRol INT NOT NULL,
        PRIMARY KEY (IdUsuario, IdRol),
        FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(IdUsuario),
        FOREIGN KEY (IdRol) REFERENCES dbo.Roles(IdRol)
    );
END;
GO

-- Relación entre roles y permisos
IF OBJECT_ID('dbo.RolPermisos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RolPermisos
    (
        IdRol INT NOT NULL,
        IdPermiso INT NOT NULL,
        PRIMARY KEY (IdRol, IdPermiso),
        FOREIGN KEY (IdRol) REFERENCES dbo.Roles(IdRol),
        FOREIGN KEY (IdPermiso) REFERENCES dbo.Permisos(IdPermiso)
    );
END;
GO
