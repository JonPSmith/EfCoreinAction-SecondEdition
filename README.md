# EfCoreInAction SecondEdition - Part2 branch

Welcome to the Git repo that is associated with part 2, that's chapters 7 to 11, of the the book Entity Framework Core in Action (second edition). Part 2 is called "Entity Framework Core in depth", and is a reference section deaing with EF Core configuration, migrations, concurrency and the DbContext.

Other branches are:

* master, which covers part 1 of the book - chapters 1 to 6.
* Part3, which covers part 3 of the book - chapters 12 to 17.


## How to use this branch

I have placed all the code relating to part 2 of the book, chapters 7 to 11. The  `BookApp` database is configured to make the database have the right column types.  

Once you have installed your chosen development tool you can run the BookApp, which is designed to create and seed its database on startup.

Also, there are  unit tests that check what I say in the book is correct. These can be useful to understand how something works. The unit tests are arranged by project, chapter and feature in the `Test.UnitTests` folder. 

## What you need to install

To run any of the code you need to.

1. Install a development tool, either **Visual Studio** or **Visual Studio Code**. If you are new to .NET Core development, then I recommend Visual Studio - here is a link on [how to install Visual Studio](http://mng.bz/2x0T).
2. Clone this repo to your local computer. See Visual Studio tutorial called [Open a project from a repo](https://docs.microsoft.com/en-us/visualstudio/get-started/tutorial-open-project-from-repo).
3. Currently need to have NET Core 3.1 installed for the code to work.

Currently I am using EF Core 5 previews, but that will change when EF Core 5 is released.


## If you have problems with the code

If you are having problems with the application then please post an issue on the [EfCoreInAction-SecondEdition issues page](https://github.com/JonPSmith/EfCoreinAction-SecondEdition/issues), with the stack trace or compile error and I will have a look and get back to you.

## If you find an error in the book

If you find an error in the book, then please add a comment via Mannings LiveBook version of the book. Please make sure you say what section it is in as it's sometimes hard for me to link your comment to the section of the book.


 