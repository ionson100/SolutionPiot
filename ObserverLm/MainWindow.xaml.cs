using ObserverLm.UserControls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _selectedButton;
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

            _selectedButton = this.ButtonLog;
        }
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        void DefaultStyle()
        {
            foreach (var button1 in ButtonHost.Children.OfType<Button>())
            {
                button1.Style = (Style)this.FindResource("ButtonHeadPaneStyle");
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender; 
           
            DefaultStyle();

            button.Style = (Style)this.FindResource("ButtonHeadPaneSelectStyle");
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
                            await new MyStatusInit().RequestInitAsync((s,sr) =>
                            {
                               
                                MyContentControl.Content = new StatusControl(s,sr);
                            });
                        }
                        finally
                        {
                            LoadingBar.Visibility = Visibility.Collapsed;

                        }
                    }
                    else
                    {
                        DefaultStyle();
                        _selectedButton.Style = (Style)this.FindResource("ButtonHeadPaneSelectStyle");
                    }
                    break;
                }
                case "b2":
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    DisposeCurrentControl();
                    try
                    {
                        await new MyStatusInit().RequestPiotAsync("status",(s,sr) =>
                        {
                            MyContentControl.Content = new StatusControl(s,sr);
                        });
                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;

                    }
                 
                    break;
                }

                case "bst":
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    DisposeCurrentControl();
                    try
                    {
                        await new MyStatusInit().RequestPiotAsync("stats", (s, sr) =>
                        {
                            MyContentControl.Content = new StatusControl(s, sr);
                        });
                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;

                    }

                    break;
                }
                case "bconfig":
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    DisposeCurrentControl();
                    try
                    {
                        await new MyStatusInit().RequestPiotAsync("config", (s, sr) =>
                        {
                            MyContentControl.Content = new StatusControl(s, sr);
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
                case "bsales":
                {

                    DisposeCurrentControl();

                    MyContentControl.Content = new SalesControl(SalesControlType.Sale);
                    break;
                }
            }
            _selectedButton=button;
        }

        void DisposeCurrentControl()
        {
            if (MyContentControl.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DisposeCurrentControl();
            base.OnClosing(e);
        }
    }
}
