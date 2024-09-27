using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 单个面的动平衡测量信息
    /// </summary>
    [SugarTable("BalanceItem")]
    public class BalanceItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 信号
        /// </summary>
        public short Signal { get; set; }
        /// <summary>
        /// 量值
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
        public float Angle { get; set; }
        [SugarColumn(IsIgnore = true)]
        public float LastValue { get; set; }
        [SugarColumn(IsIgnore = true)]
        public float LastAngle { get; set; }
        /// <summary>
        /// 光针角度
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public float NeedleAngle { get; set; } = -1;
        public PanelType PanelType { get; set; }
        public int ResultId { get; set; }
    }

    public enum PanelType
    {
        Panel1,
        Panel2
    }
}
