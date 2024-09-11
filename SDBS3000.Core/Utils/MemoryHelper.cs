namespace SDBS3000.Core.Utils
{
    /// <summary>
    /// 内存回收助手
    /// </summary>
    public partial class MemoryHelper
    {
        private static readonly Lazy<MemoryHelper> instance = new Lazy<MemoryHelper>(() => new MemoryHelper());
        public static MemoryHelper Instance => instance.Value;

        //[LibraryImport("kernel32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static partial bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        private volatile int locker = 0;
        private bool isAutoFree;

        private void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            //{
            //    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            //}
            Interlocked.Exchange(ref locker, 0);

        }
        /// <summary>
        /// 手动回收内存
        /// </summary>
        public void FreeMemory()
        {
            if (Interlocked.CompareExchange(ref locker, 1, 0) == 1)
            {
                Task.Run(FlushMemory);
            }
        }

        /// <summary>
        /// 循环自动回收内存
        /// </summary>
        /// <param name="interval"></param>
        public void FreeMemoryAuto(int interval = 30 * 1000)
        {
            isAutoFree = true;
            Task.Factory.StartNew(() =>
            {
                while (isAutoFree)
                {
                    try
                    {
                        Thread.Sleep(interval);
                        FreeMemory();
                    }
                    catch { }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void StopAutoFree()
        {
            isAutoFree = false;
        }
    }
}
