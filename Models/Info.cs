using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Info
    {
        public Info(StandingDAO standings, MatchDAO matches, TeamDAO teams, Win_TypeDAO win_types, SeasonDAO seasons)
        {
            this.win_types = win_types;
            this.teams = teams;
            this.seasons = seasons;
            this.matches = matches;
            this.standings = standings;
        }

        public StandingDAO standings { get; set; }
        public MatchDAO matches { get; set; }
        public TeamDAO teams { get; set; }
        public Win_TypeDAO win_types { get; set; }
        public SeasonDAO seasons { get; set; }
    }
}
