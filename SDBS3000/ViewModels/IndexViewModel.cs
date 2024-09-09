using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using HandyControl.Controls;
using SDBS3000.Core.Utils;
using SDBS3000.Views.Dialogs;

namespace SDBS3000.ViewModels
{
    public partial class IndexViewModel : ObservableObject
    {
        [ObservableProperty]
        private object page;

        public IndexViewModel()
        {
            SimpleEventBus<Type>.Instance.Subscribe("NavigatePage", (s, t) => NavigatePage(t));
        }

        [RelayCommand]
        public void NavigatePage(Type type)
        {
            if (Page?.GetType() == type) return;
            var view = App.Container.Resolve(type, IfUnresolved.Throw);
            Page = view;
        }

        [RelayCommand]
        public void Print()
        {
            var dialog = new PrintingDialog();
            Dialog.Show(dialog);
        }


        [RelayCommand]
        public void Exit()
        {
            App.Current.Shutdown();
        }
    }
}
