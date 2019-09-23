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
    public class FlicsController : Controller
    {
        private IAppState _appState;

        public FlicsController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public async Task<List<Flic>> GetFlics()
        {
            return await _appState.GetFlicsAsync();
        }

        [HttpPost()]
        public async Task AddFlic([FromQuery]string name)
        {
            await _appState.AddFlicAsync(name);
        }

        [HttpDelete("{flicId}")]
        public void DeleteFlic(Guid flicId)
        {
            _appState.DeleteFlic(flicId);
        }
    }
}