# EfCoreInAction SecondEdition - part1 (master branch)

Welcome to the Git repo that is associated with the first six chapters in the book Entity Framework Core in Action (second edition). This book details how to use Entity Framework Core (EF Core) to develop database access code in .NET 5 applications.

Other branches are:

* part2, which covers part 2 of the book - chapters 7 to 11.
* part3, which covers part 3 of the book - chapters 12 to 17.

## What you need to install

To run any of the code you need to.

1. Install a development tool, either **Visual Studio** or **Visual Studio Code**. If you are new to .NET Core development, then I recommend Visual Studio - here is a link on [how to install Visual Studio](http://mng.bz/2x0T).
2. Clone this repo to your local computer. See Visual Studio tutorial called [Open a project from a repo](https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo).
3. Currently need to have NET Core 3.1 installed for the code to work.

Currently I am using EF Core 5 previews, but that will change when EF Core 5 is released.

## How to use this branch

I have placed all the code relating to part 1 of the book, which covers the first six chapters. The main projects you can run are:

* `MyFirstEfCoreApp`, which is the console application found in chapter 1
* `BookApp`, which is the ASP.NET Core application that I cover in chapters 2 to 6.

Once you have installed your chosen development tool, then you can run either of these applications - both are designed to create and seed any database that they use.

There ase also unit tests that check what I say in the book is correct. These can aslo be useful to understand how something works. The unit tests are arranged by project, chapter and feature in the `Test.UnitTests` folder. For instance the file in `Test.UnitTests.TestDayaLayer.Ch02_LazyLoading.cs` contains examples and tests of how lazy loading works.


## If you have problems with the code

If you are having problems with the application then please post an issue on the [EfCoreInAction-SecondEdition issues page](https://github.com/JonPSmith/EfCoreinAction-SecondEdition/issues), with the stack trace or compile error and I will have a look and get back to you.

## If you find an error in the book

If you find an error in the book, then please add a comment via Mannings LiveBook version of the book. Please make sure you say what section it is in as it's sometimes hard for me to link your comment to the section of the book.


 