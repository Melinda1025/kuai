using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyCompensateInfo
    {
        public float Speed { get; set; }
        public float Panel1Value { get; set; }
        public float Panel1Angle { get; set; }
        public float Panel2Value { get; set; }
        public float Panel2Angle { get; set; }
        public float StaticValue { get; set; }
        public float StaticAngle { get; set; }
        public short Panel1Signal { get; set; }
        public short Panel2Signal { get; set; }
        public short StaticSignal { get; set; }
        //public short Progress { get; set; }
    }
}
