using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 测量结果
    /// </summary>
    [SugarTable("MeasureResult")]
    public class MeasureResult
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id {  get; set; }        
        public int RotorId { get; set; }
        public DateTime TimeStamp { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(RotorInfo.Id), nameof(MeasureResult.RotorId))]
        public RotorInfo RotorInfo { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(BalanceItem.Id), nameof(MeasureResult.Id))]
        public List<BalanceItem> BalanceItems { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(JumpItem.Id), nameof(MeasureResult.Id))]
        public List<JumpItem> JumpItems { get; set; }
    }
}
