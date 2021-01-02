# EfCoreInAction SecondEdition - Part2 branch

Welcome to the Git repo that is associated with part 2, that's chapters 7 to 11, of the the book Entity Framework Core in Action (second edition). Part 2 is called "Entity Framework Core in depth", and is a reference section dealing with EF Core configuration, migrations, concurrency and the DbContext.

The current release of this code is running on:

* EF Core 5.1
* ASP.NET Core 5.0 (You MUST have NET 5.0 installed)

Other branches are:

* `master`, which covers part 1 of the book - chapters 1 to 6.
* `Part3`, which covers part 3 of the book - chapters 12 to 17.

## What you need to install to run the example applications

To run any of the code you need to.

1. Install a development tool, either **Visual Studio** or **Visual Studio Code** (VS Code for short). If you are new to .NET Core development, then I recommend Visual Studio - here is a link on [how to install Visual Studio](http://mng.bz/2x0T).
2. You need to install a SQL Server to run any applications and some of the unit tests. A SQL Server called `localdb` is installed when you install **Visual Studio on Windows** by choosing the "Data storage and processing" feature (VS Code and Visual Studio on Mac needs more work).
3. Clone this repo to your local computer. See Visual Studio tutorial called [Open a project from a repo](https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo).
4. You need NET 5.0 installed for the code to work.

Currently I am using EF Core 5 previews, but that will change when EF Core 5 is released.

## What can you run in this branch?

I have placed all the code relating to part 2 of the book, chapters 7 to 11. The  `BookApp`, which is the ASP.NET Core application, has its database is configured to make the database have the right column types.  

Once you have installed your chosen development tool you can run the BookApp, which is designed to create and seed a SQL Server database on startup. The `BookApp` application can be run from **Visual Studio** or **VS Code**.

## How to find and run the unit tests

Every chapter has a set of unit tests to check that what I say in the book is correct. These unit tests are also useful to you the reader as sometimes seeing the actual code is a quicker way to see how something works.

### How to find the unit tests

The unit tests are all in the `Test` project and uses [xUnit](https://xunit.net/). The unit tests are all in the `UnitTests` directory split into directories based on what they are testing - for instance, the `UnitTests/TestDataLayer` directory have all the tests that work directly with the database.

#### How to run the unit tests

If you are using **Visual Studio**, then its Test feature via the `Test` button found on the top toolbar - see [Microsoft's VS unit test docs](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-your-code).

If you are using **VS Code**, then you use it Test feature, via the tests icon (looks like a scientific flask). VSCode needs some setting up to work with C#, but this repo has the necessary .vscode files set up to run the unit tests - see [VS Code C# docs](https://code.visualstudio.com/docs/languages/csharp) for more on using VS Code C#.

Many of the tests use SQLite in-memory databases which just work. But some tests use SQL Server. If you are using **Visual Studio on Windows** you should install the  the  "Data storage and processing" feature. This should give you a SQL Server called localdb, which the standard unit tests use. If you need to change the SQL Server name etc. then you should change the connection string called `UnitTestConnection` in the appsettings.json file in the `Test` project.

If you are using **Visual Studio on Mac**, or **VS Code on Mac or linux**, then you need to install a SQL Server to allow the unit tests that need that type of database.

### If you have problems with the code

If you are having problems with the application then please post an issue on the [EfCoreInAction-SecondEdition issues page](https://github.com/JonPSmith/EfCoreinAction-SecondEdition/issues), with the stack trace or compile error and I will have a look and get back to you.

## If you find an error in the book

If you find an error in the book, then please add a comment via Manning's LiveBook version of the book. Please make sure you say what section it is in as it's sometimes hard for me to link your comment to the section of the book.
