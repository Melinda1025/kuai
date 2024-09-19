using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SDBS3000.Core.Utils;

namespace SDBS3000.ViewModels.Dialogs
{
    public partial class ISOViewModel : ObservableObject
    {
        private float r1 = 0, r2 = 0;
        [ObservableProperty]
        private float balanceLevel = 1;

        private float speed = 1000;
        public float Speed
        {
            get => speed;
            set
            {
                if (value <= 0) value = 1;
                SetProperty(ref speed, value);
            }
        }

        private int weight = 2;
        public int Weight
        {
            get => weight;
            set
            {
                if (value <= 0) value = 1;
                SetProperty(ref weight, value);
            }
        }

        public void InitRadius(float r1, float r2)
        {
            this.r1 = r1;
            this.r2 = r2;
        }

        [RelayCommand]
        public void CaculateISO()
        {
            var panel1MaxUnbalence = r1 == 0 ? 0 : (float)Math.Round(BalanceLevel * 1000 * 10 / Speed * Weight / 2 / r1, 3);
            var panel2MaxUnbalence = r2 == 0 ? 0 : (float)Math.Round(BalanceLevel * 1000 * 10 / Speed * Weight / 2 / r2, 3);
            var staticMaxUnbalence = panel1MaxUnbalence + panel2MaxUnbalence;

            SimpleEventBus<(float, float, float)>.Instance.Publish("CaculateISO_Complete", null, (panel1MaxUnbalence, panel2MaxUnbalence, staticMaxUnbalence));
        }
    }
}
