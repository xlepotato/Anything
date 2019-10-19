using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anything.Controllers
{
    public class FilterController
    {
        public static object Filter(string Search, string ExchangeFrom, string ExchangeTo, string SortBy)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var moneyChangers = model.ExchangeRates.Where(z => z.ExchangeFromId == model.Currencies.Where(y => y.Name == ExchangeFrom).FirstOrDefault().Id
                                                && z.ExchangeToId == model.Currencies.Where(y => y.Name == ExchangeTo).FirstOrDefault().Id);
                var exchangeRates = moneyChangers.Where(z => z.MoneyChanger.Name.Contains(Search) || z.MoneyChanger.Location.Contains(Search));
                if(SortBy == "Best")
                {
                    exchangeRates = exchangeRates.OrderByDescending(z => z.Rate);
                }
                else if(SortBy == "Lowest")
                {
                    exchangeRates = exchangeRates.OrderBy(z => z.Rate);
                }                                               
                var x = exchangeRates.AsEnumerable().Select(z => new
                {
                    z.Rate,
                    z.MoneyChanger.Name,
                    z.MoneyChanger.Location,
                    LastUpdated = CalculationController.CalculateDate(z.LastUpdated),
                }).ToList();
                var zxc = new
                {
                    ExchangeRates = x,
                    MoneyChangers = moneyChangers.Count()
                };
                return zxc;
              
            }

        }
    }
}