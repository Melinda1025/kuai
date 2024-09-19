using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.Views;
using SqlSugar;
using System.Collections.ObjectModel;

namespace SDBS3000.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private User user;

        public ObservableCollection<User> Users { get; set; } = new();

        private readonly SqlSugarClient db;
        public LoginViewModel(SqlSugarClient db)
        {
            this.db = db;
        }

        [RelayCommand]
        public async Task OnLoadedAsync()
        {
            User = new User
            {
                Name = AppSetting.Default.UserName,
                Password = AppSetting.Default.Password,
            };

            var users = await db.Queryable<User>().Take(5).ToListAsync();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }


        [RelayCommand]
        public async Task LoginAsync()
        {            
            var db_user = await db.Queryable<User>().FirstAsync(u => u.Name == User.Name);
            if(db_user == null)
            {
                Growl.Error("密码错误");
                return;
            }

            User.Salt = db_user.Salt;
            if(User.HashPassword().SequenceEqual(db_user.PasswordMd5))
            {
                db_user.Password = User.Password;
                App.User = db_user;
                SimpleEventBus<Type>.Instance.Publish("Navigate", null, typeof(IndexView));
            }
            else
            {
                Growl.Error("密码错误");
            }
            
        }
    }
}
