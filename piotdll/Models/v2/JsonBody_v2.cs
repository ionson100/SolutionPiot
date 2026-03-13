using System.Collections.Generic;



/// <summary>
/// Тело ответа ПИоТ
/// </summary>
public class JsonBody_v2
{
    /// <summary>
    /// Список ответов по кодам
    /// </summary>
    public List<CodesResponse> CodesResponse { get; set; }

    /// <summary>
    /// Код ответа
    /// </summary>
    public int? Code { get; set; }

    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; set; }
}