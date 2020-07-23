CREATE TABLE [dbo].[eliminaciones] (
    [id]            UNIQUEIDENTIFIER CONSTRAINT [DF_eliminaciones_id] DEFAULT (newid()) NOT NULL,
    [torneo_id]     UNIQUEIDENTIFIER NOT NULL,
    [jugador_id]    UNIQUEIDENTIFIER NOT NULL,
    [eliminado_id]  UNIQUEIDENTIFIER NOT NULL,
    [eliminaciones] INT              NOT NULL,
    CONSTRAINT [FK_elim_jugador] FOREIGN KEY ([jugador_id]) REFERENCES [dbo].[jugadores] ([id]),
    CONSTRAINT [FK_elim_torneos] FOREIGN KEY ([torneo_id]) REFERENCES [dbo].[torneos] ([id]),
    CONSTRAINT [FK_elimelim_jugador] FOREIGN KEY ([eliminado_id]) REFERENCES [dbo].[jugadores] ([id])
);

