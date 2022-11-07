using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    class DayScore_sch1 : KBase
    {
        //参照股票数据集合
        //DataTableQ rthedt;
        //参照股票的日k线对象
        private KBase rkb;
        //参照股票的基准日也是开始日
        int _rt;
        //参照股票的当前日
        int _rc;
        //需评分股票的基准日也是开始日
        private int _t;
        //需评分股票的当前日（待评分日）
        private int _c;
        private float _highScore;
        private float _closeScore;
        /// <summary>
        /// amplitude升降幅度评分
        /// </summary>
        private float _ampScore;
        /// <summary>
        /// 相邻两日k线柱幅度评价
        /// </summary>
        private float _neibAmpScore;
        /// <summary>
        /// neighbor相邻两日k线柱上沿评价分数(c与c+1)
        /// </summary>
        private float _neibHScore;
        /// <summary>
        /// neighbor相邻两日k线柱下沿评价分数(c与c+1)
        /// </summary>
        private float _neibLScore;
        private float _sumScore;
        private float _totalScore;
        /// <summary>
        /// 本日是插值得到分数
        /// </summary>
        private bool _interpolation;

        public float ampScore
        {
            get
            {
                return _ampScore;
            }
        }
        /// <summary>
        /// 相邻两日k线柱幅度评价
        /// </summary>
        public float neibAmpScore
        {
            get
            {
                return _neibAmpScore;
            }
        }
        /// <summary>
        /// neighbor相邻两日k线柱上沿评价分数(c与c+1)
        /// </summary>
        public float neibHScore
        {
            get
            {
                return _neibHScore;
            }
        }
        public float neibLScore
        {
            get
            {
                return _neibLScore;
            }
        }
        public float highScore
        {
            get
            {
                return _highScore;
            }
        }

        public float totalScore
        {
            get
            {
                return _totalScore;
            }
        }
        /// <summary>
        /// 本日是否为插值得到分数
        /// </summary>
        public bool Interpolation
        {
            get
            {
                return _interpolation;
            }
        }
        
        /// <summary>
        /// 根据给定待评分股票的当前日（待评分日）、基准日（起始日），参照股票的当前日、基准日，计算当前待评分日的分数
        /// </summary>
        /// <param name="c">当前日（待评分日）</param>
        /// <param name="t">基准日（起始日）</param>
        /// <param name="theDT">待评分股票的数据集合</param>
        /// <param name="rc">参照股票的当前日（参照日）</param>
        /// <param name="rt">参照股票的基准日（起始日）</param>
        /// <param name="rtheDT">参照股票的数据集合</param>
        public DayScore_sch1(int c, int t, KBase kb, int rc, int rt, KBase rkb)
            :base(kb)
        {
            //待评价股票的基准日
            _t = t;
            //待评价股票的当前日
            _c = c;
            //参照股票的当前日
            _rc = rc;
            //参照股票的基准日
            _rt = rt;
            //base.theDT = theDT;
            //this.rkbpub = rkbpub;
            this.rkb = rkb;

            float ampscore1;
            float ampscore2 = -1;
            float highscore1;
            float amp = KRise(_c, 0);
            float ampp = KRise(_c - 1, 0);
            float ampf = -1;
            if (_c + 1 <= this.Count - 1)
            {
                ampf = KRise(_c + 1, 0);
            }
            float ramp = rkb.KRise(_rc, 0);
            float rampf = -1f;
            if (_rc + 1 <= rkb.Count - 1)
            {
                rampf = rkb.KRise(_rc + 1, 0);
            }
            float rampp = rkb.KRise(_rc - 1, 0);
            float tmpprc = -1f;
            float ampv1 = -999f;
            float ampv2 = -999f;
            float ampstret = -999f;
            //float r1 = 0;
            //float r2 = 0;
            float r = 1f;
            //float rb = 0f;
            //float rb1 = 0f;
            //float rb2 = 0f;

            //c\rc与相邻日幅度对比评分，在c\rc的上、下沿与相邻日k线柱之间存在空挡时计算评分
            //初始值为-1，为是否需要评分的标志，为0不需要评分
            //说明：拉伸后的幅度评分虽然可能会有改善，但拉伸后与临日幅度的对比会发生变化，因此较理想的评分是在可能影响相似度
            //的情况下得到的，因此应对相似度加权。
            //例如：假设c幅度为1，c+1幅度为10，c上沿到c+1下沿空缺为9，rc幅度为10，rc+1幅度为1，rc上沿到rc+1下沿空缺为9，
            //虽然拉伸c和rc+1后分别能得到较理想的分数，但实际上c、c+1与rc、rc+1两者k线柱幅度、形态组合正相反，评分不应过高
            //所以拉伸评分应考虑原本相邻两日的幅度及待评价股票与参照股票两者之间幅度对比的差异。
            //基本原则是拉伸前rc大于rc-1\rc+1，则拉伸前c大于c-1\c+1时，评价分数应相对较高，若拉伸前c小于c-1\c+1时则分较低。
            float ampcpr = -1f;
            //临时变量
            float ampcpr1 = 0f;

            
            ampscore1 = computeAmpScore(ramp, amp);
            highscore1 = computePriceScore(DvalueRatio(High(c), Low(t)), DvalueRatio(rHigh(_rc), rLow(_rt)));

            //c上升，且c下沿高于c-1下沿，低于c-1上沿，且rc升幅大于c升幅
            //if (_rc == _rt && rKRise(_rc, 0) > 0 && amp > 0
            //    &&
            //    High(_c - 1) > Low(_c) && Low(_c - 1) < Low(_c)
            //    &&
            //    ramp > amp
            //    )
            //{
            //    ampscore2 = computeAmpScore(ramp, DvalueRatio(High(_c), Low(_c - 1) + (Low(_c) - Low(_c - 1)) / 2));
            //}

            //从t-1向左的c日，存在重新估算c或rc升幅再进行评分的情况
            if(_c<_t)
            {


                //计算c日拉伸后幅度相对于rc幅度的评分
                //c-1下沿高于c上沿或c+1下沿高于c上沿或c-1上沿低于c下沿或c+1上沿低于c下沿
                //c与rc同为上升或同为下降
                //c幅度绝对值小于rc幅度绝对值
                if (
                        (High(_c - 1) < Low(_c) || High(_c + 1) < Low(_c) || Low(_c - 1) > High(_c) || Low(_c + 1) > High(_c))
                        &&
                        ((amp >= 0 && ramp >= 0) || (amp <= 0 && ramp <= 0))
                        &&
                        Math.Abs(amp) < Math.Abs(ramp)
                        )
                {
                    //标记是否向上拉伸
                    bool stretTop = false;

                    //c-1下沿低于c上沿，可以计算c在c-1方向的拉伸后幅度
                    if (High(_c - 1) < Low(_c))
                    {
                        //按照rc与rc-1的幅度比例计算c下沿到c-1上沿幅度中c可占的幅度（c在c-1方向的拉伸后幅度）
                        //若不按比例拉伸，则可能出现c-1已经用了拉伸空间得到较好分数，c又重复利用拉伸空间再次获得较好分数
                        ampv1 = KRiseLH(_c - 1, _c) * Math.Abs(ramp) / (Math.Abs(ramp) + Math.Abs(rampp));
                        //c幅度小于c-1幅度，rc幅度大于rc-1幅度，需要计算c、c-1，rc、rc-1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampp) < 1 && Math.Abs(ramp) / Math.Abs(rampp) > 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = false;
                    }
                    //c+1下沿低于c上沿，可以计算c在c+1方向的拉伸后幅度
                    if (_c + 1 <= this.Count - 1 && _rc + 1 <= rkb.Count - 1 && High(_c + 1) < Low(_c))
                    {
                        //按照rc与rc+1的幅度比例计算c下沿到c+1上沿幅度中c可占的幅度（c在c+1方向的拉伸后幅度）
                        ampv2 = KRiseLH(_c + 1, _c) * Math.Abs(ramp) / (Math.Abs(ramp) + Math.Abs(rampf));
                        //c幅度小于c+1幅度，rc幅度大于rc+1幅度，需要计算c、c+1，rc、rc+1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampf) < 1 && Math.Abs(ramp) / Math.Abs(rampf) > 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = false;
                    }
                    //c-1下沿高于c上沿，可以计算c在c-1方向的拉伸后幅度
                    if (Low(_c - 1) > High(_c))
                    {
                        //按照rc与rc-1的幅度比例计算c下沿到c-1上沿幅度中c可占的幅度（c在c-1方向的拉伸后幅度）
                        ampv1 = KRiseLH(_c, _c - 1) * Math.Abs(ramp) / (Math.Abs(ramp) + Math.Abs(rampp));
                        //c幅度小于c-1幅度，rc幅度大于rc-1幅度，需要计算c、c-1，rc、rc-1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampp) < 1 && Math.Abs(ramp) / Math.Abs(rampp) > 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = true;
                    }
                    //c+1下沿高于c上沿，可以计算c在c+1方向的拉伸后幅度
                    if (_c + 1 <= this.Count - 1 && _rc + 1 <= rkb.Count - 1 && Low(_c + 1) > High(_c))
                    {
                        //按照rc与rc+1的幅度比例计算c下沿到c+1上沿幅度中c可占的幅度（c在c+1方向的拉伸后幅度）
                        ampv2 = KRiseLH(_c, _c + 1) * Math.Abs(ramp) / (Math.Abs(ramp) + Math.Abs(rampf));
                        //c幅度小于c+1幅度，rc幅度大于rc+1幅度，需要计算c、c+1，rc、rc+1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampf) < 1 && Math.Abs(ramp) / Math.Abs(rampf) > 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = true;
                    }
                    if (ampv1 > Math.Abs(ramp))
                    {
                        ampv1 = Math.Abs(ramp);
                    }
                    if (ampv2 > Math.Abs(ramp))
                    {
                        ampv2 = Math.Abs(ramp);
                    }

                    if (ampv1 >= 0 && amp < 0 && !stretTop)
                    {
                        tmpprc = Open(_c) * (1 + (-ampv1));
                    }
                    //c上升，向下拉伸，tmpprc是open
                    else if (ampv1 >= 0 && amp >= 0 && !stretTop)
                    {
                        tmpprc = Close(_c) / (1 + ampv1);
                    }
                    //c下降，向上拉伸，tmpprc是open
                    else if (ampv1 >= 0 && amp < 0 && stretTop)
                    {
                        tmpprc = Close(_c) / (1 + (-ampv1));
                    }
                    //c上升，向上拉伸，tmpprc是open
                    else if (ampv1 >= 0 && amp > 0 && stretTop)
                    {
                        tmpprc = Open(_c) * (1 + ampv1);
                    }

                    if (ampv1 >= 0 && !stretTop)
                    {
                        if (tmpprc < High(_c - 1))
                        {
                            tmpprc = High(_c - 1);
                        }
                    }
                    //向下拉神，ampstret保存的是c-1方向的拉伸后幅度
                    else if(ampv1 >= 0)
                    {
                        if (tmpprc > Low(_c - 1))
                        {
                            tmpprc = Low(_c - 1);
                        }
                    }


                    //根据c上升或下降计算“相对准确”的c拉伸后幅度
                    if (ampv1 >= 0 && amp < 0 && !stretTop)
                    {
                        ampv1 = DvalueRatio(tmpprc, Open(_c));
                    }
                    else if (ampv1 >= 0 && amp >= 0 && !stretTop)
                    {
                        ampv1 = DvalueRatio(Close(_c), tmpprc);
                    }
                    else if (ampv1 >= 0 && amp < 0 && stretTop)
                    {
                        ampv1 = DvalueRatio(Close(_c), tmpprc);
                    }
                    else if (ampv1 >= 0 && amp >= 0 && stretTop)
                    {
                        ampv1 = DvalueRatio(tmpprc, Open(_c));
                    }


                    if (ampv2 >= 0 && amp < 0 && !stretTop)
                    {
                        tmpprc = Open(_c) * (1 + (-ampv2));
                    }
                    //c上升，向下拉伸，tmpprc是open
                    else if (ampv2 >= 0 && amp >= 0 && !stretTop)
                    {
                        tmpprc = Close(_c) / (1 + ampv2);
                    }
                    //c下降，向上拉伸，tmpprc是open
                    else if (ampv2 >= 0 && amp < 0 && stretTop)
                    {
                        tmpprc = Close(_c) / (1 + (-ampv2));
                    }
                    //c上升，向上拉伸，tmpprc是open
                    else if (ampv2 >= 0 && amp > 0 && stretTop)
                    {
                        tmpprc = Open(_c) * (1 + ampv2);
                    }

                    if (ampv2 >= 0 && !stretTop)
                    {
                        if (tmpprc < High(_c + 1))
                        {
                            tmpprc = High(_c + 1);
                        }
                    }
                    //向下拉神，ampstret保存的是c-1方向的拉伸后幅度
                    else if(ampv2 >= 0)
                    {
                        if (tmpprc > Low(_c + 1))
                        {
                            tmpprc = Low(_c + 1);
                        }
                    }

                    if (ampv2 >= 0 && amp < 0 && !stretTop)
                    {
                        ampv2 = DvalueRatio(tmpprc, Open(_c));
                    }
                    else if (ampv2 >= 0 && amp >= 0 && !stretTop)
                    {
                        ampv2 = DvalueRatio(Close(_c), tmpprc);
                    }
                    else if (ampv2 >= 0 && amp < 0 && stretTop)
                    {
                        ampv2 = DvalueRatio(Close(_c), tmpprc);
                    }
                    else if (ampv2 >= 0 && amp >= 0 && stretTop)
                    {
                        ampv2 = DvalueRatio(tmpprc, Open(_c));
                    }

                    if (ampv2 > -999f && ampv1 == -999f)
                    {
                        ampstret = ampv2;
                    }
                    else if (ampv2 == -999f && ampv1 > -999f)
                    {
                        ampstret = ampv1;
                    }
                    else if (ampv2 > -999f && ampv1 > -999f && Math.Abs(Math.Abs(ampv1) - Math.Abs(ramp)) < Math.Abs(Math.Abs(ampv2) - Math.Abs(ramp)))
                    {
                        ampstret = ampv1;
                    }
                    else if (ampv2 > -999f && ampv1 > -999f)
                    {
                        ampstret = ampv2;
                    }
                    if (ampstret > -999f)
                    {
                        ampscore2 = computeAmpScore(ramp, ampstret);
                    }
                }

                //计算c日幅度相对于rc日拉伸后幅度的评分
                //rc-1下沿高于rc上沿或rc+1下沿高于rc上沿或rc-1上沿低于rc下沿或rc+1上沿低于rc下沿
                //c与rc同为上升或同为下降
                //c幅度绝对值大于rc幅度绝对值
                else if (
                        (rHigh(_rc - 1) < rLow(_rc) || rHigh(_rc + 1) < rLow(_rc) || rLow(_rc - 1) > rHigh(_rc) || rLow(_rc + 1) > rHigh(_rc))
                        &&
                        ((amp >= 0 && ramp >= 0) || (amp <= 0 && ramp <= 0))
                        &&
                        Math.Abs(amp) > Math.Abs(ramp)
                        )
                {
                    //标记是否向上拉伸
                    bool stretTop = false;

                    //rc-1下沿低于rc上沿，可以计算rc在rc-1方向的拉伸后幅度
                    if (rHigh(_rc - 1) < rLow(_rc))
                    {
                        //按照c与c-1的幅度比例计算rc下沿到rc-1上沿幅度中rc可占的幅度（rc在rc-1方向的拉伸后幅度）
                        ampv1 = rKRiseLH(_rc - 1, _rc) * Math.Abs(amp) / (Math.Abs(amp) + Math.Abs(ampp));
                        //c幅度大于c-1幅度，rc幅度小于rc-1幅度，需要计算c、c-1，rc、rc-1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampp) > 1 && Math.Abs(ramp) / Math.Abs(rampp) < 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = false;
                    }
                    //rc+1下沿低于rc上沿，可以计算rc在rc+1方向的拉伸后幅度
                    if (_c + 1 <= this.Count - 1 && _rc + 1 <= rkb.Count - 1 && rHigh(_rc + 1) < rLow(_rc))
                    {
                        //按照c与c+1的幅度比例计算rc下沿到rc+1上沿幅度中rc可占的幅度（rc在rc+1方向的拉伸后幅度）
                        ampv2 = rKRiseLH(_rc + 1, _rc) * Math.Abs(amp) / (Math.Abs(amp) + Math.Abs(ampf));
                        //c幅度大于c+1幅度，rc幅度小于rc+1幅度，需要计算c、c+1，rc、rc+1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampf) > 1 && Math.Abs(ramp) / Math.Abs(rampf) < 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = false;
                    }
                    //rc-1下沿高于rc上沿，可以计算rc在rc-1方向的拉伸后幅度
                    if (rLow(_rc - 1) > rHigh(_rc))
                    {
                        //按照c与c-1的幅度比例计算rc下沿到rc-1上沿幅度中rc可占的幅度（rc在rc-1方向的拉伸后幅度）
                        ampv1 = rKRiseLH(_rc, _rc - 1) * Math.Abs(amp) / (Math.Abs(amp) + Math.Abs(ampp));
                        //c幅度大于c-1幅度，rc幅度小于rc-1幅度，需要计算c、c-1，rc、rc-1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampp) > 1 && Math.Abs(ramp) / Math.Abs(rampp) < 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = true;
                    }
                    //rc+1下沿高于rc上沿，可以计算rc在rc+1方向的拉伸后幅度
                    if (_c + 1 <= this.Count - 1 && _rc + 1 <= rkb.Count - 1 && rLow(_rc + 1) > rHigh(_rc))
                    {
                        //按照c与c+1的幅度比例计算rc下沿到rc+1上沿幅度中rc可占的幅度（rc在rc+1方向的拉伸后幅度）
                        ampv2 = rKRiseLH(_rc, _rc + 1) * Math.Abs(amp) / (Math.Abs(amp) + Math.Abs(ampf));
                        //c幅度大于c+1幅度，rc幅度小于rc+1幅度，需要计算c、c-1，rc、rc-1的幅度对比分数
                        if (Math.Abs(amp) / Math.Abs(ampf) > 1 && Math.Abs(ramp) / Math.Abs(rampf) < 1)
                        {
                        }
                        else
                        {
                            ampcpr = 0f;
                        }
                        stretTop = true;
                    }

                    if (ampv1 > Math.Abs(amp))
                    {
                        ampv1 = Math.Abs(amp);
                    }
                    if (ampv2 > Math.Abs(amp))
                    {
                        ampv2 = Math.Abs(amp);
                    }

                    if (ampv1 >= 0 && ramp < 0 && !stretTop)
                    {
                        tmpprc = rOpen(_rc) * (1 + (-ampv1));
                    }
                    //rc上升，向下拉伸，tmpprc是open
                    else if (ampv1 >= 0 && ramp >= 0 && !stretTop)
                    {
                        tmpprc = rClose(_rc) / (1 + ampv1);
                    }
                    //rc下降，向上拉伸，tmpprc是open
                    else if (ampv1 >= 0 && ramp < 0 && stretTop)
                    {
                        tmpprc = rClose(_rc) / (1 + (-ampv1));
                    }
                    //rc上升，向上拉伸，tmpprc是open
                    else if (ampv1 >= 0 && ramp > 0 && stretTop)
                    {
                        tmpprc = rOpen(_rc) * (1 + ampv1);
                    }

                    if (ampv1 >= 0 && !stretTop)
                    {
                        if (tmpprc < rHigh(_rc - 1))
                        {
                            tmpprc = rHigh(_rc - 1);
                        }
                    }
                    //向下拉神，ampstret保存的是c-1方向的拉伸后幅度
                    else if(ampv1>=0)
                    {
                        if (tmpprc > rLow(_rc - 1))
                        {
                            tmpprc = rLow(_rc - 1);
                        }
                    }

                    //根据c上升或下降计算“相对准确”的c拉伸后幅度
                    if (ampv1 >= 0 && ramp < 0 && !stretTop)
                    {
                        ampv1 = DvalueRatio(tmpprc, rOpen(_rc));
                    }
                    else if (ampv1 >= 0 && ramp >= 0 && !stretTop)
                    {
                        ampv1 = DvalueRatio(rClose(_rc), tmpprc);
                    }
                    else if (ampv1 >= 0 && ramp < 0 && stretTop)
                    {
                        ampv1 = DvalueRatio(rClose(_rc), tmpprc);
                    }
                    else if (ampv1 >= 0 && ramp >= 0 && stretTop)
                    {
                        ampv1 = DvalueRatio(tmpprc, rOpen(_rc));
                    }


                    if (ampv2 >= 0 && ramp < 0 && !stretTop)
                    {
                        tmpprc = rOpen(_rc) * (1 + (-ampv2));
                    }
                    //c上升，向下拉伸，tmpprc是open
                    else if (ampv2 >= 0 && ramp >= 0 && !stretTop)
                    {
                        tmpprc = rClose(_rc) / (1 + ampv2);
                    }
                    //c下降，向上拉伸，tmpprc是open
                    else if (ampv2 >= 0 && ramp < 0 && stretTop)
                    {
                        tmpprc = rClose(_rc) / (1 + (-ampv2));
                    }
                    //c上升，向上拉伸，tmpprc是open
                    else if (ampv2 >= 0 && ramp > 0 && stretTop)
                    {
                        tmpprc = rOpen(_rc) * (1 + ampv2);
                    }

                    if (ampv2 >= 0 && !stretTop)
                    {
                        if (tmpprc < rHigh(_rc + 1))
                        {
                            tmpprc = rHigh(_rc + 1);
                        }
                    }
                    //向下拉神，ampstret保存的是c-1方向的拉伸后幅度
                    else if(ampv2 >= 0)
                    {
                        if (tmpprc > rLow(_rc + 1))
                        {
                            tmpprc = rLow(_rc + 1);
                        }
                    }

                    if (ampv2 >= 0 && ramp < 0 && !stretTop)
                    {
                        ampv2 = DvalueRatio(tmpprc, rOpen(_rc));
                    }
                    else if (ampv2 >= 0 && ramp >= 0 && !stretTop)
                    {
                        ampv2 = DvalueRatio(rClose(_rc), tmpprc);
                    }
                    else if (ampv2 >= 0 && ramp < 0 && stretTop)
                    {
                        ampv2 = DvalueRatio(rClose(_rc), tmpprc);
                    }
                    else if (ampv2 >= 0 && ramp >= 0 && stretTop)
                    {
                        ampv2 = DvalueRatio(tmpprc, rOpen(_rc));
                    }

                    if(ampv2 > -999f && ampv1 == -999f)
                    {
                        ampstret = ampv2;
                    }
                    else if(ampv2 == -999f && ampv1 > -999f)
                    {
                        ampstret = ampv1;
                    }
                    else if (ampv2 > -999f && ampv1 > -999f && Math.Abs(Math.Abs(ampv1) - Math.Abs(amp)) < Math.Abs(Math.Abs(ampv2) - Math.Abs(amp)))
                    {
                        ampstret = ampv1;
                    }
                    else if (ampv2 > -999f && ampv1 > -999f)
                    {
                        ampstret = ampv2;
                    }
                    if (ampstret > -999f)
                    {
                        ampscore2 = computeAmpScore(ampstret, amp);
                    }
                    
                } 

            }

            _ampScore = ampscore1;
            _highScore = highscore1;
            //计算过ampscore2
            if (ampscore2>=0)
            {
                //ampscore2低于ampscore1，令_ampScore为二者平均值，得以在分数上体现符合ampscore2的情况
                if (ampscore2 < ampscore1)
                {
                     if(r<0)
                    {
                        r = 0;
                    }
                    //_ampScore = ampscore1 * 0.7f + ampscore2 * 0.3f;
                    //_ampScore = (ampscore1 * 0.2f + ampscore2 * 0.8f)/2 + ampcpr;
                    _ampScore = ampscore1 * 0.2f + ampscore2 * 0.8f;
                    if (_ampScore > ampscore1)
                    {
                        _ampScore = ampscore1;
                    }
                }
            }

            float neibampscore = -1f;
            float neibampscore1 = -1f;
            //目前只对拉伸评价过的c计算临日幅度对比分数（ampstret初始值为-999f，不为此值时说明拉伸评价过）
            if (_c < _t && ampstret>-999f)
            {
                //ampv2是向c+1、rc+1方向拉伸，之前拉伸评价幅度采用ampv2、ampv1中较大的进行计算分数，此处相同
                //if (ampv2 > ampv1)
                if (ampstret == ampv2)
                {
                    //c幅度大于c+1幅度，rc幅度小于rc+1幅度，保持neibampscore<0的标记（需要计算c、c+1，rc、rc+1的幅度对比分数）
                    if (Math.Abs(amp) / Math.Abs(ampf) > 1 && Math.Abs(ramp) / Math.Abs(rampf) < 1)
                    {
                    }
                    //c幅度小于c+1幅度，rc幅度大于rc+1幅度，保持neibampscore<0的标记（需要计算c、c+1，rc、rc+1的幅度对比分数）
                    else if (Math.Abs(amp) / Math.Abs(ampf) < 1 && Math.Abs(ramp) / Math.Abs(rampf) > 1)
                    {
                    }
                    //c幅度与c+1幅度的大小对比，与rc幅度与rc+1幅度的大小对比相同时令neibampscore=0（不计算c、c+1，rc、rc+1的幅度对比分数）
                    else
                    {
                        neibampscore = 0f;
                    }
                }
                else if (ampstret == ampv1)
                {
                    if (Math.Abs(amp) / Math.Abs(ampp) > 1 && Math.Abs(ramp) / Math.Abs(rampp) < 1)
                    {
                    }
                    else if (Math.Abs(amp) / Math.Abs(ampp) < 1 && Math.Abs(ramp) / Math.Abs(rampp) > 1)
                    {
                    }
                    else
                    {
                        neibampscore = 0f;
                    }
                }

                if (neibampscore < 0 && ampstret == ampv2)
                {
                    neibampscore = computeAmpScore(Math.Abs(amp), Math.Abs(ampf));
                    neibampscore1 = computeAmpScore(Math.Abs(ramp), Math.Abs(rampf));

                }
                else if (neibampscore < 0 && ampstret == ampv1)
                {
                    neibampscore = computeAmpScore(Math.Abs(amp), Math.Abs(ampp));
                    neibampscore1 = computeAmpScore(Math.Abs(ramp), Math.Abs(rampp));
                }
                //前提是c的邻日幅度对比（c大或临日大）与rc的邻日幅度对比相反时计算ampcpr和ampcpr1
                //ampcpr或ampcpr1越小则说明c或rc与邻日幅度越接近。ampcpr或ampcpr1小到一定程度时，
                //虽然c的邻日幅度对比与rc的邻日幅度对比相反，但其中存在相邻日幅度非常接近的情况，
                //因此可以粗略地认为此时c的邻日幅度对比与rc的邻日幅度对比相同（即c、rc均大于或小于临日）
                //ampcpr或ampcpr1越小越可以认为c或rc拉伸后幅度对比接近
                if (neibampscore1 < neibampscore)
                {
                    neibampscore = neibampscore1;
                }
            }
            else
            {
                
            }
            if(neibampscore<0)
            {
                neibampscore = 0;
            }
            _neibAmpScore = neibampscore;




            float neibhscore1 = -1f;
            float neibhscore2 = -1f;
            //c或c+1上沿改变后的值
            float vh = 0f;
            //参照股票rc到rc+1的最大幅度
            //此幅度大则评分相对宽松些
            float baseamp = 0f;
            if (rLow(_rc) < rLow(_rc + 1))
            {
                baseamp = rLow(_rc);
            }
            else
            {
                baseamp = rLow(_rc + 1);
            }
            if (rHigh(_rc) > rHigh(_rc + 1))
            {
                baseamp = DvalueRatio(rHigh(_rc), baseamp);
            }
            else 
            {
                baseamp = DvalueRatio(rHigh(_rc + 1), baseamp);
            }
            if (DvalueRatio(rHigh(_rc), rHigh(_rc + 1))<0)
            {
                baseamp = -baseamp;
            }
            //实验性减小rc到rc+1的最大幅度，使最终得分增大
            //baseamp = baseamp / 2;
            r = 0f;

            //浮动c或c+1上沿，更宽容地计算c与c+1上沿的评分
            //不计算_t
            if (_c < _t)
            {
                neibhscore1 = computeAmpScore(baseamp + DvalueRatio(rHigh(_rc), rHigh(_rc + 1)), baseamp + DvalueRatio(High(_c), High(_c + 1)));
               
                //c上沿低于c+1下沿
                //c上沿到c+1上沿的距离大于rc上沿到rc+1上沿的距离
                if (
                    High(_c) < Low(_c + 1)
                    &&
                    DvalueRatio(High(_c + 1), High(_c)) > DvalueRatio(rHigh(_rc + 1), rHigh(_rc))
                    )
                {
                    //按照rc上沿到rc+1上沿的距离计算c上沿相对c+1上沿挪动后的值
                    vh = High(_c + 1) / (1 + DvalueRatio(rHigh(_rc + 1), rHigh(_rc)));
                    //c上沿挪动后的值不应高于c+1下沿
                    if (vh > Low(_c + 1))
                    {
                        vh = Low(_c + 1);
                    }
                    r = DvalueRatio(vh, High(_c)) * 10;
                    neibhscore2 = computeAmpScore(baseamp + DvalueRatio(rHigh(_rc), rHigh(_rc + 1)), baseamp + DvalueRatio(vh, High(_c + 1)));
                }
                //rc上沿低于rc+1下沿
                //rc上沿到rc+1上沿的距离大于c上沿到c+1上沿的距离
                else if (
                    rHigh(_rc) < rLow(_rc + 1)
                    &&
                    DvalueRatio(High(_c + 1), High(_c)) < DvalueRatio(rHigh(_rc + 1), rHigh(_rc))
                    )
                {
                    //按照c上沿到c+1上沿的距离计算rc上沿相对rc+1上沿挪动后的值
                    vh = rHigh(_rc + 1) / (1 + DvalueRatio(High(_c + 1), High(_c)));
                    //rc上沿挪动后的值不应高于rc+1下沿
                    if (vh > rLow(_rc + 1))
                    {
                        vh = rLow(_rc + 1);
                    }
                    r = DvalueRatio(vh, rHigh(_rc)) * 10;
                    neibhscore2 = computeAmpScore(baseamp + DvalueRatio(vh, rHigh(_rc + 1)), baseamp + DvalueRatio(High(_c), High(_c + 1)));
                }
                //c下沿高于c+1上沿
                //c+1上沿到c上沿的距离大于rc+1上沿到rc上沿的距离
                else if (
                    Low(_c) > High(_c + 1)
                    &&
                    DvalueRatio(High(_c), High(_c + 1)) > DvalueRatio(rHigh(_rc), rHigh(_rc + 1))
                    )
                {
                    //按照rc+1上沿到rc上沿的距离计算c+1上沿相对c上沿挪动后的值
                    vh = High(_c) / (DvalueRatio(rHigh(_rc), rHigh(_rc + 1)) + 1);
                    //c+1上沿挪动后的值不应高于c下沿
                    if (vh > Low(_c))
                    {
                        vh = Low(_c);
                    }
                    r = DvalueRatio(vh, High(_c + 1)) * 10;
                    neibhscore2 = computeAmpScore(baseamp + DvalueRatio(rHigh(_rc), rHigh(_rc + 1)), baseamp + DvalueRatio(High(_c), vh));
                }
                //rc下沿高于rc+1上沿
                //rc+1上沿到rc上沿的距离大于c+1上沿到c上沿的距离
                else if (
                    rLow(_rc) > rHigh(_rc + 1)
                    &&
                    DvalueRatio(High(_c), High(_c + 1)) < DvalueRatio(rHigh(_rc), rHigh(_rc + 1))
                    )
                {
                    //按照c+1上沿到c上沿的距离计算rc+1上沿相对rc上沿挪动后的值
                    vh = rHigh(_rc) / (DvalueRatio(High(_c), High(_c + 1)) + 1);
                    //rc+1上沿挪动后的值不应高于rc下沿
                    if (vh > rLow(_rc))
                    {
                        vh = rLow(_rc);
                    }
                    r = DvalueRatio(vh, rHigh(_rc + 1)) * 10;
                    neibhscore2 = computeAmpScore(baseamp + DvalueRatio(rHigh(_rc), vh), baseamp + DvalueRatio(High(_c), High(_c + 1)));
                }
            }
            if (neibhscore1<0)
            {
                neibhscore1 = 0;
            }
            _neibHScore = neibhscore1;
            if (neibhscore2>=0)
            {
                if (neibhscore2 < neibhscore1)
                {
                    _neibHScore = neibhscore1 * r + neibhscore2 * (1 - r);
                }
            }

             _totalScore = _ampScore + _highScore; 
        }


        private float computeAmpScore(float amp_rc, float amp_c)
        {
            //rc或c中幅度较大的幅度
            //暂时令a=rc的幅度
            float a = amp_rc;
            //rc或c中幅度较小的幅度
            //暂时令b=c的幅度
            float b = amp_c;

            //加权数
            float r = 1;

            float tmp = 0;
            //x*y*10
            float score = 0f;

            if(a==0)
            {
                a = 0.001f;
            }
            if(b==0)
            {
                b = 0.001f;
            }

            //根据a、b的大小、正负关系，重新整理a、b值，令a>b>0（负值也要转换为正值进行计算）
            //rc幅度小于c幅度且均为正。a=c升幅，b=rc升幅
            if (a < b && a >= 0 && b >= 0)
            {
                tmp = a;
                a = b;
                b = tmp;
            }
            //rc幅度为正，c幅度为负。b转换为c的降幅的绝对值+rc的升幅
            else if (a >= 0 && b <= 0)
            {
                b = -b;
                if(a>b)
                {
                    //a = a + b + b;
                    a = a + b;
                }
                else
                {
                    tmp = a;
                    a = b;
                    b = tmp;
                    //a = a + b + b;
                    a = a + b;
                }
            }
            //rc幅度为负，c幅度为正，a=c升幅。b转换为rc的降幅的绝对值+c的升幅
            else if (a <= 0 && b >= 0)
            {
                a = -a;
                if (a > b)
                {
                    //a = a + b + b;
                    a = a + b;
                }
                else
                {
                    tmp = a;
                    a = b;
                    b = tmp;
                    //a = a + b + b;
                    a = a + b;
                }
            }
            //rc幅度为负，c幅度为负，rc幅度的绝对值小于c幅度的绝对值。a=c幅度的绝对值，b=rc幅度的绝对值
            else if (a <= 0 && b <= 0 && a >= b)
            {
                tmp = a;
                a = -b;
                b = -tmp;
            }
            //rc幅度为负，c幅度为负，rc幅度的绝对值大于c幅度的绝对值。a=rc幅度的绝对值，b=c幅度的绝对值
            else if (a <= 0 && b <= 0 && a <= b)
            {
                a = -a;
                b = -b;
            }
            if (b < 0.0035)
            {
                b = 0.0035f;
            }
            if (b > a)
            {
                b = a;
            }
            float c = Math.Abs(amp_rc);
            if (c < Math.Abs(amp_c))
            {
                c = Math.Abs(amp_c);
            }
            //根据rc的日周围平均日k线幅度对较小的a、b、c幅度补齐，使它们达到一个“小幅度”范畴内合适的幅度
            //使“小幅度持平”这种情况中的k线柱在遇到“小幅度”范畴内的对比k线时可以得到较好的评价
            //需配合加权数使用（目前在此对象外加权）
            if(rkb.avgNAmpList[_rc].value<0.02f)
            {
                if(a<0.015)
                {
                    a = 0.015f;
                }
                if(b<0.015)
                {
                    b = 0.015f;
                }
                if(c<0.015)
                {
                    c = 0.015f;
                }
            }
            //score = (float)Math.Pow((a - b) * (a - b)/b, 1f / 4f) * 100 * r;
            //score =(float)Math.Pow((a - b),1f/3f) * ((a - b) / b);
            //score = (float)Math.Pow(c, 1f / 2f) * 300 * (float)Math.Pow((a - b) / b, 1f / 2f);
            score = (float)Math.Pow(c, 0.47f) * 345 * (float)Math.Pow((a - b) / b, 0.78f);
            return score;
        }

        private float computePriceScore(float price_b, float price)
        {
            float score = 0f;
            float tmp = 0f;
            if(price_b==price)
            {
                return score;
            }
            if (price_b == 0)
            {
                price_b = 0.001f;
            }
            if (price == 0)
            {
                price = 0.001f;
            }
            else if(price_b<0 && price>=0)
            {
                //score=-price_b+price;
                price_b = -price_b;
                if(price_b>price)
                {
                    price_b = price_b + price + price;
                }
                else
                {
                    tmp = price_b;
                    price_b = price + price_b + price_b;
                    price = tmp;
                }
            }
            else if(price_b>=0 && price<0)
            {
                //score = -price + price_b;
                price = -price;
                if (price_b > price)
                {
                    price_b = price_b + price + price;
                }
                else
                {
                    tmp = price_b;
                    price_b = price + price_b + price_b;
                    price = tmp;
                }
            }
            else if(price_b>=0 && price>=0)
            {
                //score = Math.Abs(price_b - price);
                if (price_b < price)
                {
                    tmp = price;
                    price = price_b;
                    price_b = tmp;
                }
            }
            else if(price_b<=0 && price<=0)
            {
                price = -price;
                price_b = -price_b;
                if(price_b<price)
                {
                    tmp = price;
                    price = price_b;
                    price_b = tmp;
                }
                //score = Math.Abs(price_b - price);
            }
            //score = (float)Math.Pow((price_b - price) * 200, 2);
            //score = (float)Math.Pow((price_b - price) * (price_b - price) / price, 1f / 2f) * 200;
            //score = (float)Math.Pow(price_b, 1f / 2f) * 300 * (float)Math.Pow((price_b - price) / price, 1f / 2f);
            score = (float)Math.Pow(price_b, 0.47f) * 345 * (float)Math.Pow((price_b - price) / price, 0.78);
            return score;
        }

        private float rHigh(int t)
        {
            return rkb.High(t);
        }
        private float rOpen(int t)
        {
            return rkb.Open(t);
        }
        private float rClose(int t)
        {
            return rkb.Close(t);
        }
        private float rLow(int t)
        {
            return rkb.Low(t);
        }

        private float rKRise(int t, int offset)
        {
            return rkb.KRise(t, offset);
        }

        private float rKRiseLH(int indexOfLow,int indexOfHigh)
        {
            return rkb.KRiseLH(indexOfLow, indexOfHigh);
        }

    }
}
