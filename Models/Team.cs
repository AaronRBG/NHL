using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Team
    {
        public Team(byte iD_Team, string team_City, string team_Name)
        {
            ID_Team = iD_Team;
            Team_City = team_City;
            Team_Name = team_Name;
        }

        public byte ID_Team { get; set; }
        public string Team_City { get; set; }
        public string Team_Name { get; set; }
    }
}
