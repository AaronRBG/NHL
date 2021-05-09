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
            DataSet ds = Broker.Instance().Run("SELECT * FROM [dbo].[Standings] ORDER BY [Points] DESC, [Regulation_Wins] DESC", "Standings");
            DataTable dt = ds.Tables["Standings"];
            List<Standing> aux = new List<Standing>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Standing s = new Standing((byte)dt.Rows[i][0], (byte)dt.Rows[i][1], (byte)dt.Rows[i][2], (byte)dt.Rows[i][3], (byte)dt.Rows[i][4], (byte)dt.Rows[i][5], (byte)dt.Rows[i][6], (byte)dt.Rows[i][7], (byte)dt.Rows[i][8], (byte)dt.Rows[i][9], (byte)dt.Rows[i][10], (byte)dt.Rows[i][11], (byte)dt.Rows[i][12]);
                aux.Add(s);
            }
            standings = aux.ToArray();
            double[][] h2h = calculateH2H();
            int[][] pb = calculatePB();
            magicNumbersDivision = calculateMagicNumbersDivision(h2h, pb);
        }

        public byte[] getDivisionTeams(byte division)
        {
            return standings.Where(s => s.Division == division).Select(s => s.ID_Team).ToArray();
        }

        private double[][] calculateH2H()
        {
            const int N_TEAMS = 31;
            double[][] res = new double[N_TEAMS][];

            for (int i = 0; i < N_TEAMS; i++)
            {
                double[] aux = new double[N_TEAMS];
                for (int j = 0; j < N_TEAMS; j++)
                {
                    DataSet ds = Broker.Instance().Run("SELECT TOP 1 [dbo].calculateH2H(" + (i + 1).ToString() + "," + (j + 1).ToString() + ") FROM [dbo].[Standings]", "H2H");
                    DataTable dt = ds.Tables["H2H"];
                    byte h2h = (byte)dt.Rows[0][0];
                    switch (h2h)
                    {
                        case 1: aux[j] = 0.5; break;
                        case 2: aux[j] = -0.5; break;
                        default: aux[j] = 0; break;
                    }
                }
                res[i] = aux;
            }
            return res;
        }

        private int[][] calculatePB()
        {
            const int N_TEAMS = 31;
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
                byte[] teams = getDivisionTeams(division);

                for (int i = 0; i < teams.Length; i++)
                {
                    double[] aux = new double[teams.Length];
                    for (int j = 0; j < teams.Length; j++)
                    {
                        aux[j] = standings.Where(s => s.ID_Team == teams[i]).Select(s => s.Matches_Left * 2).First() - pb[teams[i] - 1][teams[j] - 1] + h2h[teams[i] - 1][teams[j] - 1];
                        int Clooseness = Plooseness[teams[i] - 1] / 2 + Plooseness[teams[j] - 1] / 2;
                        if (aux[j] > Plooseness[i])
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
