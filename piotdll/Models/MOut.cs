using System.Collections.Generic;
using System.Text;


/// <summary>
/// Результат проверки
/// </summary>
public class MOut
{
    /// <summary>
    /// Глобальное сообщение об ошибке, возникшей при выполнении запроса:
    /// Если это поле не null — значит проверка не прошла
    /// </summary>
    public string TotalErrorMessage { get; set; }

    /// <summary>
    /// Список результатов проверки.
    /// </summary>
    public List<MOutItems> ItemsList { get; set; }

    /// <summary>
    /// Тело ответа в формате v2.
    /// </summary>
    public JsonBody_v2 BodyV2 { get; set; }

    /// <summary>
    /// Конструктор по умолчанию — инициализирует объект с пустыми полями.
    /// </summary>
    public MOut()
    {
        TotalErrorMessage = null;
        ItemsList = null;
    }

    /// <summary>
    /// Удобный конструктор для создания результата с общей ошибкой.
    /// </summary>
    /// <param name="errorMessage">текст ошибки</param>
    public MOut(string errorMessage) : this()
    {
        TotalErrorMessage = errorMessage;
    }

    public string LogString { get; set; }

    /// <summary>
    /// Возвращает строковое представление объекта.
    /// </summary>
    public override string ToString()
    {
        return $"MOut{{TotalErrorMessage='{TotalErrorMessage}', ItemsList={(ItemsList != null ? ItemsList.Count + " items" : "null")}}}";
    }

    /// <summary>
    /// Формирует многострочный лог с результатами проверки.
    /// </summary>
    public string GetStringForLog()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Результат проверки кодов:");
        if (ItemsList != null)
        {
            foreach (var item in ItemsList)
            {
                stringBuilder.AppendLine(item.GetStringForLog());
            }
        }
        return stringBuilder.ToString();
    }

}