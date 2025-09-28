using System.Collections.Generic;

namespace card_game.GameLogic
{
    public class GameState
    {
        public List<Player> Players { get; set; }
        public Deck Deck { get; set; }
        public Suit TrumpSuit { get; set; }
        public Dictionary<Card, Card?> TableCards { get; set; } = new();
        public int CurrentAttackerIndex { get; set; }
        public int CurrentDefenderIndex { get; set; }
        public bool IsGameOver { get; set; }

        public GameState(List<Player> players, Deck deck)
        {
            Players = players;
            Deck = deck;
            TrumpSuit = deck.PeekBottomCard()?.Suit ?? Suit.Spades;
            CurrentAttackerIndex = 0;
            CurrentDefenderIndex = 1 % players.Count;
        }

        public Player Attacker => Players[CurrentAttackerIndex];
        public Player Defender => Players[CurrentDefenderIndex];
    }
}
