using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    public class StockInfo
    {
        private string _stockcode;

        private string _marketcode;

        private string _industry;

        public StockInfo(string stockcode,string marketcode, string industry)
        {
            _stockcode = stockcode;
            _marketcode = marketcode;
            _industry = industry;
        }

        public string StockCode
        {
            get
            {
                return _stockcode;
            }
        }

        public string MarketCode
        {
            get
            {
                return _marketcode;
            }
        }

        public string Industry
        {
            get
            {
                return _industry;
            }
            set
            {
                _industry = value;
            }
        }
    }
}
