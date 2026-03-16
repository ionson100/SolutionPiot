namespace piotdll.Models;

/// <summary>
/// Единица входных данных для проверки кода, применима к тестовому сопровождению
/// </summary>
public class MInItems
{
    /// <summary>
    /// Полный код, с групповыми разделителями
    /// </summary>
    public string Km { get; set; } = null!;

    /// <summary>
    /// Номер проверки кода, по спецификации (https://tspiot.sandbox.crptech.ru/)
    /// Эти поля можно не заполнять в реальных условиях или вообще исключить в проде
    /// </summary>
    public string? IdCase { get; set; }

    /// <summary>
    /// Описание кейса проверки кода по спецификации
    /// Эти поля можно не заполнять в реальных условиях или вообще исключить в проде
    /// </summary>
    public string? DescriptionCase { get; set; }
}