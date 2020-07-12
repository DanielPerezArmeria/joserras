CREATE TABLE [dbo].[torneos] (
    [id]           UNIQUEIDENTIFIER CONSTRAINT [DF_torneos_id] DEFAULT (newid()) NOT NULL,
    [fecha]        DATE             NOT NULL,
    [precio_buyin] INT              NOT NULL,
    [precio_rebuy] INT              NOT NULL,
    [entradas]     INT              NOT NULL,
    [rebuys]       INT              NOT NULL,
    [bolsa]        INT              NOT NULL,
    CONSTRAINT [PK_torneos] PRIMARY KEY CLUSTERED ([id] ASC)
);

