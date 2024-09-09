using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Simulation.Data
{
    public class CircleSeries : LineSeries
    {
        public Circle Circle { get; init; }

        public CircleSeries(Circle circle)
        {
            Circle = circle;
            this.MarkerType = MarkerType.None;
            TrackerFormatString = string.Empty;
        } 

        public void Update(int angle)
        {
            Circle.GetCurrentPoints(angle, this.Points);
        }
    }
}
