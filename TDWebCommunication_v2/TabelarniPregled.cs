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
    public partial class TabelarniPregled : Form
    {
        public TabelarniPregled()
        {
            InitializeComponent();
        }

        private void TabelarniPregled_Load(object sender, EventArgs e)
        {
            List<AR.TDShop.Porudzbina> list = AR.TDShop.Porudzbina.List();
            list.Sort((x, y) => y.PorudzbinaID.CompareTo(x.PorudzbinaID));
            dataGridView1.DataSource = list;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            using (Porudzbina p = new Porudzbina(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["PorudzbinaID"].Value)))
            {
                p.ShowDialog();
            }
        }
    }
}
