using Dm.filter;
using DryIoc;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;
using NLog.Targets;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SDBS3000.Views;
using SDBS3000.ViewModels;
using SDBS3000.ViewModels.Dialogs;
using SDBS3000.Views.Dialogs;

namespace SDBS3000
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container { get; } = new Container();
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
            logger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnStartup(StartupEventArgs e)
        {            
            //RegisterErrorHandlers();
            base.OnStartup(e);
            var window = Container.Resolve<MainWindow>();            
            window.Show();
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
                KeepFileOpen = false,
                Name = "log_file",
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
