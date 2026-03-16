namespace piot_123.Models
{
    /// <summary>
    /// Элемент кода проверки
    /// </summary>
    public class CodeUnit
    {
        /// <summary>
        /// Код маркировки полный, с групповыми разделителями
        /// </summary>
        public string Km { get; set; }=null!;

        //Цена за единицу продукта для группы 3, 15, 16
        public int PriceTobaccoGroup { get; set; }
    }
}
