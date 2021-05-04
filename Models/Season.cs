using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Season
    {
        public Season(byte iD_Season, string season_Name, DateTime start_Date, DateTime finish_Date, DateTime playoff_Start_Date, DateTime playoff_Finish_Date, byte n_Matches_Team)
        {
            ID_Season = iD_Season;
            Season_Name = season_Name;
            Start_Date = start_Date;
            Finish_Date = finish_Date;
            Playoff_Start_Date = playoff_Start_Date;
            Playoff_Finish_Date = playoff_Finish_Date;
            N_Matches_Team = n_Matches_Team;
        }

        public byte ID_Season { get; set; }
        public string Season_Name { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Finish_Date { get; set; }
        public DateTime Playoff_Start_Date { get; set; }
        public DateTime Playoff_Finish_Date { get; set; }
        public byte N_Matches_Team { get; set; }
    }
}
