using DryIoc;
using HandyControl.Tools;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using S7.Net;
using SDBS3000.Core.Models;
using SDBS3000.Core.Utils;
using SDBS3000.ViewModels;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.Views;
using SDBS3000.Views.Dialogs;
using SqlSugar;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SDBS3000
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User User { get; set; }
        public static Container Container { get; } = new();
        private readonly Logger logger;
        public App()
        {
#if !DEBUG
            if (!CheckIsSingleInstance())
            {
                Current.Shutdown();
                return;
            }
#endif
            ConfigureLogging();
            RegisterTypes();
            logger = LogManager.GetCurrentClassLogger();
            Task.Run(InitializeDatabase);
            InitializeLanguage();                        
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //SwitchThemes("DarkTheme");
#if !DEBUG
            RegisterErrorHandlers();
#endif
            base.OnStartup(e);
            MemoryHelper.Instance.FreeMemoryAuto();
            var window = Container.Resolve<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MemoryHelper.Instance.StopAutoFree();
            AppSetting.Default.Save();
            base.OnExit(e);            
        }
        /// <summary>
        /// 检测是否只有一个实例程序
        /// </summary>
        /// <returns></returns>
        private static bool CheckIsSingleInstance()
        {
            string MName = Process.GetCurrentProcess().MainModule!.ModuleName;
            string PName = Path.GetFileNameWithoutExtension(MName);
            Process[] myProcess = Process.GetProcessesByName(PName);
            return myProcess.Length == 1;
        }

        /// <summary>
        /// 配置日志功能
        /// </summary>
        private static void ConfigureLogging()
        {
            var cfg = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                FileName = "${basedir}/Logs/${shortdate}/${level}-${shortdate}.log",
                Layout = "${longdate} : ${message} ${newline} ${onexception:${exception:format=ToString} ${newline} ${stacktrace} ${newline}",
                ArchiveFileName = "${basedir}/archives/${logger}-${level}-${shortdate}-{#####}.log",
                ArchiveAboveSize = 102400,
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ConcurrentWrites = true,
                KeepFileOpen = true,
                Name = "log_file",
                MaxArchiveDays = 30,
                Encoding = System.Text.Encoding.UTF8,
                OpenFileCacheTimeout = 30,
                OpenFileFlushTimeout = 10,
                AutoFlush = false,
                MaxArchiveFiles = 99,
            };
            var asyncTarget = new AsyncTargetWrapper
            {
                WrappedTarget = fileTarget,
            };
            cfg.AddTarget("asyncFile", asyncTarget);
            
            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, asyncTarget));
            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, asyncTarget));
#if DEBUG
            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, asyncTarget));
