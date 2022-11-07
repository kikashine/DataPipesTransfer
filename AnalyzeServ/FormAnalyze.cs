using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public partial class FormAnalyze : Form
    {
        private delegate void ctrlTextInvoke(Control ctrl, string text);
        /// <summary>
        /// 累计新生成过的tabpage数
        /// </summary>
        private int tabcount;

        /// <summary>
        /// 选择、设置需要分析的模型，并生成相应的参数
        /// </summary>
        public AnalyzeModelSelection AMS;
        public string lastUpdateDataTime;
        /// <summary>
        /// 上一次选中的tcAnalyze中的tab对应的索引
        /// </summary>
        private int lastSelectedTabIndex = 0;

        private Dictionary<string, JobBase> jobs;

        private Dictionary<string, StockDataSet> sdatasets;
        //private Dictionary<string, stoc> kbset;

        private Dictionary<StockAndDate, ReferenceStockInfo> refsiset;

        public FormAnalyze()
        {
            InitializeComponent();
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "Main";
            }
            this.dtpStart.Value = DateTime.Now;
            this.dtpEnd.Value = DateTime.Now;
            AMS = new AnalyzeModelSelection();
            AMS.genAnalyzeSelection(this.tpAnalyze.Controls);
            this.tcAnalyze.ResumeLayout(false);
            this.tpAnalyze.ResumeLayout(false);
            this.tpAnalyze.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            tabcount = 1;
            lastUpdateDataTime = "1970-01-01 00:00:00";
            sdatasets = new Dictionary<string, StockDataSet>();
            //kbset = new Dictionary<string, KBase>();
            //KDayDataSets = new Dictionary<string, List<KDayData>>();
            //kpsets = new Dictionary<string, kPieceSet>();
            //mas = new Dictionary<string, MA>();
            //avgnamplists = new Dictionary<string, AvgNeighborAmpList>();
            refsiset = new Dictionary<StockAndDate, ReferenceStockInfo>();
            jobs = new Dictionary<string, JobBase>();

            //StockLists = Utility.GetStockList();
            //Hashtable stock = new Hashtable();
            //stock = Utility.GetStockht("600350");
            //string refdate = "2015-8-3";
            //PipeClient pc = new PipeClient();
            //rkb = pc.getData(rkb = new KBase(), stock);
            //refsi = new ReferenceStockInfo(rkb.RowIndexByDate(refdate), rkb);

        }

        /// <summary>
        /// 点击"分析"、"连续测试"按钮时进行的处理
        /// </summary>
        /// <param name="btn"></param>
        private void StartEndAnalyze(Button btn, JobType JType)
        {
            //在条件选择tab点击按钮

            //数据来源，0:同花顺数据文件，1:通达信复权数据文件
            int Source = 0; ;
            if (rbD1File.Checked)
            {
                Source = 0;
            }
            if (rbtdxfq.Checked)
            {
                Source = 1;
            }
            
            List<AnalyzeParameters> parameters = this.AMS.getParameters(JType, this.dtpStart.Value, this.dtpEnd.Value);
            jobs.Add(this.tabcount.ToString(), new JobAnalyze(parameters, ref sdatasets, ref refsiset));
            ((JobAnalyze)jobs[this.tabcount.ToString()]).UseARListFile = this.cbUseARListFile.Checked;
            ((JobAnalyze)jobs[this.tabcount.ToString()]).ARListFile = this.lblARListFile.Text;
            ((JobAnalyze)jobs[this.tabcount.ToString()]).OneStockCode = this.cbOneStockCode.Checked;
            ((JobAnalyze)jobs[this.tabcount.ToString()]).StockCode = this.tbStockCode.Text;
            TabPageForList newtab = new TabPageForList(parameters, ref this.tabcount, this.tcAnalyze, ref jobs);
            newtab.Name = this.tabcount.ToString();
            newtab.Text = this.tabcount.ToString();
            //newtab.DataUpdated += new TabPageWithMultiThreadBase_old.DataUpdatedEventHandler(newtab_DataUpdated);
            this.tcAnalyze.TabPages.Add(newtab);
            newtab.init();

            this.tcAnalyze.SelectTab(this.tcAnalyze.TabPages.Count - 1);
            jobs[tabcount.ToString()].Start();
            tabcount++;
            //newtab.Start();
        }

        private void ctrlText(Control ctrl, string text)
        {
            ctrl.Text = text;
        }

        private void FormAnalyze_Shown(object sender, EventArgs e)
        {
            this.tcAnalyze.Height = this.Size.Height - 55;
            this.tcAnalyze.Width = this.Size.Width - 20;
        }

        private void btnanalyze_Click(object sender, EventArgs e)
        {
            this.StartEndAnalyze(((Button)sender), JobType.Analyze);
        }

        private void lblShowOpenFileDiag_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogStockCodeTest = new System.Windows.Forms.OpenFileDialog();
            openFileDialogStockCodeTest.DefaultExt = "ARList";
            openFileDialogStockCodeTest.Filter = "分析结果列表文件(*.ARList)|*.ARList";
            openFileDialogStockCodeTest.InitialDirectory = "./";
            if (openFileDialogStockCodeTest.ShowDialog() == DialogResult.OK)
            {
                lblARListFile.Text = openFileDialogStockCodeTest.FileName;
            }
        }

        private void FormAnalyze_KeyPress(object sender, KeyPressEventArgs e)
        {
            //选择的TabPage非条件输入页面
            if (this.tcAnalyze.SelectedIndex > 0)
            {
                //TabPage.DgvAnalyzRslt具有焦点时，需要结束本事件的处理。
                //因为需要特殊的按键判断处理，TabPage.DgvAnalyzRslt也有一个股票代码输入判断处理，
                //Form.KeyPreview属性置为true时，本事件处理完成后，事件的处理流程会自动进行TabPage.DgvAnalyzRslt的按键事件处理，
                //因此会造成一次按键行为在不同事件处理两次。焦点在TabPage.DgvAnalyzRslt外时用本事件处理一次。
                if (((TabPageForList)(this.tcAnalyze.SelectedTab)).DgvAnalyzRslt.Focused)
                {
                    return;
                }
                int KeyValue = e.KeyChar;
                //KeyPressEventArgs的KeyChar内容为字符时，对应的int值与同字符的KeyEventArgs的KeyValue值相差48
                //退格键和回车键的值两者相同
                if (KeyValue != 8 && KeyValue != 13)
                {
                    KeyValue += 48;
                }
                ((TabPageForList)(this.tcAnalyze.SelectedTab)).findStockByKeyInput(KeyValue);
            }
        }

        private void FormAnalyze_Resize(object sender, EventArgs e)
        {
            this.tcAnalyze.Height = this.Size.Height - 55;
            this.tcAnalyze.Width = this.Size.Width - 20;
        }

        private void lblDtToleft_Click(object sender, EventArgs e)
        {
            dtpEnd.Value = dtpStart.Value;
        }

        private void lblDtToRight_Click(object sender, EventArgs e)
        {
            
            dtpStart.Value = dtpEnd.Value;
        }

        //private void process()
        //{
        //    ctrlTextInvoke ivkldt = new ctrlTextInvoke(ctrlText);
        //    PipeClient pc = new PipeClient();

        //    if (kbs == null || kbs.Length == 0)
        //    {
        //        kbs = new KBase[StockLists.Count];
        //        int count = 0;
        //        foreach (string stockcode in StockLists.Keys)
        //        {
        //            kbs[count] = pc.getData(kbs[count] = new KBase(), (Hashtable)StockLists[stockcode]);
        //            count++;
                    
        //            this.BeginInvoke(ivkldt, new Object[] { this.label1, "数据预读进度：" + count });
        //        }
        //    }

        //    int indexb = 0;
        //    int indexe = 0;
        //    for(int i=0;i<=kbs.Length-1;i++)
        //    {
        //        this.BeginInvoke(ivkldt, new Object[] { this.label1, "分析进度：" + (i+1) });
        //        DateTime dateb = DateTime.Parse("2013-1-1");
        //        DateTime datee = DateTime.Parse("2015-8-3");
        //        long t3 = DateTime.Now.Ticks;
        //        while(dateb.Ticks<=datee.Ticks)
        //        {
        //            indexb = kbs[i].RowIndexByDate(dateb.ToString("yyyy-MM-dd HH:mm:ss"));
        //            if(indexb>=0 && indexb!=kbs[i].Count)
        //            {
        //                break;
        //            }
        //            dateb = dateb.AddDays(1);
        //        }
        //        dateb = DateTime.Parse("2013-1-1");
        //        while (dateb.Ticks <= datee.Ticks)
        //        {
        //            indexe = kbs[i].RowIndexByDate(datee.ToString("yyyy-MM-dd HH:mm:ss"));
        //            if (indexe >= 0 && indexe != kbs[i].Count)
        //            {
        //                break;
        //            }
        //            datee = datee.AddDays(-1);
        //        }
        //        long t4 = DateTime.Now.Ticks;
        //        t4 = (t4 - t3) / 1000;
        //        if(indexb<0 || indexe<0 )
        //        {
        //            continue;
        //        }
        //        for(int j=indexb;j<=indexe;j++)
        //        {
        //            ModelTest modetest = new ModelTest(kbs[j].Stock, kbs[i],rkb, refsi);
        //            modetest.t = j;
        //        }
                
        //    }

        //    //ModelTest modetest = new ModelTest(stock, theDT, ref_kbpub, refsi);
        //    //modetest.t = indexAnalyze;
        //}

    }
}
