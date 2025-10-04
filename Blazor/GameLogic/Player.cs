using System.Collections.Generic;
using System.Linq;

namespace card_game.GameLogic
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public bool IsBot { get; set; }

        public Player(string name, bool isBot = false)
        {
            Name = name;
            IsBot = isBot;
            Hand = new List<Card>();
        }

        public void ReceiveCard(Card card)
        {
            if (card != null)
                Hand.Add(card);
        }

        public void SortHand()
        {
            Hand = Hand
                .OrderBy(card => card.Suit)
                .ThenBy(card => card.Value)
                .ToList();
        }

        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
        }

        public bool HasCards => Hand.Count > 0;
    }
}
