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
            for(int i=0; i<dt.Rows.Count; i++)
            {
                Season s = new Season((byte)dt.Rows[i][0], (string)dt.Rows[i][1], (DateTime)dt.Rows[i][2], (DateTime)dt.Rows[i][3], (DateTime)dt.Rows[i][4], (DateTime)dt.Rows[i][5], (byte)dt.Rows[i][6]);
                aux.Add(s);
            }
            seasons = aux.ToArray();
        }

        public byte getSeason(DateTime date)
        {
            DataSet ds = Broker.Instance().Run("SELECT [ID_Season] FROM [dbo].[Seasons] WHERE ( [Start_Date] <= '" 
                + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND [Finish_Date] >= '" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') OR ( [Playoff_Start_Date] <= '" 
                + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND [Playoff_Finish_Date] >= '" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')", "ID_Season");
            DataTable dt = ds.Tables["ID_Season"];
            return (byte)dt.Rows[0][0];
        }

        public bool isRegularSeason(DateTime date, byte ID_Season)
        {
            DataSet ds = Broker.Instance().Run("SELECT IIF( [Start_Date] <= '" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND [Finish_Date] >= '" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "', 1, 0) FROM [dbo].[Seasons] WHERE [ID_Season] = " + ID_Season, "Regular_Season?");
            DataTable dt = ds.Tables["Regular_Season?"];
            return (int)dt.Rows[0][0] == 1;
        }
    }
}
