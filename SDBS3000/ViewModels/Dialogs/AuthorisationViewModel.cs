using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using S7.Net;
using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels.Dialogs
{
    public partial class AuthorisationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string serialNumber;
        [ObservableProperty]
        private short daysRemaining;
        [ObservableProperty]
        private string authCode;
        [ObservableProperty]
        private bool isExpires;

        private readonly ExtendedPlc plc;
        public AuthorisationViewModel(ExtendedPlc plc)
        {
            this.plc = plc;
            Initialize();
            SimpleEventBus<bool>.Instance.Subscribe(
                "IsPlcConnectedChanged",
                (s, connected) =>
                {
                    if (connected)
                    {
                        ReadDataFromPlc();
                    }
                }
            );
        }

        [RelayCommand]
        public void Initialize()
        {
            AuthCode = AppSetting.Default.AuthCode;
            if (plc.IsConnected)
            {
                ReadDataFromPlc();
            }
        }
        
        public void ReadDataFromPlc()
        {            
            SerialNumber = (string)plc.Read(DataType.DataBlock, 10, 40, VarType.S7String, 16);
            DaysRemaining = (short)plc.Read("DB20.DBW336.0");        
            IsExpires = (bool)plc.Read("DB20.DBX460.7");
        }

        [RelayCommand]      
        public async Task AuthorizeAsync()
        {
            var codeData = Encoding.ASCII.GetBytes(AuthCode);
            if (codeData.Length > 20)
            {
                //授权码过长
                return;
            }
            //写入授权码
            plc.WriteBytes(DataType.DataBlock, 10, 0, codeData);
            plc.WriteBytes(DataType.DataBlock, 10, 20, codeData);
            //设置授权码
            plc.Write("DB20.DBX460.6", true);
            await Task.Delay(200);
            plc.Write("DB20.DBX460.6", false);
            AppSetting.Default.AuthCode = AuthCode;
        }


    }
}
