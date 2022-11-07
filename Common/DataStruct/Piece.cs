using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    /// <summary>
    /// 片段的基础类，用于k线、ma线等的升降段分段
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// 片段起始日索引
        /// 在股票数据集合（StockDataSet）中的索引
        /// </summary>
        protected int _idxbegin;
        /// <summary>
        /// 片段结束日索引
        /// 在股票数据集合（StockDataSet）中的索引
        /// </summary>
        protected int _idxend;

        /// <summary>
        /// 生成片段的实例
        /// </summary>
        /// <param name="idxbegin">片段起始日索引</param>
        /// <param name="idxend">片段结束日索引</param>
        public Piece(int idxbegin, int idxend)
        {
            this._idxbegin = idxbegin;
            this._idxend = idxend;
        }

        /// <summary>
        /// 获得片段起始日索引
        /// 在股票数据集合（StockDataSet）中的索引
        /// </summary>
        public int idxBegin
        {
            get
            {
                return this._idxbegin;
            }
        }
        /// <summary>
        /// 获得片段结束日索引
        /// 在股票数据集合（StockDataSet）中的索引
        /// </summary>
        public int idxEnd
        {
            get
            {
                return this._idxend;
            }
        }

    }
}
