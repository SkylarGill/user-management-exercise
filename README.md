# User Management Technical Exercise

[![.NET](https://github.com/SkylarGill/user-management-exercise/actions/workflows/dotnet.yml/badge.svg)](https://github.com/SkylarGill/user-management-exercise/actions/workflows/dotnet.yml)

## Task Implementation Notes

### Repo Overview and Branches

The `main` branch contains the latest, up-to-date changes.

Each task was developed on its own branch and squash merged into `main` so each commit completes a task. Task branches match the format `taskX-description`. There is also a pull request for each branch.

There is a CI GitHub Action for building the solution and running unit tests in PRs, in addition to commits to `main`. 

### Packages

- **FluentValidation** 
  - I used FluentValidation for implementing the request validation for create/update actions. 
  - Library for implementing easily-understandable validation services. 
  - No extra setup steps are required beyond restoring NuGet packages.
- **MockQueryable.Moq**
  - Used to mock `IQueryables` that work with Entity Framework's asynchronous LINQ queries
  - No extra setup steps are required beyond restoring NuGet packages.

### Task 5 - Extend

For this task, I opted to update the data access layer to use asynchronous operations.

I found some conflicting information online about whether `.ConfigureAwait(false)` is still necessary in newer versions of ASP.NET due to no `SynchronizationContext` being provided. 

This [blog article from Microsoft](https://devblogs.microsoft.com/dotnet/configureawait-faq/) indicates that external libraries may introduce a `SynchronizationContext` that could result in potential deadlocks. Therefore, I included `.ConfigureAwait(false)` on all `async` calls.

## Original Task

The exercise is an ASP.NET Core web application backed by Entity Framework Core, which faciliates management of some fictional users.
We recommend that you use [Visual Studio (Community Edition)](https://visualstudio.microsoft.com/downloads) or [Visual Studio Code](https://code.visualstudio.com/Download) to run and modify the application. 

**The application uses an in-memory database, so changes will not be persisted between executions.**

## The Exercise
Complete as many of the tasks below as you can. These are split into 3 levels of difficulty 
* **Standard** - Functionality that is common when working as a web developer
* **Advanced** - Slightly more technical tasks and problem solving
* **Expert** - Tasks with a higher level of problem solving and architecture needed

### 1. Filters Section (Standard)

The users page contains 3 buttons below the user listing - **Show All**, **Active Only** and **Non Active**. Show All has already been implemented. Implement the remaining buttons using the following logic:
* Active Only – This should show only users where their `IsActive` property is set to `true`
* Non Active – This should show only users where their `IsActive` property is set to `false`

### 2. User Model Properties (Standard)

Add a new property to the `User` class in the system called `DateOfBirth` which is to be used and displayed in relevant sections of the app.

### 3. Actions Section (Standard)

Create the code and UI flows for the following actions
* **Add** – A screen that allows you to create a new user and return to the list
* **View** - A screen that displays the information about a user
* **Edit** – A screen that allows you to edit a selected user from the list  
* **Delete** – A screen that allows you to delete a selected user from the list

Each of these screens should contain appropriate data validation, which is communicated to the end user.

### 4. Data Logging (Advanced)

Extend the system to capture log information regarding primary actions performed on each user in the app.
* In the **View** screen there should be a list of all actions that have been performed against that user. 
* There should be a new **Logs** page, containing a list of log entries across the application.
* In the Logs page, the user should be able to click into each entry to see more detail about it.
* In the Logs page, think about how you can provide a good user experience - even when there are many log entries.

### 5. Extend the Application (Expert)

Make a significant architectural change that improves the application.
Structurally, the user management application is very simple, and there are many ways it can be made more maintainable, scalable or testable.
Some ideas are:
* Re-implement the UI using a client side framework connecting to an API. Use of Blazor is preferred, but if you are more familiar with other frameworks, feel free to use them.
* Update the data access layer to support asynchronous operations.
* Implement authentication and login based on the users being stored.
* Implement bundling of static assets.
* Update the data access layer to use a real database, and implement database schema migrations.

## Additional Notes

* Please feel free to change or refactor any code that has been supplied within the solution and think about clean maintainable code and architecture when extending the project.
* If any additional packages, tools or setup are required to run your completed version, please document these thoroughly.
