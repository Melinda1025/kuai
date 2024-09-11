using S7.Net;

namespace SDBS3000.Core.Utils
{
    public class ExtendedPlc : Plc
    {
        private readonly Timer autoCheckTimer;
        public ExtendedPlc(string ip) : base(CpuType.S71200, ip, 0, 1)
        {
            autoCheckTimer = new Timer(new TimerCallback(OnCheck));
            autoCheckTimer.Change(0, Timeout.Infinite);
        }

        private async void OnCheck(object state)
        {
            if (!IsConnected)
            {
                try
                {
                    await OpenAsync();
                    SimpleEventBus<bool>.Instance.Publish("IsPlcConnectedChanged", null, true);
                }
                catch (Exception) { }
            }
            else
            {
                try
                {
                    await ReadAsync("DB20.DBX116.0");
                    await CheckErrorAsync();
                }
                catch (Exception)
                {
                    Close();
                    SimpleEventBus<bool>.Instance.Publish("IsPlcConnectedChanged", null, false);
                }
            }
            autoCheckTimer.Change(2000, Timeout.Infinite);
        }

        private async Task CheckErrorAsync()
        {
            //读取7 * 8 = 56位，有效的是前53位
            var bytes = await ReadBytesAsync(DataType.DataBlock, 21, 140, 7);
            for(int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0) continue;                             
                var bools = BitHelper.ByteToBoolArray(bytes[i]);
                for(int j = 0; j < bools.Length; j++)
                {
                    if (bools[j] == true)
                    {
                        SimpleEventBus<int>.Instance.Publish("ErrorReceived", null, i * 8 + j);
                    }
                }
            }
        }
    }
}
