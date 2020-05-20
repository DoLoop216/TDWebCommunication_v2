﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AR;

namespace TDWebCommunication_v2
{
    public partial class AnalizaCenaPremaNabavnim : Form
    {
        private static int TrenutniSlucaj = -1;

        public AnalizaCenaPremaNabavnim()
        {
            InitializeComponent();
        }

        private void AnalizaCenaPremaNabavnim_Load(object sender, EventArgs e)
        {
            List<AR.WebShop.Cena> PlatinumCene = AR.TDShop.Roba.GetVPCene(3);
            List<AR.WebShop.Cena> IronCene = AR.TDShop.Roba.GetVPCene(0);

            DataTable dt1 = AR.TDShop.Roba.List().ToDataTable();
            dt1.Columns.Add("PLATINUM", typeof(double));
            dt1.Columns.Add("IRON", typeof(double));

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                int rid = (int)dt1.Rows[i]["ROBAID"];

                dt1.Rows[i]["PLATINUM"] = PlatinumCene.Where(x => x.RobaID == rid).FirstOrDefault().VPCena;
                dt1.Rows[i]["IRON"] = IronCene.Where(x => x.RobaID == rid).FirstOrDefault().VPCena;
            }


            DataTable dt2 = TDOffice.WebCena.List().ToDataTable();

            DataColumn dc = dt1.Columns["ROBAID"];
            DataColumn dc1 = dt2.Columns["ROBAID"];
            dt1.PrimaryKey = new DataColumn[] { dc };
            dt2.PrimaryKey = new DataColumn[] { dc1 };

            dt1.Merge(TDOffice.WebCena.List().ToDataTable());

            dataGridView1.DataSource = dt1;

            dataGridView1.Columns["RobaID"].DisplayIndex = 0;
            dataGridView1.Columns["KatBr"].DisplayIndex = 1;

            //Ovde dolazi platinum cena

            dataGridView1.Columns["PLATINUM"].DefaultCellStyle.Format = "#,##0.00  RSD";
            dataGridView1.Columns["IRON"].DefaultCellStyle.Format = "#,##0.00  RSD";
            dataGridView1.Columns["PoslednjaNabavnaCena"].DefaultCellStyle.Format = "#,##0.00  RSD";
            dataGridView1.Columns["StandardnaVPCena"].DefaultCellStyle.Format = "#,##0.00  RSD";

            dataGridView1.Columns["PLATINUM"].DefaultCellStyle.BackColor = Color.FromArgb(255, 91, 91);
            dataGridView1.Columns["IRON"].DefaultCellStyle.BackColor = Color.FromArgb(255, 91, 91);
            dataGridView1.Columns["PoslednjaNabavnaCena"].DefaultCellStyle.BackColor = Color.FromArgb(3, 169, 244);
            dataGridView1.Columns["StandardnaVPCena"].DefaultCellStyle.BackColor = Color.FromArgb(3, 169, 244);

            dataGridView1.Columns["PoslednjaNabavnaCena"].DisplayIndex = 3;
            dataGridView1.Columns["StandardnaVPCena"].DisplayIndex = 4;

            DefaultDGVSetup();


            dataGridView1.Columns["Naziv"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            dataGridView1.Sort(dataGridView1.Columns["NAZIV"], ListSortDirection.Ascending);
            pretraga_cmb.SelectedIndex = 1;

            toolStripStatusLabel1.Text = "Slogova: " + dataGridView1.Rows.Count;
        }

        private void pretraga_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void pretraga_txt_KeyUp(object sender, KeyEventArgs e)
        {
            string val = pretraga_cmb.SelectedItem.ToString();
            if (pretraga_txt.Text == null || pretraga_txt.Text.Length < 1)
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Convert({0}, System.String) LIKE '%{1}%'", val, "");

            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Convert({0}, System.String) LIKE '%{1}%'", val, pretraga_txt.Text);

            toolStripStatusLabel1.Text = "Slogova: " + dataGridView1.Rows.Count;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    Slucaj1();
                    break;
                default:
                    MessageBox.Show("Greska...");
                    break;
            }
            toolStripStatusLabel1.Text = "Slogova: " + dataGridView1.Rows.Count;
        }

