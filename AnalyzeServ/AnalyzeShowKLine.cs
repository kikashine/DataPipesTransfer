using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class AnalyzeShowKLine:KBase
    {

        public AnalyzeShowKLine(KBase kbase)
            :base(kbase)
        {

        }
        //private bool ShowKLine(Hashtable stock, ref AnalyzeParameters Param)
        //{

        //    if (ShowKLine_OneDayModel(stock, ref Param))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public List<object> ShowKLine_OneDayModel(int indexAnalyze)
        {
            List<object> ResultList = new List<object>();

            float buy = 0;
            float sell = 0;
            float rise1 = 0;
            float rise2 = 0;
            if (indexAnalyze + 1 <= this.Count - 1)
            {
                if (KRise(indexAnalyze + 1, 0) < 0)
                {
                    buy = (Highest(indexAnalyze + 1) + Open(indexAnalyze + 1)) / 2.01f;
                }
                else
                {
                    buy = Open(indexAnalyze + 1);
                }

                sell = Close(indexAnalyze + 1);
                rise1 = DvalueRatio(sell, buy);
            }
            if (indexAnalyze + 2 <= this.Count - 1)
            {
                if (KRise(indexAnalyze + 2, 0) >= 0)
                {
                    sell = (Highest(indexAnalyze + 2) + Close(indexAnalyze + 2)) / 2;
                }
                else
                {
                    sell = (Highest(indexAnalyze + 2) + Open(indexAnalyze + 2)) / 2;
                }
                rise1 = DvalueRatio(sell, buy);
            }

            if (indexAnalyze + 3 <= this.Count - 1)
            {
                if (KRise(indexAnalyze + 3, 0) >= 0)
                {
                    sell = (Highest(indexAnalyze + 3) + Close(indexAnalyze + 3)) / 2;
                }
                else
                {
                    sell = (Highest(indexAnalyze + 3) + Open(indexAnalyze + 3)) / 2;
                }
                rise2 = DvalueRatio(sell, buy);
            }

            //if (rise1 >= 0 && rise1 > rise2)
            //{
            //    rise2 = rise1;
            //}
            ResultList.Add(Percent(rise1) + "%");
            ResultList.Add(Percent(rise2) + "%");
            //ResultList.Add(risePerInDays(indexAnalyze+1, 1));
            //ResultList.Add(risePerInDays(indexAnalyze + 1, 3));
            ResultList.Add("k");
            ResultList.Add("k");

            return ResultList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="ColumnText"></param>
        /// <param name="ColumnSize"></param>
        /// <param name="ColumnSort"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int BuildColumn(ref List<string> ColumnName, ref List<string> ColumnText, ref List<int> ColumnSize, ref List<string> ColumnSort, int InfoOffset)
        {
            ColumnName.Insert(InfoOffset, "1日涨");
            ColumnText.Insert(InfoOffset, "1日涨");
            ColumnSize.Insert(InfoOffset, 45);
            ColumnSort.Insert(InfoOffset, "per");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "3日涨");
            ColumnText.Insert(InfoOffset, "3日涨");
            ColumnSize.Insert(InfoOffset, 45);
            ColumnSort.Insert(InfoOffset, "per");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "mode");
            ColumnText.Insert(InfoOffset, "mode");
            ColumnSize.Insert(InfoOffset, 60);
            ColumnSort.Insert(InfoOffset, "str");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "func");
            ColumnText.Insert(InfoOffset, "func");
            ColumnSize.Insert(InfoOffset, 45);
            ColumnSort.Insert(InfoOffset, "str");
            InfoOffset++;

            return InfoOffset;
        }
    }
}
