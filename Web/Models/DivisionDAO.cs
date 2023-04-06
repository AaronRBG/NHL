using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class DivisionDAO
    {
        public Division[] divisions { get; set; }

        public DivisionDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Divisions]", "Divisions");
            DataTable dt = ds.Tables["Divisions"];
            List<Division> aux = new List<Division>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Division wt = new Division((byte)dt.Rows[i][0], (string)dt.Rows[i][1]);
                aux.Add(wt);
            }
            divisions = aux.ToArray();
        }
    }
}
