using System;



namespace piotdll.validators;

/// <summary>
/// Валидатор отдельного кода маркировки (КИЗ).
/// Проверяет статус, срок годности, структуру и другие атрибуты товара,
/// чтобы определить, разрешена ли его продажа.
/// </summary>
internal class ValidateItem : BaseValidator
{
    /// <summary>
    /// Основной метод валидации одного кода маркировки.
    /// </summary>
    /// <param name="code">Данные о коде из ответа PIOT</param>
    /// <returns>MOutItems — результат проверки с флагом разрешения продажи и сообщением об ошибке (если есть)</returns>
    public MOutItems Validate(ItemCode code)
    {
        var mOut = new MOutItems();

        if (code.GroupIds != null && code.GroupIds.Count > 0)
        {
            mOut.CodeGroup = code.GroupIds[0];
        }

        mOut.Km = code.Cis;

        // Код не найден в системе ГИС МТ
        if (!code.Found)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Код не найден в системе ГИС МТ");
            mOut.PermitSale = false;
            return mOut;
        }

        // Неверная структура кода
        if (!code.Valid)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Структура кода не верная");
            mOut.PermitSale = false;
            return mOut;
        }

        // Криптографическая подпись не прошла проверку
        if (!code.Verified)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Крипто-хвост кода не прошёл проверку");
            mOut.PermitSale = false;
            return mOut;
        }

        // Код заблокирован по решению органов государственной власти
        if (code.IsBlocked)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Код продукта заблокирован для продажи по решению ОГВ");
            mOut.PermitSale = false;
            return mOut;
        }

        // Товар уже выведен из оборота (продан)
        if (code.Sold)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Товар выведен из оборота (продан)");
            mOut.PermitSale = false;
            return mOut;
        }

        // Код маркировки ещё не нанесён на упаковку
        if (!code.Utilised)
        {
            mOut.ErrorMessage = code.GetErrorMessage("Код маркировки не нанесён");
            mOut.PermitSale = false;
            return mOut;
        }

        // Проверка реализуемости: основное правило
        if (!code.Realizable)
        {
            // Исключение: табачная продукция (groupIds содержит 3) в серой зоне
            if (code.GroupIds != null && code.GroupIds.Contains(3) && code.GrayZone == true)
            {
                mOut.PermitSale = true;
                return mOut;
            }

            // Во всех остальных случаях — запрет продажи
            mOut.ErrorMessage = code.GetErrorMessage(
                "Запрет продажи товара при отсутствии в информационной системе мониторинга сведений о его вводе в оборот."
            );
            mOut.PermitSale = false;
            return mOut;
        }

        // Проверка срока годности
        string expireError = CheckExpire(code);
        if (expireError != null)
        {
            mOut.ErrorMessage = code.GetErrorMessage(expireError);
            mOut.PermitSale = false;
            return mOut;
        }

        // Проверка цены (MRC для табака и других товаров)
        ValidatePrice.Validate(mOut, code);

        // Все проверки пройдены
        mOut.PermitSale = true;
        return mOut;
    }

    /// <summary>
    /// Проверяет, не истек ли срок годности товара.
    /// </summary>
    /// <param name="codePiot">Данные о коде</param>
    /// <returns>Сообщение об ошибке, если просрочено; иначе — null</returns>
    private string CheckExpire(ItemCode codePiot)
    {
        if (string.IsNullOrEmpty(codePiot.ExpireDate))
            return null;

        if (DateTime.TryParseExact(codePiot.ExpireDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime expireDate))
        {
            if (expireDate < DateTime.Today)
            {
                return $"Продукт просрочен.\nДата окончания реализации: {expireDate:yyyy-MM-dd}";
            }
        }
        else
        {
            // Если дата не распарсилась, можно вернуть ошибку формата, но по логике оригинала — null
            // В оригинале ParseException, здесь мы просто игнорируем или логируем.
            // Оставим null, чтобы не блокировать продажу из-за кривой даты.
        }

        return null;
    }
}