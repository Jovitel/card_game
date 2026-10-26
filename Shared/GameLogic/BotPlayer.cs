using System.Linq;  //  kortų filtravimui ir rūšiavimui

namespace card_game.GameLogic // klasė priklauso žaidimo logikos vardų sričiai
{
    // klasė „BotPlayer“ paveldi „Player“ klasę (tai yra kompiuterinis žaidėjas)
    public class BotPlayer : Player 
    {
        // konstruktorius, kuris sukuria naują botą ir nustato, kad tai yra automatinis žaidėjas
        public BotPlayer(string name) : base(name, true) { } 

        // funkcija, kuri pasirenka kortą atakai arba gynybai, priklausomai nuo situacijos
        public Card? ChooseCard(DurakGameEngine game, bool forDefense = false, Card? attackingCard = null)
        {
            // jei rankoje nėra kortų, grąžinamas null
            if (Hand.Count == 0)
                return null;

            // išsaugomas kozerio tipas (suit)
            var trump = game.State.TrumpSuit;

            // jei tai nėra gynybos fazė (t. y. botas atakuoja)
            if (!forDefense)
            {
                // galimos kortos, su kuriomis galima pulti pagal žaidimo taisykles
                var possible = Hand.Where(c => game.CanAttackWith(this, c)).ToList();

                // jei nėra tinkamų kortų – grąžinamas null
                if (possible.Count == 0)
                    return null;

                // pirmiausia bandoma pulti mažiausios vertės korta, kuri nėra kozeris
                var nonTrumps = possible
                    .Where(c => c.Suit != trump)
                    .OrderBy(c => c.Value)
                    .ToList();

                // jei yra tokių kortų – grąžinama silpniausia (mažiausios vertės)
                if (nonTrumps.Any())
                    return nonTrumps.First();

                // jei neturi ne-kozerinių kortų, naudojamas mažiausias kozeris
                return possible
                    .OrderBy(c => c.Value)
                    .First();
            }
            else
            {
                // jei tai gynybos fazė (botas ginasi)
                if (attackingCard == null)
                    return null; // jei nėra atakuojančios kortos – nieko nedaroma

                // surandamos visos kortos, kurios gali atmušti atakuojančią kortą
                var candidates = Hand
                    .Where(c => game.Beats(attackingCard, c))
                    .ToList();

                // jei nėra nei vienos galimos kortos, botas negali gintis
                if (candidates.Count == 0)
                    return null;

                // bandoma gintis tokia pačia rūšimi (suit) kaip ataka, bet be kozerio
                var nonTrumpDefenses = candidates
                    .Where(c => c.Suit == attackingCard.Suit && c.Suit != trump)
                    .OrderBy(c => c.Value)
                    .ToList();

                // jei yra tinkamų ne-kozerinių gynybos kortų, pasirenkama silpniausia
                if (nonTrumpDefenses.Any())
                    return nonTrumpDefenses.First();

                // jei nėra ne-kozerinių kortų, bandoma gintis kozeriu (silpniausiu iš jų)
                var trumpDefenses = candidates
                    .Where(c => c.Suit == trump)
                    .OrderBy(c => c.Value)
                    .ToList();

                // jei turi kozerinių gynybos kortų, pasirenkama silpniausia
                if (trumpDefenses.Any())
                    return trumpDefenses.First();

                // jei nėra nei vienos kortos, kuri galėtų atmušti ataką – grąžinamas null
                return null;
            }
        }

        //funkcija, leidžianti botui pridėti papildomų atakos kortų po pirmos atakos
        public List<Card> ChooseAdditionalAttackCards(DurakGameEngine game)
        {
            // sukuriamas sąrašas papildomų kortų, kurias botas gali pridėti
            var additionalCards = new List<Card>();

            // jei rankoje nėra kortų arba stalas tuščias – nieko nepridedama
            if (Hand.Count == 0 || game.State.TableCards.Count == 0)
                return additionalCards;

            // išsaugomas kozerio tipas
            var trump = game.State.TrumpSuit;

            // gaunamos visų kortų reikšmės, kurios jau yra ant stalo (atakos ir gynybos)
            var validRanks = game.State.TableCards
                .SelectMany(pair => new[] { pair.Key, pair.Value })
                .Where(c => c != null)
                .Select(c => c!.Value)
                .Distinct()
                .ToList();

            // surandamos kortos, kurias galima pridėti (tos pačios reikšmės kaip ant stalo)
            var possibleAdditions = Hand
                .Where(c => validRanks.Contains(c.Value) && c.Suit != trump)
                .OrderBy(c => c.Value)
                .ToList();

            // jei yra bent viena korta, kuri tinka – pridėti jas visas po vieną (iki 6 kortų limito)
            foreach (var card in possibleAdditions)
            {
                // patikrinama, ar stalas dar neturi 6 kortų limitą
                if (game.State.TableCards.Count + additionalCards.Count >= 6)
                    break;

                // korta pridedama prie papildomų atakos kortų sąrašo
                additionalCards.Add(card);
            }

            // grąžinamos visos papildomos atakos kortos
            return additionalCards;
        }
    }
}
