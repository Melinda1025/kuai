using System.Buffers;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using S7.Net;
using S7.Net.Types;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.ViewModels.Interface;
using SDBS3000.Views;
using SDBS3000.Views.Dialogs;

namespace SDBS3000.ViewModels
{
    public partial class IndexViewModel : ObservableObject
    {
        #region 错误列表
        private readonly string[] ErrorList = new string[]
        {
            "径向零位接近开关故障".Localize(),
            "径向前限位接近开关故障".Localize(),
            "径向后限位接近开关故障".Localize(),
            "径向伺服故障".Localize(),
            "周向零位接近开关故障".Localize(),
            "周向正限位接近开关故障".Localize(),
            "周向反限位接近开关故障".Localize(),
            "周向伺服故障".Localize(),
            "工件夹紧气缸到位接近开关故障".Localize(),
            "工件夹紧气缸释放接近开关故障".Localize(),
            "平衡机伺服故障".Localize(),
            "基准信号故障".Localize(),
            "液压泵电机故障".Localize(),
            "刀具电机电源故障".Localize(),
            "刀具电机变频器故障".Localize(),
            "吸尘器故障".Localize(),
            "无气压".Localize(),
            "气压高".Localize(),
            "润滑油泵压力高".Localize(),
            "润滑油泵液位低".Localize(),
            "紧急停止".Localize(),
            "门开关故障".Localize(),
            "光栅故障".Localize(),
            "液压泵压力低故障".Localize(),
            "径向前软限位距离故障".Localize(),
            "径向后软限位距离故障".Localize(),
            "周向正软限位距离故障".Localize(),
            "周向反软限位距离故障".Localize(),
            "工件半径与基准半径不一致".Localize(),
            "夹具到位接近开关故障".Localize(),
            "夹具复位接近开关故障".Localize(),
            "工件信号开关故障".Localize(),
            "安全门到位磁环开关故障".Localize(),
            "安全门复位磁环开关故障".Localize(),
            "压紧油缸1复位接近开关故障".Localize(),
            "压紧油缸2复位接近开关故障".Localize(),
            "压紧油缸3复位接近开关故障".Localize(),
            "Z轴伺服故障".Localize(),
            "Z轴零位接近开关故障".Localize(),
            "Z轴上限位接近开关故障".Localize(),
            "Z轴下限位接近开关故障".Localize(),
            "Z轴上软限位距离故障".Localize(),
            "Z轴下软限位距离故障".Localize(),
            "Z轴电气零点故障".Localize(),
            "压紧油缸1到位接近开关故障".Localize(),
            "压紧油缸2到位接近开关故障".Localize(),
            "压紧油缸3到位接近开关故障".Localize(),
            "总电源故障".Localize(),
            "液压站液位低".Localize(),
            "液压站温度高".Localize(),
            "液压站滤网堵塞".Localize(),
            "打刀缸锁紧磁环开关故障".Localize(),
            "打刀缸释放磁环开关故障".Localize(),
        };
        #endregion
        [ObservableProperty]
        private FrameworkElement page;
        [ObservableProperty]
        private AddOrRemoveInfo addOrRemoveInfo;
        /// <summary>
        /// 0-100%
        /// </summary>
        [ObservableProperty]
        private float progress;
        [ObservableProperty]
        private float speed;
        [ObservableProperty]
        private bool isMeasuring = false;


        private readonly ExtendedPlc plc;
        private const int UPDATE_INTERVAL = 100;
        private readonly System.Threading.Timer updateTimer;
        public IndexViewModel(ExtendedPlc plc, AddOrRemoveInfo info)
        {            
            SimpleEventBus<Type>.Instance.Subscribe("NavigatePage", (s, t) => NavigatePage(t));
            SimpleEventBus<int>.Instance.Subscribe("ErrorReceived", OnErrorReceived);
            //注册跨页面访问属性
            PropertyAccessor<IndexViewModel, float>.Instance.Register("Progress", this, vm => vm.Progress);
            PropertyAccessor<IndexViewModel, float>.Instance.Register("Speed", this, vm => vm.Speed);
            PropertyAccessor<IndexViewModel, bool>.Instance.Register("IsMeasuring", this, vm => vm.IsMeasuring);

            updateTimer = new System.Threading.Timer(OnUpdateSpeedAndProgress, null, Timeout.Infinite, Timeout.Infinite);

            this.AddOrRemoveInfo = info;
            this.plc = plc;
            if (plc.IsConnected)
            {
                WriteSettingsToPlc();
                ReadSettingsFromPlc();
                updateTimer.Change(UPDATE_INTERVAL, Timeout.Infinite);
            }
            SimpleEventBus<bool>.Instance.Subscribe(
                "IsPlcConnectedChanged",
                (s, connected) =>
                {
                    if (connected)
                    {
                        WriteSettingsToPlc();
                        ReadSettingsFromPlc();
                        updateTimer.Change(UPDATE_INTERVAL, Timeout.Infinite);
                    }
                }
            );
            NavigatePage(typeof(MeasureView));
        }

