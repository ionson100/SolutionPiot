using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ObserverLm.UserControlsSales
{
    /// <summary>
    /// Логика взаимодействия для CodeDialog.xaml
    /// </summary>
    public partial class CodeDialog
    {
        public int Skip { get; private set; }
        public int Limit { get; private set; }
        public CodeDialog()
        {
            InitializeComponent();
            TxtSkip.Text= Properties.Settings.Default.Skip.ToString();
            TxtLimit.Text= Properties.Settings.Default.Limit.ToString();
            TxtSkip.Focus();
            if (!string.IsNullOrEmpty(TxtSkip.Text))
            {
                TxtSkip.CaretIndex = TxtSkip.Text.Length;
            }
        }
        private void OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]");
        }

        private void GetCodes_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TxtSkip.Text, out int skip) && int.TryParse(TxtLimit.Text, out int limit))
            {
                // Проверка диапазона для Limit
                if (limit < 1 || limit > 1000)
                {
                    MessageBox.Show("Лимит должен быть в диапазоне от 1 до 1000.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Skip = skip;
                Limit = limit;
                Properties.Settings.Default.Skip=skip;
                Properties.Settings.Default.Limit=limit;
                Properties.Settings.Default.Save();
                this.DialogResult = true; // Закрывает окно и возвращает успех
            }
            else
            {
                MessageBox.Show("Введите корректные числа.");
            }
        }
    }
}
