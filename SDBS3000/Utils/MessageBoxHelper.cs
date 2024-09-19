using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SDBS3000.Utils
{
    public static class MessageBoxHelper
    {
        public static bool AskToSave()
        {
            return HandyControl.Controls.MessageBox.Ask("确定要离开吗？更改还未保存") == MessageBoxResult.OK;
        }
    }
}
