using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Team
    {
        public Team(byte iD_Team, string team_Name)
        {
            ID_Team = iD_Team;
            Team_Name = team_Name;
            Playoff_Position = false;
        }

        public byte ID_Team { get; set; }
        public string Team_Name { get; set; }
        public bool Playoff_Position { get; set; }
    }
}
