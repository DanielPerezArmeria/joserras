CREATE TABLE [dbo].[ligas]
(
	[id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (newid()), 
    [nombre] VARCHAR(15) NOT NULL, 
    [abierta] BIT NOT NULL, 
    [fecha_creada] DATE NOT NULL DEFAULT (GETDATE()), 
    [fecha_cerrada] DATE NULL, 
    [puntaje] VARCHAR(200) NULL,
		CONSTRAINT NombreUniqueC UNIQUE([nombre])
);