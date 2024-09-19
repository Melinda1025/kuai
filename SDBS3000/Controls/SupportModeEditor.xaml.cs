using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SDBS3000.Controls
{
    /// <summary>
    /// SupportModeEditor.xaml 的交互逻辑
    /// </summary>
    public partial class SupportModeEditor : UserControl
    {
        public int SupportMode
        {
            get { return (int)GetValue(SupportModeProperty); }
            set { SetValue(SupportModeProperty, value); }
        }
        public float? R1
        {
            get { return (float?)GetValue(R1Property); }
            set { SetValue(R1Property, value); }
        }
        public float? R2
        {
            get { return (float?)GetValue(R2Property); }
            set { SetValue(R2Property, value); }
        }
        public float? A
        {
            get { return (float?)GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }
        public float? B
        {
            get { return (float?)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }
        public float? C
        {
            get { return (float?)GetValue(CProperty); }
            set { SetValue(CProperty, value); }
        }
        public float? Speed
        {
            get { return (float?)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }


        public static readonly DependencyProperty SimulationModeProperty =
            DependencyProperty.Register(
                "SimulationMode",
                typeof(int),
                typeof(SupportModeEditor),
                new PropertyMetadata(0)
            );
        public static readonly DependencyProperty SupportModeProperty = DependencyProperty.Register(
            "SupportMode",
            typeof(int),
            typeof(SupportModeEditor),
            new PropertyMetadata(1, new PropertyChangedCallback(OnSupportModeChanged))
        );
        public static readonly DependencyProperty R1Property = DependencyProperty.Register(
            "R1",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty R2Property = DependencyProperty.Register(
            "R2",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty AProperty = DependencyProperty.Register(
            "A",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(
            "B",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );
        public static readonly DependencyProperty CProperty = DependencyProperty.Register(
            "C",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register(
            "Speed",
            typeof(float?),
            typeof(SupportModeEditor),
            new PropertyMetadata(null)
        );

        public SupportModeEditor()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private static void OnSupportModeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            var editor = (SupportModeEditor)d;
            editor.UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            R1Tbx.Visibility = Visibility.Visible;
            R2Tbx.Visibility = Visibility.Visible;
            ATbx.Visibility = Visibility.Visible;
            BTbx.Visibility = Visibility.Visible;
            CTbx.Visibility = Visibility.Visible;
            SpeedTbx.Visibility = Visibility.Visible;
            switch (SupportMode)
            {
                case 1:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑1"] as Brush;
                    Canvas.SetLeft(R1Tbx, 39);
                    Canvas.SetTop(R1Tbx, 210);

                    Canvas.SetLeft(R2Tbx, 870);
                    Canvas.SetTop(R2Tbx, 226);

                    Canvas.SetLeft(ATbx, 216);
                    Canvas.SetTop(ATbx, 73);

                    Canvas.SetLeft(BTbx, 460);
                    Canvas.SetTop(BTbx, 73);

                    Canvas.SetLeft(CTbx, 697);
                    Canvas.SetTop(CTbx, 73);

                    Canvas.SetLeft(SpeedTbx, 343);
                    Canvas.SetTop(SpeedTbx, 262);
                    break;
                case 2:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑2"] as Brush;
                    Canvas.SetLeft(R1Tbx, 47);
                    Canvas.SetTop(R1Tbx, 230);

                    Canvas.SetLeft(R2Tbx, 862);
                    Canvas.SetTop(R2Tbx, 228);

                    Canvas.SetLeft(ATbx, 251);
                    Canvas.SetTop(ATbx, 110);

                    Canvas.SetLeft(BTbx, 457);
                    Canvas.SetTop(BTbx, 56);

                    Canvas.SetLeft(CTbx, 660);
                    Canvas.SetTop(CTbx, 110);

                    Canvas.SetLeft(SpeedTbx, 351);
                    Canvas.SetTop(SpeedTbx, 283);
                    break;
                case 3:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑3"] as Brush;
                    Canvas.SetLeft(R1Tbx, 48);
                    Canvas.SetTop(R1Tbx, 200);

                    R2Tbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(ATbx, 328);
                    Canvas.SetTop(ATbx, 80);

                    Canvas.SetLeft(BTbx, 606);
                    Canvas.SetTop(BTbx, 80);

                    CTbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(SpeedTbx, 371);
                    Canvas.SetTop(SpeedTbx, 253);
                    break;
                case 4:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑4"] as Brush;
                    Canvas.SetLeft(R1Tbx, 53);
                    Canvas.SetTop(R1Tbx, 225);

                    Canvas.SetLeft(R2Tbx, 856);
                    Canvas.SetTop(R2Tbx, 225);

                    Canvas.SetLeft(ATbx, 318);
                    Canvas.SetTop(ATbx, 105);

                    Canvas.SetLeft(BTbx, 648);
                    Canvas.SetTop(BTbx, 105);

                    Canvas.SetLeft(CTbx, 482);
                    Canvas.SetTop(CTbx, 55);

                    Canvas.SetLeft(SpeedTbx, 375);
                    Canvas.SetTop(SpeedTbx, 278);
                    break;
                case 5:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑5"] as Brush;
                    Canvas.SetLeft(R1Tbx, 53);
                    Canvas.SetTop(R1Tbx, 225);

                    Canvas.SetLeft(R2Tbx, 856);
                    Canvas.SetTop(R2Tbx, 225);

                    Canvas.SetLeft(ATbx, 261);
                    Canvas.SetTop(ATbx, 105);

                    Canvas.SetLeft(BTbx, 428);
                    Canvas.SetTop(BTbx, 55);

                    Canvas.SetLeft(CTbx, 591);
                    Canvas.SetTop(CTbx, 105);

                    Canvas.SetLeft(SpeedTbx, 340);
                    Canvas.SetTop(SpeedTbx, 278);
                    break;
                case 6:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑6"] as Brush;
                    Canvas.SetLeft(R1Tbx, 367);
                    Canvas.SetTop(R1Tbx, 65);

                    R2Tbx.Visibility = Visibility.Collapsed;

                    ATbx.Visibility = Visibility.Collapsed;

                    BTbx.Visibility = Visibility.Collapsed;

                    CTbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(SpeedTbx, 359);
                    Canvas.SetTop(SpeedTbx, 164);
                    break;
                case 7:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑7"] as Brush;
                    Canvas.SetLeft(R1Tbx, 45);
                    Canvas.SetTop(R1Tbx, 164);

                    Canvas.SetLeft(R2Tbx, 864);
                    Canvas.SetTop(R2Tbx, 180);

                    Canvas.SetLeft(ATbx, 417);
                    Canvas.SetTop(ATbx, 395);

                    Canvas.SetLeft(BTbx, 345);
                    Canvas.SetTop(BTbx, 44);

                    Canvas.SetLeft(CTbx, 631);
                    Canvas.SetTop(CTbx, 45);

                    Canvas.SetLeft(SpeedTbx, 310);
                    Canvas.SetTop(SpeedTbx, 219);
                    break;
                case 8:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑8"] as Brush;
                    Canvas.SetLeft(R1Tbx, 45);
                    Canvas.SetTop(R1Tbx, 180);

                    Canvas.SetLeft(R2Tbx, 864);
                    Canvas.SetTop(R2Tbx, 164);

                    Canvas.SetLeft(ATbx, 278);
                    Canvas.SetTop(ATbx, 45);

                    Canvas.SetLeft(BTbx, 564);
                    Canvas.SetTop(BTbx, 45);

                    Canvas.SetLeft(CTbx, 492);
                    Canvas.SetTop(CTbx, 395);

                    Canvas.SetLeft(SpeedTbx, 406);
                    Canvas.SetTop(SpeedTbx, 218);
                    break;
                case 9:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑9"] as Brush;
                    Canvas.SetLeft(R1Tbx, 103);
                    Canvas.SetTop(R1Tbx, 231);

                    R2Tbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(ATbx, 402);
                    Canvas.SetTop(ATbx, 126);

                    BTbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(CTbx, 464);
                    Canvas.SetTop(CTbx, 56);

                    Canvas.SetLeft(SpeedTbx, 424);
                    Canvas.SetTop(SpeedTbx, 280);
                    break;
                case 10:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑10"] as Brush;
                    Canvas.SetLeft(R1Tbx, 773);
                    Canvas.SetTop(R1Tbx, 231);

                    R2Tbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(ATbx, 412);
                    Canvas.SetTop(ATbx, 56);

                    BTbx.Visibility = Visibility.Collapsed;

                    Canvas.SetLeft(CTbx, 473);
                    Canvas.SetTop(CTbx, 126);

                    Canvas.SetLeft(SpeedTbx, 258);
                    Canvas.SetTop(SpeedTbx, 280);
                    break;
                case 11:
                    BackgroundBorder.Background = App.Current.Resources["支撑编辑11"] as Brush;
                    Canvas.SetLeft(R1Tbx, 355);
                    Canvas.SetTop(R1Tbx, 42);

                    Canvas.SetLeft(R2Tbx, 369);
                    Canvas.SetTop(R2Tbx, 242);

                    Canvas.SetLeft(ATbx, 182);
                    Canvas.SetTop(ATbx, 244);

                    Canvas.SetLeft(BTbx, 728);
                    Canvas.SetTop(BTbx, 198);

                    Canvas.SetLeft(CTbx, 728);
                    Canvas.SetTop(CTbx, 293);

                    Canvas.SetLeft(SpeedTbx, 360);
                    Canvas.SetTop(SpeedTbx, 136);
                    break;
                default:
                    BackgroundBorder.Background = Brushes.Transparent;
                    R1Tbx.Visibility = Visibility.Collapsed;
                    R2Tbx.Visibility = Visibility.Collapsed;
                    ATbx.Visibility = Visibility.Collapsed;
                    BTbx.Visibility = Visibility.Collapsed;
                    CTbx.Visibility = Visibility.Collapsed;
                    SpeedTbx.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
