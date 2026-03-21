using System;
using System.Threading.Tasks;
using System.Windows;
using ObserverLm.UserControls;

namespace ObserverLm.UserControlsSales
{
    /// <summary>
    /// Логика взаимодействия для SaleControl.xaml
    /// </summary>
    public partial class SaleControl
    {
        private readonly SalesControlType _salesControlType;

        public SaleControl(SalesControlType salesControlType)
        {
            _salesControlType = salesControlType;
            InitializeComponent();
            switch (salesControlType)
            {
                case SalesControlType.Sale:
                {
                    CheckButton.Content="Продажа";
                        break;
                }
                case SalesControlType.SaleReturn:
                {
                    CheckButton.Content="Возврат";
                        break;
                }
                case SalesControlType.SaleCheck:
                {
                    CheckButton.Content="Проверить";
                        break;
                }   
            }

            InputTextBox.Focus();

        }

        private async void CheckButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                string? message = null;
                switch (_salesControlType)
                {
                    case SalesControlType.Sale:
                    {
                        message = "Введите код для продажи";
                        break;
                    }
                    case SalesControlType.SaleReturn:
                    {
                        message = "Введите код для возврата";
                        break;
                    }
                    case SalesControlType.SaleCheck:
                    {
                        message = "Введите код для проверки";
                        break;
                    }
                }
                MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }
            switch (_salesControlType)
            {
                case SalesControlType.Sale:
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    try
                    {
                        await new MyStatusInit().RequestSellAsync("cis/sell", InputTextBox.Text.Trim(), (s, s1) =>
                        {
                            OutputTextBox.Text = s;
                            CurlControlCore.SetCurlText(s1);
                        });
                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;
                    }
                   
                    break;
                }
                case SalesControlType.SaleReturn:
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    try
                    {
                        await new MyStatusInit().RequestSellAsync("cis/return", InputTextBox.Text.Trim(), (s, s1) =>
                        {
                            OutputTextBox.Text = s;
                            CurlControlCore.SetCurlText(s1);
                        });
                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;
                    }
                    
                    break;
                }//
                case SalesControlType.SaleCheck:
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    try
                    {
                        await new MyStatusInit().RequestSellAsync("cis/sold/check", InputTextBox.Text.Trim(), (s, s1) =>
                        {
                            OutputTextBox.Text = s;
                            CurlControlCore.SetCurlText(s1);
                        });

                    }
                    finally
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;
                    }
                    
                    break;
                }
            }


        }
    }
}
