using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ObserverLm
{
    internal class MySettings
    {
        /// <summary>
        /// Путь к API локального модуля. Например: http://localhost:8080/api/v1/
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Строка в формате Base64 для авторизации Basic по умолчанию стоит admin:admin
        /// </summary>
        public string Auth { get; set; }
        /// <summary>
        /// Токен для авторизации по API
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Путь к папке логов
        /// </summary>
        public string FolderLog { get; set; }
        /// <summary>
        /// Количество строк в логах, которые будут отображаться при открытии приложения, по умолчанию 100 0 не допускается.
        /// </summary>
        public int Tail { get; set; } = 100;

        public static MySettings GetSettings()
        {
            try
            {
                string str = File.ReadAllText("settings/settings.json");
                var settings = JsonConvert.DeserializeObject<MySettings>(str);
                return settings;
            }
            catch (Exception e)
            {
                MessageBox.Show("Получение настроек, ошибка. Файл не найден или нарушена его структура (json)." +
                                Environment.NewLine +
                                e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

        }
    }
}
