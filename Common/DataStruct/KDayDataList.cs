using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace StockToolKit.Common
{
    [Serializable]
    public class KDayDataList : List<KDayData>
    {
        public string StockCode = "";
        private bool dateIndexMapped = false;
        private int _maxvolumeindex = 0;
        protected Hashtable rowindexs;

        public KDayDataList()
            : base()
        {
            rowindexs = new Hashtable();
        }

        public KDayDataList(string StockCode)
            : base()
        {
            rowindexs = new Hashtable();
            this.StockCode = StockCode;
        }

        public int MaxVolumeIndex
        {
            get
            {
                return _maxvolumeindex;
            }
        }

        public new void Add(KDayData kd)
        {
            base.Add(kd);
            if(this[_maxvolumeindex].Volume<=kd.Volume)
            {
                _maxvolumeindex = this.Count - 1;
            }
            lock (rowindexs)
            {
                if (!rowindexs.ContainsKey(kd.Date))
                {
                    rowindexs.Add(kd.Date, this.Count);
                }
            }
        }

        public int DateToIndex(DateTime date)
        {
            if (rowindexs.ContainsKey(date))
            {
                return (int)rowindexs[date];
            }
            //返回值为k线数据集的长度时，说明指定日期超出k线数据最大日期
            else if (this.Count == 0 || (this.Count > 0 && this[this.Count - 1].Date.Ticks < date.Ticks))
            {
                //return _list.Count;
                return -1;
            }
            //返回值为-1时，说明指定日期不存在于k线数据中
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 准备废弃，由KDayDataListMaker.make代替
        /// </summary>
        /// <param name="theDT"></param>
        //public void Fill(DataTable theDT)
        //{
        //    for (int i = 0; i < theDT.Rows.Count; i++)
        //    {
        //        this.Add(new KDayData(theDT, i));
        //    }
        //    this.StockCode = theDT.TableName;
        //}

        /// <summary>
        /// 准备废弃，由DateToIndex替代
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int RowIndexByDate(string date)
        {
            if (!dateIndexMapped)
            {
                //oldhashcode = this.GetHashCode();
                rowindexs = new Hashtable();
                for (int i = 0; i < this.Count; i++)
                {
                    lock (rowindexs)
                    {
                        if (!rowindexs.ContainsKey(this[i].Date))
                        {
                            rowindexs.Add(this[i].Date, i);
                        }
                    }
                }
                dateIndexMapped = true;
            }
            if (rowindexs.ContainsKey(Utility.toDBDate(date)))
            {
                return (int)rowindexs[Utility.toDBDate(date)];
            }
            //返回值为k线数据集的长度时，说明指定日期超出k线数据最大日期
            else if (this.Count == 0 || (this.Count > 0 && this[this.Count - 1].Date.Ticks < DateTime.Parse(date).Ticks))
            {
                //return _list.Count;
                return -1;
            }
            //返回值为-1时，说明指定日期不存在于k线数据中
            else
            {
                return -1;
            }
        }
    }
}
