using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 夹具补偿数据
    /// </summary>
    public class ClampCompensatingItem
    {
        private double[] values;
        public double[] Values
        {
            get => values;
            init
            {
                values = value;
                var xArr = new double[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    xArr[i] = (double)i;
                }
                interpolation = Interpolate.CubicSpline(xArr, values);
            }
        }
        private IInterpolation interpolation;
        public ClampCompensatingItem(double[] values)
        {
            Values = values;            
        }

        /// <summary>
        /// 补偿误差
        /// </summary>
        /// <param name="datas"></param>
        public void Compensate(double[] datas)
        {
            var step = (double)Values.Length / datas.Length;
            for(int i = 0; i < datas.Length; i++)
            {
                datas[i] -= interpolation.Interpolate(i * step);
            }
        }
        /// <summary>
        /// 补偿误差
        /// </summary>
        /// <param name="datas"></param>
        public void Compensate(List<double> datas)
        {
            if(datas.Count == 0) return;
            var step = (double)Values.Length / datas.Count;
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i] -= interpolation.Interpolate(i * step);
            }
        }
    }
}
