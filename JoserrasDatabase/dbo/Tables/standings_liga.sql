CREATE TABLE [dbo].[standings_liga]
(
	[liga_id] UNIQUEIDENTIFIER NOT NULL , 
    [jugador_id] UNIQUEIDENTIFIER NOT NULL, 
    [puntos] INT NOT NULL, 
    [premio] INT NOT NULL, 
    PRIMARY KEY ([liga_id], [jugador_id])
)
