using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    public class TrendPiece:Piece
    {
        /// <summary>
        /// 片段的趋势
        /// </summary>
        protected Trend _trend;
        /// <summary>
        /// 生成片段的实例
        /// </summary>
        /// <param name="idxbegin">片段起始日索引</param>
        /// <param name="idxend">片段结束日索引</param>
        /// <param name="trend">片段的趋势</param>
        public TrendPiece(int idxbegin, int idxend, Trend trend) : base(idxbegin, idxend)
        {
            this._trend = trend;
        }

        /// <summary>
        /// 获得片段的趋势
        /// </summary>
        public Trend Trend
        {
            get
            {
                return this._trend;
            }
        }
    }
}
