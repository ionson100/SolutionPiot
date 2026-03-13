using System;

namespace piotdll;

/// <summary>
/// Утилита для извлечения и расшифровки цены из КИЗ (кода маркировки 29 символов) табачной продукции.
/// Цена закодирована в символах строки с использованием кастомного алфавита.
/// </summary>
public static class MrcBuilder
{
    // Алфавит, используемый для кодирования цены в КИЗ (64 символа)
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"%&'*+-./_,:;=<>?";
    private const int Base = 64;

    /// <summary>
    /// Извлекает и декодирует цену из строки КИЗ (29 символов) (символы 21-25).
    /// </summary>
    /// <param name="cis">Полный код маркировки (КИЗ) (пачка табачные изделия, размер кода 29 символов).</param>
    /// <returns>Цена в рублях (например, 123.45) или -1 при ошибке.</returns>
    public static double GetMrc(string cis)
    {
        if (cis == null || cis.Length < 29)
            return -1;

        string pricePart = cis.Substring(21, 4); // 4 символа
        double result = 0;

        for (int i = 0; i < pricePart.Length; i++)
        {
            char c = pricePart[i];
            int index = Alphabet.IndexOf(c);

            if (index == -1)
                return -1; // Символ не найден в алфавите

            // Позиционная система счисления: base^(3 - i) * digit
            double value = Math.Pow(Base, 3 - i) * index;
            result += value;
        }

        return result / 100.0; // Переводим копейки в рубли
    }
}