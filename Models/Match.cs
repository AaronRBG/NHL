using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Match
    {
        public ulong ID_Match { get; set; }
        public byte Local_Team { get; set; }
        public byte Visitor_Team { get; set; }
        public byte Local_Goals { get; set; }
        public byte Visitor_Goals { get; set; }
        public DateTime Match_Date { get; set; }
        public byte Season { get; set; }
        public byte Winner { get; set; }
        public byte Win_Type { get; set; }
        public bool Regular_Season { get; set; }
    }
}
