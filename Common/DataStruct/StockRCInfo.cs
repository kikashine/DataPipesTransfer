using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace StockToolKit.Common
{
    /// <summary>
    /// 一个股票的权息、股本数据
    /// </summary>
    [Serializable()]
    public class StockRCInfo
    {
        //除权日的日期
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
        /// a股流通股变化日的日期
        /// </summary>
        public string[] cDate;
        /// <summary>
        /// A股流通股数量
        /// </summary>
        public long[] Ccapital;
        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode;
        /// <summary>
        /// 复权信息所在日对应k线数据集合的索引值
        /// </summary>
        public int[] qxKi;
        /// <summary>
        /// a股流通股本变动日对应k线数据集合的索引值
        /// </summary>
        public int[] cKi;

        /// <summary>
        /// 一个股票的权息、股本数据
        /// </summary>
        /// <param name="qxDate">除权日的日期</param>
        /// <param name="qxA">复权参数A</param>
        /// <param name="qxB">复权参数B</param>
        /// <param name="cDate">a股流通股变化日的日期</param>
        /// <param name="Ccapital">A股流通股数量</param>
        /// <param name="qxKi">复权信息所在日对应k线数据集合的索引值</param>
        /// <param name="cKi">a股流通股本变动日对应k线数据集合的索引值</param>
        /// <param name="StockCode">股票代码</param>
        public StockRCInfo(string[] qxDate, float[] qxA, float[] qxB, string[] cDate, long[] Ccapital, int[] qxKi, int[] cKi, string StockCode)
        {
            this.qxDate = qxDate;
            this.qxA = qxA;
            this.qxB = qxB;
            this.cDate = cDate;
            this.Ccapital = Ccapital;
            this.StockCode = StockCode;
            this.qxKi = qxKi;
            this.cKi = cKi;
        }

       
    }
}
