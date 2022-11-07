using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;

namespace StockToolKit.Analyze
{
    public class StockInfoFromHttpReq
    {
        public string code;
        public string name;
        public DateTime time;
        public float price;
        public float close;
        public float open;
        public float diff;
        public float low;
        public float high;
        public string diffper;
        public long volume;
        public float quantity;
        public float buy;
        public float sell;
        public string weibi;
        public string buy1;
        public string buy2;
        public string buy3;
        public string buy4;
        public string buy5;
        public string sell1;
        public string sell2;
        public string sell3;
        public string sell4;
        public string sell5;


        public StockInfoFromHttpReq(XmlDocument doc)
        {
            parseStockInfo(doc);
        }

        public StockInfoFromHttpReq(string[] info)
        {
            name = info[0];
            price = Convert.ToSingle(info[3]);
        }

        public StockInfoFromHttpReq(string cd,string[] info)
        {
            code = cd;
            name = info[0];
            time = DateTime.Parse(info[30] + " " + info[31]);
            open = Convert.ToSingle(info[1]);
            price = Convert.ToSingle(info[3]);
            volume = Convert.ToInt64(info[8]);
        }

        private bool parseStockInfo(XmlDocument doc){
            XmlNode currentNode = doc.ChildNodes[0].FirstChild;
            code = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            name = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            time = DateTime.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            price = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            close = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            open = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            diff = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            low = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            high = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            diffper = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            volume = long.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            quantity = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            buy = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            sell = float.Parse(currentNode.InnerXml);
            currentNode = currentNode.NextSibling;
            weibi = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            buy1 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            buy2 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            buy3 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            buy4 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            buy5 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            sell1 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            sell2 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            sell3 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            sell4 = currentNode.InnerXml;
            currentNode = currentNode.NextSibling;
            sell5 = currentNode.InnerXml;

            return true;
        }
    }
}
