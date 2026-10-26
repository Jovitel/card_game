using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CardGame.Blazor; // <- svarbu, kad matytų App klasę
using System.Net.Http;
using card_game.Services;
using CardGame.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Šitas failas turi egzistuoti: App.razor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<UserSession>();

// HttpClient registracija
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7200/") // API HTTPS adresas
});

// SignalR klientas
builder.Services.AddSingleton<GameClientService>();
builder.Services.AddSingleton<CardSettingsService>();

builder.Services.AddScoped<ThemeService>();

await builder.Build().RunAsync();

