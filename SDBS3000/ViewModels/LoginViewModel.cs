using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SDBS3000.Core.Utils;
using SDBS3000.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
