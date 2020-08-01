CREATE TABLE [dbo].[puntos_liga]
(
	[torneo_id] UNIQUEIDENTIFIER NOT NULL , 
    [jugador_id] UNIQUEIDENTIFIER NOT NULL, 
    [puntos] INT NOT NULL, 
    PRIMARY KEY ([torneo_id], [jugador_id])
)
