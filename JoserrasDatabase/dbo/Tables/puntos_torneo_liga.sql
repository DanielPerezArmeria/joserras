CREATE TABLE [dbo].[puntos_torneo_liga]
(
	[liga_id] UNIQUEIDENTIFIER NOT NULL , 
    [jugador_id] UNIQUEIDENTIFIER NOT NULL, 
    [puntos] NUMERIC(5, 2) NOT NULL, 
    [premio] NUMERIC(7, 2) NOT NULL, 
    PRIMARY KEY ([liga_id], [jugador_id])
)
