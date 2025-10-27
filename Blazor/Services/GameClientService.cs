using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace card_game.Services
{
    // Bendras veiksmo „vokas“ (JSON), kad būtų lengva plėsti
    public class GameAction
    {
        public string Action { get; set; } = "";                 // Pvz.: "INIT", "ATTACK", "DEFEND", "ENDTURN"
        public Dictionary<string, string> Data { get; set; } = new(); // Lankstūs raktai: "p", "suit", "value", ...
    }

    public class GameClientService
    {
        private HubConnection? _hubConnection;
        private string? _roomCode;

        // Įvykiai, kuriuos klausys puslapis (Game1vs1)
        public event Action? OnStartGame;                         // Kai kambaryje jau 2 žaidėjai
        public event Action? OnRoomCreated;                      // Kai sėkmingai sukuriamas kambarys (host'ui)
        public event Action? OnPlayerJoined;                     // Kai kas nors prisijungia (host'ui)
        public event Action<string>? OnPlayerLeft;               // Kai kas nors išeina
        public event Action<string>? OnErrorReceived;            // Klaidos iš hub'o
        public event Action<GameAction>? OnGameActionReceived;   // Visi žaidimo veiksmai (INIT/ATTACK/DEFEND/ENDTURN...)

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task ConnectAsync()
        {
            if (_hubConnection != null && IsConnected)
                return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7200/gamehub")
                .WithAutomaticReconnect()
                .Build();

            // === Hub'o siunčiami įvykiai ===
            _hubConnection.On("StartGame", () =>
            {
                OnStartGame?.Invoke();
            });

            _hubConnection.On("RoomCreated", (string roomCode) =>
            {
                _roomCode = roomCode;
                OnRoomCreated?.Invoke();
            });

            _hubConnection.On("PlayerJoined", () =>
            {
                OnPlayerJoined?.Invoke();
            });

            _hubConnection.On<string>("PlayerLeft", (connId) =>
            {
                OnPlayerLeft?.Invoke(connId);
            });

            _hubConnection.On<string>("Error", (errorMsg) =>
            {
                OnErrorReceived?.Invoke(errorMsg);
            });

            // Visi žaidimo veiksmai (JSON)
            _hubConnection.On<string>("ReceiveGameAction", (json) =>
            {
                try
                {
                    var action = JsonSerializer.Deserialize<GameAction>(json);
                    if (action != null)
                        OnGameActionReceived?.Invoke(action);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Klaida apdorojant GameAction JSON: {ex.Message}");
                }
            });

            await _hubConnection.StartAsync();
            Console.WriteLine("Connected to SignalR hub");
        }

        public async Task CreateRoomAsync(string roomCode)
        {
            _roomCode = roomCode;
            await _hubConnection!.SendAsync("CreateRoom", roomCode);
        }

        public async Task JoinRoomAsync(string roomCode)
        {
            _roomCode = roomCode;
            await _hubConnection!.SendAsync("JoinRoom", roomCode);
        }

        // Bendra vieta siųsti veiksmus į dabartinį kambarį
        public async Task SendGameActionAsync(GameAction action)
        {
            if (_roomCode == null)
            {
                Console.WriteLine("Bandymas siųsti veiksmą be kambario!");
                return;
            }

            string json = JsonSerializer.Serialize(action);
            await _hubConnection!.SendAsync("SendGameAction", _roomCode, json);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection is not null)
                await _hubConnection.StopAsync();
        }

        
    }
}
