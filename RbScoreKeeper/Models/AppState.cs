using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RbScoreKeeper.Models
{
    public interface IAppState
    {
        List<Player> GetPlayers();

        void AddPlayer(string name);

        bool DeletePlayer(Guid playerId);

        List<Flic> GetFlics();

        void AddFlic(string Name);

        bool DeleteFlic(Guid flicId);

        List<FlicButtonBinding> GetButtonBindings();

        void AddUpdateButtonBinding(Guid flicId, Guid playerId);

        bool DeleteButtonBinding(Guid bindingId);

        List<Group> GetGroups();

        void AddGroup(List<Player> players);

        bool DeleteGroup(Guid groupId);

        List<Match> GetActiveMatches();

        void CreateMatch(Guid groupId);

        void EndMatch(Guid matchId);

        void NextGame(string buttonName);

        void NextGame(Guid matchId);

        void RestartCurrentGame(Guid matchId);

        void CancelMatch(Guid matchId);

        void IncrementScore(string buttonName);

        void DecrementScore(string buttonName);
    }

    public class AppState : IAppState
    {
        private List<Flic> _flics { get; set; }
        private List<Player> _players { get; set; }
        private List<FlicButtonBinding> _bindings { get; set; }
        private List<Group> _groups { get; set; }
        private List<Match> _activeMatches { get; set; }
        private List<Match> _matchesHistory { get; set; }
        private List<Game> _gamesHistory { get; set; }

        public AppState()
        {
            _flics = new List<Flic>();
            _players = new List<Player>();
            _bindings = new List<FlicButtonBinding>();
            _groups = new List<Group>();
            _activeMatches = new List<Match>();
            _matchesHistory = new List<Match>();
            _gamesHistory = new List<Game>();

            AddDefaultData();
        }

        private void AddDefaultData()
        {
            var flic1 = new Flic("001");
            var flic2 = new Flic("002");
            var flic3 = new Flic("003");

            _flics.Add(flic1);
            _flics.Add(flic2);
            _flics.Add(flic3);

            var jon = new Player("Jon");
            var alan = new Player("Alan");
            var tony = new Player("Tony");

            _players.Add(jon);
            _players.Add(tony);
            _players.Add(alan);
            _players.Add(new Player("Jared"));
            _players.Add(new Player("Scott"));
            _players.Add(new Player("Randy"));
            _players.Add(new Player("Nahinu"));

            AddUpdateButtonBinding(flic1.FlicId, jon.PlayerId);
            AddUpdateButtonBinding(flic2.FlicId, alan.PlayerId);
            AddUpdateButtonBinding(flic3.FlicId, tony.PlayerId);

            var defaultGroup = new Group(new List<Player>() { jon, alan, tony });
            _groups.Add(defaultGroup);

            CreateMatch(defaultGroup.GroupId);
        }

        #region Flics
        public List<Flic> GetFlics()
        {
            return _flics;
        }

        public void AddFlic(string name)
        {
            var flic = new Flic(name);
            _flics.Add(flic);
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
        public List<Player> GetPlayers()
        {
            return _players;
        }

        public void AddPlayer(string name)
        {
            var player = new Player(name);
            _players.Add(player);
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
            ++score.Score;
        }

        public void DecrementScore(string buttonName)
        {
            var score = GetPlayerScoreFromBinding(buttonName);
            --score.Score;
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
                    foreach (var match in _activeMatches)
                    {
                        var players = match.Players.Select(p => p.PlayerId).ToList();
                        if (players.Contains(binding.PlayerId))
                        {
                            result = match.CurrentGame.Scores.FirstOrDefault(s => s.PlayerId == binding.PlayerId);
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Groups
        public List<Group> GetGroups()
        {
            return _groups;
        }

        public void AddGroup(List<Player> players)
        {
            var group = new Group(players);

            _groups.Add(group);
        }

        public bool DeleteGroup(Guid groupId)
        {
            var remove = _groups.FirstOrDefault(f => f.GroupId == groupId);
            if (remove != null)
            {
                _groups.Remove(remove);
                return true;
            }

            return false;
        }

        #endregion

        #region Matches
        public List<Match> GetActiveMatches()
        {
            return _activeMatches;
        }

        public List<Match> GetMatchHistory()
        {
            return _matchesHistory;
        }

        public List<Game> GetGameHistory()
        {
            return _gamesHistory;
        }

        public void CreateMatch(Guid groupId)
        {
            var group = _groups.FirstOrDefault(g => g.GroupId == groupId);

            if (group == null) return;

            var match = new Match
            {
                MatchId = Guid.NewGuid(),
                StartTime = DateTime.Now,
                Players = group.Players,
                Games = new List<Game>() { new Game(group.Players) },

            };

            _activeMatches.Add(match);
        }

        public void CancelMatch(Guid matchId)
        {
            var activeMatch = _activeMatches.FirstOrDefault(m => m.MatchId == matchId);
            if (activeMatch != null)
            {
                _activeMatches.Remove(activeMatch);
            }
        }

        public void EndMatch(Guid matchId)
        {
            var activeMatch = _activeMatches.FirstOrDefault(m => m.MatchId == matchId);
            if (activeMatch != null)
            {
                activeMatch.EndTime = DateTime.Now;

                _gamesHistory.Add(activeMatch.CurrentGame);
                _matchesHistory.Add(activeMatch);
                _activeMatches.Remove(activeMatch);
            }
        }

        public void RestartCurrentGame(Guid matchId)
        {
            var activeMatch = _activeMatches.FirstOrDefault(m => m.MatchId == matchId);
            if (activeMatch != null)
            {
                activeMatch.CurrentGame = new Game(activeMatch.Players);
            }
        }

        public void NextGame(Guid matchId)
        {
            var activeMatch = _activeMatches.FirstOrDefault(m => m.MatchId == matchId);
            if (activeMatch != null)
            {
                activeMatch.CurrentGame.EndTime = DateTime.Now;
                _gamesHistory.Add(activeMatch.CurrentGame);
                activeMatch.Games.Add(new Game(activeMatch.Players));
            }
        }

        public void NextGame(string buttonName)
        {
            var flic = _flics.FirstOrDefault(f => f.Name == buttonName);
            if (flic == null) return;
            
            var binding = _bindings.FirstOrDefault(b => b.FlicId == flic.FlicId);
            if (binding == null) return;

            foreach (var match in _activeMatches)
            {
                var players = match.Players.Select(p => p.PlayerId).ToList();
                if (players.Contains(binding.PlayerId))
                {
                    NextGame(match.MatchId);
                    break;
                }
            }
        }

        #endregion

    }
}
