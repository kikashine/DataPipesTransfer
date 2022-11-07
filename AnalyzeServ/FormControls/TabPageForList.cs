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
    /// <summary>
    /// 带数据列表的TabPage
    /// </summary>
    public partial class TabPageForList : System.Windows.Forms.TabPage
    {
        //线程中执行方法的委托
        protected delegate void setDataGridViewInvoke(int orgIndex, List<object> ResultList, List<string> ColumnName, List<string> ColumnText, List<string> ColumnSort, List<int> ColumnSize);
        protected delegate void changeButtonTextInvoke(Button btn, string text);
        protected delegate void labelTextInvoke(Label lbl, string text);

        public List<AnalyzeParameters> Parameters;

        public bool useARListFile;

        public string ARListFileName;

        private JobAnalyze ja;

        private int didcount = 0;

        private Dictionary<string, JobBase> jobs;
        /// <summary>
        /// 对所有产生的tabpage计数
        /// </summary>
        protected int tabcount;

        //public Dictionary<string, KBase> kbset;

        public Dictionary<string, StockDataSet> sdatasets;
        /// <summary>
        /// 每个字段对应的排序类型
        /// </summary>
        protected List<string> ColumnSort;

        protected Dictionary<Color, List<DataGridViewCell>> coloredCells;

        public DataGridView DgvAnalyzRslt
        {
            get
            {
                return dgvAnalyzRslt;
            }
        }

        public FormKLineGraphic formK
        {
            get
            {
                return FormK;
            }
        }

        public string selectedStock
        {
            get
            {
                if(dgvAnalyzRslt.SelectedCells.Count>0)
                {
                    return dgvAnalyzRslt.Rows[dgvAnalyzRslt.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                }
                else
                {
                    return "";
                }

            }
        }

        public TabPageForList(List<AnalyzeParameters> Params, ref int tabcount,TabControl container, ref Dictionary<string, JobBase> jobs)
            : base()
        {
            InitializeComponent();
            this.jobs = jobs;
            this.ja = (JobAnalyze)jobs[tabcount.ToString()];
            ja.CountChanged += new JobBase.CountChangedEventHandler(ja_CountChanged);
            //ja.DataUpdated += new JobBase.DataUpdatedEventHandler(ja_DataUpdated);
            ja.Finished += new JobBase.FinishedEventHandler(ja_Finished);
            ja.HasResult += new JobBase.HasResultEventHandler(ja_HasResult);
            ja.Message += new JobBase.MessageEventHandler(ja_Message);
            ja.Stopped += new JobBase.StoppedEventHandler(ja_Stopped);
            ja.Started += new JobBase.StartedEventHandler(ja_Started);
            this.sdatasets = ja.sdatasets;
            
            Parameters = Params;
            
            ColumnSort = new List<string>();
            coloredCells = new Dictionary<Color, List<DataGridViewCell>>();
            this.container = container;
            string DateStart = Params[0].DateStart;
            string DateEnd = Params[0].DateEnd;
            labelDate.Text = "日期：" + DateStart.Substring(0, 10) + " - " + DateStart.Substring(0, 10) + " - " + DateEnd.Substring(0, 10);

            
            useARListFile = ja.UseARListFile;
            ARListFileName = ja.ARListFile;

            
        }

        void ja_Started(bool started)
        {
            //started();
        }

        void ja_Stopped(bool stopped)
        {
            this.stopped();
        }

        void ja_Message(string msg)
        {
            throw new NotImplementedException();
        }

        void ja_HasResult(Analyze Ananalyze, int orgindex)
        {
            hasResult(Ananalyze, orgindex);
        }

        void ja_Finished(bool finished)
        {
            this.finished();
        }

        //void ja_DataUpdated(string newtime)
        //{
        //   base.dataUpdated(newtime);
        //}

        void ja_CountChanged(JobBase jb, int count)
        {
            countChanged(jb, count);
        }
        //void ja_DataUpdated(string newtime)
        //{
        //   base.dataUpdated(newtime);
        //}

        protected void hasResult(Analyze Ananalyze, int orgindex)
        {
            didcount++;
            setDataGridView(orgindex, Ananalyze.ResultList, Ananalyze.ColumnName, Ananalyze.ColumnText, Ananalyze.ColumnSort, Ananalyze.ColumnSize);
        }

        protected void countChanged(JobBase jb, int count)
        {
            if(count==0)
            {
                didcount = 0;
            }

            labelTextInvoke ivkldt = new labelTextInvoke(changelabelDateText);
            this.BeginInvoke(ivkldt, new Object[] { this.labelPgsNow, "进度：" + didcount + ", " + count + @"/" + jb.totalCountJob });
        }

         protected void dgvAnalyzRslt_KeyDown(object sender, KeyEventArgs e)
        {
            if (findStockByKeyInput(e.KeyValue))
            {
            }
            //findStockByKeyInput返回false时，说明按下了回车键且dgvAnalyzRslt不应换行
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 通过按键输入股票代码并在按回车键后在dgvAnalyzRslt寻找定位对应股票数据
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns>按下了回车键且不希望换行</returns>
        public bool findStockByKeyInput(int KeyValue)
        {
            //只响应数字、退格、回车，8是退格键，13是回车键
            if (((KeyValue >= 96 && KeyValue <= 105) || KeyValue == 8 || KeyValue == 13))
            {
                //输入的是数字并且已输入长度小于6个数字，允许继续输入数字
                if (KeyValue >= 96 && KeyValue <= 105 && labelKBInput.Text.Length < 11)
                {
                    labelKBInput.Text += KeyValue - 96;
                    return true;
                }
                //输入退格键，且已经输入过数字，将最后一个输入的数字删除
                else if (KeyValue == 8 && labelKBInput.Text.Length > 5)
                {
                    labelKBInput.Text = labelKBInput.Text.Remove(labelKBInput.Text.Length - 1);
                    return true;
                }
                //输入回车键，且已经输入6位数字，查找与输入数字对应的股票数据行
                else if (labelKBInput.Text.Length == 11 && KeyValue == 13)
                {
                    for (int i = 0; i <= dgvAnalyzRslt.Rows.Count - 1; i++)
                    {
                        //找到与输入数字对应的股票数据行
                        if (labelKBInput.Text.Substring(5) == dgvAnalyzRslt.Rows[i].Cells[0].Value.ToString())
                        {
                            //清除已选单元格
                            for (int j = 0; j <= dgvAnalyzRslt.SelectedCells.Count - 1; j++)
                            {
                                dgvAnalyzRslt.SelectedCells[j].Selected = false;
                            }
                            //选择查找到的股票数据行第一个单元格
                            dgvAnalyzRslt.Rows[i].Cells[0].Selected = true;
                            //单元格焦点放在查找到的股票数据行第一个单元格
                            dgvAnalyzRslt.CurrentCell = dgvAnalyzRslt.Rows[i].Cells[0];
                            //将查找到的股票数据行第一个单元格显示在显示区域第一行
                            dgvAnalyzRslt.FirstDisplayedCell = dgvAnalyzRslt.Rows[i].Cells[0];
                            //清除输入的数字
                            labelKBInput.Text = labelKBInput.Text.Remove(5);
                            return false;
                        }
                    }
                    //为了避免按下回车键被datagridview判断并向下移动一个单元格，将KeyDown事件设置为已经处理完毕，则datagridview不会再自动判断
                    return false;
                }
                //未输满6位数时按回车键时，将KeyDown事件设置为已经处理完毕，防止焦点向下移动一个单元格，表示输入有误
                else if (labelKBInput.Text.Length > 5  && labelKBInput.Text.Length < 11 && KeyValue == 13)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            return true;
        }

        protected void dgvAnalyzRslt_KeyPress(object sender, KeyPressEventArgs e)
        {
            //textBox非数字和退格不响应，8是退格键
            if (((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == 8 || e.KeyChar == 13))
            {

                if (e.KeyChar >= '0' && e.KeyChar <= '9' && labelKBInput.Text.Length < 11)
                {
                    labelKBInput.Text += e.KeyChar;
                }
                else if (e.KeyChar == 8)
                {
                    if (labelKBInput.Text.Length > 5)
                    {
                        labelKBInput.Text = labelKBInput.Text.Remove(labelKBInput.Text.Length - 1);
                    }
                }
                else if (labelKBInput.Text.Length == 11 && e.KeyChar == 13)
                {
                    for (int i = 0; i <= dgvAnalyzRslt.Rows.Count - 1; i++)
                    {
                        if (labelKBInput.Text.Substring(5) == dgvAnalyzRslt.Rows[i].Cells[0].Value.ToString())
                        {
                            for (int j = 0; j <= dgvAnalyzRslt.SelectedCells.Count - 1; j++)
                            {
                                dgvAnalyzRslt.SelectedCells[j].Selected = false;
                            }
                            dgvAnalyzRslt.Rows[i].Cells[0].Selected = true;
                            dgvAnalyzRslt.FirstDisplayedCell = dgvAnalyzRslt.Rows[i].Cells[0];
                            labelKBInput.Text = labelKBInput.Text.Remove(5);
                        }
                    }
                    e.KeyChar=new char();
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        
        protected void btnanalyze_Click(object sender, EventArgs e)
        {
             if (!ja.isWorking)
            {
                this.labelPgsNow.Text = "";
                
                FormK.Close();
                FormK = new FormKLineGraphic();
                FormK.mainform = (Form)this.Parent.Parent;
                //设置Owner可以使FormK始终浮动在OwnerForm之上，且OwnerForm也可以获得焦点
                FormK.Owner = (Form)this.Parent.Parent;
                formK.tabpage = this;

                this.dgvAnalyzRslt.Rows.Clear();
                this.coloredCells.Clear();
                ((Button)sender).Text = "取消分析";
                ja.Start();
                //Start();
            }
            //选择的tab正处于启动工作线程状态中
            else
            {
                ja.Stop();
                //Stop();
                //Thread.Sleep(100);
                ((Button)sender).Text = "分析";

            }
        }

        /// <summary>
        /// 调用分析主进程的虚函数，为方便定义在本类中的方法(例如btnanalyze_Click)调用而定义为虚函数
        /// 本函数实际执行内容在继承类中进行重载
        /// </summary>
        public virtual void Start()
        {

        }

        public virtual void Stop()
        {

        }

        protected void started()
        {

        }

        protected void finished()
        {
            changeButtonTextInvoke ivk = new changeButtonTextInvoke(changeButtonText);
            this.BeginInvoke(ivk, new Object[] { btnanalyze, "分析" });
        }

        protected void stopped()
        {
            changeButtonTextInvoke ivk = new changeButtonTextInvoke(changeButtonText);
            this.BeginInvoke(ivk, new Object[] { btnanalyze, "分析" });
        }

        /// <summary>
        /// 为DataGridView添加一条数据，并对列进行设置
        /// </summary>
        /// <param name="ResultList"></param>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnText"></param>
        /// <param name="ColumnSort"></param>
        /// <param name="ColumnSize"></param>
        /// <returns>返回本次添加的数据在DataGridView中原始数据的索引值，该值为原始数据的添加顺序，不因排序改变</returns>
        protected void setDataGridView(int orgIndex, List<object> ResultList, List<string> ColumnName, List<string> ColumnText, List<string> ColumnSort, List<int> ColumnSize)
        {
            if (this.dgvAnalyzRslt.InvokeRequired) //用委托来操作
            {
                setDataGridViewInvoke sd = new setDataGridViewInvoke(setDataGridView);
                IAsyncResult IAR = this.BeginInvoke(sd, new object[] {orgIndex,  ResultList, ColumnName, ColumnText, ColumnSort, ColumnSize });

                //IAR.AsyncState
                //return (int)(this.EndInvoke(IAR));
            }
            else
            {
                //当前插入的数据在dgvAnalyzRslt数据列表中的索引值
                int index = -1;
                if (ResultList.Count > 0)
                {
                    //第一次添加数据，建立列信息
                    if (dgvAnalyzRslt.Columns.Count == 0)
                    {
                        this.ColumnSort = ColumnSort;
                        for (int j = 0; j < ColumnName.Count; j++)
                        {
                            dgvAnalyzRslt.Columns.Add(ColumnName[j], ColumnText[j]);
                            if (ColumnName[j] == "市场")
                            {
                                dgvAnalyzRslt.Columns[this.dgvAnalyzRslt.Columns.Count - 1].Visible = false;
                            }
                        }
                        this.setDGViewColumnSize(ColumnSize);

                        //为dgvAnalyzRslt添加一列原始数据索引列，该列值按照数据添加顺序赋值，排序后不变，以便与源数据建立对应关系。
                        this.dgvAnalyzRslt.Columns.Add("originalIndex", "originalIndex");
                        this.dgvAnalyzRslt.Columns[this.dgvAnalyzRslt.Columns.Count - 1].Width = 0;
                        this.dgvAnalyzRslt.Columns[this.dgvAnalyzRslt.Columns.Count - 1].Visible = false;
                        
                    }
                    //为此条数据在最后添加一个顺序值作为源数据索引，对应originalIndex列
                    ResultList.Add(orgIndex);
                    //lock (dgvAnalyzRslt.Rows)
                    //{
                        dgvAnalyzRslt.Rows.Add(ResultList.ToArray());
                        //index = dgvAnalyzRslt.Rows.Count - 1;
                    //}
                }
                //return index;
            }

        }

        protected void changeButtonText(Button btn, string text)
        {
            btn.Text = text;
        }

        protected void changelabelDateText(Label lbl, string text)
        {
            lbl.Text = text;
        }

        private void btnclosetp_Click(object sender, EventArgs e)
        {
            this.cMenus.Dispose();
            ((TabControl)this.Parent).SelectTab(((TabControl)this.Parent).SelectedIndex - 1);
            ja.Stop();
            jobs.Remove(this.Name);
            ja = null;
            this.Dispose();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point tp = this.PointToScreen(new Point(btn.Left, btn.Top));
            Point p = new Point(tp.X + 18, tp.Y);
            if(!cMenus.initialized)
            {
                cMenus.init();
            }
            cMenus.Show(p);
        }

        void TabPageForList_Disposed(object sender, EventArgs e)
        {
            FormK.Close();
        }

        private void setDGViewColumnSize(List<int> ColumnSize)
        {
            for (int i = 0; i < this.dgvAnalyzRslt.Columns.Count - 1; i++)
            {
                if (i < ColumnSize.Count)
                {
                    this.dgvAnalyzRslt.Columns[i].Width = ColumnSize[i];
                }
            }
        }

        private void cbShowKLineGraphic_CheckedChanged(object sender, EventArgs e)
        {
            
            if (((CheckBox)sender).Checked && this.dgvAnalyzRslt.SelectedCells.Count > 0)
            {
                FormK.Close();
                FormK.Dispose();
                FormK = new FormKLineGraphic();
                FormK.mainform = (Form)this.Parent.Parent;
                //设置Owner可以使FormK始终浮动在OwnerForm之上，且OwnerForm也可以获得焦点
                FormK.Owner = (Form)this.Parent.Parent;
                formK.tabpage = this;
                //得到存放在dgvAnalyzRslt选中行最后一列的在原始数据中的索引值，此索引值与绘图数据列表中的索引值对应
                int index = (int)this.dgvAnalyzRslt.SelectedCells[0].OwningRow.Cells[this.dgvAnalyzRslt.SelectedCells[0].OwningRow.Cells.Count - 1].Value;

                //如果本次选定绘图数据与已绘图数据不同，为绘图Form的绘图数据赋值并重新绘图
                if (FormK.indexOfDataForDrawKLine != index && ja.datasForDrawkGraphic.ContainsKey(index))
                {
                    FormK.arfd = ((AnalyzeResultForDraw)ja.datasForDrawkGraphic[index]);
                    FormK.indexOfDataForDrawKLine = index;
                    FormK.Show();
                    FormK.Location = new Point(((Form)this.Parent.Parent).Location.X + ((Form)this.Parent.Parent).Width, ((Form)this.Parent.Parent).Location.Y);
                    FormK.DrawGraphic();
                    this.dgvAnalyzRslt.Focus();

                }
                else if (ja.datasForDrawkGraphic.ContainsKey(index))
                {
                    FormK.Visible = true;
                    FormK.Location = new Point(((Form)this.Parent.Parent).Location.X + ((Form)this.Parent.Parent).Width, ((Form)this.Parent.Parent).Location.Y);
                    this.dgvAnalyzRslt.Focus();
                }
            }
            else if (!((CheckBox)sender).Checked)
            {
                FormK.Visible=false;
            }
        }

        private void dgvAnalyzRslt_SelectionChanged(object sender, EventArgs e)
        {
            
            if (cbShowKLineGraphic.Checked && this.dgvAnalyzRslt.SelectedCells.Count > 0)
            {
                

                //得到存放在dgvAnalyzRslt选中行最后一列的在原始数据中的索引值，此索引值与绘图数据列表中的索引值对应
                int index = (int)this.dgvAnalyzRslt.SelectedCells[0].OwningRow.Cells[this.dgvAnalyzRslt.SelectedCells[0].OwningRow.Cells.Count-1].Value;

                //如果本次选定绘图数据与已绘图数据不同，为绘图Form的绘图数据赋值并重新绘图
                if (FormK.indexOfDataForDrawKLine != index && ja.datasForDrawkGraphic.ContainsKey(index))
                {

                    FormK.arfd = ((AnalyzeResultForDraw)ja.datasForDrawkGraphic[index]);
                    FormK.indexOfDataForDrawKLine = index;
                    FormK.Show();
                    FormK.Location = new Point(((Form)this.Parent.Parent).Location.X + ((Form)this.Parent.Parent).Width, ((Form)this.Parent.Parent).Location.Y);
                    FormK.DrawGraphic();

                    this.dgvAnalyzRslt.Focus();

                }
                else if (ja.datasForDrawkGraphic.ContainsKey(index))
                {
                    FormK.Visible = true;
                    FormK.Location = new Point(((Form)this.Parent.Parent).Location.X + ((Form)this.Parent.Parent).Width, ((Form)this.Parent.Parent).Location.Y);
                    this.dgvAnalyzRslt.Focus();
                }
            }
            
            if (this.dgvAnalyzRslt.SelectedRows.Count > 0)
            {
                this.labelSelectRowsCount.Text = "选择股票数：" + this.dgvAnalyzRslt.SelectedRows.Count.ToString();
            }
            else
            {
                int count = 0;
                for (int i = 0; i < this.dgvAnalyzRslt.SelectedCells.Count; i++)
                {
                    if (this.dgvAnalyzRslt.SelectedCells[i].ColumnIndex == 0)
                    {
                        count++;
                    }
                }
                this.labelSelectRowsCount.Text = "选择股票数：" + count.ToString();
            }
        }

        private void dgvAnalyzRslt_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            switch (this.ColumnSort[e.Column.Index])
            {
                case "num":
                    e.SortResult = (Convert.ToDouble(e.CellValue1) - Convert.ToDouble(e.CellValue2) > 0) ? 1 : (Convert.ToDouble(e.CellValue1) - Convert.ToDouble(e.CellValue2) < 0) ? -1 : 0;
                    break;
                case "str":
                    e.SortResult = System.String.Compare(Convert.ToString(e.CellValue1), Convert.ToString(e.CellValue2));
                    break;
                case "per":
                    e.SortResult = (Convert.ToDouble(e.CellValue1.ToString().TrimEnd('%')) - Convert.ToDouble(e.CellValue2.ToString().TrimEnd('%')) > 0) ? 1 : (Convert.ToDouble(e.CellValue1.ToString().TrimEnd('%')) - Convert.ToDouble(e.CellValue2.ToString().TrimEnd('%')) < 0) ? -1 : 0;
                    break;
                default:
                    break;
            }
            e.Handled = true;
        }

        public void changeSize(TabControl container)
        {
            this.dgvAnalyzRslt.Height = container.Size.Height - 90;
            this.dgvAnalyzRslt.Width = container.Size.Width - 30;
            this.btnanalyze.Left = container.Size.Width - 115;
            this.btnclosetp.Left = container.Size.Width - 30;
        }

        public void showMenuForm()
        {
            cMenus.showMenuForm();
        }
        public void hideMenuForm()
        {
            cMenus.hideMenuForm();
        }

        public void showFormK()
        {
            if (FormK.mainform == null || FormK.mainform != (Form)this.Parent.Parent)
            {
                return;
            }
            if (cbShowKLineGraphic.Checked && this.dgvAnalyzRslt.SelectedCells.Count > 0)
            {
                FormK.TopMost = true;
                FormK.DrawGraphicFunc(Graphics.FromHwnd(FormK.Handle));
                FormK.TopMost = false;
                this.dgvAnalyzRslt.Focus();

            }
        }

 
 
        /// <summary>
        /// 将分析结果及附加的信息序列化为一个字符串，以便存储到文件
        /// </summary>
        /// <returns></returns>
        public string SerializeAnalyzeResults(Color[] colors)
        {

            StringBuilder serialized1 = new StringBuilder();
            THashTable<StringBuilder> s1 = new THashTable<StringBuilder>();
            int[] idxs = new int[1];
            if(colors[0]==Color.Empty)
            {
                idxs = new int[dgvAnalyzRslt.Rows.Count];
                for (int i = 0; i <= dgvAnalyzRslt.Rows.Count - 1; i++)
                {
                    idxs[i] = i;
                }
            }
            else
            {
                int tmpcount = 0;
                for(int i=0;i<=colors.Length-1;i++)
                {
                    tmpcount += coloredCells[colors[i]].Count;
                }
                idxs = new int[tmpcount];
                tmpcount = 0;
                for (int i = 0; i <= colors.Length - 1; i++)
                {
                    for (int j = 0; j <= coloredCells[colors[i]].Count - 1; j++)
                    {
                        idxs[tmpcount] = (int)coloredCells[colors[i]][j].OwningRow.Cells["originalIndex"].Value;
                        tmpcount++;
                    }
                }
            }
            for (int i = 0; i <= idxs.Length - 1;i++ )
            {
                ///
                ///此处应按修改后工作对象的绘图数据进行处理
                ///
                AnalyzeResultForDraw arfd = (AnalyzeResultForDraw)ja.datasForDrawkGraphic[idxs[i]];
                string key = arfd.stock["StockCode"].ToString();
                if (!s1.ContainsKey(key))
                {
                    s1.Add(key, new StringBuilder("\r\n"));
                }
                s1[key].Append("<row>\r\n");
                s1[key].Append("<arfd.id>" + arfd.id + "</arfd.id>\r\n");
                s1[key].Append("<arfd.AnalyzeDay>" + arfd.AnalyzeDay + "</arfd.AnalyzeDay>\r\n");
                s1[key].Append("<arfd.AnalyzeDayIdx>" + arfd.AnalyzeDayIdx + "</arfd.AnalyzeDayIdx>\r\n");
                s1[key].Append("<arfd.SqlStartDay>" + arfd.SqlStartDay + "</arfd.SqlStartDay>\r\n");
                s1[key].Append("<arfd.StockCode>" + arfd.stock["StockCode"].ToString() + "</arfd.StockCode>\r\n");
                s1[key].Append("<arfd.MarketCode>" + arfd.stock["MarketCode"].ToString() + "</arfd.MarketCode>\r\n");
                s1[key].Append("<arfd.Industry>" + arfd.stock["Industry"].ToString() + "</arfd.Industry>\r\n");
                s1[key].Append("<arfd.DataSource>" + arfd.DataSource + "</arfd.DataSource>\r\n");
                s1[key].Append("</row>\r\n");
            }
            for (int i=0; i < s1.Count; i++)
            {
                serialized1.Append(s1[i]);
            }
            
            return serialized1.ToString();
        }

        /// <summary>
        /// 为所选单元格设置颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblColor_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                if (((Label)sender).Name == "lblColorReset")
                {
                    foreach (Color ckey in coloredCells.Keys)
                    {
                        foreach (DataGridViewCell pkey in coloredCells[ckey])
                        {
                            this.dgvAnalyzRslt.Rows[pkey.RowIndex].Cells[pkey.ColumnIndex].Style.BackColor = ((Label)sender).BackColor;
                        }
                        coloredCells[ckey].Clear();
                    }
                }
                else
                {
                    for (int i = 0; i < this.dgvAnalyzRslt.SelectedCells.Count; i++)
                    {
                        this.dgvAnalyzRslt.SelectedCells[i].Style.BackColor = ((Label)sender).BackColor;
                        coloredCell_add(((Label)sender).BackColor, this.dgvAnalyzRslt.SelectedCells[i]);
                    }
                }
            }
        }

        private void coloredCell_add(Color c,DataGridViewCell cell)
        {
            int idx=0;
            //DataGridViewCell p= new Point(cell.ColumnIndex,cell.RowIndex);
            foreach(Color ckey in coloredCells.Keys)
            {
                idx = coloredCells[ckey].FindIndex(
                    delegate(DataGridViewCell tmpp)
                    {
                        //return user.UserID == 0;  

                        return tmpp.ColumnIndex == cell.ColumnIndex && tmpp.RowIndex == cell.RowIndex;


                    });
                if(idx>=0)
                {
                    coloredCells[ckey].RemoveAt(idx);
                    break;
                }
            }
            if (!coloredCells.ContainsKey(c))
            {
                coloredCells.Add(c, new List<DataGridViewCell>());
            }
            coloredCells[c].Add(cell);
            
        }
        ///
        //
        /// <summary>
        /// 开始拖拽动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAnalyzRslt_DragEnter(object sender, DragEventArgs e)
        {
            //拖拽效果
            e.Effect = DragDropEffects.Scroll | DragDropEffects.Move;
        }
 
        /// <summary>
        /// DragOver响应事件的方法中控制拖动行时datagrid的滚动条随鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAnalyzRslt_DragOver(object sender, DragEventArgs e)
        {
            //根据鼠标距datagrid上下沿距离决定滚动速度
            int up = e.Y - PointToScreen(new Point(dgvAnalyzRslt.Location.X, dgvAnalyzRslt.Location.Y)).Y;
            int down = e.Y - PointToScreen(new Point(dgvAnalyzRslt.Location.X, dgvAnalyzRslt.Location.Y + dgvAnalyzRslt.Height)).Y;
            int waittimeup = 0;
            int waittimedown = 0;

            if (up >= 15 && up <= 30)
            {
                waittimeup = 1;
            }
            if (up > 30 && up <= 45)
            {
                waittimeup = 80;
            }

            if (up > 45 && up <= 60)
            {
                waittimeup = 180;
            }

            if (down >= -15)
            {
                waittimedown = 1;
            }
            if (down < -15 && down >= -30)
            {
                waittimedown = 80;
            }

            if (down < -30 && down >= -45)
            {
                waittimedown = 180;
            }

            //上下滚动响应区域重合时不滚动
            if (waittimedown > 0 && waittimeup > 0)
            {
                return;
            }
            if (waittimeup > 0)
            {
                if (dgvAnalyzRslt.FirstDisplayedScrollingRowIndex > 0)
                {
                    dgvAnalyzRslt.FirstDisplayedScrollingRowIndex -= 1;
                }
                Thread.Sleep(waittimeup);
            }
            if (waittimedown > 0)
            {
                if (dgvAnalyzRslt.FirstDisplayedScrollingRowIndex < dgvAnalyzRslt.Rows.Count)
                {
                    dgvAnalyzRslt.FirstDisplayedScrollingRowIndex += 1;
                }
                Thread.Sleep(waittimedown);
            }

        }

        /// <summary>
        /// 鼠标在datagrid上移动时的处理方法，判断是否鼠标左键单击并移动时产生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAnalyzRslt_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.Clicks < 2) && (e.Button == MouseButtons.Left))
            {
                if ((e.ColumnIndex == -1) && (e.RowIndex > -1))
                {
                    //开始拖拽
                    dgvAnalyzRslt.DoDragDrop(dgvAnalyzRslt.SelectedRows, DragDropEffects.Scroll | DragDropEffects.Move);
                    
                }
            }
        }

        /// <summary>
        /// 中间变量。记录拖拽的行号
        /// </summary>
        int selectionIdx = -1;
        int selectionCount = 0;


        private void dgvAnalyzRslt_DragDrop(object sender, DragEventArgs e)
        {
            int idx = GetRowFromPoint(e.X, e.Y);
            if (idx < 0) return;

            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                int i = -1;
                DataGridViewSelectedRowCollection rows = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));
                //移除鼠标拖拽目的地所在行，并将拖拽过来的行插入，其它行顺移
                foreach (DataGridViewRow row in rows)
                {
                    if (dgvAnalyzRslt.Rows.IndexOf(row) < idx)
                    {
                        i++;
                    }
                    dgvAnalyzRslt.Rows.Remove(row);
                }
                if (i > 0)
                {
                    idx = idx - i;
                }
                i = 0;
                foreach (DataGridViewRow row in rows)
                {
                    dgvAnalyzRslt.Rows.Insert(idx + i, row);
                    i++;
                }

                selectionIdx = idx;
                selectionCount = i;
                dgvAnalyzRslt.ClearSelection();
                DragDropedReSelect();
            }
        }
        /// <summary>
        /// 从鼠标位置得到鼠标所在行号
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int GetRowFromPoint(int x, int y)
        {
            for (int i = 0; i < dgvAnalyzRslt.RowCount; i++)
            {
                Rectangle rec = dgvAnalyzRslt.GetRowDisplayRectangle(i, false);
                if (dgvAnalyzRslt.RectangleToScreen(rec).Contains(x, y))
                    return i;
            }
            return -1;

        }

        /// <summary>
        /// 拖动行后需要重新选择拖动的行
        /// </summary>
        private void DragDropedReSelect()
        {
            if (selectionIdx >= 0)
            {
                if (dgvAnalyzRslt.Rows.Count <= selectionIdx)
                    selectionIdx = dgvAnalyzRslt.Rows.Count - 1;
                for (int i = 0; i < selectionCount; i++)
                {
                    dgvAnalyzRslt.Rows[selectionIdx + i].Selected = true;
                }
                selectionIdx = -1;
            }
        }
   }
}
