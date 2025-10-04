# The card game "Durnius"

## Getting started

This project was created using Blazor WebAssembly:
```
dotnet new blazorwasm -o card_game
```

After creating the project, you can run it locally and see the changes while editing with:

One terminal: (Opens web)
```
cd card_game/Blazor
dotnet watch run
```
Another terminal: (Opens Swagger)
```
cd card_game/CardGame.Api
dotnet watch run
```
## Project structure
**card_game/ – Root folder of the Blazor project**
* CardGame.sln – Solution file referencing all projects
### Folders
**Blazor/**
* Blazor project for the frontend
* Contains the UI and Razor components
* Typical files:
  * Program.cs – Entry point of the Blazor application
  * _Imports.razor – Common using directives for Razor components
  * App.razor – Root component
* Folders:
  * wwwroot/ – Static files (images, CSS, JS)
  * images/ – Game images (cards, logos, etc.)
  * css/ – Global CSS files
* Pages/ – Razor pages (views) for the application

**CardGame.Api/**
* Backend API project
* Handles data access, authentication, and business logic
* Typical files:
  * Program.cs – Entry point for the API
  * Controllers/ – API controllers (e.g., AuthController.cs)
  * Migrations/ – EF Core database migrations
  * Properties/launchSettings.json – Debug and launch settings
  * CardGame.Api.csproj – Project file with dependencies
    
**Shared/**
* Shared library used by both Blazor and API projects
* Models/ – Data models shared across projects
* CardGame.Shared.csproj – Project file with dependencies

