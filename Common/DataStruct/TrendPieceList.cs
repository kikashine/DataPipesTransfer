using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    /// <summary>
    /// 升降趋势分片列表
    /// </summary>
    [Serializable]
    public class TrendPieceList :THashTable<TrendPiece>
    {
        private string _stockcode;
        private string _type;

        public string Type
        {
            get
            {
                return _type;
            }
        }
        public string StockCode
        {
            get
            {
                return _stockcode;
            }
        }
        public TrendPieceList(string stockcode, string type)
        {
            _stockcode = stockcode;
            _type = type;
        }
    }
}
