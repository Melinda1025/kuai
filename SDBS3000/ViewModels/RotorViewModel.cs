using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using SDBS3000.Core.Utils;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.Views.Dialogs;

namespace SDBS3000.ViewModels
{
    public partial class RotorViewModel : ObservableObject
    {
        /// <summary>
        /// 转子名称
        /// </summary>
        [ObservableProperty]
        private string rotorName;
        /// <summary>
        /// 解算模式
        /// </summary>
        [ObservableProperty]
        private int simulationMode = 1;
        /// <summary>
        /// 尺寸单位
        /// </summary>
        [ObservableProperty]
        private int sizeUnit;
        /// <summary>
        /// 测量模式
        /// </summary>
        [ObservableProperty]
        private int measurementMode = 3;
        /// <summary>
        /// 允许量单位
        /// </summary>
        [ObservableProperty]
        private int balenceUnit;
        /// <summary>
        /// 面板1的允许量
        /// </summary>
        [ObservableProperty]
        private double panel1MaxValue;
        /// <summary>
        /// 面板2的允许量
        /// </summary>
        [ObservableProperty]
        private double panel2MaxValue;
        /// <summary>
        /// 静允许量
        /// </summary>
        [ObservableProperty]
        private double staticMaxValue;


        [ObservableProperty]
        private double? r1;
        [ObservableProperty]
        private double? r2;
        [ObservableProperty]
        private double? a;
        [ObservableProperty]
        private double? b;
        [ObservableProperty]
        private double? c;
        [ObservableProperty]
        private double? speed;

        [ObservableProperty]
        private int supportMode = 1;

        public RotorViewModel()
        {
            SimpleEventBus<(double, double, double)>.Instance.Subscribe("CaculateISO_Complete", OnCaculateISO_Complete);
        }

        private void OnCaculateISO_Complete(object sender, (double panel1MaxUnbalence, double panel2MaxUnbalence, double staticMaxUnbalence) e)
        {
            Panel1MaxValue = e.panel1MaxUnbalence;
            Panel2MaxValue = e.panel2MaxUnbalence;
            StaticMaxValue = e.staticMaxUnbalence;
        }

        [RelayCommand]
        public async Task SelectSupportModeAsync()
        {
            var dialog = new SelectSupportModeDialog(SimulationMode);
            var result = await Dialog
                .Show(dialog, "MainContainer")
                .Initialize<SelectSupportModeViewModel>(vm => vm.SupportMode = SupportMode)
                .GetResultAsync<int>();
            if (result != -1)
                SupportMode = result;
        }

        [RelayCommand]
        public void CaculateISO()
        {
            var dialog = new ISODialog();
            Dialog.Show(dialog, "MainContainer").Initialize<ISOViewModel>(vm =>
            {
                vm.InitRadius(R1 ?? 0, R2 ?? 0);
                vm.Speed = Speed ?? 1000;
            });
        }
    }
}
