using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.Utils;
using SDBS3000.ViewModels.Interface;
using SDBS3000.Views.Dialogs;

namespace SDBS3000.ViewModels
{
    public partial class PositionViewModel : ObservableObject, ISaveChanged
    {        
        private bool rotationDirection;
        /// <summary>
        /// 转子方向 true逆时针 false顺时针
        /// </summary>
        public bool RotationDirection
        {
            get => rotationDirection;
            set
            {
                if (value != rotationDirection)
                {
                    SetProperty(ref rotationDirection, value);
                    HasChanges = true;
                }
            }
        }

        
        private bool switchType;
        /// <summary>
        /// 开关类型 0光电开关 1接近开关(数槽)
        /// </summary>
        public bool SwitchType
        {
            get => switchType;
            set
            {
                if (value != switchType)
                {
                    SetProperty(ref switchType, value);
                    HasChanges = true;
                }
            }
        }
        
        private float plusePerRound;
        /// <summary>
        /// 每转脉冲数
        /// </summary>
        public float PlusePerRound
        {
                        get => plusePerRound;
            set
            {
                if (value != plusePerRound)
                {
                    SetProperty(ref plusePerRound, value);
                    HasChanges = true;
                }
            }
        }
        
        private int slots;
        /// <summary>
        /// 转子槽数
        /// </summary>
        public int Slots
        {
            get => slots;
            set
            {
                if (value != slots)
                {
                    SetProperty(ref slots, value);
                    HasChanges = true;
                }
            }
        }
        
        private float positioningSpeed;
        /// <summary>
        /// 定位转速
        /// </summary>
        public float PositioningSpeed
        {
            get => positioningSpeed;
            set
            {
                if (value != positioningSpeed)
                {
                    SetProperty(ref positioningSpeed, value);
                    HasChanges = true;
                }
            }
        }
        
        private float accelerationTime;
        /// <summary>
        /// 加速时间
        /// </summary>
        public float AccelerationTime
        {
            get => accelerationTime;
            set
            {
                if (value != accelerationTime)
                {
                    SetProperty(ref accelerationTime, value);
                    HasChanges = true;
                }
            }
        }
        
        private float decelerationTime;
        /// <summary>
        /// 减速时间
        /// </summary>
        public float DecelerationTime
        {
            get => decelerationTime;
            set
            {
                if (value != decelerationTime)
                {
                    SetProperty(ref decelerationTime, value);
                    HasChanges = true;
                }
            }
        }
        
        private float positioningCompensation;
        /// <summary>
        /// 定位补偿
        /// </summary>
        public float PositioningCompensation
        {
            get => positioningCompensation;
            set
            {
                if (value != positioningCompensation)
                {
                    SetProperty(ref positioningCompensation, value);
                    HasChanges = true;
                }
            }
        }
        
        private float slipCompensation;
        /// <summary>
        /// 转差补偿
        /// </summary>
        public float SlipCompensation
        {
            get => slipCompensation;
            set
            {
                if (value != slipCompensation)
                {
                    SetProperty(ref slipCompensation, value);
                    HasChanges = true;
                }
            }
        }

        [ObservableProperty]
        private bool isDetecting;

        public bool HasChanges { get; set; } = false;
        public bool AskToSave() => MessageBoxHelper.AskToSave();


        private readonly ExtendedPlc plc;
        private const int TIMER_INTERVAL = 100;
        private readonly Timer detectTimer;
        public PositionViewModel(ExtendedPlc plc)
        {
            this.plc = plc;            
            detectTimer = new(OnDetecting, null, Timeout.Infinite, Timeout.Infinite);
            
            rotationDirection = AppSetting.Default.RotationDirection;
            switchType = AppSetting.Default.SwitchType;
            plusePerRound = AppSetting.Default.PlusePerRound;
            slots = AppSetting.Default.Slots;
            positioningSpeed = AppSetting.Default.PositioningSpeed;
            accelerationTime = AppSetting.Default.AccelerationTime;
            decelerationTime = AppSetting.Default.DecelerationTime;
            positioningCompensation = AppSetting.Default.PositioningCompensation;
            slipCompensation = AppSetting.Default.SlipCompensation;            
        }

        /// <summary>
        /// 检测脉冲 （逻辑不确定）
        /// </summary>
        /// <param name="state"></param>
        private async void OnDetecting(object state)
        {
            if (!plc.IsConnected) return;
            try
            {
                IsDetecting = (bool)await plc.ReadAsync("DB20.DBX300.0");
                if (!IsDetecting)
                {
                    PlusePerRound = (float)await plc.ReadAsync("DB20.DBD132.0");
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                //当检测脉冲为false时，读取脉冲数，停止检测
                if (IsDetecting)
                {
                    detectTimer.Change(TIMER_INTERVAL, Timeout.Infinite);
                }
            }                                               
        }

        [RelayCommand]
        public async Task OnloadAsync()
        {
            if (!plc.IsConnected) return;
            var pageInfo = new PageInfo()
            {
                Positioning = true
            };
            await plc.WriteStructAsync(pageInfo, 20, 470);
        }

        /// <summary>
        /// 开始/停止检测脉冲
        /// </summary>
        [RelayCommand]
        public void DetectPulse()
        {
            if(!plc.IsConnected)
            {
                IsDetecting = false;
                Growl.Warning("PLC未连接");
                return;
            }            
            if(IsDetecting)
            {
                plc.Write("DB20.DBX300.0", true);
                detectTimer.Change(0, Timeout.Infinite);
            }
            else
            {
                plc.Write("DB20.DBX300.0", false);
                detectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            
            AppSetting.Default.RotationDirection = RotationDirection;
            AppSetting.Default.SwitchType = SwitchType;
            AppSetting.Default.PlusePerRound = PlusePerRound;
            AppSetting.Default.Slots = Slots;
            AppSetting.Default.PositioningSpeed = PositioningSpeed;
            AppSetting.Default.AccelerationTime = AccelerationTime;
            AppSetting.Default.DecelerationTime = DecelerationTime;
            AppSetting.Default.PositioningCompensation = PositioningCompensation;
            AppSetting.Default.SlipCompensation = SlipCompensation;
            AppSetting.Default.Save();
            HasChanges = false;
            Growl.Success("保存配置成功");
            if (!plc.IsConnected) return;
            await plc.WriteAsync("DB20.DBD120.0", PositioningSpeed);
            await plc.WriteAsync("DB20.DBD124.0", AccelerationTime);
            await plc.WriteAsync("DB20.DBD128.0", DecelerationTime);
            await plc.WriteAsync("DB20.DBD132.0", PlusePerRound);
            await plc.WriteAsync("DB20.DBD136.0", PositioningCompensation);
            await plc.WriteAsync("DB20.DBD140.0", SlipCompensation);
            await plc.WriteAsync("DB20.DBD160.0", Slots);
            await plc.WriteAsync("DB20.DBX301.4", RotationDirection);
            await plc.WriteAsync("DB20.DBX301.6", SwitchType);
            Growl.Success("写入PLC成功");
        }                         
                                  
        [RelayCommand]
        public static void ViewParameters()
        {
            var dialog = App.Container.Resolve<AxisParamsDialog>();
            Dialog.Show(dialog, "MainContainer");
        }
    }
}
