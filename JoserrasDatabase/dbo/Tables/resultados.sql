CREATE TABLE [dbo].[resultados] (
    [torneo_id]  UNIQUEIDENTIFIER NOT NULL,
    [jugador_id] UNIQUEIDENTIFIER NOT NULL,
    [rebuys]     INT              NOT NULL,
    [posicion]   INT              NOT NULL,
    [podio]      BIT              NOT NULL,
    [premio]     INT              NOT NULL,
    [burbuja]    BIT              NULL,
    [premio_bounties] INT NOT NULL DEFAULT 0, 
    [kos] INT NOT NULL DEFAULT 0, 
    [puntualidad] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_DetalleTorneos] PRIMARY KEY CLUSTERED ([torneo_id] ASC, [jugador_id] ASC),
    CONSTRAINT [FK_DetalleTorneos_Jugadores] FOREIGN KEY ([jugador_id]) REFERENCES [dbo].[jugadores] ([id]),
    CONSTRAINT [FK_DetalleTorneos_Torneos] FOREIGN KEY ([torneo_id]) REFERENCES [dbo].[torneos] ([id])
);

