using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDWebCommunication
{
    public class Roba
    {
        /// <returns>List of IDs of all roba in database</returns>
        public static List<int> ListIDs()
        {
            List<int> list = new List<int>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ROBAID FROM ROBA", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                            list.Add(Convert.ToInt32(dr[0]));
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return list;
        }
    }
}
