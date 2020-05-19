CREATE TABLE [dbo].[Books] (
    [BookId]      INT             
	    IDENTITY (1, 1) NOT NULL, //#A
		CONSTRAINT [PK_Books] //#A
        PRIMARY KEY CLUSTERED //#A
    [Title]       NVARCHAR (256)  NOT NULL, //#B
    [Description] NVARCHAR (MAX)  NULL, //#C
    [Price]       DECIMAL (9, 2) NOT NULL, //#D
    [Publisher]   NVARCHAR (64)   NULL, //#E
    [PublishedOn] DATE            NOT NULL, //#F
    [ImageUrl]    VARCHAR (512)   NULL //#G
);
#A This primary key is defined by convention, with a property of the form: <ClassName>Id and a .NET type of int
#B The Title is part defined by convention, using the .NET type string which becomes NVARCHAR in SQL. The NOT NULL is set by the [Required] data attribute and its size, 256, is set by the data attribute [MaxLength(256)]
#C The Description is defined completely by convention, and can be null because the .NET type string is nullable
#D The Price I define the type and size using Fluent API
#E The Publisher type and and nullability is defined by convention, while the size is set via the data attribute [MaxLength(64)]
#F The PublishedOn SQL type has been set via some Fluent API to be of SQL type date (rather than the default of DATETIME2 (7)
#G The ImageUrl has its type modified from NVARCHAR (that is, two byte Unicode) to VARCHAR (that is, one byte ASCII) via Fluent API, with the size set by the data attribute [MaxLength(256)]

