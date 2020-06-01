namespace TDWebCommunication_v2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.azuriranjeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nabavnaCenaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ironCenastandardnaMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analizaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.premaNabavnimCenamaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.porudzbineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uporediSaMPRacunimaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proveraCsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.proizvodiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.azurirajKataloskeBrojeveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ceneToolStripMenuItem,
            this.porudzbineToolStripMenuItem,
            this.proveraCsToolStripMenuItem,
            this.proizvodiToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(647, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ceneToolStripMenuItem
            // 
            this.ceneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.azuriranjeToolStripMenuItem,
            this.analizaToolStripMenuItem});
            this.ceneToolStripMenuItem.Name = "ceneToolStripMenuItem";
            this.ceneToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.ceneToolStripMenuItem.Text = "Cene";
            // 
            // azuriranjeToolStripMenuItem
            // 
            this.azuriranjeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nabavnaCenaToolStripMenuItem,
            this.ironCenastandardnaMPToolStripMenuItem});
            this.azuriranjeToolStripMenuItem.Name = "azuriranjeToolStripMenuItem";
            this.azuriranjeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.azuriranjeToolStripMenuItem.Text = "Azuriranje";
            // 
            // nabavnaCenaToolStripMenuItem
            // 
            this.nabavnaCenaToolStripMenuItem.Name = "nabavnaCenaToolStripMenuItem";
            this.nabavnaCenaToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.nabavnaCenaToolStripMenuItem.Text = "Nabavna Cena";
            this.nabavnaCenaToolStripMenuItem.Click += new System.EventHandler(this.nabavnaCenaToolStripMenuItem_Click);
            // 
            // ironCenastandardnaMPToolStripMenuItem
            // 
            this.ironCenastandardnaMPToolStripMenuItem.Name = "ironCenastandardnaMPToolStripMenuItem";
            this.ironCenastandardnaMPToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.ironCenastandardnaMPToolStripMenuItem.Text = "Iron Cena (standardna MP)";
            this.ironCenastandardnaMPToolStripMenuItem.Click += new System.EventHandler(this.ironCenastandardnaMPToolStripMenuItem_Click);
            // 
            // analizaToolStripMenuItem
            // 
            this.analizaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.premaNabavnimCenamaToolStripMenuItem});
            this.analizaToolStripMenuItem.Name = "analizaToolStripMenuItem";
            this.analizaToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.analizaToolStripMenuItem.Text = "Analiza";
            // 
            // premaNabavnimCenamaToolStripMenuItem
            // 
            this.premaNabavnimCenamaToolStripMenuItem.Name = "premaNabavnimCenamaToolStripMenuItem";
            this.premaNabavnimCenamaToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.premaNabavnimCenamaToolStripMenuItem.Text = "Web cena";
            this.premaNabavnimCenamaToolStripMenuItem.Click += new System.EventHandler(this.premaNabavnimCenamaToolStripMenuItem_Click);
            // 
            // porudzbineToolStripMenuItem
            // 
            this.porudzbineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uporediSaMPRacunimaToolStripMenuItem});
            this.porudzbineToolStripMenuItem.Name = "porudzbineToolStripMenuItem";
            this.porudzbineToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.porudzbineToolStripMenuItem.Text = "Porudzbine";
            // 
            // uporediSaMPRacunimaToolStripMenuItem
            // 
            this.uporediSaMPRacunimaToolStripMenuItem.Name = "uporediSaMPRacunimaToolStripMenuItem";
            this.uporediSaMPRacunimaToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.uporediSaMPRacunimaToolStripMenuItem.Text = "Uporedi sa MP Racunima";
            this.uporediSaMPRacunimaToolStripMenuItem.Click += new System.EventHandler(this.uporediSaMPRacunimaToolStripMenuItem_Click);
            // 
            // proveraCsToolStripMenuItem
            // 
            this.proveraCsToolStripMenuItem.Name = "proveraCsToolStripMenuItem";
            this.proveraCsToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.proveraCsToolStripMenuItem.Text = "Provera cs";
            this.proveraCsToolStripMenuItem.Click += new System.EventHandler(this.proveraCsToolStripMenuItem_Click);
            // 
            // proizvodiToolStripMenuItem
            // 
            this.proizvodiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.azurirajKataloskeBrojeveToolStripMenuItem});
            this.proizvodiToolStripMenuItem.Name = "proizvodiToolStripMenuItem";
            this.proizvodiToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.proizvodiToolStripMenuItem.Text = "Proizvodi";
            // 
            // azurirajKataloskeBrojeveToolStripMenuItem
            // 
            this.azurirajKataloskeBrojeveToolStripMenuItem.Name = "azurirajKataloskeBrojeveToolStripMenuItem";
            this.azurirajKataloskeBrojeveToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.azurirajKataloskeBrojeveToolStripMenuItem.Text = "Azuriraj kataloske brojeve";
            this.azurirajKataloskeBrojeveToolStripMenuItem.Click += new System.EventHandler(this.azurirajKataloskeBrojeveToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 285);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "WEB";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem azuriranjeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analizaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem premaNabavnimCenamaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nabavnaCenaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ironCenastandardnaMPToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem porudzbineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uporediSaMPRacunimaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem proveraCsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem proizvodiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem azurirajKataloskeBrojeveToolStripMenuItem;
    }
}

