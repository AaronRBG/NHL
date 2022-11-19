using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NHL.Models
{
    public class Broker
    {
        private readonly SqlDataAdapter adp = new SqlDataAdapter();
        private const string connectionString = "server = localhost; database = NHL; Trusted_Connection = True;";

        private static Broker _instance;

        public Broker() { }

        public static Broker Instance()

        {
            if (_instance == null)
            {
                _instance = new Broker();
            }
            return _instance;
        }

        public DataSet Run(string query, string reff)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand comm = new SqlCommand(query, con);
            DataSet ds = new DataSet();
            try
            {
                adp.SelectCommand = comm;
                adp.Fill(ds, reff);
            }
            catch (Exception)
            {
                con.Close();
            }

            con.Close();
            return ds;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}