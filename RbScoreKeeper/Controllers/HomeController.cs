﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;

namespace RbScoreKeeper.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("mobile")]
        public IActionResult IndexMobile()
        {
            return View("Mobile");
        }

        [HttpGet("stats")]
        public IActionResult Stats()
        {
            return View("Stats");
        }

        [HttpGet("mobile/stats")]
        public IActionResult StatsMobile()
        {
            return View("MobileStats");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
