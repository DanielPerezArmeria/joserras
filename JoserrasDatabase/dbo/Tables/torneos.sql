CREATE TABLE [dbo].[torneos] (
    [id]           UNIQUEIDENTIFIER CONSTRAINT [DF_torneos_id] DEFAULT (newid()) NOT NULL,
    [fecha]        DATE             NOT NULL,
    [precio_buyin] INT              NOT NULL,
    [precio_rebuy] INT              NOT NULL,
    [entradas]     INT              NOT NULL,
    [rebuys]       INT              NOT NULL,
    [bolsa]        INT              NOT NULL,
    [tipo] NCHAR(20) NOT NULL DEFAULT 'NORMAL', 
    [premio_x_bounty] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_torneos] PRIMARY KEY CLUSTERED ([id] ASC)
);

