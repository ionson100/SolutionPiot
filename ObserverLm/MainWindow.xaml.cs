using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using ObserverLm.UserControls;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyContentControl.Content = new LogsControl();
            if (IsAdministrator())
            {
                ButtonSerwice.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonSerwice.Visibility = Visibility.Collapsed;
            }
        }
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Tag)
            {
                case "b1":
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Инициализировать локальный модуль?", 
                        "int LM",                          
                        MessageBoxButton.YesNo,                     
                        MessageBoxImage.Question                   
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        LoadingBar.Visibility = Visibility.Visible;
                        DisposeCurrentControl();
                            try
                        {
                            await new MyStatusInit().RequestInitAsync(s =>
                            {
                                MyContentControl.Content = new StatusControl(s);
                                return "";
                            });
                        }
                        finally
                        {
                            LoadingBar.Visibility = Visibility.Collapsed;

                        }
                    }
                    break;
                }
                case "b2":
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    DisposeCurrentControl();
                    try
                    {
                        await new MyStatusInit().RequestPiotAsync("status",s =>
                        {
                            MyContentControl.Content = new StatusControl(s);
                            return "";
                        });
                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;

                    }
                 
                    break;
                }
                case "b3":
                {
                    DisposeCurrentControl();
                        MyContentControl.Content = new LogsControl();
                    break;
                }

                case "b4":
                {

                    DisposeCurrentControl();
                        MyContentControl.Content = new SettingsControl();
                    break;


                        

                }
                case "b5":
                {
                    DisposeCurrentControl();
                        MyContentControl.Content = new HelpControl1();
                    break;
                }
                case "bs":
                {
                    DisposeCurrentControl();

                        MyContentControl.Content = new ServiceControlView();
                    break;
                }
                case "bc":
                {

                    DisposeCurrentControl();

                    MyContentControl.Content = new CodeCheckerControl();
                    break;
                }
            }
        }

        void DisposeCurrentControl()
        {
            if (MyContentControl.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
