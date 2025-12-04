
// Services/CardSettingsService.cs

namespace card_game.Services
{
    public class CardSettingsService
    {
        // Numatytoji spalva - raudona
        public string SelectedCardColor { get; private set; } = "red"; 

        // Įvykis, kuris bus paleistas, kai spalva pasikeis
        public event Action? OnChange;

        // Metodas spalvai pakeisti
        public void SetCardColor(string color)
        {
            // Tikrina ar spalva yra tinkama (pvz., red, blue, green)
            // Paprastumo dėlei, čia tik tiesioginis nustatymas
            SelectedCardColor = color.ToLowerInvariant(); 
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
