using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private IAppState _appState;

        public GroupsController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public List<Group> GetGroups()
        {
            return _appState.GetGroups();
        }

        [HttpPost()]
        public void AddGroup(List<Player> players)
        {
            _appState.AddGroup(players);
        }

        [HttpDelete("{groupId}")]
        public void DeleteGroup(Guid groupId)
        {
            _appState.DeleteGroup(groupId);
        }
    }
}
