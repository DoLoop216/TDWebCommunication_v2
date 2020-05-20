using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDWebCommunication
{
    public class Porudzbina
    {
        public int PorudzbinaID { get; set; }
        public int UserID { get; set; }
        public int BrDokKom { get; set; }
        public DateTime Datum { get; set; }
        public status Status { get; set; }
        public int MagacinID { get; set; }
        public List<Item> Items { get { return _Items; } set { _Items = value; } }
        public int? PPID { get; set; }

        public string UserDisplayName { get; set; }

        private List<Item> _Items = new List<Item>();

        public enum status
        {
            ObradadjujeSe = 0,
            CekaUplatu = 1,
            ZaPreuzimanje = 2,
            Realizovano = 3,
            Stornirana = 4
        }

        public class Item
        {
            public int PorudzbinaItemID { get; set; }
            public int PorudzbinaID { get; set; }
            public int RobaID { get; set; }
            public string NazivRobe { get; set; }
            public string KatBrRobe { get; set; }
            public double Kolicina { get; set; }
            public double VpCena { get; set; }

            public static List<Item> Get(int PorudzbinaID)
            {
                List<Item> list = new List<Item>();

                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT PORUDZBINA_ITEM.PORUDZBINA_ITEM_ID, PORUDZBINA_ITEM.PORUDZBINAID, PORUDZBINA_ITEM.ROBAID, PORUDZBINA_ITEM.KOLICINA, PORUDZBINA_ITEM.VPCENA, ROBA.KATBR, ROBA.NAZIV FROM PORUDZBINA_ITEM LEFT JOIN ROBA ON PORUDZBINA_ITEM.ROBAID = ROBA.ROBAID WHERE PORUDZBINAID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@P", PorudzbinaID);

                        MySqlDataReader dr = cmd.ExecuteReader();

                        while(dr.Read())
                        {
                            list.Add(new Item() { PorudzbinaItemID = Convert.ToInt32(dr[0]), PorudzbinaID = Convert.ToInt32(dr[1]), RobaID = Convert.ToInt32(dr[2]), Kolicina = Convert.ToDouble(dr[3]), VpCena = Convert.ToDouble(dr[4]), KatBrRobe = dr[5].ToString(), NazivRobe = dr[6].ToString() });
                        }
                    }
                }

                return list;
            }
        }

        public Porudzbina() { }
        public Porudzbina(int ID)
        {
            if (!TDWebCommunication.Initialized)
                throw new Exception("TDWebCommunication is not initialized!");
            try
            {
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT PORUDZBINAID, USERID, BRDOKKOM, DATUM, STATUS, MAGACINID, PPID FROM PORUDZBINA WHERE PORUDZBINAID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@P", ID);

                        MySqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            PorudzbinaID = Convert.ToInt32(dr[0]);
                            UserID = Convert.ToInt32(dr[1]);
                            BrDokKom = Convert.ToInt32(dr[2]);
                            Datum = Convert.ToDateTime(dr[3]);
                            Status = (status)Convert.ToInt32(dr[4]);
                            MagacinID = Convert.ToInt32(dr[5]);
                            PPID = (dr[6] is DBNull) ? null : (int?)Convert.ToInt32(dr[6]);
                        }
                        else
                            throw new Exception("Proudzbina sa id-em nije pronadjena!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public void UcitajStavke()
        {
            try
            {
                Items = Item.Get(PorudzbinaID);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        public void PoveziSaDokumentom(int BrDok)
        {
            if(BrDokKom > 0)
                throw new Exception("Dokument je vec povezan!");
            try
            {
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE PORUDZBINA SET BRDOKKOM = @B WHERE PORUDZBINAID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@B", BrDok);
                        cmd.Parameters.AddWithValue("@P", PorudzbinaID);

                        cmd.ExecuteNonQuery();
                        this.BrDokKom = BrDok;

                        Debug.Log(string.Format("Porudzbina {0} je povezana sa dokumentom {1}!", PorudzbinaID, BrDok));
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        public void AzurirajStatus(status Status)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE PORUDZBINA SET STATUS = @S WHERE PORUDZBINAID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@P", PorudzbinaID);
                        cmd.Parameters.AddWithValue("@S", (int)Status);

                        cmd.ExecuteNonQuery();

                        this.Status = Status;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        public void AzurirajMagacinID(int MagacinID)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE PORUDZBINA SET MAGACINID = @MID WHERE PORUDZBINAID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@MID", MagacinID);
                        cmd.Parameters.AddWithValue("@S", (int)Status);

                        cmd.ExecuteNonQuery();

                        this.MagacinID = MagacinID;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns list of all Porudzbina items in table Porudzbina</returns>
        public static List<Porudzbina> List()
        {
            if (!TDWebCommunication.Initialized)
                throw new Exception("TDWebCommunication is not initialized!");
            try
            {
                List<Porudzbina> list = new List<Porudzbina>();
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT PORUDZBINA.PORUDZBINAID, PORUDZBINA.USERID, 
                        PORUDZBINA.BRDOKKOM, PORUDZBINA.DATUM, PORUDZBINA.STATUS, PORUDZBINA.MAGACINID, USER.displayname 
                        FROM PORUDZBINA LEFT JOIN USER ON PORUDZBINA.userid = USER.USERID", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                            list.Add(new Porudzbina() { PorudzbinaID = Convert.ToInt32(dr[0]), UserID = Convert.ToInt32(dr[1]), BrDokKom = Convert.ToInt32(dr[2]), Datum = Convert.ToDateTime(dr[3]), Status = (status)Convert.ToInt32(dr[4]), MagacinID = Convert.ToInt32(dr[5]), UserDisplayName = dr[6].ToString() });
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status"></param>
        /// <returns>List of all items depending on Status</returns>
        public static List<Porudzbina> List(status Status)
        {
            if (!TDWebCommunication.Initialized)
                throw new Exception("TDWebCommunication is not initialized!");
            try
            {
                List<Porudzbina> list = new List<Porudzbina>();
                using (MySqlConnection con = new MySqlConnection(TDWebCommunication.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT PORUDZBINA.PORUDZBINAID, PORUDZBINA.USERID, PORUDZBINA.BRDOKKOM, PORUDZBINA.DATUM, PORUDZBINA.STATUS, PORUDZBINA.MAGACINID, USER.displayname FROM PORUDZBINA LEFT JOIN USER ON PORUDZBINA.userid = USER.USERID WHERE STATUS = @S", con))
                    {
                        cmd.Parameters.AddWithValue("@S", (int)Status);

                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                            list.Add(new Porudzbina() { PorudzbinaID = Convert.ToInt32(dr[0]), UserID = Convert.ToInt32(dr[1]), BrDokKom = Convert.ToInt32(dr[2]), Datum = Convert.ToDateTime(dr[3]), Status = (status)Convert.ToInt32(dr[4]), MagacinID = Convert.ToInt32(dr[5]), UserDisplayName = dr[6].ToString() });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return null;
            }
        }
    }
}
