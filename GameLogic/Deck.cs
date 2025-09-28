using System;
using System.Collections.Generic;

namespace card_game.GameLogic
{
    public class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck()
        {
            cards = new List<Card>();

            // Generate cards: 6 (value 6) to Ace (value 14) for each suit
            for (int value = 6; value <= 14; value++)
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    cards.Add(new Card(suit, value));
                }
            }

            Shuffle();
        }

        public void Shuffle()
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var temp = cards[k];
                cards[k] = cards[n];
                cards[n] = temp;
            }
        }

        public Card? DrawCard()
        {
            if (cards.Count == 0)
                return null;

            Card topCard = cards[0];
            cards.RemoveAt(0);
            return topCard;
        }

        public int CardsLeft => cards.Count;

        public Card? PeekBottomCard()
        {
            return cards.Count > 0 ? cards[^1] : null;
        }
    }
}
