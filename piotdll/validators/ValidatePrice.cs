using piotdll.Models;

namespace piotdll.validators;

/// <summary>
/// Утилитарный класс для проверки цены товаров по группам (3, 12, 16).
/// Проверяет соответствие цены продажи минимальной розничной цене (MRC/МРЦ).
/// </summary>
internal static class ValidatePrice
{
    /// <summary>
    /// Основной метод валидации цены в зависимости от группы товара.
    /// </summary>
    /// <param name="mOut">Результат проверки (обновляется: цена, разрешение, ошибка)</param>
    /// <param name="code">Данные о коде маркировки от PIOT</param>
    public static void Validate(MOutItems mOut, ItemCode code)
    {
        if (code.GroupIds == null || code.GroupIds.Count == 0)
        {
            mOut.PermitSale = true; // Нет группы — разрешаем по умолчанию
            return;
        }

        if (code.GroupIds.Contains(3))
        {
            ValidatePrice3(mOut, code);
        }
        else if (code.GroupIds.Contains(16))
        {
            ValidatePrice16(mOut, code);
        }
        else
        {
            // Группа 12 и другие — разрешаем продажу без проверки цены
            mOut.PermitSale = true;
        }
    }

    /// <summary>
    /// Проверка цены для табачной продукции (группа 3).
    /// Поддерживает упаковку типа "UNIT" (штука) и "GROUP" (блок).
    /// </summary>
    private static void ValidatePrice3(MOutItems mOut, ItemCode code)
    {
        if (!code.GroupIds.Contains(3))
        {
            mOut.PermitSale = true;
            return;
        }

        switch (code.PackageType)
        {
            case "UNIT":
                double mrcUnit = MrcBuilder.GetMrc(code.Cis);
                mOut.MrcTobacco = mrcUnit;

                if (code.Smp.HasValue && mrcUnit * 100 < code.Smp.Value)
                {
                    mOut.PermitSale = false;
                    mOut.ErrorMessage = string.Format(
                        "Цена пачки: {0:F2} руб. меньше допустимой минимальной цены: {1:F2} руб.",
                        mrcUnit, code.Smp.Value / 100.0
                    );
                }
                else
                {
                    mOut.PermitSale = true;
                }
                break;

            case "GROUP":
                // Извлекаем цену из подстроки кода (позиции 30-36, 6 символов)
                string priceStr = code.Cis.Substring(30, 6);
                double priceGroupKopecks = double.Parse(priceStr, System.Globalization.CultureInfo.InvariantCulture); // цена блока в копейках
                double priceGroupRubles = priceGroupKopecks / 100.0;
                mOut.MrcTobacco = priceGroupRubles;

                if (code.Smp.HasValue)
                {
                    double minTotalPrice = code.Smp.Value * code.PackageQuantity;
                    if (priceGroupKopecks < minTotalPrice)
                    {
                        mOut.PermitSale = false;
                        mOut.ErrorMessage = string.Format(
                            "Цена блока: {0:F2} руб. меньше допустимой минимальной цены: {1:F2} руб.",
                            priceGroupRubles, minTotalPrice / 100.0
                        );
                    }
                    else
                    {
                        mOut.PermitSale = true;
                    }
                }
                else
                {
                    mOut.PermitSale = true;
                }
                break;

            default:
                mOut.PermitSale = true;
                break;
        }
    }

    /// <summary>
    /// Проверка цены для никотиносодержащей продукции (группа 16).
    /// На данный момент поддерживается только тип упаковки "UNIT".
    /// </summary>
    private static void ValidatePrice16(MOutItems mOut, ItemCode code)
    {
        switch (code.PackageType)
        {
            case "UNIT":
                double priceKopecks = GetPriceNsp(code.Gtin);
                double priceRubles = priceKopecks / 100.0;
                mOut.MrcTobacco = priceRubles;

                if (code.Mrp.HasValue && code.Mrp.Value > priceKopecks)
                {
                    mOut.PermitSale = false;
                    mOut.ErrorMessage = string.Format(
                        "Цена единицы товара: {0:F2} руб. меньше допустимой минимальной цены: {1:F2} руб.",
                        priceRubles, code.Mrp.Value / 100.0
                    );
                }
                else
                {
                    mOut.PermitSale = true;
                }
                break;

            case "GROUP":
                mOut.PermitSale = false;
                mOut.ErrorMessage = "Проверка цены для групповой упаковки (GROUP) никотиносодержащей продукции не реализована.";
                // Вместо исключения возвращаем ошибку, как в Java (за комментировано исключение)
                break;

            default:
                mOut.PermitSale = true;
                break;
        }
    }

    /// <summary>
    /// Получает цену за единицу товара из внешнего источника (например, базы данных).
    /// Временная реализация — возвращает фиксированное значение.
    /// </summary>
    /// <param name="gtin">GTIN товара</param>
    /// <returns>цена в копейках</returns>
    private static double GetPriceNsp(string gtin)
    {
        // TODO: Заменить на реальное получение цены из БД или справочника
        return 150.0 * 100; // 150 рублей → 15000 копеек
    }
}