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
    public struct kPiece : ICloneable
    {
        private int _begin;

        private int _end;

        /// <summary>
        /// 与k线片段趋势相符的起点相对于begin的偏移量
        /// 例：k线片段趋势下降，kp.begin上升，与k线片段相符的起始点为kp.begin+1，偏移量为+1
        /// </summary>
        private int _beginto;
        /// <summary>
        /// 与k线片段趋势相符的终点相对于end的偏移量
        /// 例：k线片段趋势下降，kp.end上升，与k线片段相符的起始点为kp.end-1，偏移量为-1
        /// </summary>
        private int _endto;
        /// <summary>
        /// k线片段中k线柱上沿最高那日索引
        /// </summary>
        private int _hightop;

        private int _length;

        /// <summary>
        /// 升降幅
        /// </summary>
        private float _amp;

        /// <summary>
        /// 日均升降幅
        /// </summary>
        private float _avrAmp;

        /// <summary>
        /// k线柱下沿的升降幅
        /// </summary>
        private float _ampbottom;

        private kPieceTrend _trend;

        //private DataTableQ _thedt;
        //private KBase _kbase;
        private KDayDataList kdlist;

        //private KBasePub kbpub;

        public int Begin
        {
            get
            {
                return _begin;
            }
            set
            {
                _begin = value;
                if (_begin < _end)
                {
                    computValuesInPiece();
                }
            }
        }

        public int End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
                if (_end > _begin)
                {
                    computValuesInPiece();
                }
            }
        }
        /// <summary>
        /// 与k线片段趋势相符的起点相对于begin的偏移量
        /// 例：k线片段趋势下降，kp.begin上升，与k线片段相符的起始点为kp.begin+1，偏移量为+1
        /// </summary>
        public int BeginTO
        {
            get
            {
                return _beginto;
            }
        }
        /// <summary>
        /// 与k线片段趋势相符的终点相对于end的偏移量
        /// 例：k线片段趋势下降，kp.end上升，与k线片段相符的起始点为kp.end-1，偏移量为-1
        /// </summary>
        public int EndTO
        {
            get
            {
                return _endto;
            }
        }

        /// <summary>
        /// k线片段中k线柱上沿最高那日索引
        /// </summary>
        public int HighTop
        {
            get
            {
                return _hightop;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        /// <summary>
        /// 升降幅
        /// </summary>
        public float Amp
        {
            get
            {
                return _amp;
            }
        }

        /// <summary>
        /// 日均升降幅
        /// </summary>
        public float avrAmp
        {
            get
            {
                return _avrAmp;
            }
        }

        /// <summary>
        /// k线柱下沿的升降幅
        /// </summary>
        public float AmpBottom
        {
            get
            {
                return _ampbottom;
            }
        }

        public kPieceTrend Trend
        {
            get
            {
                return _trend;
            }
        }

        public kPiece(KDayDataList kdlist)
        {
            _begin = 0;
            _end = 0;
            _beginto = 0;
            _endto = 0;
            _length = 0;
            _hightop = 0;
            _amp = 0;
            _avrAmp = 0;
            _ampbottom = 0;
            //_kbase = kbase;
            this.kdlist = kdlist;

            _trend = kPieceTrend.Null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// 计算k线片段内各属性值
        /// </summary>
        private void computValuesInPiece()
        {
            KBase _kbase = new KBase(kdlist);
            _length = _end - _begin + 1;
            _hightop = _begin;
            if (_kbase.Low(_begin) > _kbase.Low(_end))
            {
                _trend = kPieceTrend.Fall;
            }
            //kPieceSet.genPiece()方法除非在给定的结尾，是不会产生k线片段首尾持平的情况。所以若持平，则遇到结尾，此时也定义为上升
            else
            {
                _trend = kPieceTrend.Rise;
            }
            for (int i = _begin; i <= _end; i++)
            {
                if (_kbase.High(i) >= _kbase.High(_hightop)
                    &&
                    //i为pk的最后一天且上升时不参与最高点统计
                    !(_trend == kPieceTrend.Fall && i == _end && _kbase.KRise(i, 0) > 0)
                    )
                {
                    _hightop = i;
                }
            }
            if (Trend == kPieceTrend.Rise)
            {
                _amp = _kbase.KRiseLH(_begin, _hightop);
                _ampbottom = _kbase.DvalueRatio(_kbase.Low(_begin), _kbase.Low(_end));
            }
            else
            {
                _amp = -_kbase.KRiseLH(_end, _hightop);
                _ampbottom = _kbase.DvalueRatio(_kbase.Low(_end), _kbase.Low(_begin));
            }
            _avrAmp = _amp / (_end - _begin + 1);

            if (_trend == kPieceTrend.Fall)
            {
                if (_kbase.KRise(_begin, 0) > 0)
                {
                    _beginto = 1;
                }
                if (_kbase.KRise(_end, 0) > 0)
                {
                    _endto = -1;
                }
            }

            if (_trend == kPieceTrend.Rise)
            {
                if (_kbase.KRise(_begin, 0) < 0)
                {
                    _beginto = 1;
                }
                if (_kbase.KRise(_end, 0) < 0)
                {
                    _endto = -1;
                }
            }
        }
    }
}
