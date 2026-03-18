using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ObserverLm
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private readonly MySettings _settings;
        public SettingsControl()
        {
            InitializeComponent();
            
            _settings  = MySettings.GetSettings();
            TxtUrl.Text = _settings.Url;
            TxtBasic.Text = _settings.Auth;
            TxtToken.Text = _settings.Token;
            TxtFolderPath.Text = _settings.FolderLog;
            TxtTail.Text = _settings.Tail.ToString();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры (0-9)
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            // Проверка при вставке (Ctrl+V)
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (text != null && new Regex("[^0-9]+").IsMatch(text))
                {
                    e.CancelCommand();
                    Dispatcher.BeginInvoke(new Action(() => {
                        MessageBox.Show("Only numbers allowed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));

                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
           
                var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true, // Ключевое свойство
                Title = "Выберите папку",
                DefaultDirectory = _settings.FolderLog
                
            };
                
            
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TxtFolderPath.Text = dialog.FileName;
            }
        }

        bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TxtBasic.Text))
            {
                MessageBox.Show("Basic is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtUrl.Text))
            {
                MessageBox.Show("Url is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtTail.Text))
            {
                MessageBox.Show("Tail is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtToken.Text))
            {
                MessageBox.Show("Token is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtFolderPath.Text))
            {
                MessageBox.Show("Folder logs is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            bool isValid = Uri.TryCreate(TxtUrl.Text.Trim(), UriKind.Absolute, out Uri uriResult)
                           && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isValid)
            {
                MessageBox.Show("Url string not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!IsBase64String(TxtBasic.Text.Trim()))
            {
                MessageBox.Show("Basic is not BASE64", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Guid.TryParse(TxtToken.Text.Trim(), out _))
            {
                MessageBox.Show("Token is  not UUID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Directory.Exists(TxtFolderPath.Text.Trim()))
            {
                MessageBox.Show("This is not an existing folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (int.TryParse(TxtTail.Text.Trim(), out int result) && result > 0)
            {
                
            }
            else
            {
                MessageBox.Show("The tail string must not be empty and greater than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public bool IsBase64String(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64) || base64.Length % 4 != 0)
                return false;

            try
            {
                _ = Convert.FromBase64String(base64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           bool res= Validate();
           if (res)
           {
               _settings.FolderLog= TxtFolderPath.Text.Trim();
               _settings.Auth= TxtBasic.Text.Trim();
               _settings.Token= TxtToken.Text.Trim();
               _settings.Url= TxtUrl.Text.Trim();
               _settings.Tail= int.Parse(TxtTail.Text.Trim());
               File.WriteAllText("settings/settings.json",JsonConvert.SerializeObject(_settings, Formatting.Indented));


                MessageBox.Show("Ok", "Save Settings", MessageBoxButton.OK, MessageBoxImage.Information);
           }

        }
    }
}
