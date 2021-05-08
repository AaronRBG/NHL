using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Conference
    {
        public Conference(byte iD_Conference, string conference_Name)
        {
            ID_Conference = iD_Conference;
            Conference_Name = conference_Name;
        }

        public byte ID_Conference { get; set; }
        public string Conference_Name { get; set; }
    }
}
