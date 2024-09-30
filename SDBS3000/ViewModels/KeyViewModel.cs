using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SDBS3000.Core.Models;
using SDBS3000.Core.Models.PlcStructs;
using SDBS3000.Core.Utils;
using SDBS3000.Utils;
using SDBS3000.ViewModels.Interface;

namespace SDBS3000.ViewModels
{
    public partial class KeyViewModel : ObservableObject, ISaveChanged
    {
        private short compensateTimes;

        /// <summary>
        /// 补偿次数
        /// </summary>
        public short CompensateTimes
        {
            get => compensateTimes;
            set
            {
                if (!plc.IsConnected)
                    return;
                plc.Write("DB20.DBW14.0", compensateTimes);
                SetProperty(ref compensateTimes, value);
            }
        }

        [ObservableProperty]
        private float speed;

        [ObservableProperty]
        private float panel1Value;

        [ObservableProperty]
        private float panel1Angle;

        [ObservableProperty]
        private float panel2Value;

        [ObservableProperty]
        private float panel2Angle;

        [ObservableProperty]
        private short panel1Signal;

        [ObservableProperty]
        private short panel2Signal;

        //[ObservableProperty]
        //private float progress;
        private int step;
        public int Step
        {
            get => step;
            set
            {
                SetProperty(ref step, value);
                //清除测量值
                if (value == 0)
                {
                    Panel1Signal = Panel2Signal = 0;
                    Speed = 0;
                    Tip = string.Empty;
                }
            }
        }

        public bool HasChanges { get; set; } = false;

        [ObservableProperty]
        private string tip;

        /// <summary>
        /// 测量模式
        /// </summary>
        private short measureMode;
        private readonly ExtendedPlc plc;
        private const int TIMER_INTERVAL = 500;
        private bool isUpdatingInfo,
            isMeasuring;
        private readonly Timer infoTimer,
            measureTimer;

        public KeyViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            CompensateTimes = 2;
            infoTimer = new(OnUpdateInfo, null, Timeout.Infinite, Timeout.Infinite);
            measureTimer = new(OnMeasure, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 更新提示信息
        /// </summary>
        /// <param name="state"></param>
        private async void OnUpdateInfo(object state)
        {
            if (!plc.IsConnected)
                return;
            try
            {
                bool isOpen = (bool)await plc.ReadAsync("DB20.DBX460.4");
                //462 0键补偿完成 1正在进行键补偿
                var measuringStatus = (int)await plc.ReadAsync("DB20.DBW462.0");
                if (isOpen && measuringStatus == 1)
                {
                    Tip = "正在进行键补偿".Localize();
                }
                else if (isOpen && measuringStatus == 0 && Step == 2)
                {
                    Tip = "键补偿检测完成".Localize();
                }

                if (measuringStatus == 1 && !isMeasuring)
                {
                    isMeasuring = true;
                    measureTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
                }
                else if (measuringStatus == 0)
                {
                    isMeasuring = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isUpdatingInfo)
                {
                    infoTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// 更新测量值
        /// </summary>
        /// <param name="state"></param>
        private async void OnMeasure(object state)
        {
            if (!plc.IsConnected)
                return;
            try
            {                
                var info = await plc.ReadStructAsync<KeyCompensateInfo>(20, 400);
                if (info == null)
                    return;
                HasChanges = false;
                Speed = info.Value.Speed;
                //Progress = (float)info.Value.Progress / AppSetting.Default.MeasureTimes;
                Panel1Value = info.Value.Panel1Value;
                Panel1Angle = info.Value.Panel1Angle;
                Panel2Value = info.Value.Panel2Value;
                Panel2Angle = info.Value.Panel2Angle;
                Panel1Signal = info.Value.Panel1Signal;
                Panel2Signal = info.Value.Panel2Signal;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isMeasuring)
                {
                    measureTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
                }
            }
        }

        [RelayCommand]
        public async Task OnloadedAsync()
        {
            if (!plc.IsConnected)
                return;
            isUpdatingInfo = true;
            infoTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
            var pageInfo = new PageInfo() { Compensation = true };
            await plc.WriteStructAsync(pageInfo, 20);
            measureMode = (short)plc.Read("DB20.DBW8.0");
            //false为夹具 true为键
            await plc.WriteAsync("DB20.DBX460.4", true);
        }

        [RelayCommand]
        public async Task OnUnloadAsync()
        {
            infoTimer.Change(Timeout.Infinite, Timeout.Infinite);
            measureTimer.Change(Timeout.Infinite, Timeout.Infinite);
            if (!plc.IsConnected)
                return;
            //false为夹具 true为键
            await plc.WriteAsync("DB20.DBW462.0", 0);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task StartMeasureAsync()
        {
            if (!plc.IsConnected)
                return;
            if (Step < 2)
            {
                Step++;
            }
            else
            {
                Step = 1;
            }
            await plc.WriteAsync("DB20.DBX440.5", true);
            await Task.Delay(300);
            await plc.WriteAsync("DB20.DBX440.5", false);
        }

        /// <summary>
        /// 开启补偿
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task OpenAsync()
        {
            if (!plc.IsConnected)
                return;
            await plc.WriteAsync("DB20.DBX460.3", true);
            Step = 0;
        }

        /// <summary>
        /// 清除补偿
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task CloseAsync()
        {
            if (!plc.IsConnected)
                return;
            await plc.WriteAsync("DB20.DBX460.3", false);
        }

        [RelayCommand]
        public async Task ResetAsync()
        {
            if (!plc.IsConnected)
                return;
            await plc.WriteAsync("DB20.DBW462.0", 0);
            await plc.WriteAsync("DB20.DBX440.6", true);
            await Task.Delay(500);
            await plc.WriteAsync("DB20.DBX440.6", false);
            Step = 0;
        }

        public bool AskToSave() => MessageBoxHelper.AskToSave();        
    }
}
