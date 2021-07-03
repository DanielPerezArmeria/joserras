CREATE TABLE [dbo].[knockouts] (
    [id]            UNIQUEIDENTIFIER CONSTRAINT [DF_eliminaciones_id] DEFAULT (newid()) NOT NULL,
    [torneo_id]     UNIQUEIDENTIFIER NOT NULL,
    [jugador_id]    UNIQUEIDENTIFIER NOT NULL,
    [eliminado_id]  UNIQUEIDENTIFIER NOT NULL,
    [eliminaciones] NUMERIC(5, 2)              NOT NULL DEFAULT 0,
    [mano_url] NVARCHAR(100) NULL, 
    CONSTRAINT [FK_elim_jugador] FOREIGN KEY ([jugador_id]) REFERENCES [dbo].[jugadores] ([id]),
    CONSTRAINT [FK_elim_torneos] FOREIGN KEY ([torneo_id]) REFERENCES [dbo].[torneos] ([id]),
    CONSTRAINT [FK_elimelim_jugador] FOREIGN KEY ([eliminado_id]) REFERENCES [dbo].[jugadores] ([id])
);

