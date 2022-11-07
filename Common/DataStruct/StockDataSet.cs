using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    /// <summary>
    /// 一个股票的数据集合
    /// </summary>
    public class StockDataSet
    {
        protected string _stockcode;

        protected KDayDataList _kdlist;

        protected KDayDataList _fakdlist;

        protected List<MAList> _malists = new List<MAList>();

        protected List<MAList> _famalists = new List<MAList>();

        protected List<TrendPieceList> _matplists = new List<TrendPieceList>();

        protected List<TrendPieceList> _famatplists = new List<TrendPieceList>();

        protected KDayDataFQType _getdatafqtype = KDayDataFQType.FA;

        public StockDataSet()
        {

        }
        /// <summary>
        /// 生成一个股票的数据集合的对象
        /// </summary>
        /// <param name="kdlist">k线数据列表（除权）</param>
        /// <param name="fakdlist">k线数据列表（前复权）</param>
        /// <param name="malist">简单移动平均值列表（除权）</param>
        /// <param name="famalist">简单移动平均值列表（复权）</param>
        /// <param name="stockcode">股票代码</param>
        public StockDataSet(KDayDataList kdlist, KDayDataList fakdlist, List<MAList> malists, List<MAList> famalists, string stockcode)
        {
            _kdlist = kdlist;
            _fakdlist = fakdlist;
            _malists = malists;
            _famalists = famalists;
            _stockcode = stockcode;
        }
        /// <summary>
        /// k线数据列表
        /// </summary>
        public KDayDataList KDList
        {
            get
            {
                return _kdlist;
            }
        }
        /// <summary>
        /// k线数据列表（向前复权）
        /// </summary>
        public KDayDataList FAKDList
        {
            get
            {
                return _fakdlist;
            }
        }
        /// <summary>
        /// 简单移动平均值列表（除权）
        /// </summary>
        public List<MAList> MALists
        {
            get
            {
                return _malists;
            }
        }
        /// <summary>
        /// 简单移动平均值列表（向前复权）
        /// </summary>
        public List<MAList> FAMALists
        {
            get
            {
                return _famalists;
            }
        }

        /// <summary>
        /// 简单移动平均值按趋势分片列表（除权）
        /// </summary>
        public List<TrendPieceList> MATPLists
        {
            get
            {
                return _matplists;
            }
        }

        /// <summary>
        /// 简单移动平均值按趋势分片列表（向前复权）
        /// </summary>
        public List<TrendPieceList> FAMATPLists
        {
            get
            {
                return _famatplists;
            }
        }

        public string StockCode
        {
            get
            {
                return _stockcode;
            }
        }

        public int Length
        {
            get
            {
                return _kdlist.Count;
            }
        }

        /// <summary>
        /// 设定或获得本数据集合的复权类型，默认为KDayDataFQType.FA（向前复权）
        /// 设置复权类型后，获取股票价格及价格相关数据时按复权类型计算
        /// </summary>
        public KDayDataFQType FqType
        {
            get
            {
                return _getdatafqtype;
            }
            set
            {
                _getdatafqtype = value;
            }
        }
        /// <summary>
        /// 指定日股票价格开盘价
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Open(int i)
        {
            if(i>= _kdlist.Count)
            {
                return -1;
            }
            if(_getdatafqtype== KDayDataFQType.DA)
            {
                return _kdlist[i].Open;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].Open;
            }
            return 0;
        }
        /// <summary>
        /// 指定日股票价格收盘价
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Close(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                return _kdlist[i].Close;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].Close;
            }
            return 0;
        }
        /// <summary>
        /// 指定日股票价格开盘价和收盘价中的高值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float High(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                return _kdlist[i].High;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].High;
            }
            return 0;
        }

        /// <summary>
        /// 指定日股票价格开盘价和收盘价中的低值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Low(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                return _kdlist[i].Low;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].Low;
            }
            return 0;
        }

        /// <summary>
        /// 指定日的股票价格最高值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Highest(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                return _kdlist[i].Highest;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].Highest;
            }
            return 0;
        }

        /// <summary>
        /// 指定日的股票价格最低值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float Lowest(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                return _kdlist[i].Lowest;
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                return _fakdlist[i].Lowest;
            }
            return 0;
        }

        /// <summary>
        /// 指定日的交易量
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double Volume(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            return _kdlist[i].Volume;
        }

        /// <summary>
        /// 指定日的交易额
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double Amount(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            return _kdlist[i].Amount;
        }

        /// <summary>
        /// 指定日的日期
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public DateTime Date(int i)
        {
            if (i >= _kdlist.Count)
            {
                return new DateTime();
            }
            return _fakdlist[i].Date;
        }

        /// <summary>
        /// 指定日的A股流通数量
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public long Ccapital(int i)
        {
            if (i >= _kdlist.Count)
            {
                return -1;
            }
            return _fakdlist[i].Ccapital;
        }

        /// <summary>
        /// 指定日是否权息变动日
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool isAday(int i)
        {
            if (i >= _kdlist.Count)
            {
                return false;
            }
            return _fakdlist[i].isAday;
        }

        /// <summary>
        /// 指定日是否股本变动日
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool isCday(int i)
        {
            if (i >= _kdlist.Count)
            {
                return false;
            }
            return _fakdlist[i].isCday;
        }

        /// <summary>
        /// 为本数据集合增加除权MAList
        /// </summary>
        /// <param name="malist"></param>
        public void addMAList(MAList malist)
        {
            this._malists.Add(malist);
        }

        /// <summary>
        /// 为本数据集合增加向前复权MAList
        /// </summary>
        /// <param name="malist"></param>
        public void addFAMAList(MAList famalist)
        {
            this._famalists.Add(famalist);
        }

        /// <summary>
        /// 为本数据集合增加除权MATPList
        /// </summary>
        /// <param name="matplist"></param>
        public void addMATPList(TrendPieceList matplist)
        {
            this._matplists.Add(matplist);
        }

        /// <summary>
        /// 为本数据集合增加向前复权MATPList
        /// </summary>
        /// <param name="matplist"></param>
        public void addFAMATPList(TrendPieceList famatplist)
        {
            this._famatplists.Add(famatplist);
        }
        /// <summary>
        /// 全部数据中交易量最大日的索引
        /// </summary>
        public int MaxVolumeIndex
        {
            get
            {
                return _kdlist.MaxVolumeIndex;
            }
           
        }

        /// <summary>
        /// 对比两个数值之间的涨幅
        /// </summary>
        /// <param name="b">参照值</param>
        /// <param name="c">当前值</param>
        /// <returns></returns>
        public float PriceRise(float b, float c)
        {
            return (c - b) / b;
        }

        //public float MA4(int i)
        //{
        //    return _malist[i].ma4;
        //}

        //public float MA8(int i)
        //{
        //    return _malist[i].ma8;
        //}

        //public float MA12(int i)
        //{
        //    return _malist[i].ma12;
        //}

        //public float MA24(int i)
        //{
        //    return _malist[i].ma24;
        //}
        /// <summary>
        /// 获取指定日指定类型的MA值
        /// </summary>
        /// <param name="i">指定日的k线数据索引</param>
        /// <param name="type">MA的类型(日数)</param>
        /// <returns></returns>
        public float MA(int i, string type)
        {
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                for(int j=0;j<= _malists.Count-1;j++)
                {
                    if(_malists[j].Type==type)
                    {
                        return _malists[j][i];
                    }
                }
                
            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                for (int j = 0; j <= _famalists.Count - 1; j++)
                {
                    if (_famalists[j].Type == type)
                    {
                        return _famalists[j][i];
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// MA数据的类型（日数）
        /// </summary>
        public string[] MAType
        {
            get
            {
                string[] types = new string[_malists.Count];
                if (_getdatafqtype == KDayDataFQType.DA)
                {
                    for (int j = 0; j <= _malists.Count - 1; j++)
                    {
                        types[j] = _malists[j].Type;
                    }
                }
                if (_getdatafqtype == KDayDataFQType.FA)
                {
                    for (int j = 0; j <= _famalists.Count - 1; j++)
                    {
                        types[j] = _famalists[j].Type;
                    }
                }
                return types;
            }


        }
        /// <summary>
        /// 获取指定类型MA的升降趋势分片列表
        /// </summary>
        /// <param name="type">MA类型</param>
        /// <returns></returns>
        public TrendPieceList MATPList(string type)
        {
            if (_getdatafqtype == KDayDataFQType.DA)
            {
                for (int j = 0; j <= _matplists.Count - 1; j++)
                {
                    if (_matplists[j].Type == type)
                    {
                        return _matplists[j];
                    }
                }

            }
            if (_getdatafqtype == KDayDataFQType.FA)
            {
                for (int j = 0; j <= _famatplists.Count - 1; j++)
                {
                    if (_famatplists[j].Type == type)
                    {
                        return _famatplists[j];
                    }
                }

            }
            return null;
        }
    }

}
