using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Season
    {
        public byte ID_Season { get; set; }
        public string Season_Name { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Finish_Date { get; set; }
        public byte N_Matches_Team { get; set; }
    }
}
