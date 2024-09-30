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
    /// <summary>
    /// 测量界面 端径跳部分
    /// </summary>
    public partial class MeasureViewModel : ObservableObject
    {       
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
                     
        private IntPtr DAQHandle;
        
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
                UpDiametricItem.ConvertCurrentDatas(buffer, start + samplePerChannel, end - start + 1);
                //DownDiametricItem.ConvertCurrentDatas(buffer, start + samplePerChannel * 2, end - start + 1);
                //UpEndItem.ConvertCurrentDatas(buffer, start + samplePerChannel * 3, end - start + 1);
                //DownEndItem.ConvertCurrentDatas(buffer, start + samplePerChannel * 4, end - start + 1);
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
