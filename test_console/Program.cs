using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using piotdll;
using piotdll.Models;

namespace test_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainPoint mainPoint=new MainPoint(new MySettings(),true);
            MOut mOut = mainPoint.CheckCode(new List<string>
            {
                "0104670540176099215'W9Um\u001d93dGVz", 
                "0104670540176099215<pGKy\u001d93DGVz"
            }).Result;
            Console.WriteLine(mOut.LogString);
        }
    }
}
