using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ObserverLm
{
    
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        public  const string ApplicationJson = "application/json";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Ошибки в UI-потоке (самые частые в WPF)
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 2. Ошибки в фоновых потоках (Task, Thread Pool)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 3. Ошибки в асинхронных задачах (Task)
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        // Обработка ошибок графического интерфейса
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception, "UI Dispatcher Error");

            // Чтобы приложение НЕ закрылось сразу:
            e.Handled = true;
            MessageBox.Show($"Произошла ошибка : {e.Exception.Message}", "Ошибка");
        }

        // Ошибки всего домена (обычно фатальные)
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex) LogException(ex, "AppDomain Fatal Error");

            if (e.IsTerminating)
                MessageBox.Show("Критическая ошибка. Приложение будет закрыто.");
        }

        // Ошибки внутри объектов Task
        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception, "Task Scheduler Error");
            e.SetObserved(); // Предотвращает падение приложения
        }

        private void LogException(Exception? ex, string source)
        {
           
            string logText = $"[{DateTime.Now}] [{source}] {ex?.Message}\n{ex?.StackTrace}\n";
            System.IO.File.AppendAllText("crash_log.txt", logText);
        }
    }
}
