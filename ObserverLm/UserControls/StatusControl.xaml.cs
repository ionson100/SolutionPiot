using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ObserverLm.UserControls
{
    /// <summary>
    /// Логика взаимодействия для StatusControl.xaml
    /// </summary>
    public partial class StatusControl : UserControl
    {
        public StatusControl(string s, string sr)
        {
            InitializeComponent();
            SpellCheck.SetIsEnabled(TextBoxStatus, false);
            TextBoxStatus.Text = s;
            CurrentControlCore.SetCurlText(sr);
        }

      
    }
}
