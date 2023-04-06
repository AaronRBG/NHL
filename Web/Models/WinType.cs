using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class WinType
    {
        public WinType(byte iD_Win_Type, string win_Type_Name)
        {
            ID_Win_Type = iD_Win_Type;
            Win_Type_Name = win_Type_Name;
        }

        public byte ID_Win_Type { get; set; }
        public string Win_Type_Name { get; set; }
    }
}
