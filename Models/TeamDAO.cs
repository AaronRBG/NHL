using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace NHL.Models
{
    public class TeamDAO
    {
        public List<Team> teams;

        public TeamDAO() 
        {
            teams = Broker.Instance().GetConnection().Query<Team>("SELECT ID_Team, Team_Name FROM [dbo].[Teams]").ToList();
        }

        public byte getTeamID(string teamName)
        {
            return teams.First(t => t.Team_Name == teamName).ID_Team;
        }

        public string getTeamName(byte teamID)
        {
            return teams.First(t => t.ID_Team == teamID).Team_Name;
        }

    }
}
