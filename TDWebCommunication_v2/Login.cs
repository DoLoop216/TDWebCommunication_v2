using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TDWebCommunication_v2
{
    public partial class Login : Form
    {
        public Login()
        {
            Buffer.ConnectionStrings = ConnectionStrings.Load();
            AR.AR.ConnectionString = Buffer.ConnectionStrings.Web;
            InitializeComponent();

            try
            {
                if (File.Exists("c:\\kmajcnasl") && File.ReadAllText("c:\\kmajcnasl") == "LogWithoutPassword")
                {
                    Web.Config.Update("COMMUNICATOR_RUNNING", "1");

                    Program.isServer = true;
                    Debug.Form = new Debug();
                    Debug.Form.Text = "WEB Konzola - Server";
                    this.Hide();
                    Debug.Log("Server je uspesno pokrenut!");

                    Thread t1 = new Thread(Program.Communicate);
                    t1.IsBackground = true;
                    t1.Start();

                    Thread t2 = new Thread(Program.Communicate_DocumentValidation);
                    t2.IsBackground = true;
                    //t2.Start();

                    Debug.Form.ShowDialog();
                }
                else
                {
                    File.Create("c:\\kmajcnasl");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Neispravno uneti podaci!");
                return;
            }

            if(username == "TDSERVER" && password == "MALOM")
            {
                if (Web.Config.Get("COMMUNICATOR_RUNNING").Value == "1")
                {
                    MessageBox.Show("Server je vec pokrenut negde!");
                    DialogResult dr = MessageBox.Show("Da li zelite na silu da pokrenete server?", "Potvrdi", MessageBoxButtons.YesNo);
                    if(dr != DialogResult.Yes)
                        return;
                }

                Web.Config.Update("COMMUNICATOR_RUNNING", "1");

                Program.isServer = true;
                Debug.Form = new Debug();
                Debug.Form.Text = "WEB Konzola - Server";
                this.Hide();
                Debug.Log("Server je uspesno pokrenut!");

                Thread t1 = new Thread(Program.Communicate);
                t1.IsBackground = true;
                t1.Start();

                Thread t2 = new Thread(Program.Communicate_DocumentValidation);
                t2.IsBackground = true;
                //t2.Start();

                Debug.Form.ShowDialog();

                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT PW FROM USER WHERE NAME = @U", con))
                    {
                        cmd.Parameters.AddWithValue("@U", username);

                        MySqlDataReader dr = cmd.ExecuteReader();

                        if(dr.Read())
                        {
                            string p = Web.User.Hash(Web.User.Hash(Web.User.Hash(Web.User.Hash(Web.User.Hash(Web.User.Hash(password))))));

                            if (dr[0].ToString() == p)
                            {
                                Debug.Form = new Debug();
                                Form1 f = new Form1();
                                Debug.Form.Show();
                                Debug.Log("Uspesno ste se ulogovali!");
                                f.Show();
                                this.Hide();

                                Thread t1 = new Thread(Program.Communicate);
                                t1.IsBackground = true;
                                t1.Start();
                            }
                            else
                                MessageBox.Show("Pogresna lozinka!");
                        }
                        else
                        {
                            MessageBox.Show("Korisnik ne postoji!");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Web.Config.Update("COMMUNICATOR_RUNNING", "0");
        }
    }
}
