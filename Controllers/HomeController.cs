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

namespace NHL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MatchDAO matchDAO;
        private readonly TeamDAO teamDAO;
        private readonly StandingDAO standingDAO;
        private readonly SeasonDAO seasonDAO;
        private readonly Win_TypeDAO win_typeDAO;
        private readonly Info info;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            win_typeDAO = new Win_TypeDAO();
            seasonDAO = new SeasonDAO();
            teamDAO = new TeamDAO();
            matchDAO = new MatchDAO();
            standingDAO = new StandingDAO();
            info = new Info(standingDAO, matchDAO, teamDAO, win_typeDAO, seasonDAO);
        }

        public IActionResult Index()
        {
            string url = "https://www.hockey-reference.com/leagues/NHL_2021_games.html";
            var response = CallUrl(url).Result;
            ParseHtml(response);
            info.matches = matchDAO;
            return View(info);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        private void ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var matchesInfo = htmlDoc.DocumentNode.Descendants("tr").ToList();

            List<Match> matches = new List<Match>();

            foreach (var match in matchesInfo)
            {
                if (match != matchesInfo[0])
                {
                    bool played = true;
                    HtmlNode node = match.FirstChild;
                    DateTime date = DateTime.Parse(node.InnerText);
                    node = node.NextSibling;
                    byte visitor_team = teamDAO.getTeamID(node.InnerText);
                    node = node.NextSibling;
                    byte visitor_goals = 0;
                    if (node.InnerText != "")
                    {
                        visitor_goals = byte.Parse(node.InnerText);
                    }
                    node = node.NextSibling;
                    byte local_team = teamDAO.getTeamID(node.InnerText);
                    node = node.NextSibling;
                    byte local_goals = 0;
                    if (node.InnerText != "")
                    {
                        local_goals = byte.Parse(node.InnerText);
                    }
                    node = node.NextSibling;
                    byte Win_Type = 0;
                    switch (node.InnerText)
                    {
                        case "OT":
                            Win_Type = 2; break;
                        case "SO":
                            Win_Type = 3; break;
                        default:
                            Win_Type = 1; break;
                    }
                    byte season = seasonDAO.getSeason(date);
                    bool Regular_Season = seasonDAO.isRegularSeason(date, season);
                    if (!(visitor_goals == 0 && local_goals == 0))
                    {
                        Match m = new Match(local_team, visitor_team, local_goals, visitor_goals, date, season, Win_Type, Regular_Season);
                        matches.Add(m);
                    }
                }
            }
            matchDAO.insertNewMatches(matches.ToArray());
        }

        public IActionResult MagicNumbers()
        {
            return View();
        }

    }
}
