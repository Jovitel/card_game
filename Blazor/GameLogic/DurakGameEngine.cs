using System;
using System.Collections.Generic;

namespace card_game.GameLogic
{
    public class DurakGameEngine
    {
        public GameState State { get; private set; }

        public DurakGameEngine(List<Player> players)
        {
            var deck = new Deck();
            State = new GameState(players, deck);

            DealInitialCards();
        }

        private void DealInitialCards()
        {
            foreach (var player in State.Players)
            {
                while (player.Hand.Count < 6 && State.Deck.CardsLeft > 0)
                {
                    player.ReceiveCard(State.Deck.DrawCard()!);
                }
                player.SortHand();
            }
        }

        
        public bool Attack(Player attacker, Card attackCard)
        {
            // Gali atakuoti tik tuo atveju jei atakuotojo eile ir turi korta
            if (attacker != State.Attacker || !attacker.Hand.Contains(attackCard))
                return false;

        
            // Negali atakuoti jei ant stalo daugiau nei 6 kortos
            if (State.TableCards.Count >= 6)
                return false;

            attacker.RemoveCard(attackCard);
            State.TableCards.Add(attackCard, null);

            return true;
        }

        public bool Defend(Player defender, Card attackCard, Card defendCard)
        {
        
            // Patikrina ar tinkamas gynejas
            if (defender != State.Defender || !defender.Hand.Contains(defendCard))
                return false;

            // Patikrina ar ant stalo yra korta kuria reikia atkirsti
            if (!State.TableCards.ContainsKey(attackCard) || State.TableCards[attackCard] != null)
                return false;


            // Tikrina ar atkirtimo korta nugali ismeta korta
            if (!Beats(attackCard, defendCard))
                return false;

            defender.RemoveCard(defendCard);
            State.TableCards[attackCard] = defendCard;

            return true;
        }
        private bool Beats(Card attack, Card defend)
        {
            if (defend.Suit == attack.Suit && defend.Value > attack.Value)
                return true;

            if (defend.Suit == State.TrumpSuit && attack.Suit != State.TrumpSuit)
                return true;

            return false;
        }


        public void EndTurn(bool defenderPickedUp)
        {

            // Jei gynejas paima kortas, tai visos kortos eina į jo rankas
            if (defenderPickedUp)
            {
                foreach (var pair in State.TableCards)
                {
                    State.Defender.ReceiveCard(pair.Key);
                    if (pair.Value != null)
                        State.Defender.ReceiveCard(pair.Value);
                }
            }

            State.TableCards.Clear();

            RefillHands();

            if (defenderPickedUp)
            {
            
                // Atakuotojas pasilieka tas pats, gynejas tampa sekantsi zaidejas
                State.CurrentDefenderIndex = (State.CurrentDefenderIndex + 1) % State.Players.Count;
            }
            else
            {
            
                // Gynejas tampa naujas atakuotojas, kitas zaidejas tampa gyneju
                State.CurrentAttackerIndex = State.CurrentDefenderIndex;
                State.CurrentDefenderIndex = (State.CurrentAttackerIndex + 1) % State.Players.Count;
            }

            CheckGameOver();
        }

        private void RefillHands()
        {
            // Start from attacker and go clockwise
            // Pradeda nuo atakuotojo ir eina pagal laikrodį 
            for (int i = 0; i < State.Players.Count; i++)
            {
                var player = State.Players[(State.CurrentAttackerIndex + i) % State.Players.Count];
                while (player.Hand.Count < 6 && State.Deck.CardsLeft > 0)
                {
                    player.ReceiveCard(State.Deck.DrawCard()!);
                }
            }
        }


        private void CheckGameOver()
        {
            int activePlayers = State.Players.Count(p => p.HasCards || State.Deck.CardsLeft > 0);

            if (activePlayers <= 1)
                State.IsGameOver = true;
        }



    }
}
