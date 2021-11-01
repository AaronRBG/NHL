using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class StandingDAO
    {
        public Standing[] standings { get; set; }
        public Dictionary<byte, double[]> magicNumbersDivision { get; set; }
        public Dictionary<byte, double[]> magicNumbersConference { get; set; }

        public StandingDAO()
        {
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Standings] WHERE ID_Season = " + SeasonDAO.getCurrentSeason() + " ORDER BY [Points] DESC, [Regulation_Wins] DESC", "Standings");
            DataTable dt = ds.Tables["Standings"];
            List<Standing> aux = new List<Standing>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Standing s = new Standing((byte)dt.Rows[i][0], (byte)dt.Rows[i][1], (byte)dt.Rows[i][2], (byte)dt.Rows[i][3], (byte)dt.Rows[i][4], (byte)dt.Rows[i][5], (byte)dt.Rows[i][6], (byte)dt.Rows[i][7], (byte)dt.Rows[i][8], (byte)dt.Rows[i][9], (int)dt.Rows[i][10], (byte)dt.Rows[i][11], (byte)dt.Rows[i][12], (short)dt.Rows[i][13], (short)dt.Rows[i][14], (short)dt.Rows[i][15]);
                aux.Add(s);
            }
            standings = aux.ToArray();
            foreach(Standing s in standings)
            {
                s.Division_Tiebreaker = getDivisionTiebreaker(s.ID_Team, s.ID_Season);
                s.Conference_Tiebreaker = getConferenceTiebreaker(s.ID_Team, s.ID_Season);
            }
            double[][] h2h = calculateH2H();
            int[][] pb = calculatePB();
            magicNumbersDivision = calculateMagicNumbersDivision(h2h, pb);
        }

        public short getConferenceTiebreaker(byte ID_Team, int ID_Season)
        {
            throw new NotImplementedException();
        }

        public short getDivisionTiebreaker(byte ID_Team, int ID_Season)
        {
            short res = 0;
            Standing team = standings.Where(t => t.ID_Team == ID_Team && t.ID_Season == ID_Season).First();

            standings.Where(t => t.Points == team.Points && t.Division == team.Division);

            return res;
        }

        public Standing[] getDivisionTeams(byte division)
        {
            return standings.Where(s => s.Division == division).OrderByDescending(s => s.Points).ThenByDescending(s => s.Matches_Left).ThenByDescending(s => s.Regulation_Wins).ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins).ThenByDescending(s => s.Regulation_Wins + s.Overtime_Wins + s.Shootout_Wins).ThenByDescending(s => s.Goal_Difference).ThenByDescending(s => s.Goals_For).ToArray();
        }
        public byte[] getDivisionTeamsID(byte division)
        {
            return getDivisionTeams(division).Select(s => s.ID_Team).ToArray();
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
                    DataSet ds = Broker.Instance().Run("SELECT TOP 1 [dbo].calculateH2H(" + (i + 1).ToString() + "," + (j + 1).ToString() + "," + SeasonDAO.getCurrentSeason() + ") FROM [dbo].[Standings]", "H2H");
                    DataTable dt = ds.Tables["H2H"];
                    byte h2h = (byte)dt.Rows[0][0];
                    aux[j] = h2h switch
                    {
                        1 => 0.5,
                        2 => -0.5,
                        _ => 0,
                    };
                }
                res[i] = aux;
            }
            return res;
        }

        private int[][] calculatePB()
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

            int[] Plooseness = standings.Select(s => s.Matches_Left * 2).ToArray();

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
                        aux[j] += standings.Where(s => s.ID_Team == teams[i]).Select(s => s.Matches_Left * 2).First() - pb[teams[i] - 1][teams[k] - 1];
                        int Clooseness = Plooseness[teams[i] - 1] / 2 + Plooseness[teams[k] - 1] / 2;
                        if (aux[j] > Plooseness[i]*2)
                        {
                            aux[j] = double.MaxValue;
                        }
                        else if (aux[j] > Clooseness)
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
