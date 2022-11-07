using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    public class KDayData
    {
        private float _open;

        private float _close;

        private float _high;

        private float _low;

        private float _highest;

        private float _lowest;

        private double _volume;

        private double _amount;

        private DateTime _date;

        private long _Ccapital;

        /// <summary>
        /// 本日是否复权日
        /// </summary>
        private bool _isaday;
        /// <summary>
        /// 本日是否股本变动日
        /// </summary>
        private bool _iscday;

        public float Open
        {
            get
            {
                return _open;
            }
            set
            {
                _open = value;
            }
        }

        public float Close
        {
            get
            {
                return _close;
            }
            set
            {
                _close = value;
            }
        }

        public float High
        {
            get
            {
                return _high;
            }
            set
            {
                _high = value;
            }
        }

        public float Low
        {
            get
            {
                return _low;
            }
            set
            {
                _low = value;
            }
        }

        public float Highest
        {
            get
            {
                return _highest;
            }
            set
            {
                _highest = value;
            }
        }

        public float Lowest
        {
            get
            {
                return _lowest;
            }
            set
            {
                _lowest = value;
            }
        }

        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
            }
        }
        public double Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
            }
        }
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
        public long Ccapital
        {
            get
            {
                return _Ccapital;
            }
            set
            {
                _Ccapital = value;
            }
        }

        public bool isAday
        {
            get
            {
                return _isaday;
            }
        }

        public bool isCday
        {

            get
            {
                return _iscday;
            }
        }
        public KDayData(float Open, float Close, float Highest, float Lowest, double Volume, double Amount, DateTime Date, bool isaday = false, bool iscday=false, long Ccapital = 0)
        {
            _open = Open;
            _close = Close;
            _highest = Highest;
            _lowest = Lowest;
            _volume = Volume;
            _amount = Amount;
            _date = Date;
            _Ccapital = Ccapital;
            _isaday = isaday;
            _iscday = iscday;
            if (_open <= _close)
            {
                _high = _close;
                _low = _open;
            }
            else
            {
                _high = _open;
                _low = Close;
            }
        }
        /// <summary>
        /// 准备废弃，由KDayDataMaker.make方法替代
        /// </summary>
        /// <param name="theDT"></param>
        /// <param name="t"></param>
        //public KDayData(DataTableQ theDT, int t)
        //{
        //    _open = theDT.PreFQ(t, Convert.ToSingle(theDT.Rows[t]["Open"]));
        //    _close = theDT.PreFQ(t, Convert.ToSingle(theDT.Rows[t]["Close"]));
        //    _highest = theDT.PreFQ(t, Convert.ToSingle(theDT.Rows[t]["High"]));
        //    _lowest = theDT.PreFQ(t, Convert.ToSingle(theDT.Rows[t]["Low"]));
        //    _volume = Convert.ToDouble(theDT.Rows[t]["Volume"]);
        //    _amount = Convert.ToDouble(theDT.Rows[t]["Amount"]);
        //    _date = DateTime.Parse(theDT.Rows[t]["Date"].ToString());
        //    if (_open <= _close)
        //    {
        //        _high = _close;
        //        _low = _open;
        //    }
        //    else
        //    {
        //        _high = _open;
        //        _low = Close;
        //    }
        //    /// <summary>
        //    /// 得到t日对应的a股流通股本（单位：股）

        //    if (theDT.cKi == null || theDT.cKi.Length == 0)
        //    {
        //        _Ccapital = 0;
        //    }
        //    //long cap = 0;
        //    //循环查找t日落在哪个流通股本区间内
        //    for (int i = 0; i < theDT.cKi.Length; i++)
        //    {
        //        //t日在[i,i+1)区间内时，取i对应的流通股本
        //        if (i + 1 <= theDT.cKi.Length - 1 && t >= theDT.cKi[i] && t < theDT.cKi[i + 1])
        //        {
        //            _Ccapital = theDT.Ccapital[i];
        //        }
        //        //i是最后一个股本变动日，t日大于等于i日
        //        if (i == theDT.cKi.Length - 1 && t >= theDT.cKi[i])
        //        {
        //            _Ccapital = theDT.Ccapital[i];
        //        }
        //    }

        //}
        /// <summary>
        /// 准备废弃，由KDayDataMaker.make方法替代
        /// </summary>
        /// <param name="value"></param>
        public KDayData(string[] values)
        {
            _open = Convert.ToSingle(values[1]);
            _close = Convert.ToSingle(values[4]);
            _highest = Convert.ToSingle(values[2]);
            _lowest = Convert.ToSingle(values[3]);
            _volume = Convert.ToDouble(values[5]);
            _amount = Convert.ToDouble(values[6]);
            _date = DateTime.Parse(values[0]);
            if (_open <= _close)
            {
                _high = _close;
                _low = _open;
            }
            else
            {
                _high = _open;
                _low = Close;
            }
        }
    }

}
