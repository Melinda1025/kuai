using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SDBS3000.ViewModels
{
    public class Alarm
    {
        public DateTime TimeStamp { get; set; }
        public string DateString => TimeStamp.ToLongDateString();
        public string TimeString => TimeStamp.ToLongTimeString();
        public string Info { get; set; }
    }
    public partial class AlarmViewModel : ObservableObject
    {
        public ObservableCollection<Alarm> Alarms { get; set; } = new ObservableCollection<Alarm>();
        public AlarmViewModel()
        {
            Alarms.Add(new Alarm()
            {
                TimeStamp = DateTime.Now,
                Info = "test1"
            });
            Alarms.Add(new Alarm()
            {
                TimeStamp = DateTime.Now,
                Info = "test2"
            });
        }
    }
}
