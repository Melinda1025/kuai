// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Running;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Interpolation;
using S7.Net.Types;
using SDBS3000.ConsoleTest;
using SDBS3000.Core.Utils;

//var signal = new SimulationSignal(100);
//var samples = signal.TakeSamples(1, 10000);
//var r = RotorCaculator.Calculate(samples, 100, 1000);
//var datas = ArrayPool<byte>.Shared.Rent(1000);
//BenchmarkRunner.Run<PoolTest>();
//var t = new PoolTest();
//t.Init();
//t.Pool();
var arr = ArrayPool<float>.Shared.Rent(1000);
await Task.Run(async () => 
{
    for (int i = 0; i < arr.Length; i++)
    {
        arr[i] = i;
    }
    await Task.Delay(1000).ConfigureAwait(false);
}).ConfigureAwait(false);
Console.WriteLine(arr.Length);
Console.ReadLine();

public class SimulationSignal
{
    public float Frequency { get; init; }
    private IInterpolation interpolation;
    public SimulationSignal(float frequency)
    {
        Frequency = frequency;
        var signals = new double[(int)frequency];        
        var xArr = new double[signals.Length];
        for(int i = 0; i < signals.Length; i++)
        {
            if(Random.Shared.NextDouble() > 0.9)
            {
                signals[i] = 1.1 - Random.Shared.NextDouble() * 0.2;
            }
            else
            {
                signals[i] = 1;
            }
            xArr[i] = i / frequency;
        }
        interpolation = Interpolate.CubicSpline(xArr, signals);        
    }

    public float[] TakeSamples(float duration, int count)
    {
        float delta = duration / count;
        var samples = new float[count];
        for (int i = 0; i < count; i++)
        {
            var v = i * delta;
            samples[i] = (float)interpolation.Interpolate(v - (int)v);
        }
        return samples;
    }
}

public class RotorCaculator
{
    public static (double first, double second) Calculate(float[] datas, float signalFrequency, float sampleFrequency)
    {
        var samples = ArrayPool<Complex32>.Shared.Rent(datas.Length);
        try
        {
            for (int i = 0; i < datas.Length; i++)
            {
                samples[i] = new Complex32(datas[i], 0);
            }
            Fourier.Forward(samples, FourierOptions.Matlab);
            var first = samples[(int)(signalFrequency / sampleFrequency * signalFrequency)].Magnitude / datas.Length * 2;
            var second = samples[(int)(signalFrequency / sampleFrequency * signalFrequency * 2)].Magnitude / datas.Length * 2;
            return (first, second);
        }
        finally
        {
            ArrayPool<Complex32>.Shared.Return(samples);
        }
    }    
}
