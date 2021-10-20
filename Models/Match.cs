using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Match
    {
        public Match(byte local_Team, byte visitor_Team, byte local_Goals, byte visitor_Goals, DateTime match_Date, int season, byte win_Type, bool regular_Season)
        {
            Local_Team = local_Team;
            Visitor_Team = visitor_Team;
            Local_Goals = local_Goals;
            Visitor_Goals = visitor_Goals;
            Match_Date = match_Date;
            Season = season;
            Win_Type = win_Type;
            Regular_Season = regular_Season;
        }
        public Match(long iD_Match, byte local_Team, byte visitor_Team, byte local_Goals, byte visitor_Goals, DateTime match_Date, int season, byte winner, byte win_Type, bool regular_Season)
        {
            ID_Match = iD_Match;
            Local_Team = local_Team;
            Visitor_Team = visitor_Team;
            Local_Goals = local_Goals;
            Visitor_Goals = visitor_Goals;
            Match_Date = match_Date;
            Season = season;
            Winner = winner;
            Win_Type = win_Type;
            Regular_Season = regular_Season;
        }

        public long ID_Match { get; set; }
        public byte Local_Team { get; set; }
        public byte Visitor_Team { get; set; }
        public byte Local_Goals { get; set; }
        public byte Visitor_Goals { get; set; }
        public DateTime Match_Date { get; set; }
        public int Season { get; set; }
        public byte Winner { get; set; }
        public byte Win_Type { get; set; }
        public bool Regular_Season { get; set; }
    }
}
