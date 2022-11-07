using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;
using StockToolKit.Common;


namespace StockToolKit.Analyze
{
    public class Analyze
    {
        private Dictionary<string,KBase> _kbset;

        private KBase _kb;

        public System.Data.DataRow Result;

        //���ڽ��з��������յ�k����������ֵ
        public int indexAnalyze;

        public List<string> ColumnName;

        public List<string> ColumnText;

        public List<int> ColumnSize;

        public List<string> ColumnSort;

        public List<object> ResultList;
        /// <summary>
        /// ��¼�ڽ����з���������ɸѡ�����Ĺ�Ʊ����
        /// </summary>
        public List<string> toStockListFileInTradding;

        public Hashtable stock;

        private int InfoOffset;

        private ArrayList ResultTmp;

        private Hashtable ColumnBuilded;

        private string StockCode;

        /// <summary>
        /// ���η����Ƿ��ڽ��׽�����
        /// </summary>
        private bool AnalyzeInTradding;

        private ReferenceStockInfo _refsi;

        private List<int> rhdays;

        public DataForDrawKGraphic DataForDrawK;

        public Analyze()
        {
            this.ColumnName = new List<string>();
            this.ColumnText = new List<string>();
            this.ColumnSize = new List<int>();
            this.ColumnSort = new List<string>();
            this.InfoOffset = 6;
            this.ColumnBuilded = new Hashtable();
            this.ColumnBuilded.Add("common", false);
            this.ColumnBuilded.Add("rsi1", false);
            this.ColumnBuilded.Add("rsi2", false);
            this.ColumnBuilded.Add("wltrend1", false);
            this.ColumnBuilded.Add("matrend1", false);
            this.ColumnBuilded.Add("matrend2", false);
            this.ColumnBuilded.Add("showkline", false);
            this.ColumnBuilded.Add("test", false);
            toStockListFileInTradding = new List<string>();

        }

        public AnalyzeEnded doAnalyze(KBase kbase, ReferenceStockInfo refsi, Hashtable stock, List<AnalyzeParameters> Params, int indexAnalyze)
        {
            //this._kbset = kbset;
            this._kb = kbase;
            this.stock = stock;
            AnalyzeEnded ae = new AnalyzeEnded();
            ae.StockCode = stock["StockCode"].ToString();
            this.ResultList = new List<object>();
            bool doAnalyze = false;
            this._refsi = refsi;

            DataForDrawK = new DataForDrawKGraphic();

          
            this.indexAnalyze = indexAnalyze;

            //�����µĹ�Ʊ���µ�ʱ���
            //if (StockCode != stock["StockCode"].ToString() || DateStart != Params[0].DateStart || DateEnd != Params[0].DateEnd)
            //{
            //    rhdays = null;
            //    this.indexAnalyze = 0;
            //    StockCode = stock["StockCode"].ToString();

            //}
            
            if (rhdays == null)
            {
                rhdays = new List<int>();
            }
            ResultList = new List<object>();


            //return ae;
            //rhdays = null;

            if (_kb == null || _kb.Count == 0)
            {
                ae.State = AnalyzeResultState.OutDatas;
                ae.LastIndexAnalyze = indexAnalyze;
                return ae;
            }

            genColumns();

            doAnalyze = doAnalyzeOneDayModel(stock, ref Params);

            if (doAnalyze)
            {

            }
            else
            {
                if (ResultList != null)
                {
                    ResultList.Clear();
                }
                ae.State = AnalyzeResultState.NotFit;
                ae.LastIndexAnalyze = indexAnalyze;
                return ae;
            }

            genCommonInfo(stock);

            ae.State = AnalyzeResultState.HaveResults;
            ae.LastIndexAnalyze = indexAnalyze;

            return ae;

        }