        /// <summary>
        /// 轮询更新速度
        /// </summary>
        /// <param name="state"></param>
        private async void OnUpdateSpeedAndProgress(object state)
        {
            if (!plc.IsConnected) return;
            var buffer = new byte[4];
            try
            {
                await plc.ReadBytesAsync(buffer, DataType.DataBlock, 20, 400);
                if(BitConverter.IsLittleEndian) Array.Reverse(buffer);
                Speed = BitConverter.ToSingle(buffer, 0);
                await plc.ReadBytesAsync(buffer, DataType.DataBlock, 20, 434);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                Progress = BitConverter.ToInt16(buffer, 0);
            }
            finally
            {                
                updateTimer.Change(UPDATE_INTERVAL, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 收到错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorCode"></param>
        private void OnErrorReceived(object sender, int errorCode)
        {
            if (errorCode > ErrorList.Length - 1)
                return;
            var message = ErrorList[errorCode];
            Debug.WriteLine(message);
            Growl.Error(message);
        }

        private void WriteSettingsToPlc()
        {
            //机器设置
            plc.Write("DB20.DBW312.0", AppSetting.Default.MeasureTimes);
            plc.Write("DB20.DBD314.0", AppSetting.Default.SlipRange);
            plc.Write("DB20.DBW318.0", AppSetting.Default.WorkMode);
            plc.Write("DB20.DBW320.0", AppSetting.Default.PositioningMode);
            plc.Write("DB20.DBD308.0", AppSetting.Default.RefreshRate);
            //定位参数
            plc.Write("DB20.DBD120.0", AppSetting.Default.PositioningSpeed);
            plc.Write("DB20.DBD124.0", AppSetting.Default.AccelerationTime);
            plc.Write("DB20.DBD128.0", AppSetting.Default.DecelerationTime);
            plc.Write("DB20.DBD132.0", AppSetting.Default.PlusePerRound);
            plc.Write("DB20.DBD136.0", AppSetting.Default.PositioningCompensation);
            plc.Write("DB20.DBD140.0", AppSetting.Default.SlipCompensation);
            plc.Write("DB20.DBW160.0", AppSetting.Default.Slots);
            plc.Write("DB20.DBX301.4", AppSetting.Default.RotationDirection);
            plc.Write("DB20.DBX301.6", AppSetting.Default.SwitchType);
        }

        private void ReadSettingsFromPlc()
        {
            AddOrRemoveInfo.LeftAddOrRemove = (bool)plc.Read("DB20.DBX116.0");
            AddOrRemoveInfo.RightAddOrRemove = (bool)plc.Read("DB20.DBX116.1");
            AddOrRemoveInfo.StaticAddOrRemove = (bool)plc.Read("DB20.DBX116.2");
            AddOrRemoveInfo.ElectricCompensation = (bool)plc.Read("DB20.DBX440.2");
        }

        #region 命令
        [RelayCommand]
        public void ChangeMeasureStatus(string status)
        {
            if(Page is not MeasureView)
            {
                NavigatePage(typeof(MeasureView));
                return;
            }

            switch(status)
            {
                case "start":
                    IsMeasuring = true;                    
                    break;
                case "stop":
                    IsMeasuring = false;
                    break;
            }
            SimpleEventBus<bool>.Instance.Publish("MeasureStatusChanged", null, IsMeasuring);
        }

        [RelayCommand]
        public void NavigatePage(Type type)
        {
            if (Page?.GetType() == type)
                return;

            if(Page?.DataContext is ISaveChanged changeSaver && changeSaver.HasChanges)
            {
                //需要停止导航保存内容
                if (changeSaver.AskToSave() == true) return;
            }

            var view = App.Container.Resolve(type, IfUnresolved.Throw);
            Page = view as FrameworkElement;
        }             

        [RelayCommand]
        public static void Print()
        {
            var dialog = new PrintingDialog();
            Dialog.Show(dialog);
        }

        [RelayCommand]
        public void Exit()
        {
            if (Page?.DataContext is ISaveChanged changeSaver && changeSaver.HasChanges)
            {
                //需要停止导航保存内容
                if (changeSaver.AskToSave() == true) return;
            }
            App.Current.Shutdown();
        }
        #endregion
    }
}
