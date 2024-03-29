﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace NHL.Models
{
    public class StandingDAO
    {
        public List<Standing> standings;
        public Dictionary<byte, double[]> magicNumbersDivision { get; set; }
        public Dictionary<byte, double[]> magicNumbersConference { get; set; }

        public StandingDAO()
        {
            string query = "SELECT * FROM [dbo].[Standings] WHERE ID_Season = " + SeasonDAO.Current_Season + " ORDER BY [Points] DESC, [Regulation_Wins] DESC";
            standings = Broker.Instance().GetConnection().Query<Standing>(query).ToList();

            foreach (Standing s in standings)
            {
                s.Division_Tiebreaker = getDivisionTiebreaker(s.ID_Team, s.ID_Season);
                s.Conference_Tiebreaker = getConferenceTiebreaker(s.ID_Team, s.ID_Season);
            }
            double[][] h2h = calculateH2H();
            int[][] pb = calculatePB();
            magicNumbersDivision = calculateMagicNumbersDivision(h2h, pb);
            magicNumbersConference = calculateMagicNumbersConference(h2h, pb);
        }

        public short getConferenceTiebreaker(byte ID_Team, int ID_Season)
        {
            short res = 0;
            Standing team = standings.First(t => t.ID_Team == ID_Team && t.ID_Season == ID_Season);

            byte[] teams = standings.Where(t => t.Points == team.Points && t.Conference == team.Conference && t.ID_Season == ID_Season).Select(t => t.ID_Team).ToArray();

            if (teams.Length > 0)
            {
                res = getTiebreakers(ID_Team, ID_Season, teams);
            }

            return res;
        }

        public short getDivisionTiebreaker(byte ID_Team, int ID_Season)
        {
            short res = 0;
            Standing team = standings.First(t => t.ID_Team == ID_Team && t.ID_Season == ID_Season);

            byte[] teams = standings.Where(t => t.Points == team.Points && t.Division == team.Division && t.ID_Season == ID_Season).Select(t => t.ID_Team).ToArray();

            if (teams.Length > 0)
            {
                res = getTiebreakers(ID_Team, ID_Season, teams);
            }

            return res;
        }

        private short getTiebreakers(byte ID_Team, int ID_Season, byte[] teams)
        {
            short res = 0;

            foreach (byte team in teams)
            {
                res += MatchDAO.getTiebreaker(ID_Team, team, ID_Season);
            }

            return res;
        }

        public Standing[] getDivisionTeams(byte division)
        {
            return standings.Where(s => s.Division == division)
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.Matches_Left)
                .ThenByDescending(s => s.Regulation_Wins)
                .ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins)
                .ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins + s.Shootout_Wins)
                .ThenByDescending(s => s.Goal_Difference)
                .ThenByDescending(s => s.Goals_For).ToArray();
        }
        public byte[] getDivisionTeamsID(byte division)
        {
            return getDivisionTeams(division).Select(s => s.ID_Team).ToArray();
        }

        public Standing[] getConferenceTeams(byte conference, bool prune3firsts)
        {
            List<Standing> teams = standings.Where(s => s.Conference == conference)
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.Matches_Left)
                .ThenByDescending(s => s.Regulation_Wins)
                .ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins)
                .ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins + s.Shootout_Wins)
                .ThenByDescending(s => s.Goal_Difference)
                .ThenByDescending(s => s.Goals_For).ToList();

            if (prune3firsts)
            {
                byte[] divisions = getConferenceDivisions(conference);

                byte[] teamsA = getDivisionTeamsID(divisions[0]).ToArray();
                byte[] teamsB = getDivisionTeamsID(divisions[1]).ToArray();

                for (int i = 0; i < 3; i++)
                {
                    teams.Remove(teams.First(s => s.ID_Team == teamsA[i]));
                    teams.Remove(teams.First(s => s.ID_Team == teamsB[i]));
                }
            }

            return teams.ToArray();
        }

        public byte[] getConferenceDivisions(byte conference)
        {
            return standings.Where(s => s.Conference == conference).Select(s => s.Division).Distinct().ToArray();
        }

        public byte[] getConferenceTeamsID(byte conference, bool prune3firsts)
        {
            return getConferenceTeams(conference, prune3firsts).Select(s => s.ID_Team).ToArray();
        }

        private double[][] calculateH2H()
        {
            const int N_TEAMS = 32;
            double[][] res = new double[N_TEAMS][];

            for (int i = 0; i < N_TEAMS; i++)
            {
                double[] aux = new double[N_TEAMS];
                for (int j = 0; j < N_TEAMS; j++)
                {
                    try
                    {

                        DataSet ds = Broker.Instance().Run("SELECT TOP 1 [dbo].calculateH2H(" + (i + 1).ToString() + "," + (j + 1).ToString() + "," + SeasonDAO.Current_Season + ") FROM [dbo].[Standings]", "H2H");
                        DataTable dt = ds.Tables["H2H"];
                        byte h2h = (byte)dt.Rows[0][0];
                        aux[j] = h2h switch
                        {
                            1 => 0.5,
                            2 => -0.5,
                            _ => 0,
                        };
                    } catch (Exception)
                    {
                        
                        aux[j] = (byte)0;
                    }
                }
                res[i] = aux;
            }
            return res;
        }

        private int[][] calculatePB()       // PB is Points Behind // We search in pb[ -1] because of how it is calculated
        {
            const int N_TEAMS = 32;
            int[][] res = new int[N_TEAMS][];

            for (int i = 0; i < N_TEAMS; i++)
            {
                int[] aux = new int[N_TEAMS];
                for (int j = 0; j < N_TEAMS; j++)
                {
                    Standing a = standings.First(s => s.ID_Team == i + 1);
                    Standing b = standings.First(s => s.ID_Team == j + 1);
                    aux[j] = a.Points - b.Points;
                }
                res[i] = aux;
            }
            return res;
        }

        private Dictionary<byte, double[]> calculateMagicNumbersDivision(double[][] h2h, int[][] pb)
        {
            Dictionary<byte, double[]> res = new Dictionary<byte, double[]>();

            Dictionary<byte, int> Plooseness = new Dictionary<byte, int>();

            {
                byte[] PloosenessKey = standings.Select(s => s.ID_Team).ToArray();
                int[] PloosenessValue = standings.Select(s => s.Matches_Left * 2).ToArray();
                for (int i = 0; i < PloosenessKey.Length; i++)
                {
                    Plooseness.Add(PloosenessKey[i], PloosenessValue[i]);
                }
            }

            for (byte division = 1; division < 5; division++)
            {
                byte[] teams = getDivisionTeamsID(division);

                for (int i = 0; i < teams.Length; i++)
                {
                    double[] aux = new double[teams.Length];
                    for (int j = 0; j < teams.Length; j++)
                    {
                        int k = j;
                        if (j >= i && j != teams.Length - 1)
                        {
                            k = j + 1;
                            if (j != teams.Length - 1)
                            {
                                aux[j] = h2h[teams[i] - 1][teams[k] - 1];
                            }
                        }
                        int Clooseness = Plooseness[teams[i]] + Plooseness[teams[k]];
                        aux[j] += Plooseness[teams[k]] - pb[teams[i] - 1][teams[k] - 1];
                        if (aux[j] > Clooseness)
                        {
                            aux[j] = double.MaxValue;
                        }
                        else if (aux[j] > Plooseness[teams[i]])
                        {
                            aux[j] = (aux[j] * 2 * Math.PI);
                        }
                    }
                    Array.Sort(aux);
                    Array.Reverse(aux);
                    res.Add(teams[i], aux);
                }
            }
            return res;
        }
        private Dictionary<byte, double[]> calculateMagicNumbersConference(double[][] h2h, int[][] pb)
        {
            Dictionary<byte, double[]> res = new Dictionary<byte, double[]>();

            Dictionary<byte, int> Plooseness = new Dictionary<byte, int>();

            {
                byte[] PloosenessKey = standings.Select(s => s.ID_Team).ToArray();
                int[] PloosenessValue = standings.Select(s => s.Matches_Left * 2).ToArray();
                for (int i = 0; i < PloosenessKey.Length; i++)
                {
                    Plooseness.Add(PloosenessKey[i], PloosenessValue[i]);
                }
            }

            for (byte conference = 1; conference <= 2; conference++)
            {
                byte[] teams = getConferenceTeamsID(conference, false);  // True prunes the 3 firsts

                for (int i = 0; i < teams.Length; i++)
                {
                    double[] aux = new double[teams.Length];
                    for (int j = 0; j < teams.Length; j++)
                    {
                        int k = j;
                        if (j >= i && j != teams.Length - 1)
                        {
                            k = j + 1;
                            if (j != teams.Length - 1)
                            {
                                aux[j] = h2h[teams[i] - 1][teams[k] - 1];
                            }
                        }
                        int Clooseness = Plooseness[teams[i]] + Plooseness[teams[k]];
                        aux[j] += Plooseness[teams[k]] - pb[teams[i] - 1][teams[k] - 1];
                        if (aux[j] > Clooseness)
                        {
                            aux[j] = double.MaxValue;
                        }
                        else if (aux[j] > Plooseness[teams[i]])
                        {
                            aux[j] = (aux[j] * 2 * Math.PI);
                        }
                    }
                    Array.Sort(aux);
                    Array.Reverse(aux);
                    res.Add(teams[i], aux);
                }
            }
            return res;
        }
    }
}
