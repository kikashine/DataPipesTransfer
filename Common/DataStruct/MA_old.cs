using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace StockToolKit.Common
{
    /// <summary>
    /// 指定时间域内每日的4、8、12日简单移动平均值
    /// </summary>
    [Serializable]
    public class MA_old
    {
        private int indexBegin;
        private int indexEnd;
        private KDayDataList kdlist;
        private string StockCode = "";
        ///// <summary>
        ///// MA4数据集合
        ///// </summary>
        //private THashTable<float> _ma4;
        ////
        ///// <summary>
        ///// MA8数据集合
        ///// </summary>
        //private THashTable<float> _ma8;
        ////
        ///// <summary>
        ///// MA12数据集合
        ///// </summary>
        //private THashTable<float> _ma12;

        /// <summary>
        /// MA4数据集合
        /// </summary>
        private float[] _ma4;
        //
        /// <summary>
        /// MA8数据集合
        /// </summary>
        private float[] _ma8;
        //
        /// <summary>
        /// MA12数据集合
        /// </summary>
        private float[] _ma12;

        private float[] _ma24;

        private List<int> _ma24flexpoints;

        /// <summary>
        /// 在进行拐点分析计算时记录当前拐点是否顶点
        /// </summary>
        private bool flexptop;

        /// <summary>
        /// 在进行拐点分析计算时记录当前拐点是否底点
        /// </summary>
        private bool flexpbtm;

        /// <summary>
        /// 在进行拐点分析计算时记录临时顶点k线索引值
        /// </summary>
        private int flexptopi;

        /// <summary>
        /// 在进行拐点分析计算时记录临时底点k线索引值
        /// </summary>
        private int flexpbtmi;

        private float[] _riseavr;

        private float[] _rise10day;

        //计算已经记录了几个上升日，用来计算平均值。10个为上限。
        private int countrise;
        //因为仅记录最近的10个上升日，所以_rise10day数组作为循环队列使用。
        //_rise10dayi为当前需要在_rise10day数组中插入数据的位置
        private int _rise10dayi;
        ///// <summary>
        ///// 指定k线索引对应的MA4值
        ///// </summary>
        ///// <param name="i"></param>
        ///// <returns></returns>
        //public float MA4(int i)
        //{
        //    return _ma4[i];
        //}
        ///// <summary>
        ///// 指定k线索引对应的MA8值
        ///// </summary>
        ///// <param name="i"></param>
        ///// <returns></returns>
        //public float MA8(int i)
        //{
        //    return _ma8[i];
        //}
        ///// <summary>
        ///// 指定k线索引对应的MA12值
        ///// </summary>
        ///// <param name="i"></param>
        ///// <returns></returns>
        //public float MA12(int i)
        //{
        //    return _ma12[i];
        //}

        //public float MA24(int i)
        //{
        //    return _ma24[i];
        //}
        //public THashTable<float> ma4
        //{
        //    get
        //    {
        //        return _ma4;
        //    }
        //}

        //public THashTable<float> ma8
        //{
        //    get
        //    {
        //        return _ma8;
        //    }
        //}

        //public THashTable<float> ma12
        //{
        //    get
        //    {
        //        return _ma12;
        //    }
        //}

        public float[] ma4
        {
            get
            {
                return _ma4;
            }
        }

        public float[] ma8
        {
            get
            {
                return _ma8;
            }
        }

        public float[] ma12
        {
            get
            {
                return _ma12;
            }
        }

        public float[] ma24
        {
            get
            {
                return _ma24;
            }
        }

        /// <summary>
        /// ma24的拐点集合
        /// </summary>
        public List<int> ma24FlexPoints
        {
            get
            {
                return _ma24flexpoints;
            }
        }

        /// <summary>
        /// ma24的拐点集合，ma24FlexPoints的别称
        /// </summary>
        public List<int> ma24FPs
        {
            get
            {
                return _ma24flexpoints;
            }
        }
        public float[] RiseAvr
        {
            get
            {
                return _riseavr;
            }
        }
        public MA_old()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_dt"></param>
        /// <param name="indexBegin"></param>
        /// <param name="indexEnd"></param>
        public MA_old(KDayDataList kdlist, int indexBegin, int indexEnd)
        {
            this.kdlist = kdlist;
            StockCode = kdlist.StockCode;
            KBase kbase = new KBase(kdlist);
            this.indexBegin = indexBegin;
            this.indexEnd = indexEnd;
            doCompute(kbase);
        }
        private void doCompute(KBase kbase)
        {
            countrise = 0;
            _rise10dayi = 0;
            _rise10day = new float[10];
            _riseavr = new float[indexEnd - indexBegin + 1];
            //_ma4 = new THashTable<float>(indexEnd - indexBegin + 1);
            //_ma8 = new THashTable<float>(indexEnd - indexBegin + 1);
            //_ma12 = new THashTable<float>(indexEnd - indexBegin + 1);
            _ma4 = new float[indexEnd - indexBegin + 1];
            _ma8 = new float[indexEnd - indexBegin + 1];
            _ma12 = new float[indexEnd - indexBegin + 1];
            _ma24 = new float[indexEnd - indexBegin + 1];
            _ma24flexpoints = new List<int>();
            for (int i = indexBegin; i <= indexEnd; i++)
            {
                //computeRiseAvr(indexBegin, i);
                _ma4[i] = this.computeMA(kbase, 4, i);
                //break;
                _ma8[i] = this.computeMA(kbase, 8, i);
                _ma12[i] = this.computeMA(kbase, 12, i);
                _ma24[i] = this.computeMA(kbase, 24, i);

                if (computeFlexPoints(i, indexBegin, indexEnd))
                {
                    _ma24flexpoints.Add(i - 1);
                }
                //i为最后一日，且与前拐点有指定距离时，记录i为拐点
                if (i == indexEnd && _ma24flexpoints.Count > 0 && i > _ma24flexpoints[_ma24flexpoints.Count - 1] + 3)
                {
                    //结束日前的转折点是底点，且结束日ma24低于该底点ma24，将该底点移除，更换为结束日
                    if (ma24[i] < ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]] * 0.975 && flexpbtm)
                    {
                        _ma24flexpoints.RemoveAt(_ma24flexpoints.Count - 1);
                        _ma24flexpoints.Add(i);
                    }
                    //结束日前的转折点是顶点，且结束日ma24高于该顶点ma24，将该顶点移除，更换为结束日
                    else if (ma24[i] > ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]] * 1.025 && flexptop)
                    {
                        _ma24flexpoints.RemoveAt(_ma24flexpoints.Count - 1);
                        _ma24flexpoints.Add(i);
                    }
                    else
                    {
                        _ma24flexpoints.Add(i);
                    }

                    //需添加对转折点集合的扫描，移除可以忽略的转折点
                    //至少需参考j、j+1、j+2三个点，所以截止到_ma24flexpoints.Count-3
                    for (int j = 1; j <= _ma24flexpoints.Count - 3; j++)
                    {
                        //j、j+1接近，j+2较远，j是顶点且j+2高于等于j时，移除j、j+1
                        if (_ma24flexpoints[j + 1] - _ma24flexpoints[j] <= 5 && _ma24flexpoints[j + 2] - _ma24flexpoints[j + 1] > 5
                            &&
                            ma24[_ma24flexpoints[j]] > ma24[_ma24flexpoints[j + 1]]
                            &&
                            ma24[_ma24flexpoints[j]] <= ma24[_ma24flexpoints[j + 2]]
                            )
                        {
                            _ma24flexpoints.RemoveAt(j);
                            _ma24flexpoints.RemoveAt(j);
                            continue;
                        }
                        //j、j+1接近，j+2较远，j是顶点且j+2低于j时，移除j+1、j+2
                        if (_ma24flexpoints[j + 1] - _ma24flexpoints[j] <= 5 && _ma24flexpoints[j + 2] - _ma24flexpoints[j + 1] > 5
                            &&
                            ma24[_ma24flexpoints[j]] > ma24[_ma24flexpoints[j + 1]]
                            &&
                            ma24[_ma24flexpoints[j]] > ma24[_ma24flexpoints[j + 2]]
                            )
                        {
                            _ma24flexpoints.RemoveAt(j + 1);
                            _ma24flexpoints.RemoveAt(j + 1);
                            continue;
                        }
                        //j、j+1接近，j+2较远，j是底点且j+2低于j时，移除j、j+1
                        if (_ma24flexpoints[j + 1] - _ma24flexpoints[j] <= 5 && _ma24flexpoints[j + 2] - _ma24flexpoints[j + 1] > 5
                            &&
                            ma24[_ma24flexpoints[j]] < ma24[_ma24flexpoints[j + 1]]
                            &&
                            ma24[_ma24flexpoints[j]] >= ma24[_ma24flexpoints[j + 2]]
                            )
                        {
                            _ma24flexpoints.RemoveAt(j);
                            _ma24flexpoints.RemoveAt(j);
                            continue;
                        }
                        //j、j+1接近，j+2较远，j是底点且j+2高于等于j时，移除j+1、j+2
                        if (_ma24flexpoints[j + 1] - _ma24flexpoints[j] <= 5 && _ma24flexpoints[j + 2] - _ma24flexpoints[j + 1] > 5
                            &&
                            ma24[_ma24flexpoints[j]] < ma24[_ma24flexpoints[j + 1]]
                            &&
                            ma24[_ma24flexpoints[j]] < ma24[_ma24flexpoints[j + 2]]
                            )
                        {
                            _ma24flexpoints.RemoveAt(j + 1);
                            _ma24flexpoints.RemoveAt(j + 1);
                            continue;
                        }
                    }
                }
            }
        }

        public void reCompute()
        {
            KBase kbase = new KBase(kdlist);
            doCompute(kbase);
        }
        /// <summary>
        /// 计算当日之前10个上升日的平均上升幅度
        /// </summary>
        /// <param name="indexBegin"></param>
        /// <param name="i"></param>
        private void computeRiseAvr(KBase kbase, int indexBegin, int i)
        {
            //将_rise10day数据中的数据累加起来的临时变量
            float risesum = 0;
            //指针指到数组之外时更改为指向第一个
            if (_rise10dayi == _rise10day.Length)
            {
                _rise10dayi = 0;
            }
            //第一天平均升幅为0
            if (i == indexBegin)
            {
                _rise10day[0] = 0;
                _rise10dayi++;
                countrise++;
            }
            //相比前一天上升时才记录
            else if (kbase.Low(i) > kbase.Low(i - 1))
            {
                _rise10day[_rise10dayi] = kbase.Low(i) - kbase.Low(i - 1);
                _rise10dayi++;
                //个数直到记录到数组的长度为止
                if (countrise < _rise10day.Length)
                {
                    countrise++;
                }
            }

            for (int j = 0; j < countrise; j++)
            {
                risesum = risesum + _rise10day[j];
                //return;
            }
            _riseavr[i] = risesum / countrise;
        }
        /// <summary>
        /// 计算简单移动平均值
        /// </summary>
        /// <param name="days">需要计算的天数</param>
        /// <param name="i">需要计算的指定日数据在k线数据集中的索引值</param>
        /// <returns>简单移动平均值</returns>
        private float computeMA(KBase kbase, int days, int i)
        {
            float ma = 0;
            int currentdays = 0;
            if (i <= days - 1)
            {
                currentdays = i + 1;
            }
            else
            {
                currentdays = days;
            }
            for (int j = i - currentdays + 1; j <= i; j++)
            {
                //break;
                ma = ma + kbase.Close(j);
            }

            //MA值四舍五入保留小数点后两位
            return Convert.ToSingle((ma / (float)currentdays).ToString("0.00"));
        }

        /// <summary>
        /// 分析计算ma24是否出现拐点
        /// 因为分析日当前出现了转折，所以拐点是在紧邻分析日之前出现的。
        /// 这里的分析结果设计为t-1是拐点则返回true
        /// </summary>
        /// <param name="t"></param>
        /// <param name="indexBegin"></param>
        /// <param name="indexEnd"></param>
        /// <returns></returns>
        private bool computeFlexPoints(int t, int indexBegin, int indexEnd)
        {
            bool ok = false;

            //记录第一个拐点。在indexBegin与t之间出现的第一个拐点
            if (_ma24flexpoints.Count == 0 && t > indexBegin + 1)
            {
                //t-1是顶点
                if (ma24[0] < ma24[t - 1] && ma24[t] < ma24[t - 1])
                {
                    flexptop = true;
                    flexpbtm = false;
                    flexptopi = t - 1;
                    return true;
                }
                //t-1是底点
                if (ma24[0] > ma24[t - 1] && ma24[t] > ma24[t - 1])
                {
                    flexptop = false;
                    flexpbtm = true;
                    flexpbtmi = t - 1;
                    return true;
                }

            }
            //已经存在拐点
            else if (_ma24flexpoints.Count > 0)
            {
                if (t - 2 < indexBegin)
                {
                    return false;
                }
                //前一个拐点是顶点，本拐点是底点
                if (flexptop && ((_ma24[t - 2] == _ma24[t - 1] && _ma24[t - 1] < _ma24[t]) || (_ma24[t - 2] > _ma24[t - 1] && _ma24[t - 1] < _ma24[t])))
                {
                    //与前拐点差距很小(小于指定幅度)，仅暂时记录底点。（认为基本持平）
                    //if (Math.Abs((_ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]] - _ma24[t - 1]) / _ma24[t - 1]) < 0.025)
                    //{
                    //    flexpbtmi = t - 1;
                    //}
                    ////判定t-1为底点
                    //else
                    //{
                    //_ma24flexpoints.Add(t - 1);
                    flexptop = false;
                    flexpbtm = true;
                    ok = true;
                    return ok;
                    //}

                }
                //前一个拐点是顶点，本拐点是顶点，说明之间有一个被忽略的底点（该忽略的底点与前顶点基本持平）
                //若本顶点高于前顶点，则前顶点可视为底点到本顶点上升过程中可以忽略的小波折
                //若本顶点低于前顶点，则本顶点可视为前顶点之后下降过程中可以忽略的小波折
                if (flexptop && ((_ma24[t - 2] == _ma24[t - 1] && _ma24[t - 1] > _ma24[t]) || (_ma24[t - 2] < _ma24[t - 1] && _ma24[t - 1] > _ma24[t])))
                {
                    //本顶点高于前顶点，移除前顶点，判定t-1为顶点
                    if (ma24[t - 1] > ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]])
                    {
                        _ma24flexpoints.RemoveAt(_ma24flexpoints.Count - 1);
                        flexptop = true;
                        flexpbtm = false;
                        ok = true;
                        return ok;

                    }
                }
                //前一个拐点是底点，本拐点是顶点
                if (flexpbtm && ((_ma24[t - 2] == _ma24[t - 1] && _ma24[t - 1] > _ma24[t]) || (_ma24[t - 2] < _ma24[t - 1] && _ma24[t - 1] > _ma24[t])))
                {
                    ////与前拐点差距很小(小于指定幅度)，仅暂时记录顶点。（认为基本持平）
                    //if (Math.Abs((_ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]] - _ma24[t - 1]) / _ma24[t - 1]) < 0.025)
                    //{
                    //    flexptopi = t - 1;
                    //}
                    ////判定t-1为顶点
                    //else
                    //{
                    //_ma24flexpoints.Add(t - 1);
                    flexptop = true;
                    flexpbtm = false;
                    ok = true;
                    return ok;
                    //float a = Open(t);

                    //}

                }
                //前一个拐点是底点，本拐点是底点，说明之间有一个被忽略的顶点（该忽略的底点与前顶点基本持平）
                //若本底点低于前底点，则前底点可视为顶点到本底点下降过程中可以忽略的小波折
                //若本底点高于前底点，则本底点可视为前底点之后上升过程中可以忽略的小波折
                if (flexpbtm && ((_ma24[t - 2] == _ma24[t - 1] && _ma24[t - 1] < _ma24[t]) || (_ma24[t - 2] > _ma24[t - 1] && _ma24[t - 1] < _ma24[t])))
                {
                    if (ma24[t - 1] < ma24[_ma24flexpoints[_ma24flexpoints.Count - 1]])
                    {
                        _ma24flexpoints.RemoveAt(_ma24flexpoints.Count - 1);
                        flexptop = false;
                        flexpbtm = true;
                        ok = true;
                        return ok;
                    }
                }

            }
            return ok;

        }

        /// <summary>
        /// 得到紧邻t之前的ma24拐点在ma24拐点集合中的索引值
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int getma24FPBefort(int t)
        {
            int idx = 0;
            for (int i = ma24FPs.Count - 1; i >= 0; i--)
            {
                if (ma24FPs[i] < t)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

    }
}
