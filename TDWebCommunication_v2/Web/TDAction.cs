using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDWebCommunication_v2.Web
{
    public class TDAction
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public int Sender { get; set; }

        public static TDAction Get()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ID, ACTION, DATE, SENDER FROM AKC ORDER BY ID DESC LIMIT 1", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                            return new TDAction() { ID = Convert.ToInt32(dr[0]), Action = dr[1].ToString(), Date = Convert.ToDateTime(dr[2]), Sender = Convert.ToInt32(dr[3]) };
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public void Remove()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM AKC WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
