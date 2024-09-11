using CommunityToolkit.Mvvm.ComponentModel;
using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    /// <summary>
    /// 加去重信息
    /// </summary>
    public partial class AddOrRemoveInfo : ObservableObject
    {
        private readonly ExtendedPlc plc;

        public AddOrRemoveInfo(ExtendedPlc plc)
        {
            this.plc = plc;
        }
        
        private bool leftAddOrRemove;
        public bool LeftAddOrRemove
        {
            get => leftAddOrRemove;
            set
            {
                if(!plc.IsConnected || value == leftAddOrRemove) return;
                plc.Write("DB20.DBX116.0", value);
                SetProperty(ref leftAddOrRemove, value);                
            }
        }
        
        private bool rightAddOrRemove;
        public bool RightAddOrRemove
        {
            get => rightAddOrRemove;
            set
            {
                if(!plc.IsConnected && value == rightAddOrRemove) return;
                plc.Write("DB20.DBX116.1", value);
                SetProperty(ref rightAddOrRemove, value);

            }
        }
        
        private bool staticAddOrRemove;
        public bool StaticAddOrRemove
        {
            get => staticAddOrRemove;
            set
            {
                if(!plc.IsConnected && value == staticAddOrRemove) return;
                plc.Write("DB20.DBX116.2", value);
                SetProperty(ref staticAddOrRemove, value);
            }
        }        
        private bool electricCompensation;
        public bool ElectricCompensation
        {
            get => electricCompensation;
            set
            {
                if(!plc.IsConnected && value == electricCompensation) return;
                plc.Write("DB20.DBX440.2", value);
                SetProperty(ref electricCompensation, value);
            }
        }
    }
}
