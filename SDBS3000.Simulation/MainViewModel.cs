using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SDBS3000.Core.Models;
using SDBS3000.Simulation.Data;

namespace SDBS3000.Simulation
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private PlotModel model;
        public LinearAxis XAxis,
            YAxis;

        public LineSeries OriginSeries;
        private double[] errorData,
            expectData,
            actualData;
        private const int SAMPLE_COUNT = 720;

        public MainViewModel()
        {
            Model = new PlotModel();
            Model.DefaultFont = "Microsoft YaHei";
            Model.PlotType = PlotType.XY;
            XAxis = new LinearAxis() { Position = AxisPosition.Bottom };
            YAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = -20,
                Maximum = 20,
            };
            Model.Axes.Add(XAxis);
            Model.Axes.Add(YAxis);
            Model.Legends.Add(
                new Legend
                {
                    LegendPosition = LegendPosition.RightTop,
                    LegendOrientation = LegendOrientation.Vertical,
                }
            );
            Init();
            AddProcessSeries();
            Model.InvalidatePlot(true);
        }

        public void Init()
        {
            errorData = new double[SAMPLE_COUNT];
            expectData = new double[SAMPLE_COUNT];
            actualData = new double[SAMPLE_COUNT];
            for (int i = 0; i < SAMPLE_COUNT; i++)
            {
                errorData[i] = 0.6 * Math.Cos(i * 0.1) + 0.5 * Math.Sin(i * 0.3);
            }

            var expectSeries = new LineSeries { Title = "理想曲线(无误差)" };
            var actualSeries = new LineSeries { Title = "实际曲线(有误差)" };

            for (int i = 0; i < SAMPLE_COUNT; i++)
            {
                expectData[i] = 0 ;
                actualData[i] = 0  + errorData[i];
                expectSeries.Points.Add(new DataPoint(i, expectData[i]));
                actualSeries.Points.Add(new DataPoint(i, actualData[i]));
            }
            Model.Series.Add(expectSeries);
            Model.Series.Add(actualSeries);

            errorData = new double[SAMPLE_COUNT / 2];
            for (int i = 0; i < SAMPLE_COUNT / 2; i++)
            {
                errorData[i] = 0.6 * Math.Cos(i * 2 * 0.1) + 0.5 * Math.Sin(i * 2 * 0.3);
            }
        }

        public void AddProcessSeries()
        {
            var cp = new ClampCompensatingItem(errorData);
            var datas = new double[SAMPLE_COUNT];
            Array.Copy(actualData, datas, actualData.Length);
            cp.Compensate(datas);            

            var processSeries = new LineSeries { };
            for (int i = 0; i < datas.Length; i++)
            {
                processSeries.Points.Add(new DataPoint(i, datas[i]));
            }
            var r = GoodnessOfFit.R(datas, expectData);
            processSeries.Title = $"处理后的曲线 R = {r}";
            Model.Series.Add(processSeries);
        }
    }
}
