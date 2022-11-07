namespace StockToolKit.Analyze
{
    partial class formFuncs
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
            this.labelModelManage = new System.Windows.Forms.Label();
            this.labelTradeManage = new System.Windows.Forms.Label();
            this.labelAnalyze = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelModelManage
            // 
            this.labelModelManage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelModelManage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelModelManage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelModelManage.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelModelManage.Location = new System.Drawing.Point(36, 74);
            this.labelModelManage.Name = "labelModelManage";
            this.labelModelManage.Padding = new System.Windows.Forms.Padding(20, 20, 0, 0);
            this.labelModelManage.Size = new System.Drawing.Size(253, 64);
            this.labelModelManage.TabIndex = 0;
            this.labelModelManage.Text = "Check Update";
            this.labelModelManage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelModelManage.Click += new System.EventHandler(this.labelModelManage_Click);
            // 
            // labelTradeManage
            // 
            this.labelTradeManage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTradeManage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelTradeManage.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTradeManage.Location = new System.Drawing.Point(66, 281);
            this.labelTradeManage.Name = "labelTradeManage";
            this.labelTradeManage.Padding = new System.Windows.Forms.Padding(20, 20, 0, 0);
            this.labelTradeManage.Size = new System.Drawing.Size(188, 64);
            this.labelTradeManage.TabIndex = 2;
            this.labelTradeManage.Text = "DataTrans";
            this.labelTradeManage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelTradeManage.Click += new System.EventHandler(this.labelTradeManage_Click);
            // 
            // labelAnalyze
            // 
            this.labelAnalyze.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelAnalyze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelAnalyze.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelAnalyze.Location = new System.Drawing.Point(36, 180);
            this.labelAnalyze.Name = "labelAnalyze";
            this.labelAnalyze.Padding = new System.Windows.Forms.Padding(45, 20, 0, 0);
            this.labelAnalyze.Size = new System.Drawing.Size(253, 63);
            this.labelAnalyze.TabIndex = 1;
            this.labelAnalyze.Text = "Get StockList";
            this.labelAnalyze.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelAnalyze.Click += new System.EventHandler(this.labelAnalyze_Click);
            // 
            // formFuncs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 451);
            this.Controls.Add(this.labelTradeManage);
            this.Controls.Add(this.labelAnalyze);
            this.Controls.Add(this.labelModelManage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 490);
            this.Name = "formFuncs";
            this.Text = "formFuncs";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelModelManage;
        private System.Windows.Forms.Label labelTradeManage;
        private System.Windows.Forms.Label labelAnalyze;
    }
}