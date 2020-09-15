/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

insert into premiaciones (rango_entradas, premiacion) values ('1-4', '100%');
insert into premiaciones (rango_entradas, premiacion) values ('5-9', '70%-30%');
insert into premiaciones (rango_entradas, premiacion) values ('10-27', '55%-30%-15%');
insert into premiaciones (rango_entradas, premiacion) values ('28', '55%-30%-15%-1100');