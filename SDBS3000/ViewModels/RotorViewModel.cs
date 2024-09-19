using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.Utils;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.ViewModels.Interface;
using SDBS3000.Views.Dialogs;
using SqlSugar;

namespace SDBS3000.ViewModels
{
    public partial class RotorViewModel : ObservableObject, ISaveChanged
    {
        #region 弃用
        ///// <summary>
        ///// 转子名称
        ///// </summary>
        //[ObservableProperty]
        //private string rotorName;
        ///// <summary>
        ///// 解算模式
        ///// </summary>
        //[ObservableProperty]
        //private int simulationMode = 1;
        ///// <summary>
        ///// 尺寸单位
        ///// </summary>
        //[ObservableProperty]
        //private int sizeUnit;
        ///// <summary>
        ///// 测量模式
        ///// </summary>
        //[ObservableProperty]
        //private int measurementMode = 3;
        ///// <summary>
        ///// 允许量单位
        ///// </summary>
        //[ObservableProperty]
        //private int balenceUnit;
        ///// <summary>
        ///// 面板1的允许量
        ///// </summary>
        //[ObservableProperty]
        //private double panel1MaxValue;
        ///// <summary>
        ///// 面板2的允许量
        ///// </summary>
        //[ObservableProperty]
        //private double panel2MaxValue;
        ///// <summary>
        ///// 静允许量
        ///// </summary>
        //[ObservableProperty]
        //private double staticMaxValue;


        //[ObservableProperty]
        //private double? r1;
        //[ObservableProperty]
        //private double? r2;
        //[ObservableProperty]
        //private double? a;
        //[ObservableProperty]
        //private double? b;
        //[ObservableProperty]
        //private double? c;
        //[ObservableProperty]
        //private double? speed;

        //[ObservableProperty]
        //private int supportMode = 1;
        #endregion
        private RotorInfo currentRotor;
        public RotorInfo CurrentRotor
        {
            get => currentRotor;
            set
            {
                if (value == currentRotor)
                    return;
                SetProperty(ref currentRotor, value);
                if (IsEditing)
                {
                    IsEditing = false;
                    IsAddNew = false;
                }
            }
        }

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private bool isAddNew;

        public ObservableCollection<RotorInfo> RotorInfos { get; } = new();
        public bool HasChanges
        {
            get => IsEditing;
            set => throw new NotImplementedException();
        }
        public bool AskToSave() => MessageBoxHelper.AskToSave();

        private readonly SqlSugarClient db;
        public RotorViewModel(SqlSugarClient db)
        {
            this.db = db;
            SimpleEventBus<(float, float, float)>.Instance.Subscribe(
                "CaculateISO_Complete",
                OnCaculateISO_Complete
            );
        }

        private void OnCaculateISO_Complete(
            object sender,
            (float panel1MaxUnbalence, float panel2MaxUnbalence, float staticMaxUnbalence) e
        )
        {
            if (CurrentRotor == null)
                return;
            CurrentRotor.Panel1MaxValue = e.panel1MaxUnbalence;
            CurrentRotor.Panel2MaxValue = e.panel2MaxUnbalence;
            CurrentRotor.StaticMaxValue = e.staticMaxUnbalence;
            OnPropertyChanged(nameof(CurrentRotor));
        }

        [RelayCommand]
        public async Task OnLoadedAsync()
        {
            RotorInfos.Clear();
            if (await db.Queryable<RotorInfo>().AnyAsync())
            {
                var rotors = await db.Queryable<RotorInfo>().ToListAsync();
                foreach (var rotor in rotors)
                {
                    RotorInfos.Add(rotor);
                    if (rotor.Name == AppSetting.Default.RotorName)
                        CurrentRotor = rotor;
                }
                CurrentRotor ??= RotorInfos[0];
            }
            else
            {
                CurrentRotor = new();
            }
        }

        [RelayCommand]
        public void OnUnload()
        {
            if (RotorInfos.Contains(CurrentRotor))
            {
                AppSetting.Default.RotorName = CurrentRotor.Name;
            }
            else
            {
                AppSetting.Default.RotorName = string.Empty;
            }
        }

