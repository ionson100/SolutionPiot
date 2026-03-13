using System;

namespace piotdll;

/// <summary>
/// Интерфейс для асинхронной передачи результата.
/// Аналог <see cref="Action{T}"/>, но с более семантически понятным именем метода.
/// </summary>
/// <typeparam name="T">Тип результата, передаваемого через метод Action</typeparam>
public interface IResult<in T>
{
    /// <summary>
    /// Принимает результат асинхронной операции для дальнейшей обработки.
    /// </summary>
    /// <param name="t">Результат выполнения</param>
    void Action(T t);
}