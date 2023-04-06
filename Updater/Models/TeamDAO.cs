using System.Collections.Generic;
using System.Data;

namespace NHL_Updater.Models
{
    public class TeamDAO
    {
        public Team[] teams { get; set; }

        public TeamDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT ID_Team, Team_Name FROM [dbo].[Teams]", "Teams");
            DataTable dt = ds.Tables["Teams"];
            List<Team> aux = new List<Team>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Team t = new Team((byte)dt.Rows[i][0], (string)dt.Rows[i][1]);
                aux.Add(t);
            }
            teams = aux.ToArray();
        }

        public byte getTeamID(string team_name)
        {
            //string[] levels = { "Team_Alias4", "Team_Alias3", "Team_Alias2", "Team_Alias1", "Team_Name" };
            //string[] levels = { "Team_Name", "Team_Alias1", "Team_Alias2", "Team_Alias3", "Team_Alias4" };
            bool found = false;
            byte res = 0;
            while (!found)
            {
                DataSet ds = Broker.Instance().Run("SELECT [ID_Team] FROM [dbo].[Teams] WHERE [Team_Name] = '" + team_name + "'", "ID_Team");
                DataTable dt = ds.Tables["ID_Team"];
                if(dt.Rows.Count != 0)
                {
                    found = true;
                    res = (byte)dt.Rows[0][0];
                } else
                {
                    res++;
                }
            }
            return res;
        }

    }
}
