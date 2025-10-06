using System; //  reikia atsitiktinių skaičių generavimui
using System.Collections.Generic; // biblioteka sąrašams (List)
using System.Linq; 


namespace card_game.GameLogic //  priklauso žaidimo logikos vardų sričiai

{
    // klasė, kuri aprašo kortų kaladę
    public class Deck
    {
        // privatų sąrašas, kuriame saugomos visos kortos
        private List<Card> cards;

        // atsitiktinių skaičių generatorius, naudojamas kortų maišymui
        private Random random = new Random();

        // kozerio rūšis (pvz. širdys, būgnai ir tt.)
        public Suit TrumpSuit { get; private set; }

        // grąžina, kiek kortų liko kaladėj
        public int CardsLeft => cards.Count;

        // konstruktorius, kuris sukuria naują kortų kaladę
        public Deck()
        {
            // sukuriamas tuščias kortų sąrašas
            cards = new List<Card>();

            // generuojamos  52 kortos (nuo 2 iki tūzo kiekvienai rūšiai)
            for (int value = 2; value <= 14; value++)
            {
                // eina per visas rūšis (H, D, C, S)
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    // pridedama nauja korta su konkrečia rūšimi ir reikšme
                    cards.Add(new Card(suit, value));
                }
            }

            // išmaišomos visos kortos
            Shuffle();

            // paimama paskutinė korta iš kaladės (naudojama kozerio nustatymui)
            var bottomCard = PeekBottomCard();

            // nustatoma, kad kozerio rūšis būtų tokia pati kaip paskutinės kortos
            TrumpSuit = PeekBottomCard()?.Suit ?? Suit.Hearts;
        }

        // funkcija, kuri sumaišo kortas kaladėje atsitiktine tvarka
        public void Shuffle()
        {
            // n = kiek kortų yra kaladėje
            int n = cards.Count;

            // kol dar yra kortų maišymui
            while (n > 1)
            {
                // sumažinamas skaitiklis
                n--;

                // atsitiktinai parenkamas kortos indeksas
                int k = random.Next(n + 1);

                // laikinas kintamasis kortų apsikeitimui
                var temp = cards[k];

                // sukeičia vietomis dvi kortas
                cards[k] = cards[n];
                cards[n] = temp;
            }
        }

        // funkcija, kuri ištraukia viršutinę kortą iš kaladės
        public Card? DrawCard()
        {
            // jei kaladėje nėra kortų, grąžinama null reikšmė
            if (cards.Count == 0)
                return null;

            // paimama pirmoji (viršutinė) korta
            Card topCard = cards[0];

            // pašalinama ši korta iš kaladės
            cards.RemoveAt(0);

            // grąžinama paimta korta
            return topCard;
        }

        // funkcija, kuri grąžina paskutinę kortą (bet jos nepašalina)
        public Card? PeekBottomCard()
        {
            // jei kaladė nėra tuščia, grąžinama paskutinė korta; kitaip – null
            return cards.Count > 0 ? cards[^1] : null;
        }
    }
}
