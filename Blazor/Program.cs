using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CardGame.Blazor; // <- svarbu, kad matytų App klasę
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Šitas failas turi egzistuoti: App.razor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient registracija
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5070/") // tavo API adresas
});
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5070") });
await builder.Build().RunAsync();