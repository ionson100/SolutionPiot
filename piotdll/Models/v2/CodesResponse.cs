using System.Collections.Generic;
using Newtonsoft.Json;

namespace piotdll.Models.v2;

/// <summary>
/// Класс, представляющий ответ по коду от ПИоТ.
/// </summary>
public class CodesResponse
{
    /// <summary>
    /// Результат обработки операции.
    /// Возможные значения: «0» — запрос обработан успешно; «4хх», «5хх» — получен неверный запрос.
    /// </summary>
    [JsonProperty("code")]
    public int Code { get; set; }

    /// <summary>
    /// Текстовое описание результата выполнения метода.
    /// «ok» в случае успешного выполнения или сообщение об ошибке.
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Список проверяемых кодов с детальными результатами.
    /// </summary>
    [JsonProperty("codes")]
    public List<ItemCode> Codes { get; set; }

    /// <summary>
    /// Уникальный идентификатор запроса. Формат: UUID.
    /// </summary>
    [JsonProperty("reqId")]
    public string ReqId { get; set; }

    /// <summary>
    /// Дата и время формирования запроса (в UTC) с точностью до миллисекунд.
    /// </summary>
    [JsonProperty("reqTimestamp")]
    public long ReqTimestamp { get; set; }

    /// <summary>
    /// Признак проверки марки в офлайн режиме.
    /// Возможные значения: true — проверка офлайн; false — проверка онлайн.
    /// При значении true необходимо ориентироваться на значение поля isBlocked.
    /// </summary>
    [JsonProperty("isCheckedOffline")]
    public bool IsCheckedOffline { get; set; }

    /// <summary>
    /// Идентификатор экземпляра ПО «Локальный модуль «Честный ЗНАК».
    /// </summary>
    [JsonProperty("inst")]
    public string Inst { get; set; }

    /// <summary>
    /// Версия ПО «Локальный модуль «Честный ЗНАК».
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }
}