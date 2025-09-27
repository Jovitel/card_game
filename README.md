## The card game "Durnius"

# How to use

To edit project and see changes use in the terminal:
**dotnet watch run**

# Project structure
card_game/                     - Root folder of the Blazor project
│
├─ card_game.csproj            - Project file containing build and dependency info
├─ Program.cs                  - Entry point of the application
├─ _Imports.razor              - Common using directives for Razor components
├─ App.razor                   - Root component of the Blazor application
│
├─ wwwroot/                    - Static files folder (images, CSS, JS)
│   ├─ images/                 - Game images (cards, logos, etc.)
│   └─ css/                    - Global CSS files
│
├─ Pages/                      - Razor pages (views) for the application
|   ├─ Main.razor              - Entry page
│   ├─ Start.razor             - Main game page with leaderboard, buttons, and card display
│   ├─ Login.razor             - Login page with email and password validation
|   ├─ Register.razor          - Register if you do not have an account
│   ├─ Rules.razor             - Page displaying game rules
│   ├─ Settings.razor          - User settings page
│   ├─ Friends.razor           - Friends management page
|   ├─ Vs.razor                - Choose how you want to play
│   └─ Join.razor / Vs.razor   - Multiplayer game pages
│
├─ Shared/                     - Shared components used across multiple pages
│   ├─ MainLayout.razor        - Main layout of the application
│   └─ NavMenu.razor           - Navigation menu component
│
├─ Data/                       - Data access / models (if any)
│   └─ Player.cs               - Example model for players
│
└─ _framework/                 - Auto-generated Blazor framework files (do not edit)