        /// <summary>
        /// ����ģ���жϡ�����falseʱ�����ᵼ�´����жϽ���ڽ�����б�ɾ����
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public bool doAnalyzeOneDayModel(Hashtable stock, ref List<AnalyzeParameters> Params)
        {
            bool result = false;
            for(int i= 0;i< Params.Count;i++)
            {
                AnalyzeParameters param = Params[i];
                switch (Params[i].Mode)
                {
                    //case AnalyzeModel.WLTrendRise:
                    //    if (WLTrendRise(stock, ref param))
                    //    {
                    //        //����ĳһ������ģ�͵��ж��У���ı�ģ���жϵĲ������Ա���к�������ݷ���
                    //        //������Ҫ���ı��Ĳ�������ԭ�ȵ�
                    //        Params[i] = param;
                    //        result = true;
                    //        //return true;
                    //    }
                    //    else
                    //    {
                    //        if (i == 0)
                    //        {
                    //            return false;
                    //        }
                    //    }
                    //    break;
                    case AnalyzeModel.ShowKLine:
                        AnalyzeShowKLine ask = new AnalyzeShowKLine(_kb);
                        if (!((bool)(this.ColumnBuilded["showkline"])))
                        {
                            InfoOffset = ask.BuildColumn(ref ColumnName, ref ColumnText, ref ColumnSize, ref ColumnSort, InfoOffset);
                            ColumnBuilded["showkline"] = true;
                        }
                        ResultList = ask.ShowKLine_OneDayModel(indexAnalyze);
                        if (ResultList != null && ResultList.Count>0)
                        {
                            result = true;
                            rhdays.Add(indexAnalyze);
                        }
                        else
                        {
                            if (i == 0)
                            {
                                return false;
                            }
                        }
                        break;
                    case AnalyzeModel.Test:

                        AnalyzeTest test = new AnalyzeTest(_kb);
                        if (!((bool)(this.ColumnBuilded["test"])))
                        {
                            InfoOffset = test.BuildColumn(ref ColumnName, ref ColumnText, ref ColumnSize, ref ColumnSort, InfoOffset);
                            ColumnBuilded["test"] = true;
                        }
                        ResultList = test.Test_OneDayModel(indexAnalyze, _refsi);
                        if (ResultList != null && ResultList.Count>0)
                        {
                            result = true;
                            rhdays.Add(indexAnalyze);
                        }
                        //if (Test(stock, ref param))
                        //{
                        //    //����ĳһ������ģ�͵��ж��У���ı�ģ���жϵĲ������Ա���к�������ݷ���
                        //    //������Ҫ���ı��Ĳ�������ԭ�ȵ�
                        //    Params[i] = param;
                        //    result = true;
                        //    //return true;
                        //}
                        else
                        {
                            if (i == 0)
                            {
                                return false;
                            }
                        }
                        break;
                    case AnalyzeModel.err:

                        return false;
                }
            }
            return result;
        }

         private void genColumns()
        {
            
            if ((bool)ColumnBuilded["common"] == false)
            {
                ColumnName.Add("��Ʊ����");
                ColumnName.Add("�г�");
                ColumnName.Add("��ҵ���");
                ColumnName.Add("������");
                ColumnName.Add("����");
                ColumnName.Add("����");

                ColumnText.Add("��Ʊ����");
                ColumnText.Add("�г�");
                ColumnText.Add("��ҵ���");
                ColumnText.Add("������");
                ColumnText.Add("����");
                ColumnText.Add("����");

                ColumnSize.Add(50);
                ColumnSize.Add(0);
                ColumnSize.Add(125);
                ColumnSize.Add(80);
                ColumnSize.Add(45);
                ColumnSize.Add(45);

                ColumnSort.Add("num");
                ColumnSort.Add("str");
                ColumnSort.Add("str");
                ColumnSort.Add("str");
                ColumnSort.Add("num");
                ColumnSort.Add("num");

                ColumnBuilded["common"] = true;
            }
        }

        private bool genCommonInfo(Hashtable stock)
        {
            ResultList.Insert(0,stock["StockCode"]);
            ResultList.Insert(1, stock["MarketCode"]);
            ResultList.Insert(2, stock["Industry"]);
            ResultList.Insert(3, Utility.toDBDate(_kb[indexAnalyze].Date.ToString()).Split(' ')[0]);
            ResultList.Insert(4, _kb[indexAnalyze].Open);
            ResultList.Insert(5, _kb[indexAnalyze].Close);
            return true;
        }

        public List<int> RHDays
        {
            get
            {
                return rhdays;
            }
        }
    }
}
