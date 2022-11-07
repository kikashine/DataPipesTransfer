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
    public class JobBase
    {
        protected bool isThreadding;

        protected AutoResetEvent[] autoEvents;

        public List<AnalyzeParameters> parameters;

        public int totalCountJob;

        public int didCount;

        public int Source;

        public string lastUpdateDataTime;

        public Hashtable datasForDrawkGraphic;
        /// <summary>
        /// 所有分析任务的切片是否被分析的标志位
        /// </summary>
        protected List<bool> didJobSplites;

        /// <summary>
        /// 所有分析任务的切片是否正在分析的标志位
        /// </summary>
        protected List<bool> doingJobSplites;

        /// <summary>
        /// 每一天分析任务是否被分析的标志位
        /// </summary>

        /// <summary>
        /// 切片后的所有分析任务的股票列表
        /// </summary>
        protected List<Hashtable> StockListsForJobSplites;

        /// <summary>
        /// 分析任务的线程数量
        /// </summary>
        protected int JobThreadCount;

        /// <summary>
        /// Started事件的委托
        /// </summary>
        /// <param name="stopped"></param>
        public delegate void StartedEventHandler(bool started);
        /// <summary>
        /// Stopped事件的委托
        /// </summary>
        /// <param name="stopped"></param>
        public delegate void StoppedEventHandler(bool stopped);
        /// <summary>
        /// finished事件的委托
        /// </summary>
        /// <param name="stopped"></param>
        public delegate void FinishedEventHandler(bool finished);
        /// <summary>
        /// Message事件的委托
        /// </summary>
        /// <param name="msg"></param>
        public delegate void MessageEventHandler(string msg);
        /// <summary>
        /// CountChanged事件的委托
        /// </summary>
        /// <param name="msg"></param>
        public delegate void CountChangedEventHandler(JobBase jb, int count);
        /// <summary>
        /// HasResult事件的委托
        /// </summary>
        /// <param name="msg"></param>
        public delegate void HasResultEventHandler(Analyze Ananalyze, int orgindex);
        /// <summary>
        /// DataUpdated事件的委托
        /// </summary>
        /// <param name="msg"></param>
        //public delegate void DataUpdatedEventHandler(string newtime);

        /// <summary>
        /// Stopped事件委托的实例，用来触发事件
        /// </summary>
        protected StartedEventHandler OnStartedEvent;
        /// <summary>
        /// Stopped事件委托的实例，用来触发事件
        /// </summary>
        protected StoppedEventHandler OnStoppedEvent;
        /// <summary>
        ///Finished事件委托的实例，用来触发事件
        /// </summary>
        protected FinishedEventHandler OnFinishedEvent;
        /// <summary>
        /// Message事件委托的实例，用来触发事件
        /// </summary>
        protected MessageEventHandler OnMessageEvent;
        /// <summary>
        /// CountChanged事件委托的实例，用来触发事件
        /// </summary>
        protected CountChangedEventHandler OnCountChangedEvent;
        /// <summary>
        /// HasResult事件委托的实例，用来触发事件
        /// </summary>
        protected HasResultEventHandler OnHasResultEvent;
        /// <summary>
        /// DataUpdated事件委托的实例，用来触发事件
        /// </summary>
       // protected DataUpdatedEventHandler OnDataUpdatedEvent;

        public bool isWorking
        {
            get
            {
                return isThreadding;
            }
        }


        /// <summary>
        /// Started事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event StartedEventHandler Started
        {
            add { OnStartedEvent += new StartedEventHandler(value); }
            remove { OnStartedEvent -= new StartedEventHandler(value); }
        }
        /// <summary>
        /// Stopped事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event StoppedEventHandler Stopped
        {
            add { OnStoppedEvent += new StoppedEventHandler(value); }
            remove { OnStoppedEvent -= new StoppedEventHandler(value); }
        }
        /// <summary>
        /// Finished事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event FinishedEventHandler Finished
        {
            add { OnFinishedEvent += new FinishedEventHandler(value); }
            remove { OnFinishedEvent -= new FinishedEventHandler(value); }
        }
        /// <summary>
        /// Message事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event MessageEventHandler Message
        {
            add { OnMessageEvent += new MessageEventHandler(value); }
            remove { OnMessageEvent -= new MessageEventHandler(value); }
        }
        /// <summary>
        ///  CountChanged事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event CountChangedEventHandler CountChanged
        {
            add { OnCountChangedEvent += new CountChangedEventHandler(value); }
            remove { OnCountChangedEvent -= new CountChangedEventHandler(value); }
        }
        /// <summary>
        ///  HasResult事件访问器，订阅事件时将调用方问器
        /// </summary>
        public event HasResultEventHandler HasResult
        {
            add { OnHasResultEvent += new HasResultEventHandler(value); }
            remove { OnHasResultEvent -= new HasResultEventHandler(value); }
        }
        /// <summary>
        ///  DataUpdated事件访问器，订阅事件时将调用方问器
        /// </summary>
        //public event DataUpdatedEventHandler DataUpdated
        //{
        //    add { OnDataUpdatedEvent += new DataUpdatedEventHandler(value); }
        //    remove { OnDataUpdatedEvent -= new DataUpdatedEventHandler(value); }
        //} 

        ///// <summary>
        ///// 检查k线数据更新状况
        ///// </summary>
        //protected void checkDataUpdate()
        //{
        //    PipeClient pipeClient = new PipeClient(new Hashtable());

        //    string newtime = pipeClient.getDataUpdate();
        //    if (DateTime.Parse(lastUpdateDataTime).Ticks < DateTime.Parse(newtime).Ticks)
        //    {
        //        //theDS.Tables.Clear();
        //        OnDataUpdatedEvent(newtime);
        //        lastUpdateDataTime = newtime;
        //    }
        //}

        /// <summary>
        /// 调用分析主进程的虚函数，为方便定义在本类中的方法(例如btnanalyze_Click)调用而定义为虚函数
        /// 本函数实际执行内容在继承类中进行重载
        /// </summary>
        public virtual void Start()
        {

        }

        public void Stop()
        {
            this.isThreadding = false;
        }

        /// <summary>
        /// 调用启动分析分线程工作方法的虚函数，为方便定义在本类中的方法(例如StartAHostThread)调用而定义为虚函数
        /// 本函数实际执行内容在继承类中进行重载
        /// </summary>
        protected virtual void startJob()
        {

        }
        /// <summary>
        /// 启动分析主进程的方法
        /// </summary>
        protected void StartAHostThread()
        {
            didCount = 0;
            datasForDrawkGraphic = new Hashtable();
            totalCountJob = 0;
            didJobSplites = new List<bool>();
            doingJobSplites = new List<bool>();
            StockListsForJobSplites = new List<Hashtable>();
            //用几个线程分析
            JobThreadCount = 1;

            this.isThreadding = true;
            OnStartedEvent(true);

            Thread totalThread = new Thread(new ThreadStart(startJob));
            totalThread.SetApartmentState(ApartmentState.MTA);
            totalThread.IsBackground = true;
            totalThread.Name = "Job";
            totalThread.Start();
        }

        /// <summary>
        /// 分割分析任务
        /// </summary>
        protected void spliteJob(bool UseARListFile, string ARListFile, bool OneStockCode, string StockCode)
        {
            Hashtable StockLists = Utility.GetStockList(UseARListFile, ARListFile, this.parameters[0].DateEnd);
            if (OneStockCode && !UseARListFile)
            {
                if (StockLists.ContainsKey(StockCode))
                {
                    Hashtable st = (Hashtable)(StockLists[StockCode]);
                    StockLists = new Hashtable();
                    StockLists.Add(StockCode, st);
                }
                else
                {
                    StockLists = new Hashtable();
                }

            }
            if(OneStockCode && UseARListFile)
            {
                if (StockLists.ContainsKey(StockCode))
                {
                    List<Hashtable> st = (List<Hashtable>)(StockLists[StockCode]);
                    StockLists = new Hashtable();
                    StockLists.Add(StockCode, st);
                }
                else
                {
                    StockLists = new Hashtable();
                }
            }
            int stockperthread = StockLists.Count / JobThreadCount;
            //按线程“分片”

            Hashtable sl = new Hashtable();
            foreach (string cd in StockLists.Keys)
            {
                sl.Add(cd, StockLists[cd]);
                if (sl.Count == stockperthread)
                {
                    StockListsForJobSplites.Add(sl);
                    didJobSplites.Add(false);
                    doingJobSplites.Add(false);
                    sl = new Hashtable();
                }
            }
            if (sl.Count > 0)
            {
                StockListsForJobSplites.Add(sl);
                didJobSplites.Add(false);
                doingJobSplites.Add(false);

            }

            if (UseARListFile)
            {
                totalCountJob = 0;
                foreach (List<Hashtable> slist in StockLists.Values)
                {
                    totalCountJob += slist.Count;
                }
            }
            else
            {
                totalCountJob = StockLists.Count;
            }
        }

        /// <summary>
        /// 启动分析分线程的方法
        /// </summary>
        protected void startJobThreads()
        {
            OnCountChangedEvent(this, 0);

            Thread[] jobThread = new Thread[JobThreadCount];
            autoEvents = new AutoResetEvent[JobThreadCount];

            for (int i = 0; i <= jobThread.Length - 1; i++)
            {
                autoEvents[i] = new AutoResetEvent(false);
                jobThread[i] = new Thread(new ParameterizedThreadStart(loopingDoJob));
                jobThread[i].IsBackground = true;
                jobThread[i].Name = "AnalyzeJob" + i.ToString();
                jobThread[i].Start(i);
                Thread.Sleep(10);
            }

            WaitHandle.WaitAll(autoEvents);

            if (this.isThreadding)
            {
                OnFinishedEvent(true);
            }
            else
            {
                OnStoppedEvent(true);
            }
            this.isThreadding = false;

        }

        /// <summary>
        /// 按片来进行分析
        /// </summary>
        /// <param name="autoEventNum"></param>
        private void loopingDoJob(object autoEventNum)
        {

            //逐“片”进行任务分析
            for (int j = 0; j <= StockListsForJobSplites.Count - 1; j++)
            {
                //当日已分割的股票列表
                Hashtable sl = StockListsForJobSplites[j];

                //检查该“片”任务是否已在进行

                //该“片”未分析过
                lock (doingJobSplites)
                {
                    if (!doingJobSplites[j])
                    {
                        //Thread.Sleep((int)autoEventNum * 125);
                        //标记为进行状态
                        doingJobSplites[j] = true;
                        //开始分析任务
                        
                    }
                    else
                    {
                        continue;
                    }
                }
                    this.loopingDoJobFuncs(this.parameters, sl);

                if (!this.isThreadding)
                {
                    this.autoEvents[(int)autoEventNum].Set();
                    return;
                }
                //lock (didJobSplites)
                //{
                    didJobSplites[j] = true;
                //}

            }

            this.autoEvents[(int)autoEventNum].Set();
        }






        /// <summary>
        /// 调用具体分析方法的虚函数，为方便定义在本类中的方法(例如loopingDoJob)调用而定义为虚函数
        /// 本函数实际执行内容在继承类中进行重载
        /// </summary>
        protected virtual void loopingDoJobFuncs(List<AnalyzeParameters> paramlist, Hashtable piece)
        {

        }
    }
}
