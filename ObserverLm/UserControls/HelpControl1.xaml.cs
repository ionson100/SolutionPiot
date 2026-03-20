using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для HelpControl1.xaml
    /// </summary>
    public partial class HelpControl1 : UserControl
    {
        public HelpControl1()
        {
            InitializeComponent();
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            try
            {

                //regime
                //yenisei
                // 1. Ждем инициализации движка Edge Chromium
                await WebView.EnsureCoreWebView2Async(null);




                // 3. Формируем путь к локальному файлу help.html в папке приложения
                string helpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help.html");

                if (File.Exists(helpPath))
                {
                    WebView.CoreWebView2.Navigate(new Uri(helpPath).AbsoluteUri);
                }
                else
                {
                    WebView.NavigateToString("<h1>Файл help.html не найден</h1>");
                }
            }
            finally
            {
                // 2. Скрываем прогресс-бар
                LoadingBar.Visibility = Visibility.Collapsed;
            }

        }
    }
}
