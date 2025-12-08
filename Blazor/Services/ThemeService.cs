using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class ThemeService
{
    private readonly IJSRuntime _js;
    public bool IsDarkMode { get; private set; }

    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitializeAsync()
    {
        var mode = await _js.InvokeAsync<string>("getTheme");
        IsDarkMode = mode == "dark";
        await ApplyTheme();
    }

    public async Task ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        await ApplyTheme();
        await _js.InvokeVoidAsync("setTheme", IsDarkMode ? "dark" : "light");
        OnThemeChanged?.Invoke();
    }

    private async Task ApplyTheme()
    {
        var theme = IsDarkMode ? "dark" : "light";
        await _js.InvokeVoidAsync("setTheme", theme);
    }
}
