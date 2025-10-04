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

        // reik pridet: AttackCard, DefendCard, PassTurn, RefillHands, etc.
        
    }
}
