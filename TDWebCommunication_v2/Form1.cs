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
                    using(MySqlCommand cmd = new MySqlCommand("UPDATE ROBA SET NABAVNACENA = @NC WHERE ROBAID = @RID", con))
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
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE ROBA SET PRODAJNACENA = @PC WHERE ROBAID = @RID", con))
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
                    using(MySqlCommand cmd = new MySqlCommand("UPDATE ROBA SET KATBR = @KBR WHERE ROBAID = @RID", con))
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
    }
}
