#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObserverLm.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CodeCheckerControl.xaml
    /// </summary>
    public partial class CodeCheckerControl : UserControl
    {
        public CodeCheckerControl()
        {
            //0104670540176099215'W9Um
            InitializeComponent();
            Loaded += (sender, args) => InputTextBox.Focus();
        }

        private async void CheckButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите код для проверки.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            await RequestCodeCheckAsync(InputTextBox.Text.Trim(),InputTextBoxGroup.Text.Trim(), s =>
            {
                OutputTextBox.Text = s;
                return "";
            });

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
           
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (new Regex("[^0-9]+").IsMatch(text))
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

        private class LmListCode
        {
            [JsonProperty("cis_list")] public List<LmItemCode> CisList { get; set; } = new();
        }

        private class LmItemCode
        {
            [JsonProperty("cis")] 
            public String Cis { get; set; } = null!;
            [JsonProperty("pg",NullValueHandling = NullValueHandling.Ignore)]
            public int? Pg { get; set; }
        }

        public async Task RequestCodeCheckAsync(string code,string group, Func<string, string> action)
        {

            LmListCode lmListCode = new LmListCode();
            lmListCode.CisList.Add(new LmItemCode { Cis = code,Pg = string.IsNullOrWhiteSpace(group)?null:int.Parse(group)});
            string? url = null;
            string? json = null;
            LoadingBar.Visibility = Visibility.Visible;
            try
            {
                
                MySettings settings = MySettings.GetSettings();
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(3000);

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {settings.Auth}");
                // Отправка POST-запроса
                url = settings.Url + "cis/outCheck";
                json = JsonConvert.SerializeObject(lmListCode);
                using var response = await httpClient.PostAsync(url,
                    new StringContent(json, Encoding.UTF8, "application/json"));
                int status = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();
                if (status == 200)
                {
                    string prettyJson = JToken.Parse(responseBody).ToString(Formatting.Indented);
                    action.Invoke(prettyJson);
                }
                else
                {
                    string error = "Ошибка при запросе к API. Код статуса: " + status + Environment.NewLine +
                                   responseBody + Environment.NewLine +
                                   "Url: " + url + Environment.NewLine + json;
                    action.Invoke(error);
                }



            }
            catch (Exception ex)
            {
                string error = "Ошибка при запросе к API. Exception: " + Environment.NewLine + ex.Message +
                               Environment.NewLine + url +
                               Environment.NewLine + json;
                action.Invoke(error);
            }
            finally
            {
                LoadingBar.Visibility = Visibility.Collapsed;

            }
        }
    }
}
