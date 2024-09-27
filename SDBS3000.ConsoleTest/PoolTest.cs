using BenchmarkDotNet.Attributes;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ConsoleTest
{
    [MemoryDiagnoser]
    public class PoolTest
    {
        private float[] values = new float[10000];
        [GlobalSetup]
        public void Init()
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Random.Shared.NextSingle();
            }
        }

        [Benchmark(Description = "No Pool")]
        public void NoPool()
        {
            var samples = new Complex32[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                samples[i] = new Complex32(values[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);            
        }

        [Benchmark(Description = "Pool")]
        public void Pool()
        {
            var samples = ArrayPool<Complex32>.Shared.Rent(values.Length);
            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    samples[i] = new Complex32(values[i], 0);
                }
                Fourier.Forward(samples, FourierOptions.Matlab);
            }
            finally
            {
                ArrayPool<Complex32>.Shared.Return(samples);
            }
        }
    }
}
