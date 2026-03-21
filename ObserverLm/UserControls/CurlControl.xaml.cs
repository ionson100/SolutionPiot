using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObserverLm.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CurlControl.xaml
    /// </summary>
    public partial class CurlControl : UserControl
    {
        public CurlControl()
        {
            InitializeComponent();
        }

        public void SetCurlText(string curlCommand)
        {
            OutputTextBoxR.Text = curlCommand;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.TemplatedParent is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                Clipboard.SetText(textBox.Text);
            }
        }
    }
}
