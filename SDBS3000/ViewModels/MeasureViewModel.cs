using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HarfBuzzSharp;
using NLog;
using OxyPlot;
using S7.Net.Types;
using SDBS3000.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.Views.Model;
using SqlSugar;

namespace SDBS3000.ViewModels
{
    public partial class MeasureViewModel : ObservableObject
    {
        [ObservableProperty]
        private RotorInfo rotor;

        [ObservableProperty]
        private BalanceItem panel1Item = new() { PanelType = PanelType.Panel1 };

        [ObservableProperty]
        private BalanceItem panel2Item = new() { PanelType = PanelType.Panel2 };

        /// <summary>
        /// 上径面
        /// </summary>
        [ObservableProperty]
        private JumpItemExtend upDiametricItem =
            new() { SurfaceType = SurfaceType.UpDiametricSurface };

        /// <summary>
        /// 下径面
        /// </summary>
        [ObservableProperty]
        private JumpItemExtend downDiametricItem =
            new() { SurfaceType = SurfaceType.DownDiametricSurface };

        /// <summary>
        /// 上端面
        /// </summary>
        [ObservableProperty]
        private JumpItemExtend upEndItem = new() { SurfaceType = SurfaceType.UpEndSurface };

        /// <summary>
        /// 下端面
        /// </summary>
        [ObservableProperty]
        private JumpItemExtend downEndItem = new() { SurfaceType = SurfaceType.DownEndSurface };

        [ObservableProperty]
        private string plcStatus = "未连接".Localize();

        public AddOrRemoveInfo AddOrRemoveInfo { get; init; }

        private const int BALENCE_MEASURE_INTERVAL = 500;
        private IntPtr DAQHandle;

        private readonly System.Threading.Timer balenceMeasureTimer;        
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

        private void OnIsPlcConnectedChanged(object sender, bool isConnected)
        {
            PlcStatus = isConnected ? "已连接".Localize() : "未连接".Localize();
        }

