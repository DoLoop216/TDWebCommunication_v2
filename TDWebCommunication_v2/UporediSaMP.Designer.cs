namespace TDWebCommunication_v2
{
    partial class UporediSaMP
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
            this.webId_txt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.uporedi_btn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.komercijalno_dgv = new System.Windows.Forms.DataGridView();
            this.web_dgv = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.vrednostWeb_lbl = new System.Windows.Forms.Label();
            this.vrednostKomercijalno_lbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.komercijalno_dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.web_dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // webId_txt
            // 
            this.webId_txt.Location = new System.Drawing.Point(62, 12);
            this.webId_txt.Name = "webId_txt";
            this.webId_txt.Size = new System.Drawing.Size(162, 20);
            this.webId_txt.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Web id:";
            // 
            // uporedi_btn
            // 
            this.uporedi_btn.Location = new System.Drawing.Point(230, 9);
            this.uporedi_btn.Name = "uporedi_btn";
            this.uporedi_btn.Size = new System.Drawing.Size(75, 23);
            this.uporedi_btn.TabIndex = 2;
            this.uporedi_btn.Text = "Uporedi";
            this.uporedi_btn.UseVisualStyleBackColor = true;
            this.uporedi_btn.Click += new System.EventHandler(this.uporedi_btn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(15, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.vrednostWeb_lbl);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.web_dgv);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.vrednostKomercijalno_lbl);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.komercijalno_dgv);
            this.splitContainer1.Size = new System.Drawing.Size(773, 400);
            this.splitContainer1.SplitterDistance = 386;
            this.splitContainer1.TabIndex = 3;
            // 
            // komercijalno_dgv
            // 
            this.komercijalno_dgv.AllowUserToAddRows = false;
            this.komercijalno_dgv.AllowUserToDeleteRows = false;
            this.komercijalno_dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.komercijalno_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.komercijalno_dgv.Location = new System.Drawing.Point(3, 36);
            this.komercijalno_dgv.Name = "komercijalno_dgv";
            this.komercijalno_dgv.ReadOnly = true;
            this.komercijalno_dgv.Size = new System.Drawing.Size(377, 274);
            this.komercijalno_dgv.TabIndex = 1;
            // 
            // web_dgv
            // 
            this.web_dgv.AllowUserToAddRows = false;
            this.web_dgv.AllowUserToDeleteRows = false;
            this.web_dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.web_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.web_dgv.Location = new System.Drawing.Point(4, 36);
            this.web_dgv.Name = "web_dgv";
            this.web_dgv.ReadOnly = true;
            this.web_dgv.Size = new System.Drawing.Size(377, 274);
            this.web_dgv.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "WEB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "KOMERCIJALNO";
            // 
            // vrednostWeb_lbl
            // 
            this.vrednostWeb_lbl.AutoSize = true;
            this.vrednostWeb_lbl.Location = new System.Drawing.Point(12, 313);
            this.vrednostWeb_lbl.Name = "vrednostWeb_lbl";
            this.vrednostWeb_lbl.Size = new System.Drawing.Size(95, 13);
            this.vrednostWeb_lbl.TabIndex = 4;
            this.vrednostWeb_lbl.Text = "Ukupna vrednost: ";
            // 
            // vrednostKomercijalno_lbl
            // 
            this.vrednostKomercijalno_lbl.AutoSize = true;
            this.vrednostKomercijalno_lbl.Location = new System.Drawing.Point(8, 313);
            this.vrednostKomercijalno_lbl.Name = "vrednostKomercijalno_lbl";
            this.vrednostKomercijalno_lbl.Size = new System.Drawing.Size(95, 13);
            this.vrednostKomercijalno_lbl.TabIndex = 5;
            this.vrednostKomercijalno_lbl.Text = "Ukupna vrednost: ";
            // 
            // UporediSaMP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.uporedi_btn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.webId_txt);
            this.Name = "UporediSaMP";
            this.Text = "Uporedi porudzbinu sa mp racunom";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.komercijalno_dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.web_dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox webId_txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button uporedi_btn;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label vrednostWeb_lbl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView web_dgv;
        private System.Windows.Forms.Label vrednostKomercijalno_lbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView komercijalno_dgv;
    }
}