CREATE TABLE [dbo].[torneos_liga]
(
	[liga_id] UNIQUEIDENTIFIER NOT NULL , 
    [torneo_id] UNIQUEIDENTIFIER NOT NULL, 
    [liga_fee] INT NOT NULL, 
    [puntaje] VARCHAR(200) NULL, 
    PRIMARY KEY ([liga_id], [torneo_id])
)
