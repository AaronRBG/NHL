using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class Broker
{
    private readonly SqlDataAdapter adp = new SqlDataAdapter();

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
        SqlConnection con = new SqlConnection("server = localhost; database = NHL; Trusted_Connection = True;");
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
}

