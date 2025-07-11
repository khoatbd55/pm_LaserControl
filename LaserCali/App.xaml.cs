using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LaserCali
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {

            SplashScreenManager.CreateThemed(new DXSplashScreenViewModel
            {
                Copyright = "All copyrights reserved",
                IsIndeterminate = true,
                Logo = new System.Uri("pack://application:,,,/Images/MyLogo.png",
                           UriKind.RelativeOrAbsolute),
                Status = "Starting...",
                Title = "",
                Subtitle = "Developed by VMI",
            }
            ).ShowOnStartup();
        }

        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
             // Process unhandled exception
            string errorMessage = string.Format("TaskScheduler_UnobservedException: {0}-{1}", e.Exception.Message, e.Exception.InnerException.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // Prevent default unhandled exception processing
            e.SetObserved();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            string errorMessage = string.Format("Current_DispatcherUnhandledException: {0}-{1}", e.Exception.Message, e.Exception.InnerException.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Xử lý ngoại lệ toàn cục ở đây
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show("CurrentDomain_UnhandledException: " + ex?.Message);
            // Không có e.Handled, app có thể vẫn bị crash nếu là fatal exception
            
        }

        static void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("DispatcherUnhandled: {0}-{1}", e.Exception.Message, e.Exception.InnerException.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

    }

    
}
