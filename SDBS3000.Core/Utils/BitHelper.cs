using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Utils
{
    public static class BitHelper
    {
        public static bool[] ByteToBoolArray(byte b)
        {
            bool[] boolArray = new bool[8];

            for (int i = 7; i >= 0; i--)
            {
                boolArray[i] = (b & 1) == 1;
                b = (byte)(b >> 1);
            }

            return boolArray;
        }

        public static void BoolArrayToByte(out byte switchr, params bool[] values)
        {
            switchr = 0;
            if (values[0]) switchr |= 0x01;
            if (values[1]) switchr |= 0x01 << 1;
            if (values[2]) switchr |= 0x01 << 2;
            if (values[3]) switchr |= 0x01 << 3;
            if (values[4]) switchr |= 0x01 << 4;
            if (values[5]) switchr |= 0x01 << 5;
            if (values[6]) switchr |= 0x01 << 6;
            if (values[7]) switchr |= 0x01 << 7;
        }
    }
}
