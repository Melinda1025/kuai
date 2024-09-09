using CommunityToolkit.Mvvm.ComponentModel;

namespace SDBS3000.ViewModels
{
    public partial class KeyCompensationViewModel : ObservableObject
    {
        [ObservableProperty]
        private int step;

        [ObservableProperty]
        private string tip;
    }
}
