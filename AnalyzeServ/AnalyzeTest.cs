using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class AnalyzeTest:KBase
    {
        private KBase _rkb;
        public AnalyzeTest(KBase kbase)
            :base(kbase)
        {

        }
        //private bool Test(Hashtable stock, ref AnalyzeParameters Param)
        //{

        //    if (Test_OneDayModel(stock, ref Param))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public List<object> Test_OneDayModel(int indexAnalyze, ReferenceStockInfo refsi)
        {
            List<object> ResultList = new List<object>();
            //因为modeltest需要rt、t日所在的kpiece改为rt、t日结尾，所以需要先备份rt、t日所在的kpiece
            //int rtkpid = refsi.kpSet.findKPieceIndex(refsi.t);
            int tkpid = kpSet.findKPieceIndex(indexAnalyze);
            //kPiece back_rtkp = new kPiece();
            kPiece back_tkp = new kPiece();
            //AvgNeighborAmp back_ranampt = new AvgNeighborAmp();
            AvgNeighborAmp back_anampt = new AvgNeighborAmp();
            //AvgNeighborAmp back_ranamptp = new AvgNeighborAmp();
            AvgNeighborAmp back_anamptp = new AvgNeighborAmp();
            //AvgNeighborAmp back_ranamptpp = new AvgNeighborAmp();
            //AvgNeighborAmp back_anamptpp = new AvgNeighborAmp();

            //return null;

                //back_rtkp = (kPiece)refsi.kpSet.kPieces[rtkpid].Clone();
                back_tkp = (kPiece)kpSet.kPieces[tkpid].Clone();
                //back_ranampt = (AvgNeighborAmp)refsi.avgNAmpList[refsi.t].Clone();
                back_anampt = (AvgNeighborAmp)this.avgNAmpList[indexAnalyze].Clone();
                if (refsi.t > 0)
                {
                    //back_ranamptp = (AvgNeighborAmp)refsi.avgNAmpList[refsi.t - 1].Clone();
                }
                if (indexAnalyze > 0)
                {
                    back_anamptp = (AvgNeighborAmp)this.avgNAmpList[indexAnalyze - 1].Clone();
                }

                //if (back_rtkp.Begin != refsi.t)
                //{
                //    refsi.kpSet.kPieces[rtkpid] = refsi.kpSet.genkPiece(back_rtkp.Begin, refsi.t);
                //}
                if (back_tkp.Begin != indexAnalyze)
                {
                    kpSet.kPieces[tkpid] = kpSet.genkPiece(back_tkp.Begin, indexAnalyze);
                }
                //if (refsi.t - 2 >= 0)
                //{
                //    refsi.avgNAmpList[refsi.t] = refsi.avgNAmpList[refsi.t - 2];
                //    refsi.avgNAmpList[refsi.t - 1] = refsi.avgNAmpList[refsi.t - 2];
                //}
                if (indexAnalyze - 2 >= 0)
                {
                    this.avgNAmpList[indexAnalyze] = this.avgNAmpList[indexAnalyze - 2];
                    this.avgNAmpList[indexAnalyze - 1] = this.avgNAmpList[indexAnalyze - 2];
                }

                refsi.Reset();
            ModelTest mt = new ModelTest(this, refsi);
            mt.t = indexAnalyze;


                //回复rt、t日所在的kpiece
                //refsi.kpSet.kPieces[rtkpid] = back_rtkp;
                kpSet.kPieces[tkpid] = back_tkp;
                //refsi.avgNAmpList[refsi.t] = back_ranampt;
                this.avgNAmpList[indexAnalyze] = back_anampt;
                if (refsi.t > 0)
                {
                    //refsi.avgNAmpList[refsi.t - 1] = back_ranamptp;
                }
                if (indexAnalyze > 0)
                {
                    this.avgNAmpList[indexAnalyze - 1] = back_anamptp;
                }

                //return null;

            if(!mt.Available)
            {
                return null;
            }

            string strtotalScore = mt.totalScore.ToString();
            string strampScore = mt.ampScore.ToString();
            string strneibHScore = mt.neibHScore.ToString();
            string strkpampsco = mt.kpAmpScore.ToString();
            string strextresco = mt.extreScore.ToString();
            if (strtotalScore.Length>5)
            {
                strtotalScore = strtotalScore.Substring(0, 5);

            }
            if (strampScore.Length > 5)
            {
                strampScore = strampScore.Substring(0, 5);

            }
            if (strneibHScore.Length > 5)
            {
                strneibHScore = strneibHScore.Substring(0, 5);

            }
            if (strkpampsco.Length > 5)
            {
                strkpampsco = strkpampsco.Substring(0, 5);

            }
            if (strextresco.Length > 5)
            {
                strextresco = strextresco.Substring(0, 5);

            }
            ResultList.Add(strtotalScore);
            ResultList.Add(strampScore);
            ResultList.Add(strneibHScore);
            ResultList.Add(strkpampsco);
            ResultList.Add(strextresco);
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

            ResultList.Add(Percent(rise1) + "%");
            ResultList.Add(Percent(rise2) + "%");
            //ResultList.Add("test");
            //ResultList.Add(mwlsteprise.func);

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
            ColumnName.Insert(InfoOffset, "score");
            ColumnText.Insert(InfoOffset, "score");
            ColumnSize.Insert(InfoOffset, 50);
            ColumnSort.Insert(InfoOffset, "num");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "ampsc");
            ColumnText.Insert(InfoOffset, "ampsc");
            ColumnSize.Insert(InfoOffset, 50);
            ColumnSort.Insert(InfoOffset, "num");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "neibhsc");
            ColumnText.Insert(InfoOffset, "neibhsc");
            ColumnSize.Insert(InfoOffset, 50);
            ColumnSort.Insert(InfoOffset, "num");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "kpampsc");
            ColumnText.Insert(InfoOffset, "kpampsc");
            ColumnSize.Insert(InfoOffset, 50);
            ColumnSort.Insert(InfoOffset, "num");
            InfoOffset++;
            ColumnName.Insert(InfoOffset, "extresc");
            ColumnText.Insert(InfoOffset, "extresc");
            ColumnSize.Insert(InfoOffset, 50);
            ColumnSort.Insert(InfoOffset, "num");
            InfoOffset++;
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

            return InfoOffset;
        }
    }
}
