using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Standing
    {
        public byte ID_Team { get; set; }
        public byte Pobytes { get; set; }
        public byte Matches_Played { get; set; }
        public byte Matches_Left { get; set; }
        public byte Regulation_Wins { get; set; }
        public byte Regulation_Losses { get; set; }
        public byte Overtime_Wins { get; set; }
        public byte Overtime_Losses { get; set; }
        public byte Shootout_Wins { get; set; }
        public byte Shootout_Losses { get; set; }
        public byte ID_Season_Losses { get; set; }
    }
}
