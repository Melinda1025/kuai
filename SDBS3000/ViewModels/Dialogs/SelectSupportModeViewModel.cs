using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools.Extension;

namespace SDBS3000.ViewModels.Dialogs
{
    public partial class SelectSupportModeViewModel : ObservableObject, IDialogResultable<int>
    {
        [ObservableProperty]
        private int supportMode;

        public int Result { get; set; } = -1;
        public Action CloseAction { get; set; }

        [RelayCommand]
        public void ChangeMode(string mode)
        {
            SupportMode = Convert.ToInt32(mode);
        }

        [RelayCommand]
        public void SelectMode()
        {
            Result = SupportMode;
            CloseAction?.Invoke();
        }
    }
}
