using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NHL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Data;

namespace NHL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Info info;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            info = initialize().Result;
        }

        private async Task<Info> initialize()
        {
            Task<Win_TypeDAO> win_typeDAO = Task.Run(() => new Win_TypeDAO());
            Task<SeasonDAO> seasonDAO = Task.Run(() => new SeasonDAO());
            Task<TeamDAO> teamDAO = Task.Run(() => new TeamDAO());
            Task<MatchDAO> matchDAO = Task.Run(() => new MatchDAO());

            seasonDAO.Wait();
            matchDAO.Wait();
            Task<StandingDAO> standingDAO = Task.Run(() => new StandingDAO());

            Task<Info> infoTask = Task.Run(() => new Info(standingDAO.Result, matchDAO.Result, teamDAO.Result, win_typeDAO.Result, seasonDAO.Result));

            Task.WaitAll(win_typeDAO, teamDAO, standingDAO, infoTask);

            return infoTask.Result;
        }

        public IActionResult Index()
        {
            return View("Standings", info);
        }

        public IActionResult Standings()
        {
            return View(info);
        }

        public IActionResult MagicNumbers()
        {
            return View(info);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
