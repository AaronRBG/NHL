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

namespace NHL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public MatchDAO matchDAO = new MatchDAO();
        public TeamDAO teamDAO = new TeamDAO();
        public StandingDAO standingDAO = new StandingDAO();
        public SeasonDAO seasonDAO = new SeasonDAO();
        public Win_TypeDAO win_typeDAO = new Win_TypeDAO();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            SqlConnection con = new SqlConnection("server = localhost; database = NHL; Trusted_Connection = True;");
            con.Open();

            // Save a id variable in the session to stop it for resetting, then save the connection and create the dao
            HttpContext.Session.SetString("id", HttpContext.Session.Id);
            return View();
        }

        public IActionResult MagicNumbers()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
