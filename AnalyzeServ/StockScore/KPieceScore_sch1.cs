using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class KPieceScore_sch1 : KBase
    {
        private kPieceScoreParameter param;
        
        private kPiece[] _kps;

        private kPiece[] _rkps;

        /// <summary>
        /// 需评分股票k线片段的索引值
        /// </summary>
        private int _kpid;
        /// <summary>
        /// 需评分日的k线数据索引值（处于需评分股票k线片段内）
        /// </summary>
        private int _c;
        /// <summary>
        /// 需评分股票的基准日也是开始日
        /// </summary>
        private int _t;

        private int _baset;
        ///// <summary>
        ///// 与需评分日计算幅度的k线柱上沿最高点之日的k线数据索引值
        ///// </summary>
        //private int _kpsettop;
        ///// <summary>
        ///// 与需评分日计算幅度的k线柱下沿最低点之日的k线数据索引值
        ///// </summary>
        //private int _kpsetbtm;
        /// <summary>
        /// 参照股票k线片段的索引值
        /// </summary>
        private int _rkpid;
        /// <summary>
        /// 参照股票的参照日的k线数据索引值（处于参照股票k线片段内）
        /// </summary>
        private int _rc;
        /// <summary>
        /// 参照股票的基准日也是开始日
        /// </summary>
        private int _rt;

        private int _basert;
        ///// <summary>
        ///// 与参照股票的参照日计算幅度的k线柱上沿最高点之日的k线数据索引值
        ///// </summary>
        //private int _rkpsettop;
        ///// <summary>
        ///// 与参照股票的参照日计算幅度的k线柱下沿最低点之日的k线数据索引值
        ///// </summary>
        //private int _rkpsetbtm;

        private kPiece _kp;

        private kPiece _rkp;

        private KBase _rkb;

        private kPieceTrend _trend;
        //k线片段顶部评分
        private float _topScore;
        //k线片段底部评分
        private float _bottomScore;

        private bool _iskey;
        /// <summary>
        /// 与需评分日计算涨跌幅度的极值k线片段(已计算过的顶或底)所对应的评价对象（此对象已评价）
        /// 与参照股票的参照日计算涨跌幅度也需要该对象
        /// 评分也需要参考此对象的分数
        /// </summary>
        private KPieceScore_sch1 _extrkpscore;
        /// <summary>
        /// amplitude升降幅度评分
        /// </summary>
        private float _ampScore;
        private float _sumScore;
        private float _totalScore;

        public int rkpid
        {
           get
            {
                return _rkpid;
            }
        }

        public int kpid
        {
            get
            {
                return _kpid;
            }
        }

        public int rc
        {
            get
            {
                return _rc;
            }
        }

        public float ampScore
        {
            get
            {
                if (_ampScore<0)
                {
                    //暂时仅评价下降的kp
                    if (_rkb.kpSet.kPieces[_rkpid].Trend == kPieceTrend.Rise)
                    {
                        _ampScore = 0;
                        return _ampScore;
                    }

                    float rl = rLow(_basert);
                    float rh = rLow(_rc);
                    float l = Low(_baset);
                    float h = Low(_c);
                    float tmp = 0;
                    float ramp1 = 0;
                    float amp1 = 0;
                    if (_rkb.kpSet.kPieces[_rkpid].Trend == kPieceTrend.Rise)
                    {
                        rh = rHigh(_rc);
                        h = High(_c);
                    }
                    if(rl>rh)
                    {
                        tmp = rl;
                        rl = rh;
                        rh = tmp;
                        ramp1 = -DvalueRatio(rh, rl);
                    }
                    else
                    {
                        ramp1 = DvalueRatio(rh, rl);
                    }
                    if (l > h)
                    {
                        tmp = l;
                        l = h;
                        h = tmp;
                        amp1 = -DvalueRatio(h, l);
                    }
                    else
                    {
                        amp1 = DvalueRatio(h, l);
                    }
                    //computeAmps(ref ramp1, ref amp1);
                    //_ampScore = computeAmpScore(ramp1, amp1);
                    if(ramp1>=0 && amp1>=0 || (ramp1<=0 || amp1<=0))
                    {
                        tmp = Math.Abs(ramp1 - amp1);
                    }
                    else if((ramp1>0 && amp1<0) ||(ramp1<0 && amp1>0))
                    {
                        tmp = Math.Abs(ramp1) + Math.Abs(amp1);
                    }
                    _ampScore = tmp * 2500;
                }
                return _ampScore;
            }
        }

        public float topScore
        {
            get
            {
                return _topScore;
            }
        }

        public float bottomScore
        {
            get
            {
                return _bottomScore;
            }
        }

        public kPieceTrend Trend
        {
            get
            {
                return _kp.Trend;
            }
        }

        public bool isKey
        {
            get
            {
                return _iskey;
            }
        }
        
        public KPieceScore_sch1()
        {

        }

        /// <summary>
        /// 评价k线片段
        /// </summary>
        /// <param name="kpid">待评价的k线片段索引值</param>
        /// <param name="t">基准日（起始日）</param>
        /// <param name="theDT">待评分股票的数据集合</param>
        /// <param name="rkpid">参照股票的k线片段索引值</param>
        /// <param name="rt">参照股票的基准日</param>
        /// <param name="rkbpub">参照股票的数据集合</param>
        public KPieceScore_sch1(kPieceScoreParameter param, KBase kb, KBase rkb)
            :base(kb)
        {
            _rkb = rkb;
            _t = param.t;
            _baset = param.baset;
            _c = param.c;
            _kpid = param.kpid;
            //_kpsettop = param.kpsettop;
            //_kpsetbtm = param.kpsetbtm;
            _rc = param.rc;
            _rkpid = param.rkpid;
            //_rkpsettop = param.rkpsettop;
            //_rkpsetbtm = param.rkpsetbtm;
            _rt = param.rt;

            _basert = param.basert; 
            _iskey = param.isKey;
            _extrkpscore = param.extrKPScore;

            _kps = kb.kpSet.kPieces;
            _rkps = rkb.kpSet.kPieces;
            _kp = kb.kpSet.kPieces[_kpid];
            _rkp = _rkb.kpSet.kPieces[_rkpid];

            //base.theDT = theDT;
            this._rkb = rkb;
            if (param.extrKPScore != null)
            {
                kPieceScoreParameter extrparam = param.extrKPScore.param;
            }

            _ampScore = -1;

            //float rl = rLow(_rt);
            //float rh = rLow(_rc);
            //float l = Low(_t);
            //float h = Low(_c);
            //if (rkbpub.theDT.kpset.kPieces[_rkpid].Trend == kPieceTrend.Rise)
            //{
            //    rh = rHigh(_rc);
            //    h = High(_c);
            //}
            //_ampScore = computeAmpScore(DvalueRatio(rh, rl), DvalueRatio(h, l));

     
           
        }

        /// <summary>
        /// 对待评价日到指定日（待评价股票）幅度与参照日到指定日（参照股票）幅度进行评价
        /// </summary>
        /// <param name="rextreid">指定日（参照股票）</param>
        /// <param name="extreid">指定日（待评价股票）</param>
        /// <returns></returns>
        public float extreScore(int rextreid,int extreid)
        {

            float score = 0;
            float rl = rLow(rextreid);
            float rh = rHigh(rextreid);
            float l = Low(extreid);
            float h = High(extreid);

            ///
            ///应考虑_c-1是ckp.begin且ckp上升的情况，extreScore的ckp幅度计算可为_c - (_rc - _rkps[rckpid].Begin)到c-1。
            ///
            if(_rkp.Trend== kPieceTrend.Fall)
            {
                rl = rLow(_rc);
                l = Low(_c);
            }
            if(_rkp.Trend== kPieceTrend.Rise)
            {
                rh = rHigh(_rc);
                h = High(_c);
            }
            //if (rkbpub.theDT.kpset.kPieces[_rkpid].Trend == kPieceTrend.Rise)
            //{
            //    rh = rHigh(_rc);
            //    h = High(_c);
            //}
            score = computeAmpScore(DvalueRatio(rh, rl), DvalueRatio(h, l));

            return score;
        }

        public bool compareDaysRF(int rstart, int start)
        {
            bool result = true;
            int len = _rc - rstart + 1;
            for (int i = 0; i < len; i++)
            {
                //暂时仅比较距离rt较近的
                if (rstart + i < _rt - 6)
                {
                    continue;
                }
                if (
                    (KRise(start + i, 0) > 0 && rKRise(rstart + i, 0) < 0)
                    ||
                    (KRise(start + i, 0) < 0 && rKRise(rstart + i, 0) > 0)
                    )
                {
                    return false;
                }
            }
            return result;
        }


        /// <summary>
        /// 调整输入的幅度a、b，以方便进行a、b对比评价。
        /// 调整为a大于b且a、b均大于等于零
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void computeAmps(ref float a, ref float b)
        {
            float tmp = 0;

            if (a == 0)
            {
                a = 0.001f;
            }
            if (b == 0)
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
            if (b < 0.0035f)
            {
                b = 0.0035f;
                //b = 0;
            }
            //if (a < 0.0035f)
            //{
            //    a = 0.0035f;
            //    //a = 0;
            //}
            if (b > a)
            {
                b = a;
            }
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
            ////两日一个上升一个下降，加权数应增大
            //if((a>0 && b<0) || (a < 0 && b > 0))
            //{
            //    if (_c == _t)
            //    {
            //        r = 100;
            //    }
            //    else if (_c == _t - 1)
            //    {
            //        r = 20;
            //    }
            //    else
            //    {
            //        //两者幅度相差越小，权数增加越小
            //        r = (Math.Abs(a) + Math.Abs(b)) * 100;
            //        if(r<1)
            //        {
            //            r = 1;
            //        }
            //    }
            //}

            ////rc、c均上升，且c升幅大于rc时，权数增加（认为c幅度越大对后续上升负面影响越大）
            //if(a>0 && b>0 && b>a)
            //{
            //    r = r * 1.2f;
            //}

            float tmp = 0;
            //x*y*10
            float score = 0f;

            if (a == 0)
            {
                a = 0.001f;
            }
            if (b == 0)
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
            if(b<0.0035f)
            {
                b = 0.0035f;
                //b = 0;
            }
            //if (a < 0.0035f)
            //{
            //    a = 0.0035f;
            //    //a = 0;
            //}
            if(b>a)
            {
                b = a;
            }
            float c = Math.Abs(amp_rc);
            if(c<Math.Abs(amp_c))
            {
                c = Math.Abs(amp_c);
            }
            //score = (float)Math.Pow((a - b) * (a - b) / b, 1f / 2f) * 200 * r;
            //score =(float)Math.Pow((a - b),1f/3f) * ((a - b) / b);
            //score = (float)Math.Pow(c, 1f / 2f) * 300 * (float)Math.Pow((a - b) / b, 1f / 2f);
            score = (float)Math.Pow(c, 0.47f) * 345 * (float)Math.Pow((a - b) / b, 0.78f);
            //a = 0.01f;
            //b = 0.007f;
            //c = 0.01f;
            //float score1 = (float)Math.Pow(c, 1f / 3f) * 150;
            //float score2 = (float)Math.Pow((a - b) / b, 1f / 2f);
            //score = score1 * score2;
            return score;
        }

        private float rHigh(int t)
        {
            return _rkb.High(t);
        }

        private float rLow(int t)
        {
            return _rkb.Low(t);
        }

        private float rKRise(int t, int offset)
        {
            return _rkb.KRise(t, offset);
        }
    }

    public class kPieceScoreParameter
    {

        /// <summary>
        /// 需评分股票k线片段的索引值
        /// </summary>
        public int kpid;
        /// <summary>
        /// 需评分日的k线数据索引值（处于需评分股票k线片段内）
        /// </summary>
        public int c;
        /// <summary>
        /// 需评分股票的基准日也是开始日
        /// </summary>
        public int t;

        public int baset;
        ///// <summary>
        ///// 与需评分日计算幅度的k线柱上沿最高点之日的k线数据索引值
        ///// </summary>
        //public int kpsettop;
        ///// <summary>
        ///// 与需评分日计算幅度的k线柱下沿最低点之日的k线数据索引值
        ///// </summary>
        //public int kpsetbtm;
        /// <summary>
        /// 参照股票k线片段的索引值
        /// </summary>
        public int rkpid;
        /// <summary>
        /// 参照股票的参照日的k线数据索引值（处于参照股票k线片段内）
        /// </summary>
        public int rc;
        /// <summary>
        /// 参照股票的基准日也是开始日
        /// </summary>
        public int rt;

        public int basert;
        ///// <summary>
        ///// 与参照股票的参照日计算幅度的k线柱上沿最高点之日的k线数据索引值
        ///// </summary>
        //public int rkpsettop;
        ///// <summary>
        ///// 与参照股票的参照日计算幅度的k线柱下沿最低点之日的k线数据索引值
        ///// </summary>
        //public int rkpsetbtm;
        /// <summary>
        /// 需评分日或参照股票的参照日是否k线片段的关键值，不是关键值时视为“插值”
        /// </summary>
        public bool interpolation;
        /// <summary>
        /// 需评分的k线片段起始点是k线片段列表中关键转折点
        /// </summary>
        public bool isKey;
        /// <summary>
        /// 与需评分日计算涨跌幅度的极值k线片段(已计算过的顶或底)所对应的评价对象（此对象已评价）
        /// 与参照股票的参照日计算涨跌幅度也需要该对象
        /// 评分也需要参考此对象的分数
        /// </summary>
        public KPieceScore_sch1 extrKPScore;

        public kPieceScoreParameter()
        {
            kpid = -1;
            c = -1;
            t = -1;
            baset = -1;
            rkpid = -1;
            rc = -1;
            rt = -1;
            basert = -1;
            interpolation = false;
            isKey = false;
            extrKPScore = null;

        }
    }
}
