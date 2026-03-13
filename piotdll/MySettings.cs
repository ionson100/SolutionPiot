namespace piotdll;

public class MySettings
{
    // // Остовой URL для проверки локально
    // public const string URL_LOCAL = "https://localhost:51401/api/v2/codes/check";
    // // Основной URL API PIOT эмулятор
    // public const string URL = "https://esm-emu.ao-esp.ru/api/v2/codes/check";
    // // URL локального модуля
    // public const string URL_LM = "http://localhost:5995/api/v2/cis/outCheck";
    //
    // // Информация о кассовом ПО (ПМСР)
    // public const string NAME = "bitnic";
    // public const string VERSION = "0.0.1";
    // public const string ID = "18aa4ecf-523c-4c2a-a759-d0435f4c0408"; // Идентификатор в реестре ГИС МТ
    public string UrlPiot { get; set; } = "https://esm-emu.ao-esp.ru/api/v2/codes/check";
    public string UrlLocalModule { get; set; } = "http://localhost:5995/api/v2/cis/outCheck";
    public string Id { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4 ";
    public string Version { get; set; } = "0.0.1";

    public string Name { get; set; } = "bitnic";

    public string Token { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4c0408";
}