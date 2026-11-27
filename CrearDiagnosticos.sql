IF OBJECT_ID('dbo.Diagnosticos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Diagnosticos
    (
        IdDiagnostico INT IDENTITY(1,1) PRIMARY KEY,
        IdAtencion INT NOT NULL,
        CodigoCIE10 NVARCHAR(20) NOT NULL,
        Descripcion NVARCHAR(500) NOT NULL,
        TipoDiagnostico NVARCHAR(30) NOT NULL,
        FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        Observaciones NVARCHAR(1000) NULL,
        Estado NVARCHAR(30) NOT NULL DEFAULT 'Activo',

        CONSTRAINT FK_Diagnosticos_Atenciones
        FOREIGN KEY (IdAtencion)
        REFERENCES dbo.Atenciones(IdAtencion)
    );
END;
