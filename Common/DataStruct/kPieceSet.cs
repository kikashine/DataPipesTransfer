using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockToolKit.Common
{
    /// <summary>
    /// k线片段
    /// </summary>
    [Serializable]
    public class kPieceSet : ICloneable
    {
        private int _indexBegin;

        private int _indexEnd;

        public string StockCode = "";

        /// <summary>
        /// k线片段集合
        /// </summary>
        private kPiece[] _kPieces;

        private KDayDataList kdlist;

        //private KBase kbase;

        public kPiece[] kPieces
        {
            get
            {
                return _kPieces;
            }
        }

        public int indexBegin
        {
            get
            {
                return _indexBegin;
            }
        }

        public int indexEnd
        {
            get
            {
                return _indexEnd;
            }
        }

        public kPieceSet()
        {

        }
        //public kPieceSet(DataTableQ dtq)
        //{
        //    base.theDT = dtq;
        //}

        public kPieceSet(KDayDataList DataList, int indexBegin, int indexEnd)
        {
            this.kdlist = DataList;
            StockCode = DataList.StockCode;
            this._indexBegin = indexBegin;
            this._indexEnd = indexEnd;
            doCompute();
        }
        public kPieceSet(KDayDataList DataList, kPiece[] kPieces)
        {
            _kPieces = kPieces;
            this.StockCode = DataList.StockCode;
            _indexBegin = kPieces[0].Begin;
            _indexEnd = kPieces[kPieces.Length - 1].End;
            this.kdlist = DataList;
        }
        /// <summary>
        /// 深层拷贝用构造函数
        /// </summary>
        /// <param name="s_kPieces"></param>
        /// <param name="dt"></param>
        /// <param name="indexBegin"></param>
        /// <param name="indexEnd"></param>
        private kPieceSet(kPieceSet kpset)
        {
            _kPieces = new kPiece[kpset.kPieces.Length];
            _indexBegin = kpset.indexBegin;
            _indexEnd = kpset.indexEnd;
            this.kdlist = kpset.kdlist;
            StockCode = kpset.StockCode;
            for (int i = 0; i <= kpset.kPieces.Length - 1; i++)
            {
                _kPieces[i] = (kPiece)(kpset.kPieces[i].Clone());
            }
        }

        private void doCompute()
        {
            KBase kbase = new KBase(kdlist);
            //临时数组长度。为减少数组拷贝次数使用临时数组，写满临时数组后拷贝到数组中
            int capacity = 32;
            //分析得到的kPiece列表
            _kPieces = new kPiece[0];
            //临时kp列表
            kPiece[] tmplist = new kPiece[capacity];
            kPiece ventmp;
            //临时记录得到kpiece长度
            int counttmp = 0;
            //分析的起始k线索引
            int b = indexBegin;
            //分析的结束k线索引
            int e = indexEnd;
            for (; b <= e; b++)
            {
                ventmp = genkPiece(kbase, b);

                tmplist[counttmp] = ventmp;
                counttmp++;
                //设置此次得到的End为下个kpiece的begin，因为for会令b++，所以此处应为End-1
                b = ventmp.End - 1;
                //最后一个kpiece且tmplist未填满，应截掉未填满部分
                if (ventmp.End == e && counttmp < capacity)
                {
                    kPiece[] newlist = new kPiece[counttmp];
                    for (int i = 0; i < counttmp; i++)
                    {
                        newlist[i] = tmplist[i];
                    }
                    tmplist = newlist;
                    //令counttmp = capacity，满足后面合并list和tmplist的判断条件
                    counttmp = capacity;
                }
                //tmplist写满，与list合并
                if (counttmp == capacity)
                {
                    kPiece[] newlist = new kPiece[_kPieces.Length + tmplist.Length];
                    _kPieces.CopyTo(newlist, 0);
                    tmplist.CopyTo(newlist, _kPieces.Length);
                    _kPieces = newlist;
                    tmplist = new kPiece[capacity];
                    counttmp = 0;
                }
                if (ventmp.End == e)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 根据指定的起始位置得到一个k线片段
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private kPiece genkPiece(KBase kbase, int b)
        {
            kPiece kp = new kPiece(kdlist);
            kp.Begin = b;
            kp.End = b;
            //临时记录升降类型
            kPieceTrend trend = kPieceTrend.Null;

            for (int i = b; i <= indexEnd; i++)
            {
                //i为第二个元素，与起始相比较
                if (i == b + 1)
                {
                    //上升
                    if (kbase.Low(i) > kbase.Low(b))
                    {
                        trend = kPieceTrend.Rise;
                    }
                    //下降
                    else if (kbase.Low(i) < kbase.Low(b))
                    {
                        trend = kPieceTrend.Fall;
                    }
                    //持平
                    else
                    {
                        trend = kPieceTrend.Null;
                    }
                }
                else if (i > b + 1)
                {
                    //转变为上升
                    if (trend == kPieceTrend.Fall && kbase.Low(i) > kbase.Low(i - 1))
                    {
                        kp.End = i - 1;
                        break;
                    }
                    //继续下降（已存在趋势时，持平算作持续已存在的趋势）
                    else if (trend == kPieceTrend.Fall && kbase.Low(i) <= kbase.Low(i - 1))
                    {

                    }
                    //转变为下降
                    else if (trend == kPieceTrend.Rise && kbase.Low(i) < kbase.Low(i - 1))
                    {
                        kp.End = i - 1;
                        break;
                    }
                    //继续上升（已存在趋势时，持平算作持续已存在的趋势）
                    else if (trend == kPieceTrend.Rise && kbase.Low(i) >= kbase.Low(i - 1))
                    {

                    }
                    //从起始持平，当前开始出现趋势
                    else if (trend == kPieceTrend.Null)
                    {
                        if (kbase.Low(i) > kbase.Low(i - 1))
                        {
                            trend = kPieceTrend.Rise;
                        }
                        else if (kbase.Low(i) < kbase.Low(i - 1))
                        {
                            trend = kPieceTrend.Fall;
                        }
                    }
                }
                //遇结束
                if (i == indexEnd)
                {
                    //从k线数据集合begin开始到end始终持平，为趋势不为Null，设置为Rise
                    if (kp.Trend == kPieceTrend.Null)
                    {
                        trend = kPieceTrend.Rise;
                    }
                    kp.End = i;
                }

            }

            return kp;

        }

        public kPiece genkPiece(int b, int e)
        {
            if (kdlist == null)
            {
                return new kPiece();
            }
            kPiece kp = genkPiece(new KBase(kdlist), b);
            if (e < kp.End)
            {
                kp.End = e;
            }
            return kp;
        }
        /// <summary>
        /// 得到在作为参数的k线索引所在的k线片段
        /// 逻辑：begin<kid<=end
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        public kPiece findKPiece(int kid)
        {
            return _kPieces[findKPieceIndex(kid)];
        }
        /// <summary>
        /// 得到在作为参数的k线索引所在的k线片段索引
        /// 逻辑：begin<kid<=end
        /// </summary>
        /// <param name="kid"></param>
        /// <returns></returns>
        public int findKPieceIndex(int kid)
        {
            if (kid == 0 && _kPieces.Length > 0 && _kPieces[0].Begin == 0)
            {
                return 0;
            }
            int low = 0;
            int high = _kPieces.Length - 1;

            while ((low <= high) && (low <= _kPieces.Length - 1)
                    && (high <= _kPieces.Length - 1))
            {
                int middle = low + ((high - low) >> 1);
                if (middle == 0 && kid > _kPieces[middle].Begin && kid <= _kPieces[middle].End)
                {
                    return middle;
                }
                if (middle == _kPieces.Length - 1 && kid > _kPieces[middle].Begin && kid <= _kPieces[middle].End)
                {
                    return middle;
                }
                if (kid > _kPieces[middle].Begin && kid <= _kPieces[middle].End)
                {
                    return middle;
                }
                else if (kid <= _kPieces[middle].Begin)
                {
                    high = middle - 1;
                }
                else
                {
                    low = middle + 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 返回kPieceSet的深层拷贝
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {

            return new kPieceSet(this);
        }
    }
}
