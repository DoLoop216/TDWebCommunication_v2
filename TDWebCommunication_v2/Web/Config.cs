using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDWebCommunication_v2.Web
{
    public class Config
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public static Config Get(string Name)
        {
            try
            {
                Config c = new Config();
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ID, VALUE FROM CONFIG WHERE NAME = @N", con))
                    {
                        cmd.Parameters.AddWithValue("@N", Name);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                c.ID = Convert.ToInt32(dr["ID"]);
                                c.Value = dr["VALUE"].ToString();
                                c.Name = Name;
                            }
                            else
                            {
                                throw new Exception("Confing with given name not found!");
                            }
                        }
                    }
                }
                return c;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return null;
            }
        }
        public static void Update(int ID, string Value)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE CONFIG SET VALUE = @V WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@V", Value);
                        cmd.Parameters.AddWithValue("@ID", ID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        public static void Update(string Name, string Value)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE CONFIG SET VALUE = @V WHERE NAME = @N", con))
                    {
                        cmd.Parameters.AddWithValue("@V", Value);
                        cmd.Parameters.AddWithValue("@N", Name);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
}
