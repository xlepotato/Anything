using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Controllers
{
    public class FilterController
    {
        public static object Filter(string Search, string ExchangeFrom, string ExchangeTo, string SortBy, bool IsFavourite)
        {
            string username = HttpContext.Current.Session["Username"]== null? "" : HttpContext.Current.Session["Username"].ToString();
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var moneyChangers = model.ExchangeRates.Where(z => z.ExchangeFromId == model.Currencies.Where(y => y.Name == ExchangeFrom).FirstOrDefault().Id
                                                && z.ExchangeToId == model.Currencies.Where(y => y.Name == ExchangeTo).FirstOrDefault().Id);
                var exchangeRates = moneyChangers.Where(z => z.MoneyChanger.Name.Contains(Search) || z.MoneyChanger.Location.Contains(Search));
                if(IsFavourite)
                {
                    exchangeRates = exchangeRates.Where(z => z.MoneyChanger.Favourites.Where(a => a.Username == username).FirstOrDefault() != null);
                }
                if(SortBy == "Best")
                {
                    exchangeRates = exchangeRates.OrderByDescending(z => z.Rate);
                }
                else if(SortBy == "Lowest")
                {
                    exchangeRates = exchangeRates.OrderBy(z => z.Rate);
                }
                var userFavourites = model.Favourites.Where(a => a.Username == username);
                var x = exchangeRates.AsEnumerable().Select(z => new
                {
                    z.Rate,
                    z.MoneyChanger.Name,
                    z.MoneyChanger.Location,
                    HasFavourite = userFavourites.AsEnumerable().Where(a=> a.MoneyChanger == z.MoneyChanger).FirstOrDefault() != null?true:false,
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