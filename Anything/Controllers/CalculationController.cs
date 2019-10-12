using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Controllers
{
    public class CalculationController
    {
        public static string CalculateDate(DateTime date)
        {
            var timeDiff = DateTime.Now - date;
            if (timeDiff.Days != 0)
            {
                return timeDiff.Days + " days ago";
            }
            else if (timeDiff.Hours != 0)
            {
                return timeDiff.Hours + " hours ago";
            }
            else if (timeDiff.Minutes != 0)
            {
                return timeDiff.Minutes + " minutes ago";
            }
            else if (timeDiff.Seconds != 0)
            {
                return timeDiff.Seconds + " seconds ago";
            }
            return "timespan error";
        }
        public static double ConvertCurrency(double ExchangeAmount, double ExchangeFrom, double ExchangeTo)
        {
            double amount = (ExchangeAmount / ExchangeFrom) * ExchangeTo;
            return amount;
        }
    }
}