> ### 🛑 ВНИМАНИЕ
> **Важное замечание:** \
> В связи пунктом оферты:\
> 9.14. Пользователь признает и соглашается, что использование Программы осуществляется на\
его собственный риск. Все риски, в том числе связанные с производительностью, качеством и\
результатами использования Программы, лежат на Пользователе.\
Разработчик продукта на основе модуля ТС ПИоТ от ООО ЕСП, не несет никакой ответственности\
за работу программы с этим модулем, в контексте проверки кодов маркировки и достоверности заполнения тэга 1265.\
Что в конечном итоге может вылиться в убытки предприятия в виде штрафов от налоговой службы.

#### piotdll
Написана на С# 
Использование:
```csharp
 internal class Program
    {
        static double  GetPrice(string gtin)
        {
            return 150.0 * 100; //TODO: Заменить на реальное получение цены из БД или справочника
        }
        static void Main(string[] args)
        {
            MainPoint mainPoint=new MainPoint(new MySettings(), GetPrice,true);
            MOut mOut = mainPoint.CheckCode(new List<string>
            {
                "0104670540176099215'W9Um\u001d93dGVz", 
                "0104670540176099215<pGKy\u001d93DGVz"
            }).Result;
            Console.WriteLine(mOut.LogString);
            Console.ReadLine();
        }
    }
```
```csharp
  /// <summary>
  /// Конструктор
  /// Можно держать как синглетон, так и создавать новый каждый раз
  /// </summary>
  /// <param name="mySettings">Ваши настройки проверки</param>
  /// <param name="getPrice">Функция получения цены по gtin в копейках</param>
  /// <param name="useTest">Для тестировании прохождение сертификации включить (true)</param>
  public MainPoint(MySettings mySettings, Func<string, double> getPrice, bool useTest = false)
  {
      MySettings=mySettings;
      GetPrice = getPrice;
      _useTest = useTest;
  }
```
```csharp
/// <summary>
/// Атрибуты и настройки для проверки кодов маркировки через  ПИоТ и локальный модуль Честный ЗНАК.
/// </summary>
public class MySettings
{
    /// <summary>
    /// Адрес модуля ПИоТ для проверки кодов.
    /// real https://localhost:51401/api/v2/codes/check
    /// </summary>
    public string UrlPiot { get; set; } = "https://esm-emu.ao-esp.ru/api/v2/codes/check";

    /// <summary>
    /// Адрес локального модуля Честный ЗНАК для проверки кодов в офлайн режиме.
    /// </summary>
    public string UrlLocalModule { get; set; } = "http://localhost:5995/api/v2/cis/outCheck";

    /// <summary>
    /// Идентификатор программы
    /// </summary>
    public string Id { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4 ";

    /// <summary>
    /// Версия программы
    /// </summary>
    public string Version { get; set; } = "0.0.1";

    /// <summary>
    /// Название программы, может быть любым, например, названием вашей компании или магазина
    /// </summary>
    public string Name { get; set; } = "bitnic";

    /// <summary>
    /// Токен для доступа к API ПИоТ. Выдается при регистрации в системе Честный ЗНАК и необходим для аутентификации при отправке запросов на проверку кодов.
    /// </summary>
    public string Token { get; set; } = "18aa4ecf-523c-4c2a-a759-d0435f4c0408";

    /// <summary>
    /// Таймаут для запросов к API ПИоТ в миллисекундах. Если ответ не будет получен в течение этого времени,
    /// запрос будет считаться неудачным и вернет ошибку. Рекомендуется устанавливать значение, достаточное для обработки запроса,
    /// но не слишком большое, чтобы избежать долгого ожидания при проблемах с сетью или сервером ПИоТ.
    /// </summary>
    public int Timeout { get; set; } = 5000;

    /// <summary>
    /// Параметр авторизации для доступа к локальному модулю Честный ЗНАК. Обычно это строка в формате Base64, содержащая имя пользователя и пароль для аутентификации.
    /// </summary>
    public string Authorization { get; set; } = "Basic Base64(name:password)";   
}
```

           
        