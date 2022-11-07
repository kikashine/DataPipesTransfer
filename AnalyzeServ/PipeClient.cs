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

            //theDT存在数据，发送数据最后一日日期检查是否需要更新
            //if (odt != null && odt.Rows.Count > 0)
            //{
            //    date = odt.Rows[odt.Rows.Count - 1]["Date"].ToString();
            //}

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

                    //string xmlstring = dr.ReadString();
                    //XmlSerializer xmlSerializer = new XmlSerializer(typeof(KBase));
                    //using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)))
                    //{
                    //    using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                    //    {
                    //        Object obj = xmlSerializer.Deserialize(xmlReader);
                    //        kb = (KBase)obj;
                    //    }
                    //}

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
                    //if (kl != null && kl.Count > 0)
                   // {
                        //sset.kDayDataList = kl;
                        //sset.StockCode = kl.StockCode;
                        //sset.kpSet = new kPieceSet(kl, 0, kl.Count - 1);
                        //sset.ma = new MA(kl, 0, kl.Count - 1);
                        //sset.avgNAmpList = new AvgNeighborAmpList(kl, 0, kl.Count - 1);
                    //}
                    //if (dt != null && dt.TableName != "")
                    //{
                    //    dt.ma = new MA(ref dt, 0, dt.Rows.Count - 1);
                    //    //dt.mavol = new MAVOL(ref dt, 0, dt.Rows.Count - 1);
                    //    dt.vole = new VOLEnvelope(ref dt, 0, dt.Rows.Count - 1);
                    //    //dt.rsi = new RSI(dt, 0, dt.Rows.Count - 1);
                    //    //dt.macd = new MACD(dt, 0, dt.Rows.Count - 1);
                    //    //dt.wr = new WR(dt, 0, dt.Rows.Count - 1);
                    //    dt.wlist = new WaveletList(ref dt, 0, dt.Rows.Count - 1, true, 0);
                    //    dt.kpset = new kPieceSet(ref dt, 0, dt.Rows.Count - 1);
                    //}
                    leafMemoryStreamTemp.Close();
                    pipeClient.Close();
                }

                //string cn = dr.ReadString();
                //bool ok = false;
                //if (cn == "connected")
                //{
                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, "cthreadname");
                //    string cnstate = dr.ReadString();
                //    if (cnstate == "reconnect")
                //    {
                //        dt = new DataTableQ();
                //        dt.TableName = "reconnect";
                //        pipeClient.Close();
                //        return dt;
                //    }
                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, Thread.CurrentThread.Name);

                //    //发送股票代码和市场代码
                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, "StockCode");

                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, stock["StockCode"].ToString());

                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, "MarketCode");

                //    Write(stock["StockCode"].ToString(), pipeClient, dr, dw, stock["MarketCode"].ToString());

                //    //检查是否需要更新
                //    bool getData = false;
                //    if (date != "getdate" && date != "")
                //    {
                //        //ok = Write(pipeClient, dr, dw, "checkUpdate");
                //        //if (!ok)
                //        //{
                //        //    int debug2375475 = 1;
                //        //}
                //        ////dw.Write("checkUpdate");
                //        ////dw.Flush();
                //        ////Write(pipeClient, dr, dw, date);
                //        ////dw.Write(date);
                //        ////dw.Flush();
                //        ////DateTime dt1 = DateTime.Now;
                //        //if (dr.ReadString() == "needUpdate")
                //        //{
                //        //    getData = true;
                //        //}
                //        //DateTime dt2 = DateTime.Now;
                //        //long ct1 = (dt2.Ticks - dt1.Ticks) / 10000;
                //        //if (ct1 > 10)
                //        //{
                //        //    int debug234204 = 0;
                //        //}
                //    }
                //    else
                //    {
                //        getData = true;
                //    }
                //    //取得数据
                //    if (getData)
                //    {
                //        Write(stock["StockCode"].ToString(), pipeClient, dr, dw, "getData");

                //        //得到数据长度
                //        int fl = dr.ReadInt32();
                //        BinaryFormatter leafBinaryFormatterTemp = new BinaryFormatter();
                //        byte[] bs = dr.ReadBytes(fl);
                //        MemoryStream leafMemoryStreamTemp = new MemoryStream(bs);
                //        dt = (DataTableQ)leafBinaryFormatterTemp.Deserialize(leafMemoryStreamTemp);
                //        if (dt != null && dt.TableName != "")
                //        {
                //            dt.ma = new MA(dt, 0, dt.Rows.Count - 1);
                //            dt.wlist = new WaveletList(dt, 0, dt.Rows.Count - 1, true, 0);
                //        }
                //        leafMemoryStreamTemp.Close();
                //        pipeClient.Close();

                //    }
                //    else
                //    {
                //        Write(stock["StockCode"].ToString(), pipeClient, dr, dw, "close");
                //        if (!ok)
                //        {
                //            int debug2375475 = 1;
                //        }
                //        pipeClient.Close();
                //    }
                //}
                //else
                //{

                //}
                dr.Close();
                dw.Close();
                pipeClient.Close();
            }

            return kl;
        }

        private void Write(string stockcode, NamedPipeClientStream pipeClient, BinaryReader dr, BinaryWriter dw, string txt)
        {
            //if (!pipeClient.IsConnected)
            //{
            //    this.connect(pipeClient);
            //    string cn = dr.ReadString();
            //    if (cn == "connected")
            //    {
            //        dw.Write(txt);
            //        dw.Flush();
            //    }
            //}
            //else
            //{
            //bool ok = false;
            dw.Write(txt);
            dw.Flush();
            //dw.Close();
            //if (dr.ReadString() == txt + "_ok")
            //{
            //    ok = true;
            //}


            //}

        }
    }
}
