using System.Collections.Generic;
using System.Text;
using piotdll.Models.v2;

namespace piotdll.Models;

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

    /// <summary>
    /// Лог проверки, для записи в лог файл, удобно при сертификации программы
    /// </summary>
    public string LogString { get; set; } = null!;

    /// <summary>
    /// Возвращает строковое представление объекта.
    /// </summary>
    public override string ToString()
    {
        return $"MOut{{TotalErrorMessage='{TotalErrorMessage}', ItemsList={(ItemsList != null ? ItemsList.Count + " items" : "null")}}}";
    }

    /// <summary>
    /// Формирует много строчный лог с результатами проверки.
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