        private void Slucaj1()
        {
            TrenutniSlucaj = 0;
            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            cm.SuspendBinding();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((double)row.Cells["PLATINUM"].Value < (double)row.Cells["PoslednjaNabavnaCena"].Value)
                    row.Visible = true;
                else
                    row.Visible = false;
            }

            if (checkBox1.Checked)
            {
                dataGridView1.Columns["StandardnaVPCena"].Visible = false;
                dataGridView1.Columns["IRON"].Visible = false;
            }
            else
            {
                dataGridView1.Columns["StandardnaVPCena"].Visible = true;
                dataGridView1.Columns["IRON"].Visible = true;
            }


                cm.SuspendBinding();
        }

        private void DefaultDGVSetup()
        {
            dataGridView1.Columns["Slika"].Visible = false;
            dataGridView1.Columns["PodgrupaID"].Visible = false;
            dataGridView1.Columns["Redirect"].Visible = false;
            dataGridView1.Columns["Klasa"].Visible = false;
            dataGridView1.Columns["JM"].Visible = false;
            dataGridView1.Columns["CenovnikGrupaID"].Visible = false;
            dataGridView1.Columns["Aktivan"].Visible = false;
            dataGridView1.Columns["TransportnoPakovanje"].Visible = false;
            dataGridView1.Columns["New"].Visible = false;
            dataGridView1.Columns["DisplayIndex"].Visible = false;
            dataGridView1.Columns["KratakOpis"].Visible = false;
            dataGridView1.Columns["TransportnoPakovanjeJM"].Visible = false;
            dataGridView1.Columns["KupovinaSamoUTransportnomPakovanju"].Visible = false;
            dataGridView1.Columns["PDV"].Visible = false;
            dataGridView1.Columns["keywords"].Visible = false;
            dataGridView1.Columns["VISITS"].Visible = false;
            dataGridView1.Columns["Akcija"].Visible = false;

            dataGridView1.Columns["PLATINUM"].Visible = true;
            dataGridView1.Columns["IRON"].Visible = true;
            dataGridView1.Columns["StandardnaVPCena"].Visible = true;
            dataGridView1.Columns["PoslednjaNabavnaCena"].Visible = true;
            dataGridView1.Columns["Naziv"].Visible = true;
            dataGridView1.Columns["KatBr"].Visible = true;
            dataGridView1.Columns["RobaID"].Visible = true;

            dataGridView1.Columns["WCID"].Visible = false;
            dataGridView1.Columns["ZaDokMagID"].Visible = false;
            dataGridView1.Columns["Kriterijum"].Visible = false;
            dataGridView1.Columns["CenIzMagID"].Visible = false;
            dataGridView1.Columns["WebCenaBP"].Visible = false;
            dataGridView1.Columns["Procenat"].Visible = false;
            dataGridView1.Columns["RokPlacanja"].Visible = false;
            dataGridView1.Columns["DatOD"].Visible = false;
            dataGridView1.Columns["DatDO"].Visible = false;
            dataGridView1.Columns["PocStanje"].Visible = false;
            dataGridView1.Columns["PlaniraniRabat"].Visible = false;
            dataGridView1.Columns["PlaniranaMarza"].Visible = false;
            dataGridView1.Columns["Niv"].Visible = false;
            dataGridView1.Columns["RefRobaID"].Visible = false;
            dataGridView1.Columns["NaUpit"].Visible = false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            DefaultDGVSetup();

            if ((sender as CheckBox).Checked)
            {
                switch (TrenutniSlucaj)
                {
                    case 0:
                        dataGridView1.Columns["StandardnaVPCena"].Visible = false;
                        dataGridView1.Columns["IRON"].Visible = false;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
