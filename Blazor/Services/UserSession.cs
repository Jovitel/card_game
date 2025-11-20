using CardGame.Shared.Models;
using card_game.GameLogic; // čia Card klasė žaidimo logikoje

public class UserSession
{
    public User? CurrentUser { get; private set; }
    public List<GameHistoryEntry> GameHistory { get; private set; } = new();

    public void SetUser(User user) => CurrentUser = user;

    public int UserId => CurrentUser?.Id ?? 0;
    public string Username => CurrentUser?.UserName ?? "Guest";

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
}
