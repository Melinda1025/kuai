using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
