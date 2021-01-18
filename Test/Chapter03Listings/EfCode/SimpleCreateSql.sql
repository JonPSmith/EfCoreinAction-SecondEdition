SET NOCOUNT ON;                             
INSERT INTO ExampleEntities]  //#A
   ([MyMessage]) VALUES (@p0);//#A

SELECT [ExampleEntityId] //#B
FROM [ExampleEntities]   //#B
WHERE @@ROWCOUNT = 1 AND //#B
     [ExampleEntityId] = scope_identity(); //#B
#A This inserts (creates) a new row in the table ExampleEntities
#B This reads back the row which was just created

