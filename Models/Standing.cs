using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Standing
    {
        public Standing(byte iD_Team, byte points, byte matches_Played, byte matches_Left, byte regulation_Wins, byte regulation_Losses, byte overtime_Wins, byte overtime_Losses, byte shootout_Wins, byte shootout_Losses, byte iD_Season)
        {
            ID_Team = iD_Team;
            Points = points;
            Matches_Played = matches_Played;
            Matches_Left = matches_Left;
            Regulation_Wins = regulation_Wins;
            Regulation_Losses = regulation_Losses;
            Overtime_Wins = overtime_Wins;
            Overtime_Losses = overtime_Losses;
            Shootout_Wins = shootout_Wins;
            Shootout_Losses = shootout_Losses;
            ID_Season = iD_Season;
        }
        public byte ID_Team { get; set; }
        public byte Points { get; set; }
        public byte Matches_Played { get; set; }
        public byte Matches_Left { get; set; }
        public byte Regulation_Wins { get; set; }
        public byte Regulation_Losses { get; set; }
        public byte Overtime_Wins { get; set; }
        public byte Overtime_Losses { get; set; }
        public byte Shootout_Wins { get; set; }
        public byte Shootout_Losses { get; set; }
        public byte ID_Season { get; set; }
    }
}
