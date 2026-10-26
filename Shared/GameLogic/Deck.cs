using System;
using System.Collections.Generic;
using System.Linq;

namespace card_game.GameLogic
{
    public class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        // Kozerio rūšis (suit)
        public Suit TrumpSuit { get; private set; }

        // Kozerio korta (paskutinė kaladėje)
        public Card? TrumpCard { get; private set; }

        // Kiek kortų liko
        public int CardsLeft => cards.Count;

        // Konstruktorius — sukuria, išmaišo ir nustato kozerį
        public Deck()
        {
            cards = new List<Card>();

            // Sukuriamos 52 kortos (nuo 2 iki A kiekvienai rūšiai)
            for (int value = 2; value <= 14; value++)
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    cards.Add(new Card(suit, value));
                }
            }

            Shuffle();

            // Paskutinė korta yra kozeris
            var bottomCard = cards[^1];
            TrumpCard = bottomCard;
            TrumpSuit = bottomCard.Suit;
        }
        // 🔹 Alternatyvus konstruktorius — naudojamas sinchronizuojant būseną tarp klientų
        public Deck(List<Card> existingCards, Card? trumpCard = null)
        {
            cards = existingCards;
            if (trumpCard != null)
            {
                TrumpCard = trumpCard;
                TrumpSuit = trumpCard.Suit;
            }
            else if (cards.Count > 0)
            {
                TrumpCard = cards[^1];
                TrumpSuit = cards[^1].Suit;
            }
        }
        public List<Card> GetAllCards()
        {
            // Grąžiname visas kortas, išskyrus kozerio kortą
            var list = new List<Card>(cards);

            if (TrumpCard != null)
            {
                // užtikrinam, kad ta pati korta nebūtų įtraukta du kartus
                list.RemoveAll(c => c.Suit == TrumpCard.Suit && c.Value == TrumpCard.Value);
            }

            return list;
        }

        // Kortų maišymas (Fisher-Yates algoritmas)
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

        // Kortos traukimas
    private bool trumpTaken = false;

public Card? DrawCard()
{
    // Jei dar turim kortų – traukiam paprastai
    if (cards.Count > 0)
    {
        // jei liko tik kozeris — paimam ją VIENĄ kartą
        if (cards.Count == 1 && cards[0].Equals(TrumpCard))
        {
            trumpTaken = true;
            var last = cards[0];
            cards.Clear();
            return last;
        }

        // įprastai traukiam iš viršaus
        Card topCard = cards[0];
        cards.RemoveAt(0);
        return topCard;
    }

    // Jei kortų nebėra, bet kozeris dar nepaimtas — leidžiam jį paimti vieną kartą
    if (!trumpTaken && TrumpCard != null)
    {
        trumpTaken = true;
        return TrumpCard;
    }

    // Visiškai tuščia kaladė
    return null;
}



        // gauti paskutinę kortą (kozerį)
        public Card? PeekBottomCard()
        {
            return TrumpCard;
        }

        // pakeisti kozerio kortą (naudojama kai prisijungia guest)
        public void OverrideTrumpCard(Card card)
        {
            if (cards.Count > 0)
            {
                cards[^1] = card;
                TrumpCard = card;
                TrumpSuit = card.Suit;
            }
        }

        public void OverrideToEmpty()
        {
            cards.Clear();
        }
    }

}
