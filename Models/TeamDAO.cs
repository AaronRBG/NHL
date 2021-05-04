using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class TeamDAO
    {
        public Team[] teams { get; set; }

        public TeamDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Teams]", "Teams");
            DataTable dt = ds.Tables["Teams"];
            List<Team> aux = new List<Team>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Team t = new Team((byte)dt.Rows[i][0], (string)dt.Rows[i][1], (string)dt.Rows[i][2]);
                aux.Add(t);
            }
            teams = aux.ToArray();
        }

        public byte getTeamID(string team_name)
        {
            DataSet ds = Broker.Instance().Run("SELECT [ID_Team] FROM [dbo].[Teams] WHERE CONCAT([Team_City],' ',[Team_Name]) = '" + team_name + "'", "ID_Team");
            DataTable dt = ds.Tables["ID_Team"];
            return (byte)dt.Rows[0][0];
        }

        public string getTeamFullName(byte teamID)
        {
            Team aux = teams.Single(t => t.ID_Team == teamID);
            return aux.Team_City + ' ' + aux.Team_Name;
        }
    }
}
