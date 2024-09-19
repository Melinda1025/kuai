using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using S7.Net;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;

namespace SDBS3000.ViewModels
{
    public partial class SingleStepViewModel : ObservableObject
    {
        [ObservableProperty]
        private string runWay1 = "正向".Localize();
        [ObservableProperty]
        private string runWay2 = "反向".Localize();
        /// <summary>
        /// 单步速度
        /// </summary>        
        private float speed = 0f;
        public float Speed
        {
            get => speed;
            set
            {
                if(!plc.IsConnected || speed == value) return;
                plc.Write("DB20.DBD486", value);
                SetProperty(ref speed, value);
            }
        }
        /// <summary>
        /// 单步行程
        /// </summary>        
        private float stroke = 0f;
        public float Stroke
        {
            get => stroke;
            set
            {
                if(!plc.IsConnected || stroke == value) return;
                plc.Write("DB20.DBD490", value);
                SetProperty(ref stroke, value);
            }
        }
        [ObservableProperty]
        private float xAxis;
        [ObservableProperty]
        private float upYAxis;
        [ObservableProperty]
        private float downYAxis;

        private readonly ExtendedPlc plc;
        private const int UPDATE_INTERVAL = 1000;
        private bool isUpdating = false;
        private readonly Timer updateTimer;

        public SingleStepViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            updateTimer = new(OnUpdateData, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 读取跳动信息
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void OnUpdateData(object state)
        {
            if(!plc.IsConnected) return;
            try
            {
                XAxis = (float)await plc.ReadAsync(DataType.DataBlock, 21, 122, VarType.Real, 1);
                UpYAxis = (float)await plc.ReadAsync(DataType.DataBlock, 21, 126, VarType.Real, 1);
                DownYAxis = (float)await plc.ReadAsync(DataType.DataBlock, 21, 130, VarType.Real, 1);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isUpdating)
                {
                    updateTimer.Change(UPDATE_INTERVAL, Timeout.Infinite);
                }
            }
        }

        [RelayCommand]
        public void ChangeTimer(string state)
        {
            switch (state)
            {
                case "1":
                    if(plc.IsConnected)
                    {
                        var pageInfo = new PageInfo()
                        {
                            SingleStep = true,
                        };
                        plc.WriteStruct(pageInfo, 20, 470);
                    }
                    isUpdating = true;
                    updateTimer.Change(0, Timeout.Infinite);
                    break;
                case "0":
                    isUpdating = false;
                    updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    break;
            }
        }

        /// <summary>
        /// 设置运行模式
        /// </summary>
        /// <param name="mode">0 点动 1 保持</param>
        [RelayCommand]
        public async Task SetRunModeAsync(string mode)
        {
            if (!plc.IsConnected) return;            
            switch (mode)
            {
                case "0":
                    await plc.WriteAsync("DB20.DBX494.0", false);
                    break;
                case "1":
                    await plc.WriteAsync("DB20.DBX494.0", true);
                    break;
            }
        }

        /// <summary>
        /// 设置运行方式
        /// </summary>
        /// <param name="way">0 方式1 1 方式2</param>
        [RelayCommand]
        public async Task SetRunWayAsync(string way)
        {
            if (!plc.IsConnected) return;
            switch (way)
            {
                case "0":
                    await plc.WriteAsync("DB20.DBX494.1", false);
                    break;
                case "1":
                    await plc.WriteAsync("DB20.DBX494.1", true);
                    break;
            }
        }

        /// <summary>
        /// 夹具气缸
        /// </summary>
        [RelayCommand]
        public async Task DebugClampCylinderAsync()
        {
            RunWay1 = "夹紧".Localize();
            RunWay2 = "松开".Localize();
            //await plc.WriteAsync("DB21.DBD100.0", Convert.ToInt32(Math.Pow(2, 24)));
            if(!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 24));
        }

        /// <summary>
        /// 平衡测量伺服
        /// </summary>
        [RelayCommand]
        public async Task BalanceServoAsync()
        {
            RunWay1 = "正向".Localize();
            RunWay2 = "反向".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 24));
        }

        /// <summary>
        /// 打标气缸
        /// </summary>
        [RelayCommand]
        public async Task MarkingCylinderAsync()
        {
            RunWay1 = "松开".Localize();
            RunWay2 = "夹紧".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 26));
        }

        /// <summary>
        /// 跳动径向伺服
        /// </summary>
        [RelayCommand]
        public async Task DebugRadialServoAsync()
        {
            RunWay1 = "后退".Localize();
            RunWay2 = "前进".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 17));
        }

        /// <summary>
        /// 跳动上端伺服
        /// </summary>
        [RelayCommand]
        public async Task DebugUpperServoAsync()
        {
            RunWay1 = "向下".Localize();
            RunWay2 = "向上".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 18));
        }

        /// <summary>
        /// 跳动下端伺服
        /// </summary>
        [RelayCommand]
        public async Task DebugLowerServoAsync()
        {
            RunWay1 = "向下".Localize();
            RunWay2 = "向上".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 19));
        }

        /// <summary>
        /// 跳动径向较正零点
        /// </summary>
        [RelayCommand]
        public async Task RezeroRadialAsync()
        {
            RunWay1 = "停止".Localize();
            RunWay2 = "运行".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 9));
        }

        /// <summary>
        /// 跳动上端较正零点
        /// </summary>
        [RelayCommand]
        public async Task RezeroUpperAsync()
        {
            RunWay1 = "停止".Localize();
            RunWay2 = "运行".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 11));

        }

        /// <summary>
        /// 跳动下端较正零点
        /// </summary>
        [RelayCommand]
        public async Task RezeroLowerAsync()
        {
            RunWay1 = "停止".Localize();
            RunWay2 = "运行".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 10));
        }

        /// <summary>
        /// 复位
        /// </summary>
        [RelayCommand]
        public async Task ResetAsync()
        {
            RunWay1 = "停止".Localize();
            RunWay2 = "运行".Localize();
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB21.DBD100.0", (long)Math.Pow(2, 30));
        }
    }
}
