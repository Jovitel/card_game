using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CardGame.Shared.Models;

public class ThemeService
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;
    private readonly UserSession _userSession;

    public bool IsDarkMode { get; private set; }
    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime js, HttpClient http, UserSession userSession)
    {
        _js = js;
        _http = http;
        _userSession = userSession;
    }

    public async Task InitializeAsync()
    {
        string theme = "dark"; // numatytoji tamsi

        if (_userSession.UserId > 0)
        {
            try
            {
                var user = await _http.GetFromJsonAsync<User>($"api/users/{_userSession.UserId}");
                if (user != null && !string.IsNullOrEmpty(user.ThemePreference))
                {
                    theme = user.ThemePreference;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Klaida gaunant temą: {ex.Message}");
            }
        }

        IsDarkMode = theme == "dark";
        await ApplyTheme();
    }

    public async Task ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        var theme = IsDarkMode ? "dark" : "light";

        // Išsaugome tik serveryje
        if (_userSession.UserId > 0)
        {
            await _http.PostAsync($"api/users/settheme/{_userSession.UserId}/{theme}", null);
        }

        await ApplyTheme();
        OnThemeChanged?.Invoke();
    }

    private async Task ApplyTheme()
    {
        var theme = IsDarkMode ? "dark" : "light";
        await _js.InvokeVoidAsync("setTheme", theme);
    }
}
