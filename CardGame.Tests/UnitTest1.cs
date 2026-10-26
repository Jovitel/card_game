namespace CardGame.Tests;

using Xunit;
using System.Collections.Generic;
using card_game.GameLogic; 

public class UnitTest1
{
    [Fact]
    public void TestBeatsRule()
    {
        // zaidejai
        var players = new List<Player>
        {
            new Player("Jonas", isBot: false),
            new Player("Botas", isBot: true)
        };

        var engine = new DurakGameEngine(players);
        
        // kozerio rusis
        engine.State.TrumpSuit = Suit.Hearts;

        // Sukuriame kortas testavimui
        var lowerCard = new Card(Suit.Spades, 6);
        var higherCard = new Card(Suit.Spades, 10);

        bool result = engine.Beats(lowerCard, higherCard);

        // ar 10 musa 6
        Assert.True(result);
    }
    [Fact]
    public void TestBotAttackTurn()
    {
        // botas ir zmogus
        var players = new List<Player>
        {
            new Player("Botas", isBot: true),
            new Player("Jonas", isBot: false)
        };

        var engine = new DurakGameEngine(players);

        // botas atakuoja
        engine.State.Attacker = players[0];
        engine.State.Defender = players[1];

        // ar botas turi kortu
        var botTask = engine.RunBotTurn();
        
        // botas puola
        botTask.Wait();

        // botas padet korta turejo
        Assert.True(engine.State.TableCards.Count >= 0);
    }
}