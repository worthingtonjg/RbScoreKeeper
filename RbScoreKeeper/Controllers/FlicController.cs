using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RbScoreKeeper.Controllers
{
    public class FlicController : Controller
    {
        // GET: Flic
        public ActionResult Index()
        {
            return View();
        }
    }
}