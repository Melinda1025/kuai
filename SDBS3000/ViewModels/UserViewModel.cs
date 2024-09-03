using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.ViewModels
{
    public class User
    {
        public string Name { get; set; }
        public string Access { get; set; }
    }
    public partial class UserViewModel : ObservableObject
    {
        public ObservableCollection<User> Users { get; set; }

        public UserViewModel()
        {
            Users =
            [
                new()
                {
                    Name = "张三",
                    Access = "管理员"
                },
                new()
                {                
                    Name = "李四",
                    Access = "普通用户"
                }
                ,
                new()
                {
                    Name = "网管",
                    Access = "普通用户"
                }
            ];
        }
    }
}
