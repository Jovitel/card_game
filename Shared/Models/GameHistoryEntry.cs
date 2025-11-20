using System;

namespace CardGame.Shared.Models
{
    public class GameHistoryEntry
    {
        public int TurnNumber { get; set; } 
        public string Player { get; set; } = "";
        public string Action { get; set; } = ""; // ATTACK / DEFEND / ENDTURN

        // Kortos saugojimas kaip tekstas, kad Shared projektas nepriklausytų nuo GameLogic
        public string? CardPlayed { get; set; } 
        public string? AttackCard { get; set; } 

        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
