using System; //   klasėms

using System.Collections.Generic; // sąrašų valdymui

using System.Linq; //  sąrašų filtravimui ir atrankai

using System.Threading.Tasks; // asinchroninių užduočių palaikymui

namespace card_game.GameLogic // nurodo, kad klasė priklauso žaidimo logikos vardų sričiai

{
    // pagrindinė žaidimo logikos klasė – valdo žaidimo eigą, kortas ir ėjimus
    public class DurakGameEngine
    {
        // saugo dabartinę žaidimo būseną (žaidėjai, kaladė, stalas ir kt.)
        public GameState State { get; private set; }

        // konstruktorius, kuris inicijuoja žaidimą su pateiktais žaidėjais
        public DurakGameEngine(List<Player> players)
        {
            // sukuriama nauja kortų kaladė
            var deck = new Deck();

            // sukuriama nauja žaidimo būsena su žaidėjais ir kalade
            State = new GameState(players, deck);

            // išdalinamos pradinės kortos žaidėjams
            DealInitialCards();
        }

        // funkcija, kuri išdalina pradinius 6 kortų rinkinius visiems žaidėjams
        private void DealInitialCards()
        {
            // einama per visus žaidėjus
            foreach (var player in State.Players)
            {
                // kiekvienas žaidėjas gauna kortas, kol turi mažiau nei 6 arba kol kaladė baigiasi
                while (player.Hand.Count < 6 && State.Deck.CardsLeft > 0)
                    player.ReceiveCard(State.Deck.DrawCard()!);

                // surūšiuojamos kortos žaidėjo rankoje pagal rūšį ir vertę
                player.SortHand();
            }
        }

        // funkcija, kuri leidžia žaidėjui atlikti ataką
        public bool Attack(Player attacker, Card attackCard)
        {
            // tikrinama, ar šis žaidėjas yra dabartinis atakuotojas ir turi šią kortą
            if (attacker != State.Attacker || !attacker.Hand.Contains(attackCard))
                return false;

            // jei ant stalo jau yra 6 kortos – atakuoti nebegalima
            if (State.TableCards.Count >= 6)
                return false;

            // tikrinama, ar ši korta apskritai gali būti panaudota atakai pagal žaidimo taisykles
            if (!CanAttackWith(attacker, attackCard))
                return false;

            // korta pašalinama iš žaidėjo rankos
            attacker.RemoveCard(attackCard);

            // korta dedama ant stalo (be gynybos kortos)
            State.TableCards.Add(attackCard, null);
            return true;
        }

        // funkcija, kuri leidžia žaidėjui gintis nuo atakos
        public bool Defend(Player defender, Card attackCard, Card defendCard)
        {
            // tikrinama, ar žaidėjas iš tiesų yra gynėjas ir turi šią kortą
            if (defender != State.Defender || !defender.Hand.Contains(defendCard))
                return false;

            // tikrinama, ar tokia atakos korta yra ant stalo ir ar ji dar nėra atmušta
            if (!State.TableCards.ContainsKey(attackCard) || State.TableCards[attackCard] != null)
                return false;

            // tikrinama, ar gynybos korta laimi prieš atakos kortą
            if (!Beats(attackCard, defendCard))
                return false;

            // pašalinama gynybos korta iš žaidėjo rankos
            defender.RemoveCard(defendCard);

            // priskiriama prie atakuojančios kortos kaip sėkminga gynyba
            State.TableCards[attackCard] = defendCard;
            return true;
        }

        // funkcija, kuri nustato ar gynybos korta nugali atakuojančią kortą
        public bool Beats(Card attack, Card defend)
        {
            // jei abi kortos yra tos pačios rūšies ir gynybos korta turi didesnę vertę – laimi
            if (defend.Suit == attack.Suit && defend.Value > attack.Value)
                return true;

            // jei gynybos korta yra kozeris, o ataka – ne, gynyba laimi
            if (defend.Suit == State.TrumpSuit && attack.Suit != State.TrumpSuit)
                return true;

            // jei abi kortos yra kozerinės – laimi ta, kuri turi didesnę vertę
            if (attack.Suit == State.TrumpSuit && defend.Suit == State.TrumpSuit && defend.Value > attack.Value)
                return true;

            // kitu atveju ataka laimi
            return false;
        }

        // funkcija, kuri patikrina, ar žaidėjas gali atakuoti su nurodyta korta
        public bool CanAttackWith(Player attacker, Card card)
        {
            // jei ant stalo nėra jokių kortų – galima atakuoti bet kuo
            if (State.TableCards.Count == 0)
                return true;

            // surenkamos visos kortų vertės, kurios jau yra ant stalo (atakos ir gynybos)
            var validRanks = State.TableCards
                .SelectMany(p => new[] { p.Key, p.Value })
                .Where(c => c != null)
                .Select(c => c!.Value)
                .ToHashSet();

            // galima atakuoti tik tokiomis vertėmis, kurios jau yra ant stalo
            return validRanks.Contains(card.Value);
        }

