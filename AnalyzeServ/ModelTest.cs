using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class ModelTest : KBase
    {
        private KBase _rkb;

        private AvgNeighborAmpList _ravrnamplist;

        private kPieceSet _kpset;

        private float _score;

        private int _t;

        //private Hashtable _stock;

        public string _func;
        //待评价日索引
        int _c = 0;
        //参照股票的基准日索引
        int _rt = 0;
        //参照股票的参照日索引
        int _rc = 0;
        int _baset = 0;
        int _basert = 0;
        //参照股票基准日所在的k线片段的索引
        int rtkpid = 0;
        //待评价股票基准日所在的k线片段的索引
        int tkpid = 0;
        kPiece[] _kps;
        //待评价股票的kp数据集合
        kPiece[] _rkps;
        //待评价日所在的k线片段的索引
        private int ckpid = 0;
        //参照股票的参照日所在的k线片段的索引
        private int rckpid = 0;

        /// <summary>
        /// 分析过程当中模型和条件的判定结果
        /// </summary>
        private StringBuilder _recrslt;

        private ReferenceStockInfo _refsi;

        private float _ampscore;

        private float _neibhscore;

        private float _highscore;

        private float _kpampscore;

        private float _extrescore;

        private float _totalscore;

        private bool _available;

        public int[] DisplayDays;

        public float ampScore
        {
            get
            {
                return _ampscore;
            }
        }

        public float neibHScore
        {
            get
            {
                return _neibhscore;
            }
        }

        public float kpAmpScore
        {
            get
            {
                return _kpampscore;
            }
        }

        public float extreScore
        {
            get
            {
                return _extrescore;
            }
        }

        public float totalScore
        {
            get
            {
                return _totalscore;
            }
        }

        /// <summary>
        /// 分析过程当中模型和条件的判定结果
        /// </summary>
        public String RecRslt
        {
            get
            {
                return _recrslt.ToString();
            }
        }

        public bool Available
        {
            get
            {
                return _available;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int t
        {
            set
            {
                _t = value;
                countScore();
            }
            get
            {
                return _t;
            }
        }

        public float Score
        {
            get
            {
                return _score;
            }
        }

        public ModelTest()
        {

        }
        public ModelTest(KBase kb, ReferenceStockInfo refsi)
            :base(kb)
        {
            this._rkb = (KBase)refsi;
            //this._stock = stock;
            this._refsi = refsi;
            this._func = "";
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

        private void countScore()
        {
            DisplayDays = new int[_refsi.KPIdForScore.Length];

            for (int j = 0; j < _refsi.KPIdForScore.Length; j++)
            {
                DisplayDays[j] = _rkb.kpSet.kPieces[_refsi.KPIdForScore[j]].End;
            }
            if (_t - 20 <= 0)
            {
                return;
            }

            //待评价日索引
            _c = 0;
            //参照股票的基准日索引
            _rt = _refsi.t;
            //参照股票的参照日索引
            _rc = 0;
            //参照股票基准日所在的k线片段的索引
            rtkpid = _rkb.kpSet.findKPieceIndex(_rt);
            //待评价股票基准日所在的k线片段的索引
            tkpid = kpSet.findKPieceIndex(_t);
            _kps = kpSet.kPieces;
            //待评价股票的kp数据集合
            _rkps = _rkb.kpSet.kPieces;

            if (Stock["StockCode"].ToString() == "600508")
            {
                int debug798 = 0;
            }

            DayScore[] ds = new DayScore[5];


            for (int i = ds.Length - 1; i >= ds.Length - 5; i--)
            {
                _c = _t - (ds.Length - 1 - i);
                _rc = _rt - (ds.Length - 1 - i);
                rckpid = _rkb.kpSet.findKPieceIndex(_rc);
                ckpid = kpSet.findKPieceIndex(_c);

                ds[i] = new DayScore(_t, _c, _rt, _rc, this, _refsi);
                if (
                    !ds[i].Fit
                    )
                {
                    return;
                }

            }

            if(Stock["StockCode"].ToString()=="600508")
            {
                int debug798 = 0;
            }

            KPieceScore[] ks = new KPieceScore[_refsi.kpFrameSet.Count];

            for (int i = ks.Length - 1; i >= ks.Length - 3; i--)
            {
                if (i == ks.Length - 1)
                {
                    ks[i] = new KPieceScore(_t, _rt, i, null, this, _refsi);
                }
                else
                {
                    ks[i] = new KPieceScore(_t, _rt, i, ks[i + 1], this, _refsi);
                }

                if (!ks[i].Fit)
                {
                    return;
                }
            }


                _available = true;
        }

        ///
        ///countScore for DayScore_ch1
        ///
        //private void countScore()
        //{
        //    DisplayDays = new int[_refsi.KPIdForScore.Length];

        //    for (int j = 0; j < _refsi.KPIdForScore.Length; j++)
        //    {
        //        DisplayDays[j] = _rkb.kpSet.kPieces[_refsi.KPIdForScore[j]].End;
        //    }
        //    if(_t-20<=0)
        //    {
        //        return;
        //    }

        //    //待评价日索引
        //    _c=0;
        //    //参照股票的基准日索引
        //    _rt = _refsi.t;
        //    //参照股票的参照日索引
        //    _rc=0;
        //    //参照股票基准日所在的k线片段的索引
        //    rtkpid = _rkb.kpSet.findKPieceIndex(_rt);
        //    //待评价股票基准日所在的k线片段的索引
        //    tkpid = kpSet.findKPieceIndex(_t);
        //    _kps = kpSet.kPieces;
        //    //待评价股票的kp数据集合
        //    _rkps = _rkb.kpSet.kPieces;
        //    _ravrnamplist = _rkb.avgNAmpList;
        //    _ampscore = -1;
        //    _highscore = -1;
        //    _totalscore = -1;
        //    _kpampscore = -1;
        //    DayScore[] ds = new DayScore[8];
        //    float[] r = new float[8];
        //    float[] neibHr = new float[8];
        //    //return;
        //    //float ravrnamptv = 0.025f;
        //    float ravrnamptv = 0.02f;

        //    for (int i = ds.Length - 1; i >= 0; i--)
        //    {
        //        _c = _t - (ds.Length - 1 - i);
        //        _rc = _rt - (ds.Length - 1 - i);
        //        rckpid = _rkb.kpSet.findKPieceIndex(_rc);
        //        ckpid = kpSet.findKPieceIndex(_c);
        //        //根据rc的日周围平均日k线幅度计算加权数
        //        //超过0.025的直接给予一个很低的加权数，目的为使rc不参加日评价

        //        if (i != ds.Length - 1 && _ravrnamplist[_rc].value >= ravrnamptv)
        //        {
        //            r[i] = 0.0001f;
        //        }
        //        else if (i != ds.Length - 1 && _ravrnamplist[_rc].value < ravrnamptv)
        //        {
        //            //r[i] = 0.01f;
        //            //r[i] = 0.7f + (float)Math.Pow(ravrnamptv - _ravrnamplist[_rc].value, 0.2f) / 1.5f;  //0.025
        //            //r[i] = 0.522f + (float)Math.Pow(ravrnamptv - _ravrnamplist[_rc].value, 0.11f) / 1.3f; //0.02
        //            //if ((rKRise(_rc, 0) > 0 && KRise(_c, 0) < 0) || (rKRise(_rc, 0) < 0 && KRise(_c, 0) > 0))
        //            //{
        //            //    r[i] = r[i] * 0.32f;
        //            //}
        //            //else
        //            //{
        //            //    r[i] = r[i] * 0.52f;
        //            //}
        //            r[i] = 1.46f;
        //        }
        //        else
        //        {
        //            r[i] = 1f;
        //        }
        //        //使用参照股票rkp.avrAmp为依据可以根据参照股票的幅度调整评价分数，使用待评价股票的则根据待评价股票的幅度调整评价分数
        //        //以参照股票为依据则最终结果更接近参照股票的形态
        //        r[i] = r[i] * (1 / (1 + (float)Math.Pow(Math.Abs(_rkps[rckpid].avrAmp) * 10f, 1.4f) * 2.5f));
        //        //rc为rkp.end时，引入rkp+1的avrAmp计算r
        //        if (_rc == _rkps[rckpid].End && rckpid + 1 <= _rkps.Length - 1)
        //        {
        //            //r[i] = 1/(1+(Math.Abs(_rkps[rckpid].avrAmp) + Math.Abs(_rkps[rckpid + 1].avrAmp))/2 * 10f * 1.25f);
        //        }

        //        if (
        //           ( (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End && rKRise(_rc, 0) >= 0)
        //            ||
        //            (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End - 1 && rKRise(_rc + 1, 0) < 0))
        //            &&
        //            _c+1<=_t
        //            )
        //        {
        //            if(High(_c)>High(_c+1))
        //            {
        //                neibHr[i] = 0.66f;
        //            }
        //            else
        //            {
        //                neibHr[i] = 1f;
        //            }
        //        }


        //        ds[i] = new DayScore(_c, t, this, _rc, _rt, _rkb);


        //        if(_rt-_rc<=5 && ((KRise(_c, 0) > 0 && rKRise(_rc, 0) < 0) || (KRise(_c, 0) < 0 && rKRise(_rc, 0) > 0)))
        //        {
        //            _ampscore = ds[i].ampScore / r[i] * 10;
        //            _highscore = ds[i].highScore * 10;
        //            _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].highScore * 10;
        //            //return;
        //        }
        //        ///
        //        ///改进，t日评分应与t日之前几日平均幅度等有关，前几日幅度大，对t日的要求低 000935 20150803 与 600350 20150803
        //        ///
        //        if (i == ds.Length - 1 && KRise(_c, 0) <= rKRise(_rc, 0) && (ds[i].ampScore * r[i] > 35))   //40 //37
        //        {
        //            _ampscore = ds[i].ampScore / r[i] * 10;
        //            _neibhscore = ds[i].neibHScore / r[i] * 10;
        //            _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //            return;
        //        }
        //        if (i == ds.Length - 1 && KRise(_c, 0) > rKRise(_rc, 0) && (ds[i].ampScore * r[i] > 35))   //40
        //        {
        //            _ampscore = ds[i].ampScore / r[i] * 10;
        //            _neibhscore = ds[i].neibHScore / r[i] * 10;
        //            _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //            return;
        //        }
        //        if (i == ds.Length - 2
        //            &&
        //                (
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End && rKRise(_rc, 0) >= 0)
        //                    ||
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End - 1 && rKRise(_rc + 1, 0) < 0)
        //                    ||
        //                    _ravrnamplist[_rc].value < ravrnamptv
        //                )
        //            )
        //        {
        //            if (
        //                (ds[i].ampScore * r[i] > 35)
        //                ||
        //                (ds[i].neibAmpScore * r[i] > 38)
        //                )
        //            {
        //                _ampscore = ds[i].ampScore / r[i] * 10;
        //                _neibhscore = ds[i].neibHScore / r[i] * 10;
        //                _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //                return;
        //            }
        //        }
        //        if (i == ds.Length - 3
        //            &&
        //                (
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc==_rkps[rckpid].End && rKRise(_rc, 0)>=0) 
        //                    ||
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End - 1 && rKRise(_rc + 1, 0) < 0)
        //                    ||
        //                    _ravrnamplist[_rc].value < ravrnamptv
        //                )
        //            )
        //        {
        //            if (
        //                (ds[i].ampScore * r[i] > 35)
        //                ||
        //                (ds[i].neibAmpScore * r[i] > 38)
        //                ||
        //                (ds[i].neibHScore * neibHr[i] > 44)
        //                )
        //            {
        //                _ampscore = ds[i].ampScore / r[i] * 10;
        //                _neibhscore = ds[i].neibHScore / r[i] * 10;
        //                _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //                return;
        //            }
        //        }
        //        if (i == ds.Length - 4
        //            &&
        //                (
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End && rKRise(_rc, 0) >= 0)
        //                    ||
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End - 1 && rKRise(_rc + 1, 0) < 0)
        //                    ||
        //                    _ravrnamplist[_rc].value < ravrnamptv
        //                )
        //            )
        //        {
        //            if (
        //                (ds[i].ampScore * r[i] > 35)
        //                ||
        //                (ds[i].neibAmpScore * r[i] > 38)
        //                ||
        //                (ds[i].neibHScore * neibHr[i] > 44)
        //                )
        //            {
        //                _ampscore = ds[i].ampScore / r[i] * 10;
        //                _neibhscore = ds[i].neibHScore / r[i] * 10;
        //                _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //                return;
        //            }
        //        }
        //        if (i == ds.Length - 5
        //            &&
        //                (
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End && rKRise(_rc, 0) >= 0)
        //                    ||
        //                    (_rkps[rckpid].Trend == kPieceTrend.Rise && _rc == _rkps[rckpid].End - 1 && rKRise(_rc + 1, 0) < 0)
        //                    ||
        //                    _ravrnamplist[_rc].value < ravrnamptv
        //                )
        //            )
        //        {
        //            if (
        //                (ds[i].ampScore * r[i] > 35) //
        //                ||
        //                (ds[i].neibAmpScore * r[i] > 38)
        //                ||
        //                (ds[i].neibHScore * neibHr[i] > 44)
        //                )
        //            {
        //                _ampscore = ds[i].ampScore / r[i] * 10;
        //                _neibhscore = ds[i].neibHScore / r[i] * 10;
        //                _totalscore = ds[i].ampScore / r[i] * 10 + ds[i].neibHScore * 10;
        //                return;
        //            }
        //        }

        //        if (_ampscore<0)
        //        {
        //            _ampscore = 0;
        //            _highscore = 0;
        //            _totalscore = 0;
        //        }
        //        _ampscore += ds[i].ampScore * r[i];
        //        _highscore += ds[i].highScore;
        //        //_totalscore += ds[i].totalScore * r[i];
        //        _totalscore += ds[i].ampScore * r[i] + ds[i].highScore;
        //    }
        //    _ampscore = _ampscore / 8;
        //    _highscore = _highscore / 8;
        //    _totalscore = _totalscore / 8;



        //    _c = 0;
        //    _rc = 0;

        //    //参照股票在计算过程中k线片段的低点
        //    int lid = tkpid;
        //    int rlid=rtkpid;
        //    //参照股票在计算过程中k线片段的高点
        //    int hid = tkpid;
        //    int rhid=rtkpid;
        //    //待评价日所在的k线片段的索引
        //    ckpid=0;
        //    //参照股票的参照日所在的k线片段的索引
        //    rckpid=0;
        //    //累计需评价的k线片段数量
        //    int count = 0;
        //    float extrescoretmp = 0;
        //    float ampscoretmp = 0;
        //    //低点kp评价对象
        //    KPieceScore lscore=new KPieceScore();
        //    //高点kp评价对象
        //    KPieceScore hscore = new KPieceScore();
        //    //评价对象数组
        //    KPieceScore[] scores = new KPieceScore[_refsi.KPIdForScore.Length];
        //    r = new float[_refsi.KPIdForScore.Length];
        //    _baset = _t;
        //    _basert = _rt;
        //    if (_t <= _kps[tkpid].Begin + 2 && _kps[tkpid].Trend == kPieceTrend.Rise)
        //    {
        //        _baset = _kps[tkpid].Begin;
        //    }
        //    if (_rt <= _rkps[rtkpid].Begin + 2 && _rkps[rtkpid].Trend == kPieceTrend.Rise)
        //    {
        //        _basert = _rkps[rtkpid].Begin;
        //    }

        //    //参照股票信息
        //    //refsi = new ReferenceStockInfo(_rt, rkbpub.theDT);
        //    for (int j = _refsi.KPIdForScore.Length - 1; j >= 0; j--)
        //    {
        //        ampscoretmp = 0;
        //        if(_kpampscore<0)
        //        {
        //            _kpampscore = 0;
        //        }
        //        if (_extrescore < 0)
        //        {
        //            _extrescore = 0;
        //        }
        //        rckpid=_refsi.KPIdForScore[j];
        //        //由于kpieceSet.findKPieceIndex方法的逻辑是begin<kid<=end，所以使用end做分析
        //        _rc=_rkps[rckpid].End;
        //        if(_rc>_rt)
        //        {
        //            _rc = _rt;
        //        }
        //        _c=_t-(_rt-_rc);
        //        if(_rc>_basert)
        //        {
        //            _rc = _basert;
        //        }
        //        if (_c > _baset)
        //        {
        //            _c = _baset;
        //        }
        //        ckpid = kpSet.findKPieceIndex(_c);

        //        r[j] = (1 / (1 + (float)Math.Pow(Math.Abs(_rkps[rckpid].avrAmp)*10f, 1.4f) * 2.5f));
        //        //r[j] = (1 / (1 + (float)Math.Pow(0.018 * 10f, 1.4f) * 2f));
        //        //目前先处理距离rt较近的需评价的k线片段
        //        if (
        //            //(_rkps[rckpid].Begin <= _rt - 6 && j < _refsi.KPIdForScore.Length - 2)
        //            //||
        //            (_rc <= _rt - 7)
        //            )
        //        {
        //            break;
        //        }
        //        //为与高低点比较预留
        //        //更新低值
        //        if (rLow(_rc) < rLow(_rkps[rlid].End))
        //        {
        //            rlid = rckpid;
        //            lid = ckpid;
        //        }
        //        //更新高值
        //        if (rHigh(_rc) > rHigh(_rkps[rhid].End))
        //        {
        //            rhid = rckpid;
        //            hid = ckpid;
        //        }

        //        //scores[j] = new KPieceScore(newParam(ckpid, _c, _t, rckpid, _rc, _rt, false, null), this, _rkb);
        //        scores[j] = new KPieceScore(newParam(ckpid, _c, _baset, _t, rckpid, _rc, _basert, _rt, false, null), this, _rkb);
        //        if (_rc > _basert)
        //        {
        //            continue;
        //        }
        //        //当前rkp.begin在距离rt一定距离内时，检查需评价rkp幅度与ckp幅度的差异
        //        //if (_rkps[rckpid].Begin >= _rt - 5)
        //        if (_rkps[rckpid].End >= _rt - 6)
        //        {
        //            if (!scores[j].compareDaysRF(_rkps[rckpid].Begin, _c - (_rc - _rkps[rckpid].Begin)))
        //            {
        //                return;
        //            }

        //            ///
        //            ///应考虑_c-1是ckp.begin且ckp上升的情况，extreScore的ckp幅度计算可为_c - (_rc - _rkps[rckpid].Begin)到c-1。
        //            ///

        //            //用rkp.begin计算到rc的幅度，即相当于rkp幅度，相应的应将c-(rc-rkp.begin)作为起始计算到c的幅度
        //            extrescoretmp = scores[j].extreScore(_rkps[rckpid].Begin, _c - (_rc - _rkps[rckpid].Begin));
        //            //rt为rkp.begin+1，适当减少评分
        //            if (_rkps[rckpid].Begin == _rt - 1)
        //            {
        //                extrescoretmp = extrescoretmp * 0.8f;
        //            }//待评价幅度大于参照幅度，评分可以适当减少
        //            else if (ampt01)
        //            {
        //                extrescoretmp = extrescoretmp * 0.7f;
        //            }
        //            else
        //            {
        //                extrescoretmp = extrescoretmp * 1.2f;
        //            }
        //            if (_rt - _rkps[rckpid].Begin >= 1)
        //            {
        //                if (j == _refsi.KPIdForScore.Length - 1)
        //                {
        //                    if (extrescoretmp * r[j] > 40)//59
        //                    {
        //                        return;
        //                    }
        //                }
        //                if (j == _refsi.KPIdForScore.Length - 2)
        //                {
        //                    if (extrescoretmp * r[j] > 40)
        //                    {
        //                        return;
        //                    }
        //                }
        //                //if (j == _refsi.KPIdForScore.Length - 3)
        //                //{
        //                //    if (extrescoretmp * r[j] > 40)
        //                //    {
        //                //        return;
        //                //    }
        //                //}
        //            }
        //            _extrescore += extrescoretmp; 
        //        }
                
        //        if (_rc > _rt - 2)
        //        {
        //            continue;
        //        }

        //        //评价rc到rt幅度与c到t幅度的差异
        //        //ckp下降c低于t，且rckp下降rc低于（或接近）rt时，评分应适当减少（经验中底部稍微低一些影响不大，但高一些影响相对更大）
        //        if (ampt02)
        //        {
        //            ampscoretmp = scores[j].ampScore * 1.0f;
        //            _kpampscore += ampscoretmp;
        //            //if (ampscoretmp > 70)
        //            //{
        //            //    //return;
        //            //}
        //        }
        //        //ckp下降c低于t，且rckp下降rc高于rt时，评分应适当增加
        //        else if (ampt03)
        //        {
        //            ampscoretmp = scores[j].ampScore;
        //            _kpampscore += ampscoretmp;
        //        }
        //        //ckp下降c高于t，且rckp下降rc低于rt时，评分应适当增加
        //        else if (ampt04)
        //        {
        //            ampscoretmp = scores[j].ampScore * 1.0f;
        //            _kpampscore += ampscoretmp;
        //        }
        //        else
        //        {
        //            ampscoretmp = scores[j].ampScore;
        //            _kpampscore += ampscoretmp;
        //        }

        //        if (j == _refsi.KPIdForScore.Length - 1)
        //        {
        //            if (ampscoretmp > 50)//50
        //            {
        //                return;
        //            }
        //        }
        //        if (j == _refsi.KPIdForScore.Length - 2)
        //        {
        //            if (ampscoretmp >50)//50
        //            {
        //                return;
        //            }
        //        }
        //        if (j == _refsi.KPIdForScore.Length - 3)
        //        {
        //            if (ampscoretmp > 50) //50
        //            {
        //                return;
        //            }
        //        }
        //        if (j == _refsi.KPIdForScore.Length - 4)
        //        {
        //            if (ampscoretmp > 50) //50
        //            {
        //                return;
        //            }
        //        }
        //        count++;
        //    }
        //    _kpampscore = kpAmpScore / count;

        //    //for (int j = scores.Length - 1; j >= 0; j--)
        //    //{
        //    //    if(_extrescore<0)
        //    //    {
        //    //        _extrescore = 0;
        //    //    }
        //    //    if (scores[j]!=null && scores[j].rkpid == rlid)
        //    //    {
        //    //        _extrescore += scores[j].extreScore(_rkps[rhid].End, _kps[hid].End);
        //    //        if (_rkps[rtkpid].Trend == kPieceTrend.Rise && scores[j].rc != _rt)
        //    //        {
        //    //            _extrescore += scores[j].extreScore(_rt, _t);
        //    //        }
        //    //    }
        //    //    if (scores[j] != null && scores[j].rkpid == rhid)
        //    //    {
        //    //        if (_rkps[rtkpid].Trend == kPieceTrend.Fall && scores[j].rc != _rt)
        //    //        {
        //    //            _extrescore += scores[j].extreScore(_rt, _t);
        //    //        }
        //    //    }
                
        //    //}

        //    _available = true;
        //}

        
        /// <summary>
        /// 待评价k线幅度是否大于参照幅度
        /// </summary>
        private bool ampt01
        {
            get
            {
                if (
                        (
                            _rkps[rckpid].Trend == kPieceTrend.Fall
                            &&
                            DvalueRatio(rHigh(_rkps[rckpid].Begin), rLow(_rkps[rckpid].End)) < DvalueRatio(High(_c - (_rkps[rckpid].End - _rkps[rckpid].Begin)), Low(_c))
                        )
                        ||
                        (
                            _rkps[rckpid].Trend == kPieceTrend.Rise
                            &&
                            DvalueRatio(rHigh(_rkps[rckpid].End), rLow(_rkps[rckpid].Begin)) < DvalueRatio(High(_c), Low(_c - (_rkps[rckpid].End - _rkps[rckpid].Begin)))
                        )
                    )
                {
                    return true;
                }
                return false;
            }

        }

        /// <summary>
        /// ckp下降c.low相对rc.low更低
        /// </summary>
        private bool ampt02
        {
            get
            {
                if (_rkps[rckpid].Trend == kPieceTrend.Fall
                    &&
                    ((Low(_baset) >= Low(_c) && rLow(_basert) >= rLow(_rc)) || (Low(_baset) <= Low(_c) && rLow(_basert) <= rLow(_rc)))
                    &&
                    //t高于c、rt高于rc时，c.low相对更低于rc.low；t低于c、rt低于tc时，c.low相对更低于rc.low
                    //即c.low相对rc.low更低
                    DvalueRatio(Low(_baset), Low(_c)) > DvalueRatio(rLow(_basert), rLow(_rc)))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// ckp下降c低于t，且rckp下降rc高于rt
        /// </summary>
        private bool ampt03
        {
            get
            {
                if (_rkps[rckpid].Trend == kPieceTrend.Fall
                    &&
                    ((Low(_baset) > Low(_c) && rLow(_basert) < rLow(_rc)))
                    )
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// ckp下降c高于t，且rckp下降rc低于rt
        /// </summary>
        private bool ampt04
        {
            get
            {
                if (_rkps[rckpid].Trend == kPieceTrend.Fall
                    &&
                    ((Low(_baset) < Low(_c) && rLow(_basert) > rLow(_rc)))
                    )
                {
                    return true;
                }
                return false;
            }
        }

        private kPieceScoreParameter newParam(int kpid,int c,int baset,int t,int rkpid,int rc,int basert,int rt,bool iskey, KPieceScore_sch1 extrkpscore)
        {
            kPieceScoreParameter param = new kPieceScoreParameter();
            param.kpid = kpid;
            param.c = c;
            param.t = t;
            param.baset = baset;
            param.rkpid = rkpid;
            param.rc = rc;
            param.rt = rt;
            param.basert = basert;
            param.isKey = iskey;
            param.extrKPScore = extrkpscore;

            param.interpolation = false;

            return param;

        }
    }
}
