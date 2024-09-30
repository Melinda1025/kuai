using HandyControl.Controls;
using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Utils
{
    public static class GrowlHelper
    {
        public static void PlcNotConnected()
        {
            Growl.Error("PLC未连接".Localize());
        }
    }
}
