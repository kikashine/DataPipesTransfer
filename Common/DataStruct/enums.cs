using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace StockToolKit.Common
{
    public enum DBInfoType
    {
        /// <summary>
        /// 日K线数据
        /// </summary>
        KDay,
        /// <summary>
        /// 股票权息、股本信息
        /// </summary>
        RCInfo,
        /// <summary>
        /// 权息中分红部分
        /// </summary>
        FH,
        /// <summary>
        /// 权息中配股部分
        /// </summary>
        PG,
        /// <summary>
        /// 检查新日线数据
        /// </summary>
        CheckNewKDay

    }

    /// <summary>
    /// 股票权息、股本结构数据导入工作核心的错误类型
    /// </summary>
    public enum JobCoreStockDROErrType
    {
        /// <summary>
        /// 连接错误(direct连接)
        /// </summary>
        DirectLinkNetworkErr,

        /// <summary>
        /// 连接错误（Proxy连接）
        /// </summary>
        ProxyLinkNetworkErr,

        /// <summary>
        /// 目标服务器状态异常(direct连接)
        /// </summary>
        DirectLinkNServerErr,

        /// <summary>
        /// 目标服务器状态异常（Proxy连接）
        /// </summary>
        ProxyLinkServerErr,

        /// <summary>
        /// 分红数据错误(direct连接)
        /// </summary>
        DirectLinkDVErr,

        /// <summary>
        /// 分红数据错误（Proxy连接）
        /// </summary>
        ProxyLinkDVErr,

        /// <summary>
        /// 配股数据错误(direct连接)
        /// </summary>
        DirectLinkRSErr,

        /// <summary>
        ///配股数据错误（Proxy连接）
        /// </summary>
        ProxyLinkRSErr,

        /// <summary>
        /// 股本结构数据错误(direct连接)
        /// </summary>
        DirectLinkOSErr,

        /// <summary>
        /// 股本结构数据错误（Proxy连接）
        /// </summary>
        ProxyLinkOSErr,

        /// <summary>
        /// 数据库错误
        /// </summary>
        DBErr
    }
    /// <summary>
    /// k线片段的趋势
    /// </summary>
    [Serializable]
    public enum Trend
    {
        /// <summary>
        /// 上升
        /// </summary>
        Rise,
        /// <summary>
        /// 持平
        /// </summary>
        Flat,
        /// <summary>
        /// 下降
        /// </summary>
        Fall,

        Null
    }

    public enum KDayDataFQType
    {
        /// <summary>
        /// 向前复权
        /// </summary>
        FA,
        /// <summary>
        /// 除权
        /// </summary>
        DA,
        /// <summary>
        /// 向后复权
        /// </summary>
        BA
    }

    public enum PieceType
    {
        MA,
        K
    }
}
