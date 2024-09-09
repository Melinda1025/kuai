using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace SDBS3000.Simulation.Data
{
    public class Circle
    {
        /// <summary>
        /// 理想半径
        /// </summary>
        public double StdRadius { get; set; } = 1;

        /// <summary>
        /// 夹具中心
        /// </summary>
        public DataPoint Center { get; set; } = new DataPoint(0, 0);

        /// <summary>
        /// 不平衡点集 (角度, 不平衡量)
        /// </summary>
        public Dictionary<int, double> UnBalenceDict { get; set; } = new();

        public void GetCurrentPoints(int startAngle, List<DataPoint> points)
        {
            points.Clear();
            
            for (int angle = 0; angle < 360; angle += 5)
            {
                double r = StdRadius;
                int current = startAngle + angle;
                if (UnBalenceDict.TryGetValue(angle, out double value))
                {
                    r += value;
                }
                var x1 = r * Math.Cos(current * Math.PI / 180);
                var y1 = r * Math.Sin(current * Math.PI / 180);
                //绕center旋转后的点
                var x2 = x1 * Math.Cos(Center.X * Math.PI / 180) - y1 * Math.Sin(Center.X * Math.PI / 180) + Center.X;
                var y2 = x1 * Math.Sin(Center.Y * Math.PI / 180) + y1 * Math.Cos(Center.Y * Math.PI / 180) + Center.Y;
                points.Add(new DataPoint(x2, y2));            
            }
            points.Add(new DataPoint(points[0].X, points[0].Y));
        }
    }
}
