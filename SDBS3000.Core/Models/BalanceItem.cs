using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    public class BalanceItem
    {
        public short Signal { get; set; }
        public float Value { get; set; }
        public float Angle { get; set; }
        public float LastValue { get; set; }
        public float LastAngle { get; set; }
    }
}
