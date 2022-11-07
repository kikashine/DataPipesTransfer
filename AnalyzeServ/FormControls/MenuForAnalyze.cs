using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class ContextMenuStripForAnalyze : ContextMenuStrip
    {
        /// <summary>
        /// menuPatternFit对应的form
        /// </summary>
        private MenuFormPatternFit mfpatternfit;

        private MenuFormShowKLine mfshowkline;

        private MenuFormSaveList mfsavelist;
        /// <summary>
        /// 调用本menu实例的控件
        /// </summary>
        public Control Master;
        /// <summary>
        /// 选择的Menu项名称
        /// </summary>
        public string selectMenu;

        public bool initialized=false;

        public ContextMenuStripForAnalyze(Control control)
            : base()
        {
            this.Master = control;
            
        }
        public void init()
        {
            //
            //形态匹配menuitem
            //
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem();
            menuItem1.Name = "patternFit";
            menuItem1.Size = new System.Drawing.Size(178, 22);
            menuItem1.Text = "形态匹配";
            menuItem1.Click += new EventHandler(menuItem1_Click);
            this.Items.Add(menuItem1);
            //
            //FormPatternFit
            //
            mfpatternfit = new MenuFormPatternFit(this);
            //设置Owner可以让form一直浮在主form之上
            mfpatternfit.Owner = (Form)this.Master.Parent.Parent;

            //
            //保存列表menuitem
            //
            ToolStripMenuItem menuItem2 = new ToolStripMenuItem();
            menuItem2.Name = "saveList";
            menuItem2.Size = new System.Drawing.Size(178, 22);
            menuItem2.Text = "保存列表";
            menuItem2.Click += new EventHandler(menuItem2_Click);
            this.Items.Add(menuItem2);
            //
            //FormSaveList
            //
            mfsavelist = new MenuFormSaveList(this);
            //设置Owner可以让form一直浮在主form之上
            mfsavelist.Owner = (Form)this.Master.Parent.Parent;
            //
            //查看k线menuitem
            //
            ToolStripMenuItem menuItem3 = new ToolStripMenuItem();
            menuItem3.Name = "showKLine";
            menuItem3.Size = new System.Drawing.Size(178, 22);
            menuItem3.Text = "查看k线";
            menuItem3.Click += new EventHandler(menuItem3_Click);
            this.Items.Add(menuItem3);
            //
            //FormShowKLine
            //
            mfshowkline = new MenuFormShowKLine(this);
            //设置Owner可以让form一直浮在主form之上
            mfshowkline.Owner = (Form)this.Master.Parent.Parent;

            this.Disposed += new EventHandler(ContextMenuStripForAnalyze_Disposed);
            initialized = true;
        }

        void ContextMenuStripForAnalyze_Disposed(object sender, EventArgs e)
        {
            this.mfpatternfit.Close();
            this.mfpatternfit.Dispose();
        }

        /// <summary>
        /// 点击形态匹配menuitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (selectMenu != "")
            {
                hideMenuForm();
            }
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            this.selectMenu = menu.Name;
            this.showMenuForm();
        }

        /// <summary>
        /// 点击保存列表menuitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem2_Click(object sender, EventArgs e)
        {
            if (selectMenu != "")
            {
                hideMenuForm();
            }
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            this.selectMenu = menu.Name;
            this.showMenuForm();
        }

        /// <summary>
        /// 点击查看k线menuitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem3_Click(object sender, EventArgs e)
        {
            if (selectMenu != "")
            {
                hideMenuForm();
            }
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            this.selectMenu = menu.Name;
            this.showMenuForm();
        }

        public void showMenuForm()
        {
            //hideMenuForm();
            Point tp = this.PointToScreen(new Point(((TabPageForList)this.Master).btnMenu.Left, ((TabPageForList)this.Master).btnMenu.Top));
            switch (this.selectMenu)
            {
                case "patternFit":
                    mfpatternfit.Show();
                    mfpatternfit.Location = new Point(tp.X + 18, tp.Y);
                    break;
                case "showKLine":
                    mfshowkline.Show();
                    //Point tp = this.PointToScreen(new Point(((TabPageWithMultiThreadBase_old)this.Master).btnMenu.Left, ((TabPageWithMultiThreadBase_old)this.Master).btnMenu.Top));
                    mfshowkline.Location = new Point(tp.X + 18, tp.Y);
                    //((TabPageWithMultiThreadBase_old)this.Master).Focus();
                    break;
                case "saveList":
                    mfsavelist.Show();
                    mfsavelist.Location = new Point(tp.X + 18, tp.Y);
                    mfsavelist.RedrawSelectedColor();
                    break;
                default:
                    break;
            }
            ((TabPageForList)this.Master).Focus();
        }
        public void hideMenuForm()
        {
            switch (this.selectMenu)
            {
                case "patternFit":
                    mfpatternfit.Hide();
                    //Point tp = this.PointToScreen(new Point(((TabPageWithMultiThreadBase_old)this.Master).btnMenu.Left, ((TabPageWithMultiThreadBase_old)this.Master).btnMenu.Top));
                    //mfpatternfit.Location = new Point(tp.X + 18, tp.Y);
                    //((TabPageWithMultiThreadBase_old)this.Master).Focus();
                    break;
                case "showKline":
                    mfshowkline.Hide();
                    break;
                case "saveList":
                    mfsavelist.Hide();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 选择菜单项后弹出的Form，此为基类，对应各不同菜单项的Form需要根据它派生
    /// </summary>
    public class MenuForm : Form
    {
        protected System.Windows.Forms.Label lblClose;
        protected System.Windows.Forms.Label lblMove;
        protected System.Windows.Forms.Panel panel;
        protected bool _canMoveForm = false;
        protected int _mouseLocationX;
        protected int _mouseLocationY;
        /// <summary>
        /// 本form的调用menu
        /// </summary>
        protected ContextMenuStripForAnalyze Master;
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

        protected MenuForm(Control control)   
        {
            this.lblClose = new System.Windows.Forms.Label();
            this.lblMove = new System.Windows.Forms.Label();
            panel = new Panel();
            this.SuspendLayout();


            // 
            // lblClose
            // 
            this.lblClose.ForeColor = System.Drawing.Color.Black;
            this.lblClose.Location = new System.Drawing.Point(742, 2);
            this.lblClose.Name = "lblClose";
            this.lblClose.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClose.Size = new System.Drawing.Size(16, 14);
            this.lblClose.TabIndex = 3;
            this.lblClose.Text = "X";
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            this.lblClose.BorderStyle = BorderStyle.FixedSingle;
            // 
            // lblMove
            // 
            this.lblMove.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.lblMove.ForeColor = System.Drawing.Color.Black;
            this.lblMove.Location = new System.Drawing.Point(4, 3);
            this.lblMove.Name = "lblMove";
            this.lblMove.Size = new System.Drawing.Size(732, 14);
            this.lblMove.TabIndex = 2;
            this.lblMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseMove);
            this.lblMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseDown);
            this.lblMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblMove_MouseUp);

            panel.Width = 760;
            panel.Height = 726;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = SystemColors.Info;
            panel.Controls.Add(this.lblClose);
            panel.Controls.Add(this.lblMove);
            // 
            // FormKLineGraphic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(760, 726);
            this.ControlBox = false;
            //this.Controls.Add(this.lblClose);
            //this.Controls.Add(this.lblMove);
            this.Controls.Add(panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuForm";
            this.ShowIcon = false;
            this.Text = "MenuForm";
            this.ShowInTaskbar = false;
            this.Master = (ContextMenuStripForAnalyze)control;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected void lblClose_Click(object sender, EventArgs e)
        {
            //CLinelastPoint = new Point(-1, -1);
            this.Hide();
            this.Master.selectMenu = "";
            //this.Master.Show();
        }
        protected void lblMove_MouseDown(object sender, MouseEventArgs e)
        {
            _canMoveForm = true;
            _mouseLocationX = e.X;
            _mouseLocationY = e.Y;
        }

        protected void lblMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (_canMoveForm)
            {
                Point p = new Point(this.Location.X + e.X - _mouseLocationX, this.Location.Y + e.Y - _mouseLocationY);
                //Point p = new Point(this.Location.X - e.X, this.Location.Y - e.Y);
                this.Location = p;
            }
        }

        protected void lblMove_MouseUp(object sender, MouseEventArgs e)
        {
            _canMoveForm = false;
        }

    }

    public class MenuFormPatternFit : MenuForm
    {
        private DateTimePicker dtpEnd;
        private DateTimePicker dtpStart;

        public MenuFormPatternFit(Control control)
            : base(control)
        {
            this.ClientSize = new System.Drawing.Size(400, 226);
            panel.Width = this.Width;
            panel.Height = this.Height;
            this.lblMove.Width = this.Width - this.lblClose.Width - 3;
            this.lblClose.Left = this.lblMove.Width;
            this.lblMove.Text = "形态匹配条件";

            //
            //确定按钮
            //
            Button btnok = new Button();
            btnok.Size= new Size(50, 22);
            btnok.Location = new Point((this.Width - btnok.Width) / 2, this.Height - 40);
            btnok.Text = "确定";
            btnok.Name = "btnok";
            btnok.UseVisualStyleBackColor = true;
            btnok.FlatAppearance.BorderSize = 0;
            btnok.FlatStyle = FlatStyle.Flat;
            btnok.Click += new EventHandler(btnok_Click);

            //
            //时间选择
            //
            List<AnalyzeParameters> oParams = ((TabPageForList)this.Master.Master).Parameters;
            dtpEnd = new DateTimePicker();
            dtpStart = new DateTimePicker();
            dtpEnd.Location = new System.Drawing.Point(140, 18);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.Size = new System.Drawing.Size(109, 21);
            dtpEnd.TabIndex = 15;
            dtpEnd.Value = DateTime.Parse(oParams[0].DateEnd);
            dtpStart.Location = new System.Drawing.Point(9, 18);
            dtpStart.Name = "dtpStart";
            dtpStart.Size = new System.Drawing.Size(111, 21);
            dtpStart.TabIndex = 14;
            dtpStart.Value = DateTime.Parse(oParams[0].DateStart); ;

            //
            //起始时间控件之间的连接符
            //
            Label label3 = new Label();
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(123, 21);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(11, 21);
            label3.TabIndex = 16;
            label3.Text = "-";

            panel.Controls.Add(btnok);
            panel.Controls.Add(dtpStart);
            panel.Controls.Add(dtpEnd);
            panel.Controls.Add(label3);
            panel.PerformLayout();
        }

        void btnok_Click(object sender, EventArgs e)
        {
            PatternFitParameters Params = new PatternFitParameters();

            Hashtable pmark = ((TabPageForList)this.Master.Master).formK.patternMark;
            if (pmark.Count < 2)
            {
                MessageBox.Show("未选择形态的起始及结束点。");
                return;
            }
            //处理形态起始结束点
            int[] keys=new int[2];
            int keyscount = 0;
            foreach(object key in pmark.Keys)
            {
                keys[keyscount] = (int)key;
                keyscount++;
            }
            if (keys[0] > keys[1])
            {
                Params.PatternBeginI = keys[1];
                Params.PatternBeginD = (DateTime)pmark[keys[1]];
                Params.PatternEndI = keys[0];
                Params.PatternEndD = (DateTime)pmark[keys[0]];
            }
            else
            {
                Params.PatternBeginI = keys[0];
                Params.PatternBeginD = (DateTime)pmark[keys[0]];
                Params.PatternEndI = keys[1];
                Params.PatternEndD = (DateTime)pmark[keys[1]];
            }
            Params.DateStart = Utility.toDBDate(dtpStart.Value);
            Params.DateEnd = Utility.toDBDate(dtpEnd.Value);
            this.Hide();
            this.Master.selectMenu = "";

            //((TabPageForList)this.Master.Master).patternFit(Params);

        }
    }

    public class MenuFormShowKLine : MenuForm
    {
        private DateTimePicker dtpEnd;
        private TextBox tbStockCode;
        //private DateTimePicker dtpStart;

        public MenuFormShowKLine(Control control)
            : base(control)
        {
            this.ClientSize = new System.Drawing.Size(400, 226);
            panel.Width = this.Width;
            panel.Height = this.Height;
            this.lblMove.Width = this.Width - this.lblClose.Width - 3;
            this.lblClose.Left = this.lblMove.Width;
            this.lblMove.Text = "查看k线";

            //
            //确定按钮
            //
            Button btnok = new Button();
            btnok.Size = new Size(50, 22);
            btnok.Location = new Point((this.Width - btnok.Width) / 2, this.Height - 40);
            btnok.Text = "确定";
            btnok.Name = "btnok";
            btnok.UseVisualStyleBackColor = true;
            btnok.FlatAppearance.BorderSize = 0;
            btnok.FlatStyle = FlatStyle.Flat;
            btnok.Click += new EventHandler(btnok_Click);


            Label lbdate = new Label();
            lbdate.Location = new System.Drawing.Point(4, 21);
            lbdate.Name = "lbdate";
            lbdate.Size = new System.Drawing.Size(42, 12);
            lbdate.Text = "日期：";

            //
            //时间选择
            //
            List<AnalyzeParameters> oParams = ((TabPageForList)this.Master.Master).Parameters;
            dtpEnd = new DateTimePicker();
            //dtpStart = new DateTimePicker();
            dtpEnd.Location = new System.Drawing.Point(46, 18);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.Size = new System.Drawing.Size(109, 21);
            dtpEnd.TabIndex = 15;
            dtpEnd.Value = DateTime.Parse(oParams[0].DateEnd);
            //dtpStart.Location = new System.Drawing.Point(9, 18);
            //dtpStart.Name = "dtpStart";
            //dtpStart.Size = new System.Drawing.Size(111, 21);
            //dtpStart.TabIndex = 14;
            //dtpStart.Value = DateTime.Parse(oParams[0].DateStart); ;

            
            Label lbstockcode = new Label();
            lbstockcode.Location = new System.Drawing.Point(160, 21);
            lbstockcode.Name = "lbstockcode";
            lbstockcode.Size = new System.Drawing.Size(65, 12);
            lbstockcode.Text = "股票代码：";

            tbStockCode = new TextBox();
            tbStockCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tbStockCode.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            tbStockCode.Location = new System.Drawing.Point(225, 18);
            tbStockCode.Name = "tbStockCode";
            tbStockCode.Size = new System.Drawing.Size(44, 21);
            tbStockCode.TabIndex = 29;
            //tbStockCode.Text = "600000";
            //
            //起始时间控件之间的连接符
            //
            //Label label3 = new Label();
            //label3.AutoSize = true;
            //label3.Location = new System.Drawing.Point(123, 21);
            //label3.Name = "label3";
            //label3.Size = new System.Drawing.Size(11, 21);
            //label3.TabIndex = 16;
            //label3.Text = "-";

            panel.Controls.Add(btnok);
            panel.Controls.Add(lbdate);
            panel.Controls.Add(lbstockcode);
            panel.Controls.Add(tbStockCode);
            //panel.Controls.Add(dtpStart);
            panel.Controls.Add(dtpEnd);
            //panel.Controls.Add(label3);
            panel.PerformLayout();
        }

        public new void Show()
        {
            tbStockCode.Text = ((TabPageForList)this.Master.Master).selectedStock;
            tbStockCode.SelectAll();
            base.Show();
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            ShowKLineParameters Params = new ShowKLineParameters();
            Params.DateStart = Utility.toDBDate(dtpEnd.Value);
            Params.DateEnd = Utility.toDBDate(dtpEnd.Value);
            Params.StockCode = tbStockCode.Text;
            this.Hide();
            this.Master.selectMenu = "";
            //((TabPageForList)this.Master.Master).showKLine(Params);
        }
    }

    public class MenuFormSaveList : MenuForm
    {
        private Dictionary<string, Color> selectColor;

        private System.Windows.Forms.Label lblColor1;
        private System.Windows.Forms.Label lblColor7;
        private System.Windows.Forms.Label lblColor6;
        private System.Windows.Forms.Label lblColor5;
        private System.Windows.Forms.Label lblColor4;
        private System.Windows.Forms.Label lblColor3;
        private System.Windows.Forms.Label lblColor2;
        //private System.Windows.Forms.Label lblColor8;
        private System.Windows.Forms.Label lblColorReset;

        public MenuFormSaveList(Control control)
            : base(control)
        {
            selectColor =  new Dictionary<string, Color>();
            lblColor1 = new System.Windows.Forms.Label();
            lblColor2 = new System.Windows.Forms.Label();
            lblColor3 = new System.Windows.Forms.Label();
            lblColor4 = new System.Windows.Forms.Label();
            lblColor5 = new System.Windows.Forms.Label();
            lblColor6 = new System.Windows.Forms.Label();
            lblColor7 = new System.Windows.Forms.Label();
            lblColorReset = new System.Windows.Forms.Label();
            //lblColor8 = new System.Windows.Forms.Label();

            this.ClientSize = new System.Drawing.Size(400, 226);
            panel.Width = this.Width;
            panel.Height = this.Height;
            this.lblMove.Width = this.Width - this.lblClose.Width - 3;
            this.lblClose.Left = this.lblMove.Width;
            this.lblMove.Text = "保存列表";

            //
            //确定按钮
            //
            Button btnok = new Button();
            btnok.Size = new Size(50, 22);
            btnok.Location = new Point((this.Width - btnok.Width) / 2, this.Height - 40);
            btnok.Text = "保存";
            btnok.Name = "btnok";
            btnok.UseVisualStyleBackColor = true;
            btnok.FlatAppearance.BorderSize = 0;
            btnok.FlatStyle = FlatStyle.Flat;
            btnok.Click += new EventHandler(btnok_Click);
            // 
            // lblColorReset
            // 
            lblColorReset.BackColor = System.Drawing.Color.White;
            lblColorReset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColorReset.Location = new System.Drawing.Point(4, 25);
            lblColorReset.Name = "lblColorReset";
            lblColorReset.Size = new System.Drawing.Size(50, 12);
            lblColorReset.TabIndex = 30;
            lblColorReset.Tag = "取消所选颜色，保存所有";
            lblColorReset.Text = "ALLCELL";
            lblColorReset.Click += new System.EventHandler(lblColor_Click);

            // 
            // lblColor1
            // 
            lblColor1.BackColor = System.Drawing.Color.LightSalmon;
            lblColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor1.Location = new System.Drawing.Point(64, 25);
            lblColor1.Name = "lblColor1";
            lblColor1.Size = new System.Drawing.Size(12, 12);
            lblColor1.TabIndex = 23;
            lblColor1.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor2
            // 
            lblColor2.BackColor = System.Drawing.Color.Gold;
            lblColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor2.Location = new System.Drawing.Point(86, 25);
            lblColor2.Name = "lblColor2";
            lblColor2.Size = new System.Drawing.Size(12, 12);
            lblColor2.TabIndex = 24;
            lblColor2.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor3
            // 
            lblColor3.BackColor = System.Drawing.Color.LightGreen;
            lblColor3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor3.Location = new System.Drawing.Point(108, 25);
            lblColor3.Name = "lblColor3";
            lblColor3.Size = new System.Drawing.Size(12, 12);
            lblColor3.TabIndex = 25;
            lblColor3.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor4
            // 
            lblColor4.BackColor = System.Drawing.Color.MediumTurquoise;
            lblColor4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor4.Location = new System.Drawing.Point(130, 25);
            lblColor4.Name = "lblColor4";
            lblColor4.Size = new System.Drawing.Size(12, 12);
            lblColor4.TabIndex = 26;
            lblColor4.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor5
            // 
            lblColor5.BackColor = System.Drawing.Color.LightBlue;
            lblColor5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor5.Location = new System.Drawing.Point(152, 25);
            lblColor5.Name = "lblColor5";
            lblColor5.Size = new System.Drawing.Size(12, 12);
            lblColor5.TabIndex = 27;
            lblColor5.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor6
            // 
            lblColor6.BackColor = System.Drawing.Color.Thistle;
            lblColor6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor6.Location = new System.Drawing.Point(174, 25);
            lblColor6.Name = "lblColor6";
            lblColor6.Size = new System.Drawing.Size(12, 12);
            lblColor6.TabIndex = 28;
            lblColor6.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor7
            // 
            lblColor7.BackColor = System.Drawing.Color.Pink;
            lblColor7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblColor7.Location = new System.Drawing.Point(196, 25);
            lblColor7.Name = "lblColor7";
            lblColor7.Size = new System.Drawing.Size(12, 12);
            lblColor7.TabIndex = 29;
            lblColor7.Click += new System.EventHandler(lblColor_Click);
            // 
            // lblColor8
            // 
            //lblColor8.BackColor = System.Drawing.Color.White;
            //lblColor8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //lblColor8.Location = new System.Drawing.Point(198, 25);
            //lblColor8.Name = "lblColor8";
            //lblColor8.Size = new System.Drawing.Size(12, 12);
            //lblColor8.TabIndex = 31;
            //lblColor8.Click += new System.EventHandler(lblColor_Click);

            //panel.Controls.Add(this.lblColor8);
            panel.Controls.Add(this.lblColorReset);
            panel.Controls.Add(this.lblColor7);
            panel.Controls.Add(this.lblColor6);
            panel.Controls.Add(this.lblColor5);
            panel.Controls.Add(this.lblColor4);
            panel.Controls.Add(this.lblColor3);
            panel.Controls.Add(this.lblColor2);
            panel.Controls.Add(this.lblColor1);
            panel.Controls.Add(btnok);
            panel.PerformLayout();

            selectColor.Add("lblColorReset", Color.Empty);
        }
        /// <summary>
        /// 在调用本menu.show()方法后，应调用RedrawSelectedColor()方法重绘选择框
        /// </summary>
        public void RedrawSelectedColor()
        {
            foreach (string ckey in selectColor.Keys)
            {
                Label lbltmp = (Label)panel.Controls[ckey];
                drawBorder(lbltmp, true);
            }
        }
        private void lblColor_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                switch(((Label)sender).Name)
                {
                    case "lblColorReset":
                        
                        foreach(string ckey in selectColor.Keys)
                        {
                            Label lbltmp = (Label)panel.Controls[ckey];
                            drawBorder(lbltmp, false);
                        }
                        drawBorder(lblColorReset, true);
                        selectColor.Clear();
                        selectColor.Add("lblColorReset", Color.Empty);
                        break;
                    default:
                        if (selectColor.ContainsKey("lblColorReset"))
                        {
                            drawBorder(lblColorReset, false);
                            selectColor.Remove("lblColorReset");
                        }
                        if (selectColor.ContainsKey(((Label)sender).Name))
                        {
                            selectColor.Remove(((Label)sender).Name);
                            drawBorder(((Label)sender), false);
                        }
                        else
                        {
                            selectColor.Add(((Label)sender).Name, ((Label)sender).BackColor);
                            drawBorder(((Label)sender), true);
                        }
                        if (selectColor.Count==0)
                        {
                            drawBorder(lblColorReset, true);
                            selectColor.Add("lblColorReset", Color.Empty);
                        }
                        break;
                }

            }
        }

        /// <summary>
        /// 绘制选择框
        /// </summary>
        /// <param name="l"></param>
        /// <param name="selected"></param>
        private void drawBorder(Label l,bool selected)
        {
            Pen p;
            //选择
            if(selected)
            {
                p = new Pen(Color.Blue);
            }
            //取消选择
            else
            {
                p = new Pen(panel.BackColor);
            }
            p.Width = 1;
            Graphics g = panel.CreateGraphics();

            Rectangle myRectangle = new Rectangle(l.Left - 1, l.Top - 1, l.Width + 1, l.Height + 1);
            g.DrawRectangle(p, myRectangle);
        }
        private void btnok_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.AddExtension = true;
            s.DefaultExt = "ARList";
            s.Filter = "分析结果列表文件(*.ARList)|*.ARList";
            if (((TabPageForList)this.Master.Master).useARListFile)
            {
                int splitb = ((TabPageForList)this.Master.Master).ARListFileName.LastIndexOf("\\");
                int spliti = ((TabPageForList)this.Master.Master).ARListFileName.LastIndexOf("_");
                s.FileName = ((TabPageForList)this.Master.Master).ARListFileName.Substring(splitb + 1, spliti - splitb - 1) + "_" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"); ;
            }
            else
            {
                s.FileName = ((TabPageForList)this.Master.Master).Parameters[0].DateStart.Substring(0, 10) + "~" + ((TabPageForList)this.Master.Master).Parameters[0].DateEnd.Substring(0, 10) + "_" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            }
            s.FileName = s.FileName.Replace(":", "");
            s.FileName = s.FileName.Replace("-", "");
            s.OverwritePrompt = true;
            DialogResult result = s.ShowDialog();
            if (result == DialogResult.OK)
            {
                Color[] colors = new Color[selectColor.Count];
                int i = 0;
                foreach(string key in selectColor.Keys)
                {
                    colors[i] = selectColor[key];
                    i++;
                }
                FileStream fs = new FileStream(s.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sr = new StreamWriter(fs);
                sr.Write(((TabPageForList)this.Master.Master).SerializeAnalyzeResults(colors));
                sr.Close();
                fs.Close();
                this.Hide();
                this.Master.selectMenu = "";
                //this.Master.Master.Parent.Show();
            }


        }
    }
}
