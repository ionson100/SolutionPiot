

using System.Collections.Generic;

/// <summary>
/// Модель данных кода маркировки (КИЗ) из ответа PIOT.
/// Содержит статус, атрибуты и метаданные товара для принятия решения о продаже.
/// </summary>
public class ItemCode
{
    /// <summary>
    /// Код идентификации (КИ / КиЗ) из запроса.
    /// </summary>
    public  string Cis { get; set; }

    /// <summary>
    /// Признак наличия кода в ГИС МТ.
    /// true — найден; false — не найден.
    /// </summary>
    public bool Found { get; set; }

    /// <summary>
    /// Результат проверки валидности структуры КИ / КиЗ.
    /// true — структура валидна; false — невалидна.
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// Представление кода без крипто-подписи.
    /// </summary>
    public string PrintView { get; set; }

    /// <summary>
    /// Код товара (GTIN).
    /// </summary>
    public string Gtin { get; set; }

    /// <summary>
    /// Массив идентификаторов товарных групп.
    /// Например: 3 — табачная продукция, 16 — никотиносодержащая продукция.
    /// </summary>
    public List<int> GroupIds { get; set; }

    /// <summary>
    /// Результат проверки крипто-подписи КМ.
    /// true — подпись верна; false — ошибка проверки.
    /// Для «Товары из натурального меха»: true — найден в ИР Маркировки.
    /// </summary>
    public bool Verified { get; set; }

    /// <summary>
    /// Признак возможности реализации (статус «В обороте»).
    /// true — можно продать; false — нельзя.
    /// </summary>
    public bool Realizable { get; set; }

    /// <summary>
    /// Признак нанесения КИ / КиЗ на упаковку.
    /// true — нанесён; false — не нанесён.
    /// </summary>
    public bool Utilised { get; set; }

    /// <summary>
    /// Дата истечения срока годности.
    /// Формат: yyyy-MM-dd’T’HH:mm:ss.SSSz.
    /// </summary>
    public string ExpireDate { get; set; }

    /// <summary>
    /// Информация о вариативном сроке годности (для молочной продукции).
    /// </summary>
    public string VariableExpirations { get; set; }

    /// <summary>
    /// Дата производства.
    /// Формат: yyyy-MM-dd’T’HH:mm:ss.SSSz.
    /// </summary>
    public string ProductionDate { get; set; }

    /// <summary>
    /// Переменный вес продукции в граммах (для молочной продукции).
    /// </summary>
    public int ProductWeight { get; set; }

    /// <summary>
    /// Производственный ветеринарный документ (для молочной продукции).
    /// </summary>
    public string PrVetDocument { get; set; }

    /// <summary>
    /// Признак принадлежности кода запрошенному владельцу (по ИНН).
    /// true — принадлежит; false — не принадлежит.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Признак блокировки продажи по решению ОГВ.
    /// true — заблокирован; false — разрешён.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Органы государственной власти, установившие блокировку.
    /// Возможные значения: RAR, FTS, FNS, RSHN, RPN, MVD, RZN.
    /// </summary>
    public List<string> Ogvs { get; set; }

    /// <summary>
    /// Код ошибки при обработке КМ.
    /// 0 — нет ошибки; остальные — соответствуют конкретным проблемам.
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// Признак включения контроля прослеживаемости.
    /// true — включён; false — выключен.
    /// </summary>
    public bool IsTracking { get; set; }

    /// <summary>
    /// Признак того, что товар уже продан.
    /// true — продан; false — не продан.
    /// </summary>
    public bool Sold { get; set; }

    /// <summary>
    /// Причина выбытия, разрешающая продажу:
    /// 1 — дистанционная продажа / образцы;
    /// 2 — собственные нужды / производство.
    /// </summary>
    public int EliminationState { get; set; }

    /// <summary>
    /// Максимальная розничная цена (в копейках).
    /// Только для табачной, альтернативной и никотиносодержащей продукции.
    /// </summary>
    public double? Mrp { get; set; }

    /// <summary>
    /// Единая минимальная цена (ЕМЦ, в копейках).
    /// Только для табачной продукции.
    /// </summary>
    public int? Smp { get; set; }

    /// <summary>
    /// Признак принадлежности табачной продукции к «серой зоне».
    /// true — серая зона; false — не серая.
    /// </summary>
    public bool GrayZone { get; set; }

    /// <summary>
    /// Количество единиц в упаковке / объём / вес.
    /// Зависит от товарной группы.
    /// </summary>
    public int InnerUnitCount { get; set; }

    /// <summary>
    /// Счётчик проданных единиц / объёма / веса.
    /// </summary>
    public int SoldUnitCount { get; set; }

    /// <summary>
    /// Тип упаковки: "UNIT", "GROUP" и др.
    /// </summary>
    public string PackageType { get; set; }

    /// <summary>
    /// КИ агрегата (родительский код).
    /// Не возвращается для «Товары из натурального меха».
    /// </summary>
    public string Parent { get; set; }

    /// <summary>
    /// ИНН производителя.
    /// Не возвращается для молочной продукции из Беларуси.
    /// </summary>
    public string ProducerInn { get; set; }

    /// <summary>
    /// Номер производственной серии (для медицинских изделий).
    /// </summary>
    public string ProductionSerialNumber { get; set; }

    /// <summary>
    /// Номер производственной партии (для медицинских изделий).
    /// </summary>
    public string ProductionBatchNumber { get; set; }

    /// <summary>
    /// Заводской серийный номер (для медицинских изделий).
    /// </summary>
    public string FactorySerialNumber { get; set; }

    /// <summary>
    /// Ёмкость потребительской упаковки (например, количество пачек в блоке).
    /// По умолчанию: 10.
    /// </summary>
    public int PackageQuantity { get; set; } = 10;

    /// <summary>
    /// Возвращает текстовое описание ошибки по её коду.
    /// </summary>
    /// <returns>Сообщение об ошибке или null, если код 0.</returns>
    private string ServerErrorMessage()
    {
        switch (ErrorCode)
        {
            case 1: return "Ошибка валидации КМ";
            case 2: return "КМ не содержит GTIN";
            case 3: return "КМ не содержит серийный номер";
            case 4: return "КМ содержит недопустимые символы";
            case 5: return "Ошибка верификации крипто-подписи КМ (формат крипто-подписи не соответствует типу КМ)";
            case 6: return "Ошибка верификации крипто-подписи КМ (криптоподпись невалидная)";
            case 7: return "Ошибка верификации крипто-подписи КМ (крипто-ключ не валиден)";
            case 8: return "КМ не прошел верификацию в стране эмитента";
            case 9: return "Найденные AI в КМ не поддерживаются";
            case 10: return "КМ не найден в ГИС МТ";
            case 11: return "КМ не найден в трансгране";
            default: return null;
        }
    }

    /// <summary>
    /// Формирует полное сообщение об ошибке.
    /// </summary>
    /// <param name="altMessage">Сообщение, используемое, если код ошибки неизвестен.</param>
    /// <returns>Строка с префиксом и причиной запрета продажи.</returns>
    public string GetErrorMessage(string altMessage)
    {
        string prefix = "Продажа запрещена. Причина: ";
        string serverMsg = ServerErrorMessage();
        return prefix + (serverMsg ?? altMessage);
    }
}