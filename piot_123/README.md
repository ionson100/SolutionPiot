
> ### 🛑 ВНИМАНИЕ
> **Важное замечание:** \
> В связи пунктом оферты:
> 
> 9.14. Пользователь признает и соглашается, что использование Программы осуществляется на\
>его собственный риск. Все риски, в том числе связанные с производительностью, качеством и\
>результатами использования Программы, лежат на Пользователе.
>
>Разработчик продукта на основе модуля ТС ПИоТ от ООО ЕСП, не несет никакой ответственности\
>за работу программы с этим модулем, в контексте проверки кодов маркировки и достоверности заполнения тэга 1265.\
>Что в конечном итоге может вылиться в убытки предприятия в виде штрафов от налоговой службы или невозможности продажи.

##### Веб сервис для проверки кодов
Создан в тестовых целях, для проверки кодов, которые выдаются при выполнении заданий. Версия API: 2\
В дальнейшем планируется расширить функционал, добавив возможность сохранять результаты и предоставлять статистику по выполнению заданий.\
Решение реализовано на ASP.NET Core Web API, использует Newtonsoft.Json для обработки JSON данных и Swagger для удобного тестирования API.\
Для тестирования кодов:```https://esm-emu.ao-esp.ru/?mode=online&tab=general ```
[gitHub](https://github.com/ionson100/SolutionPiot) \
Для ручной проверки кодов, можно использовать :[sawagger](http://localhost:5253/swagger/index.html)
Внимание!\
Код в swagger должен вставляться полностьтю, включая группы разделителя ```"0104607010350246215kRdG-1%W(Umn\u001d93dGVz"```\
Тело POST запроса предствляет список (List) объектов ```CodeUnit``` 
```csharp
/// <summary>
/// Элемент кода проверки
/// </summary>
public class CodeUnit
{
    /// <summary>
    /// Код маркировки полный, с групповыми разделителями
    /// </summary>
    public string Km { get; set; }=null!;
    //Цена за единицу продукта для группы 3, 15, 16 
    //(табачная, никотиносодержащая и альтернативная продукция), в копейках.) из локальной базы данных. 
    //Для остальных групп товаров должно быть равно 0.
    public int PriceTobaccoGroup { get; set; }
}
```
Адрес запроса: ```http://localhost:5253/api/v2/check ``` \
Ответ:
```csharp
/// <summary>
/// Результат проверки
/// </summary>
public class MOut
{
    /// <summary>
    /// Глобальное сообщение об ошибке, возникшей при выполнении запроса:
    /// Если это поле не null — значит проверка не прошла
    /// </summary>
    public string? TotalErrorMessage { get; set; }

    /// <summary>
    /// Список результатов проверки, по кодам в запросе
    /// </summary>
    public List<MOutItems>? ItemsList { get; set; }

    /// <summary>
    /// Тело ответа в формате JsonBodyV2.
    /// </summary>
    public JsonBodyV2 BodyV2 { get; set; } = null!;

    /// <summary>
    /// Лог проверки, для записи в лог файл, удобно при сертификации программы
    /// </summary>
    public string LogString { get; set; } = null!;

    /// <summary>
    /// Возвращает строковое представление объекта.
    /// </summary>
    public override string ToString()

    /// <summary>
    /// Формирует много строчный лог с результатами проверки.
    /// </summary>
    public string GetStringForLog()
}
```
```csharp
/// <summary>
/// Результат проверки кода маркировки.
/// Содержит статус продажи, причину отказа (если есть), тег 1265 и цену для табачной продукции.
/// Внимание! Проверку лучше осуществлять по одному коду при добавлении в чек,
/// при пакетной проверке модуль может вести себя не предсказуемо
/// </summary>
public class MOutItems : MInItems
{
    /// <summary>
    /// ID группы товара из справочника товаров.
    /// Внимание! При пакетной проверке он на некоторые кода может не возвращаться,
    /// а так же при локальной проверке
    /// </summary>
    public int? CodeGroup { get; set; }

    /// <summary>
    /// Разрешение на продажу: true — можно продать, false — запрещено.
    /// </summary>
    public bool PermitSale { get; set; }

    /// <summary>
    /// Причина запрета продажи. Отображается кассиру.
    /// Может быть null, если продажа разрешена.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Значение тега 1265 (ФНС), формируется при успешной проверке.
    /// Равно null при аварийном режиме (статус 203) — тогда тег 1260 не заполняется.
    /// </summary>
    public string? Tag1265 { get; set; }

    /// <summary>
    /// Минимальная розничная цена (МРЦ) для табачной продукции (группа 3), в рублях.
    /// Должна быть указана в чеке независимо от внутренней цены в учётной системе.
    /// Для серой зоны и локальной проверки (null).
    /// </summary>
    public double? MrcTobacco { get; set; }

    /// <summary>
    /// Формирует много строчный лог с деталями проверки.
    /// </summary>
    public string GetStringForLog()
}
```
```csharp
/// <summary>
/// Тело ответа ПИоТ
/// </summary>
public class JsonBodyV2
{
    /// <summary>
    /// Список ответов по кодам
    /// </summary>
    [JsonProperty("codesResponse")]
    public List<CodesResponse>? CodesResponse { get; set; }

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
```
```csharp
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
    public List<ItemCode>? Codes { get; set; }

    /// <summary>
    /// Уникальный идентификатор запроса. Формат: UUID.
    /// </summary>
    [JsonProperty("reqId")]
    public string? ReqId { get; set; }

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
    public string? Inst { get; set; }

    /// <summary>
    /// Версия ПО «Локальный модуль «Честный ЗНАК».
    /// </summary>
    [JsonProperty("version")]
    public string? Version { get; set; }
}
```C#
/// <summary>
/// Модель данных кода маркировки (КИЗ) из ответа PIOT.
/// Содержит статус, атрибуты и метаданные товара для принятия решения о продаже.
/// </summary>
public class ItemCode
{
    /// <summary>
    /// Код идентификации (КИ / КиЗ) из запроса.
    /// </summary>
    [JsonProperty("cis")]
    public  string? Cis { get; set; }

    /// <summary>
    /// Признак наличия кода в ГИС МТ.
    /// true — найден; false — не найден.
    /// </summary>
    [JsonProperty("found")]
    public bool Found { get; set; }

    /// <summary>
    /// Результат проверки валидности структуры КИ / КиЗ.
    /// true — структура валидна; false — невалидна.
    /// </summary>
    [JsonProperty("valid")]
    public bool Valid { get; set; }

    /// <summary>
    /// Представление кода без крипто-подписи.
    /// </summary>
    [JsonProperty("printView")]
    public string? PrintView { get; set; }

    /// <summary>
    /// Код товара (GTIN).
    /// </summary>
    [JsonProperty("gtin")]
    public string? Gtin { get; set; }

    /// <summary>
    /// Массив идентификаторов товарных групп.
    /// Например: 3 — табачная продукция, 16 — никотиносодержащая продукция.
    /// </summary>
    [JsonProperty("groupIds")]
    public List<int> GroupIds { get; set; }

    /// <summary>
    /// Результат проверки крипто-подписи КМ.
    /// true — подпись верна; false — ошибка проверки.
    /// Для «Товары из натурального меха»: true — найден в ИР Маркировки.
    /// </summary>
    [JsonProperty("verified")]
    public bool Verified { get; set; }

    /// <summary>
    /// Признак возможности реализации (статус «В обороте»).
    /// true — можно продать; false — нельзя.
    /// </summary>
    [JsonProperty("realizable")]
    public bool Realizable { get; set; }

    /// <summary>
    /// Признак нанесения КИ / КиЗ на упаковку.
    /// true — нанесён; false — не нанесён.
    /// </summary>
    [JsonProperty("utilised")]
    public bool Utilised { get; set; }

    /// <summary>
    /// Дата истечения срока годности.
    /// Формат: yyyy-MM-dd’T’HH:mm:ss.SSSz.
    /// </summary>
    [JsonProperty("expireDate")]
    public string? ExpireDate { get; set; }

    /// <summary>
    /// Информация о вариативном сроке годности (для молочной продукции).
    /// </summary>
    [JsonProperty("variableExpirations")]
    public string? VariableExpirations { get; set; }

    /// <summary>
    /// Дата производства.
    /// Формат: yyyy-MM-dd’T’HH:mm:ss.SSSz.
    /// </summary>
    [JsonProperty("productionDate")]
    public string? ProductionDate { get; set; }

    /// <summary>
    /// Переменный вес продукции в граммах (для молочной продукции).
    /// </summary>
    [JsonProperty("productWeight")]
    public int ProductWeight { get; set; }

    /// <summary>
    /// Производственный ветеринарный документ (для молочной продукции).
    /// </summary>
    [JsonProperty("prVetDocument")]
    public string? PrVetDocument { get; set; }

    /// <summary>
    /// Признак принадлежности кода запрошенному владельцу (по ИНН).
    /// true — принадлежит; false — не принадлежит.
    /// </summary>
    [JsonProperty("isOwner")]
    public bool IsOwner { get; set; }

    /// <summary>
    /// Признак блокировки продажи по решению ОГВ.
    /// true — заблокирован; false — разрешён.
    /// </summary>
    [JsonProperty("isBlocked")]
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Органы государственной власти, установившие блокировку.
    /// Возможные значения: RAR, FTS, FNS, RSHN, RPN, MVD, RZN.
    /// </summary>
    [JsonProperty("ogvs")]
    public List<string>? Ogvs { get; set; }

    /// <summary>
    /// Код ошибки при обработке КМ.
    /// 0 — нет ошибки; остальные — соответствуют конкретным проблемам.
    /// </summary>
    [JsonProperty("errorCode")]
    public int ErrorCode { get; set; }

    /// <summary>
    /// Признак включения контроля прослеживаемости.
    /// true — включён; false — выключен.
    /// </summary>
    [JsonProperty("isTracking")]
    public bool IsTracking { get; set; }

    /// <summary>
    /// Признак того, что товар уже продан.
    /// true — продан; false — не продан.
    /// </summary>
    [JsonProperty("sold")]
    public bool Sold { get; set; }

    /// <summary>
    /// Причина выбытия, разрешающая продажу:
    /// 1 — дистанционная продажа / образцы;
    /// 2 — собственные нужды / производство.
    /// </summary>
    [JsonProperty("eliminationState")]
    public int EliminationState { get; set; }

    /// <summary>
    /// Максимальная розничная цена (в копейках).
    /// Только для табачной, альтернативной и никотиносодержащей продукции.
    /// </summary>
    [JsonProperty("mrp")]
    public double? Mrp { get; set; }

    /// <summary>
    /// Единая минимальная цена (ЕМЦ, в копейках).
    /// Только для табачной продукции.
    /// </summary>
    [JsonProperty("smp")]
    public int? Smp { get; set; }

    /// <summary>
    /// Признак принадлежности табачной продукции к «серой зоне».
    /// true — серая зона; false — не серая.
    /// </summary>
    [JsonProperty("grayZone")]
    public bool GrayZone { get; set; }

    /// <summary>
    /// Количество единиц в упаковке / объём / вес.
    /// Зависит от товарной группы.
    /// </summary>
    [JsonProperty("innerUnitCount")]
    public int InnerUnitCount { get; set; }

    /// <summary>
    /// Счётчик проданных единиц / объёма / веса.
    /// </summary>
    [JsonProperty("soldUnitCount")]
    public int SoldUnitCount { get; set; }

    /// <summary>
    /// Тип упаковки: "UNIT", "GROUP" и др.
    /// </summary>
    [JsonProperty("packageType")]
    public string? PackageType { get; set; }

    /// <summary>
    /// КИ агрегата (родительский код).
    /// Не возвращается для «Товары из натурального меха».
    /// </summary>
    [JsonProperty("parent")]
    public string? Parent { get; set; }

    /// <summary>
    /// ИНН производителя.
    /// Не возвращается для молочной продукции из Беларуси.
    /// </summary>
    [JsonProperty("producerInn")]
    public string? ProducerInn { get; set; }

    /// <summary>
    /// Номер производственной серии (для медицинских изделий).
    /// </summary>
    [JsonProperty("productionSerialNumber")]
    public string? ProductionSerialNumber { get; set; }

    /// <summary>
    /// Номер производственной партии (для медицинских изделий).
    /// </summary>
    [JsonProperty("productionBatchNumber")]
    public string? ProductionBatchNumber { get; set; }

    /// <summary>
    /// Заводской серийный номер (для медицинских изделий).
    /// </summary>
    [JsonProperty("factorySerialNumber")]
    public string? FactorySerialNumber { get; set; }

    /// <summary>
    /// Ёмкость потребительской упаковки (например, количество пачек в блоке).
    /// По умолчанию: 10.
    /// </summary>
    [JsonProperty("packageQuantity")]
    public int PackageQuantity { get; set; }

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
```

#### Пример запроса

```
curl --location 'http://localhost:5253/api/v2/check' \
--header 'Content-Type: application/json' \
--data '[
  {
    "km": "00840147505712Zz;ZnRbAAAAdGVz",
    "priceTobaccoGroup": 15000
  }
]'
```
#### Пример ответа
```json
{
  "totalErrorMessage": null,
  "itemsList": [
    {
      "codeGroup": 16,
      "permitSale": false,
      "errorMessage": "Цена единицы товара: 150,00 руб. меньше допустимой минимальной цены: 200,00 руб.",
      "tag1265": "UUID=f7b51a1b-75f0-4c3b-8843-52daa4765dea&Time=1773524146442",
      "mrcTobacco": 150,
      "km": "00840147505712Zz;ZnRbAAAAdGVz",
      "idCase": "5.26",
      "descriptionCase": "Запрет продажи НСП по минимальной розничной цене"
    }
  ],
  "bodyV2": {
    "codesResponse": [
      {
        "code": 0,
        "description": "ok",
        "codes": [
          {
            "cis": "00840147505712Zz;ZnRbAAAAdGVz",
            "found": true,
            "valid": true,
            "printView": "00840147505712Zz;ZnRb",
            "gtin": "00840147505712",
            "groupIds": [
              16
            ],
            "verified": true,
            "realizable": true,
            "utilised": true,
            "expireDate": null,
            "variableExpirations": null,
            "productionDate": "2025-09-18T00:00:00.000Z",
            "productWeight": 0,
            "prVetDocument": null,
            "isOwner": true,
            "isBlocked": false,
            "ogvs": null,
            "errorCode": 0,
            "isTracking": false,
            "sold": false,
            "eliminationState": 0,
            "mrp": 20000,
            "smp": null,
            "grayZone": false,
            "innerUnitCount": 0,
            "soldUnitCount": 0,
            "packageType": "UNIT",
            "parent": null,
            "producerInn": "7731376812",
            "productionSerialNumber": null,
            "productionBatchNumber": null,
            "factorySerialNumber": null,
            "packageQuantity": 0
          }
        ],
        "reqId": "f7b51a1b-75f0-4c3b-8843-52daa4765dea",
        "reqTimestamp": 1773524146442,
        "isCheckedOffline": false,
        "inst": null,
        "version": null
      }
    ],
    "code": null,
    "message": null
  },
  "logString": "
  ************** Проверка кодов с помощью ТС ПИоТ **************\
  Список проверяемых кодов:
  00840147505712Zz;ZnRbAAAAdGVz [MDA4NDAxNDc1MDU3MTJaejtablJiQUFBQWRHVno=]
  URL TC РИоТ: https://esm-emu.ao-esp.ru/api/v2/codes/check
  Тело запроса:
  {\"codes\":[\"MDA4NDAxNDc1MDU3MTJaejtablJiQUFBQWRHVno=\"],\"client_info\":{\"name\":\"bitnic\",\"version\":\"0.0.1\",\"id\":\"18aa4ecf-523c-4c2a-a759-d0435f4 \",\"token\":\"18aa4ecf-523c-4c2a-a759-d0435f4c0408\"}}
  Http code: 200
  Тело ответа:
  {\"codesResponse\":[{\"code\":0,\"description\":\"ok\",\"codes\":[{\"cis\":\"00840147505712Zz;ZnRbAAAAdGVz\",\"found\":true,\"valid\":true,\"printView\":\"00840147505712Zz;ZnRb\",\"gtin\":\"00840147505712\",\"groupIds\":[16],\"verified\":true,\"realizable\":true,\"utilised\":true,\"productionDate\":\"2025-09-18T00:00:00.000Z\",\"isOwner\":true,\"isBlocked\":false,\"errorCode\":0,\"isTracking\":false,\"sold\":false,\"mrp\":20000,\"grayZone\":false,\"packageType\":\"UNIT\",\"producerInn\":\"7731376812\"}],\"reqId\":\"f7b51a1b-75f0-4c3b-8843-52daa4765dea\",\"reqTimestamp\":1773524146442,\"isCheckedOffline\":false}]}\r\nКода были проверены online.
  Результат проверки кодов:
  Код: 00840147505712Zz;ZnRbAAAAdGVz
  Продажа: запретить
  Причина: Цена единицы товара: 150,00 руб. меньше допустимой минимальной цены: 200,00 руб.
  Цена за единицу руб.: 150,00
  Код группы: 16
  Тэг 1265: UUID=f7b51a1b-75f0-4c3b-8843-52daa4765dea&Time=1773524146442
  Тест: 5.26
  Описание теста: Запрет продажи НСП по минимальной розничной цене"
}
```

