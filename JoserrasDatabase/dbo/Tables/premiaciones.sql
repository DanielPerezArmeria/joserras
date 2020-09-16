CREATE TABLE [dbo].[premiaciones]
(
	[id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (newid()), 
    [rango_entradas] VARCHAR(15) NOT NULL,
		[premiacion] VARCHAR(200) NOT NULL
);