        // funkcija, kuri tikrina, ar žaidėjas gali gintis su nurodyta korta
        public bool CanDefendWith(Player defender, Card card)
        {
            // randama paskutinė atakuojanti korta, kuri dar neturi gynybos
            var attackCard = State.TableCards
                .Where(p => p.Value == null)
                .Select(p => p.Key)
                .LastOrDefault();

            // jei nėra tokios kortos, gynyba negalima
            if (attackCard == null)
                return false;

            // patikrinama, ar pasirinkta korta nugali atakuojančią
            return Beats(attackCard, card);
        }

        // funkcija, kuri tikrina, ar žaidėjas gali baigti ėjimą
        public bool CanEndTurn(Player player)
        {
            // ėjimą gali baigti tik atakuotojas, jei ant stalo yra bent viena korta
            return State.Attacker == player && State.TableCards.Count > 0;
        }

        // funkcija, kuri užbaigia ėjimą (kai gynėjas paima kortas arba visos atakos atmuštos)
        public void EndTurn(bool defenderPickedUp)
        {
            //Išvalom stalą
            State.TableCards.Clear();

            //  Papildome rankas kortomis iš kaladės (visada nuo atakuotojo ratu)
            RefillHands();

            // pašalinam žaidėjus be kortų iš aktyvaus rato (bet paliekam juos žiūrėti)
            var activePlayers = State.Players.Where(p => p.Hand.Count > 0).ToList();

            // jei liko mažiau nei 2 aktyvūs – žaidimas baigtas
            if (activePlayers.Count < 2)
            {
                State.IsGameOver = true;
                return;
            }

            // pritaikom naują rotaciją tik pagal aktyvius žaidėjus
            // randam atakuotojo ir gynėjo pozicijas aktyviame sąraše
            var attackerIndex = activePlayers.IndexOf(State.Attacker);
            var defenderIndex = activePlayers.IndexOf(State.Defender);

            // jei atakuotojas iškrito (neturi kortų) — perstumiam prie kito
            if (attackerIndex == -1)
            {
                attackerIndex = 0; // nuo pirmo aktyvaus
                State.Attacker = activePlayers[attackerIndex];
            }

            // jei gynėjas iškrito — perstumiam prie kito aktyvaus
            if (defenderIndex == -1)
            {
                defenderIndex = (attackerIndex + 1) % activePlayers.Count;
                State.Defender = activePlayers[defenderIndex];
            }

            // MULTI-žaidėjų rotacija
            if (activePlayers.Count > 2)
            {
                if (defenderPickedUp)
                {
                    // Gynėjas pasiėmė kortas → naujas atakuotojas po jo
                    attackerIndex = (defenderIndex + 1) % activePlayers.Count;
                    defenderIndex = (attackerIndex + 1) % activePlayers.Count;
                }
                else
                {
                    // Gynėjas apsigynė → tampa nauju atakuotoju
                    attackerIndex = defenderIndex;
                    defenderIndex = (attackerIndex + 1) % activePlayers.Count;
                }
            }
            else
            {
                // === 1v1 režimas ===
                if (!defenderPickedUp)
                {
                    // Gynėjas apsigynė – keičiam roles
                    attackerIndex = defenderIndex;
                    defenderIndex = (attackerIndex + 1) % activePlayers.Count;
                }
                // jei pasiėmė – atakuotojas lieka tas pats
            }

            // Pritaikom rezultatus į bendrą žaidimo būseną
            State.Attacker = activePlayers[attackerIndex];
            State.Defender = activePlayers[defenderIndex];
            State.CurrentAttackerIndex = State.Players.IndexOf(State.Attacker);
            State.CurrentDefenderIndex = State.Players.IndexOf(State.Defender);

            if (State.TableCards.Count > 0)
                return; // negalim baigti round'o kol yra kortų ant stalo


            // Patikrinam, ar dabar dar yra kas žaistų
            CheckGameOver();

        }



        // funkcija, kuri papildomai duoda kortų žaidėjams po kiekvieno ėjimo
        private void RefillHands()
        {
            // pradedama nuo dabartinio atakuotojo ir einama ratu
            for (int i = 0; i < State.Players.Count; i++)
            {
                var player = State.Players[(State.CurrentAttackerIndex + i) % State.Players.Count];
                while (player.Hand.Count < 6 && State.Deck.CardsLeft > 0)
                    player.ReceiveCard(State.Deck.DrawCard()!);
            }
        }

