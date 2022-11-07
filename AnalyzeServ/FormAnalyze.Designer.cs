namespace StockToolKit.Analyze
{
    partial class FormAnalyze
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tcAnalyze = new System.Windows.Forms.TabControl();
            this.tpAnalyze = new System.Windows.Forms.TabPage();
            this.lblDtToRight = new System.Windows.Forms.Label();
            this.cbOneStockCode = new System.Windows.Forms.CheckBox();
            this.tbStockCode = new System.Windows.Forms.TextBox();
            this.lbStockCode = new System.Windows.Forms.Label();
            this.rbtdxfq = new System.Windows.Forms.RadioButton();
            this.rbD1File = new System.Windows.Forms.RadioButton();
            this.cbToStockCodeListFile = new System.Windows.Forms.CheckBox();
            this.cbAnalyzeInTradding = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblARListFile = new System.Windows.Forms.Label();
            this.cbUseARListFile = new System.Windows.Forms.CheckBox();
            this.lblShowOpenFileDiag = new System.Windows.Forms.Label();
            this.lblDtToleft = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.btnanalyze = new System.Windows.Forms.Button();
            this.tcAnalyze.SuspendLayout();
            this.tpAnalyze.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcAnalyze
            // 
            this.tcAnalyze.Controls.Add(this.tpAnalyze);
            this.tcAnalyze.Location = new System.Drawing.Point(-3, 1);
            this.tcAnalyze.Name = "tcAnalyze";
            this.tcAnalyze.SelectedIndex = 0;
            this.tcAnalyze.Size = new System.Drawing.Size(647, 452);
            this.tcAnalyze.TabIndex = 12;
            // 
            // tpAnalyze
            // 
            this.tpAnalyze.Controls.Add(this.lblDtToRight);
            this.tpAnalyze.Controls.Add(this.cbOneStockCode);
            this.tpAnalyze.Controls.Add(this.tbStockCode);
            this.tpAnalyze.Controls.Add(this.lbStockCode);
            this.tpAnalyze.Controls.Add(this.rbtdxfq);
            this.tpAnalyze.Controls.Add(this.rbD1File);
            this.tpAnalyze.Controls.Add(this.cbToStockCodeListFile);
            this.tpAnalyze.Controls.Add(this.cbAnalyzeInTradding);
            this.tpAnalyze.Controls.Add(this.label4);
            this.tpAnalyze.Controls.Add(this.label1);
            this.tpAnalyze.Controls.Add(this.lblARListFile);
            this.tpAnalyze.Controls.Add(this.cbUseARListFile);
            this.tpAnalyze.Controls.Add(this.lblShowOpenFileDiag);
            this.tpAnalyze.Controls.Add(this.lblDtToleft);
            this.tpAnalyze.Controls.Add(this.dtpEnd);
            this.tpAnalyze.Controls.Add(this.dtpStart);
            this.tpAnalyze.Controls.Add(this.btnanalyze);
            this.tpAnalyze.Location = new System.Drawing.Point(4, 22);
            this.tpAnalyze.Name = "tpAnalyze";
            this.tpAnalyze.Padding = new System.Windows.Forms.Padding(3);
            this.tpAnalyze.Size = new System.Drawing.Size(639, 426);
            this.tpAnalyze.TabIndex = 0;
            this.tpAnalyze.Text = "条件/项目";
            this.tpAnalyze.UseVisualStyleBackColor = true;
            // 
            // lblDtToRight
            // 
            this.lblDtToRight.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDtToRight.Location = new System.Drawing.Point(119, 18);
            this.lblDtToRight.Margin = new System.Windows.Forms.Padding(0);
            this.lblDtToRight.Name = "lblDtToRight";
            this.lblDtToRight.Size = new System.Drawing.Size(20, 12);
            this.lblDtToRight.TabIndex = 31;
            this.lblDtToRight.Text = "←";
            this.lblDtToRight.Click += new System.EventHandler(this.lblDtToRight_Click);
            // 
            // cbOneStockCode
            // 
            this.cbOneStockCode.AutoSize = true;
            this.cbOneStockCode.Location = new System.Drawing.Point(293, 45);
            this.cbOneStockCode.Name = "cbOneStockCode";
            this.cbOneStockCode.Size = new System.Drawing.Size(72, 16);
            this.cbOneStockCode.TabIndex = 30;
            this.cbOneStockCode.Text = "股票代码";
            this.cbOneStockCode.UseVisualStyleBackColor = true;
            // 
            // tbStockCode
            // 
            this.tbStockCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbStockCode.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbStockCode.Location = new System.Drawing.Point(365, 43);
            this.tbStockCode.Name = "tbStockCode";
            this.tbStockCode.Size = new System.Drawing.Size(44, 21);
            this.tbStockCode.TabIndex = 29;
            this.tbStockCode.Text = "600000";
            // 
            // lbStockCode
            // 
            this.lbStockCode.AutoEllipsis = true;
            this.lbStockCode.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbStockCode.Location = new System.Drawing.Point(308, 46);
            this.lbStockCode.Name = "lbStockCode";
            this.lbStockCode.Size = new System.Drawing.Size(65, 12);
            this.lbStockCode.TabIndex = 28;
            this.lbStockCode.Text = "股票代码";
            // 
            // rbtdxfq
            // 
            this.rbtdxfq.AutoSize = true;
            this.rbtdxfq.Location = new System.Drawing.Point(84, 29);
            this.rbtdxfq.Name = "rbtdxfq";
            this.rbtdxfq.Size = new System.Drawing.Size(71, 16);
            this.rbtdxfq.TabIndex = 25;
            this.rbtdxfq.Text = "复权数据";
            this.rbtdxfq.UseVisualStyleBackColor = true;
            // 
            // rbD1File
            // 
            this.rbD1File.AutoSize = true;
            this.rbD1File.Checked = true;
            this.rbD1File.Location = new System.Drawing.Point(10, 29);
            this.rbD1File.Name = "rbD1File";
            this.rbD1File.Size = new System.Drawing.Size(71, 16);
            this.rbD1File.TabIndex = 24;
            this.rbD1File.TabStop = true;
            this.rbD1File.Text = "除权数据";
            this.rbD1File.UseVisualStyleBackColor = true;
            // 
            // cbToStockCodeListFile
            // 
            this.cbToStockCodeListFile.AutoSize = true;
            this.cbToStockCodeListFile.Location = new System.Drawing.Point(366, 28);
            this.cbToStockCodeListFile.Name = "cbToStockCodeListFile";
            this.cbToStockCodeListFile.Size = new System.Drawing.Size(15, 14);
            this.cbToStockCodeListFile.TabIndex = 21;
            this.cbToStockCodeListFile.UseVisualStyleBackColor = true;
            // 
            // cbAnalyzeInTradding
            // 
            this.cbAnalyzeInTradding.AutoSize = true;
            this.cbAnalyzeInTradding.Location = new System.Drawing.Point(293, 28);
            this.cbAnalyzeInTradding.Name = "cbAnalyzeInTradding";
            this.cbAnalyzeInTradding.Size = new System.Drawing.Size(15, 14);
            this.cbAnalyzeInTradding.TabIndex = 21;
            this.cbAnalyzeInTradding.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Location = new System.Drawing.Point(379, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "结果输出为测试用列表";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Location = new System.Drawing.Point(308, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 20;
            this.label1.Text = "盘中分析";
            // 
            // lblARListFile
            // 
            this.lblARListFile.AutoEllipsis = true;
            this.lblARListFile.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblARListFile.Location = new System.Drawing.Point(386, 12);
            this.lblARListFile.Name = "lblARListFile";
            this.lblARListFile.Size = new System.Drawing.Size(92, 13);
            this.lblARListFile.TabIndex = 19;
            // 
            // cbUseARListFile
            // 
            this.cbUseARListFile.AutoSize = true;
            this.cbUseARListFile.Location = new System.Drawing.Point(293, 11);
            this.cbUseARListFile.Name = "cbUseARListFile";
            this.cbUseARListFile.Size = new System.Drawing.Size(15, 14);
            this.cbUseARListFile.TabIndex = 18;
            this.cbUseARListFile.UseVisualStyleBackColor = true;
            // 
            // lblShowOpenFileDiag
            // 
            this.lblShowOpenFileDiag.AutoSize = true;
            this.lblShowOpenFileDiag.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblShowOpenFileDiag.Location = new System.Drawing.Point(308, 12);
            this.lblShowOpenFileDiag.Name = "lblShowOpenFileDiag";
            this.lblShowOpenFileDiag.Size = new System.Drawing.Size(77, 12);
            this.lblShowOpenFileDiag.TabIndex = 8;
            this.lblShowOpenFileDiag.Text = "分析结果列表";
            this.lblShowOpenFileDiag.Click += new System.EventHandler(this.lblShowOpenFileDiag_Click);
            // 
            // lblDtToleft
            // 
            this.lblDtToleft.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDtToleft.Location = new System.Drawing.Point(120, 3);
            this.lblDtToleft.Margin = new System.Windows.Forms.Padding(0);
            this.lblDtToleft.Name = "lblDtToleft";
            this.lblDtToleft.Size = new System.Drawing.Size(20, 12);
            this.lblDtToleft.TabIndex = 16;
            this.lblDtToleft.Text = "→";
            this.lblDtToleft.Click += new System.EventHandler(this.lblDtToleft_Click);
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(144, 6);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(109, 21);
            this.dtpEnd.TabIndex = 15;
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(6, 6);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(111, 21);
            this.dtpStart.TabIndex = 14;
            this.dtpStart.Value = new System.DateTime(2012, 2, 22, 0, 0, 0, 0);
            // 
            // btnanalyze
            // 
            this.btnanalyze.Location = new System.Drawing.Point(554, 7);
            this.btnanalyze.Name = "btnanalyze";
            this.btnanalyze.Size = new System.Drawing.Size(75, 20);
            this.btnanalyze.TabIndex = 11;
            this.btnanalyze.Text = "分析";
            this.btnanalyze.UseVisualStyleBackColor = true;
            this.btnanalyze.Click += new System.EventHandler(this.btnanalyze_Click);
            // 
            // FormAnalyze
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 452);
            this.Controls.Add(this.tcAnalyze);
            this.KeyPreview = true;
            this.Name = "FormAnalyze";
            this.Text = "FormAnalyze";
            this.Shown += new System.EventHandler(this.FormAnalyze_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormAnalyze_KeyPress);
            this.Resize += new System.EventHandler(this.FormAnalyze_Resize);
            this.tcAnalyze.ResumeLayout(false);
            this.tpAnalyze.ResumeLayout(false);
            this.tpAnalyze.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.TabControl tcAnalyze;
        private System.Windows.Forms.TabPage tpAnalyze;
        private System.Windows.Forms.Button btnanalyze;
        private System.Windows.Forms.Label lblDtToleft;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.CheckBox cbUseARListFile;
        private System.Windows.Forms.Label lblShowOpenFileDiag;
        private System.Windows.Forms.Label lblARListFile;
        private System.Windows.Forms.CheckBox cbAnalyzeInTradding;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbToStockCodeListFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rbtdxfq;
        private System.Windows.Forms.RadioButton rbD1File;
        private System.Windows.Forms.TextBox tbStockCode;
        private System.Windows.Forms.Label lbStockCode;
        private System.Windows.Forms.CheckBox cbOneStockCode;
        private System.Windows.Forms.Label lblDtToRight;
    }
}