#endif

            LogManager.Configuration = cfg;
        }

        /// <summary>
        /// 注册IOC类型
        /// </summary>
        private void RegisterTypes()
        {            
            Container.Register<LoginViewModel>(Reuse.Transient);
            Container.Register<LoginView>(Reuse.Transient);

            Container.Register<MainViewModel>(Reuse.Singleton);
            Container.Register<MainWindow>(Reuse.Singleton);            
            Container.Register<IndexViewModel>(Reuse.Singleton);
            Container.Register<IndexView>(Reuse.Singleton);
            Container.Register<MeasureViewModel>(Reuse.Singleton);
            Container.Register<MeasureView>(Reuse.Singleton);
            Container.Register<CalViewModel>(Reuse.Singleton);
            Container.Register<CalView>(Reuse.Singleton);
            Container.Register<RotorViewModel>(Reuse.Singleton);
            Container.Register<RotorView>(Reuse.Singleton);
            Container.Register<SettingViewModel>(Reuse.Singleton);
            Container.Register<SettingView>(Reuse.Singleton);
            Container.Register<PositionViewModel>(Reuse.Singleton);
            Container.Register<PositionView>(Reuse.Singleton);
            Container.Register<AxisParamsViewModel>(Reuse.Singleton);
            Container.Register<AxisParamsDialog>(Reuse.Singleton);
            Container.Register<KeyViewModel>(Reuse.Singleton);
            Container.Register<KeyView>(Reuse.Singleton);
            Container.Register<SingleStepViewModel>(Reuse.Singleton);
            Container.Register<SingleStepView>(Reuse.Singleton);
            Container.Register<UserViewModel>(Reuse.Singleton);
            Container.Register<UserView>(Reuse.Singleton);
            Container.Register<AlarmViewModel>(Reuse.Singleton);
            Container.Register<AlarmView>(Reuse.Singleton);
            Container.Register<AuthorisationViewModel>(Reuse.Singleton);
            Container.Register<AuthorisationDialog>(Reuse.Singleton);

            Container.Register<AddOrRemoveInfo>(Reuse.Singleton);


            Container.RegisterDelegate(() =>
            {
                string ip = AppSetting.Default.PlcIP;
                var plc = new ExtendedPlc(ip);
                return plc;
            }, Reuse.Singleton);
            Container.RegisterDelegate(LogManager.GetCurrentClassLogger, Reuse.Transient);
            //数据库orm            
            Container.RegisterDelegate(() =>
            {
                var db = new SqlSugarClient(new ConnectionConfig
                {
                    DbType = AppSetting.Default.DbType,
                    ConnectionString = AppSetting.Default.ConnectionString,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true,
                });                
#if DEBUG
                db.Aop.OnLogExecuting = (sql, pars) =>
                {                    
                    var sqls = UtilMethods.GetNativeSql(sql, pars);
                    Debug.WriteLine(sqls);
                    logger.Log(LogLevel.Debug, $"执行{sqls}");
                };
#endif
                db.Aop.OnError = (ex) =>
                {
                    Debug.WriteLine($"执行:\r\n{ex.Sql}\r\n时发生错误 : {ex.Message}");
                    logger.Log(LogLevel.Error, $"执行:\r\n{ex.Sql}\r\n时发生错误 : {ex.Message}");
                };
                return db;
            }, Reuse.Transient, Setup.With(allowDisposableTransient:true));//允许注册瞬时的IDisposable对象，自行释放
        }

        private static void InitializeDatabase()
        {
            var db = Container.Resolve<SqlSugarClient>();
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(
                typeof(RotorInfo),
                typeof(User)
            );

            //var defaultUsers = new List<User>()
            //{
            //    new User
            //    {
            //        Name = "admin",
            //        Password = "123456",
            //        Permission = 0,
            //        Remark = "我是管理员",
            //    },
            //    new User
            //    {
            //        Name = "debugger",
            //        Password = "123456",
            //        Permission = 1,
            //        Remark = "我是调试员",
            //    },
            //    new User
            //    {
            //        Name = "operator",
            //        Password = "123456",
            //        Permission = 2,
            //        Remark = "我是操作员",
            //    },
            //};
            //defaultUsers.ForEach(u => u.HashPassword());
            //db.Insertable(defaultUsers).ExecuteCommand();
        }

        private static void InitializeLanguage()
        {
            LanguageManager.Instance.Init("SDBS3000.Core.Resources.Lang");
            var systemLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (systemLanguage == "zh")
            {
                ConfigHelper.Instance.SetLang("zh-CN");
                LanguageManager.Instance.ChangeLanguage(new CultureInfo("zh-CN"));
            }
            else
            {
                ConfigHelper.Instance.SetLang("en");
                LanguageManager.Instance.ChangeLanguage(new CultureInfo("en"));
            }
        }

        public static void SwitchThemes(string theme)
        {
            const int THEME_INDEX = 2;
            ResourceDictionary resource = new ()
            {
                Source = new Uri($"pack://application:,,,/SDBS3000;component/Dictionary/Themes/{theme}.xaml")
            };
            Current.Resources.MergedDictionaries[THEME_INDEX] = resource;
        }

        /// <summary>
        /// 注册全局异常
        /// </summary>
        private void RegisterErrorHandlers()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                logger.Error(ex, $"非UI线程异常:{ex.Message}");
            }
            else
            {
                logger.Error(e.ExceptionObject.ToString());
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            logger.Error($"Task异常:{e.Exception.Message}", e.Exception);
            e.SetObserved();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                logger.Error(e.Exception, $"UI线程异常:{e.Exception.Message}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"致命错误:{ex.Message}");
            }
        }
    }

}
