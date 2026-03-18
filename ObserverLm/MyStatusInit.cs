using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ObserverLm
{
    class MyStatusInit
    {
        public async Task RequestPiotAsync(string append,Func<string,string> action)
        {


            try
            {
               
                MySettings settings  = MySettings.GetSettings();
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(3000);
              
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {settings.Auth}");
                // Отправка POST-запроса
                string url = settings.Url + append;
                using var response = await httpClient.GetAsync(url);

                int status = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();
                if (status == 200)
                {
                    string prettyJson = JToken.Parse(responseBody).ToString(Formatting.Indented);
                    action.Invoke(prettyJson);
                }
                else
                {
                    string error="Ошибка при запросе к API. Код статуса: " + status+Environment.NewLine+ responseBody+Environment.NewLine+
                                 "Url: "+url;
                    action.Invoke(error);
                }
                    

                
            }
            catch (Exception ex)
            {
                string error = "Ошибка при запросе к API. Exception: " + Environment.NewLine + ex.Message;
                action.Invoke(error);
            }

        }

        class TempInit
        {
            [JsonProperty("token")] 
            public string Token { get; set; } = null!;
        }
        public async Task RequestInitAsync( Func<string, string> action)
        {

            string? url=null;
            string? json=null;
            try
            {
                
                MySettings settings  = MySettings.GetSettings();
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(3000);

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {settings.Auth}");
                // Отправка POST-запроса
                url = settings.Url + "init"; 
                json=JsonConvert.SerializeObject(new TempInit() { Token = settings.Token });
                using var response = await httpClient.PostAsync(url,
                    new StringContent(json, Encoding.UTF8, "application/json"));
                int status = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();
                if (status == 200)
                {
                    string prettyJson = $"Инициализация успешно.Http status:200{Environment.NewLine}Смотри вкладку Status, наблюдай за логами.";
                    action.Invoke(prettyJson);
                }
                else
                {
                    string error = "Ошибка при запросе к API. Код статуса: " + status + Environment.NewLine + responseBody + Environment.NewLine +
                                   "Url: " + url+Environment.NewLine+json;
                    action.Invoke(error);
                }



            }
            catch (Exception ex)
            {
                string error = "Ошибка при запросе к API. Exception: " + Environment.NewLine + ex.Message+Environment.NewLine+url+
                               Environment.NewLine+json;
                action.Invoke(error);
            }

        }
    }
}