using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Extensions
{
    public static class DataPointExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataPoint PolorToCartesian(this DataPoint p)
        {
            return new DataPoint(p.X * Math.Cos(p.Y / 180 * Math.PI), p.X * Math.Sin(p.Y / 180 * Math.PI));
        }

        public static bool Between(this DataPoint p, DataPoint p1, DataPoint p2)
        {
            if(p1.X < p2.X)
            {
                if(p.X < p1.X || p.X > p2.X)
                {
                    return false;
                }            
            }
            else
            {
                if(p.X > p1.X || p.X < p2.X)
                {
                    return false;
                }
            }

            if(p1.Y < p2.Y)
            {
                if(p.Y < p1.Y || p.Y > p2.Y)
                {
                    return false;
                }
            }
            else
            {
                if(p.Y > p1.Y || p.Y < p2.Y)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
