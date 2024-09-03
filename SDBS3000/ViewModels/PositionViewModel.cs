using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using SDBS3000.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    public partial class PositionViewModel : ObservableObject
    {

        [RelayCommand]
        public async Task DetectPulseAsync()
        {
            await Task.Delay(2000);
        }

        [RelayCommand]
        public void ViewParameters()
        {
            var dialog = App.Container.Resolve<AxisParamsDialog>();
            Dialog.Show(dialog, "MainContainer");
        }
    }
}
