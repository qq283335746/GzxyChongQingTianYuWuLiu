using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TygaSoft.Model;
using TygaSoft.ThreadProcessor;
using System.Globalization;
using TygaSoft.BLL;

namespace TygaSoft.CA_Processor
{
    class Program
    {
        static DayStatInfo dsInfo;

        static void Main(string[] args)
        {
            //TyUserProcessor.Processor();

            SmsProcessor.Processor();

            //double aaa = dsInfo.early;
            //if (aaa == 0.0)
            //{
            //    aaa = 0;
            //}

            //float bb = (float)aaa;

            //// Gets a NumberFormatInfo associated with the en-US culture.
            //NumberFormatInfo nfi = new CultureInfo("zh-CN", false).NumberFormat;

            //// Displays a negative value with the default number of decimal digits (2).
            //float aa = 28.656599f;
            ////float bb = float.Parse(aa.ToString("N1"));

            //nfi.PercentSymbol = "%";
            //Console.WriteLine(aa.ToString("F", nfi));

            //Console.WriteLine("aa");

            //RunFirstGameProcessor.Processor();

            //EmailProcessor.Processor();

            //List<UserBetLotteryInfo> list = new List<UserBetLotteryInfo>
            //{
            //    new UserBetLotteryInfo{UserId = "0001",BetValue = "012",BetPrice = 1,WinPrice = 45},
            //    new UserBetLotteryInfo{UserId = "0002",BetValue = "013",BetPrice = 2,WinPrice = 90},
            //    new UserBetLotteryInfo{UserId = "0002",BetValue = "01x2",BetPrice = 3,WinPrice = 2550},
            //    new UserBetLotteryInfo{UserId = "0003",BetValue = "0124",BetPrice = 3,WinPrice = 2550},
            //    new UserBetLotteryInfo{UserId = "0001",BetValue = "0124",BetPrice = 3,WinPrice = 2550},
            //    new UserBetLotteryInfo{UserId = "0001",BetValue = "0x13",BetPrice = 5,WinPrice = 4250}
            //};

            //string sTicketNum = "0124";

            //int payTotalUser = 0;
            //decimal payTotalPrice = 0;

            //var currUblList = list.Where(m => sTicketNum.Contains(m.BetValue) && !m.BetValue.ToUpper().Contains("X"));
            //payTotalUser = currUblList.GroupBy(m => m.UserId).Count();
            //payTotalPrice = currUblList.Sum(m => m.WinPrice);

            //string a = "0012";
            //string b = "001X";
            //bool isc = a.Contains(b);
            //return;

            //int a = (int)DateTime.Now.DayOfWeek;
            //string d1 = string.Format("{0:dddd}", DateTime.Now);
            //string d2 = DateTime.Now.ToString("dddd");
            //string d3 = string.Format("{0:ddd}", DateTime.Now);
            //string d4 = string.Format("{0:d}", DateTime.Now);
            //string d5 = DateTime.Now.ToString("d");
            //var names = Enum.GetNames(typeof(DayOfWeek));
            //var values = Enum.GetValues(typeof(DayOfWeek));
            //DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            Console.WriteLine("按任意键结束");
            Console.ReadLine();
        }
    }
}