        //[RelayCommand]
        //public async Task SelectSupportModeAsync()
        //{
        //    var dialog = new SelectSupportModeDialog(SimulationMode);
        //    var result = await Dialog
        //        .Show(dialog, "MainContainer")
        //        .Initialize<SelectSupportModeViewModel>(vm => vm.SupportMode = SupportMode)
        //        .GetResultAsync<int>();
        //    if (result != -1)
        //        SupportMode = result;
        //}
        [RelayCommand]
        public void AddRotor()
        {
            CurrentRotor = new RotorInfo();
            IsAddNew = true;
            IsEditing = true;
        }

        [RelayCommand]
        public void CaculateISO()
        {
            var dialog = new ISODialog();
            Dialog
                .Show(dialog, "MainContainer")
                .Initialize<ISOViewModel>(vm =>
                {
                    vm.InitRadius(CurrentRotor.R1 ?? 0, CurrentRotor.R2 ?? 0);
                    vm.Speed = CurrentRotor.Speed ?? 1000;
                });
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentRotor.Name))
            {
                Growl.Warning("转子名称不能为空");
                return;
            }
            if (CurrentRotor.Name.Length > 10)
            {
                Growl.Warning("转子名称过长");
                return;
            }
            
            if (
                CurrentRotor.Panel1MaxValue == null
                || CurrentRotor.Panel2MaxValue == null
                || CurrentRotor.StaticMaxValue == null
                || CurrentRotor.A == null
                || CurrentRotor.B == null
                || CurrentRotor.C == null
                || CurrentRotor.R1 == null
                || CurrentRotor.R2 == null
                || CurrentRotor.Speed == null
                || CurrentRotor.BalenceUnit == -1
                || CurrentRotor.SizeUnit == -1
            )
            {
                Growl.Warning("转子参数不能为空");
                return;
            }

            if (
                IsAddNew
                && await db.Queryable<RotorInfo>()
                    .AnyAsync(info => info.Name == CurrentRotor.Name && info.Id != CurrentRotor.Id)
            )
            {
                Growl.Warning("转子名称重复");
                return;
            }
            else if(!IsAddNew)
            {
                var oldRotor = await db.Queryable<RotorInfo>().FirstAsync(info => info.Id == CurrentRotor.Id);
                if(CurrentRotor.Equals(oldRotor)) return;
            }

            if (HandyControl.Controls.MessageBox.Ask("确定要保存这个转子吗?") != MessageBoxResult.OK) return;

            if (IsAddNew)
            {
                await db.Insertable(CurrentRotor).ExecuteCommandAsync();
                RotorInfos.Add(CurrentRotor);
            }
            else
            {
                await db.Updateable(CurrentRotor).ExecuteCommandAsync();
            }            
            AppSetting.Default.RotorName = CurrentRotor.Name;
            Growl.Success("保存成功");
            IsAddNew = false;
            IsEditing = false;
        }

        [RelayCommand]
        public async Task DeleteAsync()
        {
            if (CurrentRotor == null)
                return;
            if (await db.Queryable<RotorInfo>().AnyAsync(info => info.Name == CurrentRotor.Name))
            {
                if (
                    HandyControl.Controls.MessageBox.Ask("确定要删除这个转子吗?")
                    == MessageBoxResult.OK
                )
                {
                    await db.Deleteable(CurrentRotor).ExecuteCommandAsync();
                    RotorInfos.Remove(CurrentRotor);
                    if (CurrentRotor.Name == AppSetting.Default.RotorName)
                    {
                        if (RotorInfos.Count > 0)
                        {
                            CurrentRotor = RotorInfos[0];
                            AppSetting.Default.RotorName = CurrentRotor.Name;
                        }
                        else
                        {
                            CurrentRotor = new();
                            AppSetting.Default.RotorName = string.Empty;
                        }
                    }
                }
            }
            else
            {
                CurrentRotor = new();
            }
        }           
    }
}
