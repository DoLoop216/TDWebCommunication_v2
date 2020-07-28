using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AR;
using MySql.Data.MySqlClient;
using FirebirdSql.Data.FirebirdClient;
using System.Threading;
using System.Globalization;
using System.Data.Odbc;

namespace TDWebCommunication_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = false;

            Komercijalno.Initialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void porudzbineToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (TabelarniPregled mf = new TabelarniPregled())
            {
                mf.ShowDialog();
                Application.Exit();
            }
        }
        private void iRONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented!");
        }

        private void premaNabavnimCenamaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AnalizaCenaPremaNabavnim acpn = new AnalizaCenaPremaNabavnim())
            {
                acpn.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.IsRunning = false;
            Application.Exit();
        }

        private void nabavnaCenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Ovim cete pokrenuti azuriranje nabavnih cena na sajtu. Bicete obavesteni kada se proces zavrsi! Da li zelite da nastavite?", "Potvrdi", MessageBoxButtons.YesNo);

            if (d != DialogResult.Yes)
                return;


            try
            {
                List<Tuple<int, double>> list = new List<Tuple<int, double>>();
                using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                {
                    con.Open();
                    using(FbCommand cmd = new FbCommand("SELECT ROBAID, WEB_CENA_BP FROM WEB_CENA", con))
                    {
                        using(FbDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                list.Add(new Tuple<int, double>(Convert.ToInt32(dr[0]), Convert.ToDouble(dr[1])));
                    }
                }

                using(MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using(MySqlCommand cmd = new MySqlCommand("UPDATE PRODUCT SET PURCHASEPRICE = @NC WHERE ID = @RID", con))
                    {
                        cmd.Parameters.Add("@NC", MySqlDbType.Double);
                        cmd.Parameters.Add("@RID", MySqlDbType.Int32);

                        foreach(Tuple<int, double> i in list)
                        {
                            cmd.Parameters["@NC"].Value = i.Item2;
                            cmd.Parameters["@RID"].Value = i.Item1;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Azuriranje nabavnih cena izvrseno!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ironCenastandardnaMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult d = MessageBox.Show("Ovim cete pokrenuti akciju azuriranja iron cena. Vucem standardne MP cene iz M50 i njih koristim kao IRON. Bicete obavesteni kada se akcija zavrsi! Da li zelite da nastavite?", "Potvrdi", MessageBoxButtons.YesNo);

            if (d != DialogResult.Yes)
                return;


            try
            {
                List<Tuple<int, double>> list = new List<Tuple<int, double>>();
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand("SELECT ROBAID, PRODAJNACENA FROM ROBAUMAGACINU WHERE MAGACINID = 50", con))
                    {
                        using (FbDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                list.Add(new Tuple<int, double>(Convert.ToInt32(dr[0]), Convert.ToDouble(dr[1])));
                    }
                }

                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE PRODUCT SET PRICE = @PC WHERE ID = @RID", con))
                    {
                        cmd.Parameters.Add("@PC", MySqlDbType.Double);
                        cmd.Parameters.Add("@RID", MySqlDbType.Int32);

                        foreach (Tuple<int, double> i in list)
                        {
                            cmd.Parameters["@PC"].Value = i.Item2;
                            cmd.Parameters["@RID"].Value = i.Item1;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Azuriranje iron cena izvrseno!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void uporediSaMPRacunimaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (UporediSaMP u = new UporediSaMP())
                u.ShowDialog();
        }

        private void proveraCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Komercijalno: " + Buffer.ConnectionStrings.Komercijalno);
            MessageBox.Show("TDOffice: " + Buffer.ConnectionStrings.TDOffice);
            MessageBox.Show("Web: " + Buffer.ConnectionStrings.Web);
            MessageBox.Show("Config: " + Buffer.ConnectionStrings.Config);
        }

        private void azurirajKataloskeBrojeveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Da li sigurno zelite da pobucete kataloske brojeve iz komercijalnog na web?", "Potvrdi", MessageBoxButtons.YesNo);
            if(dr == DialogResult.Yes)
            {
                List<Komercijalno.Roba> list = Komercijalno.Roba.List();

                using(MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using(MySqlCommand cmd = new MySqlCommand("UPDATE PRODUCT SET CATALOGUE = @KBR WHERE ID = @RID", con))
                    {
                        cmd.Parameters.Add("@KBR", MySqlDbType.VarChar);
                        cmd.Parameters.Add("@RID", MySqlDbType.Int32);

                        foreach(Komercijalno.Roba r in list)
                        {
                            cmd.Parameters["@KBR"].Value = r.KatBr.Substring(0, 4);
                            cmd.Parameters["@RID"].Value = r.ROBAID;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            MessageBox.Show("Gotovo!");
        }


        private void povuciSaSajtaUTDOfficeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Da li sigurno zelite da izvrsite ovu akciju?", "Potvrdi", MessageBoxButtons.YesNo);

            if(dr == DialogResult.Yes)
            {
                MessageBox.Show("Sacekajte poruku potvrde!");
                try
                {
                    List<Komercijalno.Roba> robaKom = Komercijalno.Roba.List();

                    List<int> pl = new List<int>(); // Robaid unutar web baze
                    List<int> pl1 = new List<int>(); // Robaid unutar tdoffice

                    MessageBox.Show("Part 1 done");

                    using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("SELECT ID FROM PRODUCT", con))
                        using (MySqlDataReader dr1 = cmd.ExecuteReader())
                            while (dr1.Read())
                            {
                                try
                                {
                                    pl.Add(Convert.ToInt32(dr1[0]));
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                    }
                    MessageBox.Show("Part 2 done");

                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT ROBAID FROM WEB_CENA", con))
                        using (FbDataReader dr2 = cmd.ExecuteReader())
                            while (dr2.Read())
                            {
                                try
                                {
                                    pl1.Add(Convert.ToInt32(dr2[0]));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                    }
                    MessageBox.Show("Part 3 done");

                    for (int i = pl1.Count() - 1; i >= 0; i--)
                    {
                        int y = pl1[i];
                        if (pl.Contains(y))
                            pl.Remove(y);
                    }

                    MessageBox.Show("Part 4 done");

                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                    {
                        con.Open();
                        using(FbCommand cmd = new FbCommand("INSERT INTO WEB_CENA (WCID, ROBAID, ZADOKMAGID, GRUPAID, PROID, KRITERIJUM, CENAIZMAGID, WEB_CENA_BP, PROCENAT, ROKPLACANJA, DATOD, DATDO, POCSTANJE, PLANIRANIRABAT," +
                            "STANDARDNAVPCENA, NAZIV, KATBR, KATBRPRO, POSLEDNJANABAVNACENA, PLANIRANAMARZA, NIV, REF_ROBAID, NAUPIT, PONDERPROCENAT, PONDERRABAT) VALUES (" +
                            "(SELECT COALESCE(MAX(WCID), 0) FROM WEB_CENA) + 1, @RID, 0, 0, 0, 1, 50, 0, 0, 0, @DATOD, @DATDO, 0, 0, 0, @NAZ, @KATBR, @KATBRPRO, 0, 0, 0, 0, NULL, 33, 0)", con))
                        {
                            cmd.Parameters.AddWithValue("@DATOD", DateTime.Now);
                            cmd.Parameters.AddWithValue("@DATDO", DateTime.Now.AddMonths(-6));
                            cmd.Parameters.Add("@RID", FbDbType.Integer);
                            cmd.Parameters.Add("@NAZ", FbDbType.VarChar);
                            cmd.Parameters.Add("@KATBR", FbDbType.VarChar);
                            cmd.Parameters.Add("@KATBRPRO", FbDbType.VarChar);

                            foreach(int a in pl)
                            {
                                try
                                {
                                    Komercijalno.Roba r = robaKom.Where(x => x.ROBAID == a).FirstOrDefault();
                                    cmd.Parameters["@RID"].Value = a;
                                    cmd.Parameters["@NAZ"].Value = r == null ? "UNDEFINED" : r.Naziv;
                                    cmd.Parameters["@KATBR"].Value = r == null ? "UNDEFINED" : r.KatBr;
                                    cmd.Parameters["@KATBRPRO"].Value = "UNDEFINED";

                                    cmd.ExecuteNonQuery();
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }

                    MessageBox.Show("Gotovo!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
