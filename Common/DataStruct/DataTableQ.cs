using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace StockToolKit.Common
{

    /// <summary>
    /// 准备废弃，由其它类替代
    /// 附加权息数据的DataTable
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]//Sequential有序
    [Serializable()]
    public class DataTableQ : DataTable, ISerializable
    {
        /// <summary>
        /// 是否复权
        /// </summary>
        public bool qx = true;
        /// <summary>
        /// 除权日的日期
        /// </summary>
        public string[] qxDate;
        /// <summary>
        /// 复权参数A
        /// </summary>
        public float[] qxA;
        /// <summary>
        /// 复权参数B
        /// </summary>
        public float[] qxB;
        /// <summary>
        /// 复权信息所在日对应k线数据集合的索引值
        /// </summary>
        public int[] qxKi;
        /// <summary>
        /// a股流通股本变动日期
        /// </summary>
        public string[] cDate;
        /// <summary>
        /// a股流通股本变动数
        /// </summary>
        public long[] Ccapital;
        /// <summary>
        /// a股流通股本变动日对应k线数据集合的索引值
        /// </summary>
        public int[] cKi;

        //public MA ma;

        //public MAVOL mavol;

        //public VOLEnvelope vole;

        //public RSI rsi;

        //public MACD macd;

        //public WR wr;

        //public WaveletList wlist;

        public kPieceSet kpset;

        //new internal readonly DataRowCollection rowCollection;

        private bool DateIndexMapped = false;

        private Hashtable rowindexs;

        public DataTableQ()
        {
            rowindexs = new Hashtable();
        }

        public DataTableQ(string TableName)
        {
            this.TableName = TableName;
            rowindexs = new Hashtable();
        }

        public DataTableQ(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            qx = (bool)info.GetValue("qx", typeof(bool));
            qxDate = (string[])info.GetValue("qxDate", typeof(string[]));
            qxA = (float[])info.GetValue("qxA", typeof(float[]));
            qxB = (float[])info.GetValue("qxB", typeof(float[]));
            qxKi = (int[])info.GetValue("qxKi", typeof(int[]));
            cDate = (string[])info.GetValue("cDate", typeof(string[]));
            Ccapital = (long[])info.GetValue("Ccapital", typeof(long[]));
            cKi = (int[])info.GetValue("cKi", typeof(int[]));
            //ma = (MA)info.GetValue("ma", typeof(MA));
            //mavol = (MAVOL)info.GetValue("mavol", typeof(MAVOL));
            //vole = (VOLEnvelope)info.GetValue("mavol", typeof(VOLEnvelope));
            //rsi = (RSI)info.GetValue("rsi", typeof(RSI));
            //macd = (MACD)info.GetValue("macd", typeof(MACD));
            //wr = (WR)info.GetValue("wr", typeof(WR));
            //wlist = (WaveletList)info.GetValue("wlist", typeof(WaveletList));
            kpset = (kPieceSet)info.GetValue("kpset", typeof(kPieceSet));

            rowindexs = new Hashtable();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("qx", qx);
            info.AddValue("qxDate", qxDate);
            info.AddValue("qxA", qxA);
            info.AddValue("qxB", qxB);
            info.AddValue("qxKi", qxKi);
            info.AddValue("cDate", cDate);
            info.AddValue("Ccapital", Ccapital);
            info.AddValue("cKi", cKi);
            //info.AddValue("ma", ma);
            //info.AddValue("mavol", mavol);
            //info.AddValue("vole", vole);
            //info.AddValue("rsi", rsi);
            //info.AddValue("macd", macd);
            //info.AddValue("wr", wr);
            //info.AddValue("wlist", wlist);
            info.AddValue("kpset", kpset);
            base.GetObjectData(info, ctxt);
        }

        /// <summary>
        /// 返回Date字段值与给定日期相同的数据在数据集中的行号
        /// </summary>
        /// <param name="date"></param>
        /// <returns>
        /// 返回值为k线数据集的长度时，说明指定日期超出k线数据最大日期
        /// 返回值为-1时，说明指定日期不存在于k线数据中
        /// </returns>
        public int RowIndexByDate(string date)
        {
            if (!DateIndexMapped)
            {
                //oldhashcode = this.GetHashCode();

                for (int i = 0; i < this.Rows.Count; i++)
                {
                    lock (rowindexs)
                    {
                        if (!rowindexs.ContainsKey(Utility.toDBDate(this.Rows[i]["Date"].ToString())))
                        {
                            rowindexs.Add(Utility.toDBDate(this.Rows[i]["Date"].ToString()), i);
                        }
                    }
                }
                DateIndexMapped = true;
            }
            if (rowindexs.ContainsKey(Utility.toDBDate(date)))
            {
                return (int)rowindexs[Utility.toDBDate(date)];
            }
            //返回值为k线数据集的长度时，说明指定日期超出k线数据最大日期
            else if (this.Rows.Count == 0 || (this.Rows.Count > 0 && DateTime.Parse(this.Rows[this.Rows.Count - 1]["Date"].ToString()).Ticks < DateTime.Parse(date).Ticks))
            {
                return this.Rows.Count;
            }
            //返回值为-1时，说明指定日期不存在于k线数据中
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 绑定除权数据到k线数据集合
        /// </summary>
        /// <param name="qxinfo"></param>
        public void BindQXInfo_old(StockRCInfo qxinfo, int indexBegin)
        {
            this.qxA = qxinfo.qxA;
            this.qxB = qxinfo.qxB;
            this.qxDate = qxinfo.qxDate;

            //qxDate中第i个日期
            DateTime dti = DateTime.Now;
            //k线数据中第j个数据的日期
            DateTime dtj = DateTime.Now;
            //k线数据中第j-1个数据的日期
            DateTime dtnj = DateTime.Now;

            //与k线数据中某日进行对比的结果
            int comparedt = 2;
            //与k线数据中某日的次日进行对比的结果
            int comparedtn = 2;

            if (qxDate == null || qxDate.Length == 0)
            {
                return;
            }
            qxKi = new int[qxDate.Length];
            ///
            ///将qxDate中每一个日期与k线数据中每一个日期作对照，在qxKi数组中记录每个qxDate中除权日在对应的k线数据中的索引值
            ///
            int j = this.Rows.Count - 1;
            //注意：qxDate中，日期由大到小存储
            for (int i = 0; i < qxDate.Length; i++)
            {
                dti = DateTime.Parse(qxDate[i].ToString());
                //k线数据中，按日期由小到大排序，为配合qxDate的正向循环，k线数据需要反向循环
                for (; j >= indexBegin; j--)
                {

                    //不是第this.Rows.Count - 1条数据，comparedtn上一次循环已经有结果，可以作为此次的comparedt
                    if (j < this.Rows.Count - 1)
                    {
                        comparedt = comparedtn;
                        //j-1大于等于0，才可以进行j-1日的对比
                        if (j > 0)
                        {
                            dtnj = DateTime.Parse(this.Rows[j - 1]["Date"].ToString());

                            if (dti.Ticks > dtnj.Ticks)
                            {
                                //标记为i日大于j-1日
                                comparedtn = 1;
                            }
                            else if (dti.Ticks == dtnj.Ticks)
                            {
                                //标记为i日等于j-1日
                                comparedtn = 0;
                            }
                            else
                            {
                                //标记为i日小于j-1日
                                comparedtn = -1;
                            }
                        }
                        else
                        {
                            comparedtn = 2;
                        }
                    }
                    //第this.Rows.Count - 1条数据时不存在comparedt和comparedtn的对比结果
                    else
                    {
                        dtj = DateTime.Parse(this.Rows[j]["Date"].ToString());

                        if (dti.Ticks > dtj.Ticks)
                        {
                            comparedt = 1;
                        }
                        else if (dti.Ticks == dtj.Ticks)
                        {
                            comparedt = 0;
                        }
                        else
                        {
                            comparedt = -1;
                        }
                        //j-1大于等于0，才可以进行j-1日的对比
                        if (j > 0)
                        {
                            dtnj = DateTime.Parse(this.Rows[j - 1]["Date"].ToString());

                            if (dti.Ticks > dtnj.Ticks)
                            {
                                comparedtn = 1;
                            }
                            else if (dti.Ticks == dtnj.Ticks)
                            {
                                comparedtn = 0;
                            }
                            else
                            {
                                comparedtn = -1;
                            }
                        }
                        else
                        {
                            comparedtn = 2;
                        }
                    }
                    //string1.CompareTo(string2)方法的返回值：1：string1大于string2，0：string1等于string2，-1：string1小于string2

                    //i日大于this.Rows.Count - 1日，除权日对应索引设置为this.Rows.Count - 1
                    if (comparedt == 1 && j == this.Rows.Count - 1)
                    {
                        qxKi[i] = this.Rows.Count;
                        break;
                    }

                    //qxDate的i日与k线数据的j日相等,除权日对应索引设置为j
                    if (comparedt == 0)
                    {
                        qxKi[i] = j;
                        //j--;
                        break;
                    }

                    //i日小于j日和j - 1日，需要继续进行j循环
                    if (comparedt == -1 && comparedtn == -1)
                    {
                        //j--;
                        continue;
                    }
                    //i日处于j日、j-1日中间，除权日对应索引设置为j
                    if (comparedt == -1 && comparedtn == 1)
                    {
                        qxKi[i] = j;
                        //j --;
                        break;
                    }

                }

                if (j < indexBegin && !(j - 1 == qxKi[i] && qxKi[i] >= 0))
                {
                    qxKi[i] = -1;
                }
            }
        }

        /// <summary>
        /// 绑定权息、股本数据到k线数据集合
        /// </summary>
        /// <param name="qxinfo"></param>
        public void BindRCInfo(StockRCInfo rcinfo, int indexBegin)
        {
            this.qxA = rcinfo.qxA;
            this.qxB = rcinfo.qxB;
            this.qxDate = rcinfo.qxDate;
            this.cDate = rcinfo.cDate;
            this.Ccapital = rcinfo.Ccapital;

            //qxDate中第i个日期
            DateTime dti = DateTime.Now;
            //k线数据中第j个数据的日期
            DateTime dtj = DateTime.Now;
            DateTime dtib = DateTime.Now;
            DateTime dtk = DateTime.Now;

            if (qxDate != null && qxDate.Length > 0)
            {
                this.qxKi = new int[qxDate.Length];
            }

            if (cDate != null && cDate.Length > 0)
            {
                this.cKi = new int[cDate.Length];
            }

            ///
            ///将qxDate、cDate中每一个日期与k线数据中每一个日期作对照，在qxKi、cKi数组中记录每个qxDate、cDate中日期在对应的k线数据中的索引值
            ///其中：
            ///除权日、股本变动日小于k线数据最小日期的对除权没有影响
            ///邻近的大于k线数据最大日期的除权日、股本变动日对除权有影响
            ///除权日、股本变动日在k线数据邻近两日之间的，对k线数据两日中较小那一日及之前有影响
            ///

            //权息数据的索引值
            int j = 0;
            //股本数据的索引值
            int k = 0;
            //权息数据索引值对应日期与k线数据对应日期比较后返回的k线索引值
            int comparedj = -3;
            //股本数据索引值对应日期与k线数据对应日期比较后返回的k线索引值
            int comparedk = -3;
            //上一次循环k线数据的索引值
            int bki = indexBegin;
            //上一次循环k线数据的索引值对应的日期
            if (indexBegin <= this.Rows.Count - 1)
            {
                dti = DateTime.Parse(this.Rows[indexBegin]["Date"].ToString());
            }
            //在k线数据中进行日期对比
            for (int i = indexBegin; i <= this.Rows.Count - 1; i++)
            {
                //i>indexbegin时，说明上一次的索引和日期已经得到
                if (i > indexBegin)
                {
                    dtib = dti;
                    bki = i - 1;
                }
                //本次日期
                dti = DateTime.Parse(this.Rows[i]["Date"].ToString());
                //循环qxDate，取得qxDate对应的k线索引
                if (qxDate != null)
                {
                    for (; j <= qxDate.Length - 1; j++)
                    {
                        dtj = DateTime.Parse(qxDate[j]);
                        //将j对应日与i及i-1对应日进行比较，返回j对应日对应k线数据索引值
                        comparedj = DateIndexOfKLine(dtj, i, bki, dti, dtib);
                        //-2表示j日期大于i日期，应暂时中断j循环，进行下一个i循环再和j日期比较
                        if (comparedj == -2)
                        {
                            break;
                        }
                        //
                        else
                        {
                            this.qxKi[j] = comparedj;
                        }
                    }
                }
                //循环cDate，取得cDate对应的k线索引
                if (cDate != null)
                {
                    for (; k <= cDate.Length - 1; k++)
                    {
                        dtk = DateTime.Parse(cDate[k]);
                        //将k对应日与i及i-1对应日进行比较，返回k对应日对应k线数据索引值
                        comparedk = DateIndexOfKLine(dtk, i, bki, dti, dtib);
                        //-2表示k日期大于i日期，应暂时中断j循环，进行下一个i循环再和j日期比较
                        if (comparedk == -2)
                        {
                            break;
                        }
                        //
                        else
                        {
                            this.cKi[k] = comparedk;
                        }
                    }
                }
                //qxDate和cDate均处理完，跳出i循环
                if (
                    (
                        (qxDate != null && j == qxDate.Length)
                        ||
                        qxDate == null
                    )
                    &&
                    (
                        (cDate != null && k == cDate.Length)
                        ||
                        cDate == null
                    )
                    )
                {
                    break;
                }
            }
        }

        private int DateIndexOfKLine(DateTime date, int ki, int bki, DateTime kdate, DateTime bkdate)
        {
            //DateTime kdate=DateTime.Parse(this.Rows[ki]["Date"].ToString());
            //DateTime bkdate=DateTime.Parse(this.Rows[bki]["Date"].ToString());
            int index = -2;
            //date与ki对应日期相同，date对应索引为ki
            if (date.Ticks == kdate.Ticks)
            {
                index = ki;
            }
            //date在k线索引0对应之日之前，date对应索引为-1
            if (ki == 0 && date.Ticks < kdate.Ticks)
            {
                index = -1;
            }
            //date在bki和ki之间，date对应索引为ki
            if (ki > bki && date.Ticks > bkdate.Ticks && date.Ticks < kdate.Ticks)
            {
                index = ki;
            }
            //date在ki之后且ki=this.Rows.count-1 date对应索引为this.Rows.Count
            if (ki == this.Rows.Count - 1 && date.Ticks > kdate.Ticks)
            {
                index = this.Rows.Count;
            }
            //date在ki之后，且ki<this.Rows.count-1，date对应索引设置为-2，表示应继续向后取ki、bki、kdate、bkdate
            if (ki < this.Rows.Count - 1 && date.Ticks > kdate.Ticks)
            {
                index = -2;
            }

            return index;

        }

        /// <summary>
        /// 准备废弃，由KDayDataMaker.PreFQ代替
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public float PreFQ(int t, float value)
        {

            //不需要进行向前复权
            if (!this.qx || this.qxKi == null || this.qxKi.Length == 0)
            {
                return value;
            }
            //复权后的值
            float fqvalue = 0;

            //权息数据按照日期从小到大排序
            for (int i = 0; i < this.qxKi.Length; i++)
            {
                //t大于等于日期最大的除权日，则无需向前复权
                if (i == this.qxKi.Length - 1 && t >= this.qxKi[i])
                {
                    return value;
                }
                //t日小于日期最小的除权日，需要向前复权
                if (
                    (i == 0 && t < this.qxKi[i])
                    //||
                    ////i除权日在k线数据中，而i+1除权日不在k线数据中，则i除权日算是对k线数据有效的最小的除权日
                    //(i < theDT.qxKi.Length - 1 && theDT.qxKi[i] >= 0 && theDT.qxKi[i + 1] < 0 && t < theDT.qxKi[i])
                    )
                {
                    fqvalue = value / this.qxA[i] + this.qxB[i];
                    return Convert.ToSingle(fqvalue.ToString("0.00"));
                }
                //t日在两个除权日之间（区间包括较小的除权日，不包括较大的除权日），需要向前复权
                if (i <= this.qxKi.Length - 2 && t >= this.qxKi[i] && t < this.qxKi[i + 1])
                {
                    fqvalue = value / this.qxA[i + 1] + this.qxB[i + 1];
                    break;
                }
            }

            //return (float)Math.Round(fqvalue,2,MidpointRounding.ToEven);
            return Convert.ToSingle(fqvalue.ToString("0.00"));
            //return fqvalue;
        }
    }


}
