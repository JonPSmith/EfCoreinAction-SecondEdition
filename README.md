# EfCoreInAction SecondEdition - part1 (master branch)

Welcome to the Git repo that is associated with the first six chapters in the book Entity Framework Core in Action (second edition). This book details how to use Entity Framework Core (EF Core) to develop database access code in .NET 5 applications.

The current release of this code is running on:

* EF Core 5.1
* ASP.NET Core 5.0 (You MUST have NET 5.0 installed)

Other branches are:

* `Part2`, which covers part 2 of the book - chapters 7 to 11.
* `Part3`, which covers part 3 of the book - chapters 12 to 17.

## What you need to install to run the example applications

To run any of the code you need to.

1. Install a development tool, either **Visual Studio** or **Visual Studio Code** (VS Code for short). If you are new to .NET Core development, then I recommend Visual Studio - here is a link on [how to install Visual Studio](http://mng.bz/2x0T).
2. You need to install the NET 5 or above SDK. You can find this a [Microsoft NET download](https://dotnet.microsoft.com/download/dotnet/5.0) - select the **Build apps - SDK** version.  
 *NOTE: If you installed the latest version of **Visual Studio** it should have installed NET 5 SDK for you. Use the console command `dotnet --list-sdks` to check what NET versions are installed.*
3. You need to install a SQL Server to run any applications and some of the unit tests. A SQL Server called `localdb` is installed when you install **Visual Studio on Windows** by choosing the "Data storage and processing" feature (VS Code and Visual Studio on Mac needs more work).
4. Clone this repo to your local computer. See Visual Studio tutorial called [Open a project from a repo](https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo).

## What can you run in this branch?

I have placed all the code relating to part 1 of the book, which covers the first six chapters. The main projects you can run are:

* `MyFirstEfCoreApp`, which is the console application found in chapter 1
* `BookApp`, which is the ASP.NET Core application that I cover in chapters 2 to 6. Best to mark that project as your Startup Project

Once you have installed your chosen development tool, then you can run either of these applications - both are designed to create and seed any database that they use.

## How to find and run the unit tests

Every chapter has a set of unit tests to check that what I say in the book is correct. These unit tests are also useful to you the reader as sometimes seeing the actual code is a quicker way to see how something works.

### How to find the unit tests

The unit tests are all in the `Test` project and uses [xUnit](https://xunit.net/). The unit tests are all in the `UnitTests` directory split into directories based on what they are testing - for instance, the `UnitTests/TestDataLayer` directory have all the tests that work directly with the database.

#### How to run the unit tests

If you are using **Visual Studio**, then its Test feature via the `Test` button found on the top toolbar - see [Microsoft's VS unit test docs](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-your-code).

If you are using , then you use it Test feature, via the tests icon (looks like a scientific flask). VS Code needs some setting up to work with C#, but once thats done then this repo has the necessary .vscode files set up to run the unit tests - see [VS Code C# docs](https://code.visualstudio.com/docs/languages/csharp) for more on using VS Code C#.

Many of the tests use SQLite in-memory databases which just work. But some tests use SQL Server. If you are using **Visual Studio on Windows** you should install the  the  "Data storage and processing" feature. This should give you a SQL Server called localdb, which the standard unit tests use. If you need to change the SQL Server name etc. then you should change the connection string called `UnitTestConnection` in the appsettings.json file in the `Test` project.

If you are using **Visual Studio on Mac**, or **VS Code on Mac or linux**, then you need to install a SQL Server to allow the unit tests that need that type of database.

## If you have problems with the code

If you are having problems with the application then please post an issue on the [EfCoreInAction-SecondEdition issues page](https://github.com/JonPSmith/EfCoreinAction-SecondEdition/issues), with the stack trace or compile error and I will have a look and get back to you.

## If you find an error in the book

If you find an error in the book, then please add a comment via Manning's LiveBook version of the book. Please make sure you say what section it is in as it's sometimes hard for me to link your comment to the section of the book.


 
