using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels.Interface
{
    /// <summary>
    /// 切换窗口时是否需要保存
    /// </summary>
    public interface ISaveChanged
    {
        bool HasChanges { get; set; }        
        /// <summary>
        /// 询问是否需要保存
        /// </summary>
        /// <returns>true 表示需要保存，取消切换窗口</returns>
        /// <returns>false 表示不需要保存，继续切换窗口</returns>
        bool AskToSave();
    }
}
