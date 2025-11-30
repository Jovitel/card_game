using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardGame.Api.Hubs
{
    public class GameHub : Hub
    {
        // Kambario kodas → žaidėjų sąrašas (pagal vardus)
        private static readonly ConcurrentDictionary<string, List<string>> _rooms = new();

        // Kambario kodas → žaidimo režimas ("1v1" arba "1vsmulti")
        private static readonly ConcurrentDictionary<string, string> _roomModes = new();

        // Prisijungimų žemėlapis: connectionId → playerName
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        // KAMBARIO KŪRIMAS
        public async Task CreateRoom(string roomCode, string playerName, string gameMode)
        {
            // Sukuriame naują įrašą ir priskiriame žaidėją
            _rooms[roomCode] = new List<string> { playerName };
            _connections[Context.ConnectionId] = playerName;
            _roomModes[roomCode] = gameMode;

            // Pridedame žaidėją į SignalR grupę
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            // Informuojame host'ą, kad kambarys sukurtas
            await Clients.Caller.SendAsync("RoomCreated", roomCode);

            // Pranešame visiems grupėje (t. y. tik hostui dabar)
            await Clients.Group(roomCode).SendAsync("PlayerJoined", playerName);

            Console.WriteLine($"Kambarys {roomCode} sukurtas ({gameMode}). Pirmas žaidėjas: {playerName}");
        }


        // PRISIJUNGIMAS PRIE KAMBARIO
        public async Task JoinRoom(string roomCode, string playerName)
        {
            if (!_rooms.TryGetValue(roomCode, out var players))
            {
                await Clients.Caller.SendAsync("Error", "Toks kambarys nerastas.");
                return;
            }

            // Neleidžiame naudoti to paties vardo
            if (players.Contains(playerName))
            {
                await Clients.Caller.SendAsync("Error", "Toks vardas jau naudojamas šiame kambaryje.");
                return;
            }

            // Pridedame žaidėją į sąrašą
            players.Add(playerName);
            _connections[Context.ConnectionId] = playerName;

            // Įtraukiame į grupę
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            // Pranešame visiems grupėje apie naują žaidėją
            await Clients.Group(roomCode).SendAsync("PlayerJoined", playerName);
            Console.WriteLine($"Žaidėjas '{playerName}' prisijungė prie kambario {roomCode} (viso: {players.Count})");

            // Patikriname režimą
            if (!_roomModes.TryGetValue(roomCode, out var mode))
            {
                mode = "1v1"; // jei kažkodėl nebuvo įrašyta
            }

            // 1v1 – automatinis startas po 2 žaidėjų
            if (mode == "1v1" && players.Count == 2)
            {
                await Clients.Group(roomCode).SendAsync("StartGame");
                Console.WriteLine($"Automatinis startas kambaryje {roomCode} (1v1).");
            }
            // 1vsmulti – laukia 3 ar daugiau žaidėjų, tada host'ui siunčiamas signalas, kad galima pradėti
            else if (mode == "1vsmulti" && players.Count >= 3)
            {
                await Clients.Group(roomCode).SendAsync("ReadyToStart", players);
                Console.WriteLine($"Kambaryje {roomCode} galima pradėti žaidimą ({players.Count} žaidėjai).");
            }
        }


        // ŽAIDIMO VEIKSMAI
        public async Task SendGameAction(string roomCode, string actionJson)
        {
            // Persiunčiame žaidimo veiksmą visiems žaidėjams tame kambaryje
            await Clients.Group(roomCode).SendAsync("ReceiveGameAction", actionJson);
        }

        // ŽAIDĖJO ATSIJUNGIMAS
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? playerName = null;
            _connections.TryRemove(Context.ConnectionId, out playerName);

            if (playerName == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            foreach (var room in _rooms)
            {
                if (room.Value.Contains(playerName))
                {
                    room.Value.Remove(playerName);
                    await Clients.Group(room.Key).SendAsync("PlayerLeft", playerName);
                    Console.WriteLine($"Žaidėjas {playerName} atsijungė nuo kambario {room.Key}");

                    // Jei kambarys ištuštėjo – pašaliname jį iš atminties
                    if (room.Value.Count == 0)
                    {
                        _rooms.TryRemove(room.Key, out _);
                        _roomModes.TryRemove(room.Key, out _);
                        Console.WriteLine($"Kambarys {room.Key} uždarytas (visi žaidėjai išėjo).");
                    }
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
        // === Kai žaidėjas paprašo paimti kortas (REQUEST_PICKUP_MULTI) ===
        public async Task RequestPickUp(string roomCode, string playerName)
        {
            if (_rooms.TryGetValue(roomCode, out var players))
            {
                Console.WriteLine($"{playerName} paėmė kortas kambaryje {roomCode}.");

                // Pranešam visiems kambaryje
                var action = new
                {
                    Action = "FORCE_PICKUP_MULTI",
                    Data = new { Player = playerName }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(action);
                await Clients.Group(roomCode).SendAsync("ReceiveGameAction", json);
            }
        }

    // Kai žaidėjas paprašo užbaigti ėjimą (REQUEST_ENDTURN_MULTI)
public async Task RequestEndTurn(string roomCode, string playerName)
{
    if (_rooms.TryGetValue(roomCode, out var players))
    {
        Console.WriteLine($"{playerName} paprašė užbaigti ėjimą kambaryje {roomCode}.");

        var action = new
        {
            Action = "FORCE_ENDTURN_MULTI",
            Data = new { Player = playerName }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(action);
        await Clients.Group(roomCode).SendAsync("ReceiveGameAction", json);
    }
}

    }
}
