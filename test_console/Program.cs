using System;
using System.Collections.Generic;
using piotdll;
using piotdll.Models;

namespace test_console
{
    internal class Program
    {
        static double  GetPrice(string gtin)
        {
            return 150.0 * 100; //TODO: Заменить на реальное получение цены из БД или справочника
        }
        static void Main(string[] args)
        {
            MainPoint mainPoint=new MainPoint(new MySettings(), GetPrice,true);
            MOut mOut = mainPoint.CheckCode(new List<string>
            {
                "0104670540176099215'W9Um\u001d93dGVz", 
                "0104670540176099215<pGKy\u001d93DGVz"
            }).Result;
            Console.WriteLine(mOut.LogString);
            Console.ReadLine();
        }
    }
}
