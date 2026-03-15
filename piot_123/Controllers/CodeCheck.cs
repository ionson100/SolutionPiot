using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using piot_123.Models;
using piotdll;
using piotdll.Models;

namespace piot_123.Controllers;

/// <summary>
/// Поверка кодов от PIOT. Принимает массив кодов, проверяет их и возвращает результат в виде MOut.
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
[ApiController]
[Route("/")]
public class CodeCheck(ILogger<CodeCheck> logger, IOptions<MySettings> options) : ControllerBase
{
    private readonly MySettings _settings = options.Value;

   

    /// <summary>
    /// Проверяет массив кодов
    /// </summary>
    /// <param name="codesList">Массив объектов CodeUnit</param> 
    /// <returns>MOut — результат проверки</returns>
    [HttpPost]
    [Route("/api/v2/check")]
    public async Task<MOut> Post( List<CodeUnit>? codesList)
    {
       


        if (codesList == null || codesList.Count == 0)
        {
            
            //codesList = new List<CodeUnit>() { new() { Km = "0104670540176099215'W9Um\u001d93dGVz" } };
            //codesList = new List<CodeUnit>() { new() { Km = "010461013628057121/798DM%\u001d8005199000\u001d93dGVz" } };
            //codesList = new List<CodeUnit>() { new() { Km = "00840147505712Zz;ZnRbAAAAdGVz",PriceTobaccoGroup = 15000} };
            codesList = new List<CodeUnit>() { new() { Km = "0104670540176099215<pGKy\u001D93dGVz", PriceTobaccoGroup = 15000 } };
            //return new MOut("Список кодов пустой");
        }

        List<string> codes = codesList.Select(c => c.Km).ToList();

        double GetPrice(string s)
        {
            return (from codesIn in codesList where codesIn.Km.Contains(s) select codesIn.PriceTobaccoGroup).FirstOrDefault();
        }

        MainPoint mainPoint = new MainPoint(_settings, GetPrice, true);
        var t = await mainPoint.CheckCode(codes);
        logger.LogInformation(t.LogString);
        return t;
    }

   
}