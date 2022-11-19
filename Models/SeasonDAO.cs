using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace NHL.Models
{
    public class SeasonDAO
    {
        public List<Season> seasons;
        public static int Current_Season;
        public SeasonDAO()
        {
            string query = "SELECT* FROM[dbo].[Seasons]";
            seasons = Broker.Instance().GetConnection().Query<Season>(query).ToList();

            Current_Season = getCurrentSeason();
        }

        private static int getCurrentSeason()
        {
            DataSet ds = Broker.Instance().Run("SELECT ID_Season FROM Seasons WHERE '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' > [Start_Date] AND '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' < [Finish_Date]", "Current_Season");
            DataTable dt = ds.Tables["Current_Season"];
            int result;
            if (dt.Rows.Count > 0)
            {
                result = (int)dt.Rows[0][0];
            }
            else
            {
                result = DateTime.Today.Year;
            }

            return result;
        }

        public void insert(Season s)
        {
            Broker.Instance().Run("INSERT INTO [dbo].[Seasons] VALUES ('" + s.Start_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', '" + s.Finish_Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," + s.N_Matches_Team + ")", "Current_Season");
        }

    }
}
