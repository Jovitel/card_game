namespace card_game.GameLogic
{
    // galimos kortos
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public class Card
    {
        public Suit Suit { get; set; }
        public int Value { get; set; } // 2–14 (2–10, Jack=11, Queen=12, King=13, Ace=14)

        public Card(Suit suit, int value)
        {
            Suit = suit;
            Value = value;
        }

        public override string ToString()
        {
            string valueString = Value switch
            {
                11 => "J",
                12 => "Q",
                13 => "K",
                14 => "A",
                _ => Value.ToString()
            };

            return $"{valueString} of {Suit}";
        }
    }
}
