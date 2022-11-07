using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    public class MAList_old : List<MA>
    {
        private string _stockcode;
        /// <summary>
        /// MA数据的类型（日数）
        /// </summary>
        private string[] _types;
        /// <summary>
        /// MA数据的类型（日数）
        /// </summary>
        public string[] Types
        {
            get
            {
                return _types;
            }
        }

        public MAList_old(string stockcode, string[] types)
        {
            _stockcode = stockcode;
            _types = types;
        }
    }
    [Serializable]
    public class MAList
    {
        private string _stockcode;
        private float[] _values;
        private string _type;

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public MAList(string stockcode,int count, string type)
        {
            _stockcode = stockcode;
            _type = type;
            _values = new float[count];
        }

        public float this[int index]
        {
            get
            {
                return _values[index];
            }
            set
            {
                _values[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return _values.Length;
            }
        }

    }
}
