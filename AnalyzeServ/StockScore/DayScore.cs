using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class DayScore : KBase
    {

        private KBase rkb;
        /// <summary>
        /// 待评价股票的基准日
        /// </summary>
        private int _t;
        /// <summary>
        /// 参照股票的基准日
        /// </summary>
        private int _rt;
        /// <summary>
        /// 待评价股票的待评价当前日
        /// </summary>
        private int _c;
        /// <summary>
        /// 参照股票的参照当前日
        /// </summary>
        private int _rc;

        private float _totalscore;

        private ReferenceStockInfo _refsi;

        private bool _fit;

        public bool Fit
        {
            get
            {
                return _fit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <param name="rt"></param>
        /// <param name="rc"></param>
        /// <param name="kbase"></param>
        /// <param name="refsi"></param>
        public DayScore(int t, int c, int rt, int rc, KBase kbase, ReferenceStockInfo refsi)
            :base(kbase)
        {
            _t = t;
            _rt = rt;
            _c = c;
            _rc = rc;
            _refsi = refsi;
            this.rkb = refsi;
            compareKDayFrame();
        }

        private void compareKDayFrame()
        {
            //得到适合c的参照值，用来计算c各值相对参照值的比例
            float refvalue = findRefValue(_c, _t);
            //上沿比例
            float highp = DvalueRatio(this[_c].High, refvalue);
            //下沿比例
            float lowp = DvalueRatio(this[_c].Low, refvalue);
            //kp幅度
            float ckpamp = kpAmp(_c);

            kPiece rkp=_refsi.kpSet.findKPiece(_rt);

            //int acount = _rt - rkp.Begin + 1;
            int acount = 4;
            //if(acount<3 || acount>3)
            //{
            //    acount = 3;
            //}
            //待评价kp和参照kp的幅度比作为框架的调整比率
            //if(rkp.End!=_rt)
            if (_rc != _rt)
            {
                _refsi.kDayFrameSet[_rc].Ratio = _refsi.kDayFrameSet[_rc].Ratio * ckpamp / _refsi.kDayFrameSet[_rc].kpAMP;
            }
           
            //判断待评价k线柱是否在框架内
            if (
                _t - _c <= acount - 1
                &&
                    (
                        highp > _refsi.kDayFrameSet[_rc].HighH || highp < _refsi.kDayFrameSet[_rc].HighL
                        ||
                        lowp > _refsi.kDayFrameSet[_rc].LowH || lowp < _refsi.kDayFrameSet[_rc].LowL
                        ||
                        DvalueRatio(High(_c), Low(_c)) < _refsi.kDayFrameSet[_rc].AbsAmpL
                    )
                )
            {
                _fit = false;
                return;
            }
            //待评价k线柱与框架的升降方向应一致
            if (
                _t - _c <= acount - 1
                &&
                (//排除连续小幅持平的情况：日周围平均日k线幅度较小
                    (_refsi.avgNAmpList[_rc].value>=0.017f
                     ||//排除连续小幅持平的情况：临近两日总的幅度较小
                     Math.Abs(DvalueRatio(rHigh(_rc),rLow(_rc-1)))>0.03f
                     ||
                     Math.Abs(DvalueRatio(rHigh(_rc - 1), rLow(_rc))) > 0.03f
                     ||
                     (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc), rLow(_rc + 1))) > 0.03f)
                     ||
                     (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc + 1), rLow(_rc))) > 0.03f)
                     ||//c日幅度较大也应判断升降方向
                     Math.Abs(KRise(_c, 0)) > 0.017f
                     ||//rc日幅度较大也应判断升降方向
                     Math.Abs(_refsi.KRise(_rc, 0)) > 0.017f
                     )
                 )
                &&
                    (
                        KRise(_c,0)>0 &&rKRise(_rc,0)<0
                        ||
                        KRise(_c, 0) < 0 && rKRise(_rc, 0) > 0
                    )
                )
            {
                _fit = false;
                return;
            }
            //待评价c与c-1的k线柱大与小应和框架的rc与rc-1一致
            if (
                _t - _c <= acount - 1
                &&
                //(this[_c-1].Low>this[_c].High || this[_c-1].High<this[_c].Low)
                //&&
                (
                _refsi.avgNAmpList[_rc].value >= 0.017f
                 ||
                 Math.Abs(DvalueRatio(rHigh(_rc), rLow(_rc - 1))) > 0.03f
                 ||
                 Math.Abs(DvalueRatio(rHigh(_rc - 1), rLow(_rc))) > 0.03f
                 ||
                 (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc), rLow(_rc + 1))) > 0.03f)
                 ||
                 (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc + 1), rLow(_rc))) > 0.03f)
                 )
                &&
                (
                    (KRise(_c-1,0)>KRise(_c,0) && rKRise(_rc-1,0)<rKRise(_rc,0))
                    ||
                    (KRise(_c-1,0)<KRise(_c,0) && rKRise(_rc-1,0)>rKRise(_rc,0))
                )
                )
            {
                //_fit = false;
                //return;
            }
            //待评价c与c+1的k线柱大与小应和框架的rc与rc+1一致
            if (
                _t - _c <= acount - 1
                &&
                //_c < _t 
                //&& (this[_c + 1].Low > this[_c].High || this[_c + 1].High < this[_c].Low)
                //&&
                //_c=_t-1时不用判断，既不判定t-1和t的升幅谁大谁小
                _c + 1 < _t
                &&
                (
                _refsi.avgNAmpList[_rc].value >= 0.017f
                 ||
                 Math.Abs(DvalueRatio(rHigh(_rc), rLow(_rc - 1))) > 0.03f
                 ||
                 Math.Abs(DvalueRatio(rHigh(_rc - 1), rLow(_rc))) > 0.03f
                 ||
                 (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc), rLow(_rc + 1))) > 0.03f)
                 ||
                 (_rc + 1 < _rt && Math.Abs(DvalueRatio(rHigh(_rc + 1), rLow(_rc))) > 0.03f)
                 )
                &&
                (
                    (KRise(_c + 1, 0) > KRise(_c, 0) && rKRise(_rc + 1, 0) < rKRise(_rc, 0))
                    ||
                    (KRise(_c + 1, 0) < KRise(_c, 0) && rKRise(_rc + 1, 0) > rKRise(_rc, 0))
                )
                )
            {
                //_fit = false;
                //return;
            }
            _fit = true;
            
            ///
            ///因为框架的算法导致相邻k线柱的框架高低值间存在空隙，拉伸高低值后两个框架有重合部分，
            ///因此在评价完当前k线柱后才能确定相邻框架的高值或低值，以解决重合问题，此时需要调整相邻框架
            ///
            //得到适合c-1的参照值，用来调整rc-1的框架
            //因为适合c与适合c-1的参照值可能不一样，所以依据参照值计算的各值比例并非同一参照标准。
            //调整rc-1框架时，需要参考c的上、下沿，rc-1框架上下沿应在c上下沿附近。因为可能存在上述参照标准
            //不统一的情况，因此假设c上、下沿出现在c-1位置，以c-1的参照值计算c的上、下沿比例，可解决参照标准问题
            //refvalue = findRefValue(_c - 1, _t);
            ////rc框架计算时c的下沿曾向rc-1的上沿拉伸，需调整rc-1框架的上沿
            //if (_refsi.kDayFrameSet[_rc].prevH && _refsi.kDayFrameSet.ContainsKey(_rc-1))
            //{
            //    _refsi.kDayFrameSet[_rc - 1].adjustHigh(DvalueRatio(this[_c].Low, refvalue));
            //}
            ////rc框架计算时c的上沿曾向rc-1的下沿拉伸，需调整rc-1框架的下沿
            //if (_refsi.kDayFrameSet[_rc].prevL && _refsi.kDayFrameSet.ContainsKey(_rc - 1))
            //{
            //    _refsi.kDayFrameSet[_rc - 1].adjustLow(DvalueRatio(this[_c].High, refvalue));
            //}
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

        private float rKRiseLH(int indexOfLow, int indexOfHigh)
        {
            return rkb.KRiseLH(indexOfLow, indexOfHigh);
        }
    }
}
