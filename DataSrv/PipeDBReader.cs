using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using StockToolKit.Common;

namespace StockToolKit.DataSrv
{
    class PipeDBReader
    {
        private Stream m_stream;
        private Hashtable m_stock;
        private int m_Source;
        private StockDataSet sds;

        public PipeDBReader()
        {
        }

        public PipeDBReader(Stream stream, Hashtable stock, int Source)
        {
            m_stream = stream;
            m_stock = stock;
            m_Source = Source;
        }

        public void GetData()
        {

            sds = new StockDataSet(null,null,null,null, m_stock["StockCode"].ToString());
            byte[] kd = ToBytes(sds);
            using (BinaryWriter dw = new BinaryWriter(m_stream))
            {
                dw.Write((long)kd.Length);
                dw.Flush();
                dw.Write(kd);
                dw.Flush();
            }
        }
        private byte[] ToBytes(StockDataSet sds)
        {
            byte[] leafMyClassByteTemp;
            MemoryStream leafMemoryStreamTemp = new MemoryStream();
            BinaryFormatter leafBinaryFormatterTemp = new BinaryFormatter();
            leafBinaryFormatterTemp.Serialize(leafMemoryStreamTemp, sds);
            leafMyClassByteTemp = leafMemoryStreamTemp.GetBuffer();
            return leafMyClassByteTemp;
        }
    }
}
