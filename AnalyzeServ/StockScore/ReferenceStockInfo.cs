using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 进行股票评价时，参照股票的信息
    /// </summary>
    public class ReferenceStockInfo:KBase
    {

        //private KBasePub _kbpub;

        private kPiece[] _kps;
        /// <summary>
        /// 分析日的k线数据集合索引
        /// </summary>
        private int _t;
        /// <summary>
        /// 分析日所在的k线片段索引
        /// </summary>
        private int _tkpid;
        /// <summary>
        /// 参与评价的k线片段索引集合
        /// </summary>
        private int[] _kpidforscore;
        /// <summary>
        /// 参与评价的关键k线片段索引集合
        /// </summary>
        private int[] _keykpidforscore;
        /// <summary>
        /// 标记k线片段是否关键k线片段，与_kpidforscore对应
        /// </summary>
        private bool[] _iskeykpid;
        /// <summary>
        /// 参与评价的k线柱框架列表
        /// </summary>
        private Dictionary<int, KDayFrame> _kdfset;

        private Dictionary<int, kPieceFrame> _kpfset;
        /// <summary>
        /// 分析日的k线数据集合索引
        /// </summary>
        public int t
        {
            get
            {
                return _t;
            }
        }
        /// <summary>
        /// 分析日所在的k线片段索引
        /// </summary>
        public int tKPId
        {
            get
            {
                return _tkpid;
            }
        }
        /// <summary>
        /// 参与评价的k线片段索引集合
        /// </summary>
        public int[] KPIdForScore
        {
            get
            {
                return _kpidforscore;
            }
        }
        /// <summary>
        /// 标记k线片段是否关键k线片段，与KPIdForScore对应
        /// </summary>
        public bool[] isKeyKPId
        {
            get
            {
                return _iskeykpid;
            }
        }
        /// <summary>
        /// 参与评价的关键k线片段索引集合
        /// </summary>
        public int[] keyKPIdForScore
        {
            get
            {
                return _keykpidforscore;
            }
        }

        /// <summary>
        /// 参与评价的k线柱框架列表
        /// </summary>
        public Dictionary<int,KDayFrame> kDayFrameSet
        {
            get
            {
                return _kdfset;
            }
        }

        public Dictionary<int, kPieceFrame> kpFrameSet
        {
            get
            {
                return _kpfset;
            }
        }
        //public KBasePub kbpub
        //{
        //    get
        //    {
        //        return _kbpub;
        //    }
        //}

        public ReferenceStockInfo()
        {

        }

        public ReferenceStockInfo(int t,KBase kbase)
            :base(kbase)
        {
            _t = t;
            //base.theDT = theDT;
            //_kbpub = new KBasePub(ref theDT);
            _kps = kpSet.kPieces;
            _tkpid = kpSet.findKPieceIndex(_t);
            //_kps[_tkpid] = theDT.kpset.genkPiece(_kps[_tkpid].Begin, _t);
            _kdfset = new Dictionary<int,KDayFrame>();
            _kpfset = new Dictionary<int, kPieceFrame>();
            findKPIDForScore();
            genFrameSet();

        }
        /// <summary>
        /// 查找需要参与评价的k线片段
        /// </summary>
        private void findKPIDForScore()
        {
            //指定范围内的k线片段索引值，用于排序
            int[] array = new int[20];
            int i = kpSet.findKPieceIndex(_t) - 1;
            int count = 0;
            //记录一定范围内的k线片段结束点，在其中寻找关键点
            //由于kpieceSet.findKPieceIndex方法的逻辑是begin<kid<=end，所以使用end做分析
            while (i >= 1 && _kps[i].End >= _t - 20)
            {
                //距离t一定范围内的kpiece结束点不记录
                if (_kps[i].End < _t - 7)
                {
                    array[count] = i;
                    count++;
                }
                i--;
            }

            int[] newarray = new int[count];
            //收缩array，去掉空元素
            for (int j = 0; j < count; j++)
            {
                newarray[j] = array[j];
            }
            array = newarray;
            //按照k线柱下沿值从低到高对索引排序
            sortByValue(array, 0, count - 1, _kps);

            int[] pass1 = doPass1ForKey(array);


            //将结果按kp数据集合的id顺序排序
            sortById(pass1, 0, pass1.Length - 1);

            //_keykpidforscore = pass1;
            //return;

            int[] pass2 = doPass2ForKey(pass1);

            if (pass2.Length == 0)
            {
                pass2 = new int[1];
                pass2[0] = 0;
            }

            //填充评分用kpid数组及关键点标记
            _kpidforscore = new int[_tkpid - pass2[0] + 1];

            _iskeykpid = new bool[_kpidforscore.Length];
            count = 0;
            for (int j = 0; j <= pass2.Length - 1; j++)
            {
                //处理pass2最后一个kp到t日所在kp
                if (j == pass2.Length - 1)
                {
                    for (int k = pass2[j]; k <= _tkpid; k++)
                    {

                        if (k != pass2[j] && _kps[k].End < _t - 7)
                        {
                            continue;
                        }
                        //pass2元素在长度为2的kp中时，保留pass2元素，抛弃紧邻kp
                        //if (_kps[k].Begin == _kps[pass2[j]].Begin + 1)
                        //{
                        //    continue;
                        //}
                        ////当前kp+1长度为2且下降，且处于上升过程中，抛弃当前和当前+1的kp
                        //if (k + 2<=_tkpid
                        //    &&
                        //    _kps[k + 1].Length == 2 && _kps[k + 1].Trend == kPieceTrend.Fall
                        //    &&
                        //    Low(_kps[k].Begin) < Low(_kps[k + 1].End)
                        //    &&
                        //    Low(_kps[k + 2].End) > Low(_kps[k + 1].Begin)
                        //    )
                        //{
                        //    k = k + 1;
                        //    continue;
                        //}
                        ////当前kp+1长度为2且上升，且处于下降过程中，抛弃当前和当前+1的kp
                        //if (k + 2 <= _tkpid
                        //    &&
                        //    _kps[k + 1].Length == 2 && _kps[k + 1].Trend == kPieceTrend.Rise
                        //    &&
                        //    Low(_kps[k].Begin) > Low(_kps[k + 1].End)
                        //    &&
                        //    Low(_kps[k + 2].End) < Low(_kps[k + 1].Begin)
                        //    )
                        //{
                        //    k = k + 1;
                        //    continue;
                        //}
                        _kpidforscore[count] = k;
                        //pass2中元素标记为关键点
                        if (k == pass2[j])
                        {
                            _iskeykpid[count] = true;
                        }
                        else
                        {
                            _iskeykpid[count] = false;
                        }
                        count++;
                    }
                    break;
                }
                //处理pass2两个元素之间(不包括后元素)的kp
                for (int k = pass2[j]; k < pass2[j + 1]; k++)
                {
                   
                    //pass2元素在长度为2的kp中时，保留pass2元素，抛弃紧邻kp
                    if (_kps[k].End == _kps[pass2[j + 1]].Begin && _kps[pass2[j + 1]].Length == 2)
                    {
                        continue;
                    }
                    //当前kp+1长度为2且下降，且处于上升过程中，抛弃当前和当前+1的kp
                    if (_kps[k + 1].Length == 2 && _kps[k + 1].Trend == kPieceTrend.Fall
                        &&
                        Low(_kps[k].Begin) < Low(_kps[k + 1].End)
                        &&
                        Low(_kps[k + 2].End) > Low(_kps[k + 1].Begin)
                        )
                    {
                        k = k + 1;
                        continue;
                    }
                    //当前kp+1长度为2且上升，且处于下降过程中，抛弃当前和当前+1的kp
                    if (_kps[k + 1].Length == 2 && _kps[k + 1].Trend == kPieceTrend.Rise
                        &&
                        Low(_kps[k].Begin) > Low(_kps[k + 1].End)
                        &&
                        Low(_kps[k + 2].End) < Low(_kps[k + 1].Begin)
                        )
                    {
                        k = k + 1;
                        continue;
                    }

                    _kpidforscore[count] = k;
                    //pass2中元素标记为关键点
                    if (k == pass2[j])
                    {
                        _iskeykpid[count] = true;
                    }
                    else
                    {
                        _iskeykpid[count] = false;
                    }
                    count++;
                }
            }
            bool[] tmpiskey = new bool[count];
            newarray = new int[count];
            //收缩array，去掉空元素
            for (int j = 0; j < count; j++)
            {
                tmpiskey[j] = _iskeykpid[j];
                newarray[j] = _kpidforscore[j];
            }
            _iskeykpid = tmpiskey;
            _kpidforscore = newarray;
            
            _keykpidforscore = pass2;
        

        }

        /// <summary>
        ///一次处理
        ///处理后得到kp.end在左右指定范围内是最高、低值的k线片段索引集合
        ///处理后可能存在长度为2的kp
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private int[] doPass1ForKey(int[] array)
        {
           
            int count = array.Length;
            int[] pass1 = new int[count];
            //标记是否为关键kp
            bool iskey = false;
            count = 0;
            int tmpkpid = 0;

            //在k线片段结束日的高值、低值中寻找关键kp
            for (int j = 0; j <= array.Length - 1; j++)
            {
                //临时存放本次循环需要分析的k线片段索引
                tmpkpid = array[j];
                if (_kps[tmpkpid].End==544)
                {
                    int debug1234124 = 0;
                }
                //判断在本次指定的k线片段左右两侧一定范围内是否存在低于低值、高于高值的情况
                //因为k线片段是上升下降间隔的，所以步进为2，才能比较同样类型k线片段的起始值
                for (int g = 2; ; g = g + 2)
                {
                    //处于k线片段数据集合的边缘时不作为关键kp
                    if (tmpkpid - g < 0
                        ||
                        tmpkpid + g > _kps.Length - 1
                        )
                    {
                        iskey = false;
                        break;
                    }
                    else if (//当前k线片段(kp)为下降，kp的end是低值，kp-2的end低于kp的end，且处于kp的end左侧指定范围内，则当前k线片段不能作为关键kp
                             (_kps[tmpkpid].Trend == kPieceTrend.Fall && tmpkpid - g >= 0 && Low(_kps[tmpkpid - g].End) < Low(_kps[tmpkpid].End) && _kps[tmpkpid].End - _kps[tmpkpid - g].End <= 3)
                            ||
                        //当前k线片段(kp)为下降，kp的end是低值，kp+2的end低于kp的end，且处于kp的end右侧指定范围内，则当前k线片段不能作为关键kp
                            (_kps[tmpkpid].Trend == kPieceTrend.Fall && tmpkpid + g <= _kps.Length - 1 && Low(_kps[tmpkpid + g].End) < Low(_kps[tmpkpid].End) && _kps[tmpkpid + g].End - _kps[tmpkpid].End <= 3)
                            )
                    {
                        iskey = false;
                        break;
                    }
                    else if (//当前k线片段(kp)为上升，kp的end是高值，kp-2的end高于kp的end，且处于kp的end左侧指定范围内，则当前k线片段不能作为关键kp
                             (_kps[tmpkpid].Trend == kPieceTrend.Rise && tmpkpid - g >= 0 && Low(_kps[tmpkpid - g].End) > Low(_kps[tmpkpid].End) && _kps[tmpkpid].End - _kps[tmpkpid - g].End <= 3)
                            ||
                        //当前k线片段(kp)为上升，kp的end是低值，kp-2的end高于kp的end，且处于kp的end左侧指定范围内，则当前k线片段不能作为关键kp
                            (_kps[tmpkpid].Trend == kPieceTrend.Rise && tmpkpid + g <= _kps.Length - 1 && Low(_kps[tmpkpid + g].End) > Low(_kps[tmpkpid].End) && _kps[tmpkpid + g].End - _kps[tmpkpid].End <= 3)
                            )
                    {
                        iskey = false;
                        break;
                    }
                    else if (//临近范围内没有低于低值或高于高值的情况，则当前k线片段为关键kp
                        _kps[tmpkpid].End - _kps[tmpkpid - g].End > 3
                        ||
                        _kps[tmpkpid + g].End - _kps[tmpkpid].End > 3
                        )
                    {
                        iskey = true;

                    }
                    if (iskey)
                    {
                        pass1[count] = tmpkpid;
                        count++;
                        break;
                    }
                }
            }
            int[] newarray = new int[count];
            //收缩pass1，去掉空元素
            for (int j = 0; j < count; j++)
            {
                newarray[j] = pass1[j];
            }
            pass1 = newarray;

            return pass1;
        }

        /// <summary>
        /// 二次处理
        /// 剔除两个在k线数据索引上相邻(其中一个长度为2)的kp其中之一
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private int[] doPass2ForKey(int[] array)
        {
            
            int[] pass2 = array;
            int[] newarray = new int[pass2.Length];
            int count = 0;
            //剔除两个在k线数据索引上相邻(其中一个长度为2)的数据其中之一
            for (int j = 0; j <= pass2.Length - 1; j++)
            {
                if (_kps[pass2[j]].End == 539)
                {
                    int debug1234124 = 0;
                }
                //两个关键k线片段转折kp在k线数据集合的索引上连续
                if (j + 1 <= pass2.Length - 1 && _kps[pass2[j + 1]].Begin == _kps[pass2[j]].End && _kps[pass2[j + 1]].Length == 2)
                {
                    //抛弃左侧第一个关键转折kp（减小长度即减小误差）
                    if (j == 0)
                    {
                        continue;
                    }
                    //相邻的两点是低点+高点（_kps[pass2[j]].end是低点，_kps[pass2[j + 1]].end是高点）
                    else if (_kps[pass2[j + 1]].Trend == kPieceTrend.Rise)
                    {
                        //若在高点右侧较近范围内有更高点，则说明高点是上升趋势内可以忽略的波动
                        for (int k = 1; k <= 4; k++)
                        {
                            //在高点右侧指定范围内k线高于高点，抛弃高点保留低点
                            if (Low(_kps[pass2[j + 1]].End + k) > Low(_kps[pass2[j + 1]].End))
                            {
                                newarray[count] = pass2[j];
                                count++;
                                j++;
                                break;
                            }
                            //在高点右侧指定范围内k线不高于高点，抛弃低点保留高点
                            if (k == 4)
                            {
                                newarray[count] = pass2[j + 1];
                                count++;
                                j++;
                                break;
                            }
                        }
                    }
                    //相邻的两点是高点+低点（_kps[pass2[j]].end是高点，_kps[pass2[j + 1]].end是低点）
                    else if (_kps[pass2[j + 1]].Trend == kPieceTrend.Fall)
                    {
                        
                        for (int k = 1; k <= 4; k++)
                        {
                            //在低点右侧指定范围内k线低于低点，抛弃低点(j+1)保留高点(j)
                            if (Low(_kps[pass2[j + 1]].End + k) < Low(_kps[pass2[j + 1]].End))
                            {
                                newarray[count] = pass2[j];
                                count++;
                                j++;
                                break;
                            }
                            //在低点右侧指定范围内k线不低于低点，抛弃高点保留低点
                            if (k == 4)
                            {
                                newarray[count] = pass2[j + 1];
                                count++;
                                j++;
                                break;
                            }
                        }
                    }
                }
                //在t指定范围外的最后一个kp，此kp上升，kp+1长度为2，kp+2.end高于kp.end，kp到kp+2一直上升，所以抛弃此kp
                else if (j == pass2.Length - 1)
                {
                    if (pass2[j] + 2 <= _kps.Length - 1
                        &&
                        _kps[pass2[j]].Trend == kPieceTrend.Rise && _kps[pass2[j] + 1].Length == 2
                        &&
                        Low(_kps[pass2[j] + 2].End) > Low(_kps[pass2[j]].End))
                    {
                        break;
                    }
                    newarray[count] = pass2[j];
                    count++;
                }
                else
                {
                    newarray[count] = pass2[j];
                    count++;
                }
            }
            pass2 = new int[count];
            for (int j = 0; j < count; j++)
            {
                pass2[j] = newarray[j];
            }
            return pass2;
        }

        /// <summary>
        /// 生成日k线框架列表
        /// </summary>
        private void genFrameSet()
        {
            for (int i = t; i >= t - 4; i--)
            {
                _kdfset.Add(i, new KDayFrame(i, _t, this));
            }
            int count = _tkpid - _keykpidforscore[_keykpidforscore.Length - 1] + _keykpidforscore.Length;

            for (int i = _tkpid; i > _keykpidforscore[_keykpidforscore.Length - 1];i-- )
            {
                if (i == _tkpid)
                {
                    _kpfset.Add(--count, new kPieceFrame(_kps[i].Begin, _kps[i].End, t, null, this));
                }
                else
                {
                    _kpfset.Add(--count, new kPieceFrame(_kps[i].Begin, _kps[i].End, t, _kpfset[count + 1], this));
                }
            }

            for (int i = _keykpidforscore.Length - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    _kpfset.Add(--count, new kPieceFrame(_kps[_keykpidforscore[i - 1]].End, _kps[_keykpidforscore[i]].End, t, _kpfset[count + 1], this));
                }
                else
                {
                    _kpfset.Add(--count, new kPieceFrame(_kps[_keykpidforscore[i]].Begin, _kps[_keykpidforscore[i]].End, t, _kpfset[count + 1], this));
                }
            }
        }

        public void Reset()
        {
            foreach (KDayFrame kdf in _kdfset.Values)
            {
                kdf.Reset();
            }
            foreach (kPieceFrame kpf in _kpfset.Values)
            {
                kpf.Reset();
            }
        }
        /// <summary>
        /// 快速排序的一次排序单元，完成此方法，key左边都比key小，key右边都比key大
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low">low排序起始位置</param>
        /// <param name="high">high排序结束位置</param>
        /// <returns></returns>
        private int sortByValueUnit(int[] array, int low, int high, kPiece[] kps)
        {
            int key = array[low];

            while (low < high)
            {
                //从后向前搜索比key小的值
                while (Low(kps[array[high]].End) >= Low(kps[key].End) && high > low)
                {
                    --high;
                }
                //比key小的放左边
                array[low] = array[high];

                //从前向后搜索比key大的值，比key大的放右边
                while (Low(kps[array[low]].End) <= Low(kps[key].End) && high > low)
                {
                    ++low;
                }
                //比key大的放右边
                array[high] = array[low];

            }
            //左边都比key小，右边都比key大。
            //将key放在游标当前位置。
            //此时low等于high
            array[low] = key;

            return high;

        }

        /// <summary>
        /// 快速排序，用来为k线片段数据集合的索引进行排序，排序依据是k线片段起始点的k线柱下沿值
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void sortByValue(int[] array, int low, int high, kPiece[] kps)
        {
            if (low >= high)
                return;
            /*完成一次单元排序*/
            int index = sortByValueUnit(array, low, high, kps);
            /*对左边单元进行排序*/
            sortByValue(array, low, index - 1, kps);
            /*对右边单元进行排序*/
            sortByValue(array, index + 1, high, kps);
        }

        /// <summary>
        /// 快速排序，用来为k线片段数据集合的索引进行升序排序，排序依据是索引的大小
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void sortById(int[] array, int low, int high)
        {
            if (low >= high)
                return;
            /*完成一次单元排序*/
            int index = sortByIdUnit(array, low, high);
            /*对左边单元进行排序*/
            sortById(array, low, index - 1);
            /*对右边单元进行排序*/
            sortById(array, index + 1, high);
        }

        /// <summary>
        /// 快速排序的一次排序单元，完成此方法，key左边都比key小，key右边都比key大
        /// </summary>
        /// <param name="array"></param>
        /// <param name="low">low排序起始位置</param>
        /// <param name="high">high排序结束位置</param>
        /// <returns></returns>
        private int sortByIdUnit(int[] array, int low, int high)
        {

            int key = array[low];
            while (low < high)
            {
                /*从后向前搜索比key小的值*/
                while (array[high] >= key && high > low)
                    --high;
                /*比key小的放左边*/
                array[low] = array[high];
                /*从前向后搜索比key大的值，比key大的放右边*/
                while (array[low] <= key && high > low)
                    ++low;
                /*比key大的放右边*/
                array[high] = array[low];
            }
            /*左边都比key小，右边都比key大。//将key放在游标当前位置。//此时low等于high */
            array[low] = key;

            return high;
        }


    }
}
