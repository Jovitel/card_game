using System.Collections.Generic; //  reikalingas sąrašams 

using System.Linq; //  kortų rūšiavimui

namespace card_game.GameLogic // klasė priklauso žaidimo logikos vardų sričiai

{
    // klasė, aprašanti žaidėją (tiek žmogų, tiek botą)
    public class Player
    {
        // žaidėjo vardas
        public string Name { get; set; }

        // žaidėjo turimų kortų sąrašas
        public List<Card> Hand { get; set; }

        // nurodo, ar žaidėjas yra botas (kompiuterinis oponentas)
        public bool IsBot { get; set; }

        // konstruktorius, kuris sukuria naują žaidėją su vardu ir nurodo, ar tai botas
        public Player(string name, bool isBot = false)
        {
            Name = name;      // nustatomas žaidėjo vardas
            IsBot = isBot;    // pažymima, ar žaidėjas yra botas
            Hand = new List<Card>(); // sukuriamas tuščias kortų sąrašas
        }

        // funkcija, kuri prideda naują kortą į žaidėjo ranką
        public void ReceiveCard(Card card)
        {
            // jei korta nėra null, ji pridedama į sąrašą
            if (card != null)
                Hand.Add(card);
        }

        // funkcija, kuri surūšiuoja kortas pagal rūšį ir vertę
        public void SortHand()
        {
            // rūšiuojama pagal rūšį, tada pagal kortos vertę (nuo mažiausios iki didžiausios)
            Hand = Hand
                .OrderBy(card => card.Suit)
                .ThenBy(card => card.Value)
                .ToList();
        }

        // funkcija, kuri pašalina kortą iš žaidėjo rankos
        public void RemoveCard(Card card)
        {
            // pašalinama nurodyta korta
            Hand.Remove(card);
        }

        // savybė, kuri nurodo, ar žaidėjas dar turi kortų
        public bool HasCards => Hand.Count > 0;

        // funkcija, kuri patikrina, ar žaidėjas gali sužaisti pasirinktą kortą pagal žaidimo taisykles
        public bool CanPlayCard(DurakGameEngine game, Card card)
        {
            // jei žaidėjas yra atakuotojas – tikrinama, ar gali pulti su šia korta
            if (this == game.State.Attacker)
                return game.CanAttackWith(this, card);

            // jei žaidėjas yra gynėjas – tikrinama, ar gali gintis su šia korta
            if (this == game.State.Defender)
                return game.CanDefendWith(this, card);

            // jei žaidėjas nėra nei atakuotojas, nei gynėjas – žaisti negali
            return false;
        }
    }
}
