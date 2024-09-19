using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 切换页面信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PageInfo
    {
        /// <summary>
        /// 开机界面
        /// </summary>
        public bool P1 { get; set; }
        /// <summary>
        /// 双面动平衡
        /// </summary>
        public bool Measure { get; set; }
        /// <summary>
        /// 静平衡
        /// </summary>
        public bool P3 { get; set; }
        /// <summary>
        /// 动静平衡
        /// </summary>
        public bool P4 { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public bool Rotor { get; set; }
        /// <summary>
        /// 定位
        /// </summary>
        public bool Positioning { get; set; }
        /// <summary>
        /// 补偿
        /// </summary>
        public bool Compensation { get; set; }
        /// <summary>
        /// 标定
        /// </summary>
        public bool P8 { get; set; }

        public bool P9 { get; set; }

        public bool P10 { get; set; }
        /// <summary>
        /// 单步
        /// </summary>        
        public bool SingleStep { get; set; }
        /// <summary>
        /// 报警
        /// </summary>        
        public bool P12 { get; set; }
        /// <summary>
        /// 记录
        /// </summary>
        public bool P13 { get; set; }
        /// <summary>
        /// 设置
        /// </summary>
        public bool P14 { get; set; }
        public bool P15 { get; set; }
        public bool P16 { get; set; }
    }
}
