using System.Windows.Controls;
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
            Loaded += (s, e) => updateTimer.Start();
            Unloaded += (s, e) => updateTimer.Stop();
        }

        private void UpdateDisplayTime()
        {
            TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            DateTextBlock.Text = DateTime.Now.ToString("yyyy/MM/dd dddd");
        }
    }
}
