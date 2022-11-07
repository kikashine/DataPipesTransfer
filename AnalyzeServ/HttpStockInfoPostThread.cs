using System.Threading;
using System.Net;
using System.IO;
using System.Text;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace StockToolKit.Analyze
{
    class HttpStockInfoPostThread
    {
        private int interval;
        private string stockCodes;
        public Hashtable stockinfo;

        public HttpStockInfoPostThread()
        {
            //this.interval = interval;
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            stockinfo = new Hashtable();
        }
        /// <summary>
        /// 从新浪金融得到数据的方法
        /// </summary>
        /// <param name="stockCodes"></param>
        public void runsina(string stockCodes)
        {
            string stockWebService_Url = @"http://hq.sinajs.cn/list=" ;
            string[] scslpited = stockCodes.Split(',');
            //整理股票列表
            for (int i = 0; i <= scslpited.Length - 1; i++)
            {
                if (i == 0)
                {
                    if (!scslpited[i].StartsWith("sh") && !scslpited[i].StartsWith("sz"))
                    {
                        if (scslpited[i].StartsWith("6"))
                        {
                            stockWebService_Url =stockWebService_Url + "sh" + scslpited[i];
                        }
                        else
                        {
                            stockWebService_Url =stockWebService_Url + "sz" + scslpited[i];
                        }
                    }
                    else
                    {
                        stockWebService_Url = scslpited[i];
                    }
                }
                else
                {
                    if (!scslpited[i].StartsWith("sh") && !scslpited[i].StartsWith("sz"))
                    {
                        if (scslpited[i].StartsWith("6"))
                        {
                            stockWebService_Url =stockWebService_Url + ",sh" + scslpited[i];
                        }
                        else
                        {
                            stockWebService_Url =stockWebService_Url +  ",sz" + scslpited[i];
                        }
                    }
                    else
                    {
                        stockWebService_Url =stockWebService_Url + "," + scslpited[i];
                    }
                }
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(stockWebService_Url);
            req.Method = "GET";
            req.AllowAutoRedirect = false;

            HttpWebResponse res;

            try
            {

                res = (HttpWebResponse)req.GetResponse();

            }
            catch (WebException ex)
            {

                res = (HttpWebResponse)ex.Response;

            }

            if (res.StatusCode != HttpStatusCode.OK)
            {
                stockinfo = null;
            }
            else
            {
                using (Stream smRes = res.GetResponseStream())
                {
                    string line;
                    string[] strinfo1;
                    string[] strinfodetail;
                    System.IO.StreamReader respStreamReader = new StreamReader(smRes, Encoding.Default);
                    while (!respStreamReader.EndOfStream)
                    {
                        line = respStreamReader.ReadLine();
                        line = line.Replace("\"", "");
                        line = line.Replace("\\", "");
                        line = line.Replace(";", "");
                        strinfo1 = line.Split('=');
                        strinfo1[0] = strinfo1[0].Substring(strinfo1[0].LastIndexOf('_') + 1);
                        strinfo1[0] = strinfo1[0].Replace("sz", "");
                        strinfo1[0] = strinfo1[0].Replace("sh", "");
                        strinfodetail = strinfo1[1].Split(',');
                        if (strinfodetail.Length >= 32)
                        {
                            StockInfoFromHttpReq sinfo = new StockInfoFromHttpReq(strinfo1[0], strinfodetail);
                            stockinfo.Add(strinfo1[0], sinfo);
                        }
                        else
                        {
                            stockinfo.Add(strinfo1[0], null);
                        }
                    }
                }

            }
            res.Close();
            res = null;
            req.Abort();
            req = null;
        }
    }
}
