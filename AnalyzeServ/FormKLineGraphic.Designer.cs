namespace StockToolKit.Analyze
{
    partial class FormKLineGraphic
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
            this.lblMove = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.lblkinfo = new System.Windows.Forms.Label();
            this.lblvinfo = new System.Windows.Forms.Label();
            this.cbkbgcolor = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblMove
            // 
            this.lblMove.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lblMove.ForeColor = System.Drawing.Color.White;
            this.lblMove.Location = new System.Drawing.Point(4, 3);
            this.lblMove.Name = "lblMove";
            this.lblMove.Size = new System.Drawing.Size(1032, 14);
            this.lblMove.TabIndex = 2;
            this.lblMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseDown);
            this.lblMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseMove);
            this.lblMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseUp);
            // 
            // lblClose
            // 
            this.lblClose.ForeColor = System.Drawing.Color.White;
            this.lblClose.Location = new System.Drawing.Point(1042, 4);
            this.lblClose.Name = "lblClose";
            this.lblClose.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblClose.Size = new System.Drawing.Size(16, 12);
            this.lblClose.TabIndex = 3;
            this.lblClose.Text = "X";
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            // 
            // lblkinfo
            // 
            this.lblkinfo.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lblkinfo.ForeColor = System.Drawing.Color.Snow;
            this.lblkinfo.Location = new System.Drawing.Point(12, 29);
            this.lblkinfo.Name = "lblkinfo";
            this.lblkinfo.Size = new System.Drawing.Size(85, 240);
            this.lblkinfo.TabIndex = 4;
            this.lblkinfo.Text = "label1";
            this.lblkinfo.Paint += new System.Windows.Forms.PaintEventHandler(this.label1_Paint);
            this.lblkinfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblkinfo_MouseDown);
            this.lblkinfo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblkinfo_MouseMove);
            this.lblkinfo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblkinfo_MouseUp);
            // 
            // lblvinfo
            // 
            this.lblvinfo.ForeColor = System.Drawing.Color.Snow;
            this.lblvinfo.Location = new System.Drawing.Point(33, 397);
            this.lblvinfo.Name = "lblvinfo";
            this.lblvinfo.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.lblvinfo.Size = new System.Drawing.Size(650, 23);
            this.lblvinfo.TabIndex = 5;
            this.lblvinfo.Text = "lblvinfo";
            // 
            // cbkbgcolor
            // 
            this.cbkbgcolor.AutoSize = true;
            this.cbkbgcolor.Checked = true;
            this.cbkbgcolor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbkbgcolor.ForeColor = System.Drawing.Color.White;
            this.cbkbgcolor.Location = new System.Drawing.Point(958, 20);
            this.cbkbgcolor.Name = "cbkbgcolor";
            this.cbkbgcolor.Size = new System.Drawing.Size(78, 16);
            this.cbkbgcolor.TabIndex = 6;
            this.cbkbgcolor.Text = "k线柱背景";
            this.cbkbgcolor.UseVisualStyleBackColor = true;
            this.cbkbgcolor.CheckedChanged += new System.EventHandler(this.cbkbgcolor_CheckedChanged);
            // 
            // FormKLineGraphic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1060, 772);
            this.ControlBox = false;
            this.Controls.Add(this.cbkbgcolor);
            this.Controls.Add(this.lblkinfo);
            this.Controls.Add(this.lblvinfo);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.lblMove);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormKLineGraphic";
            this.ShowIcon = false;
            this.Text = "FormKLineGraphic";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormKLineGraphic_FormClosed);
            this.Shown += new System.EventHandler(this.FormKLineGraphic_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormKLineGraphic_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormKLineGraphic_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormKLineGraphic_MouseDown);
            this.MouseEnter += new System.EventHandler(this.FormKLineGraphic_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.FormKLineGraphic_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormKLineGraphic_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMove;
        private System.Windows.Forms.Label lblClose;
        private System.Windows.Forms.Label lblkinfo;
        private System.Windows.Forms.Label lblvinfo;
        private System.Windows.Forms.CheckBox cbkbgcolor;
    }
}