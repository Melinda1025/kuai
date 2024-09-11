using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using SDBS3000.Core.Utils;

namespace SDBS3000.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private short measureTimes;
        [ObservableProperty]
        private float slipRange;
        [ObservableProperty]
        private float refreshRate;
        [ObservableProperty]
        private int digits;
        [ObservableProperty]
        private short workMode;
        [ObservableProperty]
        private short positioningMode;
        [ObservableProperty]
        private short needleMode;
        [ObservableProperty]
        private short safetyMode;

        private readonly ExtendedPlc plc;
        public SettingViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            MeasureTimes = AppSetting.Default.MeasureTimes;
            SlipRange = AppSetting.Default.SlipRange;
            RefreshRate = AppSetting.Default.RefreshRate;
            Digits = AppSetting.Default.Digits;
            WorkMode = AppSetting.Default.WorkMode;
            PositioningMode = AppSetting.Default.PositioningMode;
            NeedleMode = AppSetting.Default.NeedleMode;
            SafetyMode = AppSetting.Default.SafetyMode;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if(!plc.IsConnected) return;
            AppSetting.Default.MeasureTimes = MeasureTimes;
            AppSetting.Default.SlipRange = SlipRange;
            AppSetting.Default.RefreshRate = RefreshRate;
            AppSetting.Default.Digits = Digits;
            AppSetting.Default.WorkMode = WorkMode;
            AppSetting.Default.PositioningMode = PositioningMode;
            AppSetting.Default.NeedleMode = NeedleMode;
            AppSetting.Default.SafetyMode = SafetyMode;
            AppSetting.Default.Save();
            await plc.WriteAsync("DB20.DBW312.0", MeasureTimes);
            await plc.WriteAsync("DB20.DBD314.0", SlipRange);
            await plc.WriteAsync("DB20.DBD308.0", RefreshRate);
            await plc.WriteAsync("DB20.DBW318.0", WorkMode);
            await plc.WriteAsync("DB20.DBW320.0", PositioningMode);
            await plc.WriteAsync("DB20.DBW330.0", NeedleMode);
            await plc.WriteAsync("DB20.DBW328.0", SafetyMode);
            Growl.Success("保存成功");
        }
    }
}
