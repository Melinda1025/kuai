using DryIoc;
using HandyControl.Tools;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using SDBS3000.Core.Utils;
using SDBS3000.ViewModels;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.Views;
using SDBS3000.Views.Dialogs;
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
        public static Container Container { get; } = new Container();
        private readonly ILogger logger;
        public App()
        {
            //if(!CheckIsSingleInstance())
            //{
            //    Current.Shutdown();
            //    return;
            //}
            ConfigureLogging();
            RegisterTypes();
            InitializeLanguage();
            logger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //RegisterErrorHandlers();
            base.OnStartup(e);
            MemoryHelper.Instance.FreeMemoryAuto();
            var window = Container.Resolve<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            MemoryHelper.Instance.StopAutoFree();
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
                Layout = "${longdate} : ${message} ${onexception:${exception:format=ToString} ${newline} ${stacktrace} ${newline}",
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
            };
            var asyncTarget = new AsyncTargetWrapper
            {
                WrappedTarget = fileTarget,
            };
            cfg.AddTarget("asyncFile", asyncTarget);

            var rule = new LoggingRule("*", LogLevel.Info, asyncTarget);
            cfg.LoggingRules.Add(rule);

            LogManager.Configuration = cfg;
        }

        /// <summary>
        /// 注册IOC类型
        /// </summary>
        private static void RegisterTypes()
        {
            Container.Register<MainViewModel>(Reuse.Singleton);
            Container.Register<MainWindow>(Reuse.Singleton);
            Container.Register<LoginViewModel>(Reuse.Transient);
            Container.Register<LoginView>(Reuse.Transient);
            Container.Register<IndexViewModel>(Reuse.Singleton);
            Container.Register<IndexView>(Reuse.Singleton);
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
            Container.Register<KeyCompensationViewModel>(Reuse.Singleton);
            Container.Register<KeyCompensationView>(Reuse.Singleton);
            Container.Register<SingleStepViewModel>(Reuse.Singleton);
            Container.Register<SingleStepView>(Reuse.Singleton);
            Container.Register<UserViewModel>(Reuse.Singleton);
            Container.Register<UserView>(Reuse.Singleton);
            Container.Register<AlarmViewModel>(Reuse.Singleton);
            Container.Register<AlarmView>(Reuse.Singleton);
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
