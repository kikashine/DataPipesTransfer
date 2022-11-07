using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public partial class FormKLineGraphic : Form
    {
        //protected delegate void DrawReversibleLineInvoke(List<Rectangle> rects);

        protected delegate void DrawReversibleLineInvoke(Point point);

        protected delegate Point PointToScreenInvoke(Point point);

        public bool activated;

        public Form mainform;

        public TabPageForList tabpage;
        /// <summary>
        /// 窗体是否可以开始移动的标志（MouseDown时设置为true）
        /// </summary>
        private bool _canMoveForm;

        private bool _canMoveKlbl;
        /// <summary>
        /// 按下鼠标键时的x位置
        /// </summary>
        private int _mouseLocationX;

        /// <summary>
        /// 按下鼠标键时的y位置
        /// </summary>
        private int _mouseLocationY;

        /// <summary>
        /// 作为描绘K线图参照点的那天在k线数据集合中的索引值
        /// 通常为分析日
        /// 以该天向前若干天和向后若干天为绘图范围
        /// </summary>
        private int PDayIndex;
        /// <summary>
        /// 分析日在分析用k线数据集合与显示用k线数据集合中索引值之差
        /// 因为显示时取得k线数据可能已经更新，数据集长度可能改变
        /// </summary>
        private int ADayIdxOfst;
        /// <summary>
        /// Analyze分析后得到的一条绘图用数据
        /// </summary>
        public AnalyzeResultForDraw arfd;
        /// <summary>
        /// 记录已取得数据的当前绘制的AnalyzeResultForDraw对应的id，避免重复取数据
        /// </summary>
        private long AnalyzeResultForDrawId;

        private KBase kbase;

        //private DBWorker DBW;

        /// <summary>
        /// 绘制所有内容的缓存画板
        /// </summary>
        private DrawGraphics dg = new DrawGraphics(762, 683);
        //private  DrawGraphics dg = new DrawGraphics(762, 683);
        /// <summary>
        /// 本次绘图的数据在绘图数据列表中的索引值
        /// </summary>
        public int indexOfDataForDrawKLine;
        /// <summary>
        /// 绘图开始日对应的k线数据索引值
        /// </summary>
        private int indexb;
        /// <summary>
        /// 绘图结束日对应的k线数据索引值
        /// </summary>
        private int indexe;

        ///// <summary>
        ///// MA4数据集合
        ///// </summary>
        //public THashTable<float> ma4;
        ////
        ///// <summary>
        ///// MA8数据集合
        ///// </summary>
        //public THashTable<float> ma8;
        ////
        ///// <summary>
        ///// MA12数据集合
        ///// </summary>
        //public THashTable<float> ma12;

        /// <summary>
        /// MA4数据集合
        /// </summary>
        private float[] ma4;
        //
        /// <summary>
        /// MA8数据集合
        /// </summary>
        private float[] ma8;
        //
        /// <summary>
        /// MA12数据集合
        /// </summary>
        private float[] ma12;

        private float[] ma24;

        ///// <summary>
        ///// 历次MA趋势判断情况的记录
        ///// </summary>
        //public List<MATrendHistory> mtHistory;
        ///// <summary>
        ///// 
        ///// </summary>
        //public List<WaveLetKeyDays> wlKeyDays;

        /// <summary>
        /// 需要绘制k线柱背景的各日期及对应类型
        /// </summary>
        private THashTable<DayTypesOfADay> DaysForDrawBack;
        /// <summary>
        /// 记录各需要标记的天对应的类型
        /// </summary>
        private DayTypesOfADay[] DType;
        /// <summary>
        /// 开盘价的原始坐标值（相对indexe收盘价(设为纵坐标0点)的纵坐标值）
        /// </summary>
        private int[] openYs;
        /// <summary>
        /// 收盘价的原始坐标值（相对indexe收盘价(设为纵坐标0点)的纵坐标值）
        /// </summary>
        private int[] closeYs;
        /// <summary>
        /// 最高价的原始坐标值（相对indexe收盘价(设为纵坐标0点)的纵坐标值）
        /// </summary>
        private int[] highestYs;
        /// <summary>
        /// 最低价的原始坐标值（相对indexe收盘价(设为纵坐标0点)的纵坐标值）
        /// </summary>
        private int[] lowestYs;
        /// <summary>
        /// 坐标经过纵向调整后，纵坐标0点相对原始坐标0点(indexe收盘价)的坐标偏移量
        /// </summary>
        private int baselineY;
        /// <summary>
        /// 绘制对应基准价格0%~100%范围的k线值需要的纵坐标像素数
        /// 注：k线图绘图区的纵坐标像素数可以绘制基准价格0%~80%范围(DrawGraphic类预设值)的k线值
        /// </summary>
        private int pix100per;
        /// <summary>
        /// 绘制对应最高交易量的k线值需要的纵坐标像素数
        /// </summary>
        private int TopVolHeight;
        /// <summary>
        /// 最高交易量在k线数据中的索引值
        /// </summary>
        private int TopVoli;
        /// <summary>
        /// 绘图区域左上角点在窗体中的纵坐标值
        /// </summary>
        private int DrawTop = 40;
        /// <summary>
        /// 绘图区域左上角点在窗体中的横坐标值
        /// </summary>
        private int DrawLeft = 2;
        /// <summary>
        /// 绘图区域的宽
        /// </summary>
        private int DrawWidth = 762;
        /// <summary>
        /// 绘图区域的高
        /// </summary>
        private int DrawHeight = 683;
        /// <summary>
        /// 可绘制k线柱的个数
        /// </summary>
        private int KCountToDraw;
        /// <summary>
        /// 实际绘制k线柱个数
        /// </summary>
        private int currentKCount;
        /// <summary>
        /// 最左侧k线柱左上角在dg画布坐标系中鼠标跟随线绘制区域中x坐标值
        /// </summary>
        private int LKBarLeft;
        /// <summary>
        /// 最右侧k线柱右上角在dg画布坐标系中鼠标跟随线绘制区域中x坐标值
        /// </summary>
        private int RKBarRight;

        //private Point lastMousePoit = new Point(-1, -1);
        /// <summary>
        /// 标记正在绘制鼠标跟随线
        /// </summary>
        private bool drawingCline = false;

        public Hashtable patternMark;

        private int tmpcount = 0;

        private bool moving = false;

        /// <summary>
        /// 绘制反色线所需Graphics
        /// </summary>
        Graphics grfx;
        /// <summary>
        /// 绘制反色线所需窗体句柄
        /// </summary>
        System.IntPtr hdc;

        /// <summary>
        /// 上次绘制的反色线所依据的rectangle集合
        /// </summary>
        private List<Rectangle> lastrects = new List<Rectangle>();

         [DllImport("gdi32.dll")]  
        
        private static extern bool BitBlt(  
            IntPtr hdcDest, //目标设备的句柄  
            int nXDest,     // 目标对象的左上角的X坐标  
            int nYDest,     // 目标对象的左上角的Y坐标  
            int nWidth,     // 目标对象的矩形的宽度  
            int nHeight,    // 目标对象的矩形的长度  
            IntPtr hdcSrc,  // 源设备的句柄  
            int nXSrc,      // 源对象的左上角的X坐标  
            int nYSrc,      // 源对象的左上角的Y坐标  
            int dwRop       // 光栅的操作值  
            );  
        public const int SRCCOPY = 0xCC0020;  
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]  
        public static extern IntPtr CreateCompatibleDC(IntPtr hdcPtr);  
        [DllImport("gdi32.dll", ExactSpelling = true)]  
        public static extern IntPtr SelectObject(IntPtr hdcPtr, IntPtr hObject);  
        [DllImport("gdi32.dll", ExactSpelling = true)]  
        public static extern bool DeleteDC(IntPtr hdcPtr);  
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]  
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("Gdi32.dll")]
        static extern IntPtr CreatePen(int fnPenStyle, int width, int color);
        [DllImport("Gdi32.dll")]
        //用于设定当前前景色的混合模式
        //函数调用成功后返回调用前的模式，调用失败则返回零
        static extern int SetROP2(System.IntPtr hdc, int rop);
        [DllImport("Gdi32.dll")]
        //将当前绘图位置移动到某个具体的点，同时也可获得之前位置的坐标
        //LPPOINT lpPoint：传出参数：一个指向POINT结构的指针，用来存放上一个点的位置，若此参数为NULL，则不保存上一个点的位置
        static extern int MoveToEx(IntPtr hdc, int x, int y, IntPtr lppoint);
        [DllImport("Gdi32.dll")]
        //用当前画笔画一条线，从当前位置连到一个指定的点。这个函数调用完毕，当前位置变成x,y。
        static extern int LineTo(IntPtr hdc, int X, int Y);


        /// <summary>
        /// 绘制反色线所需函数，根据背景色得到绘制颜色模式nDrawMode
        /// 移植自ControlPaint对象
        /// </summary>
        /// 

        private int GetColorRop(Color color, int darkROP, int lightROP)
        {
            if (color.GetBrightness() < 0.5)
            {
                return darkROP;
            }
            return lightROP;
        }

        /// <summary>
        /// 绘制反色线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="backColor"></param>
        public void DrawReversibleLine(Point[] starts, Point[] ends, Color backColor)
        {
            //Graphics grfx = base.CreateGraphics();
            ////可以尝试hdc在窗体初始化的时候创建一次，窗体退出时销毁，有利于提高效率
            //System.IntPtr hdc = grfx.GetHdc();
            //根据背景色得到绘制颜色模式nDrawMode
            int nDrawMode = GetColorRop(backColor, 10, 7);
            System.IntPtr hpen = CreatePen(0, 1, System.Drawing.ColorTranslator.ToWin32(backColor));
            //设定当前前景色的混合模式
            int rop = SetROP2(hdc, nDrawMode);
            IntPtr oldpen = SelectObject(hdc, hpen);
            //可能需要所有lineto先绘制到内存中，再BitBlt到hdc提高速度
            //也可能通过设置hdc的无效区域，仅绘制无效区域可以提高速度
            //参考http://bbs.csdn.net/topics/330039279#post-330682029
            //或http://zhidao.baidu.com/link?url=Kt4rhHt6zxAtKlRqu-X3ul6Rw7ONOxRj5FuhLoWuSysdAE-H1SuY4Dxapj2Rw_xVGx7zxIxc37BKSyai6IoOAqhwU9BzHRFHHnsEC7Uu7mq
            for (int i = 0; i <= starts.Length - 1;i++ )
            {
                //将当前绘图位置移动到具体的点
                MoveToEx(hdc, starts[i].X, starts[i].Y, IntPtr.Zero);
                //用当前画笔画一条线，从当前位置连到指定的点
                LineTo(hdc, ends[i].X, ends[i].Y);
            }
            SelectObject(hdc, oldpen);
            SetROP2(hdc, rop);
            DeleteObject(hpen);
            DeleteObject(oldpen);
        
        }

        public FormKLineGraphic()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);
            _canMoveForm = false;
            _canMoveKlbl = false;
            indexOfDataForDrawKLine = -1;
            indexb = 0;
            indexe = 0;

          
            //DrawTop = 40;
            //DrawLeft = 2;
            //DrawHeight = 683;
            //DrawWidth = 762;
            
            //dg = new DrawGraphics(760, 729);
            dg = new DrawGraphics(this.Width, 729);
            //DBW = new DBWorker();
            AnalyzeResultForDrawId = 0;
            //dg = new DrawGraphics(DrawWidth, DrawHeight);
            this.lblkinfo.Left = DrawLeft + dg.KRect.Left - 1;
            this.lblkinfo.Top = DrawTop + dg.KRect.Top - 1;
            this.lblvinfo.Left = DrawLeft + dg.VInfoRect.Left;
            this.lblvinfo.Top = DrawTop + dg.VInfoRect.Top + 1;
            this.lblvinfo.Width = dg.VInfoRect.Width;
            this.lblvinfo.Height = dg.VInfoRect.Height;
            this.lblvinfo.Text = "";
            KCountToDraw = dg.KCountToDraw;
            patternMark = new Hashtable();
            //因为鼠标跟随线的绘制使用ControlPaint.DrawReversibleLine方法，需要以绘图Form为参照，映射描绘到屏幕的坐标，所以需要从DrawLeft，DrawTop开始计算边缘
            //CrossLineRectangle = new Rectangle(DrawLeft + dg.KRect.Left, DrawTop + dg.KRect.Top, dg.KRect.Width, dg.KRect.Height + dg.VRect.Height + 1);
        }

        public KBase kBase
        {
            set
            {
                this.kbase = value;
                dg.init(value);
            }
            get
            {
                return kbase;
            }
        }

        //public DataTableQ theDT
        //{
        //    set
        //    {
        //        kbase = new KBasePub(ref value);
        //        dg.theDT = kbase.theDT;
        //     }
        //    get
        //    {
        //        return kbase.theDT;
        //    }
        //}

        private void lblClose_Click(object sender, EventArgs e)
        {
            //CLinelastPoint = new Point(-1, -1);
            this.Hide();
        }

        private void lblMove_MouseDown(object sender, MouseEventArgs e)
        {
            _canMoveForm = true;
            _mouseLocationX = e.X;
            _mouseLocationY = e.Y;
        }

        private void lblMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (_canMoveForm)
            {
                Point p = new Point(this.Location.X + e.X - _mouseLocationX, this.Location.Y + e.Y - _mouseLocationY);
                //Point p = new Point(this.Location.X - e.X, this.Location.Y - e.Y);
                this.Location = p;
            }
        }

        private void lblMove_MouseUp(object sender, MouseEventArgs e)
        {
            _canMoveForm = false;
        }

        private void lblkinfo_MouseDown(object sender, MouseEventArgs e)
        {
            _canMoveKlbl = true;
            _mouseLocationX = e.X;
            _mouseLocationY = e.Y;
        }

        private void lblkinfo_MouseMove(object sender, MouseEventArgs e)
        {

            //在边界时的处理
            if (_canMoveKlbl
                &&
                (
                    (this.lblkinfo.Top + this.lblkinfo.Height <= 5 && _mouseLocationY > e.Y)
                    ||
                    (this.lblkinfo.Left + this.lblkinfo.Width <= 5 && _mouseLocationX > e.X)
                    ||
                    (dg.LocalBitmap.Height + DrawTop - this.lblkinfo.Top <= 5 && _mouseLocationY < e.Y)
                    ||
                    (dg.LocalBitmap.Width + DrawLeft - this.lblkinfo.Left <= 5 && _mouseLocationX < e.X)
                )

                )
            {
                _canMoveKlbl = false;
                if (this.lblkinfo.Top + this.lblkinfo.Height <= 5)
                {
                    this.lblkinfo.Top = 6 - this.lblkinfo.Height;

                }
                if (dg.LocalBitmap.Height + DrawTop - this.lblkinfo.Top <= 5)
                {
                    this.lblkinfo.Top = dg.LocalBitmap.Height + DrawTop - 6;

                }
                if (this.lblkinfo.Left + this.lblkinfo.Width <= 5)
                {
                    this.lblkinfo.Left = 6 - this.lblkinfo.Width;

                }
                if (dg.LocalBitmap.Width + DrawLeft - this.lblkinfo.Left <= 5)
                {
                    this.lblkinfo.Left = dg.LocalBitmap.Width + DrawLeft - 6;

                }
                return;
            }
            else if (_canMoveKlbl)
            {
                Point p = new Point(this.lblkinfo.Location.X + e.X - _mouseLocationX, this.lblkinfo.Location.Y + e.Y - _mouseLocationY);
                this.lblkinfo.Location = p;
                return;
            }
        }

        private void lblkinfo_MouseUp(object sender, MouseEventArgs e)
        {
            _canMoveKlbl = false;
            _mouseLocationY = -1;
            _mouseLocationX = -1;
        }

        public void DrawGraphic()
        {
            dg.ResetGraphics();
            
            patternMark.Clear();

            
            getData();
            
            
            if (kbase == null || kbase.Count == 0)
            {
                return;
            }
            //计算准备绘制k线图所需各数据
            ProcessKLineDatas();
            //绘制框架、k线、均线图时，先绘制到dg的缓冲中，所有内容绘制完毕，再一次性输出
            DrawFrame();
            DrawMALine();
            DrawKline();
            this.lblkinfo.Text = kbase.Stock["StockCode"].ToString();
            this.lblvinfo.Text = "";
            DrawGraphicFunc(this.CreateGraphics());
        }

        public void DrawGraphicFunc(Graphics g)
        {
            //重绘k线图时，会抹掉已绘制的跟随线（DrawCrossLineByFunc方法），所以需清空lastrects
            lastrects.Clear();
            Graphics clientDC = this.CreateGraphics();
            IntPtr hdcPtr = clientDC.GetHdc();
            IntPtr memdcPtr = CreateCompatibleDC(hdcPtr);   // 创建兼容DC  
            IntPtr bmpPtr = dg.LocalBitmap.GetHbitmap();
            SelectObject(memdcPtr, bmpPtr);
            BitBlt(hdcPtr, DrawLeft, DrawTop, dg.LocalBitmap.Width, dg.LocalBitmap.Height, memdcPtr, 0, 0, SRCCOPY);
            DeleteDC(memdcPtr);             // 释放内存  
            DeleteObject(bmpPtr);           // 释放内存  
            clientDC.ReleaseHdc(hdcPtr);    // 释放内存  
            clientDC.Dispose();
        }
        /// <summary>
        /// 根据Analyze结果的绘图数据，取得k线数据、ma数据、k线背景数据等
        /// </summary>
        private void getData()
        {
            if (arfd == null || AnalyzeResultForDrawId == arfd.id)
            {
                return;
            }

            if (tabpage.sdatasets.ContainsKey(arfd.stock["StockCode"].ToString()))
            {
                this.kbase = new KBase(tabpage.sdatasets[arfd.stock["StockCode"].ToString()]);
                ma4 = kbase.ma.ma4;
                ma8 = kbase.ma.ma8;
                ma12 = kbase.ma.ma12;
                ma24 = kbase.ma.ma24;
                dg.init(kbase);
            }
            else
            {
                PipeClient pc = new PipeClient();
                this.kbase =new KBase(pc.getData(arfd.stock));
                if (this.kbase != null)
                {
                    ma4 = kbase.ma.ma4;
                    ma8 = kbase.ma.ma8;
                    ma12 = kbase.ma.ma12;
                    ma24 = kbase.ma.ma24;
                    dg.init(kbase);
                }
            }

            //需要绘制k线柱背景的各日期索引值及对应类型
            DaysForDrawBack = new THashTable<DayTypesOfADay>();
            for (int i = kbase.Count - 1; i >= 0; i--)
            {
                if (arfd.KeyDays.ContainsKey(kbase[i].Date))
                {
                    DaysForDrawBack.Add(i,new DayTypesOfADay());
                    for (int j = 0; j < arfd.KeyDays[kbase[i].Date].Count; j++)
                    {
                        DaysForDrawBack[(object)i].Add(arfd.KeyDays[kbase[i].Date][j]);
                        if (arfd.KeyDays[kbase[i].Date][j] == DayType.AnalyzeDay)
                        {
                            PDayIndex = i;
                            ADayIdxOfst = PDayIndex - arfd.AnalyzeDayIdx;
                        }
                    }

                }
            }

            AnalyzeResultForDrawId = arfd.id;  
        }

        /// <summary>
        /// 在dg缓存中画框架
        /// 并不立刻显示在Form中
        /// </summary>
        private void DrawFrame()
        {
            dg.DrawFrame();
        }
         /// <summary>
        /// 处理k线数据，得到绘制开始、结束日、所有k线价格对应的原始坐标、相对原始坐标原点偏移量等
        /// </summary>
        private void ProcessKLineDatas()
        {
            ///
            ///确定绘制的起始索引值和结束索引值(自右向左)
            ///
            if (kbase == null || kbase.Count==0)
            {
                return;
            }

            if (kbase.Count - 1 - PDayIndex >= 12)
            {
                indexb = PDayIndex + 12;
            }
            else
            {
                indexb = kbase.Count - 1;
            }

            if (indexb >= KCountToDraw - 1)
            {
                indexe = indexb - KCountToDraw + 1;
            }
            else
            {
                indexe = 0;
            }

            currentKCount = indexb - indexe + 1; 

            ///
            ///寻找k线开、收盘价中最高值和最低值，计算各值对应的相对坐标值
            ///

            openYs = new int[currentKCount];
            closeYs = new int[currentKCount];
            highestYs = new int[currentKCount];
            lowestYs = new int[currentKCount];
            DType = new DayTypesOfADay[currentKCount];

            //最靠近顶端索引值
            int t = 0;
            //最靠近底端索引值
            int b = 0;
            //最高值纵坐标（纵坐标自上而下从小到大）
            int tv = 999999;
            //最低值纵坐标（纵坐标自上而下从小到大）
            int bv = -999999;
            //分析日高值纵坐标（纵坐标自上而下从小到大）
            //int adayv = -999999;
            //100%对应的纵坐标像素数：k线图绘图区域高度/12个10%
            pix100per = (int)((float)dg.KRect.Height / 1.2);

            TopVolHeight = dg.VRect.Height;
            TopVoli = indexe;

            int counttodraw = 0;
            //第一天收盘价纵坐标设为0
            closeYs[0] = 0;

            //计算每一天k线值的原始坐标
            for (int i = indexe; i <= indexb; i++)
            {
                //以indexe的收盘价为总坐标0点
                if (counttodraw == 0)
                {
                    //indexe当日价格以indexe的收盘价为参照点
                    //因为绘图坐标轴左上角为原点，所以在这个坐标系下需要注意高于indexe的收盘价的值对应的坐标值应该较小
                    openYs[0] = -(int)Math.Round((double)(((kbase.Open(i) - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0);
                    highestYs[0] = -(int)Math.Round((double)(((kbase.Highest(i) - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0);
                    lowestYs[0] = -(int)Math.Round((double)(((kbase.Lowest(i) - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0);
                }
                else
                {
                    //每一天的价格以前一天的收盘价为参照点
                    //closeYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Close(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                    //openYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Open(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                    //highestYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Highest(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                    //lowestYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Lowest(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                    openYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Open(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                    closeYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Close(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);
                    highestYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Highest(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);
                    lowestYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Lowest(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);
                }
                //记录最高值的坐标
                if (highestYs[counttodraw] <= tv)
                {
                    tv = highestYs[counttodraw];
                    t = i;
                }
                //记录最低值的坐标
                if (lowestYs[counttodraw] >= bv)
                {
                    bv = lowestYs[counttodraw];
                    b = i;
                }
                //记录分析日最高值坐标
                //if (PDayIndex==i)
                //{
                //    adayv = highestYs[counttodraw];
                //}
                ///
                ///记录每天对应的类型
                ///

                DType[counttodraw] = null;

                if (DaysForDrawBack.ContainsKey(i))
                {
                    DType[counttodraw] = DaysForDrawBack[(object)i];
                }

                ///
                ///记录交易量最高的那日对应的k线数据索引值
                ///
                if (kbase.Volume(i) > kbase.Volume(TopVoli))
                {
                    TopVoli = i;
                }

                counttodraw++;
            }

            ///
            ///根据最顶端纵坐标值和最底端纵坐标值决定基准值纵坐标偏移量
            ///绘图时，需要绘制内容的纵坐标经过偏移量的校正，显示在显示
            ///区域的中央。
            ///当最高值与最低值纵坐标之差超出显示区域的高时，在保证分析
            ///日被显示之余，优先显示出索引较小的最值。
            ///当最高值与最低值纵坐标之差小于显示区域的高时，将显示内容
            ///剧中显示。
            ///

            if (Math.Abs(tv) + Math.Abs(bv) > dg.KRect.Height)
            {
                //最底端值在最顶端值之后，优先绘制最底端，从最底端值计算基准值坐标偏移量
                if (b > t)
                {
                    baselineY = dg.KRect.Height - bv;
                }
                //低值在高值之前，优先绘制高值，从高值计算基准值
                else
                {
                    baselineY = 0 - tv;
                }
            }
            else
            {
                //最低和最高坐标值的中间值与k线图绘图区域纵坐标中点的差
                baselineY = (dg.KRect.Height / 2) - (tv + bv) / 2;

            }
        }
        /// <summary>
        /// 在dg的缓存中绘制均线
        /// 并不立刻显示在Form中
        /// </summary>
        private void DrawMALine()
        {
            int ma4Y = 0;
            int ma8Y = 0;
            int ma12Y = 0;
            int ma24Y = 0;
            //前一天的ma值
            int ma4YP = 0;
            int ma8YP = 0;
            int ma12YP = 0;
            int ma24YP = 0;
            int drawcount = 0;
            //需要绘制的天数
            int ctd = currentKCount;
            for (int i = indexb; i >= indexe; i--)
            {
                //基准值纵坐标在KBitmap正中间，坐标轴上小下大，价格相对上升时的升幅坐标量为正，价格相对下降的降幅坐标量为负
                //所以需要用基准值纵坐标-升(降)幅坐标量
                ctd--;
                //if (ma4.ContainsKey(i))
                if (ma4.Length > i)
                {
                    if (ctd == 0)
                    {
                        ma4Y = -(int)Math.Round((double)(((ma4[i] - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0) ;
                        ma8Y = -(int)Math.Round((double)(((ma8[i] - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0) ;
                        ma12Y = -(int)Math.Round((double)(((ma12[i] - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0) ;
                        ma24Y = -(int)Math.Round((double)(((ma24[i] - kbase.Close(i)) / kbase.Close(i)) * pix100per), 0) ;
                    }
                    else
                    {
                        //ma4Y = closeYs[ctd - 1] - (int)Math.Round((double)(((ma4[i] - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0) + baselineY;
                        //ma8Y = closeYs[ctd - 1] - (int)Math.Round((double)(((ma8[i] - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0) + baselineY;
                        //ma12Y = closeYs[ctd - 1] - (int)Math.Round((double)(((ma12[i] - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0) + baselineY;
                        //ma24Y = closeYs[ctd - 1] - (int)Math.Round((double)(((ma24[i] - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0) + baselineY;
                        //openYs[counttodraw] = closeYs[counttodraw - 1] - (int)Math.Round((double)(((kbase.Open(i) - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                        //closeYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Close(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);
                        //highestYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Highest(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);
                        //lowestYs[counttodraw] = openYs[counttodraw] - (int)Math.Round((double)(((kbase.Lowest(i) - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0);

                        ma4Y = closeYs[ctd - 1] - (int)Math.Round((double)(((ma4[i] - kbase.Close(i - 1)) / kbase.Close(i - 1)) * pix100per), 0);
                        //ma8Y = ma4Y - (int)Math.Round((double)(((ma8[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                        //ma12Y = ma4Y - (int)Math.Round((double)(((ma12[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                        //ma24Y = ma4Y - (int)Math.Round((double)(((ma24[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                        //ma4Y = openYs[ctd] - (int)Math.Round((double)(((ma4[i] - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0) ;
                        //ma8Y = openYs[ctd] - (int)Math.Round((double)(((ma8[i] - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0) ;
                        //ma12Y = openYs[ctd] - (int)Math.Round((double)(((ma12[i] - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0) ;
                        ////ma24Y = openYs[ctd] - (int)Math.Round((double)(((ma24[i] - kbase.Open(i)) / kbase.Open(i)) * pix100per), 0) ;
                        //ma4Y = ma4YP + (int)Math.Round((double)(((ma4[i] - ma4[i-1]) / ma4[i-1]) * pix100per), 0);
                        ma8Y = ma4Y - (int)Math.Round((double)(((ma8[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                        ma12Y = ma4Y - (int)Math.Round((double)(((ma12[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                        ma24Y = ma4Y - (int)Math.Round((double)(((ma24[i] - ma4[i]) / ma4[i]) * pix100per), 0);
                    }
                    if (ma4YP != 0 && ma4Y != 0)
                    {
                        dg.DrawMALine(drawcount + 1, ma4Y + baselineY, ma8Y + baselineY, ma12Y + baselineY, ma24Y + baselineY, ma4YP + baselineY, ma8YP + baselineY, ma12YP + baselineY, ma24YP + baselineY);
                    }
                    ma4YP = ma4Y;
                    ma8YP = ma8Y;
                    ma12YP = ma12Y;
                    ma24YP = ma24Y;
                }
                drawcount++;
            }
        }
        /// <summary>
        /// 在dg的缓存中绘制k线图
        /// 并不立刻显示在Form中
        /// </summary>
        private void DrawKline()
        {
            //return;
             dg.DrawKline(currentKCount, indexb, indexe, baselineY, openYs, closeYs, highestYs, lowestYs, TopVoli,TopVolHeight, cbkbgcolor.Checked, DType);

             RKBarRight = dg.CrossLineRect.Right - dg.kOffset - dg.kBarBlank;
             LKBarLeft = dg.CrossLineRect.Right - dg.kOffset - (indexb - indexe + 1) * (dg.kBarBlank + dg.kBarWidth);
        }

        private void DrawLable(Graphics g,string txt)
        {

        }

        private void FormKLineGraphic_Paint(object sender, PaintEventArgs e)
        {
            DrawGraphicFunc(e.Graphics);
        }

        private void FormKLineGraphic_MouseLeave(object sender, EventArgs e)
        {
            //擦出上一次在绘制跟随线响应区域内的跟随线
            //DrawCrossLine1(new Point(-1, -1));
        }

        private void FormKLineGraphic_MouseMove(object sender, MouseEventArgs e)
        {

            ///
            ///以下所有坐标值均为dg画布的坐标系值
            ///
            if (kbase == null || kbase.Count == 0)
            {
                return;
            }
            //把鼠标在form中的坐标值转换为在dg画布中的坐标值
            Point eIndg = new Point(e.Location.X - DrawLeft, e.Location.Y - DrawTop);

            //跟随线的x坐标。鼠标x坐标在某一个k线柱的区域内，描绘鼠标跟随线时，y坐标随鼠标y坐标改变，但x坐标设定为k线柱正中央，直到移动到另一个k线柱范围内。
            int px = 0;
            int currentIndex = 0;

            xInDG(eIndg, ref px, ref currentIndex);

            //px移动到另一个k线柱时，需要显示该k线柱信息
            //在标签中显示当日信息
            if (px != dg.CLinelastPoint.X
                ||//鼠标位置在最左侧和最右侧k线住外侧，通常方式描绘跟随线 
                (eIndg.X <= LKBarLeft || eIndg.X >= RKBarRight + 1))
            {
                //绘制Info1方法中包含绘制跟随线方法，并且绘制跟随线方法会更新Form图像
                //所以不用单独调用DrawCrossLine1
                DrawInfo1(currentIndex, new Point(px, eIndg.Y));
                //画跟随线时会更新Form
                DrawCrossLine1(new Point(px, eIndg.Y));
            }
            else
            {
                DrawCrossLine1(new Point(px, e.Location.Y - DrawTop));
            }


        }
        /// <summary>
        /// 得到鼠标x轴在画布dg中的位置以及鼠标x轴坐标所处k线柱对应的k线索引值
        /// </summary>
        private void xInDG(Point eIndg, ref int px, ref int currentIndex)
        {

            //鼠标位置在最左侧和最右侧k线住外侧，直接取x值，以便通常方式描绘跟随线
            if (eIndg.X <= LKBarLeft || eIndg.X >= RKBarRight + 1)
            {
                //DrawInfo1(-1, eIndg);
                currentIndex = -1;
                px = eIndg.X;
            }
            //鼠标位置在最左侧和最右侧k线柱内测时，鼠标x坐标移动到一个k线柱范围内时，跟随线的竖线会“跳跃”地描绘在k线柱宽度正中间
            else
            {
                //鼠标x坐标在某一个k线柱的区域内，距该k线柱最左侧的距离（k线柱左上角x坐标为0）
                int rem;
                //跟随线的x坐标。鼠标x坐标在某一个k线柱的区域内，描绘鼠标跟随线时，y坐标随鼠标y坐标改变，但x坐标设定为k线柱正中央，直到移动到另一个k线柱范围内。
                //int px = 0;
                //鼠标在某个k线柱范围内时，从左侧计算该k线柱是第几个
                int lcount = 0;

                //int currentIndex = 0;
                //用鼠标x值与最左侧k线柱左上角x值之差与一个k线柱区域宽度求余，计算距离最左侧k线柱个数和鼠标x值与所在某k线柱范围最左侧距离
                lcount = Math.DivRem(eIndg.X - LKBarLeft, dg.kBarWidth + dg.kBarBlank, out rem);
                //鼠标x值在某k线柱范围内时，跟随线x坐标始终是该k线柱宽度正中间
                if (rem <= dg.kBarWidth + dg.kBarBlank / 2)
                {
                    px = LKBarLeft + lcount * (dg.kBarWidth + dg.kBarBlank) + (dg.kBarWidth / 2 + 1);
                    currentIndex = indexe + lcount;
                }
                else
                {
                    px = LKBarLeft + (lcount + 1) * (dg.kBarWidth + dg.kBarBlank) + (dg.kBarWidth / 2 + 1);
                    currentIndex = indexe + lcount + 1;
                }
            }
        }
        /// <summary>
        /// 根据给定的dg坐标系中的点，绘制鼠标跟随线
        /// </summary>
        /// <param name="p"></param>
        private void DrawCrossLine1(Point p)
        {
            if (!drawingCline)
            {
                drawingCline = true;
                Thread th = new Thread(new ParameterizedThreadStart(DrawReversibleLine));
                th.Name = "DrawCLine";
                th.IsBackground = true;
                th.Start(p);
            }
            else
            {
                int debug97798345 = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point PointToScreenFunc(Point point)
        {
            return point;
        }
        /// <summary>
        /// 根据给定dg坐标系中的点，绘制以此点为中心的十字反色线
        /// </summary>
        /// <param name="_p"></param>
        private void DrawReversibleLine(object _p)
        {
            Point p = (Point)_p;

            //鼠标移出绘图区域时抹除上次绘制的线
            if (p.X < dg.CrossLineRect.Left + DrawLeft || p.Y < dg.CrossLineRect.Top || p.X > dg.CrossLineRect.Right + DrawLeft || p.Y > dg.CrossLineRect.Bottom)
            {
                DrawReversibleLineByRects(lastrects);
                lastrects.Clear();
                drawingCline = false;
                return;
            }
            
            //纵线坐标
            Point x1 = new Point(p.X + DrawLeft, DrawTop + dg.KRect.Top);
            Point y1 = new Point(p.X + DrawLeft, DrawTop + dg.VRect.Bottom);
            //横线坐标
            Point x2 = new Point(DrawLeft + dg.KRect.Left, DrawTop + p.Y);
            Point y2 = new Point(DrawLeft + dg.KRect.Width, DrawTop + p.Y);
            //lblkinfo左上角坐标
            Point kinfo1 = new Point(lblkinfo.Left, lblkinfo.Top);
            //lblinfo右下角坐标
            Point kinfo2 = new Point(lblkinfo.Right, lblkinfo.Bottom);
            //lblvinfo左上角坐标
            Point vinfo1 = new Point(lblvinfo.Left, lblvinfo.Top);
            //lblvinfo右下角坐标
            Point vinfo2 = new Point(lblvinfo.Right, lblvinfo.Bottom);

            //待绘制的线对应的Rectangle列表
            List<Rectangle> rects = new List<Rectangle>();
            //临时存储新的点需绘制的线所对应的Rectangle列表
            List<Rectangle> rectstmp = new List<Rectangle>();
            //上次绘制的线需要绘制为反色（抹去），所以上次绘制线对应的Rectangle列表也添加到待绘制的线对应的Rectangle列表
            rects.AddRange(lastrects);
            //生成本次新绘制的线对应的Rectangle列表
            //纵线穿过kinfo
            if (x1.X >= kinfo1.X && x1.X <= kinfo2.X)
            {
                //增加顶部到kinfo上边缘线段
                rectstmp.Add(new Rectangle(x1.X, x1.Y, 0, kinfo1.Y - x1.Y));
                //增加kinfo下边缘到vinfo上边缘线段
                rectstmp.Add(new Rectangle(x1.X, kinfo2.Y, 0, vinfo1.Y - kinfo2.Y));
                //增加vinfo下边缘到底部线段
                rectstmp.Add(new Rectangle(x1.X, vinfo2.Y, 0, y1.Y - vinfo2.Y));
            }
            else
            {
                //增加顶部到vinfo上边缘线段
                rectstmp.Add(new Rectangle(x1.X, x1.Y, 0, vinfo1.Y - x1.Y));
                //增加vinfo下边缘到底部线段
                rectstmp.Add(new Rectangle(x1.X, vinfo2.Y, 0, y1.Y - vinfo2.Y));
            }

            //横线穿过kinfo
            if (x2.Y >= kinfo1.Y && x2.Y <= kinfo2.Y)
            {
                //增加左部到kinfo左边缘线段
                rectstmp.Add(new Rectangle(x2.X, x2.Y, kinfo1.X - x2.X, 0));
                //增加kinfo右边缘到右部线段
                rectstmp.Add(new Rectangle(kinfo2.X, x2.Y, y2.X - kinfo2.X, 0));
            }
            else
            {
                rectstmp.Add(new Rectangle(x2.X, x2.Y, y2.X - x2.X, 0));
            }
            
            rects.AddRange(rectstmp);

            //绘制反色线
            DrawReversibleLineByRects(rects);

            rects.Clear();
            lastrects.Clear();
            //本次新绘制的线作为最后一次绘制的线
            lastrects.AddRange(rectstmp);
            //Thread.Sleep(5);
            drawingCline = false;
        }

        /// <summary>
        /// 根据给出的Rectangle绘制与Rectangle相符的反色线
        /// </summary>
        /// <param name="rects"></param>
        private void DrawReversibleLineByRects(List<Rectangle> rects)
        {
            Point[] starts = new Point[rects.Count];
            Point[] ends = new Point[rects.Count];
            for (int i = 0; i < rects.Count; i++)
            {
                if (rects[i].Width < 0 || rects[i].Height < 0)
                {
                    continue;
                }
                starts[i] = new Point(rects[i].X, rects[i].Y);
                ends[i] = new Point(rects[i].X + rects[i].Width, rects[i].Height + rects[i].Y);
            }
            
            DrawReversibleLine(starts, ends, Color.Black);
        }

        /// <summary>
        /// 在dg.localbitmap中绘制跟随线
        /// 暂不使用
        /// </summary>
        /// <param name="_p"></param>
        private void DrawCrossLine(object _p)
        {

            
            lock (dg)
            {
                Point p = (Point)_p;
                DateTime dt1 = DateTime.Now;
                dg.DrawCrossLine1(p);
                //return;
                //this.
                DateTime dt2 = DateTime.Now;
                Graphics clientDC = this.CreateGraphics();
                IntPtr hdcPtr = clientDC.GetHdc();
                IntPtr memdcPtr = CreateCompatibleDC(hdcPtr);   // 创建兼容DC  
                IntPtr bmpPtr = dg.LocalBitmap.GetHbitmap();
                SelectObject(memdcPtr, bmpPtr);
                lock (clientDC)
                {
                    BitBlt(hdcPtr, DrawLeft, DrawTop, dg.LocalBitmap.Width, dg.LocalBitmap.Height, memdcPtr, 0, 0, SRCCOPY);
                }
                DateTime dt3 = DateTime.Now;
                long ct1 = (dt2.Ticks - dt1.Ticks) / 10000;
                long ct2 = (dt3.Ticks - dt2.Ticks) / 10000;
                if (p.X > 100)
                {
                    int debug234124 = 1;
                }



                //Graphics ClientDC = this.CreateGraphics();
                //Rectangle cliprect = new Rectangle(p.X + DrawLeft, 0, 1, dg.LocalBitmap.Height);
                //ClientDC.SetClip(cliprect);
                //IntPtr hdcPtr = ClientDC.GetHdc();
                //IntPtr memdcPtr = CreateCompatibleDC(hdcPtr);
                //Bitmap bitmap = new Bitmap(dg.LocalBitmap.Width, dg.LocalBitmap.Height);
                //IntPtr bmpPtr = bitmap.GetHbitmap();
                //Graphics bg = Graphics.FromImage(bitmap);
                //bg.Clear(Color.Transparent);


                ////x位置改变时才重绘
                //if (lastMousePoit.X != p.X && p.X > 0 && p.X < dg.LocalBitmap.Width)
                //{
                //    //擦除上次绘制跟随线
                //    if (lastMousePoit.X > 0)
                //    {
                //        //bg.DrawImage(dg.LocalBitmap, lastMousePoit.X, 0, new Rectangle(lastMousePoit.X, 0, 1, dg.LocalBitmap.Height), GraphicsUnit.Pixel);
                //        //SelectObject(memdcPtr, bitmap.GetHbitmap());
                //        //BitBlt(hdcPtr, lastMousePoit.X + DrawLeft, DrawTop, lastCrossLineY.Width, lastCrossLineY.Height, memdcPtr, 0, 0, 13369376);
                //    }

                //    bg.DrawImage(dg.LocalBitmap, p.X, 0, new Rectangle(p.X, 0, 1, dg.LocalBitmap.Height), GraphicsUnit.Pixel);
                //    //lastCrossLineY = bitmap;


                //    Rectangle rect = new Rectangle(p.X, 0, 1, bitmap.Height);
                //    BitmapData xline = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                //    unsafe
                //    // 不安全代码  
                //    {// 像素首地址
                //        byte* pix = (byte*)xline.Scan0;
                //        //对每一个点颜色取反  
                //        for (int i = 0; i < xline.Stride * xline.Height; i++) pix[i] = (byte)(255 - pix[i]);
                //    }
                //    bitmap.UnlockBits(xline);


                //    SelectObject(memdcPtr, bmpPtr);
                //    BitBlt(hdcPtr, 0, 0, bitmap.Width, bitmap.Height, memdcPtr, 0, 0, 13369376);
                //    lastMousePoit = p;
                //}
                //bg.Dispose();

                DeleteDC(memdcPtr);             // 释放内存  
                DeleteObject(bmpPtr);           // 释放内存  
                clientDC.ReleaseHdc(hdcPtr);    // 释放内存  
                clientDC.Dispose();
                
            }
            Thread.Sleep(5);
            
            drawingCline = false;
            tmpcount--;
        }

        /// <summary>
        /// 在dg的缓存中绘制Info1信息，因为需绘制时，必然鼠标跟随线也需要重绘，所以方法内部调用绘制鼠标跟随线方法
        /// 本方法立刻把绘制后的dg缓存显示在Form中
        /// </summary>
        /// <param name="info"></param>
        /// <param name="p"></param>
        private void DrawInfo1(int currentIndex, Point p)
        {
            if (p.X < dg.CrossLineRect.Left + DrawLeft || p.Y < dg.CrossLineRect.Top || p.X > dg.CrossLineRect.Right + DrawLeft || p.Y > dg.CrossLineRect.Bottom)
            {
                //lastrects.Clear();
                //drawingCline = false;
                return;
            }
            //return;
            string info1 = kbase.Stock["StockCode"].ToString();
            string info2 = "";
            if (currentIndex >= 0)
            {
                info1 += "\r\n开:" + kbase.Open(currentIndex).ToString();
                info1 += "\r\n收:" + kbase.Close(currentIndex).ToString();
                info1 += "\r\n高:" + kbase.Highest(currentIndex).ToString();
                info1 += "\r\n低:" + kbase.Lowest(currentIndex).ToString();
                if (currentIndex > 0)
                {
                    info1 += "\r\n前比:" + kbase.Percent(kbase.Close(currentIndex) - kbase.Close(currentIndex - 1), kbase.Close(currentIndex - 1)) + "%";
                }
                else
                {
                    info1 += "\r\n前比:--";
                }
                info1 += "\r\n今比:" + kbase.Percent(kbase.Close(currentIndex) - kbase.Open(currentIndex), kbase.Open(currentIndex)) + "%";
                if (kbase.Close(currentIndex) >= kbase.Open(currentIndex))
                {
                    info1 += "\r\n极值比:" + kbase.Percent(kbase.Highest(currentIndex) - kbase.Lowest(currentIndex), kbase.Lowest(currentIndex)) + "%";
                }
                else
                {
                    info1 += "\r\n极值比:" + kbase.Percent(kbase.Lowest(currentIndex) - kbase.Highest(currentIndex), kbase.Highest(currentIndex)) + "%";
                }
                info1 += "\r\n上影线:" + kbase.Percent(kbase.Highest(currentIndex) - kbase.High(currentIndex), kbase.High(currentIndex)) + "%";
                info1 += "\r\n下影线:" + kbase.Percent(kbase.Low(currentIndex) - kbase.Lowest(currentIndex), kbase.Lowest(currentIndex)) + "%";
                //if (ma4.ContainsKey(currentIndex))
                if (ma4.Length > currentIndex)
                {
                    info1 += "\r\nMA4:" + ma4[currentIndex].ToString();
                }
                //if (ma8.ContainsKey(currentIndex))
                if (ma8.Length > currentIndex)
                {
                    info1 += "\r\nMA8:" + ma8[currentIndex].ToString();
                }
                //if (ma12.ContainsKey(currentIndex))
                if (ma12.Length > currentIndex)
                {
                    info1 += "\r\nMA12:" + ma12[currentIndex].ToString();
                }
                if (ma24.Length > currentIndex)
                {
                    info1 += "\r\nMA24:" + ma24[currentIndex].ToString();
                }
                if (DaysForDrawBack.ContainsKey(currentIndex))
                {
                    DayTypesOfADay dtypes = DaysForDrawBack[(object)currentIndex];

                    for (int j = 0; j <= dtypes.Count - 1; j++)
                    {
                        switch (dtypes[j])
                        {
                            case DayType.AnalyzeDay:
                                info1 += "\r\n分析日";
                                break;
                            case DayType.TrendJudgedR3:
                                info1 += "\r\nR3";
                                break;
                            case DayType.TrendJudgedF3:
                                info1 += "\r\nF3";
                                break;
                            case DayType.MA4HDay:
                                info1 += "\r\nMA4高";
                                break;
                            case DayType.MA4LDay:
                                info1 += "\r\nMA4低";
                                break;
                            case DayType.BeginDayLv1:
                                info1 += "\r\n子波：始";
                                break;
                            case DayType.EndDayLv1:
                                info1 += "\r\n子波：终";
                                break;
                            case DayType.RHDay:
                                info1 += "\r\nRHDay";
                                break;
                            case DayType.Debug1:
                                info1 += "\r\ndebug1";
                                break;
                        }
                    }
                }

                info1 += "\r\n" + kbase[currentIndex].Date.Split(' ')[0].Replace("-", "/");
                info1 += "\r\nIndex:" + (currentIndex - ADayIdxOfst);

                info2 += "vol:" + kbase.Volume(currentIndex);
                //info2 += " mavol12:" + kbase.mavol12(currentIndex);
                info2 += " 前比:";
                if (currentIndex > 0)
                {
                    info2 += (kbase.Volume(currentIndex) / kbase.Volume(currentIndex - 1)).ToString("0.00");
                }

                info2 += " 流通比:1/" + (1 / ((double)kbase.Volume(currentIndex) / (double)kbase.Ccapital(currentIndex))).ToString("0.00");
                ///
                ///包络线点对象未更新，暂时屏蔽
                ///
                //if(kbase.isVolENode(currentIndex))
                //{
                //    int nodeidx=kbase.findVOLEnvelopeNodeIndex(currentIndex);
                //    if (kbase.VolENodes[nodeidx].tpType == tPointType.Top)
                //    {
                //        info2 += " 包络线点顶";
                //    }
                //    else if (kbase.VolENodes[nodeidx].tpType == tPointType.Bottom)
                //    {
                //        info2 += " 包络线点底";
                //    }
                //    else
                //    {
                //        info2 += " 包络线点";
                //    }
                //}
            }
            //先绘制Info1，再绘制鼠标跟随线，使鼠标跟随线处于最上方
            //DrawInfo1方法只是更新dg.localbitmap，并不更新本Form
            //dg.DrawkInfo(info1);
            this.lblkinfo.Text = info1;
            this.lblvinfo.Text = info2;
           //dg.DrawvInfo(info2);
            
        }

        private void FormKLineGraphic_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {
            //绘制边框
            ControlPaint.DrawBorder(e.Graphics,
                            this.lblkinfo.ClientRectangle,
                            Color.Red,//7f9db9
                            1,
                            ButtonBorderStyle.Solid,
                            Color.Red,
                            1,
                            ButtonBorderStyle.Solid,
                            Color.Red,
                            1,
                            ButtonBorderStyle.Solid,
                            Color.Red,
                            1,
                            ButtonBorderStyle.Solid);
        }

        private void FormKLineGraphic_Shown(object sender, EventArgs e)
        {
            //首次显示窗体时初始化绘制反色线用的Graphic和窗体句柄
            grfx = base.CreateGraphics();
            hdc = grfx.GetHdc();
        }

        private void FormKLineGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawingCline)
            {
                return;
            }
            drawingCline = true;

            ///
            ///以下所有坐标值均为dg画布的坐标系值
            ///
            if (kbase == null)
            {
                return;
            }
            //把鼠标在form中的坐标值转换为在dg画布中的坐标值
            Point eIndg = new Point(e.Location.X - DrawLeft, e.Location.Y - DrawTop);

            //跟随线的x坐标。鼠标x坐标在某一个k线柱的区域内，描绘鼠标跟随线时，y坐标随鼠标y坐标改变，但x坐标设定为k线柱正中央，直到移动到另一个k线柱范围内。
            int px = 0;
            int currentIndex = 0;

            xInDG(eIndg, ref px, ref currentIndex);

            //鼠标位于有效k线柱范围内
            if (currentIndex >= 0 && eIndg.Y > dg.KRect.Top && eIndg.Y < dg.KRect.Bottom)
            {

                //形态匹配标记目前可以有2个
                //当前位置没有被记录在形态匹配标记中
                //记录
                if (patternMark.Count < 2 && !patternMark.ContainsKey(currentIndex))
                {
                    
                    patternMark.Add(currentIndex, kbase[currentIndex].Date);
                    //在dg.localbitmap中绘制标志
                    dg.DrawMark(currentIndex, indexe, currentIndex, new Point(px, eIndg.Y));
                    //将新dg.localbitmap显示
                    DrawGraphicFunc(this.CreateGraphics());
                    //DrawGraphicFunc会抹掉已绘制跟随线，所以需要重新绘制
                    //DrawCrossLineByFunc(new Point(px, eIndg.Y));
                    DrawReversibleLine(new Point(px, eIndg.Y));
                }
                //当前位置已被记录
                //删除
                else if (patternMark.Count > 0 && patternMark.ContainsKey(currentIndex))
                {
                    patternMark.Remove(currentIndex);
                    //在dg.localbitmap中抹掉标志
                    dg.EraseMark(currentIndex, indexe, currentIndex, new Point(px, eIndg.Y));
                    //将新dg.localbitmap显示
                    DrawGraphicFunc(this.CreateGraphics());
                    //DrawGraphicFunc会抹掉已绘制跟随线，所以需要重新绘制
                    //DrawCrossLineByFunc(new Point(px, eIndg.Y));
                    DrawReversibleLine(new Point(px, eIndg.Y));
                }
            }
           
            drawingCline = false;

        }

        private void FormKLineGraphic_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cbkbgcolor_CheckedChanged(object sender, EventArgs e)
        {
            dg.ResetGraphics();
            patternMark.Clear();
            //绘制框架、k线、均线图时，先绘制到dg的缓冲中，所有内容绘制完毕，再一次性输出
            DrawFrame();
            DrawMALine();
            DrawKline();
            DrawGraphicFunc(this.CreateGraphics());
        }

        void FormKLineGraphic_VisibleChanged(object sender, System.EventArgs e)
        {

        }

        private void FormKLineGraphic_FormClosed(object sender, FormClosedEventArgs e)
        {
            //退出时释放绘制反色线用Graphic和窗体句柄
            if (grfx != null)
            {
                grfx.ReleaseHdc(hdc);
                grfx.Dispose();
            }
        }
    }
}
