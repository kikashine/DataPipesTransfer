using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class PipeClient
    {
        //private Hashtable stock;

        public PipeClient()
        {
            //this.stock = stock;
        }

        private bool connect(NamedPipeClientStream pipeClient)
        {
            int count=0;
            //int rundom = (new Random()).Next();
            while (!pipeClient.IsConnected)
            {
                try
                {
                    pipeClient.Connect();
                }
                catch(Exception ex)
                {
                    count++;
                }
                if (count > 10)
                {
                    int debug24314;
                }
            }

            return true;
        }
        public string getDataUpdate()
        {
            using (NamedPipeClientStream pipeClient =
                 new NamedPipeClientStream(
                     //"192.168.11.6",
                     "localhost",
                     "testpipe",
                     PipeDirection.InOut, PipeOptions.None,
                     TokenImpersonationLevel.Impersonation))
            using (BinaryWriter dw = new BinaryWriter(pipeClient))
            using (BinaryReader dr = new BinaryReader(pipeClient))
            {
                this.connect(pipeClient);
                //pipeClient.Connect();
                //string cn = dr.ReadString();
                //if (cn == "connected")
                if (pipeClient.IsConnected)
                {
                    //dw.Write("checkUpdate");
                    Write("", pipeClient, dr, dw, "checkUpdate");
                    string date = dr.ReadString();
                    //dw.Write("close");
                    dr.Close();
                    dw.Close();
                    pipeClient.Close();
                    return (date);
                }
                dr.Close();
                dw.Close();
                pipeClient.Close();
            }
            return "1970-01-01 00:00:00";
        }

        public Dictionary<string, StockInfo> getStockList()
        {
            Dictionary<string, StockInfo> stocklist = new Dictionary<string, StockInfo>();
            using (NamedPipeClientStream pipeClient =
                                             new NamedPipeClientStream(
                                             //"192.168.11.6",
                                             "localhost",
                                             "testpipe",
                                             PipeDirection.InOut, PipeOptions.None,
                                             TokenImpersonationLevel.Impersonation))
            using (BinaryWriter dw = new BinaryWriter(pipeClient))
            using (BinaryReader dr = new BinaryReader(pipeClient))
            {
                this.connect(pipeClient);
                //pipeClient.Connect();
                //string cn = dr.ReadString();
                //if (cn == "connected")
                if (pipeClient.IsConnected)
                {
                    //dw.Write("checkUpdate");
                    Write("", pipeClient, dr, dw, "stockList");
                    //得到数据长度
                    //win8 64位下，ReadInt32方法接收数据长度时，只能得到最大65536的int值。server端发送长整形，client端按长整形接受则能正常得到长度。
                    int fl = (int)dr.ReadInt64();
                    BinaryFormatter leafBinaryFormatterTemp = new BinaryFormatter();
                    byte[] bs = new byte[fl];
                    //win8 64位下ReadBytes方法最长只能接收65536字节，Read方法则能正常接收全部数据
                    dr.Read(bs, 0, bs.Length);
                    //byte[] bs = dr.ReadBytes(fl);
                    MemoryStream leafMemoryStreamTemp = new MemoryStream(bs);
                    //leafMemoryStreamTemp.Write(bs, 0, bs.Length);
                    //leafMemoryStreamTemp.Position = 0;
                    stocklist = (Dictionary<string, StockInfo>)leafBinaryFormatterTemp.Deserialize(leafMemoryStreamTemp);

                    leafMemoryStreamTemp.Close();
                    pipeClient.Close();
                }
                dr.Close();
                dw.Close();
                pipeClient.Close();
            }
            return stocklist;
        }
        public StockDataSet getData(StockInfo stock)
        {
            //System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            StockDataSet kl = getDataFromSvr(stock);
            //kb = getDataFromSvr(okb);
            return kl;
        }

        public StockDataSet getDataFromSvr(StockInfo stock)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            //KBase kb = null;
            StockDataSet kl = null;
            //StockDataSet sset = new StockDataSet();
            string date = "";


            using (NamedPipeClientStream pipeClient =
                     new NamedPipeClientStream(
                // "192.168.11.6", 
                         "localhost",
                         "testpipe",
                         PipeDirection.InOut, PipeOptions.None,
                         TokenImpersonationLevel.Impersonation))
            using (BinaryWriter dw = new BinaryWriter(pipeClient))
            using (BinaryReader dr = new BinaryReader(pipeClient))
            {

                //pipeClient.Connect();
                this.connect(pipeClient);
                if (pipeClient.IsConnected)
                {
                    Write(stock.StockCode, pipeClient, dr, dw,
                        "getData"
                        + "_" + stock.StockCode
                        + "_" + stock.MarketCode
                        );


                    //得到数据长度
                    //win8 64位下，ReadInt32方法接收数据长度时，只能得到最大65536的int值。server端发送长整形，client端按长整形接受则能正常得到长度。
                    int fl = (int)dr.ReadInt64();
                    BinaryFormatter leafBinaryFormatterTemp = new BinaryFormatter();
                    byte[] bs = new byte[fl];
                    //win8 64位下ReadBytes方法最长只能接收65536字节，Read方法则能正常接收全部数据
                    dr.Read(bs, 0, bs.Length);
                    //byte[] bs = dr.ReadBytes(fl);
                    MemoryStream leafMemoryStreamTemp = new MemoryStream(bs);
                    //leafMemoryStreamTemp.Write(bs, 0, bs.Length);
                    //leafMemoryStreamTemp.Position = 0;
                    kl = (StockDataSet)leafBinaryFormatterTemp.Deserialize(leafMemoryStreamTemp);
                    leafMemoryStreamTemp.Close();
                    pipeClient.Close();
                }
                dr.Close();
                dw.Close();
                pipeClient.Close();
            }

            return kl;
        }

        private void Write(string stockcode, NamedPipeClientStream pipeClient, BinaryReader dr, BinaryWriter dw, string txt)
        {
            dw.Write(txt);
            dw.Flush();
        }
    }
}
