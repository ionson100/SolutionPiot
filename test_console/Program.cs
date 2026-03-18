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
            MySettings mySettings = new MySettings();
            mySettings.UrlPiot= "https://localhost:51401/api/v2/codes/check";
            MainPoint mainPoint=new MainPoint(mySettings, GetPrice,true);
            MOut mOut = mainPoint.CheckCode(new List<string>
            {
                "0104670540176099215'W9Um\u001d93dGVz", 
               
            }).Result;
            Console.WriteLine(mOut.LogString);
            Console.ReadLine();
        }
    }
}
