using NHL_Updater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net;
using System.Diagnostics;

namespace NHL_Updater
{
    class Program
    {
        private static readonly MatchDAO matchDAO;
        private static readonly TeamDAO teamDAO;
        private static readonly SeasonDAO seasonDAO;

        static Program()
        {
            seasonDAO = new SeasonDAO();
            teamDAO = new TeamDAO();
            matchDAO = new MatchDAO();
        }


        public static void Main(string[] args)
        {

            //Console.WriteLine(args[0]);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            int season = SeasonDAO.Current_Season;
            string url = "https://www.hockey-reference.com/leagues/NHL_" + season + "_games.html";
            var response = CallUrl(url).Result;
            //ParseSeason(response, season);    // Only needed when inputting old seasons
            ParseHtml(response, true, season);  // Regular season games
            ParseHtml(response, false, season); // Keep commented each year until playoffs time comes

            sw.Stop();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        private static void ParseHtml(string html, bool RegularSeason, int season)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            string id = "all_games";
            if (!RegularSeason) { id += "_playoffs"; }
            var table = htmlDoc.GetElementbyId(id);
            if (table != null)
            {
                var matchesInfo = table.Descendants("tr").ToList();

                List<Match> matches = new List<Match>();

                foreach (var match in matchesInfo)
                {
                    if (match != matchesInfo[0])
                    {
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
                        if (!(visitor_goals == 0 && local_goals == 0))
                        {
                            Match m = new Match(local_team, visitor_team, local_goals, visitor_goals, date, season, Win_Type, RegularSeason);
                            matches.Add(m);
                        }
                    }
                }
                matchDAO.insertNewMatches(matches.ToArray());
            }
        }

        private static void ParseSeason(string html, int season)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var matchesInfo = htmlDoc.DocumentNode.Descendants("tr").ToArray();
            HtmlNode node = matchesInfo[1].FirstChild;
            DateTime start_date = DateTime.Parse(node.InnerText);
            node = matchesInfo[matchesInfo.Length - 1].FirstChild;
            DateTime finish_date = DateTime.Parse(node.InnerText);
            var array = htmlDoc.GetElementbyId("all_games").Descendants("tr").ToArray();
            string team = array[1].FirstChild.NextSibling.InnerText;
            byte count = 0;
            for (int i = 1; i < array.Length; i++)
            {
                string aux = array[i].FirstChild.NextSibling.InnerText;
                if (aux == team)
                {
                    count++;
                }
                else
                {
                    aux = array[i].FirstChild.NextSibling.NextSibling.NextSibling.InnerText;
                    if (aux == team)
                    {
                        count++;
                    }
                }
            }
            Season s = new Season(start_date, finish_date, count);
            seasonDAO.insert(s);
        }


    }
}
