using CommunityToolkit.Mvvm.ComponentModel;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    public partial class MeasureViewModel : ObservableObject
    {
        public AddOrRemoveInfo AddOrRemoveInfo { get; init; }

        private readonly ExtendedPlc plc;
        public MeasureViewModel(ExtendedPlc plc, AddOrRemoveInfo info)
        {
            this.plc = plc;            
            this.AddOrRemoveInfo = info;
        }

        
    }
}
