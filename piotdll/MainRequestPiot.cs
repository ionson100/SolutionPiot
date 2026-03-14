using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using piotdll.Models;
using piotdll.validators;

namespace piotdll;

/// <summary>
/// Класс для выполнения HTTP-запросов к системе PIOT и обработки ответов.
/// В случае ошибок или тайм аутов переключается на локальный модуль проверки.
/// </summary>
public class MainRequestPiot
{
    /// <summary>
    /// Информация о клиенте (кассовом ПО), отправляемая в теле запроса
    /// </summary>
    private class ClientInfo
    {
        /// <summary>
        /// Наименование ПМСР
        /// </summary>
        [JsonProperty("name")]
        public string  Name { get; set; } = null!;

        /// <summary>
        ///  // Версия ПМСР
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; } = null!;

        /// <summary>
        ///  Идентификатор в реестре ГИС МТ
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Токен авторизации
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } = null!;
    }

    /// <summary>
    /// Временная модель тела запроса к PIOT
    /// </summary>
    private class TempBodyPiot
    {

        [JsonProperty("codes")]
        public List<string> Codes { get; set; } = new();


        [JsonProperty("client_info")]
        public ClientInfo ClientInfo { get; set; } = null!;
    }

    /// <summary>
    /// Основной метод для выполнения запроса к PIOT.
    /// При неудаче (ошибка сети, 5xx, тайм аут) — используется локальный модуль.
    /// </summary>
    /// <param name="mInItems">Список входных элементов</param>
    /// <param name="log">Лог-строитель</param>
    /// <param name="resultCallback">Колбэк для возврата результата</param>
    public async Task RequestPiotAsync(List<MInItems> mInItems, StringBuilder log, Action<MOut> resultCallback)
    {
        log.AppendLine();
        log.AppendLine("************** Проверка кодов с помощью ТС ПИоТ **************");
        log.AppendLine("Список проверяемых кодов:");

        HttpResponseMessage? response = null;
        HttpClient? httpClient = null;

        try
        {
            // Формирование тела запроса
            var tempBody = new TempBodyPiot();
            foreach (var item in mInItems)
            {
                string cisBase64 = UtilsPiot.CodeToBase64(item.Km);
                tempBody.Codes.Add(cisBase64);
                log.AppendLine($"{item.Km} [{cisBase64}]");
            }

           
            string token = MainPoint.MySettings.Token
                           ?? throw new InvalidOperationException("Токен не задан");

            tempBody.ClientInfo = new ClientInfo
            {
                Id = MainPoint.MySettings.Id,
                Name = MainPoint.MySettings.Name,
                Version = MainPoint.MySettings.Version,
                Token = token
            };

            // Сериализация в JSON
            string jsonBody = JsonConvert.SerializeObject(tempBody);
            byte[] postDataBytes = Encoding.UTF8.GetBytes(jsonBody);

            // Выбор URL в зависимости от режима отладки
            string urlCore =  MainPoint.MySettings.UrlPiot;
            log.AppendLine($"URL TC РИоТ: {urlCore}");
            log.AppendLine("Тело запроса:");
            log.AppendLine(jsonBody);

            // Настройка HttpClient
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(MainPoint.MySettings.Timeout); // Тайм аут соединения и чтения

            using var request = new HttpRequestMessage(HttpMethod.Post, urlCore);
            request.Content = new ByteArrayContent(postDataBytes);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // Отправка запроса
            response = await httpClient.SendAsync(request);
            int status = (int)response.StatusCode;
            log.AppendLine($"Http code: {status}");

            string responseBody = await response.Content.ReadAsStringAsync();
            log.AppendLine("Тело ответа:");
            log.AppendLine(responseBody);

            // Обработка ответа по статусу
            switch (status)
            {
                case 200:
                    MOut mOut = new MainValidator().Validate(responseBody, mInItems);
                    if (!string.IsNullOrEmpty(mOut.TotalErrorMessage))
                    {
                        log.AppendLine($"Произошла общая ошибка при проверке кодов: {mOut.TotalErrorMessage}");
                    }
                    else
                    {
                        if (mOut.BodyV2.CodesResponse.First().IsCheckedOffline)
                        {
                            log.AppendLine("Кода были проверены локально.");
                        }
                        else
                        {
                            log.AppendLine("Кода были проверены online.");
                        }
                        log.AppendLine(mOut.GetStringForLog());
                    }
                    resultCallback(mOut);
                    break;

                case 404:
                    string errorMessage =$"Путь Url: {MainPoint.MySettings.UrlPiot} не найден (404)";
                    log.AppendLine($"Ошибка проверки кодов: {errorMessage}");
                    HandleError(resultCallback, errorMessage);
                    break;

                case 203:
                    log.AppendLine("ТС_ПИоТ вернул аварийный режим 203");
                    ReturnSuccessForAll(resultCallback, mInItems, log);
                    break;

                default:
                    if (status >= 400 && status < 500)
                    {
                        string errorText = $"Клиентская ошибка: код {status}{Environment.NewLine}{responseBody}";
                        log.AppendLine(errorText);
                        HandleError(resultCallback, errorText);
                    }
                    else
                    {
                        // Серверные ошибки (5xx): fallback на локальный модуль
                        log.AppendLine($"ТС ПИоТ вернул статус: {status}. Переходим к проверке через локальный модуль.");
                        await ErrorAction(mInItems, log, resultCallback);
                    }
                    break;
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || ex.InnerException is SocketException)
        {
            // Тайм аут соединения или неизвестный хост
            log.AppendLine("ТС ПИоТ ошибка подключения либо тайм-аут.");
            log.AppendLine("Переходим к проверке через локальный модуль.");
            await ErrorAction(mInItems, log, resultCallback);
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException)
        {
            // Например, UnknownHostException
            log.AppendLine("ТС ПИоТ ошибка подключения (неизвестный хост).");
            log.AppendLine("Переходим к проверке через локальный модуль.");
            await ErrorAction(mInItems, log, resultCallback);
        }
        catch (Exception ex)
        {
            // Любая другая ошибка (парсинг, внутренняя)
            var errorOut = new MOut { TotalErrorMessage = $"Внутренняя ошибка: {ex.Message}" };
            resultCallback(errorOut);
        }
        finally
        {
            httpClient?.Dispose();
            response?.Dispose();
        }
    }

    private async Task ErrorAction(List<MInItems> mInItems, StringBuilder log, Action<MOut> resultCallback)
    {
        MOut mOut1 = await Task.Run(() => ProxyLocal(mInItems, log)); // Локальный модуль может быть синхронным
        if (!string.IsNullOrEmpty(mOut1.TotalErrorMessage))
        {
            log.AppendLine($"Произошла ошибка при проверке через локальный модуль: {mOut1.TotalErrorMessage}");
        }
        else
        {
            log.AppendLine(mOut1.GetStringForLog());
        }
        resultCallback(mOut1);
    }

    /// <summary>
    /// Возвращает ошибку через колбэк
    /// </summary>
    private void HandleError(Action<MOut> resultCallback, string message)
    {
        resultCallback(new MOut { TotalErrorMessage = message });
    }

    /// <summary>
    /// Возвращает успешный ответ для всех товаров
    /// </summary>
    private void ReturnSuccessForAll(Action<MOut> resultCallback, List<MInItems> mInItems, StringBuilder log)
    {
        var mOut = new MOut
        {
            ItemsList = mInItems.Select(item => new MOutItems
            {
                DescriptionCase = item.DescriptionCase,
                Km = item.Km,
                IdCase = item.IdCase,
                PermitSale = true
            }).ToList()
        };
        log.AppendLine(mOut.GetStringForLog());
        resultCallback(mOut);
    }

    /// <summary>
    /// Fallback-метод: запрос к локальному модулю при недоступности PIOT
    /// </summary>
    private async Task<MOut> ProxyLocal(List<MInItems> mInItems, StringBuilder log)
    {
        var localResponse = await new MainRequestLocalModule().CheckAsync(mInItems, log);
        var mOut = new MOut();

        if (!string.IsNullOrEmpty(localResponse.TotalError))
        {
            mOut.TotalErrorMessage = localResponse.TotalError;
            return mOut;
        }

        mOut.ItemsList = new List<MOutItems>(localResponse.CodeItems.Count);
        try
        {
            foreach (var codeItem in localResponse.CodeItems)
            {
                var mIn = UtilsPiot.GetMInItem(mInItems, codeItem.Cis);
                mOut.ItemsList.Add(new MOutItems
                {
                    DescriptionCase = mIn?.DescriptionCase,
                    IdCase = mIn?.IdCase,
                    Km = mIn?.Km ?? codeItem.Cis,
                    Tag1265 = codeItem.Tag1265,
                    PermitSale = codeItem.PermitSale,
                    ErrorMessage = codeItem.ErrorMessage
                });
            }
        }
        catch (Exception ex)
        {
            mOut.TotalErrorMessage = $"Ошибка при обработке ответа локального модуля: {ex.Message}";
        }

        return mOut;
    }
}