        // funkcija, kuri patikrina, ar žaidimas baigtas
        private void CheckGameOver()
        {
            // jei žaidimas jau pažymėtas baigtu – nieko nedarom
            if (State.IsGameOver)
                return;

            // jei dar yra kortų ant stalo – round'as nebaigtas
            if (State.TableCards.Count > 0)
                return;

            // jei dar yra kortų kaladėje – žaidimas tęsiasi
            if (State.Deck != null && State.Deck.CardsLeft > 0)
                return;

            // aktyvūs žaidėjai – tie, kurie dar turi kortų rankoje
            var activePlayers = State.Players.Where(p => p.Hand.Count > 0).ToList();

            // jei liko tik vienas žaidėjas su kortomis – žaidimas baigtas
            if (activePlayers.Count <= 1)
            {
                State.IsGameOver = true;
            }
        }



        // asinchroninė versija – leidžia vizualiai parodyti pertrauką prieš ėjimo pabaigą
        public async Task EndTurnWithDelay(bool defenderPickedUp, Func<Task>? refreshUI = null)
        {
            // jei perduota funkcija UI atnaujinimui – iškviečiama
            if (refreshUI != null) await refreshUI();

            // padaro pauzė, kad žaidėjas spėtų pamatytų, kas vyksta 
            await Task.Delay(1500);

            // užbaigiamas ėjimas
            EndTurn(defenderPickedUp);

            // po ėjimo atnaujinamas vaizdas
            if (refreshUI != null) await refreshUI();
        }

        // funkcija, kuri vykdo boto ėjimą (atakuoja arba ginasi)
        public async Task RunBotTurn(Func<Task>? refreshUI = null)
        {
            // randamas pirmas bot žaidėjas
            var bot = State.Players.FirstOrDefault(p => p.IsBot);

            // jei boto nėra arba žaidimas baigtas – funkcija nutraukiama
            if (bot is not BotPlayer botPlayer || State.IsGameOver)
                return;

            // jei botas yra gynėjas
            if (State.Defender == botPlayer)
            {
                // randama paskutinė atakuojanti korta, kuri dar nėra atmušta
                var attackCard = State.TableCards.FirstOrDefault(p => p.Value == null).Key;
                if (attackCard == null)
                    return;

                // botas pasirenka kortą gynybai
                var defendCard = botPlayer.ChooseCard(this, forDefense: true, attackCard);

                // jei botas turi kuo atsimušti
                if (defendCard != null)
                {
                    // botas atmuša kortą
                    Defend(botPlayer, attackCard, defendCard);

                    // atnaujinamas ekranas
                    if (refreshUI != null) await refreshUI();

                    // pauzė, kad pamatytų
                    await Task.Delay(1000);

                    // jei visos kortos atmuštos
                    if (State.TableCards.All(p => p.Value != null))
                    {
                        if (State.Attacker.IsBot)
                        {
                            // jei botas buvo atakuotojas, ėjimas baigiamas iškart
                            await EndTurnWithDelay(false, refreshUI);
                            await RunBotTurn(refreshUI);
                        }
                        else
                        {
                            // jei atakavo žmogus – paliekamas laikas pridėti dar kortų
                            if (refreshUI != null) await refreshUI();
                            await Task.Delay(3000); // stalas lieka matomas 3 sek.
                        }
                    }
                }
                else
                {
                    // jei botas negali atsimušti – jis paima kortas
                    if (refreshUI != null) await refreshUI();
                    await Task.Delay(1000);
                    await EndTurnWithDelay(true, refreshUI);
                }
            }
            // jei botas yra atakuotojas
            else if (State.Attacker == botPlayer)
            {
                // botas pasirenka atakos kortą
                var attackCard = botPlayer.ChooseCard(this);

                // jei turi kuo atakuoti
                if (attackCard != null)
                {
                    Attack(botPlayer, attackCard);
                    if (refreshUI != null) await refreshUI();
                    await Task.Delay(1000);
                }
                else
                {
                    // jei nebeturi kuo pulti – baigia ėjimą
                    await EndTurnWithDelay(false, refreshUI);
                }
            }
        }

        // funkcija, kuri grąžina tekstą, rodantį kieno eilė
        public string GetTurnText(Player humanPlayer)
        {
            // jei žaidimas baigtas
            if (State.IsGameOver)
                return "Žaidimas baigtas!";

            // jei dabar žmogaus ėjimas (ataka)
            if (State.Attacker == humanPlayer)
                return "Jūsų eilė (atakuojate)";

            // jei žmogus ginasi
            if (State.Defender == humanPlayer)
                return "Jūsų eilė (ginatės)";

            // jei botas atakuoja
            if (State.Attacker.IsBot)
                return "Boto eilė (atakuoja)";

            // jei botas ginasi
            if (State.Defender.IsBot)
                return "Boto eilė (ginasi)";

            // numatytasis atvejis
            return "";
        }
    }
}
