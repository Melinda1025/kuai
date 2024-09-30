using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models.PlcStructs
{
    public struct BalenceData
    {
        public float Panel1Value { get; set; }
        public float Panel1Angle { get; set; }
        public float Panel2Value { get; set; }
        public float Panel2Angle { get; set; }
        public short Panel1Signal { get; set; }
        public short Panel2Signal { get; set; }

        public static async Task<BalenceData> ReadFromPlcAsync(Plc plc)
        {
            var buffer = new byte[26];
            await plc.ReadBytesAsync(buffer, DataType.DataBlock, 20, 404);            
            var data = new BalenceData();
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer, 0, 4);
                Array.Reverse(buffer, 4, 4);
                Array.Reverse(buffer, 8, 4);
                Array.Reverse(buffer, 12, 4);
                Array.Reverse(buffer, 24, 2);
                Array.Reverse(buffer, 26, 2);
            }
            data.Panel1Value = BitConverter.ToSingle(buffer, 0);
            data.Panel1Angle = BitConverter.ToSingle(buffer, 4);
            data.Panel2Value = BitConverter.ToSingle(buffer, 8);
            data.Panel2Angle = BitConverter.ToSingle(buffer, 12);
            data.Panel1Signal = BitConverter.ToInt16(buffer, 16);
            data.Panel2Signal = BitConverter.ToInt16(buffer, 18);

            return data;
        }        
    }
}
