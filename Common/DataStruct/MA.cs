using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    [Serializable]
    public class MA
    {

        private float[] _values;

        public float[] Values
        {
            get
            {
                return _values;
            }
        }
       

        public MA(float[] values)
        {
            _values = values;
        }
    }
}
