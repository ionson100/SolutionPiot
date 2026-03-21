using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ObserverLm.UserControlsSales;

namespace ObserverLm.UserControls
{
    public enum SalesControlType
    {
        Sale,
        SaleReturn,
        SaleCheck,
       
    }
    /// <summary>
    /// Логика взаимодействия для SaleControl.xaml
    /// </summary>
    public partial class SalesControl : UserControl
    {
        private readonly SalesControlType _controlType;

        public SalesControl(SalesControlType controlType)
        {
            _controlType = controlType;
            InitializeComponent();
            MyContentControl.Content = new SaleControl(_controlType);
        }
        void DefaultStyle()
        {
            foreach (var button1 in PanelHost.Children.OfType<Button>())
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
                case "bSale":
                {
                    MyContentControl.Content = new SaleControl(SalesControlType.Sale);
                        break;
                }
                case "bSaleReturn":
                {
                    MyContentControl.Content = new SaleControl(SalesControlType.SaleReturn);
                        break;
                }
                case "bSaleCheck":
                {
                    MyContentControl.Content = new SaleControl(SalesControlType.SaleCheck);
                        break;
                }
                case "bSaleList":
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    try
                    {
                        await new MyStatusInit().RequestPiotAsync("cis/sold?skip=0&limit=1000", (s, s1) =>
                        {
                            MyContentControl.Content = new StatusControl(s, s1);
                        });

                    }
                    finally
                    {

                        LoadingBar.Visibility = Visibility.Collapsed;
                    }
                    break;
                }
                case "bApi":
                {
                    string str = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"123.pdf");
                    if (File.Exists(str))
                    {
                        Process.Start(new ProcessStartInfo(str) { UseShellExecute = true });
                    }
                    else
                    {
                        MessageBox.Show($"Файл: {str} не найден");
                    }


                    break;
                }
            }
        }
    }
}
