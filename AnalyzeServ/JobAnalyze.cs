using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class JobAnalyze:JobBase
    {

        private Dictionary<StockAndDate, ReferenceStockInfo> refsiset;

        //public Dictionary<string, KBase> kbset;
        public Dictionary<string, StockDataSet> sdatasets;

        public bool UseARListFile;

        public string ARListFile;

        public bool AnalyzeInTradding;

        public bool OneStockCode;

        public string StockCode;

        private string alock = "";

        public JobAnalyze(List<AnalyzeParameters> parameters, ref Dictionary<string, StockDataSet> sdatasets, ref Dictionary<StockAndDate, ReferenceStockInfo> refsiset)
            : base()
        {
            this.parameters = parameters;
            this.sdatasets = sdatasets;
            //this.refsiset = refsiset;
            this.refsiset = new Dictionary<StockAndDate, ReferenceStockInfo>();
            UseARListFile = false;
            ARListFile = "";
        }

        public override void Start()
        {
            this.upDateRefStockInfo();
            //StartAHostThread中调用本实例的startJob
            this.StartAHostThread();
        }

        /// <summary>
        /// 分割分析任务
        /// </summary>
        private void spliteJob()
        {
            this.spliteJob(this.UseARListFile, this.ARListFile, this.OneStockCode, this.StockCode);
        }

        protected override void startJob()
        {
            this.spliteJob();
            this.startJobThreads();
        }

        protected override void loopingDoJobFuncs(List<AnalyzeParameters> paramlist, Hashtable piece)
        {
            if (AnalyzeInTradding)
            {
                loopingInTradding(paramlist, piece);
            }
            else
            {
                loopingInNormal(paramlist, piece);
            }
        }

        /// <summary>
        /// 在交易时段中进行分析的循环处理
        /// </summary>
        /// <param name="Ananalyze"></param>
        /// <param name="StockList"></param>
        /// <param name="tab"></param>
        private void loopingInTradding(List<AnalyzeParameters> paramlist, Hashtable StockList)
        {
            int i = 0;

            Analyze Ananalyze = new Analyze();
            AnalyzeEnded state = new AnalyzeEnded();
            int countToGetInfoFromHttp = 0;
            Hashtable StockListSplited = new Hashtable();
            string cdlist = "";
            HttpStockInfoPostThread hsipt = new HttpStockInfoPostThread();
            foreach (string cd in StockList.Keys)
            {
                //为了从http得到股票信息以及信息的实时性，每40个股票分批进行分析
                if (countToGetInfoFromHttp == 40)
                {

                    hsipt.runsina(cdlist);
                    foreach (string cd1 in StockListSplited.Keys)
                    {
                        if (!this.isThreadding)
                        {
                            return;
                        }

                        //state = Ananalyze.doAnalyze(ref kbset, refsi, (StockInfoFromHttpReq)(hsipt.stockinfo[cd1]), (Hashtable)StockList[cd1], paramlist, DateTime.Parse(paramlist[0].DateStart), 0, state.LastIndexAnalyze, this.AnalyzeInTradding, Source);

                        //OnHasResultEvent(Ananalyze);

                        didCount++;
                        OnCountChangedEvent(this, didCount);
                        i++;
                    }
                    StockListSplited = new Hashtable();
                    countToGetInfoFromHttp = 0;
                    cdlist = "";
                    hsipt = new HttpStockInfoPostThread();
                }
                if (countToGetInfoFromHttp == 0)
                {
                    cdlist = cd;
                }
                else
                {
                    cdlist = cdlist + "," + cd;
                }
                StockListSplited.Add(cd, StockList[cd]);
                countToGetInfoFromHttp++;

            }
            //最后会有未满40个的部分未分析，进行分析
            if (countToGetInfoFromHttp > 0)
            {
                //Analyze Ananalyze = new Analyze();
                //AnalyzeEnded state = new AnalyzeEnded();
                hsipt.runsina(cdlist);
                foreach (string cd1 in StockListSplited.Keys)
                {

                    if (!this.isThreadding)
                    {
                        return;
                    }

                    //state = Ananalyze.doAnalyze(ref theDS, ref refsi, (StockInfoFromHttpReq)(hsipt.stockinfo[cd1]), (Hashtable)StockList[cd1], paramlist, DateTime.Parse(paramlist[0].DateStart), 0, state.LastIndexAnalyze, this.AnalyzeInTradding, Source);

                    //OnHasResultEvent(Ananalyze);
                    didCount++;
                    OnCountChangedEvent(this, didCount);
                    i++;
                }
                StockListSplited = new Hashtable();
                countToGetInfoFromHttp = 0;
            }

        }

        /// <summary>
        /// 非交易时段中进行分析的循环处理
        /// </summary>
        /// <param name="Ananalyze"></param>
        /// <param name="StockList"></param>
        /// <param name="tab"></param>
        private void loopingInNormal(List<AnalyzeParameters> paramlist, Hashtable StockList)
        {
            if (this.UseARListFile)
            {
                loopingInNormal_UseARListFile(paramlist, StockList);
            }
            else
            {
                loopingInNormal_Normal(paramlist, StockList);
            }

        }

        private void loopingInNormal_Normal(List<AnalyzeParameters> paramlist, Hashtable StockList)
        {


            foreach (string cd in StockList.Keys)
            {
                if (!this.isThreadding)
                {
                    return;
                }
                DateTime analyzeDay = DateTime.Parse(paramlist[0].DateStart);

                //lock(StockList[cd])
                //{
                    Hashtable stock = (Hashtable)StockList[cd];
                    StockDataSet _sds = upDateData(stock);
                    if(_sds.StockCode==null)
                    {
                        int debug4867 = 0;
                        lock (alock)
                        {
                            didCount++;
                        }
                        OnCountChangedEvent(this, didCount);
                        continue;
                    }
                    KBase _kb = new KBase(_sds);
                    DateTime dateb = DateTime.Parse(paramlist[0].DateStart);
                    DateTime datee = DateTime.Parse(paramlist[0].DateEnd);
                    //起始日对应的k线数据索引
                    int indexb = 0;
                    //结束日对应的k线数据索引
                    int indexe = 0;
                    //得到起始日对应的k线数据索引
                    //起始日可能不存在对应的数据，向后寻找最接近起始日的存在数据的日期索引，作为起始日索引
                    while (dateb.Ticks <= datee.Ticks)
                    {
                        indexb = _kb.RowIndexByDate(dateb.ToString("yyyy-MM-dd HH:mm:ss"));
                        //找到索引
                        if (indexb >= 0 && indexb != _kb.Count)
                        {
                            break;
                        }
                        dateb = dateb.AddDays(1);
                    }
                    dateb = DateTime.Parse(paramlist[0].DateStart);
                    //得到结束日对应的k线数据索引
                    while (dateb.Ticks <= datee.Ticks)
                    {
                        indexe = _kb.RowIndexByDate(datee.ToString("yyyy-MM-dd HH:mm:ss"));
                        if (indexe >= 0 && indexe != _kb.Count)
                        {
                            break;
                        }
                        datee = datee.AddDays(-1);
                    }
                    //从起始日到结束日之间不存在数据，跳过此股票分析
                    if (indexb < 0 || indexe < 0)
                    {
                        lock (alock)
                        {
                            didCount++;
                        }
                        OnCountChangedEvent(this, didCount);
                        continue;
                    }
                    Analyze Ananalyze = new Analyze();
                    AnalyzeEnded state = new AnalyzeEnded();
                    //在起始日到结束日之间进行分析
                    for (int j = indexb; j <= indexe; j++)
                    {
                        if (!this.isThreadding)
                        {
                            return;
                        }
                        //对每个参照股票信息进行对比分析
                        foreach (StockAndDate sad in refsiset.Keys)
                        {
                            if (!this.isThreadding)
                            {
                                return;
                            }
                            state = doAnalyze(Ananalyze, _kb, refsiset[sad], stock, paramlist, j);
                        }
                    }
                //}


                //alock在这里给didCount当锁用，保证多个线程中didCount的计数正确。
                lock (alock)
                {
                    didCount++;
                    
                }
                OnCountChangedEvent(this, didCount);
            }
            //GC.Collect();

        }

        private void loopingInNormal_UseARListFile(List<AnalyzeParameters> paramlist, Hashtable StockList)
        {
            foreach (List<Hashtable> htlist in StockList.Values)
            {
                if (!this.isThreadding)
                {
                    return;
                }
                Analyze Ananalyze = new Analyze();
                foreach (Hashtable stock in htlist)
                {
                    KBase _kb = new KBase(upDateData(stock));
                    int indexAnalyze = _kb.RowIndexByDate(stock["AnalyzeDay"].ToString());
                    if (!this.isThreadding)
                    {
                        return;
                    }
                    //对每个参照股票信息进行对比分析
                    foreach (StockAndDate sad in refsiset.Keys)
                    {
                        if (!this.isThreadding)
                        {
                            return;
                        }
                        doAnalyze(Ananalyze, _kb, refsiset[sad], stock, paramlist, indexAnalyze);
                    }
                    didCount++;
                    
                }
                OnCountChangedEvent(this, didCount);
            }
            //GC.Collect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="paramlist"></param>
        /// <param name="Ananalyze"></param>
        /// <param name="state"></param>
        /// <returns>返回分析结果状态AnalyzeEnded</returns>
        private AnalyzeEnded doAnalyze(Analyze Ananalyze,  KBase kbase, ReferenceStockInfo refsi, Hashtable stock, List<AnalyzeParameters> Params, int indexAnalyze)
        {
            if (!this.isThreadding)
            {
                return null;
            }
            AnalyzeEnded state = Ananalyze.doAnalyze(kbase, refsi, stock, Params, indexAnalyze);

            if (state.State == AnalyzeResultState.HaveResults)
            {
                OnHasResultEvent(Ananalyze,AddDataForDrawKGraphic(Ananalyze));
            }

            return state;

        }


        private StockDataSet upDateData(Hashtable stock)
        {
            //KBase _kb = new KBase();
            StockDataSet _sds = new StockDataSet();
            lock (sdatasets)
            {

                if (sdatasets.ContainsKey(stock["StockCode"].ToString()))
                {
                    _sds = sdatasets[stock["StockCode"].ToString()];
                }
                else
                {
                    PipeClient pc = new PipeClient();
                    _sds = pc.getData(stock);
                    if (_sds.StockCode != null)
                    {
                        sdatasets.Add(stock["StockCode"].ToString(), _sds);
                    }
                }

            }

            return _sds; 
        }

        private void upDateRefStockInfo()
        {
            refsiset.Clear();
            //2015-8-3
            string refdate = "2015-8-3";
            string ref_stockcode = "600350";
            //string ref_stockcode = "600073";
            //string ref_stockcode = "600824";
            //string ref_stockcode = "000935";
            //string ref_stockcode = "600697";
            //string ref_stockcode = "600716";
            //string ref_stockcode = "600606";
            //2015-8-4
            //string refdate = "2015-8-4";
            //string ref_stockcode = "600350";
            //2015-4-27
            //string refdate = "2015-4-27";
            //string ref_stockcode = "600697"; 
            //2015-4-3
            //string refdate = "2015-1-21";
            //string ref_stockcode = "600606";
            //2015-3-24
            //string refdate = "2015-3-24";
            //string ref_stockcode = "002435"; //小幅持平+一天大涨
            //2015-3-16
            //string refdate = "2015-3-16";
            //string ref_stockcode = "600350";//小幅持平+一天大涨
            //2015-3-13
            //string refdate = "2015-3-17";
            //string ref_stockcode = "000656";
            //2015-3-30
            //string refdate = "2015-3-30";
            //string ref_stockcode = "300318";
            //2015-2-10
            //string refdate = "2015-2-10";
            //string ref_stockcode = "002177";
            //2014-7-20
            //string refdate = "2014-7-18";
            //string ref_stockcode = "000736";
            //2014-6-13
            //string refdate = "2014-6-13";
            //string ref_stockcode = "002067";
            //2013-8-12
            //string refdate = "2013-8-12";
            //string ref_stockcode = "300288";
            StockAndDate sad = new StockAndDate();
            Hashtable stock = Utility.GetStockht(ref_stockcode);
            StockDataSet sds = upDateData(stock);
            StockDataSet newsds = new StockDataSet();
            newsds.kDayDataList = sds.kDayDataList;
            newsds.ma = sds.ma;
            newsds.StockCode = sds.StockCode;
            int t= sds.kDayDataList.RowIndexByDate(refdate);
            int tkpid = sds.kpSet.findKPieceIndex(t);
            int count = 1;
            for(int i=tkpid-1;i>=0;i--)
            {
                if (sds.kpSet.kPieces[i].Begin >= t - 50)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            kPiece[] kps = new kPiece[count];
            for (int i = 0; i <= count - 1; i++)
            {
                if (i == count - 1)
                {
                    kps[i] = sds.kpSet.genkPiece(sds.kpSet.kPieces[tkpid - (count - 1) + i].Begin, t);
                }
                else
                {
                    kps[i] = sds.kpSet.kPieces[tkpid - (count - 1) + i];
                }
                
            }
            newsds.kpSet = new kPieceSet(sds.kDayDataList, kps);
            newsds.avgNAmpList = (AvgNeighborAmpList)sds.avgNAmpList.Clone();
            if (t - 2 >= 0)
            {
                newsds.avgNAmpList[t] = newsds.avgNAmpList[t - 2];
                newsds.avgNAmpList[t - 1] = newsds.avgNAmpList[t - 2];
            }
            KBase kb = new KBase(newsds);
            sad.StockCode = ref_stockcode;
            sad.Date = DateTime.Parse(refdate);
            if(!refsiset.ContainsKey(sad))
            {
                if (t >= 0)
                {
                    refsiset.Add(sad, new ReferenceStockInfo(kb.RowIndexByDate(refdate), kb));
                }
            }
            //非股票评价的分析类型时，若refsiset无内容，则添加一个虚假参照信息以
            //便之后的foreach (StockAndDate sad in refsiset.Keys)可以有内容进行一次循环
            if(this.parameters[0].Mode== AnalyzeModel.ShowKLine)
            {
                if(refsiset.Count==0)
                {
                    sad.StockCode = "dummy";
                    sad.Date = DateTime.Now;
                    refsiset.Add(sad, new ReferenceStockInfo());
                }
            }
        }

        /// <summary>
        /// 将Analyze分析出的一条结果保存在列表中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="an"></param>
        private int AddDataForDrawKGraphic(Analyze an)
        {
            int key = 0;
            KBase kb = new KBase(sdatasets[an.stock["StockCode"].ToString()]);
            AnalyzeResultForDraw arfd = new AnalyzeResultForDraw();
            Random rdm = new Random(int.Parse(DateTime.Now.Ticks.ToString().Substring(9, 9)) + datasForDrawkGraphic.Count + 1);
            arfd.id = DateTime.Now.Ticks + rdm.Next();
            arfd.AnalyzeDay = kb[an.indexAnalyze].Date;
            arfd.AnalyzeDayIdx = an.indexAnalyze;
            arfd.SqlStartDay = kb[an.indexAnalyze].Date;
            arfd.stock = an.stock;
            arfd.AnalyzeModel = "";
            arfd.AnalyzeModelFunc = "";
            arfd.DataSource = Source;

            if (!arfd.KeyDays.ContainsKey(kb[an.indexAnalyze].Date))
            {
                arfd.KeyDays.Add(kb[an.indexAnalyze].Date, new DayTypesOfADay());
            }
            arfd.KeyDays[kb[an.indexAnalyze].Date].Add(DayType.AnalyzeDay);

            int begin = 0;
            //if (an.wList != null)
            //{
            //    for (int j = begin; j < an.wList.Count; j++)
            //    {
            //        if (!arfd.KeyDays.ContainsKey(an.theDT.Rows[an.wList[j].Begin]["Date"].ToString()))
            //        {
            //            arfd.KeyDays.Add(an.theDT.Rows[an.wList[j].Begin]["Date"].ToString(), new DayTypesOfADay());
            //        }
            //        arfd.KeyDays[an.theDT.Rows[an.wList[j].Begin]["Date"].ToString()].Add(DayType.BeginDayLv1);

            //        if (!arfd.KeyDays.ContainsKey(an.theDT.Rows[an.wList[j].End]["Date"].ToString()))
            //        {
            //            arfd.KeyDays.Add(an.theDT.Rows[an.wList[j].End]["Date"].ToString(), new DayTypesOfADay());
            //        }
            //        arfd.KeyDays[an.theDT.Rows[an.wList[j].End]["Date"].ToString()].Add(DayType.EndDayLv1);
            //    }
            //}

            if (kb.kpSet != null)
            {
                for (int j = begin; j < kb.kpSet.kPieces.Length; j++)
                {
                    if (!arfd.KeyDays.ContainsKey(kb[kb.kpSet.kPieces[j].Begin].Date))
                    {
                        arfd.KeyDays.Add(kb[kb.kpSet.kPieces[j].Begin].Date, new DayTypesOfADay());
                    }
                    arfd.KeyDays[kb[kb.kpSet.kPieces[j].Begin].Date].Add(DayType.BeginDayLv1);

                    //if (!arfd.KeyDays.ContainsKey(kb[kb.kpSet.kPieces[j].End].Date))
                    //{
                    //    arfd.KeyDays.Add(kb[kb.kpSet.kPieces[j].End].Date, new DayTypesOfADay());
                    //}
                    //arfd.KeyDays[kb[kb.kpSet.kPieces[j].End].Date].Add(DayType.EndDayLv1);
                }
            }

            begin = 0;
            if (an.RHDays != null)
            {
                for (int j = begin; j < an.RHDays.Count; j++)
                {
                    if (!arfd.KeyDays.ContainsKey(kb[an.RHDays[j]].Date))
                    {
                        arfd.KeyDays.Add(kb[an.RHDays[j]].Date, new DayTypesOfADay());
                    }
                    arfd.KeyDays[kb[an.RHDays[j]].Date].Add(DayType.RHDay);
                }
            }

            lock (datasForDrawkGraphic)
            {
                key = datasForDrawkGraphic.Count;
                foreach (int k in datasForDrawkGraphic.Keys)
                {
                    if (((AnalyzeResultForDraw)datasForDrawkGraphic[k]).id == arfd.id)
                    {
                        arfd.id++;
                    }
                }
                datasForDrawkGraphic.Add(key, arfd);
            }
            return key;
        }
    }
}
