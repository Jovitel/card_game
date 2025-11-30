using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace card_game.Services
{
    // Bendras veiksmo „vokas“ (JSON) – naudojamas siųsti tarp kliento ir serverio
    public class GameAction
    {
        public string Action { get; set; } = "";                  // Pvz.: "INIT", "ATTACK", "DEFEND", "ENDTURN"
        public Dictionary<string, string> Data { get; set; } = new(); // Dinamiškas duomenų rinkinys
    }

    public class GameClientService
    {
        private HubConnection? _hubConnection; // SignalR jungtis su serveriu
        private string? _roomCode;             // Dabartinio kambario kodas

        // Laikome prisijungusių žaidėjų sąrašą ir savo vardą
        public List<string> ConnectedPlayers { get; private set; } = new();  // Prisijungusių žaidėjų vardai
        public string MyName { get; private set; } = $"Player_{Guid.NewGuid().ToString("N").Substring(0, 4)}"; // Automatinis vardas

        // Įvykiai, kuriuos klausys Blazor puslapiai (UI pusė)
        public event Action? OnStartGame;                          // Kai žaidimas prasideda
        public event Action? OnRoomCreated;                        // Kai kambarys sukurtas
        public event Action? OnPlayerJoined;                       // Kai prisijungia naujas žaidėjas
        public event Action<string>? OnPlayerLeft;                 // Kai žaidėjas išeina
        public event Action<string>? OnErrorReceived;              // Kai gaunama klaida
        public event Action<GameAction>? OnGameActionReceived;     // Kai gaunamas žaidimo veiksmas (pvz. ataka)
        public event Action<List<string>>? OnReadyToStart;         // Nauja – kai hub praneša, kad galima pradėti multi žaidimą

        // Patogus property patikrinti ar vis dar prijungta prie SignalR
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        // Prisijungimo dalis 
        public async Task ConnectAsync(string? username = null)
        {
            if (_hubConnection != null && IsConnected)
                return;

            // Jei duotas username – naudok jį, kitu atveju sugeneruok random
            MyName = string.IsNullOrEmpty(username)
                ? $"Guest_{Guid.NewGuid().ToString("N").Substring(0, 4)}"
                : username;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7200/gamehub")
                .WithAutomaticReconnect()
                .Build();

            // HUB'O SIUNČIAMI ĮVYKIAI

            // Kai serveris pasako, kad žaidimas prasideda
            _hubConnection.On("StartGame", () =>
            {
                OnStartGame?.Invoke();
            });

            // Kai sėkmingai sukuriamas kambarys
            _hubConnection.On("RoomCreated", (string roomCode) =>
            {
                _roomCode = roomCode;
                OnRoomCreated?.Invoke();
            });

            // Kai naujas žaidėjas prisijungia
            _hubConnection.On<string>("PlayerJoined", (string playerName) =>
            {
                if (!ConnectedPlayers.Contains(playerName))
                    ConnectedPlayers.Add(playerName);

                Console.WriteLine($"Žaidėjas prisijungė: {playerName}");
                OnPlayerJoined?.Invoke();
            });

            // Kai serveris praneša, kad galima pradėti multi žaidimą
            _hubConnection.On<List<string>>("ReadyToStart", (List<string> players) =>
            {
                ConnectedPlayers = players;
                OnReadyToStart?.Invoke(players);
                Console.WriteLine("Galima pradėti žaidimą – prisijungę: " + string.Join(", ", players));
            });

            // Kai žaidėjas išeina iš kambario
            _hubConnection.On<string>("PlayerLeft", (string playerName) =>
            {
                if (ConnectedPlayers.Contains(playerName))
                    ConnectedPlayers.Remove(playerName);

                Console.WriteLine($"Žaidėjas paliko: {playerName}");
                OnPlayerLeft?.Invoke(playerName);
            });

            // Kai serveris grąžina klaidą
            _hubConnection.On<string>("Error", (errorMsg) =>
            {
                Console.WriteLine($"Serverio klaida: {errorMsg}");
                OnErrorReceived?.Invoke(errorMsg);
            });

            // Kai gaunamas žaidimo veiksmas (bet koks JSON)
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

            // Prisijungiame prie serverio
            await _hubConnection.StartAsync();
            Console.WriteLine($"Prisijungta prie SignalR kaip {MyName}");
        }

        // Kambario valdymas 

        // Sukuria naują kambarį (host pusėje)
        // gameMode gali būti "1v1" arba "1vsmulti"
        public async Task CreateRoomAsync(string roomCode, string gameMode)
        {
            _roomCode = roomCode;
            ConnectedPlayers.Clear();
            ConnectedPlayers.Add(MyName);

            await _hubConnection!.SendAsync("CreateRoom", roomCode, MyName, gameMode);
            Console.WriteLine($"Sukurtas kambarys {roomCode} (host: {MyName}, režimas: {gameMode})");
        }


        // Prisijungia prie egzistuojančio kambario (guest pusėje)
        public async Task JoinRoomAsync(string roomCode)
        {
            _roomCode = roomCode;
            await _hubConnection!.SendAsync("JoinRoom", roomCode, MyName);
            Console.WriteLine($"Prisijungta prie kambario {roomCode} kaip {MyName}");
        }

        // Žaidimo veiksmai  

        // Siunčia bet kokį žaidimo veiksmą (pvz.: ataka, gynyba, pabaiga)
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

        // Atsijungia nuo serverio (naudinga kai išeinama iš žaidimo)
        public async Task DisconnectAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                Console.WriteLine("Atsijungta nuo SignalR serverio.");
            }
        }
        // Siunčia prašymą serveriui, kad gynėjas paėmė kortas (multiplayer režimas)
        public async Task RequestPickUpAsync()
        {
            if (_hubConnection == null || _roomCode == null)
            {
                Console.WriteLine("Neprisijungta prie hub'o arba nėra kambario kodo.");
                return;
            }

            await _hubConnection.SendAsync("RequestPickUp", _roomCode, MyName);
            Console.WriteLine($"Išsiųstas 'RequestPickUp' signalas iš {MyName}");
        }

        public async Task RequestEndTurnAsync()
        {
            if (_hubConnection == null || _roomCode == null)
                return;

            await _hubConnection.SendAsync("RequestEndTurn", _roomCode, MyName);
            Console.WriteLine($"Išsiųstas 'RequestEndTurn' signalas ({MyName})");
        }


    }
}
