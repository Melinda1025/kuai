using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SDBS3000.Core.Utils;
using SDBS3000.Views;

namespace SDBS3000.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [RelayCommand]
        public void Login()
        {
            SimpleEventBus<Type>.Instance.Publish("Navigate", null, typeof(IndexView));
        }
    }
}
