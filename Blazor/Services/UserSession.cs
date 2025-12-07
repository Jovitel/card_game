using System.Collections.Generic;
using CardGame.Shared.Models;
using card_game.GameLogic;

public class UserSession
{
    public User? CurrentUser { get; private set; }
    public List<GameHistoryEntry> GameHistory { get; private set; } = new();
    public void SetUser(User user) => CurrentUser = user;
    // Būdas pasiekti avataro URL (vietoj SelectedAvatarUrl)
    public string SelectedAvatarUrl => CurrentUser?.AvatarUrl ?? "images/default_avatar.png";
   
    public int UserId => CurrentUser?.Id ?? 0;
    public string Username => CurrentUser?.UserName ?? "Guest";
    
    public event Action? OnChange;
  
    public void AddGameHistoryEntry(int turnNumber, string player, string action, Card? cardPlayed = null, Card? attackCard = null)
    {
        GameHistory.Add(new GameHistoryEntry
        {
            TurnNumber = turnNumber,
            Player = player,
            Action = action,
            CardPlayed = cardPlayed?.ToString(),
            AttackCard = attackCard?.ToString(),
            TimeStamp = DateTime.Now
        });
    }
    // Metodas avataro URL nustatymui (naudojamas Settings.razor)
    public void SetAvatar(string url)
    {
        if (CurrentUser != null)
        {
            CurrentUser.AvatarUrl = url;
            // BŪTINA: Reikės išsaugoti šį pakeitimą duomenų bazėje per API kvietimą.
            NotifyChange();
        }
    }
    
    public void NotifyChange() => OnChange?.Invoke();
    
}