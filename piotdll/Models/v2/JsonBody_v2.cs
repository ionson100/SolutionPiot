using System.Collections.Generic;
using Newtonsoft.Json;

namespace piotdll.Models.v2;

/// <summary>
/// Тело ответа ПИоТ
/// </summary>
public class JsonBodyV2
{
    /// <summary>
    /// Список ответов по кодам
    /// </summary>
    [JsonProperty("codesResponse")]
    public List<CodesResponse> CodesResponse { get; set; }

    /// <summary>
    /// Код ответа
    /// </summary>
    [JsonProperty("code")]
    public int? Code { get; set; }

    /// <summary>
    /// Сообщение
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }
}