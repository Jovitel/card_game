# The card game "Durnius"

## Getting started

This project was created using Blazor WebAssembly:
```
dotnet new blazorwasm -o card_game
```

After creating the project, you can run it locally and see the changes while editing with:
```
cd card_game
dotnet watch run
```
## Project structure
card_game/ – Root folder of the Blazor project
* card_game.csproj – Project file containing build and dependency information
* Program.cs – Entry point of the application
* _Imports.razor – Common using directives for Razor components
* App.razor – Root component of the Blazor application

### Folders
* wwwroot/ – Static files (images, CSS, JS)
  * images/ – Game images (cards, logos, etc.)
  * css/ – Global CSS files
* Pages/ – Razor pages (views) for the application
  * Main.razor – Entry page
  * Start.razor – Main game page with leaderboard, buttons, and card display
  * Login.razor – Login page with email and password validation
  * Register.razor – Registration page for new users
  * Rules.razor – Page displaying game rules
  * Settings.razor – User settings page
  * Friends.razor – Friends management page
  * Vs.razor – Choose how you want to play
  * Join.razor – Multiplayer game page
* Shared/ – Shared components used across multiple pages
  * MainLayout.razor – Main layout of the application
  * NavMenu.razor – Navigation menu component
* Data/ – Data access / models (if any)
  * Player.cs – Example model for players
* _framework/ – Auto-generated Blazor framework files (do not edit)
