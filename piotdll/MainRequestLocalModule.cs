using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using piotdll.Models;

namespace piotdll;

/// <summary>
/// Модуль для обращения к локальному модулю проверки кодов маркировки.
/// Используется как fallback при недоступности основного сервиса PIOT.
/// </summary>
public class MainRequestLocalModule
{
    // Входные данные для запроса к локальному модулю
    private class LmListCode
    {
        [JsonProperty("cis_list")]
        public List<LmItemCode> CisList { get; set; } = new();
    }

    private class LmItemCode
    {
        [JsonProperty("cis")]
        public string Cis { get; set; }
    }

    // Ответ от локального модуля (публичные классы для возврата)
    public class LocalResponseCodeItem
    {
        public string Cis { get; set; }
        public bool PermitSale { get; set; }
        public string ErrorMessage { get; set; }
        public string Tag1265 { get; set; }
    }

    public class LocalResponse
    {
        public string TotalError { get; set; }
        public List<LocalResponseCodeItem> CodeItems { get; set; } = new();
    }

    // Модели для десериализации ответа от локального модуля
    private class Code
    {
        [JsonProperty("sold")]
        public bool Sold { get; set; }


        [JsonProperty("isBlocked")]
        public bool IsBlocked { get; set; }


        [JsonProperty("gtin")]
        public string Gtin { get; set; }


        [JsonProperty("cis")]
        public string Cis { get; set; }
    }

    private class Result
    {

        [JsonProperty("red_id")]
        public string ReqId { get; set; }

      
        public List<Code> Codes { get; set; }


        [JsonProperty("red_timestamp")]
        public long ReqTimestamp { get; set; }


        [JsonProperty("inst")]
        public string Inst { get; set; }


        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

      
        public int Code { get; set; }
    }

    private class Root
    {
        [JsonProperty]
        public List<Result> Results { get; set; }
    }

    /// <summary>
    /// Отправляет список кодов в локальный модуль и возвращает результат проверки.
    /// </summary>
    /// <param name="mInItems">Список входных элементов (с кодами маркировки)</param>
    /// <param name="log">Лог-строитель</param>
    /// <returns>LocalResponse — результат проверки или ошибка</returns>
    public async Task<LocalResponse> CheckAsync(List<MInItems> mInItems, StringBuilder log)
    {
        var localResponse = new LocalResponse();

        // Формирование тела запроса
        var bodyListCode = new LmListCode();
        foreach (var item in mInItems)
        {
            bodyListCode.CisList.Add(new LmItemCode
            {
                Cis = UtilsPiot.GetCodeCore(item.Km) // Извлечение ядра КИЗ
            });
        }

        // Сериализация в JSON
        var jsonBody = JsonConvert.SerializeObject(bodyListCode);

        log.AppendLine("Проверка через локальный модуль.");
        log.AppendLine($"URL LM: {MainPoint.MySettings.UrlLocalModule}");
        log.AppendLine("Тело запроса:");
        log.AppendLine(jsonBody);

    
        var authToken = MainPoint.MySettings.Authorization
                        ?? throw new InvalidOperationException("AUTHORIZATION environment variable is not set.");

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
        httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

        try
        {
            // Отправка POST-запроса
            using var response = await httpClient.PostAsync(MainPoint.MySettings.UrlLocalModule,
                new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            int status = (int)response.StatusCode;
            string responseBody = await response.Content.ReadAsStringAsync();

            log.AppendLine($"Html код: {status}");
            log.AppendLine($"Тело ответа: {responseBody}");

            if (status != 200)
            {
                localResponse.TotalError = $"Ошибка обращения к локальному модулю: HTTP {status}. Ответ: {responseBody}";
                return localResponse;
            }

            // Парсинг JSON-ответа
            var root = JsonConvert.DeserializeObject<Root>(responseBody);
            if (root?.Results == null || root.Results.Count == 0)
            {
                localResponse.TotalError = "Ответ локального модуля пуст: отсутствует поле 'results'.";
                return localResponse;
            }

            var result = root.Results[0];

            if (result.Code != 0)
            {
                localResponse.TotalError = $"Ошибка локального модуля: code={result.Code}, описание={result.Description}";
                return localResponse;
            }

            if (result.Codes == null || result.Codes.Count == 0)
            {
                localResponse.TotalError = "Локальный модуль вернул пустой список кодов.";
                return localResponse;
            }

            // Формирование результата
            foreach (var code in result.Codes)
            {
                var codeItem = new LocalResponseCodeItem
                {
                    Cis = code.Cis,
                    PermitSale = !code.IsBlocked
                };

                if (!codeItem.PermitSale)
                {
                    codeItem.ErrorMessage = "Код не прошел проверку в локальном модуле (заблокирован)";
                }

                // Формирование тэга 1265
                codeItem.Tag1265 = string.Format(
                    "UUID={0}&Time={1}&Inst={2}&Ver={3}",
                    result.ReqId, result.ReqTimestamp, result.Inst, result.Version
                );

                localResponse.CodeItems.Add(codeItem);
            }

            return localResponse;
        }
        catch (Exception ex)
        {
            localResponse.TotalError = $"Исключение при обращении к локальному модулю: {ex.Message}";
            // В реальном приложении можно добавить логирование через ILogger
            return localResponse;
        }
    }
}