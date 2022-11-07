using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class DataKeeper
    {
        private PipeClient _pclient;
        /// <summary>
        /// stock列表
        /// </summary>
        private Dictionary<string, StockInfo> _sidict;
        /// <summary>
        /// stockDataSet列表
        /// </summary>
        private Dictionary<string, StockDataSet> _sdsdict;

        public DataKeeper()
        {
            _pclient = new PipeClient();
            _sidict = _pclient.getStockList();
            _sdsdict = new Dictionary<string, StockDataSet>();
        }

        public Dictionary<string, StockInfo> getStockInfoDict()
        {
            return _sidict;
        }

        public StockDataSet getStockDataSet(string stockcode)
        {
            
            if(_sdsdict.Keys.Contains(stockcode))
            {
                return _sdsdict[stockcode];
            }
            else
            {
                if(_sidict.Keys.Contains(stockcode))
                {
                    _sdsdict.Add(stockcode, _pclient.getData(_sidict[stockcode]));
                    return _sdsdict[stockcode];
                }
                else
                {
                    return null;
                }
            }
        }
        
    }
}
