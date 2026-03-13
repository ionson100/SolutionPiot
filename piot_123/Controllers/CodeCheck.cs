using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using piotdll;

namespace piot_123.Controllers;

[ApiController]
[Route("/")]
public class CodeCheck : ControllerBase
{
    private readonly ILogger<CodeCheck> _logger;
    private readonly MySettings _settings;
    public CodeCheck(ILogger<CodeCheck> logger, IOptions<MySettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    [HttpPost]
    [Route("/api/v1/check")]
    public async Task<MOut> Post(List<string> codesList)
    {
        
        codesList = new List<string>() { "0104670540176099215'W9Um\u001d93dGVz" };
        //codesList = new List<string>() { "0104670540176099215<pGKy\u001d93DGVz" };
        MainPoint mainPoint=new MainPoint(new MySettings(),true);
       var t= await mainPoint.CheckCode(codesList);
       _logger.LogInformation(t.LogString);
        return t;
    }

}