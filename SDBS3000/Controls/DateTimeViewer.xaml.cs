using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SDBS3000.Controls
{
    /// <summary>
    /// DateTimeViewer.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeViewer : TextBlock
    {
        private DispatcherTimer updateTimer;
        public DateTimeViewer()
        {
            InitializeComponent();
            UpdateDisplayTime();
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 1);
            updateTimer.Tick += (s, e) => UpdateDisplayTime();
            updateTimer.Start();
        }

        private void UpdateDisplayTime()
        {
            TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            DateTextBlock.Text = DateTime.Now.ToString("yyyy/MM/dd dddd");
        }
    }
}
