﻿using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;
using SDBS3000.Core.Utils;
using SDBS3000.Views;

namespace SDBS3000.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object content;

        public MainViewModel()
        {
            SimpleEventBus<Type>.Instance.Subscribe("Navigate", (s, t) => Navigate(t));
            Navigate(typeof(LoginView));
        }
        private void Navigate(Type type)
        {
            var view = App.Container.Resolve(type, IfUnresolved.Throw);
            Content = view;
        }
    }
}
