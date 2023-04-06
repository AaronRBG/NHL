using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Division
    {
        public Division(byte iD_Division, string division_Name)
        {
            ID_Division = iD_Division;
            Division_Name = division_Name;
        }

        public byte ID_Division { get; set; }
        public string Division_Name { get; set; }
    }
}
