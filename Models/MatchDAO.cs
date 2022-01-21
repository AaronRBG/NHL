using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class MatchDAO
    {
        public static List<Match> matches { get; set; }

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
                    MatchDAO.matches.Add(m);
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

        public static short getTiebreaker(byte ID_TeamA, byte ID_TeamB, int ID_Season)
        {
            short res = 0;

            List<Match> loc = MatchDAO.matches.Where(m => m.Local_Team == ID_TeamA && m.Visitor_Team == ID_TeamB && m.Season == ID_Season).ToList();
            List<Match> vis = MatchDAO.matches.Where(m => m.Local_Team == ID_TeamB && m.Visitor_Team == ID_TeamA && m.Season == ID_Season).ToList();

            if (loc.Count != 0 && vis.Count != 0)
            {
                while (loc.Count > vis.Count)
                {
                    loc.RemoveAt(loc.Count - 1);
                }
                while (vis.Count > loc.Count)
                {
                    vis.RemoveAt(vis.Count - 1);
                }

                int a = loc.Count(m => m.Winner == ID_TeamA) * 2 + vis.Count(m => m.Winner == ID_TeamA) * 2 + loc.Count(m => m.Winner != ID_TeamA && m.Win_Type != 0) + vis.Count(m => m.Winner != ID_TeamA && m.Win_Type != 0);

                res = (short)(a / (loc.Count + vis.Count));
            }
            return res;
        }

    }
}
