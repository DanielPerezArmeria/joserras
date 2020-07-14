CREATE TABLE [dbo].[eliminaciones] (
    [id]            UNIQUEIDENTIFIER CONSTRAINT [DF_eliminaciones_id] DEFAULT (newid()) NOT NULL,
    [torneo_id]     UNIQUEIDENTIFIER NOT NULL,
    [jugador_id]    UNIQUEIDENTIFIER NOT NULL,
    [eliminado_id]  UNIQUEIDENTIFIER NOT NULL,
    [eliminaciones] INT              NOT NULL
);

