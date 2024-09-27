using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SDBS3000.Core.Utils
{
    public static class Algorithm
    {        
        /// <summary>
        /// 获取基波和一次谐波
        /// </summary>
        /// <param name="datas">采样点数据</param>
        /// <param name="signalFrequency">原始信号频率</param>
        /// <param name="samplingRate">采样频率</param>
        /// <returns></returns>
        public static (double first, double second) GetTwoHarmonics(List<double> datas)
        {
            var span = CollectionsMarshal.AsSpan(datas);
            var samples = new Complex32[span.Length];
            for (int i = 0; i < span.Length; i++)
            {
                samples[i] = new Complex32((float)span[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);            
            var magnitudes = new float[samples.Length / 2];
            for (int i = 0; i < magnitudes.Length; i++)
            {
                magnitudes[i] = samples[i].Magnitude;
            }

            var (fidx, sidx) = GetTwoMax(magnitudes);
            var first = magnitudes[fidx] / span.Length * 2;
            var second = magnitudes[sidx] / span.Length * 2;
            return (first, second);
        }

        /// <summary>
        /// 获取最大值和最小值索引
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static (int max, int min) FindMaxAndMin(double[] datas)
        {
            int max = 0, min = 0;
            for(int i = 1; i < datas.Length; i++)
            {
                if (datas[i] > datas[max]) max = i;
                if (datas[i] < datas[min]) min = i;
            }
            return (max, min);
        }

        /// <summary>
        /// 获取最大值和最小值索引
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static (int max, int min) FindMaxAndMin(List<double> datas)
        {
            var span = CollectionsMarshal.AsSpan(datas);
            int max = 0, min = 0;
            for (int i = 1; i < span.Length; i++)
            {
                if (span[i] > span[max]) max = i;
                if (span[i] < span[min]) min = i;
            }
            return (max, min);
        }

        public static (int first, int second) GetTwoMax(Span<float> values)
        {
            int first, second;
            if (values[0] > values[1])
            {
                first = 0;
                second = 1;
            }
            else
            {
                first = 1;
                second = 0;
            }

            for (int i = 2; i < values.Length; i++)
            {
                if(values[i] > values[first])
                {
                    second = first;
                    first = i;
                }
                else if(values[i] > values[second])
                {
                    second = i;
                }
            }
            return (first, second);
        }
    }
}
