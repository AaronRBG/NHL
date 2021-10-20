using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Season
    {

        public Season(DateTime start_Date, DateTime finish_Date, byte n_Matches_Team)
        {
            Start_Date = start_Date;
            Finish_Date = finish_Date;
            N_Matches_Team = n_Matches_Team;
        }
        public Season(int iD_Season, DateTime start_Date, DateTime finish_Date, byte n_Matches_Team) :this(start_Date, finish_Date, n_Matches_Team)
        {
            ID_Season = iD_Season;
        }

        public int ID_Season { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Finish_Date { get; set; }
        public byte N_Matches_Team { get; set; }
    }
}
