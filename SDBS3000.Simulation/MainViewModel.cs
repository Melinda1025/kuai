using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using SDBS3000.Simulation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SDBS3000.Simulation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private PlotModel model;        
        public LinearAxis XAxis, YAxis;

        public List<CircleSeries> CircleSeries { get; set; } = new();

        private int currentAngle = 0;
        private DispatcherTimer renderTimer;

        public MainViewModel()
        {
            Model = new PlotModel();
            //Model.PlotMargins = new OxyThickness(50);
            //Model.PlotAreaBorderThickness = new OxyThickness(0);
            Model.PlotType = PlotType.XY;
            XAxis = new LinearAxis() { Minimum = -2, Maximum = 2, Position = AxisPosition.Bottom };
            YAxis = new LinearAxis() { Minimum = -2, Maximum = 2, Position = AxisPosition.Left };
            Model.Axes.Add(XAxis);
            Model.Axes.Add(YAxis);
            Model.ResetAllAxes();

            InitCircles();

            renderTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1.0 / 60)
            };
            renderTimer.Tick += OnRender;
            renderTimer.Start();
        }

        public void InitCircles()
        {
            //var stdCircle = new CircleSeries(new Circle());
            //CircleSeries.Add(stdCircle);

            var unBalenceCircle = new CircleSeries(new Circle()
            {
                UnBalenceDict = new Dictionary<int, double>()
                {
                    {0, 0.1},
                    {30, -0.2},                    
                },
                Center = new DataPoint(0.1, 0.1)
            });
            CircleSeries.Add(unBalenceCircle);

            foreach (var series in CircleSeries)
            {
                Model.Series.Add(series);
            }
        }

        private void OnRender(object sender, EventArgs e)
        {         
            renderTimer.Stop();
            foreach(var series in CircleSeries)
            {
                series.Update(currentAngle);
            }
            Model.InvalidatePlot(true);
            currentAngle += 5;
            if (currentAngle > 360) currentAngle -= 360;
            renderTimer.Start();
        }
    }
}
