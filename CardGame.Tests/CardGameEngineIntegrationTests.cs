
namespace CardGame.Tests;

using Xunit;
using System.Collections.Generic;
using System.Linq;
using card_game.GameLogic;

public class GameEngineIntegrationTests
{
    [Fact]
    public void Attack_WithValidCard_RemovesCardFromHandAndPlacesItOnTable()
    {
        // Arrange – paruošiami žaidėjai ir žaidimo variklis
        var attacker = new Player("Jonas");
        var defender = new Player("Petras");

        var players = new List<Player> { attacker, defender };
        var engine = new DurakGameEngine(players);

        // Parenkama korta, kuri tikrai priklauso atakuotojui
        var attackCard = attacker.Hand.First();

        // Act – žaidėjas atakuoja
        bool result = engine.Attack(attacker, attackCard);

        // Assert – tikrinamas bendras žaidimo rezultatas
        Assert.True(result);
        Assert.DoesNotContain(attackCard, attacker.Hand);
        Assert.Contains(attackCard, engine.State.TableCards.Keys);
        Assert.Null(engine.State.TableCards[attackCard]);
    }
}
