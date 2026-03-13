namespace piotdll;

public class MySettings
{

    public string UrlPiot { get; set; } = "https://esm-emu.ao-esp.ru/api/v2/codes/check";//"https://localhost:51401/api/v2/codes/check";
    public string UrlLocalModule { get; set; } = "http://localhost:5995/api/v2/cis/outCheck";
    public string Id { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4 ";
    public string Version { get; set; } = "0.0.1";

    public string Name { get; set; } = "bitnic";

    public string Token { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4c0408";

    public int Timeout { get; set; } = 5000;

    public string Authorization { get; set; } = "Basic YW1pdGljOjEyMzQ1Njc4OTA=";   
}