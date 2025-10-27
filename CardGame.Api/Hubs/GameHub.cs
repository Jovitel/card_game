using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CardGame.Api.Hubs
{
    public class GameHub : Hub
    {
        // Atmintyje laikome aktyvius kambarius ir juose esančius žaidėjus
        private static readonly ConcurrentDictionary<string, List<string>> _rooms = new();

        // Sukuria kambarį su nurodytu kodu
        public async Task CreateRoom(string roomCode)
        {
            _rooms.TryAdd(roomCode, new List<string> { Context.ConnectionId });
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Caller.SendAsync("RoomCreated", roomCode);
            Console.WriteLine($"Kambarys {roomCode} sukurtas. Pirmas žaidėjas prisijungė: {Context.ConnectionId}");
        }

        // Prisijungia prie egzistuojančio kambario
        public async Task JoinRoom(string roomCode)
        {
            if (_rooms.TryGetValue(roomCode, out var connections))
            {
                connections.Add(Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                await Clients.Group(roomCode).SendAsync("PlayerJoined", Context.ConnectionId);

                Console.WriteLine($"Žaidėjas prisijungė prie kambario {roomCode}: {Context.ConnectionId}");

                // Pranešam abiem, kad žaidimas gali prasidėti
                if (connections.Count == 2)
                {
                    await Clients.Group(roomCode).SendAsync("StartGame");
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Toks kambarys nerastas.");
            }
        }

        // Siunčia veiksmą visiems kambaryje
        public async Task SendGameAction(string roomCode, string actionJson)
        {
            await Clients.Group(roomCode).SendAsync("ReceiveGameAction", actionJson);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var room in _rooms)
            {
                if (room.Value.Contains(Context.ConnectionId))
                {
                    room.Value.Remove(Context.ConnectionId);
                    await Clients.Group(room.Key).SendAsync("PlayerLeft", Context.ConnectionId);
                    Console.WriteLine($"Žaidėjas atsijungė nuo kambario {room.Key}: {Context.ConnectionId}");
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
