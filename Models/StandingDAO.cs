using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class StandingDAO
    {
        public Standing[] standings { get; set; }

        public StandingDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Standings] ORDER BY [Points] DESC, [Regulation_Wins] DESC", "Standings");
            DataTable dt = ds.Tables["Standings"];
            List<Standing> aux = new List<Standing>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Standing s = new Standing((byte)dt.Rows[i][0], (byte)dt.Rows[i][1], (byte)dt.Rows[i][2], (byte)dt.Rows[i][3], (byte)dt.Rows[i][4], (byte)dt.Rows[i][5], (byte)dt.Rows[i][6], (byte)dt.Rows[i][7], (byte)dt.Rows[i][8], (byte)dt.Rows[i][9], (byte)dt.Rows[i][10]);
                aux.Add(s);
            }
            standings = aux.ToArray();
        }
    }
}
