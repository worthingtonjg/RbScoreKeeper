using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : Controller
    {
        private IAppState _appState;

        public StatsController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public async Task<List<Stats>> GetStats()
        {
            return await _appState.GetStatsAsync();
        }

        [HttpGet("partial")]
        public async Task<IActionResult> GetMatchPartial()
        {
            var stats = await _appState.GetStatsAsync();

            return PartialView("StatsPartial", stats);
        }
    }
}