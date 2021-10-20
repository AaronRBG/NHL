using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class SeasonDAO
    {
        public Season[] seasons { get; set; }
        public SeasonDAO()
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Seasons]", "Seasons");
            DataTable dt = ds.Tables["Seasons"];
            List<Season> aux = new List<Season>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Season s = new Season((int)dt.Rows[i][0], (DateTime)dt.Rows[i][1], (DateTime)dt.Rows[i][2], (byte)dt.Rows[i][3]);
                aux.Add(s);
            }
            seasons = aux.ToArray();
        }

        public static int getCurrentSeason()
        {
            DataSet ds = Broker.Instance().Run("SELECT ID_Season FROM Seasons WHERE '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' > [Start_Date] AND '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' < [Finish_Date]", "Current_Season");
            DataTable dt = ds.Tables["Current_Season"];
            if (dt.Rows.Count != 0)
            {
                return (int)dt.Rows[0][0];
            }
            else
            {
                return 2022;    // Return the default season value if between seasons
            }
        }

        public void insert(Season s)
        {
            Broker.Instance().Run("INSERT INTO [dbo].[Seasons] VALUES ('" + s.Start_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', '" + s.Finish_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," + s.N_Matches_Team + ")", "Current_Season");
        }

    }
}
