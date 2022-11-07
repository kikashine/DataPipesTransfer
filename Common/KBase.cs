using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace StockToolKit.Common
{
    [Serializable]
    public class KBase
    {
        protected KDayDataList _list;

        protected bool dateIndexMapped = false;

        public kPieceSet kpSet;

        protected Hashtable _stock;

        public MA_old ma;

        /// <summary>
        /// 日k线相关的幅度数据
        /// </summary>
        //public AvgNeighborAmpList avgNAmpList;

        public KBase()
        {

        }
        //public KBase(DataTableQ theDT)
        //{
        //    _list = new KDayDataList(theDT.TableName);
        //    for (int i = 0; i < theDT.Rows.Count; i++)
        //    {
        //        _list.Add(new KDayData(theDT, i));
        //    }
        //    _stock = Utility.GetStockht(theDT.TableName);
        //}
        public KBase(StockDataSet sdataset)
        {
            _list = sdataset.kDayDataList;
            //rowindexs = sdataset.DateToRowIndex;
            kpSet = sdataset.kpSet;
            ma = sdataset.ma;
            //avgNAmpList = sdataset.avgNAmpList;
            _stock = Utility.GetStockht(sdataset.StockCode);
        }
        public KBase(KDayDataList dataList)
        {
            _list = dataList;
            _stock = Utility.GetStockht(dataList.StockCode);
        }
        public KBase(KBase kbase)
        {
            this._list = kbase._list;
            //avgNAmpList = kbase.avgNAmpList;
            this.dateIndexMapped = kbase.dateIndexMapped;
            this.kpSet = kbase.kpSet;
            this._stock = kbase._stock;
        }
        public void init(KBase kbase)
        {
            this._list = kbase._list;
            //avgNAmpList = kbase.avgNAmpList;
            this.dateIndexMapped = kbase.dateIndexMapped;
            this.kpSet = kbase.kpSet;
            this._stock = kbase._stock;
        }
        new public KDayData this[int index]
        {
            get
            {
                return _list[index];
            }
        }
        public List<KDayData> List
        {
            get
            {
                return _list;
            }
        }

        public bool DateIndexMapped
        {
            get
            {
                return dateIndexMapped;
            }
        }
        public Hashtable Stock
        {
            get
            {
                return _stock;
            }
        }

        public int RowIndexByDate(string date)
        {
            //if (!dateIndexMapped)
            //{
            //    //oldhashcode = this.GetHashCode();
            //    rowindexs = new Hashtable();
            //    for (int i = 0; i < _list.Count; i++)
            //    {
            //        lock (rowindexs)
            //        {
            //            if (!rowindexs.ContainsKey(_list[i].Date))
            //            {
            //                rowindexs.Add(_list[i].Date, i);
            //            }
            //        }
            //    }
            //    dateIndexMapped = true;
            //}
            //if (rowindexs.ContainsKey(Utility.toDBDate(date)))
            //{
            //    return (int)rowindexs[Utility.toDBDate(date)];
            //}
            ////返回值为k线数据集的长度时，说明指定日期超出k线数据最大日期
            //else if (_list.Count == 0 || (_list.Count > 0 && DateTime.Parse(_list[_list.Count - 1].Date).Ticks < DateTime.Parse(date).Ticks))
            //{
            //    //return _list.Count;
            //    return -1;
            //}
            ////返回值为-1时，说明指定日期不存在于k线数据中
            //else
            //{
            //    return -1;
            //}
            return _list.RowIndexByDate(date);
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public float High(int Index)
        {
            return _list[Index].High;
        }
        public float Low(int Index)
        {
            return _list[Index].Low;
        }
        public float Highest(int Index)
        {
            return _list[Index].Highest;
        }
        public float Lowest(int Index)
        {
            return _list[Index].Lowest;
        }
        public float Open(int Index)
        {
            return _list[Index].Open;
        }

        public float Close(int Index)
        {
            return _list[Index].Close;
        }

        public double Volume(int Index)
        {
            return _list[Index].Volume;
        }
        public long Ccapital(int Index)
        {
            return _list[Index].Ccapital;
        }
        /// <summary>
        /// 返回升幅比
        /// </summary>
        /// <param name="t">需计算升幅比的日期索引</param>
        /// <param name="offset">用来做对比的日期索引偏移量</param>
        /// <returns></returns>
        public float KRise(int t, int offset)
        {
            //return 0;
            if (offset == 0)
            {
                return (_list[t].Close - _list[t].Open) / _list[t].Open;
            }
            else
            {
                return (_list[t].Close - _list[t + offset].Close) / _list[t + offset].Close;
            }
        }

        /// <summary>
        /// 返回升幅比，此为从IndexOfLow日的低值到IndexOfHigh日高值的升幅比
        /// </summary>
        /// <param name="t"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public float KRiseLH(int IndexOfLow, int IndexOfHigh)
        {
            return (_list[IndexOfHigh].High - _list[IndexOfLow].Low) / _list[IndexOfLow].Low;
        }

        /// <summary>
        /// 值A与值B之差与值B的比值(valueA-valueB)/valueB
        /// </summary>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        public float DvalueRatio(float valueA, float valueB)
        {
            return (valueA - valueB) / valueB;
        }

        public float Percent(float a, float b)
        {
            string tmp;
            if (a == 0 || b == 0)
            {
                tmp = "0";
            }
            else
            {
                tmp = ((float)(a / b * 100)).ToString();
            }
            if (tmp.Length > 5)
            {
                return Convert.ToSingle(tmp.Substring(0, 5));
            }
            return Convert.ToSingle(tmp);
        }

        public float Percent(object a, object b)
        {
            string tmp;
            if (Convert.ToSingle(a) == 0 || Convert.ToSingle(b) == 0)
            {
                tmp = "0";
            }
            else
            {
                tmp = ((float)(Convert.ToSingle(a) / Convert.ToSingle(b) * 100)).ToString();
            }
            if (tmp.Length > 5)
            {
                return Convert.ToSingle(tmp.Substring(0, 5));
            }
            return Convert.ToSingle(tmp);
        }

        public float Percent(float a)
        {
            string tmp = ((float)(a * 100)).ToString();
            if (tmp.Length > 5)
            {
                return Convert.ToSingle(tmp.Substring(0, 5));
            }
            return Convert.ToSingle(tmp);
        }

        /// <summary>
        /// 当前日索引所在k线片段的幅度
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float kpAmp(int i)
        {
            float kpamp = 0;
            kPiece kp = this.kpSet.kPieces[this.kpSet.findKPieceIndex(i)];
            float ltmp = this[i].Low;
            float htmp = this[i].Low;

            ///
            ///获得kp幅度方案1
            ///kp的高低值取起始、结束的上或下沿，既kp的上沿和下沿
            ///
            if (kp.Trend == kPieceTrend.Fall)
            {
                htmp = this[kp.Begin].High;
                ltmp = this[kp.End].Low;
            }
            if (kp.Trend == kPieceTrend.Rise)
            {
                htmp = this[kp.End].High;
                ltmp = this[kp.Begin].Low;
            }

            ///
            /// 获得kp幅度方案2
            ///kp的高低值取其中上升或下降的上下沿，既去掉下降kp首尾上升的k线柱或上升kp首尾下降的k线柱
            ///
            //if (kp.Trend == kPieceTrend.Fall)
            //{
            //    htmp = this[kp.Begin].Low;
            //    if (this.KRise(i, 0) > 0 && i == kp.End)
            //    {
            //        ltmp = this[i - 1].Low;
            //    }
            //    if (this.KRise(kp.Begin, 0) > 0)
            //    {
            //        htmp = this[kp.Begin + 1].Low;
            //    }
            //    if (this.KRise(kp.Begin, 0) <= 0)
            //    {
            //        htmp = this[kp.Begin + 1].Low;
            //    }
            //}
            //if (kp.Trend == kPieceTrend.Rise)
            //{
            //    htmp = this[kp.End].Low;
            //    if (this.KRise(i, 0) < 0 && i == kp.End)
            //    {
            //        htmp = this[i - 1].Low;
            //    }
            //    if (this.KRise(kp.Begin, 0) < 0)
            //    {
            //        ltmp = this[kp.Begin + 1].Low;
            //    }
            //    if (this.KRise(kp.Begin, 0) >= 0)
            //    {
            //        ltmp = this[kp.Begin + 1].Low;
            //    }
            //}
            kpamp = this.DvalueRatio(htmp, ltmp);

            return kpamp;
        }

        /// <summary>
        /// 得到合适的计算k线价格百分比的参照值
        /// 原则：
        /// //i在上升趋势中，以上升趋势begin.low作为参照值。
        /// i在上升趋势中，以上升趋势end.low作为参照值。
        /// i在下降趋势中，以下降趋势end.low作为参照值。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float findRefValue(int i, int t)
        {
            float refvalue = 0f;
            kPiece[] _kps = kpSet.kPieces;
            int kpid = kpSet.findKPieceIndex(i);
            //i为t日，以t下沿作为参照值
            if (i == t)
            {
                return Low(t);
            }
            ////i所在kp下降，i=kp.end，kp.end上升，说明i在上升趋势的起始，此时以i.low作为参照值

            if (_kps[kpid].Trend == kPieceTrend.Fall)
            {
                //i所在kp下降，i=kp.end，kp.end上升，说明i在上升趋势的起始，此时以kp+1.end.low作为参照值
                if (i != t && i == _kps[kpid].End && KRise(_kps[kpid].End, 0) > 0)
                {
                    //return Low(i);
                    kpid++;
                    if (KRise(_kps[kpid].End, 0) < 0)
                    {
                        return this[_kps[kpid].End - 1].Low;
                    }
                    return this[_kps[kpid].End].Low;
                }
                else if (i < _kps[kpid].End && KRise(_kps[kpid].End, 0) > 0)
                {
                    return this[_kps[kpid].End - 1].Low;
                }
                else if (i <= _kps[kpid].End && KRise(_kps[kpid].End, 0) <= 0)
                {
                    return this[_kps[kpid].End].Low;
                }
            }

            if (_kps[kpid].Trend == kPieceTrend.Rise)
            {
                //i所在kp上升，i=kp.end，kp.end下降，说明i在下降阶段的起始，此时以kp+1.end.low作为参照值
                if (i != t && i == _kps[kpid].End && KRise(_kps[kpid].End, 0) < 0)
                {
                    kpid++;
                    if (KRise(_kps[kpid].End, 0) > 0)
                    {
                        return this[_kps[kpid].End - 1].Low;
                    }
                    return this[_kps[kpid].End].Low;
                }
                else if (i < _kps[kpid].End && KRise(_kps[kpid].End, 0) < 0)
                {
                    return this[_kps[kpid].End - 1].Low;
                }
                else if (i <= _kps[kpid].End && KRise(_kps[kpid].End, 0) >= 0)
                {
                    return this[_kps[kpid].End].Low;
                }
            }
            ///
            ///以t所在的kp.begin.low（kp上升）或kp.end.low（kp下降）为参照值
            ///
            //kpid = kpSet.findKPieceIndex(t);
            //if (kpSet.kPieces[kpid].Trend == kPieceTrend.Rise)
            //{
            //    refvalue = this[kpSet.kPieces[kpid].Begin].Low;
            //}
            //else
            //{
            //    refvalue = this[kpSet.kPieces[kpid].End].Low;
            //}

            ///
            ///以所在kp.end.low为参照值
            ///
            //i日所在kp下降，且kp.end下降，以kp.end.low作为基准值
            if (_kps[kpid].Trend == kPieceTrend.Fall && KRise(_kps[kpid].End, 0) <= 0)
            {
                refvalue = this[_kps[kpid].End].Low;
            }
            //i日所在kp下降，且kp.end上升
            else if (_kps[kpid].Trend == kPieceTrend.Fall && KRise(_kps[kpid].End, 0) > 0)
            {
                //i非kp.end，且end上升
                //以kp.end-1.low作为基准值
                if (i < _kps[kpid].End)
                {
                    refvalue = this[_kps[kpid].End - 1].Low;
                }
                //i为kp.end，且end上升时需另处理
                else
                {
                    //当前kp非kps中最后一个kp,kp+1上升
                    //kp+1.end.low作为基准值
                    //参与评价的kps中t所在的kp已经以t为end做过修改，所以当kp+1为t所在kp时，t为kp+1.end
                    if (i < t)
                    {
                        if (KRise(_kps[kpid + 1].End, 0) > 0)
                        {
                            refvalue = this[_kps[kpid + 1].End].Low;
                        }
                        else
                        {
                            refvalue = this[_kps[kpid + 1].End - 1].Low;
                        }
                    }
                    //当前i=kp.end=t
                    //参与评价的kps中t所在的kp已经以t为end做过修改，所以当kp为kps最后一个kp时，i为kp.end，t为kp.end
                    else
                    {
                        refvalue = this[i].Low;
                    }
                }
            }

            ///
            ///i日所在kp上升
            ///
            else if (_kps[kpid].Trend == kPieceTrend.Rise && KRise(_kps[kpid].End, 0) > 0)
            {
                refvalue = this[_kps[kpid].End].Low;
            }
            else if (_kps[kpid].Trend == kPieceTrend.Rise && KRise(_kps[kpid].End, 0) <= 0)
            {
                if (i < _kps[kpid].End)
                {
                    refvalue = this[_kps[kpid].End - 1].Low;
                }
                else
                {
                    if (i < t)
                    {
                        if (KRise(_kps[kpid + 1].End, 0) < 0)
                        {
                            refvalue = this[_kps[kpid + 1].End].Low;
                        }
                        else
                        {
                            refvalue = this[_kps[kpid + 1].End - 1].Low;
                        }
                    }
                    else
                    {
                        refvalue = this[i].Low;
                    }
                }
            }


            return refvalue;
        }
    }

}

