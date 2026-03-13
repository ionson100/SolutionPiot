using System;
using System.Collections.Generic;
using piotdll.Models;

namespace piotdll;

public class FactoryTest
{
    private readonly Dictionary<string, MInItems> _dictionary = new Dictionary<string, MInItems>();

    public FactoryTest InitFactory()
    {
        // 5.1
        {
            var items = new MInItems
            {
                Km = "0104670540176099215'W9Um\u001D93dGVz",
                IdCase = "5.1",
                DescriptionCase = "Запрет продажи товара при отсутствии в информационной системе мониторинга сведений о его нанесении"
            };
            _dictionary[items.Km] = items;
        }

        // 5.2
        {
            var items = new MInItems
            {
                Km = "0104670540176099215LnOjv\u001D93dGVz",
                IdCase = "5.2",
                DescriptionCase = "Запрет продажи товара при отсутствии в информационной системе мониторинга сведений о его вводе в оборот"
            };
            _dictionary[items.Km] = items;
        }

        // 5.3
        {
            var items = new MInItems
            {
                Km = "010462930887704421DzkcYt2\u001D8005090000\u001D93dGVz",
                IdCase = "5.3",
                DescriptionCase = "Успешная продажа табачной продукции (цена до 100 руб. за пачку для прохождения на МГМ)"
            };
            _dictionary[items.Km] = items;
        }

        // 5.4
        {
            var items = new MInItems
            {
                Km = "010462930887704421DzkcWqS\u001D8005177000\u001D93dGVz",
                IdCase = "5.4",
                DescriptionCase = "Успешная продажа табачной продукции (цена более 135 руб.\nза пачку для соблюдения законодательства РФ), потребительская или\nгрупповая упаковка которых относится к временно не прослеживаемой\n(т.н. «серая зона»)"
            };
            _dictionary[items.Km] = items;
        }

        // 5.5
        {
            var items = new MInItems
            {
                Km = "0104670540176099215NN*cM\u001D93dGVz",
                IdCase = "5.5",
                DescriptionCase = "Запрет продажи товара, который на момент проверки выведен из оборота"
            };
            _dictionary[items.Km] = items;
        }

        // 5.6
        {
            var items = new MInItems
            {
                Km = "0104602220006549215opFcmK\u001D93dGVz",
                IdCase = "5.6",
                DescriptionCase = "Запрет продажи товара, заблокированного или приостановленного для реализации по решению органов власти"
            };
            _dictionary[items.Km] = items;
        }

        // 5.7
        {
            var items = new MInItems
            {
                Km = "0104670540176099215<pGKy\u001D93dGVz",
                IdCase = "5.7",
                DescriptionCase = "Продажа товара с истекшим сроком годности"
            };
            _dictionary[items.Km] = items;
        }

        // 5.8
        {
            var items = new MInItems
            {
                Km = "010461013628057121%798DM%\u001D8005090000\u001D93dGVz",
                IdCase = "5.8",
                DescriptionCase = "Успешная продажа блока сигарет (папирос) по максимальной\nрозничной цене (цена до 100 руб. за пачку для прохождения на МГМ), указанной в коде маркировки"
            };
            _dictionary[items.Km] = items;
        }

        // 5.9
        {
            var items = new MInItems
            {
                Km = "010461013628057121%008iVk\u001D8005180000\u001D93dGVz",
                IdCase = "5.9",
                DescriptionCase = "Успешная продажа блока сигарет (папирос) по максимальной\nрозничной цене (цена более 135 руб. за пачку для соблюдения\nзаконодательства РФ), указанной в коде маркировки"
            };
            _dictionary[items.Km] = items;
        }

        // 5.10
        {
            var items = new MInItems
            {
                Km = "04601653035829H;dV)bFABVUdGVz",
                IdCase = "5.10",
                DescriptionCase = "Продажа пачки сигарет (папирос) по максимальной\nрозничной цене (цена до 100 руб. за пачку для прохождения на МГМ), указанной в коде маркировки"
            };
            _dictionary[items.Km] = items;
        }

        // 5.11
        {
            var items = new MInItems
            {
                Km = "04601653035829H;dV)bFACVUdGVz",
                IdCase = "5.11",
                DescriptionCase = "Продажа пачки сигарет (папирос) по максимальной\nрозничной цене (цена более 135 руб. за пачку для соблюдения законодательства РФ), указанной в коде маркировки"
            };
            _dictionary[items.Km] = items;
        }

        // 5.12
        {
            var items = new MInItems
            {
                Km = "04601653035829H;vE)bFABBUdGVz",
                IdCase = "5.12",
                DescriptionCase = "Продажа товара, сведения о маркировке средствами\nидентификации которого отсутствуют в информационной системе мониторинга"
            };
            _dictionary[items.Km] = items;
        }

        // 5.14
        {
            var items = new MInItems
            {
                Km = "0104670540176099215<pGKy\u001D93DGVz",
                IdCase = "5.14",
                DescriptionCase = "Запрет продажи товара с некорректным кодом проверки"
            };
            _dictionary[items.Km] = items;
        }

        // 5.17
        {
            var items = new MInItems
            {
                Km = "0104607010350246215kRdG-1%2(UmV\u001D93dGVz",
                IdCase = "5.17",
                DescriptionCase = "Продажа товара в режиме офлайн, отсутствующего в\nчерном списке локального модуля."
            };
            _dictionary[items.Km] = items;
        }

        // 5.18
        {
            var items = new MInItems
            {
                Km = "0104607010350246215kRdG-1%W(Umn\u001D93dGVz",
                IdCase = "5.18",
                DescriptionCase = "Продажа товара в режиме проверки офлайн,\nотсутствующего в черном списке ЛМ ЧЗ (ответ от ГИС МТ 5 секунд)"
            };
            _dictionary[items.Km] = items;
        }

        // 5.19
        {
            var items = new MInItems
            {
                Km = "0104602220006549215opRcmR\u001D93dGVz",
                IdCase = "5.19",
                DescriptionCase = "Запрет продажи товара в режиме проверки офлайн, присутствующего в черном списке ЛМ ЧЗ"
            };
            _dictionary[items.Km] = items;
        }

        // 5.20
        {
            var items = new MInItems
            {
                Km = "0104607010350246215kRdG-X%W(Rnb\u001D93dGVz",
                IdCase = "5.20",
                DescriptionCase = "Запрет продажи товара в режиме проверки офлайн, присутствующего в черном списке ЛМ ЧЗ (ответ от ГИС МТ 5 секу"
            };
            _dictionary[items.Km] = items;
        }

        // 5.21
        {
            var items = new MInItems
            {
                Km = "0104670540176099215LpGKy\u001D93dGVz",
                IdCase = "5.21",
                DescriptionCase = "Сканирование кода маркировки, который возвращает 203 - ошибку и переводит ТС ПИоТ в аварийный режим"
            };
            _dictionary[items.Km] = items;
        }

        // 5.22
        {
            var items = new MInItems
            {
                Km = "01046700190342882151aj\"K>X+mFcP\u001D93dGVz",
                IdCase = "5.22",
                DescriptionCase = "Запрет продажи товара, когда РД признан прекращенным или недействительным \nпо решению государственного органа контроля (надзора) за соблюдением требований технических регламентов"
            };
            _dictionary[items.Km] = items;
        }

        // 5.23
        {
            var items = new MInItems
            {
                Km = "0108607405401894215cC3O4\u001D93dGVz",
                IdCase = "5.23",
                DescriptionCase = "Запрет продажи товара при аннулированном ВСД"
            };
            _dictionary[items.Km] = items;
        }

        // 5.24
        {
            var items = new MInItems
            {
                Km = "04601653035829H;dV)bFADI8dGVz",
                IdCase = "5.24",
                DescriptionCase = "Запрет продажи пачки сигарет (папирос) по минимальной розничной цене"
            };
            _dictionary[items.Km] = items;
        }

        // 5.25
        {
            var items = new MInItems
            {
                Km = "010461013628057121/798DM%\u001D8005199000\u001D93dGVz",
                IdCase = "5.25",
                DescriptionCase = "Запрет продажи блока сигарет (папирос) по минимальной розничной цене"
            };
            _dictionary[items.Km] = items;
        }

        // 5.26
        {
            var items = new MInItems
            {
                Km = "00840147505712Zz;ZnRbAAAAdGVz",
                IdCase = "5.26",
                DescriptionCase = "Запрет продажи НСП по минимальной розничной цене"
            };
            _dictionary[items.Km] = items;
        }

      //  // 00
      //  {
      //      var items = new MInItems
      //      {
      //          Km = "0104670540176099215'W9Um\u001D93dGVz"
      //          // IdCase и DescriptionCase не заданы (останутся null)
      //      };
      //      _dictionary[items.Km] = items; ;
      //  }

      //  // 5000
      //  {
      //      var items = new MInItems
      //      {
      //          Km = "0104670540176099215'W9Um\u001D93dGVz",
      //          IdCase = "5000",
      //          DescriptionCase = "Дополнительная марка для тестирования 5000 ошибки."
      //      };
      //      _dictionary[items.Km] = items;
      //  }

      // // 514
      // {
      //     var items = new MInItems
      //     {
      //         Km = "0104670540176099215'W9Um\u001D93dGVz",
      //         IdCase = "514",
      //         DescriptionCase = "Таймаут соединения. Не удалось получить ответ от ГИС МТ или ЛМ ЧЗ за 1800мс."
      //     };
      //     _dictionary[items.Km] = items;
      // }
      //
      // // 504
      // {
      //     var items = new MInItems
      //     {
      //         Km = "0104670540176099215!pGKy\u001D93dGVz",
      //         IdCase = "504",
      //         DescriptionCase = "Gateway Timeout - превышено время ожидания ответа от upstream сервиса."
      //     };
      //     _dictionary[items.Km] = items;
      // }

        return this;
    }

    public List<MInItems> BuildRequest(params string[] codes)
    {
        var result = new List<MInItems>(codes.Length);
        foreach (var key in codes)
        {
            result.Add(_dictionary.TryGetValue(key, out var item) ? item : new MInItems { Km = key });
        }
        return result;
    }
}