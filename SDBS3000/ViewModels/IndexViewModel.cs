using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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
        private void NavigatePage(Type type)
        {
            var view = App.Container.Resolve(type, IfUnresolved.Throw);
            Page = view;
        }
    }
}
