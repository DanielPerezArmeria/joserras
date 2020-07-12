CREATE TABLE [dbo].[jugadores] (
    [id]     UNIQUEIDENTIFIER CONSTRAINT [DF_jugadores_id] DEFAULT (newid()) NOT NULL,
    [nombre] VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_jugadores] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [UNQ_nombre] UNIQUE NONCLUSTERED ([nombre] ASC)
);

