using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using SDBS3000.Core.Models;
using SqlSugar;
using System.Collections.ObjectModel;

namespace SDBS3000.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {        
        private User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                SetProperty(ref selectedUser, value);
                if(IsEditing)
                {
                    IsEditing = false;
                    IsAddNew = false;
                }
            }
        }
        [ObservableProperty]
        private bool isEditing;
        [ObservableProperty]
        private bool isAddNew;

        public ObservableCollection<User> Users { get; set; } = new();

        private readonly SqlSugarClient db;
        public UserViewModel(SqlSugarClient db)
        {
            this.db = db;
        }

        [RelayCommand]
        public async Task OnLoadedAsync()
        {            
            Users.Clear();
            var users = await db.Queryable<User>().ToListAsync();
            foreach (var user in users)
            {
                Users.Add(user);
                if(user.Id == App.User.Id) SelectedUser = user;
            }
            SelectedUser ??= Users[0];
        }

        [RelayCommand]
        public void Add()
        {
            SelectedUser = new User();
            IsEditing = true;
            IsAddNew = true;
        }

        [RelayCommand]
        public void Edit()
        {
            IsEditing = true;
            IsAddNew = false;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedUser.Name) || string.IsNullOrWhiteSpace(SelectedUser.Password))
            {
                Growl.Warning("用户名或密码不能为空");
                return;
            }
            if (SelectedUser.Name.Length > 10)
            {
                Growl.Warning("用户名过长");
                return;
            }           

            if(SelectedUser.Permission == -1)
            {
                Growl.Warning("请选择用户权限");
                return;
            }
            
            //新增
            if(SelectedUser.Id == -1)
            {
                //不能创建高于自己权限的用户
                if(SelectedUser.Permission < App.User.Permission)
                {
                    Growl.Warning("无法创建高于自己权限的用户");
                    return;
                }

                if (await db.Queryable<User>().AnyAsync(u => u.Name == SelectedUser.Name)) return;

                if (MessageBox.Ask("确定要保存吗?") != System.Windows.MessageBoxResult.OK) return;

                await db.Insertable(SelectedUser).ExecuteCommandIdentityIntoEntityAsync();
                Users.Add(SelectedUser);             
                OnPropertyChanged(nameof(SelectedUser));                
            }
            else
            {
                if (MessageBox.Ask("确定要保存吗?") != System.Windows.MessageBoxResult.OK) return;

                await db.Updateable(SelectedUser).ExecuteCommandAsync();
                if(SelectedUser.Id == App.User.Id) App.User = SelectedUser;
            }
            IsEditing = false;
            IsAddNew = false;
        }

        [RelayCommand]
        public async Task DeleteAsync()
        {
            if(SelectedUser.Id < User.DEFAULT_USER_COUNT)
            {
                Growl.Warning("无法删除默认用户");
                return;
            }
            if(SelectedUser.Id == App.User.Id)
            {
                Growl.Warning("无法删除自己");
                return ;
            }

            if(MessageBox.Ask("确定要删除这个用户吗?") == System.Windows.MessageBoxResult.OK)
            {
                await db.Deleteable(SelectedUser).ExecuteCommandAsync();
                Users.Remove(SelectedUser);
                SelectedUser = App.User;
            }
        }
    }
}
