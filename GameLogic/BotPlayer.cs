namespace card_game.GameLogic
{
    public class BotPlayer : Player
    {
        public BotPlayer(string name) : base(name, true)
        {
        }

        // Boto kortos pasirinkimo logika (labai parsata siam kartui)
        public Card? ChooseCard(Card? attackingCard = null)
        {
            if (Hand.Count == 0)
                return null;

            // Grazina zemiausia korta
            var sorted = Hand.OrderBy(card => card.Value).ToList();

            // jei nera attack kortos, tiesiog ismeta pirma
            if (attackingCard == null)
                return sorted.First();

            // bando atsimust su zemiausia korta
            foreach (var card in sorted)
            {
                if (card.Suit == attackingCard.Suit && card.Value > attackingCard.Value)
                    return card;
            }

            // negali atsimust
            return null;
        }
    }
}
