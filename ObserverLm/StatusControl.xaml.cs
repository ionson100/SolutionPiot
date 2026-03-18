using System.Windows.Controls;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для StatusControl.xaml
    /// </summary>
    public partial class StatusControl : UserControl
    {
        public StatusControl(string s)
        {
            InitializeComponent();
            SpellCheck.SetIsEnabled(TextBoxStatus, false);
            TextBoxStatus.Text = s;
        }
    }
}