        private void OnMeasureStatusChanged(object sender, bool isMeasure)
        {
            if (isMeasure)
            {
                balenceMeasureTimer.Change(BALENCE_MEASURE_INTERVAL, Timeout.Infinite);
                //Task.Run(StartMeasureJumpingAsync);
                //var arr = new List<double>();
                Task.Run(async () =>
                {
                    double angle = 0;
                    UpDiametricItem.Reset();
                    DownDiametricItem.Reset();
                    UpEndItem.Reset();
                    DownEndItem.Reset();
                    while (PropertyAccessor<IndexViewModel, bool>.Instance.GetValue("IsMeasuring"))
                    {
                        if (angle >= 360)
                        {
                            angle = 0;
                            UpDiametricItem.Calculate(1);
                            DownDiametricItem.Calculate(2);
                            DownEndItem.Calculate(3);
                            UpEndItem.Calculate(4);
                            OnPropertyChanged(nameof(UpDiametricItem));
                            OnPropertyChanged(nameof(DownDiametricItem));
                            OnPropertyChanged(nameof(UpEndItem));
                            OnPropertyChanged(nameof(DownEndItem));
                            //UpDiametricItem.Points.Clear();
                            //UpDiametricItem.Reset();
                            //UpDiametricItem.Buffer.Clear();
                            StopMeasureJumping();
                            return;
                        }
                        angle += 1;
                        UpDiametricItem.Buffer.Add(Random.Shared.NextDouble() * 2 + 1);
                        DownDiametricItem.Buffer.Add(Random.Shared.NextDouble() * 2 + 1);
                        UpEndItem.Buffer.Add(Random.Shared.NextDouble() * 2 + 1);
                        DownEndItem.Buffer.Add(Random.Shared.NextDouble() * 2 + 1);
                        //UpDiametricItem.Jumping = UpDiametricItem.Buffer[^1];
                        //OnPropertyChanged(nameof(UpDiametricItem));

                        double add = 0;
                        if (UpDiametricItem.Buffer.Count > 0)
                        {
                            var max = UpDiametricItem.Buffer.Max();
                            var min = UpDiametricItem.Buffer.Min();
                            var delta = max - min;
                            var radius = Math.Max(2.5, delta * 2.5);
                            add = radius;
                        }
                        UpDiametricItem.Points.Clear();
                        for (int i = 0; i < UpDiametricItem.Buffer.Count; i++)
                        {
                            UpDiametricItem.Points.Add(new DataPoint(add + UpDiametricItem.Buffer[i], i + 1));
                        }

                        add = 0;
                        if (DownDiametricItem.Buffer.Count > 0)
                        {
                            var max = DownDiametricItem.Buffer.Max();
                            var min = DownDiametricItem.Buffer.Min();
                            var delta = max - min;
                            var radius = Math.Max(2.5, delta * 2.5);
                            add = radius;
                        }
                        DownDiametricItem.Points.Clear();
                        for (int i = 0; i < DownDiametricItem.Buffer.Count; i++)
                        {
                            DownDiametricItem.Points.Add(new DataPoint(add + DownDiametricItem.Buffer[i], i + 1));
                        }

                        add = 0;
                        if (UpEndItem.Buffer.Count > 0)
                        {
                            var max = UpEndItem.Buffer.Max();
                            var min = UpEndItem.Buffer.Min();
                            var delta = max - min;
                            var radius = Math.Max(2.5, delta * 2.5);
                            add = radius;
                        }
                        UpEndItem.Points.Clear();
                        for (int i = 0; i < UpEndItem.Buffer.Count; i++)
                        {
                            UpEndItem.Points.Add(new DataPoint(add + UpEndItem.Buffer[i], i + 1));
                        }

                        add = 0;
                        if (DownEndItem.Buffer.Count > 0)
                        {
                            var max = DownEndItem.Buffer.Max();
                            var min = DownEndItem.Buffer.Min();
                            var delta = max - min;
                            var radius = Math.Max(2.5, delta * 2.5);
                            add = radius;
                        }
                        DownEndItem.Points.Clear();
                        for (int i = 0; i < DownEndItem.Buffer.Count; i++)
                        {
                            DownEndItem.Points.Add(new DataPoint(add + DownEndItem.Buffer[i], i + 1));
                        }

                        UpDiametricItem.Points.NotifyChanged();
                        DownDiametricItem.Points.NotifyChanged();
                        UpEndItem.Points.NotifyChanged();
                        DownEndItem.Points.NotifyChanged();
                        //OnPropertyChanged(nameof(UpDiametricItem));
                        await Task.Delay(10);
                    }
                });
            }
            else
            {
                balenceMeasureTimer.Change(Timeout.Infinite, Timeout.Infinite);
                StopMeasureJumping();
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

        private const string TASK_NAME = "MeasureJumping";
        public async Task StartMeasureJumpingAsync()
        {            
            //清除绘制的曲线
            UpDiametricItem.Reset();
            DownDiametricItem.Reset();
            UpEndItem.Reset();
            DownEndItem.Reset();
            OnPropertyChanged(nameof(UpDiametricItem));
            OnPropertyChanged(nameof(DownDiametricItem));
            OnPropertyChanged(nameof(UpEndItem));
            OnPropertyChanged(nameof(DownEndItem));

            DAQHandle = IntPtr.Zero;
            int errorCode = 0;
            //创建采集卡任务
            errorCode = ArtDAQ.ArtDAQ_CreateTask(TASK_NAME, out DAQHandle);
            if (errorCode < 0)
            {
                HandleDAQError();
                return;
            }
            errorCode = ArtDAQ.ArtDAQ_CreateAICurrentChan(
                DAQHandle,
                "Dev2/ai0",
                "0",
                ArtDAQ.ArtDAQ_Val_Cfg_Default,
                0.004,
                0.02,
                ArtDAQ.ArtDAQ_Val_Amps,
                ArtDAQ.ArtDAQ_Val_Internal,
                259,
                null
            );
            if (errorCode < 0)
            {
                HandleDAQError();
                return;
            }
            errorCode = ArtDAQ.ArtDAQ_CreateAICurrentChan(
                DAQHandle,
                "Dev2/ai1",
                "1",
                ArtDAQ.ArtDAQ_Val_Cfg_Default,
                0.004,
                0.02,
                ArtDAQ.ArtDAQ_Val_Amps,
                ArtDAQ.ArtDAQ_Val_Internal,
                259,
                null
            );
            if (errorCode < 0)
            {
                HandleDAQError();
                return;
            }
            //配置时钟
            const int readInterval = 100;
            const int bufferSize = 40; //const int bufferSize = 100;
            errorCode = ArtDAQ.ArtDAQ_CfgSampClkTiming(
                DAQHandle,
                string.Empty,
                readInterval,
                ArtDAQ.ArtDAQ_Val_Rising,
                ArtDAQ.ArtDAQ_Val_ContSamps,
                bufferSize
            );
            if (errorCode < 0)
            {
                HandleDAQError();
                return;
            }
            //启动采集卡任务
            errorCode = ArtDAQ.ArtDAQ_StartTask(DAQHandle);
            if (errorCode < 0)
            {
                HandleDAQError();
                return;
            }

            var buffer = new double[bufferSize];
            //0 未接触到光电纸
            //1 第一次接触到光电纸
            //2 第一次离开光电纸
            //3 第二次接触到光电纸
            int completeFlag = 0;
            //int readCount = 0;
            //采集时长（ms），一次循环加一次
            double measureTime = 0;
            var stopwatch = new Stopwatch();
            while (completeFlag < 3)
            {
                if (!PropertyAccessor<IndexViewModel, bool>.Instance.GetValue("IsMeasuring"))
                {
                    StopMeasureJumping();
                    return;
                }
                stopwatch.Restart();
                const int samplePerChannel = 10;
                errorCode = await Task.Run(() =>
                    {
                        return ArtDAQ.ArtDAQ_ReadAnalogF64(
                            DAQHandle,
                            samplePerChannel,
                            10.0,
                            ArtDAQ.ArtDAQ_Val_GroupByChannel,
                            buffer,
                            samplePerChannel * 2, //samplePerChannel * 5,
                            out _,
                            IntPtr.Zero
                        );
                    })
                    .ConfigureAwait(false);
                stopwatch.Stop();
                if (errorCode < 0)
                {
                    HandleDAQError();
                    return;
                }
                const double threshold = 0.005;
                int start = 0;
                int end = samplePerChannel - 1;
                //光电通道
                for (int i = 0; i < samplePerChannel; i++)
                {
                    //第一次接触到光电纸
                    if (completeFlag == 0 && buffer[i] >= threshold)
                    {
                        completeFlag++;
                        //记录起始点
                        start = i;
                    }
                    //第一次离开光电纸
                    else if (completeFlag == 1 && buffer[i] < threshold)
                    {
                        completeFlag++;
                    }
                    //第二次接触到光电纸
                    else if (completeFlag == 2 && buffer[i] >= threshold)
                    {
                        completeFlag++;
                        //记录终点
                        end = i;
                        break;
                    }
                }
                //未开始采集则跳过
                if (completeFlag == 0)
                    continue;
                measureTime +=
                    (double)(end - start + 1) / samplePerChannel * stopwatch.ElapsedMilliseconds;
                //加入采集数据缓存
                var totalBufferSpan  = new ReadOnlySpan<double>(buffer);
                UpDiametricItem.ConvertCurrentDatas(totalBufferSpan.Slice(start + samplePerChannel, end - start + 1));                                
                //DownDiametricItem.ConvertCurrentDatas(totalBufferSpan.Slice(start + samplePerChannel * 2, end - start + 1));                                
                //UpEndItem.ConvertCurrentDatas(totalBufferSpan.Slice(start + samplePerChannel * 3, end - start + 1));                                
                //DownEndItem.ConvertCurrentDatas(totalBufferSpan.Slice(start + samplePerChannel * 4, end - start + 1));
            }

            //夹具补偿
            if(AppSetting.Default.ClampCompensatingItem != null)
            {
                AppSetting.Default.ClampCompensatingItem.Compensate(UpDiametricItem.Buffer);
                AppSetting.Default.ClampCompensatingItem.Compensate(DownDiametricItem.Buffer);
                AppSetting.Default.ClampCompensatingItem.Compensate(UpEndItem.Buffer);
                AppSetting.Default.ClampCompensatingItem.Compensate(DownEndItem.Buffer);
            }
            
            //计算转速
            var rotorSpeed = (float)(1 / measureTime * 1000 * 60);
            PropertyAccessor<IndexViewModel, float>.Instance.SetValue("Speed", rotorSpeed);

            //计算结果
            UpDiametricItem.Calculate();
            DownDiametricItem.Calculate();
            UpEndItem.Calculate();
            DownEndItem.Calculate();
            OnPropertyChanged(nameof(UpDiametricItem));
            OnPropertyChanged(nameof(DownDiametricItem));
            OnPropertyChanged(nameof(UpEndItem));
            OnPropertyChanged(nameof(DownEndItem));

            //MessageBox.Show(UpDiametricItem.TraceInfo());

            StopMeasureJumping();
        }

        public void StopMeasureJumping()
        {
            PropertyAccessor<IndexViewModel, bool>.Instance.SetValue("IsMeasuring", false);
            //UpDiametricItem.Points.Clear();
            //DownDiametricItem.Points.Clear();
            //UpEndItem.Points.Clear();
            //DownEndItem.Points.Clear();
            if (DAQHandle == IntPtr.Zero)
                return;
            _ = ArtDAQ.ArtDAQ_StopTask(DAQHandle);
            _ = ArtDAQ.ArtDAQ_ClearTask(DAQHandle);
            DAQHandle = IntPtr.Zero;
        }

        private void HandleDAQError()
        {
            var buffer = new byte[2048];
            try
            {
                ArtDAQ.ArtDAQ_GetExtendedErrorInfo(buffer, 2048);
                var message = $"数据采集卡发生错误：{Encoding.UTF8.GetString(buffer)}";
                logger.Error(message);
                Growl.Error(message);
            }
            finally
            {                
                if(PropertyAccessor<IndexViewModel, bool>.Instance.GetValue("IsMeasuring"))
                {
                    StopMeasureJumping();
                }                
            }
        }
    }
}
