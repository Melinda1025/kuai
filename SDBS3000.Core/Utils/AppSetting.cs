using CommunityToolkit.Mvvm.ComponentModel;
using Kdbndp.KingbaseTypes;
using SqlSugar;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SDBS3000.Core.Utils
{
    public class AppSetting
    {
        private static readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };
        public const string SettingPath = "App.settings";
        public static AppSetting Default { get; private set; }


        #region 配置属性        
        public string UserName { get; set; } = "admin";
        public string Password { get; set; } = "123456";
        public DbType DbType { get; set; } = DbType.Sqlite;
        public string ConnectionString { get; set; } = "Data Source = data.db";
        public string PlcIP { get; set; } = "127.0.0.1";
        /// <summary>
        /// 测量次数
        /// </summary>
        public short MeasureTimes { get; set; } = 5;
        /// <summary>
        /// 转差范围
        /// </summary>
        public float SlipRange { get; set; } = 0.1f;
        /// <summary>
        /// 工作模式   连续普通测量0  普通测量停止1  测量定位2
        /// </summary>
        public short WorkMode { get; set; } = 0;
        /// <summary>
        /// 定位模式 0先左后右 1只左面 2只右面
        /// </summary>
        public short PositioningMode { get; set; } = 0;
        /// <summary>
        /// 刷新频率
        /// </summary>
        public float RefreshRate { get; set; } = 10f;
        /// <summary>
        /// 保留位数 可选0到3位
        /// </summary>
        public string Digits {  get; set; } = "3";
        /// <summary>
        /// 定位转速
        /// </summary>
        public float PositioningSpeed { get; set; } = 60f;
        /// <summary>
        /// 加速时间
        /// </summary>
        public float AccelerationTime { get; set; } = 2.2f;
        /// <summary>
        /// 减速时间
        /// </summary>
        public float DecelerationTime { get; set; } = 2.2f;
        /// <summary>
        /// 每转脉冲数
        /// </summary>
        public float PlusePerRound { get; set; } = 2200f;
        /// <summary>
        /// 定位补偿
        /// </summary>
        public float PositioningCompensation { get; set; } = 0f;
        /// <summary>
        /// 转差补偿
        /// </summary>
        public float SlipCompensation { get; set; } = 0f;
        /// <summary>
        /// 转子名称
        /// </summary>
        public string RotorName {  get; set; } = string.Empty;
        /// <summary>
        /// 转子槽数
        /// </summary>
        public int Slots { get; set; } = 1;
        /// <summary>
        /// 转子方向 true逆时针 false顺时针
        /// </summary>
        public bool RotationDirection { get; set; } = false;
        /// <summary>
        /// 开关类型 0光电开关 1接近开关(数槽)
        /// </summary>
        public bool SwitchType { get; set; } = false;
        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthCode { get; set; } = string.Empty;
        /// <summary>
        /// 光针模式 0不适用 1使用手动检测每秒脉冲数 2使用自动检测每秒脉冲数
        /// </summary>
        public short NeedleMode {  get; set; } = 0;
        /// <summary>
        /// 安全门模式 0不使用 1使用
        /// </summary>
        public short SafetyMode { get; set; } = 0;
        #endregion


        static AppSetting()
        {
            if (!File.Exists(SettingPath))
            {
                Default = new();
                Default.Save();
            }
            else
            {
                try
                {
                    Default = Load();
                }
                catch
                {
                    Default = new();
                    throw;
                }
            }
        }

        public static AppSetting Load()
        {
            AppSetting setting = null;
            using (var fs = File.OpenRead(SettingPath))
            {
                if (fs.Length == 0) return new();
                setting = JsonSerializer.Deserialize<AppSetting>(fs, options);
            }
            return setting;
        }

        public void Save()
        {
            using var fs = File.OpenWrite(SettingPath);
            JsonSerializer.Serialize<AppSetting>(fs, this, options);
        }
    }
}
