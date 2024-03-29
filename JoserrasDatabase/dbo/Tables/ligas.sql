﻿CREATE TABLE [dbo].[ligas]
(
	[id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (newid()), 
    [nombre] VARCHAR(40) NOT NULL, 
    [abierta] BIT NOT NULL, 
    [fecha_inicio] DATE NOT NULL DEFAULT (GETDATE()), 
    [fecha_cierre] DATE NULL, 
    [puntaje] VARCHAR(200) NOT NULL,
		[fee] INT NOT NULL, 
    [premiacion] VARCHAR(200) NOT NULL, 
    [desempate] VARCHAR(200) NULL, 
    CONSTRAINT NombreUniqueC UNIQUE([nombre])
);