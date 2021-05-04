using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Win_TypeDAO
    {
        public Win_Type[] win_types { get; set; }

        public Win_TypeDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Win_Types]", "Win_Types");
            DataTable dt = ds.Tables["Win_Types"];
            List<Win_Type> aux = new List<Win_Type>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Win_Type wt = new Win_Type((byte)dt.Rows[i][0], (string)dt.Rows[i][1]);
                aux.Add(wt);
            }
            win_types = aux.ToArray();
        }
    }
}
