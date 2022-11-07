using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class KPieceScore:KBase
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

        private int _kpid;

        private int _kpfid;
        /// <summary>
        /// 上一个进行过评价的kpScore
        /// </summary>
        private KPieceScore _nkpsc;

        private kPieceFrame _kpframe;

        private kPiece _kp;

        private kPiece _rkp;

        private kPiece[] _kps;

        private kPiece[] _rkps;

        private float _totalscore;

        private ReferenceStockInfo _refsi;

        private bool _fit;

        private int _begin;

        private int _end;

        public kPieceFrame kpframe
        {
            get
            {
                return _kpframe;
            }
        }

        public int Begin
        {
            get
            {
                return _begin;
            }
        }

        public int End
        {
            get
            {
                return _end;
            }
        }

        public bool Fit
        {
            get
            {
                return _fit;
            }
        }

        public KPieceScore(int t, int rt, int kpfid, KPieceScore nkpsc, KBase kbase, ReferenceStockInfo refsi)
            :base(kbase)
        {
            _t = t;
            _rt = rt;
            //_kpid = kpid;
            _kpfid = kpfid;
            _nkpsc = nkpsc;
            _kps = kbase.kpSet.kPieces;
            _rkps = refsi.kpSet.kPieces;
            //_kp = kbase.kpSet.kPieces[_kpid];
            //_rkp = refsi.kpSet.kPieces[_rkpid];
            _refsi = refsi;
            this.rkb = refsi;
            _kpframe = _refsi.kpFrameSet[_kpfid];
            compareFrame();
        }

        private void compareFrame()
        {
            //kp上沿比例
            float htmp = 0;
            //kp下沿比例
            float ltmp = 0;
            //kp上沿对应k线索引
            int hi = 0;
            //kp下沿对应k线索引
            int li = 0;

            int b = 0;

            int e = 0;

            int bkid = 0;

            int ekid = 0;

            int tkpid = kpSet.findKPieceIndex(_t);
            //参照股票基准日所在的k线片段的索引
            int rtkpid = _refsi.kpSet.findKPieceIndex(_rt);

            float ratio = 1f;
            float ratior = 1f;

            //参照框架的end=rt，begin+1=rt，begin下降，此时不做评价
            if (
                _kpframe.End == _rt
                &&
                _kpframe.Begin + 1 == _rt
                &&
                _kpframe.Trend == kPieceTrend.Rise
                &&
                _refsi.KRise(_kpframe.Begin, 0) < 0
              )
            {
                ///
                ///精确b、e位置
                ///
                _begin = _t - (_rt - _kpframe.Begin);
                _end = _t - (_rt - _kpframe.End);
                bkid = kpSet.findKPieceIndex(_begin);
                ekid = kpSet.findKPieceIndex(_end);
                if (_kps[bkid].End == _begin)
                {
                    bkid++;
                }
                //_begin处于其所在kp的begin+1位置，且kp趋势偏移量+1，既_begin为kp内趋势的起点，此时令_begin等于kp.begin(向左对齐)
                //（例，趋势下降，kp.begin上升，_begin下降且_begin==kp.begin+1）
                if (_kps[bkid].BeginTO == 1 && _begin == _kps[bkid].Begin + 1)
                {
                    _begin--;
                }
                //_begin处于其所在kp的end-1位置，且该kp趋势与框架趋势相反，说明框架.begin对应的点应是kp+1.begin，既_begin+1(向右对齐)
                else if (_kps[bkid].Trend != _kpframe.Trend && _begin == _kps[bkid].End - 1)
                {
                    _begin++;
                }
                else if (_begin != _kps[bkid].Begin)
                {
                    _begin = _kps[bkid].Begin;
                }
                //_end处于其所在kp的end-1位置，且kp趋势偏移量-1，既_end为kp内趋势的终点，此时令_end等于kp.end(向右对齐)
                //（例，趋势下降，kp.end上升，_end下降且_end==kp.end-1）
                if (_kps[ekid].BeginTO == -1 && _end == _kps[ekid].End - 1)
                {
                    _end++;
                }
                //_end处于其所在kp的begin+1位置，且该kp趋势与框架趋势相反，说明框架.end对应的点应是kp-1.end，既_end-1(向左对齐)
                else if (_kps[ekid].Trend != _kpframe.Trend && _end == _kps[ekid].Begin + 1)
                {
                    _end--;
                }
                else if (_end != _kps[ekid].End)
                {
                    _end = _kps[ekid].End;
                }
                _fit = true;
                return;
            }



            //参照框架.begin距离rt较近，应精确评价待评价股票kp的长度和幅度
            if (
                   _kpframe.Begin >= _rt - 6
                   ||
                   (//rt所在kp上升，且该kp长度较短
                       (
                           (_rkps[rtkpid].End - _rkps[rtkpid].Begin <= 2 && _refsi.KRise(_rkps[rtkpid].Begin, 0) >= 0 && _rkps[rtkpid].Trend == kPieceTrend.Rise)
                           ||
                           (_rkps[rtkpid].End - _rkps[rtkpid].Begin <= 3 && _refsi.KRise(_rkps[rtkpid].Begin, 0) < 0 && _rkps[rtkpid].Trend == kPieceTrend.Rise)
                       )
                       &&
                       _kpframe.Begin >= _rkps[rtkpid].Begin - 6
                   )
               )
            {

                ///
                ///精确b、e位置
                ///
                _begin = _t - (_rt - _kpframe.Begin);
                _end = _t - (_rt - _kpframe.End);
                bkid = kpSet.findKPieceIndex(_begin);
                ekid = kpSet.findKPieceIndex(_end);
                if (_kps[bkid].End == _begin)
                {
                    bkid++;
                }
                //_begin处于其所在kp的begin+1位置，且kp趋势偏移量+1，既_begin为kp内趋势的起点，此时令_begin等于kp.begin(向左对齐)
                //（例，趋势下降，kp.begin上升，_begin下降且_begin==kp.begin+1）
                if (_kps[bkid].BeginTO == 1 && _begin == _kps[bkid].Begin + 1)
                {
                    _begin--;
                }
                //_begin处于其所在kp的end-1位置，且该kp趋势与框架趋势相反，说明框架.begin对应的点应是kp+1.begin，既_begin+1(向右对齐)
                else if (_kps[bkid].Trend != _kpframe.Trend && _begin == _kps[bkid].End - 1)
                {
                    _begin++;
                }
                else if (_begin != _kps[bkid].Begin)
                {
                    _begin = _kps[bkid].Begin;
                }
                //_end处于其所在kp的end-1位置，且kp趋势偏移量-1，既_end为kp内趋势的终点，此时令_end等于kp.end(向右对齐)
                //（例，趋势下降，kp.end上升，_end下降且_end==kp.end-1）
                if (_kps[ekid].BeginTO == -1 && _end == _kps[ekid].End - 1)
                {
                    _end++;
                }
                //_end处于其所在kp的begin+1位置，且该kp趋势与框架趋势相反，说明框架.end对应的点应是kp-1.end，既_end-1(向左对齐)
                else if (_kps[ekid].Trend != _kpframe.Trend && _end == _kps[ekid].Begin + 1)
                {
                    _end--;
                }
                else if (_end != _kps[ekid].End)
                {
                    _end = _kps[ekid].End;
                }
                //待评价股票趋势需与框架趋势一致
                if (
                    (Low(_begin) > Low(_end) && _kpframe.Trend == kPieceTrend.Rise)
                    ||
                    (Low(_begin) < Low(_end) && _kpframe.Trend == kPieceTrend.Fall)
                    )
                {
                    _fit = false;
                    return;
                }
                ///
                ///应尝试根据趋势和begin、end升降来确定长度差的范围
                ///
                if(Math.Abs((_t-_begin)-(_rt-_kpframe.Begin))>1)
                {
                    _fit = false;
                    return;
                }
                if (Math.Abs((_t - _end) - (_rt - _kpframe.End)) > 1)
                {
                    _fit = false;
                    return;
                }
                //在_begin和_end之间得到高低值索引值
                hi = _begin;
                li = _begin;
                for (int i = _begin; i <= _end; i++)
                {
                    if (High(i) > High(hi)
                        &&
                        //i为最后一天且上升时不参与最高点统计
                    !(Low(_begin) > Low(_end) && i == _end && KRise(i, 0) > 0))
                    {
                        hi = i;
                    }
                    if (Low(i) < Low(li))
                    {
                        li = i;
                    }
                }
                ////下降趋势，则以hi上沿为基准值
                //if (Low(_begin) > Low(_end))
                //{
                //    htmp = DvalueRatio(High(hi), High(hi));
                //    ltmp = DvalueRatio(Low(li), High(_begin));
                //}
                ////上升趋势，则以li下沿为基准值
                //else
                //{
                    htmp = DvalueRatio(High(hi), Low(li));
                    ltmp = DvalueRatio(Low(li), Low(li));
                //}

                ///
                ///根据kpscore+1的幅度调整当前框架，保持kpscore和kpscore+1幅度在一定比例内
                ///考虑_nkpsc.end为rt时不调整当前框架ratio
                ///
                    if (_nkpsc != null && _nkpsc.kpframe.Trend == kPieceTrend.Rise)
                    {
                        //ratio = DvalueRatio(High(_begin), Low(_end)) / DvalueRatio(High(_nkpsc.End), Low(_nkpsc.Begin));
                        //ratior= DvalueRatio(_refsi.High(_kpframe.Begin), _refsi.Low(_kpframe.End)) / DvalueRatio(_refsi.High(_nkpsc.kpframe.End), _refsi.Low(_nkpsc.kpframe.Begin));
                        _kpframe.Ratio = DvalueRatio(_refsi.High(_kpframe.Begin), _refsi.Low(_kpframe.End)) / DvalueRatio(_refsi.High(_nkpsc.kpframe.End), _refsi.Low(_nkpsc.kpframe.Begin)) * DvalueRatio(High(_nkpsc.End), Low(_nkpsc.Begin));
                    }
                    else if (_nkpsc != null && _nkpsc.kpframe.Trend == kPieceTrend.Fall)
                    {
                        //ratio = DvalueRatio(High(_end), Low(_begin)) / DvalueRatio(High(_nkpsc.Begin), Low(_nkpsc.End));
                        //ratior = DvalueRatio(_refsi.High(_kpframe.End), _refsi.Low(_kpframe.Begin)) / DvalueRatio(_refsi.High(_nkpsc.kpframe.Begin), _refsi.Low(_nkpsc.kpframe.End));
                        _kpframe.Ratio = (DvalueRatio(_refsi.High(_kpframe.End), _refsi.Low(_kpframe.Begin)) / DvalueRatio(_refsi.High(_nkpsc.kpframe.Begin), _refsi.Low(_nkpsc.kpframe.End))) * DvalueRatio(High(_nkpsc.Begin), Low(_nkpsc.End));
                    }

                //幅度应在框架范围内
                if (
                    (
                   htmp > _kpframe.HighH || htmp < _kpframe.HighL
                   ||
                   ltmp > _kpframe.LowH || ltmp < _kpframe.LowL
                   )
                   )
                {
                    _fit = false;
                    return;
                }

            }

            ////框架kpf下降
            ////距离t较近，e所在kp上升(在需评价的kp之后)，框架kpf下降，kpf.end上升，kp.begin下降，e=kp.begin+1，将e对齐到kp.begin
            //if (
            //    (_t - _kps[ekid].End <= 7)
            //    &&
            //    _kps[ekid].Trend == kPieceTrend.Rise
            //    &&
            //    _refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.End, 0) > 0
            //    &&
            //    KRise(_kps[ekid].Begin, 0) < 0
            //    &&
            //    e == _kps[ekid].Begin + 1
            //    )
            //{
            //    e = _kps[ekid].Begin;
            //}
            //else if(//距离t较近，e所在kp下降(是需评价的kp)，框架kpf下降，kpf.end下降，kp.end上升，e=kp.end - 1，将e对齐到kp.end
            //    (_t - _kps[ekid].End <= 7)
            //    &&
            //    _kps[ekid].Trend == kPieceTrend.Fall
            //    &&
            //    _refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.End, 0) < 0
            //    &&
            //    KRise(_kps[ekid].End, 0) > 0
            //    &&
            //    e == _kps[ekid].End - 1
            //    )
            //{
            //    e = _kps[ekid].End;
            //}
            ////框架kpf上升
            ////距离t较近，e所在kp下降(在需评价的kp之后)，框架kpf上升，kpf.end下降，kp.begin上升，e=kp.begin+1，将e对齐到kp.begin
            //else if (
            //    (_t - _kps[ekid].End <= 7)
            //    &&
            //    _kps[ekid].Trend == kPieceTrend.Fall
            //    &&
            //    _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.End, 0) < 0
            //    &&
            //    KRise(_kps[ekid].Begin, 0) > 0
            //    &&
            //    e == _kps[ekid].Begin + 1
            //    )
            //{
            //    e = _kps[ekid].Begin;
            //}
            //else if (//距离t较近，e所在kp上升(是需评价的kp)，框架kpf上升，kpf.end上升，kp.end下降，e=kp.end - 1，将e对齐到kp.end
            //    (_t - _kps[ekid].End <= 7)
            //    &&
            //    _kps[ekid].Trend == kPieceTrend.Rise
            //    &&
            //    _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.End, 0) > 0
            //    &&
            //    KRise(_kps[ekid].End, 0) < 0
            //    &&
            //    e == _kps[ekid].End - 1
            //    )
            //{
            //    e = _kps[ekid].End;
            //}

            ////框架kpf下降
            //if (//距离t较近，b所在kp上升(在需评价的kp之前)，框架kpf下降，kpf.begin上升，kp.end下降，b=kp.End - 1，将b对齐到kp.End
            //    (_t - _kps[bkid].Begin <= 7 || (tkpid - bkid <= 2 && _t - _kps[bkid].Begin <= 9))
            //    &&
            //    _kps[bkid].Trend == kPieceTrend.Rise
            //    &&
            //    _refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.Begin, 0) > 0
            //    &&
            //    KRise(_kps[bkid].End, 0) < 0
            //    &&
            //    b == _kps[bkid].End - 1
            //    )
            //{
            //    b = _kps[bkid].End;
            //}
            //else if (//距离t较近，b所在kp下降(是需评价的kp)，框架kpf下降，kpf.begin下降，kp.begin上升，b=kp.begin + 1，将b对齐到kp.begin
            //    (_t - _kps[bkid].Begin <= 7 || (tkpid - bkid <= 2 && _t - _kps[bkid].Begin <= 9))
            //    &&
            //    _kps[bkid].Trend == kPieceTrend.Fall
            //    &&
            //    _refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.Begin, 0) < 0
            //    &&
            //    KRise(_kps[bkid].Begin, 0) > 0
            //    &&
            //    b == _kps[bkid].Begin + 1
            //    )
            //{
            //    b = _kps[bkid].Begin;
            //}
            ////框架kpf上升
            //else if (//距离t较近，b所在kp下降(在需评价的kp之前)，框架kpf上升，kpf.begin下降，kp.end上升，b=kp.End - 1，将b对齐到kp.End
            //    (_t - _kps[bkid].Begin <= 7 || (tkpid - bkid <= 2 && _t - _kps[bkid].Begin <= 9))
            //    &&
            //    _kps[bkid].Trend == kPieceTrend.Fall
            //    &&
            //    _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.Begin, 0) < 0
            //    &&
            //    KRise(_kps[bkid].End, 0) > 0
            //    &&
            //    b == _kps[bkid].End - 1
            //    )
            //{
            //    b = _kps[bkid].End;
            //}
            //else if (//距离t较近，b所在kp上升(是需评价的kp)，框架kpf上升，kpf.begin上升，kp.begin下降，b=kp.begin + 1，将b对齐到kp.begin
            //    (_t - _kps[bkid].Begin <= 7 || (tkpid - bkid <= 2 && _t - _kps[bkid].Begin <= 9))
            //    &&
            //    _kps[bkid].Trend == kPieceTrend.Rise
            //    &&
            //    _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End)
            //    &&
            //    _refsi.KRise(_kpframe.Begin, 0) > 0
            //    &&
            //    KRise(_kps[bkid].Begin, 0) < 0
            //    &&
            //    b == _kps[bkid].Begin + 1
            //    )
            //{
            //    b = _kps[bkid].Begin;
            //}



            //if (
            //    (Low(b) > Low(e) && _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End))
            //    ||
            //    (Low(b) < Low(e) && _refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End))
            //    )
            //{
            //    _fit = false;
            //    return;
            //}
            

            //htmp = DvalueRatio(High(hi), Low(li));
            //ltmp = DvalueRatio(Low(li), Low(li));

            
            //int rtkpid = _refsi.kpSet.findKPieceIndex(_rt);

            //待评价kp的上下沿应在框架范围内
            //目前仅评价rkp.begin距离rt较近的框架
            //if (
            //        (
            //            _kpframe.Begin > _rt - 6
            //            ||
            //            _kpframe.Begin == _rt
            //            ||
            //            (//rt所在kp很短且当前kp为rt所在kp-1
            //            _rkps[rtkpid].End - _rkps[rtkpid].Begin <= 2
            //            &&
            //            _rkps[rtkpid].Begin == _kpframe.End
            //            )
            //        )
            //    &&
            //    (
            //    htmp > _kpframe.HighH || htmp < _kpframe.HighL
            //    ||
            //    ltmp > _kpframe.LowH || ltmp < _kpframe.LowL
            //    )
            //    )
            //{
            //    _fit = false;
            //    return;
            //}
            ////rkp.begin距离rt稍远时，仅判断rkp趋势和rkp.begin、rkp.end对应的待评价股票k线柱所表现的趋势是否一致
            //else if(
            //        !(
            //            _kpframe.Begin > _rt - 6
            //            ||
            //            _kpframe.Begin == _rt
            //            ||
            //            (//rt所在kp很短且当前kp为rt所在kp-1
            //            _rkps[rtkpid].End - _rkps[rtkpid].Begin <= 2
            //            &&
            //            _rkps[rtkpid].Begin == _kpframe.End
            //            )
            //        )
            //    )
            //{
            //    if (_refsi.Low(_kpframe.Begin) > _refsi.Low(_kpframe.End) && Low(hi) < Low(li))
            //    {
            //        _fit = false;
            //        return;
            //    }
            //    _kpframe.RatioHH = 1.2f;
            //    _kpframe.RatioHL = 0.5f;
            //    _kpframe.RatioLH = 1.2f;
            //    _kpframe.RatioLL = 0.5f;
            //    if (
            //                        (
            //    htmp > _kpframe.HighH || htmp < _kpframe.HighL
            //    ||
            //    ltmp > _kpframe.LowH || ltmp < _kpframe.LowL
            //    )
            //        )
            //    {
            //        _fit = false;
            //        return;
            //    }
                    
            //}
            
            //除rt所在rkp的且rkp上升的框架外，需根据当前待评价kp的幅度调整rkp-1的框架比率大小，以适应待评价股票整体缩放的情况
            //if (!(_rt == _kpframe.End && _refsi.Low(_kpframe.Begin) < _refsi.Low(_kpframe.End)))
            //{
            //    if(_refsi.kpFrameSet.ContainsKey(_kpfid-1))
            //    {
            //        //比率设置为当前kp幅度与当前框架幅度对比
            //        _refsi.kpFrameSet[_kpfid - 1].Ratio = DvalueRatio(High(hi), Low(li)) / DvalueRatio(rHigh(_kpframe.HighIndex), rLow(_kpframe.LowIndex));
            //    }
            //}
            _fit = true;
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
