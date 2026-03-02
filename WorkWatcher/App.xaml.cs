using System.Configuration;
using System.Data;
using System.Windows;

namespace WorkWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static System.Threading.Mutex? _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "WorkWatcher_SingleInstance";
            bool createdNew;

            _mutex = new System.Threading.Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // 이미 실행 중인 인스턴스가 있음
                MessageBox.Show(
                    "WorkWatcher가 이미 실행 중입니다.",
                    "중복 실행 방지",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
