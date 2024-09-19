using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.Core.Models
{
    [SugarTable("User")]
    public class User
    {        
        public const int DEFAULT_USER_COUNT = 3;

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; } = -1;
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string Password { get; set; }

        private int salt = -1;
        [SugarColumn(IsNullable = false)]
        public int Salt
        {
            get
            {
                if (salt == -1)
                {
                    salt = Random.Shared.Next();
                }
                return salt;
            }
            set => salt = value;
        }
        
        [SugarColumn(IsNullable = false)]
        public byte[] PasswordMd5 {  get; set; }

        [SugarColumn(IsNullable = false)]
        public int Permission { get; set; } = -1;
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public string MES_UserName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string MES_Password { get; set; }


        public byte[] HashPassword()
        {
            PasswordMd5 = MD5.HashData(Encoding.UTF8.GetBytes($"{Password}{Salt}"));
            return PasswordMd5;
        }
    }
}
