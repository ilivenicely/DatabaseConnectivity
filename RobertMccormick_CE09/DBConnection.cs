// Robert Mccormick
// Frameworks
// Term 3
// RobertMcCormick_CE09

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstnameLastname_CE09
{
    public class DBConnection
    {
        private DBConnection()
        {
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        //set null instance
        private static DBConnection _instance = null;

        //if null then instantiate
        public  static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }



        //connect to IP and database
        public bool IsConnect()
        {
            bool result = true;
            if (Connection == null)
            {
                try
                {

                    // @"server= " + GetConnectionString() + " ; userid=dbsAdmin; password=password; database= example_dbs1; port=8889"
                    //connection string
                    connection = new MySqlConnection(@"server= " + GetConnectionString() + " ; userid=dbsAdmin; password=password; database= example_dbs1; port=8889");
                    connection.Open();
                    result = true;
                }catch(Exception ex)
                {
                    result = false;
                }
            }

            return result;
        }

        public void Close()
        {
            connection.Close();
        }

        public string GetConnectionString()
        {
             return File.ReadAllText(@"C:\\VFW\\connect.txt");

           // return (@"server=172.16.29.1 ; userid=dbsAdmin; password=password; database= example_dbs1; port=8889");

           // uh = new MySqlConnection(@"server=172.16.29.1 ; userid=dbsAdmin; password=password; database= example_dbs1; port=8889")
        }
    }
}
