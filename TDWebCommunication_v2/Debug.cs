using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TDWebCommunication_v2
{
    public partial class Debug : Form
    {
        public static Debug Form;

        public Debug()
        {
            InitializeComponent();
        }

        public void _Log(string Message)
        {
            if (!Form.IsHandleCreated)
                return;
            Form.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(Message + Environment.NewLine);
            }));
        }

        public static void Log(string Message)
        {
            if(Form != null)
                Form._Log(Message);
        }

        private void Debug_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.IsRunning = false;
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }
    }
}
