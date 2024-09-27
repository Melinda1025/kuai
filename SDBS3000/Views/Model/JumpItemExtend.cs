using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using SDBS3000.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;

namespace SDBS3000.Views.Model
{
    public class JumpItemExtend : JumpItem
    {
        public DataPointList Points { get; private set; } = new();
        public List<double> Buffer { get; private set; } = new();
        public bool? IsQualified { get; set; } = null;

        public void ConvertCurrentDatas(ReadOnlySpan<double> currentDatas)
        {
            for (int i = 0; i < currentDatas.Length; i++)
            {
                Buffer.Add((currentDatas[i] - 0.004) * 312.5);
            }
        }

        public void Calculate()
        {
            if(Buffer.Count == 0) return;
            //计算谐波
            var (first, second) = Algorithm.GetTwoHarmonics(Buffer);
            FirstHarmonic = first;
            SecondHarmonic = second;
            var (max, min) = Algorithm.FindMaxAndMin(Buffer);
            HighAngle = 360.0 / Buffer.Count * max;
            LowAngle = 360.0 / Buffer.Count * min;

            //计算显示跳动的圆的半径
            //1、计算最大值与最小值的差值
            var delta = Buffer[max] - Buffer[min];
            Jumping = delta;
            //2、计算圆的半径
            Points.Clear();
            const double minRadius = 2.5;
            var radius = Math.Max(minRadius, delta * 2.5);

            var step = 360.0 / Buffer.Count;
            for (int i = 0; i < Buffer.Count; i++)
            {
                Points.Add(new DataPoint(Buffer[i] + radius, step * i));
            }
            Points.NotifyChanged();
        }

        public void Calculate(double maxBalenceValue)
        {
            Calculate();
            IsQualified = Jumping <= maxBalenceValue;
        }

        public void Reset()
        {
            Points.Clear();
            Buffer.Clear();
            Jumping = HighAngle = LowAngle = FirstHarmonic = SecondHarmonic = 0.0;
            IsQualified = null;
        }

        public string TraceInfo()
        {
            return $"高点角度：{HighAngle:f3}°\r\n低点角度：{LowAngle:f3}°\r\n一次谐波：{FirstHarmonic:f3}\r\n二次谐波：{SecondHarmonic:f3}";
        }
    }
}
