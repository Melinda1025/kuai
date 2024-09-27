using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace SDBS3000.Core.Models
{
    [SugarTable("Alarm")]
    public class Alarm
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string DateString => TimeStamp.ToLongDateString();
        [SugarColumn(IsIgnore = true)]
        public string TimeString => TimeStamp.ToLongTimeString();
        public string Info { get; set; }
    }
}
