using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace piotdll.validators;

/// <summary>
/// Основной валидатор ответа от PIOT.
/// Преобразует JSON-ответ в структуру MOut, проверяя статус и детали каждого кода маркировки.
/// </summary>
public class MainValidator : BaseValidator
{
    /// <summary>
    /// Основной метод валидации ответа от сервера PIOT.
    /// </summary>
    /// <param name="json">JSON-ответ от сервера</param>
    /// <param name="mInItems">Входные данные (тестовые кейсы с КИЗ и метаданными)</param>
    /// <returns>MOut — результат проверки</returns>
    public MOut Validate(string json, List<MInItems> mInItems)
    {
        var mOut = new MOut
        {
            ItemsList = new List<MOutItems>(mInItems.Count)
        };

        // Парсинг JSON
        JsonBody_v2 bodyV2;
        try
        {
            bodyV2 = JsonConvert.DeserializeObject<JsonBody_v2>(json);
            mOut.BodyV2 = bodyV2;
        }
        catch (Exception ex)
        {
            mOut.TotalErrorMessage = $"Ошибка парсинга JSON: {ex.Message}";
            return mOut;
        }

        // Проверка общего кода ошибки
        if (bodyV2.Code != null)
        {
            mOut.TotalErrorMessage = $"Неизвестная ошибка при статусе 200. code: {bodyV2.Code} json: {json}";
            return mOut;
        }

        if (bodyV2.CodesResponse == null || bodyV2.CodesResponse.Count == 0)
        {
            mOut.TotalErrorMessage = "Ответ содержит пустой массив codesResponse";
            return mOut;
        }

        var codeBox = bodyV2.CodesResponse[0];

        // Обработка случая проверки в оффлайне
        if (codeBox.IsCheckedOffline == true)
        {
            foreach (var code in codeBox.Codes)
            {
                var mIn = UtilsPiot.GetMInItem(mInItems, code.Cis);
                var outItem = CreateMOutItemFromOffline(code, mIn, codeBox);
                mOut.ItemsList.Add(outItem);
            }
            return mOut;
        }

        // Проверка результата внутри codesResponse
        if (codeBox.Code != 0 || codeBox.Description != "ok")
        {
            mOut.TotalErrorMessage = $"Сервер вернул ошибку: code={codeBox.Code}, description={codeBox.Description}";
            return mOut;
        }

        // Онлайн-режим: обработка через ValidateItem
        foreach (var itemCode in codeBox.Codes)
        {
            var mOutItem = new ValidateItem().Validate(itemCode);
            var mIn = UtilsPiot.GetMInItem(mInItems, mOutItem.Km);

            // Дополнение метаданных из входных данных
            if (mIn != null)
            {
                mOutItem.DescriptionCase = mIn.DescriptionCase;
                mOutItem.IdCase = mIn.IdCase;
            }

            // Формирование тега 1265 (без version и inst, так как не используется в онлайн)
            mOutItem.Tag1265 = string.Format("UUID={0}&Time={1}", codeBox.ReqId, codeBox.ReqTimestamp);

            mOut.ItemsList.Add(mOutItem);
        }

        return mOut;
    }

    /// <summary>
    /// Создаёт объект результата для случая оффлайн-проверки.
    /// </summary>
    private MOutItems CreateMOutItemFromOffline(ItemCode code, MInItems mIn, CodesResponse codeBox)
    {
        var outItem = new MOutItems
        {
            Km = code.Cis,
            PermitSale = !code.IsBlocked,
            ErrorMessage = code.IsBlocked ? "Продажа заблокирована в локальном модуле." : null
        };

        if (mIn != null)
        {
            outItem.DescriptionCase = mIn.DescriptionCase;
            outItem.IdCase = mIn.IdCase;
        }

        outItem.Tag1265 = string.Format(
            "UUID={0}&Time={1}&Inst={2}&Ver={3}",
            codeBox.ReqId, codeBox.ReqTimestamp, codeBox.Inst, codeBox.Version
        );

        return outItem;
    }
}