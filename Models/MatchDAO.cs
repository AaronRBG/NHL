using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class MatchDAO
    {
        public List<Match> matches { get; set; }

        public MatchDAO()
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Matches]", "Matches");
            DataTable dt = ds.Tables["Matches"];
            List<Match> aux = new List<Match>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Match m = new Match((long)dt.Rows[i][0], (byte)dt.Rows[i][1], (byte)dt.Rows[i][2], (byte)dt.Rows[i][3], (byte)dt.Rows[i][4], (DateTime)dt.Rows[i][5], (int)dt.Rows[i][6], (byte)dt.Rows[i][7], (byte)dt.Rows[i][8], (bool)dt.Rows[i][9]);
                aux.Add(m);
            }
            matches = aux;
        }

        public void insertNewMatches(Match[] matches)
        {
            foreach (Match m in matches)
            {
                if (!exists(m))
                {
                    byte rs = 0;
                    if (m.Regular_Season)
                    {
                        rs = 1;
                    }
                    Broker.Instance().Run("INSERT INTO [dbo].[Matches] VALUES ("
                        + m.Local_Team + ','
                        + m.Visitor_Team + ','
                        + m.Local_Goals + ','
                        + m.Visitor_Goals + ",'"
                        + m.Match_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "',"
                        + m.Season + ','
                        + m.Win_Type + ','
                        + rs + ')', "insertNewMatches");
                    this.matches.Add(m);
                }
            }
        }

        public bool exists(Match m)
        {
            DataSet ds = Broker.Instance().Run("SELECT [ID_Match] FROM [dbo].[Matches] WHERE [Local_Team] = " + m.Local_Team
                + " AND [Visitor_Team] = " + m.Visitor_Team
                + " AND [Match_Date] = '" + m.Match_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'", "Exists?");
            DataTable dt = ds.Tables["Exists?"];
            bool b = false;
            if (dt.Rows.Count > 0)
            {
                b = true;
            }
            return b;
        }
    }
}
