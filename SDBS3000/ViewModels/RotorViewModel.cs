using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    public partial class RotorViewModel : ObservableObject
    {
        [ObservableProperty]
        private int supportMode = 1;
        [ObservableProperty]
        private int simulationMode = 1;
        [ObservableProperty]
        private int measurementMode = 3;

        [RelayCommand]
        public async Task SelectSupportModeAsync()
        {
            var dialog = new SelectSupportModeDialog(SimulationMode);
            var result = await Dialog
                .Show(dialog, "MainContainer")
                .Initialize<SelectSupportModeViewModel>(vm => vm.SupportMode = SupportMode)
                .GetResultAsync<int>();
            if(result != -1) SupportMode = result;
        }
    }
}
