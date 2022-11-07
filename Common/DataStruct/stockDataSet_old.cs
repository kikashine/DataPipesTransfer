using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    /// <summary>
    /// 股票数据集合
    /// </summary>
    public struct StockDataSet_old
    {
        public string StockCode;
        /// <summary>
        /// k线数据列表
        /// </summary>
        public KDayDataList kDayDataList;

        /// <summary>
        /// k线片段列表
        /// </summary>
        public kPieceSet kpSet;

        /// <summary>
        /// 日周围平均日k线幅度集合
        /// </summary>
        //public AvgNeighborAmpList avgNAmpList;

        /// <summary>
        /// 简单移动平均值集合
        /// </summary>
        public MA_old ma;
    }
}
