using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SDBS3000.Core.Models;
using SDBS3000.Extensions;

namespace SDBS3000.Controls
{
    /// <summary>
    /// JumpingCircle.xaml 的交互逻辑
    /// </summary>
    public partial class JumpingCircle : UserControl
    {
        public DataPointList Points
        {
            get { return (DataPointList)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public Color LineStroke
        {
            get { return (Color)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }

        public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register(
            "LineStroke",
            typeof(Color),
            typeof(JumpingCircle),
            new PropertyMetadata(
                Color.FromRgb(0, 230, 206),
                (d, e) =>
                {
                    var circle = (JumpingCircle)d;
                    var color = (OxyColor)e.NewValue;                    
                    circle.lineSeries.Color = OxyColor.FromRgb(color.R, color.G, color.B);
                }
            )
        );

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points",
            typeof(DataPointList),
            typeof(JumpingCircle),
            new PropertyMetadata(
                new DataPointList(),
                (d, e) =>
                {
                    var circle = (JumpingCircle)d;
                    if (e.NewValue is DataPointList points)
                    {
                        points.ListChanged += circle.OnListChanged;
                    }
                    if (e.OldValue is DataPointList points1)
                    {
                        points1.ListChanged -= circle.OnListChanged;
                    }
                }
            )
        );

        private void OnListChanged(object sender, EventArgs e)
        {
            MapPoints(sender as List<DataPoint>);
        }

        private readonly AngleAxis angleAxis = new();
        private readonly MagnitudeAxis magnitudeAxis = new();
        private readonly LineSeries lineSeries = new();
        private readonly PlotModel model = new();

        public JumpingCircle()
        {
            InitializeComponent();
            model = new PlotModel();
            InitializePlotModel();
        }

        private void InitializePlotModel()
        {
            model.PlotType = PlotType.Polar;
#pragma warning disable CS0618 // 类型或成员已过时
            model.MouseDown += (s, e) => e.Handled = true;
#pragma warning restore CS0618 // 类型或成员已过时
            model.PlotAreaBorderThickness = new OxyThickness(0);
            model.Padding = new OxyThickness(0);
            model.PlotMargins = new OxyThickness(0);
            angleAxis.Minimum = 0;
            angleAxis.Maximum = 360;
            angleAxis.MajorGridlineStyle = LineStyle.None;
            angleAxis.MinorGridlineStyle = LineStyle.None;
            angleAxis.LabelFormatter = v => string.Empty;
            //angleAxis.LabelFormatter = v => $"{v}°";

            magnitudeAxis.Minimum = 0;
            magnitudeAxis.Maximum = 1;
            magnitudeAxis.MajorGridlineStyle = LineStyle.None;
            magnitudeAxis.MinorGridlineStyle = LineStyle.None;
            magnitudeAxis.LabelFormatter = v => string.Empty;
            model.Axes.Add(angleAxis);
            model.Axes.Add(magnitudeAxis);
            model.Series.Add(lineSeries);
            lineSeries.Color = OxyColor.FromRgb(0, 230, 206);
            //lineSeries.LineJoin = LineJoin.Round;
            //lineSeries.BrokenLineColor = OxyColor.FromRgb(255, 0, 0);
            lineSeries.StrokeThickness = 1;
            //lineSeries.BrokenLineThickness = 1.5;
            lineSeries.MarkerType = MarkerType.None;

            PlotView.Model = model;
        }

        #region 弃用的分段变色版本
        //private void MapPoints(List<DataPoint> datas)
        //{
        //    lineSeries.Points.Clear();
        //    if (datas == null || datas.Count == 0)
        //        return;

        //    lineSeries.Points.Add(datas[0]);
        //    for (int i = 1; i < datas.Count; i++)
        //    {
        //        //添加交叉点
        //        if (datas[i].X > radius && datas[i - 1].X < radius)
        //        {
        //            lineSeries.Points.Add(GetCrossPoint(datas[i - 1], datas[i], radius));
        //        }
        //        else if (datas[i].X < radius && datas[i - 1].X > radius)
        //        {
        //            lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
        //            lineSeries.Points.Add(GetCrossPoint(datas[i - 1], datas[i], radius));
        //        }
        //        //超出
        //        if (datas[i].X > radius)
        //        {
        //            lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
        //            lineSeries.Points.Add(datas[i]);
        //        }
        //        else
        //        {
        //            lineSeries.Points.Add(datas[i]);
        //        }
        //    }

        //    //首尾相连
        //    if (datas[0].X > radius && datas[^1].X < radius)
        //    {
        //        lineSeries.Points.Add(GetCrossPoint(datas[^1], datas[0], radius));
        //    }
        //    else if (datas[0].X < radius && datas[^1].X > radius)
        //    {
        //        lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
        //        lineSeries.Points.Add(GetCrossPoint(datas[^1], datas[0], radius));
        //    }
        //    if (datas[0].X > radius)
        //    {
        //        lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
        //        lineSeries.Points.Add(datas[0]);
        //    }
        //    else
        //    {
        //        lineSeries.Points.Add(datas[0]);
        //    }

        //    model.InvalidatePlot(true);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static DataPoint GetCrossPoint(DataPoint point1, DataPoint point2, double r)
        //{
        //    var p1 = point1.PolorToCartesian();
        //    var p2 = point2.PolorToCartesian();

        //    var k = (p2.Y - p1.Y) / (p2.X - p1.X);
        //    var b = p1.Y - k * p1.X;

        //    var A = k * k + 1;
        //    var B = 2 * k * b;
        //    var C = b * b - r * r;

        //    var x1 = (-B + Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
        //    var y1 = k * x1 + b;
        //    if (IsBetween(x1, y1, p1, p2))
        //    {
        //        return new DataPoint(r, Math.Atan2(y1, x1) * 180 / Math.PI);
        //    }
        //    else
        //    {
        //        var x2 = (-B - Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
        //        var y2 = k * x2 + b;
        //        return new DataPoint(r, Math.Atan2(y2, x2) * 180 / Math.PI);
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static bool IsBetween(double x, double y, DataPoint p1, DataPoint p2)
        //{
        //    if (p1.X < p2.X)
        //    {
        //        if (x < p1.X || x > p2.X)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (x > p1.X || x < p2.X)
        //        {
        //            return false;
        //        }
        //    }

        //    if (p1.Y < p2.Y)
        //    {
        //        if (y < p1.Y || y > p2.Y)
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (y > p1.Y || y < p2.Y)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        #endregion
        private void MapPoints(List<DataPoint> datas)
        {
            const double rate = 140.0 / 100;

            lineSeries.Points.Clear();
            if (datas == null || datas.Count == 0)
                return;

            //magnitudeAxis.Maximum = datas[0].X * rate;

            var arr = datas.ToArray();
            Array.Sort(arr, (p1, p2) => p1.X.CompareTo(p2.X));
            magnitudeAxis.Maximum = arr[arr.Length / 2].X * rate;

            lineSeries.Points.AddRange(datas);
            model.InvalidatePlot(true);
        }
    }

    public class DataPointList : List<DataPoint>
    {
        public event EventHandler ListChanged;

        public void NotifyChanged()
        {
            ListChanged?.Invoke(this, EventArgs.Empty);
        }

        public new void Clear()
        {
            base.Clear();
            NotifyChanged();
        }
    }
}
