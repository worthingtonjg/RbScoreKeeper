using RbScoreKeeper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public interface IAppState
    {
        Task<List<Player>> GetPlayersAsync();

        Task AddPlayerAsync(string name);

        bool DeletePlayer(Guid playerId);

        Task<List<Flic>> GetFlicsAsync();

        Task AddFlicAsync(string Name);

        bool DeleteFlic(Guid flicId);

        List<FlicButtonBinding> GetButtonBindings();

        Match GetActiveMatch();

        List<Match> GetMatchHistory();

        List<Game> GetGameHistory();

        void CreateMatch(List<Guid> playerIds);

        void EndMatch();

        void NextGame(string buttonName);

        void NextGame();

        void RestartCurrentGame();

        void CancelMatch();

        void IncrementScore(string buttonName);

        void DecrementScore(string buttonName);
    }

    public class AppState : IAppState
    {
        private IStorageHelper _storageHelper;
        private List<Flic> _flics { get; set; }
        private List<Player> _players { get; set; }
        private List<FlicButtonBinding> _bindings { get; set; }
        private Match _activeMatch { get; set; }
        private Game _activeGame { get; set; }
        private List<Match> _matchesHistory { get; set; }
        private List<Game> _gamesHistory { get; set; }

        public AppState(IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
            _flics = new List<Flic>();
            _players = new List<Player>();
            _bindings = new List<FlicButtonBinding>();
            _matchesHistory = new List<Match>();
            _gamesHistory = new List<Game>();
            _activeMatch = null;
            _activeGame = null;

            AddDefaultData();
        }

        private async void AddDefaultData()
        {
            await GetFlicsAsync();
            await GetPlayersAsync();
        }

        #region Flics
        public async Task<List<Flic>> GetFlicsAsync()
        {
            var flics = await _storageHelper.GetFlicsAsync();

            _flics = flics.Select(f => new Flic(f)).ToList();

            return _flics;
        }

        public async Task AddFlicAsync(string name)
        {
            var entity = await _storageHelper.AddFlicAsync(name);

            await GetFlicsAsync();
        }

        public bool DeleteFlic(Guid flicId)
        {
            var remove = _flics.FirstOrDefault(f => f.FlicId == flicId);
            var binding = _bindings.FirstOrDefault(b => b.FlicId == flicId);

            if (remove != null)
            {
                if(binding != null)
                {
                    DeleteButtonBinding(binding.BindingId);
                }

                _flics.Remove(remove);
                return true;
            }

            return false;
        }

        #endregion

        #region Players
        public async Task<List<Player>> GetPlayersAsync()
        {
            var players = await _storageHelper.GetPlayersAsync();

            _players = players.Select(p => new Player(p)).ToList();

            return _players;
        }

        public async Task AddPlayerAsync(string name)
        {
            var entity = _storageHelper.AddPlayerAsync(name);
            await GetPlayersAsync();
        }

        public bool DeletePlayer(Guid playerId)
        {
            var remove = _players.FirstOrDefault(f => f.PlayerId == playerId);
            if (remove != null)
            {
                _players.Remove(remove);
                return true;
            }

            return false;
        }

        #endregion

        #region FlicButtonBindings
        public List<FlicButtonBinding> GetButtonBindings()
        {
            return _bindings;
        }

        public void AddUpdateButtonBinding(Guid flicId, Guid playerId)
        {
            var flic = _flics.FirstOrDefault(f => f.FlicId == flicId);
            var player = _players.FirstOrDefault(p => p.PlayerId == playerId);

            if(flic == null || player == null)
            {
                throw new Exception("could not find flic or player");
            }

            var existing = _bindings.FirstOrDefault(b => b.FlicId == flicId);

            if (existing != null)
            {
                // reassign button to new player
                existing.PlayerId = playerId;
            }
            else
            {
                // add new button
                var binding = new FlicButtonBinding(flicId, playerId);
                _bindings.Add(binding);
            }
        }

        public bool DeleteButtonBinding(Guid bindingId)
        {
            var remove = _bindings.FirstOrDefault(f => f.BindingId == bindingId);
            if (remove != null)
            {
                _bindings.Remove(remove);
                return true;
            }

            return false;
        }

        public void IncrementScore(string buttonName)
        {
            var score = GetPlayerScoreFromBinding(buttonName);
            if (score != null)
            {
                ++score.Score;
            }
        }

        public void DecrementScore(string buttonName)
        {
            var score = GetPlayerScoreFromBinding(buttonName);
            if (score != null)
            {
                --score.Score;
            }
        }

        private PlayerScore GetPlayerScoreFromBinding(string buttonName)
        {
            PlayerScore result = null;

            var flic = _flics.FirstOrDefault(f => f.Name == buttonName);
            if (flic != null)
            {
                var binding = _bindings.FirstOrDefault(b => b.FlicId == flic.FlicId);
                if (binding != null)
                {
                    var players = _activeMatch.Players.Select(p => p.PlayerId).ToList();
                    if (players.Contains(binding.PlayerId))
                    {
                        result = _activeMatch.CurrentGame.Scores.FirstOrDefault(s => s.PlayerId == binding.PlayerId);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Match
        public Match GetActiveMatch()
        {
            return _activeMatch;
        }

        public List<Match> GetMatchHistory()
        {
            return _matchesHistory;
        }

        public List<Game> GetGameHistory()
        {
            return _gamesHistory;
        }

        public void CreateMatch(List<Guid> playerIds)
        {
            var players = _players.Where(p => playerIds.Contains(p.PlayerId)).ToList();

            // Loop through the players and bind them to a flic
            int index = 0;
            foreach(var player in players)
            {
                if(index < _flics.Count)
                {
                    AddUpdateButtonBinding(_flics[index++].FlicId, player.PlayerId);
                }
            }

            _activeMatch = new Match
            {
                MatchId = Guid.NewGuid(),
                StartTime = DateTime.Now,
                Players = players,
                Games = new List<Game>() { new Game(players) },
            };

            _activeGame = _activeMatch.CurrentGame;
        }

        public void CancelMatch()
        {
            _activeMatch = null;
            _activeGame = null;
        }

        public void EndMatch()
        {
            if (_activeMatch != null)
            {
                _activeMatch.EndTime = DateTime.Now;

                _gamesHistory.Add(_activeMatch.CurrentGame);
                _matchesHistory.Add(_activeMatch);
                _activeMatch = null;
            }
        }

        public void RestartCurrentGame()
        {
            if (_activeMatch != null && _activeMatch.CurrentGame != null)
            {
                foreach(var score in _activeMatch.CurrentGame.Scores)
                {
                    score.Score = 0;
                }
            }
        }

        public void NextGame()
        {
            if (_activeMatch != null)
            {
                _activeMatch.CurrentGame.EndTime = DateTime.Now;
                _gamesHistory.Add(_activeMatch.CurrentGame);
                _activeMatch.Games.Add(new Game(_activeMatch.Players));
            }
        }

        public void NextGame(string buttonName)
        {
            var flic = _flics.FirstOrDefault(f => f.Name == buttonName);
            if (flic == null) return;
            
            var binding = _bindings.FirstOrDefault(b => b.FlicId == flic.FlicId);
            if (binding == null) return;

            var players = _activeMatch.Players.Select(p => p.PlayerId).ToList();
            if (players.Contains(binding.PlayerId))
            {
                NextGame();
            }
        }

        #endregion

    }
}
