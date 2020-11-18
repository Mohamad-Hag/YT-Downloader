using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Windows.Forms;

namespace YT_Downloader.Classes
{
    class DatabaseConnectivity
    {
        private string DatabaseName { get; set; }
        private MySqlConnection Connection = new MySqlConnection();
        public  DatabaseConnectivity()
        {
            Connection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
        }
        /// <summary>
        /// connect to mysql server.
        /// </summary>
        /// <returns>true if connection successfull and false otherwise.</returns>
        public bool Connect()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Unconnect to mysql server.
        /// </summary>
        /// <returns>true if unconnection successfull and false otherwise.</returns>
        public bool Unconnect()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public MySqlDataReader ExecuteReader(string query)
        {
            try
            {
                MySqlCommand command = new MySqlCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                command.Connection = Connection;
                return command.ExecuteReader();
            }
            catch
            {
                return null;
            }
        }
        public void ExecuteNonQuery(string query)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            command.Connection = Connection;
            command.ExecuteNonQuery();
        }
        public void Execute(MySqlCommand command, string query)
        {
            command.CommandType = CommandType.Text;
            command.Connection = Connection;
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
        public void ExecuteReader(MySqlCommand command, string query)
        {
            command.CommandType = CommandType.Text;
            command.Connection = Connection;
            command.CommandText = query;
        }
    }
}
