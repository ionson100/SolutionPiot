using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace piotdll;

/// <summary>
/// Утилитарный класс с константами и вспомогательными методами для работы с PIOT и кодами маркировки.
/// </summary>
public static class UtilsPiot
{
    // Остовой URL для проверки локально
    //public const string URL_LOCAL = "https://localhost:51401/api/v2/codes/check";
    // Основной URL API PIOT эмулятор
    //public const string URL = "https://esm-emu.ao-esp.ru/api/v2/codes/check";
    // URL локального модуля
    //public const string URL_LM = "http://localhost:5995/api/v2/cis/outCheck";

    // Информация о кассовом ПО (ПМСР)
    //public const string NAME = "bitnic";
    //public const string VERSION = "0.0.1";
    //public const string ID = "18aa4ecf-523c-4c2a-a759-d0435f4c0408"; // Идентификатор в реестре ГИС МТ

    // Заголовки HTTP-запросов
    //public const string CONTENT_TYPE = "application/json";

    /// <summary>
    /// Кодирует строку КИЗ в формат Base64.
    /// </summary>
    /// <param name="km">Полный код маркировки (КИЗ)</param>
    /// <returns>строка в формате Base64</returns>
    public static string CodeToBase64(string km)
    {
        byte[] originalBytes = Encoding.UTF8.GetBytes(km);
        return Convert.ToBase64String(originalBytes);
    }

    /// <summary>
    /// Извлекает "ядро" кода маркировки (CIS), убирая дополнительные данные после разделителя \u001D.
    /// Для кодов длиной 29 символов — возвращает первые 21 символ.
    /// </summary>
    /// <param name="km">Полный код маркировки</param>
    /// <returns>ядро кода (21 символ) или null, если не удалось извлечь</returns>
    public static string GetCodeCore(string km)
    {
        if (km == null) return null;

        if (km.Length == 29)
        {
            return km.Substring(0, 21);
        }

        int index = km.IndexOf('\u001D'); // GS (Group Separator)
        return index != -1 ? km.Substring(0, index) : null;
    }



    /// <summary>
    /// Находит соответствующий MInItems по коду маркировки (полностью или по ядру).
    /// </summary>
    /// <param name="mInItems">Список входных элементов</param>
    /// <param name="code">Код для поиска (CIS)</param>
    /// <returns>Найденный элемент или null</returns>
    public static MInItems GetMInItem(List<MInItems> mInItems, string code)
    {
        if (mInItems == null || code == null) return null;

        return mInItems.FirstOrDefault(item => code.Equals(item.Km) || (item.Km?.Contains(code) == true));
    }

    #region Отключение проверки сертификатов 

    /// <summary>
    /// Глобально отключает проверку сертификатов SSL для всех последующих HTTPS-запросов.
    /// Использовать только в тестовых средах! В продакшене крайне не рекомендуется.
    /// </summary>
    public static void DisableCertificateValidationGlobally()
    {
        ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;
        // Также можно отключить проверку имени хоста (но обычно достаточно вышеуказанного)
    }

    /// <summary>
    /// Создаёт HttpClientHandler с отключённой проверкой сертификатов.
    /// Позволяет применять настройки только к конкретному HttpClient.
    /// </summary>
    /// <returns>HttpClientHandler с отключённой проверкой</returns>
    public static HttpClientHandler CreateInsecureHttpClientHandler()
    {
        var handler = new HttpClientHandler();
        // Для .NET Core / .NET 5+ используем ServerCertificateCustomValidationCallback
        handler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => true;
        // Для .NET Framework можно установить CheckCertificateRevocationList = false,
        // но лучше использовать тот же callback.
        return handler;
    }

    #endregion
}