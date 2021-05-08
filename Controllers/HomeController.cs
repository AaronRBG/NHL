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

        public IActionResult MagicNumbers()
        {
            float[][] h2h = calculateH2H();
            int[][] pb = calculatePB();
            info.magicNumbersDivision = calculateMagicNumbersDivision(h2h, pb);
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

        private float[][] calculateH2H()
        {
            const int N_TEAMS = 31;
            float[][] res = new float[N_TEAMS][];

            for (int i = 0; i < N_TEAMS; i++)
            {
                float[] aux = new float[N_TEAMS];
                for (int j = 0; j < N_TEAMS; j++)
                {
                    /*DataSet ds = Broker.Instance().Run("SELECT TOP 1 calculateH2H(" + (i+1).ToString() + "," + (j+1)(i+1).ToString() + ") FROM [dbo].[Standings]", "H2H");
                    DataTable dt = ds.Tables["H2H"];
                    int h2h = (int)dt.Rows[0][0];
                    switch(h2h)
                    {
                        case 1: aux[j] = 0.5; break;
                        case 2: aux[j] = -0.5; break;
                        default: aux[j] = 0; break;
                    }*/
                    aux[j] = 0;
                }
                res[i] = aux;
            }
            return res;
        }

        private int[][] calculatePB()
        {
            const int N_TEAMS = 31;
            int[][] res = new int[N_TEAMS][];

            for (int i = 0; i < N_TEAMS; i++)
            {
                int[] aux = new int[N_TEAMS];
                for (int j = 0; j < N_TEAMS; j++)
                {
                    Standing a = info.standings.standings.First(s => s.ID_Team == i + 1);
                    Standing b = info.standings.standings.First(s => s.ID_Team == j + 1);
                    aux[j] = a.Points - b.Points;
                }
                res[i] = aux;
            }
            return res;
        }

        private Dictionary<byte, float[]> calculateMagicNumbersDivision(float[][] h2h, int[][] pb)
        {
            Dictionary<byte, float[]> res = new Dictionary<byte, float[]>();

            for (byte division = 1; division < 5; division++)
            {
                byte[] teams = info.standings.getDivisionTeams(division);

                for (int i = 0; i < teams.Length; i++)
                {
                    float[] aux = new float[teams.Length];
                    for (int j = 0; j < teams.Length; j++)
                    {
                        aux[j] = info.standings.standings.Where(s => s.ID_Team == teams[i]).Select(s => s.Matches_Left * 2).First() - pb[teams[i]-1][teams[j]-1] + h2h[teams[i]-1][teams[j]-1];
                    }
                    Array.Sort(aux);
                    Array.Reverse(aux);
                    res.Add(teams[i], aux);
                }
            }
            return res;
        }
    }
}
