using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SDBS3000.ViewModels
{
    public partial class SingleStepViewModel : ObservableObject
    {
        [ObservableProperty]
        private string runWay1 = "正向";
        [ObservableProperty]
        private string runWay2 = "反向";

        /// <summary>
        /// 设置运行模式
        /// </summary>
        /// <param name="mode">0 点动 1 保持</param>
        [RelayCommand]
        public void SetRunMode(string mode)
        {
            switch (mode)
            {
                case "0":
                    break;
                case "1":
                    break;
            }
        }

        /// <summary>
        /// 设置运行方式
        /// </summary>
        /// <param name="way">0 方式1 1 方式2</param>
        [RelayCommand]
        public void SetRunWay(string way)
        {
            switch (way)
            {
                case "0":
                    break;
                case "1":
                    break;
            }
        }

        /// <summary>
        /// 夹具气缸
        /// </summary>
        [RelayCommand]
        public void DebugClampCylinder()
        {
        }

        /// <summary>
        /// 平衡测量伺服
        /// </summary>
        [RelayCommand]
        public void BalanceServo()
        {

        }

        /// <summary>
        /// 打标气缸
        /// </summary>
        [RelayCommand]
        public void MarkingCylinder()
        {

        }

        /// <summary>
        /// 跳动径向伺服
        /// </summary>
        [RelayCommand]
        public void DebugRadialServo()
        {

        }

        /// <summary>
        /// 跳动上端伺服
        /// </summary>
        [RelayCommand]
        public void DebugUpperServo()
        {

        }

        /// <summary>
        /// 跳动下端伺服
        /// </summary>
        [RelayCommand]
        public void DebugLowerServo()
        {

        }

        /// <summary>
        /// 跳动径向较正零点
        /// </summary>
        [RelayCommand]
        public void RezeroRadial()
        {

        }

        /// <summary>
        /// 跳动上端较正零点
        /// </summary>
        [RelayCommand]
        public void RezeroUpper()
        {

        }

        /// <summary>
        /// 跳动下端较正零点
        /// </summary>
        [RelayCommand]
        public void RezeroLower()
        {

        }

        /// <summary>
        /// 复位
        /// </summary>
        [RelayCommand]
        public void Reset()
        {

        }
    }
}
