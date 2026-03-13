using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using piotdll;
using piotdll.Models;

namespace piot_123.Controllers;

[ApiController]
[Route("/")]
public class CodeCheck(ILogger<CodeCheck> logger, IOptions<MySettings> options) : ControllerBase
{
    private readonly MySettings _settings = options.Value;

    /// <summary>
    /// Проверяет массив кодов
    /// </summary>
    /// <param name="codesList">Массив кодов полный, с разделителями групп</param>
    /// <returns>MOut — результат проверки</returns>
    [HttpPost]
    [Route("/api/v1/check")]
    public async Task<MOut> Post(List<string> codesList)
    {
        codesList = ["0104670540176099215'W9Um\u001d93dGVz"];
        //codesList = new List<string>() { "0104670540176099215<pGKy\u001d93DGVz" };
        MainPoint mainPoint = new MainPoint(_settings, true);
        var t = await mainPoint.CheckCode(codesList);
        logger.LogInformation(t.LogString);
        return t;
    }

}