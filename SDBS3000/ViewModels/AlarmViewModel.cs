using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SDBS3000.Core.Models;
using SqlSugar;
using System.Collections.ObjectModel;

namespace SDBS3000.ViewModels
{
    public partial class AlarmViewModel : ObservableObject
    {
        public ObservableCollection<Alarm> Alarms { get; set; } = new();

        private int pageIndex = 0;
        private int max = -1;
        private const int PAGE_SIZE = 20;
        private readonly SqlSugarClient db;
        public AlarmViewModel(SqlSugarClient db)
        {
            this.db = db;                        
            Task.Run(LoadDataAsync);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            if(max == -1)
            {
                max = await db.Queryable<Alarm>().CountAsync();
            }
            if(Alarms.Count == max) return;
            var alarms = await db.Queryable<Alarm>().ToPageListAsync(pageIndex, PAGE_SIZE);
            pageIndex += alarms.Count;
            foreach (var alarm in alarms)
            {
                Alarms.Add(alarm);
            }            
        }
    }
}
