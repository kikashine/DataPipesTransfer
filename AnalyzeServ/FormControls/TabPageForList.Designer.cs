using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public partial class TabPageForList : System.Windows.Forms.TabPage
    {
        protected System.Windows.Forms.Label labelPgsNow;
        protected System.Windows.Forms.Label labelARListFile;
        protected System.Windows.Forms.Label labelSelectRowsCount;
        protected System.Windows.Forms.Label labelDate;
        protected System.Windows.Forms.Label labelKBInput;
        protected System.Windows.Forms.Button btnanalyze;
        protected System.Windows.Forms.Button btnclosetp;
        protected System.Windows.Forms.DataGridView dgvAnalyzRslt;
        protected System.Windows.Forms.CheckBox cbShowKLineGraphic;
        protected System.Windows.Forms.Label lblColor1;
        protected System.Windows.Forms.Label lblColor7;
        protected System.Windows.Forms.Label lblColor6;
        protected System.Windows.Forms.Label lblColor5;
        protected System.Windows.Forms.Label lblColor4;
        protected System.Windows.Forms.Label lblColor3;
        protected System.Windows.Forms.Label lblColor2;
        protected System.Windows.Forms.Label lblColor8;
        protected System.Windows.Forms.Label lblColorReset;
        public System.Windows.Forms.Button btnMenu;
        protected ContextMenuStripForAnalyze cMenus;
        protected TabControl container;

        protected System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1;
        protected System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2;
        protected System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3;
        protected System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStylePercent;

        protected FormKLineGraphic FormK;

        private void InitializeComponent()
        {
            this.dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewCellStylePercent = new System.Windows.Forms.DataGridViewCellStyle();

            this.dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            this.dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            this.dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;

            this.dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            this.dataGridViewCellStylePercent.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewCellStylePercent.BackColor = System.Drawing.SystemColors.Window;
            this.dataGridViewCellStylePercent.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridViewCellStylePercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridViewCellStylePercent.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dataGridViewCellStylePercent.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridViewCellStylePercent.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCellStylePercent.Format = "0%";
            this.dataGridViewCellStylePercent.NullValue = null;

            labelPgsNow = new Label();
            labelARListFile = new Label();
            labelSelectRowsCount = new Label();
            labelKBInput = new Label();
            labelDate = new Label();
            cbShowKLineGraphic = new CheckBox();
            btnanalyze = new Button();
            btnclosetp = new Button();
            dgvAnalyzRslt = new System.Windows.Forms.DataGridView();
            lblColor1 = new System.Windows.Forms.Label();
            lblColor2 = new System.Windows.Forms.Label();
            lblColor3 = new System.Windows.Forms.Label();
            lblColor4 = new System.Windows.Forms.Label();
            lblColor5 = new System.Windows.Forms.Label();
            lblColor6 = new System.Windows.Forms.Label();
            lblColor7 = new System.Windows.Forms.Label();
            lblColorReset = new System.Windows.Forms.Label();
            lblColor8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(dgvAnalyzRslt)).BeginInit();
            this.cMenus = new ContextMenuStripForAnalyze(this);
            btnMenu = new Button();

            // 
            // labelPgsNow
            // 
            labelPgsNow.AutoSize = true;
            labelPgsNow.Location = new System.Drawing.Point(8, 7);
            labelPgsNow.Name = "labelPgsNow";
            labelPgsNow.Size = new System.Drawing.Size(39, 12);
            labelPgsNow.TabIndex = 9;
            labelPgsNow.Text = "    ";

            // 
            // labelDate
            // 
            labelDate.AutoSize = true;
            labelDate.Location = new System.Drawing.Point(7, 23);
            labelDate.Name = "labelDate" + this.Name;
            labelDate.Size = new System.Drawing.Size(39, 12);
            labelDate.TabIndex = 9;
            

            //
            //labelKBInput
            //
            labelKBInput.AutoSize = true;
            labelKBInput.Location = new System.Drawing.Point(230, 40);
            labelKBInput.Name = "labelKBInput" + this.Name;
            labelKBInput.Size = new System.Drawing.Size(39, 12);
            labelKBInput.TabIndex = 9;
            labelKBInput.Text = "输入代码：";


            // 
            // labelARListFile
            // 
            labelARListFile.AutoEllipsis = true;
            labelARListFile.Location = new System.Drawing.Point(290, 26);
            labelARListFile.Name = "labelARListFile";
            labelARListFile.Size = new System.Drawing.Size(160, 12);
            labelARListFile.AutoSize = false;
            labelARListFile.TabIndex = 9;


            // 
            // labelSelectRowsCount
            // 
            labelSelectRowsCount.AutoSize = true;
            labelSelectRowsCount.Location = new System.Drawing.Point(230, 7);
            labelSelectRowsCount.Name = "labelSelectRowsCount";
            labelSelectRowsCount.Size = new System.Drawing.Size(39, 12);
            labelSelectRowsCount.TabIndex = 9;
            labelSelectRowsCount.Text = "选择股票数：";

            // 
            // lblColor1
            // 
            lblColor1.BackColor = System.Drawing.Color.LightSalmon;
            lblColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor1.Location = new System.Drawing.Point(462, 25);
            lblColor1.Name = "lblColor1";
            lblColor1.Size = new System.Drawing.Size(12, 12);
            lblColor1.TabIndex = 23;
            lblColor1.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor2
            // 
            lblColor2.BackColor = System.Drawing.Color.Gold;
            lblColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor2.Location = new System.Drawing.Point(480, 25);
            lblColor2.Name = "lblColor2";
            lblColor2.Size = new System.Drawing.Size(12, 12);
            lblColor2.TabIndex = 24;
            lblColor2.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor3
            // 
            lblColor3.BackColor = System.Drawing.Color.LightGreen;
            lblColor3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor3.Location = new System.Drawing.Point(498, 25);
            lblColor3.Name = "lblColor3";
            lblColor3.Size = new System.Drawing.Size(12, 12);
            lblColor3.TabIndex = 25;
            lblColor3.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor4
            // 
            lblColor4.BackColor = System.Drawing.Color.MediumTurquoise;
            lblColor4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor4.Location = new System.Drawing.Point(516, 25);
            lblColor4.Name = "lblColor4";
            lblColor4.Size = new System.Drawing.Size(12, 12);
            lblColor4.TabIndex = 26;
            lblColor4.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor5
            // 
            lblColor5.BackColor = System.Drawing.Color.LightBlue;
            lblColor5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor5.Location = new System.Drawing.Point(534, 25);
            lblColor5.Name = "lblColor5";
            lblColor5.Size = new System.Drawing.Size(12, 12);
            lblColor5.TabIndex = 27;
            lblColor5.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor6
            // 
            lblColor6.BackColor = System.Drawing.Color.Thistle;
            lblColor6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor6.Location = new System.Drawing.Point(552, 25);
            lblColor6.Name = "lblColor6";
            lblColor6.Size = new System.Drawing.Size(12, 12);
            lblColor6.TabIndex = 28;
            lblColor6.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor7
            // 
            lblColor7.BackColor = System.Drawing.Color.Pink;
            lblColor7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor7.Location = new System.Drawing.Point(570, 25);
            lblColor7.Name = "lblColor7";
            lblColor7.Size = new System.Drawing.Size(12, 12);
            lblColor7.TabIndex = 29;
            lblColor7.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor8
            // 
            lblColor8.BackColor = System.Drawing.Color.White;
            lblColor8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor8.Location = new System.Drawing.Point(588, 25);
            lblColor8.Name = "lblColor8";
            lblColor8.Size = new System.Drawing.Size(12, 12);
            lblColor8.TabIndex = 31;
            lblColor8.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColorReset
            // 
            lblColorReset.BackColor = System.Drawing.Color.White;
            lblColorReset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColorReset.Location = new System.Drawing.Point(606, 25);
            lblColorReset.Name = "lblColorReset";
            lblColorReset.Size = new System.Drawing.Size(12, 12);
            lblColorReset.TabIndex = 30;
            lblColorReset.Tag = "取消所有颜色";
            lblColorReset.Text = "X";
            lblColorReset.Click += new System.EventHandler(lblColor_Click);


            //
            //cbShowKLineGraphic
            //
            cbShowKLineGraphic.Location = new Point(380, 5);
            cbShowKLineGraphic.Size = new System.Drawing.Size(78, 15);
            cbShowKLineGraphic.Name = "cbShowKLineGraphic";
            cbShowKLineGraphic.TabIndex = 10;
            cbShowKLineGraphic.Text = "显示k线图";
            cbShowKLineGraphic.CheckedChanged += new EventHandler(cbShowKLineGraphic_CheckedChanged);

            // 
            // dgvAnalyzRslt
            // 
            dgvAnalyzRslt.AllowUserToAddRows = false;
            dgvAnalyzRslt.AllowUserToDeleteRows = false;
            dgvAnalyzRslt.AllowUserToResizeRows = false;
            dgvAnalyzRslt.AllowUserToResizeColumns = true;

            dgvAnalyzRslt.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvAnalyzRslt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            dgvAnalyzRslt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvAnalyzRslt.DefaultCellStyle = dataGridViewCellStyle2;
            dgvAnalyzRslt.Location = new System.Drawing.Point(10, 63);
            dgvAnalyzRslt.Name = "dgvAnalyzRslt";
            dgvAnalyzRslt.ReadOnly = true;
            dgvAnalyzRslt.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvAnalyzRslt.RowTemplate.Height = 23;
            dgvAnalyzRslt.ShowEditingIcon = false;
            
            dgvAnalyzRslt.TabIndex = 1;
            dgvAnalyzRslt.SortCompare += new DataGridViewSortCompareEventHandler(
                                      this.dgvAnalyzRslt_SortCompare);
            dgvAnalyzRslt.SelectionChanged += new System.EventHandler(this.dgvAnalyzRslt_SelectionChanged);
            dgvAnalyzRslt.AllowDrop = true;
            dgvAnalyzRslt.DragEnter += new DragEventHandler(dgvAnalyzRslt_DragEnter);
            dgvAnalyzRslt.CellMouseMove += new DataGridViewCellMouseEventHandler(dgvAnalyzRslt_CellMouseMove);
            dgvAnalyzRslt.DragDrop += new DragEventHandler(dgvAnalyzRslt_DragDrop);
            dgvAnalyzRslt.DragOver += new DragEventHandler(dgvAnalyzRslt_DragOver);
            dgvAnalyzRslt.SelectionChanged += new EventHandler(dgvAnalyzRslt_SelectionChanged);
            dgvAnalyzRslt.KeyDown += new KeyEventHandler(dgvAnalyzRslt_KeyDown); //new KeyPressEventHandler(dgvAnalyzRslt_KeyPress);

            //
            //btnMenu
            //
            this.btnMenu.Location = new System.Drawing.Point(12, 40);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(18, 18);
            this.btnMenu.TabIndex = 20;
            this.btnMenu.Text = "+";
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);

            //
            //cMenus
            //
            this.cMenus.Name = "cMenus";
            this.cMenus.Size = new System.Drawing.Size(120, 40);

            this.Disposed += new EventHandler(TabPageForList_Disposed);
            this.Controls.Add(btnanalyze);
            this.Controls.Add(btnclosetp);

            this.Controls.Add(labelPgsNow);
            this.Controls.Add(labelARListFile);
            this.Controls.Add(labelSelectRowsCount);
            this.Controls.Add(labelDate);
            this.Controls.Add(labelKBInput);
            this.Controls.Add(cbShowKLineGraphic);
            this.Controls.Add(this.lblColor8);
            this.Controls.Add(this.lblColorReset);
            this.Controls.Add(this.lblColor7);
            this.Controls.Add(this.lblColor6);
            this.Controls.Add(this.lblColor5);
            this.Controls.Add(this.lblColor4);
            this.Controls.Add(this.lblColor3);
            this.Controls.Add(this.lblColor2);
            this.Controls.Add(this.lblColor1);
            this.Controls.Add(btnMenu);
            this.Controls.Add(dgvAnalyzRslt);

            //为每个生成的tab分配唯一的name，此name也关系到该tab的分析按钮和关闭按钮的唯一性
            this.Name = DateTime.Now.Ticks.ToString();
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(548, 288);
            this.TabIndex = 0;
            this.Text = "分析结果" + Convert.ToString(tabcount + 1);
            this.UseVisualStyleBackColor = true;


           
            btnanalyze.Name = "btn" + this.Name;
            btnanalyze.Size = new System.Drawing.Size(75, 20);
            btnanalyze.TabIndex = 11;
            btnanalyze.Text = "取消分析";
            btnanalyze.UseVisualStyleBackColor = true;
            btnanalyze.Click += new System.EventHandler(this.btnanalyze_Click);

            
            btnclosetp.Name = "close" + this.Name;
            btnclosetp.Size = new System.Drawing.Size(20, 20);
            btnclosetp.TabIndex = 11;
            btnclosetp.Text = "X";
            btnclosetp.UseVisualStyleBackColor = true;
            btnclosetp.Click += new System.EventHandler(this.btnclosetp_Click);


            ((System.ComponentModel.ISupportInitialize)(dgvAnalyzRslt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void init()
        {
            btnclosetp.Location = new System.Drawing.Point(container.Size.Width - 30, 2);
            btnanalyze.Location = new System.Drawing.Point(container.Size.Width - 115, 2);
            dgvAnalyzRslt.Size = new System.Drawing.Size(container.Size.Width - 30, container.Size.Height - 90);
            FormK = new FormKLineGraphic();
            FormK.tabpage = this;
            FormK.mainform = (Form)this.Parent.Parent;
            //设置Owner可以使FormK始终浮动在OwnerForm之上，且OwnerForm也可以获得焦点
            FormK.Owner = (Form)this.Parent.Parent;
            cMenus.init();
            labelARListFile.Text = ARListFileName;
        
        }
    }
}
