using System.Collections.Generic; // sąrašams

namespace card_game.GameLogic // klasė priklauso žaidimo logikos vardų sričiai
{
    // klasė, kuri aprašo visą dabartinę žaidimo būseną
    public class GameState
    {
        // visų žaidime dalyvaujančių žaidėjų sąrašas
        public List<Player> Players { get; set; }

        // kortų kaladė, iš kurios dalinamos kortos
        public Deck Deck { get; set; }

        // dabartinis kozerio tipas (rūšis)
        public Suit TrumpSuit { get; set; }

        // stalo kortos – žodynas, kuriame raktas yra atakuojanti korta, o reikšmė – gynybos korta (gali būti null)
        public Dictionary<Card, Card?> TableCards { get; set; }

        // nurodo, ar žaidimas jau baigtas
        public bool IsGameOver { get; set; }

        // dabartinis ėjimo numeris
        public int TurnNumber { get; set; } = 1;

        // dabartinio atakuotojo indeksas sąraše
        public int CurrentAttackerIndex { get; set; }

        // dabartinio gynėjo indeksas sąraše
        public int CurrentDefenderIndex { get; set; }

        // dabartinis atakuotojas (žaidėjas, kuris puola)
        public Player Attacker { get; set; }   

        // dabartinis gynėjas (žaidėjas, kuris ginasi)
        public Player Defender { get; set; }   

        // konstruktorius, kuris sukuria naują žaidimo būseną su nurodytais žaidėjais ir kalade
        public GameState(List<Player> players, Deck deck)
        {
            Players = players; // išsaugomas žaidėjų sąrašas
            Deck = deck;       // priskiriama kortų kaladė

            // nustatoma, kad pirmas žaidėjas yra atakuotojas
            CurrentAttackerIndex = 0;

            // antras žaidėjas tampa gynėju (1 % players.Count apsaugo, jei žaidėjas tik vienas)
            CurrentDefenderIndex = 1 % players.Count;

            // nustatomi aktyvūs žaidėjai pagal indeksus
            Attacker = Players[CurrentAttackerIndex];
            Defender = Players[CurrentDefenderIndex];

            // sukuriamas tuščias žodynas stalo kortoms laikyti
            TableCards = new Dictionary<Card, Card?>();

            // išsaugomas kaladės kozerio tipas
            TrumpSuit = deck.TrumpSuit;

            // nustatoma, kad žaidimas dar nėra baigtas
            IsGameOver = false;
        }
    }
}
