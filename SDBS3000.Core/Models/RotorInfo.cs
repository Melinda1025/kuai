using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace SDBS3000.Core.Models
{
    [SugarTable("RotorInfo")]
    public class RotorInfo : IEquatable<RotorInfo>
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int SizeUnit { get; set; } = -1;
        public int BalenceUnit { get; set; } = -1;

        [SugarColumn(IsNullable = false)]
        public float? Panel1MaxValue { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? Panel2MaxValue { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? StaticMaxValue { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? A { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? B { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? C { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? R1 { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? R2 { get; set; }

        [SugarColumn(IsNullable = false)]
        public float? Speed { get; set; }

        public bool Equals(RotorInfo other)
        {
            if (other == null) return false;            
            return Name == other.Name
                && SizeUnit == other.SizeUnit
                && BalenceUnit == other.BalenceUnit
                && Panel1MaxValue == other.Panel1MaxValue
                && Panel2MaxValue == other.Panel2MaxValue
                && StaticMaxValue == other.StaticMaxValue
                && A == other.A
                && B == other.B
                && C == other.C
                && R1 == other.R1
                && R2 == other.R2;
        }
    }
}
