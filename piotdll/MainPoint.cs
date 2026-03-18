using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using piotdll.Models;

namespace piotdll
{
    public class MainPoint
    {
        public  static MySettings MySettings = null!;
        public static Func<string, double> GetPrice = null!;
        private readonly bool _useTest;

        /// <summary>
        /// Конструктор
        /// Можно держать как синглетон, так и создавать новый каждый раз
        /// </summary>
        /// <param name="mySettings">Ваши настройки проверки</param>
        /// <param name="getPrice">Функция получения цены по gtin в копейках</param>
        /// <param name="useTest">Для тестировании прохождение сертификации включить (true)</param>
        public MainPoint(MySettings mySettings, Func<string, double> getPrice, bool useTest = false)
        {
            MySettings=mySettings;
            GetPrice = getPrice;
            _useTest = useTest;
        }



        /// <summary>
        /// Проверка списка кодов
        /// </summary>
        /// <param name="codes">Список кодов (код полный с разделителем групп</param>
        /// <returns>MOut — результат проверки или описание ошибки, записанное в поле: TotalErrorMessage</returns>
        public async Task<MOut> CheckCode(List<string> codes)
        {
            if(codes == null || codes.Count == 0)
            {
                return new MOut("Список кодов пустой.");
            }
            HashSet<string> set = new HashSet<string>();
            foreach (string code in codes)
            {
                if (set.Contains(code))
                {
                    return new MOut($"Код: {code} повторяется в запросе.");
                }

                set.Add(code);
            }
            var requestHandler = new MainRequestPiot();
            List<MInItems> mInItems;
            if (_useTest)
            {
                mInItems = new FactoryTest().InitFactory().BuildRequest(codes.ToArray());
            }
            else
            {
                mInItems=new List<MInItems>(codes.Count);
                mInItems.AddRange(codes.Select(code => new MInItems { Km = code }));
            }
            

            var builderLog=new StringBuilder();
            var result = new MOut();
             await requestHandler.RequestPiotAsync(mInItems, builderLog, res =>
            {
                res.LogString=builderLog.ToString();
                result = res;
            });
            return result;

        }

    }
}
