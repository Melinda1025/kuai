using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using SDBS3000.Views.Dialogs;

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
