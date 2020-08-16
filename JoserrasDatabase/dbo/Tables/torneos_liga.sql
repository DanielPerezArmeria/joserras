CREATE TABLE [dbo].[torneos_liga]
(
	[liga_id] UNIQUEIDENTIFIER NOT NULL , 
    [torneo_id] UNIQUEIDENTIFIER NOT NULL, 
    PRIMARY KEY ([liga_id], [torneo_id]) 
)

GO
