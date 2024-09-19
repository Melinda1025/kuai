using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using SDBS3000.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SqlSugar;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.ViewModels
{
    public partial class MeasureViewModel : ObservableObject
    {
        [ObservableProperty]
        private RotorInfo rotor;

        [ObservableProperty]
        private BalanceItem panel1Item = new();
        [ObservableProperty]
        private BalanceItem panel2Item = new();

        [ObservableProperty]
        private DataPointList panel1Points = new();
        [ObservableProperty]
        private DataPointList panel2Points = new();
        [ObservableProperty]
        private DataPointList upRadialPoints = new();
        [ObservableProperty]
        private DataPointList downRadialPoints = new();
        [ObservableProperty]
        private DataPointList upLateralPoints = new();
        [ObservableProperty]
        private DataPointList downLateralPoints = new();



        [ObservableProperty]
        private string plcStatus = "未连接".Localize();

        public AddOrRemoveInfo AddOrRemoveInfo { get; init; }        

        private const int MEASURE_INTERVAL = 500;

        private readonly Timer measureTimer;
        private readonly ExtendedPlc plc;
        private readonly SqlSugarClient db;
        public MeasureViewModel(ExtendedPlc plc, AddOrRemoveInfo info, SqlSugarClient db)
        {
            this.db = db;
            this.plc = plc;            
            this.AddOrRemoveInfo = info;

            SimpleEventBus<bool>.Instance.Subscribe("IsPlcConnectedChanged", OnIsPlcConnectedChanged);
            SimpleEventBus<bool>.Instance.Subscribe("MeasureStatusChanged", OnMeasureStatusChanged);

            measureTimer = new Timer(OnUpdateData, null, Timeout.Infinite, Timeout.Infinite);

            //void OnUpdateData(object state)
            //{
            //    Points1.Clear();
            //    for (int i = 0; i < 360; i += 5)
            //    {
            //        Points1.Add(new DataPoint(1.1 - Random.Shared.NextDouble() * 0.2, i));
            //    }
            //    //OnPropertyChanged(nameof(Points1));
            //    Points1.NotifyChanged();
            //    timer.Change(20, Timeout.Infinite);
            //}
        }

        private async void OnUpdateData(object state)
        {
            try
            {
                if (!plc.IsConnected) return;

                var data = await plc.ReadStructAsync<MeasureData>(20, 376);
                if (data == null) return;

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
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                measureTimer.Change(MEASURE_INTERVAL, Timeout.Infinite);
            }
            
        }

        private void OnIsPlcConnectedChanged(object sender, bool isConnected)
        {
            PlcStatus = isConnected ? "已连接".Localize() : "未连接".Localize();
        }

        private void OnMeasureStatusChanged(object sender, bool isMeasure)
        {
            if(isMeasure)
            {
                measureTimer.Change(MEASURE_INTERVAL, Timeout.Infinite);
            }
            else
            {
                measureTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        [RelayCommand]
        public async Task OnLoadedAsync()
        {
            if(string.IsNullOrWhiteSpace(AppSetting.Default.RotorName))
            {
                Rotor = null;
            }
            else
            {
                Rotor = await db.Queryable<RotorInfo>().FirstAsync(info => info.Name == AppSetting.Default.RotorName);
            }
            if (!plc.IsConnected) return;
            var pageInfo = new PageInfo()
            {
                Measure = true,
            };
            await plc.WriteStructAsync(pageInfo, 20, 470);
        }
    }
}
