using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using OxyPlot;
using SDBS3000.Core.Models;
using SDBS3000.Core.Models.PlcStructs;
using SDBS3000.Core.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    /// <summary>
    /// 测量界面 动平衡部分
    /// </summary>
    public partial class MeasureViewModel
    {
        [ObservableProperty]
        private RotorInfo rotor;

        [ObservableProperty]
        private string plcStatus = "离线".Localize();

        [ObservableProperty]
        private BalanceItem panel1Item = new() { PanelType = PanelType.Panel1 };

        [ObservableProperty]
        private BalanceItem panel2Item = new() { PanelType = PanelType.Panel2 };

        public AddOrRemoveInfo AddOrRemoveInfo { get; init; }

        private readonly System.Threading.Timer balenceMeasureTimer;
        private const int BALENCE_MEASURE_INTERVAL = 500;

        private readonly ExtendedPlc plc;
        private readonly SqlSugarClient db;
        private readonly Logger logger;

        public MeasureViewModel(
            ExtendedPlc plc,
            AddOrRemoveInfo info,
            SqlSugarClient db,
            Logger logger
        )
        {
            this.db = db;
            this.plc = plc;
            this.AddOrRemoveInfo = info;
            this.logger = logger;

            SimpleEventBus<bool>.Instance.Subscribe(
                "IsPlcConnectedChanged",
                OnIsPlcConnectedChanged
            );
            SimpleEventBus<bool>.Instance.Subscribe("MeasureStatusChanged", OnMeasureStatusChanged);

            balenceMeasureTimer = new System.Threading.Timer(
                OnUpdateBalenceData,
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        private void OnIsPlcConnectedChanged(object sender, bool isConnected)
        {
            PlcStatus = isConnected ? "在线".Localize() : "离线".Localize();
        }

        private async void OnUpdateBalenceData(object state)
        {
            try
            {
                if (!plc.IsConnected)
                    return;

                var data = await plc.ReadStructAsync<MeasureData>(20, 376);
                if (data == null)
                    return;

                Panel1Item.Value = data.Value.Panel1Value;
                Panel1Item.Angle = data.Value.Panel1Angle;
                Panel1Item.LastValue = data.Value.LastPanel1Value;
                Panel1Item.LastAngle = data.Value.LastPanel1Angle;
                Panel1Item.Signal = data.Value.Panel1Signal;

                Panel2Item.Value = data.Value.Panel2Value;
                Panel2Item.Angle = data.Value.Panel2Angle;
                Panel2Item.LastValue = data.Value.LastPanel2Value;
                Panel2Item.LastAngle = data.Value.LastPanel2Angle;
                Panel2Item.Signal = data.Value.Panel2Signal;

                PropertyAccessor<IndexViewModel, float>.Instance.SetValue(
                    "Speed",
                    data.Value.Speed
                );
                PropertyAccessor<IndexViewModel, float>.Instance.SetValue(
                    "Progress",
                    (float)data.Value.Progress / AppSetting.Default.MeasureTimes
                );
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                balenceMeasureTimer.Change(BALENCE_MEASURE_INTERVAL, Timeout.Infinite);
            }
        }

        [RelayCommand]
        public async Task OnLoadedAsync()
        {
            if (string.IsNullOrWhiteSpace(AppSetting.Default.RotorName))
            {
                Rotor = null;
            }
            else
            {
                Rotor = await db.Queryable<RotorInfo>()
                    .FirstAsync(info => info.Name == AppSetting.Default.RotorName);
            }
            if (!plc.IsConnected)
                return;
            var pageInfo = new PageInfo() { Measure = true };
            await plc.WriteStructAsync(pageInfo, 20, 470);
        }

        private void OnMeasureStatusChanged(object sender, bool isMeasure)
        {
            if (isMeasure)
            {
                //balenceMeasureTimer.Change(BALENCE_MEASURE_INTERVAL, Timeout.Infinite);
                //Task.Run(StartMeasureJumpingAsync);                
                Task.Run(async () =>
                {                    
                    UpDiametricItem.Reset();
                    DownDiametricItem.Reset();
                    UpEndItem.Reset();
                    DownEndItem.Reset();

                    while(PropertyAccessor<IndexViewModel, bool>.Instance.GetValue("IsMeasuring"))
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                        var datas = Enumerable.Range(0, 360).Select(i => Random.Shared.NextDouble() * 0.01 + 0.004).ToArray();
                        UpDiametricItem.Reset();
                        UpDiametricItem.ConvertCurrentDatas(datas, 0, datas.Length);
                        UpDiametricItem.Calculate(3.1);
                        UpDiametricItem.Points.NotifyChanged();

                        datas = datas.Select(i => Random.Shared.NextDouble() * 0.01 + 0.004).ToArray();
                        DownDiametricItem.Reset();
                        DownDiametricItem.ConvertCurrentDatas(datas, 0, datas.Length);
                        DownDiametricItem.Calculate(3.1);
                        DownDiametricItem.Points.NotifyChanged();

                        datas = datas.Select(i => Random.Shared.NextDouble() * 0.01 + 0.004).ToArray();
                        UpEndItem.Reset();
                        UpEndItem.ConvertCurrentDatas(datas, 0, datas.Length);
                        UpEndItem.Calculate(3.1);
                        UpEndItem.Points.NotifyChanged();

                        datas = datas.Select(i => Random.Shared.NextDouble() * 0.01 + 0.004).ToArray();
                        DownEndItem.Reset();
                        DownEndItem.ConvertCurrentDatas(datas, 0, datas.Length);
                        DownEndItem.Calculate(3.1);
                        DownEndItem.Points.NotifyChanged();

                        OnPropertyChanged(nameof(UpDiametricItem));
                        OnPropertyChanged(nameof(DownDiametricItem));
                        OnPropertyChanged(nameof(UpEndItem));
                        OnPropertyChanged(nameof(DownEndItem));
                    }
                    StopMeasureJumping();
                });
                Task.Run(async () =>
                {
                    float angle = 0;
                    while (PropertyAccessor<IndexViewModel, bool>.Instance.GetValue("IsMeasuring"))
                    {                        
                        await Task.Delay(10).ConfigureAwait(false);
                        angle += 1;
                        if (angle > 360) angle = 0;                        
                        
                        Panel1Item.Angle = angle;                        
                        Panel2Item.Angle = angle;

                        Panel1Item.Value = (float)(30 + Math.Sin(angle / 180 * Math.PI) * 10);
                        Panel2Item.Value = (float)(30 + Math.Cos(angle / 180 * Math.PI) * 10);

                        Panel1Item.Signal = (short)(50 + Math.Sin(angle / 180 * Math.PI) * 50);
                        Panel2Item.Signal = (short)(50 + Math.Cos(angle / 180 * Math.PI) * 50);

                        PropertyAccessor<IndexViewModel, float>.Instance.SetValue("Speed", 1000 + (float)Math.Sin(angle / 180 * Math.PI) * 10);
                        PropertyAccessor<IndexViewModel, float>.Instance.SetValue("Progress", angle / 360);

                        OnPropertyChanged(nameof(Panel1Item));
                        OnPropertyChanged(nameof(Panel2Item));
                    }
                    Panel1Item.LastValue = Panel1Item.Value;
                    Panel1Item.LastAngle = Panel1Item.Angle;
                    Panel2Item.LastValue = Panel2Item.Value;
                    Panel2Item.LastAngle = Panel2Item.Angle;
                });
            }
            else
            {
                balenceMeasureTimer.Change(Timeout.Infinite, Timeout.Infinite);
                StopMeasureJumping();
            }
        }
    }
}
