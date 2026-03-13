using System;
using System.Text;

namespace piotdll.Models;

/// <summary>
/// Результат проверки кода маркировки.
/// Содержит статус продажи, причину отказа (если есть), тег 1265 и цену для табачной продукции.
/// Внимание! Проверку лучше осуществлять по одному коду, при пакетной проверке модуль может во...
/// </summary>
public class MOutItems : MInItems
{
    /// <summary>
    /// ID группы товара из справочника товаров.
    /// Внимание! При пакетной проверке он на некоторые кода может не возвращаться.
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
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Значение тега 1265 (ФНС), формируется при успешной проверке.
    /// Равно null при аварийном режиме (статус 203) — тогда тег 1260 не заполняется.
    /// </summary>
    public string Tag1265 { get; set; }

    /// <summary>
    /// Минимальная розничная цена (МРЦ) для табачной продукции (группа 3), в рублях.
    /// Должна быть указана в чеке независимо от внутренней цены в учётной системе.
    /// Для серой зоны и локальной проверки (null).
    /// </summary>
    public double? MrcTobacco { get; set; }

    /// <summary>
    /// Конструктор по умолчанию — инициализирует поля значениями по умолчанию.
    /// </summary>
    public MOutItems()
    {
        PermitSale = true;
        ErrorMessage = null;
        Tag1265 = null;
        MrcTobacco = null;
    }

    /// <summary>
    /// Удобный конструктор для создания результата с основными полями.
    /// </summary>
    /// <param name="km">Код маркировки</param>
    /// <param name="permitSale">Разрешение на продажу</param>
    /// <param name="errorMessage">Причина отказа (или null)</param>
    public MOutItems(string km, bool permitSale, string errorMessage) : this()
    {
        Km = km;
        PermitSale = permitSale;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Возвращает строковое представление объекта.
    /// </summary>
    public override string ToString()
    {
        return $"MOutItems{{IdCase='{IdCase}', DescriptionCase='{DescriptionCase}', Km='{Km}', PermitSale={PermitSale}, ErrorMessage='{ErrorMessage}', Tag1265='{Tag1265}', MrcTobacco={(MrcTobacco != null ? MrcTobacco.Value.ToString("F2") : "null")}}}";
    }

    /// <summary>
    /// Формирует много строчный лог с деталями проверки.
    /// </summary>
    public string GetStringForLog()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Код: {Km}");
        sb.AppendLine($"Продажа: {(PermitSale ? "разрешить" : "запретить")}");
        if (!PermitSale)
        {
            sb.AppendLine($"Причина: {ErrorMessage}");
        }
        if (MrcTobacco != null)
        {
            sb.AppendLine($"Цена за единицу руб.: {MrcTobacco.Value:F2}");
        }
        if (CodeGroup != null)
        {
            sb.AppendLine($"Код группы: {CodeGroup}");
        }
       
        sb.AppendLine($"Тэг 1265: {Tag1265}"); 
        if(this.IdCase!=null)
        {
            sb.AppendLine($"Тест: {IdCase}");
        }

        if (DescriptionCase!=null)
        {
            sb.AppendLine($"Описание теста: {DescriptionCase}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Сравнивает текущий объект с другим для определения равенства.
    /// </summary>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null || GetType() != obj.GetType()) return false;

        var that = (MOutItems)obj;

        return PermitSale == that.PermitSale &&
               string.Equals(Km, that.Km) &&
               string.Equals(ErrorMessage, that.ErrorMessage) &&
               string.Equals(Tag1265, that.Tag1265) &&
               Nullable.Equals(MrcTobacco, that.MrcTobacco);
    }

    /// <summary>
    /// Возвращает хэш-код экземпляра.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Km?.GetHashCode() ?? 0);
            hash = hash * 31 + PermitSale.GetHashCode();
            hash = hash * 31 + (ErrorMessage?.GetHashCode() ?? 0);
            hash = hash * 31 + (Tag1265?.GetHashCode() ?? 0);
            hash = hash * 31 + (MrcTobacco?.GetHashCode() ?? 0);
            return hash;
        }
    }
}