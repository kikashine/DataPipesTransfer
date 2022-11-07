using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    public class DrawGraphics:KBase
    {
        private Form f;
        /// <summary>
        /// 绘图位图，所有图像描绘在位图上，再由调用Form显示
        /// </summary>
        private Bitmap localBitmap;
        /// <summary>
        /// 绘图图面
        /// </summary>
        private Graphics g;
        /// <summary>
        /// 绘制K线的Bitmap，绘制完k线图后，需要将KBitmap绘制到localBitmap
        /// </summary>
        private Bitmap KBitmap;
        /// <summary>
        /// k线绘图图面
        /// </summary>
        private Graphics KBitmapGraphics;
        /// <summary>
        /// 绘制交易量的Bitmap，绘制完交易量图后，需要将VBitmap绘制到localBitmap
        /// </summary>
        private Bitmap VBitmap;
        /// <summary>
        /// 交易量绘图图面
        /// </summary>
        private Graphics VBitmapGraphics;
        /// <summary>
        /// 绘制标记的Bitmap，绘制完后，需要绘制到localBitmap
        /// </summary>
        private Bitmap MarkBitmap;
        /// <summary>
        /// 标记绘图图面
        /// </summary>
        private Graphics MarkBitmapGraphics;

        private THashTable<Bitmap> MarkOriginalBitMaps;
        /// <summary>
        /// 上升k线柱的画笔
        /// </summary>
        private Pen risePen;
        /// <summary>
        /// 下降k线柱的画笔
        /// </summary>
        private Pen fallPen;
        /// <summary>
        /// 红色虚线画笔
        /// </summary>
        private Pen redDotPen;
        /// <summary>
        /// 红色实线画笔
        /// </summary>
        private Pen redLinePen;
        /// <summary>
        /// ma4线画笔
        /// </summary>
        private Pen ma4Pen;
        /// <summary>
        /// ma8线画笔
        /// </summary>
        private Pen ma8Pen;
        /// <summary>
        /// ma12线画笔
        /// </summary>
        private Pen ma12Pen;

        private Pen ma24Pen;
        /// <summary>
        /// 红色刷子
        /// </summary>
        private Brush redBrush;
        /// <summary>
        /// 绿色刷子
        /// </summary>
        private Brush greenBrush;
        /// <summary>
        /// 白色刷子
        /// </summary>
        private Brush whiteBrush;
        /// <summary>
        /// 下降k线柱的填充刷子
        /// </summary>
        private Brush fallBrush;
        /// <summary>
        /// 下降k线柱的填充刷子
        /// </summary>
        private Brush riseBrush;
        /// <summary>
        /// 分析结果符合条件的分析日背景刷子
        /// </summary>
        private Brush AnalyzeDaybrush;
        /// <summary>
        /// 趋势ma4最值之日背景刷子
        /// </summary>
        private Brush MA4HLDayBrush;
        /// <summary>
        /// 趋势判断之日背景刷子
        /// </summary>
        private Brush TrendJudgedBrush;

        private Brush BeginDayLv1Brush;

        private Brush EndDayLv1Brush;

        private Brush RHDayBrush;

        private Brush Debug1Brush;

        private Brush Debug2Brush;
        
        private Brush MarkBrush1;

        private Font font;

        private int DrawWidth;

        private int DrawHeight;
        /// <summary>
        /// k线绘图区域
        /// </summary>
        private Rectangle kRect;
        /// <summary>
        /// kInfo绘制在localbitmap上的区域
        /// </summary>
        private Rectangle kInfoRect;
        /// <summary>
        /// 交易量信息区域
        /// </summary>
        private Rectangle vInfoRect;
        /// <summary>
        /// 交易量绘图区域
        /// </summary>
        private Rectangle vRect;
        /// <summary>
        /// 鼠标跟随线下端的浮动日期标签绘图区域
        /// </summary>
        private Rectangle dateTipRect;
        /// <summary>
        /// 鼠标跟随线的鼠标响应区域
        /// 仅在此区域内相应鼠标事件绘制跟随线
        /// </summary>
        private Rectangle crossLineRect;
        /// <summary>
        /// 鼠标跟随线上一个绘制位置的鼠标坐标
        /// </summary>
        private Point cLinelastPoint = new Point(-1, -1);

        private Point cLinePoint = new Point(-1, -1);
        /// <summary>
        /// k线右侧第一个柱线与k线图区域右侧距离
        /// </summary>
        private int koffset;
        /// <summary>
        /// k线柱宽度
        /// </summary>
        private int kbarwidth;
        /// <summary>
        /// k线柱间距
        /// </summary>
        private int kbarblank;
        /// <summary>
        /// 根据绘制区域大小计算出的允许绘制的k线柱数量
        /// </summary>
        private int kcounttodraw;
        /// <summary>
        /// 新绘制了新的k线图，需要将KBitmap再次绘制到localbitmap上
        /// </summary>
        private bool drewNewKLine = false;
        /// <summary>
        /// 在新k线图上绘制了鼠标跟随线
        /// </summary>
        private bool drewNewCrossLine = false;
        /// <summary>
        /// 绘制\更新鼠标跟随线的区域
        /// </summary>
        private Region crossLineRegion = new Region(new Rectangle(0, 0, 0, 0));
        /// <summary>
        /// kInfo所绘制前，该区域的原始图像(绘制k线、均线图后，绘制鼠标跟随线之前的localbitmap的kInfoRect区域的图像)
        /// 每次绘制kInfo新内容前，需要用该区域的原始图像重置localbitmap对应的区域，抹掉旧的kInfo内容
        /// 重新绘制k线、均线后，kInfoRectOriginalImage会重置为null
        /// </summary>
        private Bitmap kInfoRectOriginalImage = null;
        /// <summary>
        /// 需要在kInfoRect区域擦出上一次绘制的跟随线
        /// 当kInfo绘制内容更新时会用没有跟随线的原始图像重置kInfoRect，所以此时置为false
        /// 除此之外置为true
        /// </summary>
        private bool NeedEraseCrossLinevInfoRect;

        private Bitmap vInfoRectOriginalImage = null;
        /// <summary>
        /// 标志正在绘图
        /// </summary>
        private bool drawing;

        /// <summary>
        /// 需要在kInfoRect区域擦出上一次绘制的跟随线
        /// 当kInfo绘制内容更新时会用没有跟随线的原始图像重置kInfoRect，所以此时置为false
        /// 除此之外置为true
        /// </summary>
        private bool NeedEraseCrossLinekInfoRect;

        /// <summary>
        /// 绘制完k线图
        /// </summary>
        private bool DrewKLine;

        private int tmpcount = 0;
        /// <summary>
        /// 绘制好的区域
        /// </summary>
        public Region DrewRegion
        {
            get
            {
                return crossLineRegion;
            }
        }
        /// <summary>
        /// k线右侧第一个柱线与k线图区域右侧距离
        /// </summary>
        public int kOffset
        {
            get
            {
                return koffset;
            }

        }
        /// <summary>
        /// k线柱宽度
        /// </summary>
        public int kBarWidth
        {
            get
            {
                return kbarwidth;
            }
        }
        /// <summary>
        /// k线柱间距
        /// </summary>
        public int kBarBlank
        {
            get
            {
                return kbarblank;
            }
        }
        /// <summary>
        /// 根据绘制区域大小计算出的允许绘制的k线柱数量
        /// </summary>
        public int KCountToDraw
        {
            get
            {
                return kcounttodraw;
            }
        }
        public Rectangle KRect
        {
            get
            {
                return kRect;
            }
        }

        public Rectangle VRect
        {
            get
            {
                return vRect;
            }
        }

        public Rectangle VInfoRect
        {
            get
            {
                return vInfoRect;
            }
        }

        public Rectangle DateTipRect
        {
            get
            {
                return dateTipRect;
            }
        }

        public Rectangle CrossLineRect
        {
            get
            {
                return crossLineRect;
            }
        }

        public Point CLinelastPoint
        {
            get
            {
                return cLinelastPoint;
            }
        }
        public Bitmap LocalBitmap
        {
            get
            {
                //首先将绘制有k线图的KBitmap绘制到localBitmap
                //if (drewNewKLine)
                //{
                //    g.DrawImage(KBitmap, kRect.Left, kRect.Top);
                //    drewNewKLine = false;
                //}
                //将鼠标跟随线画在最上方
                //DrawCrossLineFunc();
                return localBitmap;
            }
        }

        //public DataTableQ theDT
        //{
        //    set
        //    {
        //        base.theDT = value;
        //    }
        //}

        public bool drewKLine
        {
            get
            {
                return DrewKLine;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DrawWidth"></param>
        /// <param name="DrawHeight"></param>

        public DrawGraphics(int DrawWidth, int DrawHeight)
        {

            this.DrawWidth = DrawWidth;
            this.DrawHeight = DrawHeight;

            int KRegionTop = 1;
            int KRegionLeft = 1;
            int KRegionHeight = 576;
            //int KRegionWidth = 700;
            int KRegionWidth = this.DrawWidth - 60;
            //int VInfoRegionTop = 524;
            int VInfoRegionLeft = 1;
            int VInfoRegionHeight = 20;
            //int VInfoRegionWidth = 700;
            int VInfoRegionWidth = this.DrawWidth - 60;
            //int VRegionTop = 545;
            int VRegionLeft = 1;
            int VRegionHeight = 110;
            //int VRegionWidth = 700;
            int VRegionWidth = this.DrawWidth - 60;
            //int DateTipRegionTop = 655;
            int DateTipRegionLeft = 1;
            int DateTipRegionHeight = 20;
            //int DateTipRegionWidth = 700;
            int DateTipRegionWidth = this.DrawWidth - 60;

            kRect = new Rectangle(KRegionLeft, KRegionTop, KRegionWidth, KRegionHeight);
            vInfoRect = new Rectangle(VInfoRegionLeft, kRect.Bottom + 1, VInfoRegionWidth, VInfoRegionHeight);
            vRect = new Rectangle(VRegionLeft, vInfoRect.Bottom + 1, VRegionWidth, VRegionHeight);
            dateTipRect = new Rectangle(DateTipRegionLeft, vRect.Bottom + 1, DateTipRegionWidth, DateTipRegionHeight);
            crossLineRect = new Rectangle(kRect.Left, kRect.Top, KRect.Width, vInfoRect.Height + vRect.Height + kRect.Height + 1);
            kInfoRect = new Rectangle(kRect.Left, kRect.Top, 100, 230);

            localBitmap = new Bitmap(DrawWidth, DrawHeight);
            g = Graphics.FromImage(localBitmap);
            //g.Clear(Color.Black);

            //绘制K线的Bitmap
            KBitmap = new Bitmap(kRect.Width, kRect.Height);
            KBitmapGraphics = Graphics.FromImage(KBitmap);
            //绘制标记的Bitmap
            MarkBitmap = new Bitmap(kRect.Width, kRect.Height);
            MarkBitmapGraphics = Graphics.FromImage(MarkBitmap);
            //绘制交易量的Bitmap
            VBitmap = new Bitmap(vRect.Width, vRect.Height);
            VBitmapGraphics = Graphics.FromImage(VBitmap);
            //KBitmapGraphics.Clear(Color.Transparent);


            risePen = new Pen(Color.FromArgb(255, 50, 50));
            fallPen = new Pen(Color.FromArgb(84, 252, 252));
            redDotPen = new Pen(Color.Red);
            redDotPen.DashPattern = new float[] { 1, 3 };
            redDotPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            redLinePen = new Pen(Color.Red);
            ma4Pen = new Pen(Color.FromArgb(255, 255, 255));
            ma8Pen = new Pen(Color.FromArgb(255, 255, 11));
            ma12Pen = new Pen(Color.FromArgb(255, 128, 255));
            ma24Pen = new Pen(Color.FromArgb(0, 230, 0));
            redBrush = Brushes.Red;
            greenBrush = Brushes.LimeGreen;
            whiteBrush = Brushes.White;
            fallBrush = new SolidBrush(Color.FromArgb(84,252,252));
            riseBrush = new SolidBrush(Color.FromArgb(255, 50, 50));
            AnalyzeDaybrush = Brushes.Gold;
            MA4HLDayBrush = new SolidBrush(Color.FromArgb(255, 215, 241));
            TrendJudgedBrush = new SolidBrush(Color.FromArgb(254, 130, 210));
            BeginDayLv1Brush = Brushes.DarkOrange;
            EndDayLv1Brush = BeginDayLv1Brush;
            RHDayBrush = Brushes.OrangeRed;
            Debug1Brush = Brushes.MediumOrchid;
            Debug2Brush = MA4HLDayBrush;
            MarkBrush1 = Brushes.Violet;

            //font = new Font(FontFamily.GenericSerif, 10);
            font = new Font("宋体", 9);


            koffset = 15;
            //kbarwidth = 5;
            kbarwidth = 3;
            //kbarblank = 2;
            kbarblank = 1;
            kcounttodraw = (KRegionWidth - koffset) / (kbarwidth + kbarblank);
            if (KRegionWidth - (KCountToDraw * (kbarwidth + kbarblank) + 15) < 10)
            {
                kcounttodraw--;
            }

            DrewKLine = false;

            MarkOriginalBitMaps = new THashTable<Bitmap>();
        }

        public void ResetGraphics()
        {
            g.Clear(Color.Black);
            KBitmapGraphics.Clear(Color.Transparent);
            VBitmapGraphics.Clear(Color.Transparent);
            DrewKLine = false;
            cLinelastPoint = new Point(-1, -1);
            MarkOriginalBitMaps = new THashTable<Bitmap>();
        }
        /// <summary>
        /// 绘制所有框架线和框架文字
        /// </summary>
        public void DrawFrame()
        {
            DrawBaseKPercentlines();
            DrawBaseKPercentLineNumbers();
            DrawBaseFramelines();
        }
        /// <summary>
        /// 绘制k线价格比参考线
        /// </summary>
        private void DrawBaseKPercentlines()
        {

            for (int i = kRect.Top + 48; i <= kRect.Bottom; )
            {
                g.DrawLine(redDotPen, kRect.Left, i, kRect.Right, i);

                i = i + 48;
            }
        }
        /// <summary>
        /// 绘制k线价格比参考线对应的百分比值
        /// </summary>
        private void DrawBaseKPercentLineNumbers()
        {
            g.DrawString("60%", font, redBrush, kRect.Right + 2, kRect.Top - 2);
            g.DrawString("50%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 1 * 48);
            g.DrawString("40%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 2 * 48);
            g.DrawString("30%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 3 * 48);
            g.DrawString("20%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 4 * 48);
            g.DrawString("10%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 5 * 48);
            //g.DrawString("0%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 6 * 40);
            //g.DrawString("0%", font, redBrush, kRect.Right + 2, kRect.Top - 7 + 7 * 32);
            g.DrawString("0%", font, whiteBrush, kRect.Right + 2, kRect.Top - 7 + 6 * 48);
            g.DrawString("10%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 7 * 48);
            g.DrawString("20%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 8 * 48);
            g.DrawString("30%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 9 * 48);
            g.DrawString("40%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 10 * 48);
            g.DrawString("50%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 11 * 48);
            g.DrawString("60%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 12 * 48);
            //g.DrawString("70%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 14 * 40);
            //g.DrawString("80%", font, greenBrush, kRect.Right + 2, kRect.Top - 7 + 16 * 32);
        }

        private void DrawBaseFramelines()
        {
            //左竖线
            g.DrawLine(redLinePen, 0, kRect.Top - 1, 0, vRect.Bottom + 1);
            //右竖线
            g.DrawLine(redLinePen, kRect.Right + 1, kRect.Top - 1, kRect.Right + 1, vRect.Bottom + 1);
            //上横线
            g.DrawLine(redLinePen, 0, kRect.Top - 1, kRect.Right + 1, kRect.Top - 1);
            //k线图区与交易量信息区分割线
            g.DrawLine(redLinePen, 0, kRect.Bottom + 1, kRect.Right + 1, kRect.Bottom + 1);
            //交易量信息区与交易量区分割线
            g.DrawLine(redDotPen, 0, vInfoRect.Bottom + 1, kRect.Right + 1, vInfoRect.Bottom + 1);
            //底边
            g.DrawLine(redLinePen, 0, vRect.Bottom + 1, kRect.Right + 1, vRect.Bottom + 1);

        }
        /// <summary>
        /// 绘制k线图和k线柱背景
        /// </summary>
        /// <param name="KCount">绘制k线柱的个数</param>
        /// <param name="indexb">开始日在k线数据中的索引</param>
        /// <param name="indexe">结束日在k线数据中的索引</param>
        /// <param name="baselineY">基准值纵坐标偏移量，经过偏移量的校正，k线图的主要部分显示在显示区域中央</param>
        /// <param name="openYs">各日开盘价对应的原始y坐标</param>
        /// <param name="closeYs">各日收盘价对应的原始y坐标</param>
        /// <param name="highestYs">各日最高价对应的原始y坐标</param>
        /// <param name="lowestYs">各日最低价对应的原始y坐标</param>
        /// <param name="DType1">各日对应的类型1</param>
        /// <param name="DType2">各日对应的类型2</param>
        public void DrawKline(int KCount, int indexb, int indexe, int baselineY, int[] openYs, int[] closeYs, int[] highestYs, int[] lowestYs, int topVoli, int topVolHeight, bool drawkbarback, DayTypesOfADay[] DType)
        {
            //return;
            //各k线柱值对应的纵坐标值
            int OpenY;
            int CloseY;
            int HighestY;
            int LowestY;
            //左侧柱线的收盘价y坐标值
            int LeftCloseY;
            int volHeight;
            //mavol12的高度
            int mavol12Height=0;
            //前一日mavol12的高度
            int mavol12HeightP=0;
            //交易量包络线上点的高度
            int volenodeHeight = 0;
            //前一个交易量包络线上点的高度
            int volenodeHeightp = 0;
            //绘制前一个交易量包络线上点时的drawcount值
            int drawvolenodecountp = 0;
            int drawcount = 0;
            int ctd = KCount;
            for (int i = indexb; i >= indexe; i--)
            {
                //基准值纵坐标在KBitmap正中间，坐标轴上小下大，价格相对上升时的升幅坐标量为正，价格相对下降的降幅坐标量为负
                //所以需要用基准值纵坐标-升(降)幅坐标量
                ctd--;
                OpenY = openYs[ctd] + baselineY;
                CloseY = closeYs[ctd] + baselineY;
                HighestY = highestYs[ctd] + baselineY;
                LowestY = lowestYs[ctd] + baselineY;
                if (ctd != 0)
                {
                    LeftCloseY = closeYs[ctd - 1] + baselineY;
                }
                else
                {
                    LeftCloseY = 0;
                }
                volHeight = (int)Math.Round(((double)((double)Volume(i) / (double)Volume(topVoli)) * topVolHeight),0);
                //mavol12Height = (int)Math.Round(((double)((double)mavol12(i) / (double)Volume(topVoli)) * topVolHeight), 0);
                drawcount++;
                DrawVolBar(i, drawcount, volHeight);
                if (mavol12HeightP != 0 && mavol12Height != 0)
                {
                    //DrawMAVol(drawcount, mavol12Height, mavol12HeightP);
                }
                mavol12HeightP = mavol12Height;
                ///
                ///交易量包络线未更新，暂时屏蔽
                ///
                ////当日是交易量包络线上的点，计算高度
                //if (isVolENode(i))
                //{
                //    volenodeHeight = (int)Math.Round(((double)((double)Volume(i) / (double)Volume(topVoli)) * topVolHeight), 0);
                //}
                //if (isVolENode(i) && volenodeHeight != 0 && volenodeHeightp == 0)
                //{
                //    volenodeHeightp = volenodeHeight;
                //    drawvolenodecountp = drawcount;
                //}
                ////不是第一个交易量包络线上的点，绘制包络线
                //if (isVolENode(i) && volenodeHeight != 0 && volenodeHeightp!=0)
                //{
                //    DrawVolEnvelope(drawcount, volenodeHeight, drawvolenodecountp, volenodeHeightp);
                //    volenodeHeightp = volenodeHeight;
                //    drawvolenodecountp = drawcount;
                //}


                if (drawkbarback)
                {
                    //DrawKBarBack(DType[ctd], drawcount, OpenY, CloseY);
                    DrawKBarBack(DType[ctd], drawcount, HighestY, LowestY);
                }
                DrawAKBar(i, drawkbarback, DType[ctd], drawcount, OpenY, CloseY, HighestY, LowestY, LeftCloseY);
            }
            g.DrawImage(KBitmap, kRect.Left, kRect.Top);
            g.DrawImage(VBitmap, vRect.Left, vRect.Top);
            DrewKLine = true;
        }
        /// <summary>
        /// 绘制K线柱
        /// </summary>
        /// <param name="indexe">当日在k线数据中的索引</param>
        /// <param name="count"></param>
        /// <param name="OpenY"></param>
        /// <param name="CloseY"></param>
        /// <param name="HighestY"></param>
        /// <param name="LowestY"></param>
        private void DrawAKBar(int indextd, bool drawkbarback, DayTypesOfADay dtype, int count, int OpenY, int CloseY, int HighestY, int LowestY, int LeftCloseY)
        {
            drewNewKLine = true;
            ////标志为未在新k线图上绘制过跟随线
            //drewNewCrossLine = false;
            ////因为k线、均线图位于kInfo之下，所以k线、均线图重绘前需要将kInfo的原始图像重置为null，
            ////绘制kInfo时再将kInfo的原始图像置为包含新k线、均线图的localbitmap的kInfoRect区域部分
            //if (kInfoRectOriginalImage != null)
            //{
            //    kInfoRectOriginalImage = null;
            //}

            //k线柱左侧横坐标起点
            int left;
            //k线柱高度
            int height;
            //k线柱的横坐标中点
            int middle;

            left = kRect.Right - koffset - count * (kbarwidth + kbarblank);
            //middle = left + 2;
            middle = left + 1;

            float leftClose = 0f;
            if(indextd > 0)
            {
                leftClose = Close(indextd - 1);
            }
            //上涨的情况用红色
            //注意价格越高，坐标之越小
            //if (OpenY > CloseY || (OpenY == CloseY && LeftCloseY >= CloseY))
            if (Open(indextd) < Close(indextd) || Open(indextd) == Close(indextd) && leftClose <= Close(indextd))
            {
                height = OpenY - CloseY;
                if (height == 0)
                {
                    height = 1;
                }
                
                
                //如果当日没有指定类型，则上升柱线内部涂黑
                //if (!drawkbarback || dtype == null)
                //{
                    //KBitmapGraphics.FillRectangle(Brushes.Black, left + 1, CloseY + 1, kbarwidth - 2, height - 1);
                KBitmapGraphics.FillRectangle(Brushes.Black, left + 1, CloseY + 1, kbarwidth - 1, height - 1);
                    KBitmapGraphics.DrawRectangle(risePen, left, CloseY, kbarwidth - 1, height);
                //}
                //else
                //{
                    //KBitmapGraphics.FillRectangle(riseBrush, left + 1, CloseY + 1, kbarwidth - 2, height - 1);
                    //为从背景中突出，画黑色线框
                //    KBitmapGraphics.DrawRectangle(Pens.Black, left, CloseY, kbarwidth - 1, height);
                //}

                KBitmapGraphics.DrawLine(risePen, middle, HighestY, middle, CloseY);
                KBitmapGraphics.DrawLine(risePen, middle, LowestY, middle, OpenY);
            }
            else
            {
                height = CloseY - OpenY;
                if (height == 0)
                {
                    height = 1;
                }
                KBitmapGraphics.FillRectangle(fallBrush, left, OpenY, kbarwidth, height);
                KBitmapGraphics.DrawLine(fallPen, middle, HighestY, middle, OpenY);
                KBitmapGraphics.DrawLine(fallPen, middle, LowestY, middle, CloseY);
            }

        }
        /// <summary>
        /// 为需要标记的k线柱绘制背景
        /// </summary>
        /// <param name="dtype1"></param>
        /// <param name="dtype2"></param>
        /// <param name="count"></param>
        /// <param name="OpenY"></param>
        /// <param name="CloseY"></param>
        private void DrawKBarBack(DayTypesOfADay dtype, int count, int HighestY, int LowestY)
        {
            if (dtype == null)
            {
                return;
            }
            //左侧横坐标起点
            int left;
            int top11;
            int top12;
            int width;
            int height;
            int top21 = 0;
            int top22 = 0;
            int height2 = 0;
            int top31 = 0;
            int top32 = 0;
            int height3 = 0;
            Point[] points11=new Point[3];
            Point[] points21=new Point[3];
            Point[] points31=new Point[3];
            Point[] points12 = new Point[3];
            Point[] points22 = new Point[3];
            Point[] points32 = new Point[3];
            Brush abrush = null;
            Brush abrush2 = null;
            Brush abrush3 = null;

            left = kRect.Right - koffset - count * (kbarwidth + kbarblank) -1;
            //width = kbarwidth + 2;
            width = kbarwidth + 1;

            top11 = HighestY - 5;
            points11[0] = new Point(left, top11);
            //points11[1] = new Point(left + kbarwidth + 2, top11);
            points11[1] = new Point(left + kbarwidth + 1, top11);
            points11[2] = new Point(left + kbarwidth / 2 + 1, HighestY);
            top12 = LowestY + 5;
            points12[0] = new Point(left, top12);
            //points12[1] = new Point(left + kbarwidth + 2, top12);
            points12[1] = new Point(left + kbarwidth + 1, top12);
            points12[2] = new Point(left + kbarwidth / 2 + 1, LowestY);
            //if (OpenY >= CloseY)
            //{
            //    top = CloseY - 2;
            //    height = OpenY - CloseY + 5;
            //}
            //else
            //{
            //    top = OpenY - 2;
            //    height = CloseY - OpenY + 5;
            //}

            //选择刷子
            abrush = chooseBrush(dtype[0]);
            if (dtype.Count > 1)
            {
                abrush2 = chooseBrush(dtype[1]);
            }
            if (dtype.Count > 2)
            {
                abrush3 = chooseBrush(dtype[2]);
            }

            //遇到同一天有两种类型的，需要在同一天标出两种颜色
            if (dtype.Count == 2)
            {
                top21 = HighestY - 10;
                points21[0] = new Point(left, top21);
                //points21[1] = new Point(left + kbarwidth + 2, top21);
                points21[1] = new Point(left + kbarwidth + 1, top21);
                points21[2] = new Point(left + kbarwidth / 2 + 1, top11);
                top22 = LowestY + 10;
                points22[0] = new Point(left, top22);
                //points22[1] = new Point(left + kbarwidth + 2, top22);
                points22[1] = new Point(left + kbarwidth + 1, top22);
                points22[2] = new Point(left + kbarwidth / 2 + 1, top12);
                //上半部分绘制第一种颜色，下半部分绘制第二种颜色
                //重新计算第一种颜色的位置和高度
                //top = top - 2;
                //height = (height+4) / 2; 
                //top2 = top + height + 1;
                //height2 = height;

                //绘制第二种
                KBitmapGraphics.FillPolygon(abrush2, points21);
                KBitmapGraphics.FillPolygon(abrush2, points22);
            }

            //遇到同一天有3种类型的，需要在同一天标出3种颜色
            if (dtype.Count >= 3)
            {
                top21 = HighestY - 10;
                points21[0] = new Point(left, top21);
                //points21[1] = new Point(left + kbarwidth + 2, top21);
                points21[1] = new Point(left + kbarwidth + 1, top21);
                points21[2] = new Point(left + kbarwidth / 2 + 1, top11);
                top22 = LowestY + 10;
                points22[0] = new Point(left, top22);
                //points22[1] = new Point(left + kbarwidth + 2, top22);
                points22[1] = new Point(left + kbarwidth + 1, top22);
                points22[2] = new Point(left + kbarwidth / 2 + 1, top12);
                top31 = HighestY - 15;
                points31[0] = new Point(left, top31);
                points31[1] = new Point(left + kbarwidth + 2, top31);
                points31[2] = new Point(left + kbarwidth / 2 + 1, top21);
                top32 = LowestY + 15;
                points32[0] = new Point(left, top32);
                //points32[1] = new Point(left + kbarwidth + 2, top32);
                points32[1] = new Point(left + kbarwidth + 1, top32);
                points32[2] = new Point(left + kbarwidth / 2 + 1, top22);
                //重新计算各种颜色的位置和高度
                //top = top - 3;
                //height = (height + 6) / 3;
                //top2 = top + height + 1;
                //height2 = height;
                //top3 = top2 + height + 1;
                //height3 = height;

                //绘制第二种
                KBitmapGraphics.FillPolygon(abrush2, points21);
                KBitmapGraphics.FillPolygon(abrush3, points31);
                KBitmapGraphics.FillPolygon(abrush2, points22);
                KBitmapGraphics.FillPolygon(abrush3, points32);
                //KBitmapGraphics.FillRectangle(abrush2, left, top2, width, height2);

                //KBitmapGraphics.FillRectangle(abrush3, left, top3, width, height3);
            }
            KBitmapGraphics.FillPolygon(abrush, points11);
            KBitmapGraphics.FillPolygon(abrush, points12);
            //KBitmapGraphics.FillRectangle(abrush, left, top, width, height);
        }

        private void DrawVolBar(int i, int count, int VolHeight)
        {
            //k线柱左侧横坐标起点
            int left;
            //k线柱高度
            int height;

            int top;

            left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
            top = vRect.Height - VolHeight;
            //if (height == 0)
            //{
                //height = 1;
            //}

            float leftClose = 0f;
            if (i > 0)
            {
                leftClose = Close(i - 1);
            }

            if (Open(i) < Close(i) || Open(i) == Close(i) && leftClose <= Close(i))
            {
                VBitmapGraphics.DrawRectangle(risePen, left, top, kbarwidth - 1, VolHeight);
            }
            else
            {
                VBitmapGraphics.FillRectangle(fallBrush, left, top, kbarwidth, VolHeight);
            }

            ////当日k线柱的横坐标中点
            //int middle = 0;
            ////前一天k线柱的横坐标中点
            //int middlep = 0;

            ////ma12Y = vRect.Height - ma12Y;

            ////ma12YP = vRect.Height - ma12YP;

            ////left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
            //middle = left + 2;
            //middlep = middle + kbarwidth + kbarblank;


            //VBitmapGraphics.DrawLine(ma12Pen, middlep, top, middle, top);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="ma12Y">12日交易量移动平均值</param>
        /// <param name="ma12YP">前日的12日交易量移动平均值</param>
        public void DrawMAVol(int count, int ma12Y, int ma12YP)
        {
            //k线柱左侧横坐标起点
            int left;
            //当日k线柱的横坐标中点
            int middle = 0;
            //前一天k线柱的横坐标中点
            int middlep = 0;

            ma12Y = vRect.Height - ma12Y;

            ma12YP = vRect.Height - ma12YP;

            left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
            //middle = left + 2;
            middle = left + 1;
            middlep = middle + kbarwidth + kbarblank;


            VBitmapGraphics.DrawLine(ma8Pen, middlep, ma12YP, middle, ma12Y);
        }

        public void DrawVolEnvelope(int count, int volenodeY, int countp, int volenodeYP)
        {
            //k线柱左侧横坐标起点
            int left;

            int leftp;
            //当日k线柱的横坐标中点
            int middle = 0;
            //前一天k线柱的横坐标中点
            int middlep = 0;

            volenodeY = vRect.Height - volenodeY;

            volenodeYP = vRect.Height - volenodeYP;

            left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
            //middle = left + 2;
            middle = left + 1;
            leftp = vRect.Right - koffset - countp * (kbarwidth + kbarblank);
            //middlep = leftp + 2;
            middlep = leftp + 1;


            VBitmapGraphics.DrawLine(ma8Pen, middlep, volenodeYP, middle, volenodeY);
        }

        /// <summary>
        /// 绘制标志
        /// 暂不使用
        /// </summary>
        /// <param name="i"></param>
        /// <param name="count"></param>
        /// <param name="key"></param>
        /// <param name="p"></param>
        public void DrawMark(int i, int count, object key, Point p)
        {
            lock (g)
            {
                //tmpcount++;
                //if (tmpcount > 1)
                //{
                //    int debug325234 = 1;
                //}
                //鼠标所在k线柱左侧横坐标起点
                int left, width;
                //left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
                left = p.X - (kbarwidth + kbarblank) / 2;
                width = kbarwidth + kbarblank;

                /////
                /////首先擦除当前位置的跟随线，得到原始k线图
                /////
                //int x1, y1, w1, h1;
                //if (cLinelastPoint.X == p.X && cLinelastPoint.Y == p.Y)
                //{
                //    //横线左上角坐标
                //    x1 = crossLineRect.Left;
                //    //y1 = p.Y;
                //    y1 = cLinelastPoint.Y;
                //    w1 = crossLineRect.Width;
                //    h1 = 1;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //    //纵线左上角坐标

                //    x1 = p.X;
                //    y1 = crossLineRect.Top;
                //    w1 = 1;
                //    h1 = crossLineRect.Height;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //}
                //tmpcount--;
                ///
                ///保存欲画标志所在位置的原始k线图
                ///
                MarkOriginalBitMaps.Add(key, new Bitmap(width, kRect.Height));
                Graphics mg = Graphics.FromImage(MarkOriginalBitMaps[key]);
                mg.DrawImage(localBitmap, new Rectangle(0, 0, width, kRect.Height), new Rectangle(left, kRect.Top, width, kRect.Height), GraphicsUnit.Pixel);

                ///
                ///绘制标志
                ///
                g.FillPie(MarkBrush1, new Rectangle(left + kbarblank / 2 - 1, kRect.Bottom - 9, 7, 7), 0, 360);

                //tmpcount++;
                //if (tmpcount > 1)
                //{
                //    int debug325234 = 1;
                //}
                /////
                /////重新绘制跟随线
                /////
                //if (cLinelastPoint.X == p.X && cLinelastPoint.Y == p.Y)
                //{
                //    //横线左上角坐标
                //    x1 = crossLineRect.Left;
                //    //y1 = p.Y;
                //    y1 = cLinelastPoint.Y;
                //    w1 = crossLineRect.Width;
                //    h1 = 1;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //    //纵线左上角坐标

                //    x1 = p.X;
                //    y1 = crossLineRect.Top;
                //    w1 = 1;
                //    h1 = crossLineRect.Height;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //}
                //tmpcount--;
            }
        }

        /// <summary>
        /// 擦出标志
        /// 暂不使用
        /// </summary>
        /// <param name="i"></param>
        /// <param name="count"></param>
        /// <param name="key"></param>
        /// <param name="p"></param>
        public void EraseMark(int i, int count, object key, Point p)
        {
            lock (g)
            {
                //鼠标所在k线柱左侧横坐标起点
                int left, width;
                //left = vRect.Right - koffset - count * (kbarwidth + kbarblank);
                left = p.X - (kbarwidth + kbarblank) / 2;
                width = kbarwidth + kbarblank;

                ///
                ///首先擦除当前位置的跟随线，得到原始k线图
                ///
                //int x1, y1, w1, h1;
                //tmpcount++;
                //if (tmpcount > 1)
                //{
                //    int debug325234 = 1;
                //}
                //if (cLinelastPoint.X == p.X && cLinelastPoint.Y == p.Y)
                //{//横线左上角坐标
                //    x1 = crossLineRect.Left;
                //    //y1 = p.Y;
                //    y1 = cLinelastPoint.Y;
                //    w1 = crossLineRect.Width;
                //    h1 = 1;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //    //纵线左上角坐标

                //    x1 = p.X;
                //    y1 = crossLineRect.Top;
                //    w1 = 1;
                //    h1 = crossLineRect.Height;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //}
                //tmpcount--;
                ///
                ///回复k线图原始区域
                ///
                //MarkOriginalBitMaps.Add(key, new Bitmap(width, kRect.Height));
                Graphics mg = Graphics.FromImage(MarkOriginalBitMaps[key]);
                g.DrawImage(MarkOriginalBitMaps[key], new Rectangle(left, kRect.Top, width, kRect.Height), new Rectangle(0, 0, width, kRect.Height), GraphicsUnit.Pixel);
                MarkOriginalBitMaps.Remove(key);
                tmpcount++;
                if (tmpcount > 1)
                {
                    int debug325234 = 1;
                }
                /////
                /////重新绘制跟随线
                /////
                //if (cLinelastPoint.X == p.X && cLinelastPoint.Y == p.Y)
                //{
                //    //横线左上角坐标
                //    x1 = crossLineRect.Left;
                //    //y1 = p.Y;
                //    y1 = cLinelastPoint.Y;
                //    w1 = crossLineRect.Width;
                //    h1 = 1;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //    //纵线左上角坐标

                //    x1 = p.X;
                //    y1 = crossLineRect.Top;
                //    w1 = 1;
                //    h1 = crossLineRect.Height;
                //    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //}
                //tmpcount--;
            }
        }

        private Brush chooseBrush(DayType daytype)
        {
            switch (daytype)
            {
                case DayType.TrendJudgedF3:
                    return TrendJudgedBrush;
                case DayType.TrendJudgedR3:
                    return TrendJudgedBrush;
                case DayType.MA4HDay:
                    return MA4HLDayBrush;
                case DayType.MA4LDay:
                    return MA4HLDayBrush;
                case DayType.AnalyzeDay:
                    return AnalyzeDaybrush;
                case DayType.BeginDayLv1:
                    return BeginDayLv1Brush;
                case DayType.EndDayLv1:
                    return EndDayLv1Brush;
                case DayType.RHDay:
                    return RHDayBrush;
                case DayType.Debug1:
                    return Debug1Brush;
                case DayType.Debug2:
                    return Debug2Brush;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 绘制均线
        /// </summary>
        /// <param name="KBitmapGraphics"></param>
        /// <param name="count"></param>
        /// <param name="ma4Y"></param>
        /// <param name="ma8Y"></param>
        /// <param name="ma12Y"></param>
        /// <param name="ma4YP"></param>
        /// <param name="ma8YP"></param>
        /// <param name="ma12YP"></param>
        public void DrawMALine(int count, int ma4Y, int ma8Y, int ma12Y, int ma24Y, int ma4YP, int ma8YP, int ma12YP,int ma24YP )
        {
            //k线柱左侧横坐标起点
            int left;
            //当日k线柱的横坐标中点
            int middle = 0;
            //前一天k线柱的横坐标中点
            int middlep = 0;

            left = kRect.Right - koffset - count * (kbarwidth + kbarblank);
            //middle = left + 2;
            middle = left + 1;
            middlep = middle + kbarwidth + kbarblank;

            KBitmapGraphics.DrawLine(ma4Pen, middlep, ma4YP, middle, ma4Y);
            KBitmapGraphics.DrawLine(ma8Pen, middlep, ma8YP, middle, ma8Y);
            KBitmapGraphics.DrawLine(ma12Pen, middlep, ma12YP, middle, ma12Y);
            KBitmapGraphics.DrawLine(ma24Pen, middlep, ma24YP, middle, ma24Y);
        }

        /// <summary>
        /// 绘制跟随线
        /// 暂不使用
        /// </summary>
        /// <param name="p"></param>
        public void DrawCrossLine1(Point p)
        {
                int x1, y1, w1, h1;

                //去掉前一次画的跟随线
                //在新k线图上绘制过跟随线，且绘制在区域内
                if (drewNewCrossLine && cLinelastPoint.X >= 0)
                {
                    x1 = crossLineRect.Left;
                    w1 = crossLineRect.Width;
                    y1 = cLinelastPoint.Y;
                    h1 = 1;
                    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                    //crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));
                    //纵线
                    cLinelastPoint.Y = -1;

                    x1 = cLinelastPoint.X;
                    w1 = 1;
                    y1 = crossLineRect.Top;
                    h1 = crossLineRect.Height;
                    DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                    cLinelastPoint.X = -1;

                    //crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));

                }


                if (p.X < crossLineRect.Left || p.Y < crossLineRect.Top || p.X > crossLineRect.Right || p.Y > crossLineRect.Bottom)
                {
                    cLinelastPoint = new Point(-1, -1);
                    return;
                }

                //横线左上角坐标
                x1 = crossLineRect.Left;
                y1 = p.Y;
                w1 = crossLineRect.Width;
                h1 = 1;
                DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                //纵线左上角坐标

                x1 = p.X;
                y1 = crossLineRect.Top;
                w1 = 1;
                h1 = crossLineRect.Height;
                DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));


                cLinelastPoint = new Point(-1, -1);
                cLinelastPoint = p;
                //标志为已在新k线图上绘制跟随线
                drewNewCrossLine = true;

        }

        /// <summary>
        /// k线信息和交易量信息绘制在localbitmap时的跟随线绘制方法
        /// 暂时无效
        /// </summary>
        /// <param name="p"></param>
        public void DrawCrossLine1_old(Point p)
        {
            //绘制k线图后，FormKLineGraphic会重新描绘Form，使新KBitmap覆盖到localbitmap上，kInfoRectOriginalImage会重置为null
            //在新的localbitmap上重新取得kInfoRectOriginalImage
            //绘制跟随线前需要得到没有跟随线的干净的kInfoRectOriginalImage
            if (kInfoRectOriginalImage == null)
            {
                setkInfoRectOriginalImage();
            }

            //vInfoRect同理
            if (vInfoRectOriginalImage == null)
            {
                setvInfoRectOriginalImage();
            }

            crossLineRegion = new Region(new Rectangle(0, 0, 0, 0));
            
            int x1, y1, w1, h1;
            Region crossline = new Region();

            //去掉前一次画的跟随线
            //在新k线图上绘制过跟随线，且绘制在区域内
            if (drewNewCrossLine && cLinelastPoint.X >= 0)
            {

                //横线
                //前一次的跟随线画在kInfoRect内，并且kInfoRect刚刚更新过，kInfoRect内的跟随线已经被原始画面擦掉
                //NeedEraseCrossLinekInfoRect 为 false时的使用是一次性的，使用完后置为true。只在DrawkInfo方法中kInfoRect刚更新时置为false
                //只擦除kInfoRect外面的部分
                if (cLinelastPoint.Y < kInfoRect.Bottom && !NeedEraseCrossLinekInfoRect)
                {
                    x1 = kInfoRect.Right;
                    w1 = crossLineRect.Width - kInfoRect.Right + kRect.Left;
                }
                //前一次的跟随线没有画在kInfoRect内的部分，或者有画在kInfoRect内的部分且标记为需要擦除
                //擦除整条线
                else
                {
                    x1 = crossLineRect.Left;
                    w1 = crossLineRect.Width;
                }
                y1 = cLinelastPoint.Y;
                h1 = 1;
                DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));
                //纵线
                x1 = cLinelastPoint.X;
                w1 = 1;
                //前一次的跟随线画在kInfoRect内，并且kInfoRect刚刚更新过，kInfoRect内的跟随线已经被原始画面擦掉
                //NeedEraseCrossLinekInfoRect 为 false时的使用是一次性的，使用完后置为true。只在DrawkInfo方法中kInfoRect刚更新时置为false
                //只擦除kInfoRect外面的部分
                if (cLinelastPoint.X < kInfoRect.Right && !NeedEraseCrossLinekInfoRect)
                {
                    y1 = kInfoRect.Bottom;
                    h1 = crossLineRect.Height - kInfoRect.Bottom + kRect.Top;
                }
                //前一次的跟随线没有画在kInfoRect内的部分，或者有画在kInfoRect内的部分且标记为需要擦除
                //擦除整条线
                else
                {
                    y1 = crossLineRect.Top;
                    h1 = crossLineRect.Height;
                }
                DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
                crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));

            }
            //NeedEraseCrossLinekInfoRect 为 false时的使用是一次性的，使用完后置为true。只在DrawkInfo方法中kInfoRect刚更新时置为false
            //因为kInfoRect的一次更新后不一定立刻再次更新，所以将kInfoRect标记为需要擦除内部跟随线，
            //以便下次需要擦除跟随线且没有更新kInfoRect区域时，可以顺利擦除前一次绘制的跟随线的完整部分
            NeedEraseCrossLinekInfoRect = true;

            cLinelastPoint = new Point(-1, -1);
            if (p.X < crossLineRect.Left || p.Y < crossLineRect.Top || p.X > crossLineRect.Right || p.Y > crossLineRect.Bottom) return;

            //横线左上角坐标
            x1 = crossLineRect.Left;
            y1 = p.Y;
            w1 = crossLineRect.Width;
            h1 = 1;
            DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
            crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));
            //纵线左上角坐标
            x1 = p.X;
            y1 = crossLineRect.Top;
            w1 = 1;
            h1 = crossLineRect.Height;
            DrawAReversibleLine(new Rectangle(x1, y1, w1, h1));
            crossLineRegion.Exclude(new Rectangle(x1, y1, w1, h1));

            cLinelastPoint = p;
            //标志为已在新k线图上绘制跟随线
            drewNewCrossLine = true;

        }

        public void DrawAReversibleLine(Rectangle rect)
        {
            BitmapData xline = localBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            // 不安全代码  
            {// 像素首地址
                byte* pix = (byte*)xline.Scan0;
                //对每一个点颜色取反  
                for (int i = 0; i < xline.Stride * xline.Height; i++) pix[i] = (byte)(255 - pix[i]);
            }
            localBitmap.UnlockBits(xline);
        }

        /// <summary>
        /// 绘制k线信息
        /// 暂时无效
        /// </summary>
        /// <param name="info"></param>
        public void DrawkInfo(string info)
        {
            //绘制k线图后，FormKLineGraphic会重新描绘Form，使新KBitmap覆盖到localbitmap上，kInfoRectOriginalImage会重置为null
            //在新的localbitmap上重新取得kInfoRectOriginalImage
            if (kInfoRectOriginalImage == null)
            {
                setkInfoRectOriginalImage();
            }
            //return;
            //已经用原始图像覆盖kInfoRect区域，跟随线被抹掉，所以在紧接着下一次绘制跟随线时不需要擦除上一次的跟随线(对上一次跟随线颜色取反)
            NeedEraseCrossLinekInfoRect = false;
            if (kInfoRectOriginalImage == null)
            {
                return;
            }
            g.DrawImage(kInfoRectOriginalImage, kRect.Left, kRect.Top);
            g.DrawString(info, font, whiteBrush, kRect.Left, KRect.Top);
        }

        /// <summary>
        /// 绘制交易量信息
        /// 暂时无效
        /// </summary>
        /// <param name="info"></param>
        public void DrawvInfo(string info)
        {
            //绘制k线图后，FormKLineGraphic会重新描绘Form，使新KBitmap覆盖到localbitmap上，kInfoRectOriginalImage会重置为null
            //在新的localbitmap上重新取得kInfoRectOriginalImage
            if (vInfoRectOriginalImage == null)
            {
                setvInfoRectOriginalImage();
            }
            //return;
            //已经用原始图像覆盖kInfoRect区域，跟随线被抹掉，所以在紧接着下一次绘制跟随线时不需要擦除上一次的跟随线(对上一次跟随线颜色取反)
            NeedEraseCrossLinevInfoRect = false;
            if (vInfoRectOriginalImage == null)
            {
                return;
            }
            g.DrawImage(vInfoRectOriginalImage, vInfoRect.Left, vInfoRect.Top);
            g.FillRectangle(Brushes.Black, vInfoRect.Left + 1, vInfoRect.Top + 1, vInfoRect.Width - 2, vInfoRect.Height - 1);
            g.DrawString(info, font, whiteBrush, vInfoRect.Left, vInfoRect.Top + 5);
        }

        /// <summary>
        /// 将原始图像绘制回localBitmap的kInfoRect区域
        /// 暂时无效
        /// </summary>
        private void setkInfoRectOriginalImage()
        {
            //未绘制k线图
            if (!DrewKLine)
            {
                return;
            }
            //得到localBitmap中kInfoRect区域的BitmapData
            BitmapData localBitmapbd = localBitmap.LockBits(kInfoRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //得到首指针
            IntPtr ptr1 = localBitmapbd.Scan0;
            //localBitmapbd的数据大小
            int bytes = kInfoRect.Width * kInfoRect.Height * 3;
            //临时存放kInfoRect区域原始图像的数据
            Byte[] tmpBytes = new Byte[bytes];
            //拷贝数据到tmpBytes
            System.Runtime.InteropServices.Marshal.Copy(ptr1, tmpBytes, 0, bytes);

            kInfoRectOriginalImage = new Bitmap(kInfoRect.Width, kInfoRect.Height);
            //得到kInfoRectOriginalImage的BitmapData，准备写入
            BitmapData kInfoRectbd = kInfoRectOriginalImage.LockBits(new Rectangle(0, 0, kInfoRect.Width, kInfoRect.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //得到首指针
            IntPtr ptr2 = kInfoRectbd.Scan0;
            //从tmpBytes拷贝到kInfoRectOriginalImage的BitmapData
            System.Runtime.InteropServices.Marshal.Copy(tmpBytes, 0, ptr2, bytes);
            //unlock kInfoRectOriginalImage的BitmapData
            kInfoRectOriginalImage.UnlockBits(kInfoRectbd);
            //unlock localBitmap中kInfoRect区域的BitmapData
            localBitmap.UnlockBits(localBitmapbd);
        }

        /// <summary>
        /// 将原始图像绘制回localBitmap的vInfoRect区域
        /// 暂时无效
        /// </summary>
        private void setvInfoRectOriginalImage()
        {
            //未绘制k线图
            if (!DrewKLine)
            {
                return;
            }
            //得到localBitmap中vInfoRect区域的BitmapData
            BitmapData localBitmapbd = localBitmap.LockBits(vInfoRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //得到首指针
            IntPtr ptr1 = localBitmapbd.Scan0;
            //localBitmapbd的数据大小
            int bytes = vInfoRect.Width * vInfoRect.Height * 3;
            //临时存放vInfoRect区域原始图像的数据
            Byte[] tmpBytes = new Byte[bytes];
            //拷贝数据到tmpBytes
            System.Runtime.InteropServices.Marshal.Copy(ptr1, tmpBytes, 0, bytes);

            vInfoRectOriginalImage = new Bitmap(vInfoRect.Width, vInfoRect.Height);
            //得到vInfoRectOriginalImage的BitmapData，准备写入
            BitmapData vInfoRectbd = vInfoRectOriginalImage.LockBits(new Rectangle(0, 0, vInfoRect.Width, vInfoRect.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //得到首指针
            IntPtr ptr2 = vInfoRectbd.Scan0;
            //从tmpBytes拷贝到vInfoRectOriginalImage的BitmapData
            System.Runtime.InteropServices.Marshal.Copy(tmpBytes, 0, ptr2, bytes);
            //unlock vInfoRectOriginalImage的BitmapData
            vInfoRectOriginalImage.UnlockBits(vInfoRectbd);
            //unlock localBitmap中vInfoRect区域的BitmapData
            localBitmap.UnlockBits(localBitmapbd);
        }
    }
}
