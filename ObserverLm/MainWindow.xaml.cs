using System;
using System.Windows;
using System.Windows.Controls;

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
                        if (MyContentControl.Content is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
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
                    if (MyContentControl.Content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
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
                    if (MyContentControl.Content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    MyContentControl.Content = new LogsControl();
                    break;
                }

                case "b4":
                {

                    if (MyContentControl.Content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    MyContentControl.Content = new SettingsControl();
                    break;


                        

                }
                case "b5":
                {
                    if (MyContentControl.Content is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    MyContentControl.Content = new HelpControl1();
                    break;
                }
            }
        }
    }
}
