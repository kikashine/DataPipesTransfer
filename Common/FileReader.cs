using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    public class FileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datapath"></param>
        /// <param name="Source">数据来源，0:同花顺数据文件，1:通达信复权数据文件</param>
        /// <returns></returns>
        public bool HasFile(string datapath, int Source)
        {
            //if (Source == 0)
            //{
            //    return HasFileD1(datapath);
            //}
            if (Source == 1 || Source == 2)
            {
                return HasFileTXT(datapath);
            }
            return false;
        }

        public bool HasFileTXT(string datapath)
        {
            try
            {
                if (!Directory.Exists(datapath))
                {
                    return false;
                }
                else
                {
                    if (Directory.GetFiles(datapath, "*.TXT").Length == 0)
                    {
                        return false;
                    }
                }
            }
            finally
            {

            }
            return true;
        }
        /// <summary>
        /// 存在数据文件
        /// 
        /// </summary>
        public bool HasFileD1(string datapath)
        {
            //FileList D1FileList = this.InitFileList();
            bool extshafile = true;
            bool extshefile = true;
            try
            {

                if (!Directory.Exists(datapath + @"\shase\day"))
                {
                    extshafile = false;

                }
                else
                {
                    // D1FileList.sha = Directory.GetFiles(datapath + @"\shase\day", "*.day");
                    if (Directory.GetFiles(datapath + @"\shase\day", "*.day").Length == 0)
                    {
                        extshafile = false;
                    }
                }

                if (!Directory.Exists(datapath + @"\sznse\day"))
                {
                    extshefile = false;
                }
                else
                {
                    //D1FileList.she = Directory.GetFiles(datapath + @"\sznse\day", "*.day");
                    if (Directory.GetFiles(datapath + @"\sznse\day", "*.day").Length == 0)
                    {
                        extshefile = false;
                    }
                }
                if (!extshafile && !extshefile)
                {
                    return false;
                }
            }
            finally
            {

            }
            return true;
            //return D1FileList;
        }

        public bool ReadFile(string filename, ref KDayDataList kdlist, int Source)
        {
            //if (Source == 0)
            //{
            //    return ReadFileD1(filename, ref FD1BarFile);
            //}
            if (Source == 1 || Source == 2)
            {
                return ReadFileTXT(filename, ref kdlist);
            }

            return false;
        }
        ///// <summary>
        ///// 读取文件中的数据
        ///// 
        ///// </summary>
        //public bool ReadFileD1(string filename, ref D1BarFile FD1BarFile)
        //{
        //    if (!File.Exists(filename))
        //    {
        //        return false;
        //    }
        //    //Thread.Sleep(1000);
        //    FD1BarFile = new D1BarFile();
        //    //Thread.Sleep(1000);
        //    bool FIsSucceed = false;
        //    //Thread.Sleep(1000);
        //    using (FileStream FReader = File.OpenRead(filename))
        //    {
        //        //Thread.Sleep(1000);
        //        FIsSucceed = D1BarFile.Read(ref FD1BarFile, FReader);
        //        //Thread.Sleep(1000);
        //    }

        //    if (!FIsSucceed)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public bool ReadFileTXT(string filename, ref KDayDataList kdlist)
        {
            List<string> strSRLines = new List<string>();
            string[] values;
            if (!File.Exists(filename))
            {
                return false;
            }
            using (StreamReader SReader = File.OpenText(filename))
            {
                while (!SReader.EndOfStream)
                {
                    //strSRLine = SReader.ReadLine();
                    //if (strSRLine.Length < 15)
                    //{
                    //    break;
                    //}
                    strSRLines.Add(SReader.ReadLine());
                    if (strSRLines[strSRLines.Count - 1].Length < 17)
                    {
                        strSRLines.RemoveAt(strSRLines.Count - 1);
                        break;
                    }
                }
                SReader.Close();
            }
            if (strSRLines.Count == 0)
            {
                return false;
            }

//            FD1BarList = new D1BarRecord[strSRLines.Count];
            for (int i = 0; i < strSRLines.Count; i++)
            {
                values = strSRLines[i].Split(',');
                //if (values.Length < 7)
                //{
                //    if (i > 0)
                //    {
                //        FD1BarFile.RecordList[i] = FD1BarFile.RecordList[i - 1];
                //    }
                //    else
                //    {
                //        FD1BarFile.RecordList[i].Open = 0;
                //    }
                //}
                kdlist.Add(new KDayData(values));
                //FD1BarList[i].Date = DateTime.Parse(values[0]);
                //FD1BarList[i].Open = Convert.ToDouble(values[1]);
                //FD1BarList[i].High = Convert.ToDouble(values[2]);
                //FD1BarList[i].Low = Convert.ToDouble(values[3]);
                //FD1BarList[i].Close = Convert.ToDouble(values[4]);
                //FD1BarList[i].Volume = Convert.ToDouble(values[5]);
                //FD1BarList[i].Amount = Convert.ToDouble(values[6]);
                //if (i > 0)
                //{
                //    if (FD1BarFile.RecordList[i - 1].Open == 0)
                //    {
                //        FD1BarFile.RecordList[i - 1] = FD1BarFile.RecordList[i];
                //    }
                //}
            }
            return true;
        }
    }
}
