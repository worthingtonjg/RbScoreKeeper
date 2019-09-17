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
    public class BindingsController : Controller
    {
        private IAppState _appState;

        public BindingsController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public List<FlicButtonBinding> GetBindings()
        {
            return _appState.GetButtonBindings();
        }

        [HttpPost()]
        public void AddUpdateButtonBinding([FromQuery]Guid flicId, [FromQuery]Guid playerId)
        {
            _appState.AddUpdateButtonBinding(flicId, playerId);
        }

        [HttpDelete("{bindingId}")]
        public void DeleteBinding(Guid bindingId)
        {
            _appState.DeleteButtonBinding(bindingId);
        }
    }
}