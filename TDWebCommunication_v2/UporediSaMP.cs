using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDWebCommunication_v2
{
    public partial class UporediSaMP : Form
    {
        public UporediSaMP()
        {
            InitializeComponent();
        }

        private void uporedi_btn_Click(object sender, EventArgs e)
        {
            komercijalno_dgv.DataSource = null;
            web_dgv.DataSource = null;

            int webID;
            try
            {
                webID = Convert.ToInt32(webId_txt.Text);
            }
            catch(Exception)
            {
                MessageBox.Show("Neispravan id!");
                return;
            }

            Web.Porudzbina p = new Web.Porudzbina(webID);

            p.UcitajStavke();

            if(p.BrDokKom <= 0)
            {
                MessageBox.Show("Ova porudzbina nije pretvorena u proracun");
                return;
                // 3461778
            }

            Komercijalno.Dokument d = new Komercijalno.Dokument(32, p.BrDokKom);
            if(d.BrDokOUT == null)
            {
                MessageBox.Show("Porudzbina je pretvorena u proracun ali ne i u MP racun");
                return;
            }

            label3.Text = "KOMERCIJALNO: " + d.BrDokOUT;

            DataTable t1 = new DataTable();
            t1.Columns.Add("RobaID");
            t1.Columns.Add("Naziv");
            t1.Columns.Add("Cena");
            t1.Columns.Add("Kolicina");
            t1.Columns.Add("Vrednost");

            foreach (Web.Porudzbina.Item i in p.Items)
            {
                DataRow dr = t1.NewRow();
                dr["RobaID"] = i.RobaID;
                dr["Naziv"] = i.NazivRobe;
                dr["Cena"] = i.VpCena;
                dr["Kolicina"] = i.Kolicina;
                dr["Vrednost"] = i.Kolicina * i.VpCena;
                t1.Rows.Add(dr);
            }

            DataTable t2 = t1.Clone();
            List<Komercijalno.Stavka> ls = Komercijalno.Stavka.List(15, (int)d.BrDokOUT);
            MessageBox.Show("Komercijalno stavki: " + ls.Count());
            foreach (Komercijalno.Stavka s in ls)
            {
                DataRow dr = t2.NewRow();
                dr["RobaID"] = s.RobaID;
                dr["Naziv"] = s.Naziv;
                dr["Cena"] = s.ProdCenaBP;
                dr["Kolicina"] = s.Kolicina;
                dr["Vrednost"] = s.Kolicina * s.ProdCenaBP;
                t2.Rows.Add(dr);
            }


            web_dgv.DataSource = t1;
            komercijalno_dgv.DataSource = t2;
        }
    }
}
