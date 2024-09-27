using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SDBS3000.Core.Utils
{
    public class LanguageManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ResourceManager resourceManager;
        private bool isInit;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="baseName">资源文件的根名称，没有其扩展名但是包含所有完全限定的命名空间名称。 例如，名为 MyApplication.MyResource.en-US.resources 的资源文件的根名称为 MyApplication.MyResource。</param>
        /// <param name="cultureInfo">初始化语言</param>
        public void Init(string baseName, CultureInfo cultureInfo = null)
        {
            if (!isInit)
            {
                resourceManager = new ResourceManager(baseName, typeof(LanguageManager).Assembly);
                if (cultureInfo != null) ChangeLanguage(cultureInfo);
                isInit = true;
            }
        }

        private static Lazy<LanguageManager> instance = new();
        public static LanguageManager Instance => instance.Value;

        public string this[string name]
        {
            get
            {
                if (!isInit)
                {
                    Init("SDBS3000.Core.Resources.Lang");
                }
                if (string.IsNullOrEmpty(name)) return string.Empty;

                string str;
                str = resourceManager.GetString(name);
                if (string.IsNullOrWhiteSpace(str))
                {
                    str = name;
                    Debug.WriteLine($"字符串资源: {name} 未找到");                    
                }
                return str;
            }
        }

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));
        }
    }

    public static class LanguageExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Localize(this string key)
        {
            return LanguageManager.Instance[key];
        }
    }
}
