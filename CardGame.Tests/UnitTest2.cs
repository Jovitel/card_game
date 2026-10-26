namespace CardGame.Tests;

using Xunit;
using System.Collections.Generic;
using card_game.GameLogic;

public class DurakGameEngineNewTests
{
    // ==========================================
    // 1. METODO "Beats" TESTAI
    // ==========================================

    [Fact]
    public void Beats_SameSuitAndHigherValue_ReturnsTrue()
    {
        // Duota: Abidvi kortos tos pačios rūšies, gynybos korta didesnė už atakos kortą
        var players = new List<Player> { new Player("Jonas", false), new Player("Petras", false) };
        var engine = new DurakGameEngine(players);
        engine.State.TrumpSuit = Suit.Hearts; // Kozeris

        var attackCard = new Card(Suit.Spades, 7);
        var defendCard = new Card(Suit.Spades, 10);

        // Kai
        bool result = engine.Beats(attackCard, defendCard);

        // Tada: Gynyba turi būti sėkminga
        Assert.True(result);
    }

    [Fact]
    public void Beats_SameSuitAndLowerValue_ReturnsFalse()
    {
        // Duota: Tos pačios rūšies korta, bet gynybos vertė mažesnė nei atakos
        var players = new List<Player> { new Player("Jonas", false), new Player("Petras", false) };
        var engine = new DurakGameEngine(players);
        engine.State.TrumpSuit = Suit.Hearts;

        var attackCard = new Card(Suit.Spades, 10);
        var defendCard = new Card(Suit.Spades, 7);

        // Kai
        bool result = engine.Beats(attackCard, defendCard);

        // Tada: Gynyba nesėkminga
        Assert.False(result);
    }

    [Fact]
    public void Beats_TrumpCardAgainstNonTrump_ReturnsTrue()
    {
        // Duota: Atakoje paprasta korta, o gynyboje – kozeris
        var players = new List<Player> { new Player("Jonas", false), new Player("Petras", false) };
        var engine = new DurakGameEngine(players);
        engine.State.TrumpSuit = Suit.Hearts; // Kozeris širdys

        var attackCard = new Card(Suit.Spades, 14); // Tūzas (paprasta rūšis)
        var defendCard = new Card(Suit.Hearts, 6);  // Kozeris 6

        // Kai
        bool result = engine.Beats(attackCard, defendCard);

        // Tada: Kozeris laimi prieš paprastą kortą
        Assert.True(result);
    }

    [Fact]
    public void Beats_NonTrumpAgainstTrump_ReturnsFalse()
    {
        // Duota: Atakoje kozeris, o gynyboje – paprasta korta (ne kozeris)
        var players = new List<Player> { new Player("Jonas", false), new Player("Petras", false) };
        var engine = new DurakGameEngine(players);
        engine.State.TrumpSuit = Suit.Hearts;

        var attackCard = new Card(Suit.Hearts, 6);  // Kozeris
        var defendCard = new Card(Suit.Spades, 14); // Paprastas tūzas

        // Kai
        bool result = engine.Beats(attackCard, defendCard);

        // Tada: Paprasta korta negali sumušti kozerio
        Assert.False(result);
    }


    // ==========================================
    // 2. METODO "RunBotTurn" TESTAI
    // ==========================================

    [Fact]
    public async Task RunBotTurn_BotAsAttackerWithCards_PlacesCardOnTable()
    {
        // Duota: Botas yra atakuotojas ir turi kortą rankoje
        var bot = new BotPlayer("Botas");
        var human = new Player("Jonas", isBot: false);
        
        var players = new List<Player> { bot, human };
        var engine = new DurakGameEngine(players);

        // Paruošiame aplinką: išvalome botui ranką ir įdedame vieną aiškią kortą
        bot.Hand.Clear();
        var cardToPlay = new Card(Suit.Diamonds, 9);
        bot.ReceiveCard(cardToPlay);

        // Nustatome roles: botas dabar atakuoja
        engine.State.Attacker = bot;
        engine.State.Defender = human;
        engine.State.TableCards.Clear(); // Tuščias stalas leidžia pulti bet kokia korta

        // Kai: Vykdomas boto ėjimas
        await engine.RunBotTurn();

        // Tada: Patikriname, kad botas tikrai atliko veiksmą ir korta atsidūrė ant stalo
        Assert.Single(engine.State.TableCards);
        Assert.True(engine.State.TableCards.ContainsKey(cardToPlay));
    }
}