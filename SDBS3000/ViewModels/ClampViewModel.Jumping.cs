using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    /// <summary>
    /// 夹具补偿 端径跳部分
    /// </summary>
    public partial class ClampViewModel
    {
        private readonly ExtendedPlc plc;
        public ClampViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
        }
    }
}
