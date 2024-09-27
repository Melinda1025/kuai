using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 单个面的跳动测量信息
    /// </summary>
    [SugarTable("JumpItem")]
    public class JumpItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 跳动
        /// </summary>
        public double Jumping { get; set; }
        /// <summary>
        /// 高点角度
        /// </summary>
        public double HighAngle { get; set; }
        /// <summary>
        /// 低点角度
        /// </summary>
        public double LowAngle { get; set; }
        /// <summary>
        /// 基波
        /// </summary>
        public double FirstHarmonic { get; set; }
        /// <summary>
        /// 二次谐波
        /// </summary>
        public double SecondHarmonic { get; set; }
        /// <summary>
        /// 表面类型
        /// </summary>
        public SurfaceType SurfaceType { get; set; }
        public int ResultId { get; set; }
    }

    public enum SurfaceType
    {
        /// <summary>
        /// 上径面
        /// </summary>
        UpDiametricSurface,
        /// <summary>
        /// 下径面
        /// </summary>
        DownDiametricSurface,
        /// <summary>
        /// 上端面
        /// </summary>
        UpEndSurface,
        /// <summary>
        /// 上端面
        /// </summary>
        DownEndSurface,
    }
}
