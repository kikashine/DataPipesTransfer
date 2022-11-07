using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    public class AnalyzeResult
    {
        /// <summary>
        /// 结果日对应的k线索引
        /// </summary>
        private int _i;

        private string _stockcode;

        public AnalyzeResult(string stockcode, int index)
        {
            _stockcode = stockcode;
            _i = index;
        }
        /// <summary>
        /// 结果日对应的k线索引
        /// </summary>
        public int Index
        {
            get
            {
                return _i;
            }
        }

        public string StockCode
        {
            get
            {
                return _stockcode;
            }
        }

    }
}
