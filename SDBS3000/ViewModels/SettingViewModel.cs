using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using SDBS3000.Core.Utils;
using SDBS3000.Utils;
using SDBS3000.ViewModels.Interface;

namespace SDBS3000.ViewModels
{
    public partial class SettingViewModel : ObservableObject, ISaveChanged
    {
        private short measureTimes;
        public short MeasureTimes
        {
            get => measureTimes;
            set
            {
                if (value != measureTimes)
                {
                    SetProperty(ref measureTimes, value);
                    HasChanges = true;
                }
            }
        }

        private float slipRange;
        public float SlipRange
        {
            get => slipRange;
            set
            {
                if (value != slipRange)
                {
                    SetProperty(ref slipRange, value);
                    HasChanges = true;
                }
            }
        }
        private float refreshRate;
        public float RefreshRate
        {
            get => refreshRate;
            set
            {
                if (value != refreshRate)
                {
                    SetProperty(ref refreshRate, value);
                    HasChanges = true;
                }
            }
        }

        private int digits;
        public int Digits
        {
            get => digits;
            set
            {
                if (value != digits)
                {
                    SetProperty(ref digits, value);
                    HasChanges = true;
                }
            }
        }

        private short workMode;
        public short WorkMode
        {
            get => workMode;
            set
            {
                if (value != workMode)
                {
                    SetProperty(ref workMode, value);
                    HasChanges = true;
                }
            }
        }

        private short positioningMode;
        public short PositioningMode
        {
            get => positioningMode;
            set
            {
                if (value != positioningMode)
                {
                    SetProperty(ref positioningMode, value);
                    HasChanges = true;
                }
            }
        }

        private short safetyMode;
        public short SafetyMode
        {
            get => safetyMode;
            set
            {
                if (value != safetyMode)
                {
                    SetProperty(ref safetyMode, value);
                    HasChanges = true;
                }
            }
        }

        public bool HasChanges { get; set; } = false;

        public bool AskToSave() => MessageBoxHelper.AskToSave();

        private readonly ExtendedPlc plc;

        public SettingViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            measureTimes = AppSetting.Default.MeasureTimes;
            slipRange = AppSetting.Default.SlipRange;
            refreshRate = AppSetting.Default.RefreshRate;
            digits = AppSetting.Default.Digits;
            workMode = AppSetting.Default.WorkMode;
            positioningMode = AppSetting.Default.PositioningMode;            
            safetyMode = AppSetting.Default.SafetyMode;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            AppSetting.Default.MeasureTimes = MeasureTimes;
            AppSetting.Default.SlipRange = SlipRange;
            AppSetting.Default.RefreshRate = RefreshRate;
            AppSetting.Default.Digits = Digits;
            AppSetting.Default.WorkMode = WorkMode;
            AppSetting.Default.PositioningMode = PositioningMode;            
            AppSetting.Default.SafetyMode = SafetyMode;
            AppSetting.Default.Save();
            Growl.Success("保存配置成功");
            HasChanges = false;
            if (!plc.IsConnected)
                return;
            await plc.WriteAsync("DB20.DBW312.0", MeasureTimes);
            await plc.WriteAsync("DB20.DBD314.0", SlipRange);
            await plc.WriteAsync("DB20.DBD308.0", RefreshRate);
            await plc.WriteAsync("DB20.DBW318.0", WorkMode);
            await plc.WriteAsync("DB20.DBW320.0", PositioningMode);            
            await plc.WriteAsync("DB20.DBW328.0", SafetyMode);
            Growl.Success("写入PLC成功");
        }
    }
}
