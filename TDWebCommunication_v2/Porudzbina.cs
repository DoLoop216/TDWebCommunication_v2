using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using AR;

namespace TDWebCommunication_v2
{
    public partial class Porudzbina : Form
    {
        Web.Porudzbina por;

        public Porudzbina(int ID)
        {
            InitializeComponent();

            por = new Web.Porudzbina(ID);
            por.UcitajStavke();

            this.id_txt.Text = por.PorudzbinaID.ToString();
            this.korisnik_txt.Text = por.UserID.ToString();
            this.datum_txt.Text = por.Datum.ToShortDateString();
            this.magacin_cmb.DataSource = Komercijalno.Magacin.List();
            this.magacin_cmb.ValueMember = "MagacinID";
            this.magacin_cmb.DisplayMember = "Naziv";
            this.magacin_cmb.SelectedValue =  por.MagacinID;
            this.proracun_txt.Text = por.BrDokKom.ToString();
            this.nupl_cmb.SelectedIndex = 0;
            if (por.PPID != null && por.PPID > 1)
                this.partner_txt.Text = Komercijalno.Partner.GetNaziv((int)por.PPID);
            else
                this.partner_txt.Text = "Fizicko lice";

            dataGridView1.DataSource = por.Items;

            dataGridView1.Columns["PorudzbinaItemID"].Visible = false;
            dataGridView1.Columns["ROBAID"].Visible = false;
            dataGridView1.Columns["PorudzbinaID"].Visible = false;

            if (por.Status != Web.Porudzbina.status.ObradadjujeSe)
            {
                button2.Enabled = false;
                panel2.BackColor = Color.Red;
                button1.Enabled = false;
                magacin_cmb.Enabled = false;
                nupl_cmb.Enabled = false;
            }
            
            if(por.Status == Web.Porudzbina.status.ObradadjujeSe)
            {
                panel2.BackColor = Color.Green;
                button2.Enabled = true;
                button1.Enabled = true;
                magacin_cmb.Enabled = true;
                nupl_cmb.Enabled = true;
            }

            if (por.BrDokKom > 0)
            {
                button1.Enabled = false;
                magacin_cmb.Enabled = false;
                nupl_cmb.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Porudzbina je prazna!");
                return;
            }
            try
            {
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    int noviBrDok;
                    con.Open();
                    using (FbCommand cmd = new FbCommand("NAPRAVIDOKUMENT", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("VRDOK", 32);
                        cmd.Parameters.AddWithValue("BR_NAR", "Web: " + por.PorudzbinaID.ToString());
                        cmd.Parameters.AddWithValue("PPID", (por.PPID != null && por.PPID > 1) ? por.PPID : -1);
                        cmd.Parameters.AddWithValue("NAPOMENA", "Neka napomena");
                        cmd.Parameters.AddWithValue("NACUPLID", 5);
                        cmd.Parameters.AddWithValue("MAGACINID", this.magacin_cmb.SelectedValue);

                        cmd.Parameters.Add("BRDOK", FbDbType.Integer).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteScalar();

                        noviBrDok = (int)cmd.Parameters["BRDOK"].Value;
                        
                        por.PoveziSaDokumentom(noviBrDok);

                        if ((int)this.magacin_cmb.SelectedValue != por.MagacinID)
                            por.AzurirajMagacinID((int)magacin_cmb.SelectedValue);
                    }

                    using (FbCommand cmd = new FbCommand("NAPRAVISTAVKU", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("VRDOK", 32);
                        cmd.Parameters.AddWithValue("BRDOK", noviBrDok);
                        cmd.Parameters.Add("ROBAID", FbDbType.Integer);
                        cmd.Parameters.Add("CENA_BEZ_PDV", FbDbType.Double);
                        cmd.Parameters.Add("KOL", FbDbType.Double);
                        cmd.Parameters.AddWithValue("RABAT", 0);

                        foreach (Web.Porudzbina.Item i in por.Items)
                        {
                            cmd.Parameters["ROBAID"].Value = i.RobaID;
                            cmd.Parameters["CENA_BEZ_PDV"].Value = i.VpCena * (1.2);
                            cmd.Parameters["KOL"].Value = i.Kolicina;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                proracun_txt.Text = por.BrDokKom.ToString();
                MessageBox.Show("Porudzbina je uspesno pretvorena u proracun!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            por.AzurirajStatus(Web.Porudzbina.status.CekaUplatu);
        }
    }
}
