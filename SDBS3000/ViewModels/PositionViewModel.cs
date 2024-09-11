using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.Views.Dialogs;

namespace SDBS3000.ViewModels
{
    public partial class PositionViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool rotationDirection;
        [ObservableProperty]
        private bool switchType;
        [ObservableProperty]
        private float plusePerRound;
        [ObservableProperty]
        private int slots;
        [ObservableProperty]
        private float positioningSpeed;
        [ObservableProperty]
        private float accelerationTime;
        [ObservableProperty]
        private float decelerationTime;
        [ObservableProperty]
        private float positioningCompensation;
        [ObservableProperty]
        private float slipCompensation;

        [ObservableProperty]
        private bool isDetecting;


        private readonly ExtendedPlc plc;
        private const int TIMER_INTERVAL = 100;
        private readonly Timer detectTimer;
        public PositionViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            RotationDirection = AppSetting.Default.RotationDirection;
            detectTimer = new(OnDetecting, null, Timeout.Infinite, Timeout.Infinite);

            RotationDirection = AppSetting.Default.RotationDirection;
            SwitchType = AppSetting.Default.SwitchType;
            PlusePerRound = AppSetting.Default.PlusePerRound;
            Slots = AppSetting.Default.Slots;
            PositioningSpeed = AppSetting.Default.PositioningSpeed;
            AccelerationTime = AppSetting.Default.AccelerationTime;
            DecelerationTime = AppSetting.Default.DecelerationTime;
            PositioningCompensation = AppSetting.Default.PositioningCompensation;
            SlipCompensation = AppSetting.Default.SlipCompensation;            
        }

        /// <summary>
        /// 检测脉冲 （逻辑不确定）
        /// </summary>
        /// <param name="state"></param>
        private async void OnDetecting(object state)
        {
            if (!plc.IsConnected) return;
            IsDetecting = (bool)await plc.ReadAsync("DB20.DBX300.0");
            //当检测脉冲为false时，读取脉冲数，停止检测
            if(!IsDetecting)
            {
                PlusePerRound = (float)await plc.ReadAsync("DB20.DBD132.0");
            }
            else
            {
                detectTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
            }            
        }

        [RelayCommand]
        public async Task OnloadAsync()
        {
            if (!plc.IsConnected) return;
            var pageInfo = new PageInfo()
            {
                Positioning = true
            };
            await plc.WriteStructAsync(pageInfo, 20, 470);
        }

        /// <summary>
        /// 开始/停止检测脉冲
        /// </summary>
        [RelayCommand]
        public void DetectPulse()
        {
            if(!plc.IsConnected)
            {
                IsDetecting = false;
                Growl.Warning("PLC未连接");
                return;
            }            
            if(IsDetecting)
            {
                plc.Write("DB20.DBX300.0", true);
                detectTimer.Change(0, Timeout.Infinite);
            }
            else
            {
                plc.Write("DB20.DBX300.0", false);
                detectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        [RelayCommand]
        public static void ViewParameters()
        {
            var dialog = App.Container.Resolve<AxisParamsDialog>();
            Dialog.Show(dialog, "MainContainer");
        }
    }
}
