namespace card_game.GameLogic// klasė priklauso žaidimo logikos vardų sričiai

{
    // galimos kortų rūšys (širdys, būgnai, kryžiai, vynai)
    public enum Suit
    {
        Hearts,   // širdys
        Diamonds, // būgnai
        Clubs,    // kryžiai
        Spades    // vynai
    }

    // klasė, aprašanti vieną kortą
    public class Card
    {
        // kortos rūšis (širdys, būgnai ir tt)
        public Suit Suit { get; set; }

        // kortos reikšmė (nuo 2 iki 14, kur 11=Valetas, 12=Dama, 13=Karalius, 14=Tūzas)
        public int Value { get; set; } 

        // konstruktorius, kuris sukuria naują kortos objektą su nurodyta rūšimi ir reikšme
        public Card(Suit suit, int value)
        {
            Suit = suit;   // priskiriama rūšis
            Value = value; // priskiriama reikšmė
        }

        // funkcija, kuri paverčia kortos informaciją į tekstinį pavadinimą
        public override string ToString()
        {
            // nustatomas kortos reikšmės simbolis (pvz., 11 -> J, 12 -> Q, 13 -> K, 14 -> A)
            string valueString = Value switch
            {
                11 => "J", // valet
                12 => "Q", // dama
                13 => "K", // karalius
                14 => "A", // tūzas
                _ => Value.ToString() // visos kitos reikšmės rodomos kaip skaičius
            };

            // grąžinamas kortos pavadinimas, pvz., 'K of Hearts' arba '10 of Spades'
            return $"{valueString} of {Suit}";
        }
    }
}
