using System.Collections.Generic;



/// <summary>
/// Класс, представляющий ответ по коду от ПИоТ.
/// </summary>
public class CodesResponse
{
    /// <summary>
    /// Результат обработки операции.
    /// Возможные значения: «0» — запрос обработан успешно; «4хх», «5хх» — получен неверный запрос.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Текстовое описание результата выполнения метода.
    /// «ok» в случае успешного выполнения или сообщение об ошибке.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Список проверяемых кодов с детальными результатами.
    /// </summary>
    public List<ItemCode> Codes { get; set; }

    /// <summary>
    /// Уникальный идентификатор запроса. Формат: UUID.
    /// </summary>
    public string ReqId { get; set; }

    /// <summary>
    /// Дата и время формирования запроса (в UTC) с точностью до миллисекунд.
    /// </summary>
    public long ReqTimestamp { get; set; }

    /// <summary>
    /// Признак проверки марки в офлайн режиме.
    /// Возможные значения: true — проверка офлайн; false — проверка онлайн.
    /// При значении true необходимо ориентироваться на значение поля isBlocked.
    /// </summary>
    public bool IsCheckedOffline { get; set; }

    /// <summary>
    /// Идентификатор экземпляра ПО «Локальный модуль «Честный ЗНАК».
    /// </summary>
    public string Inst { get; set; }

    /// <summary>
    /// Версия ПО «Локальный модуль «Честный ЗНАК».
    /// </summary>
    public string Version { get; set; }
}