CREATE TABLE [dbo].[eliminaciones] (
    [torneo_id]     UNIQUEIDENTIFIER NOT NULL,
    [jugador_id]    UNIQUEIDENTIFIER NOT NULL,
    [eliminado_id]  UNIQUEIDENTIFIER NOT NULL,
    [eliminaciones] INT              NOT NULL,
    CONSTRAINT [PK_eliminaciones] PRIMARY KEY CLUSTERED ([torneo_id] ASC, [jugador_id] ASC, [eliminado_id] ASC),
    CONSTRAINT [FK_Elim_JElim] FOREIGN KEY ([eliminado_id]) REFERENCES [dbo].[jugadores] ([id]),
    CONSTRAINT [FK_Elim_Jugadores] FOREIGN KEY ([eliminado_id]) REFERENCES [dbo].[jugadores] ([id]),
    CONSTRAINT [FK_Elim_Torneos] FOREIGN KEY ([torneo_id]) REFERENCES [dbo].[torneos] ([id])
);

