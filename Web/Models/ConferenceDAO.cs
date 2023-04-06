using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class ConferenceDAO
    {
        public Conference[] conferences { get; set; }

        public ConferenceDAO() 
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Conferences]", "Conferences");
            DataTable dt = ds.Tables["Conferences"];
            List<Conference> aux = new List<Conference>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Conference wt = new Conference((byte)dt.Rows[i][0], (string)dt.Rows[i][1]);
                aux.Add(wt);
            }
            conferences = aux.ToArray();
        }
    }
}
