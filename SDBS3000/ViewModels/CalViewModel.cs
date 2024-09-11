using CommunityToolkit.Mvvm.ComponentModel;
using NLog;
using S7.Net;
using SDBS3000.Core.Utils;

namespace SDBS3000.ViewModels
{
    public partial class CalViewModel : ObservableObject
    {
        private readonly ExtendedPlc plc;
        private readonly Logger logger;
        public CalViewModel(ExtendedPlc plc, Logger logger)
        {
            this.plc = plc;
            this.logger = logger;
        }
    }
}
