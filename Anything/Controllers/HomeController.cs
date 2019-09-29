using Anything.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Anything.Controllers
{
    public class HomeController : Controller
    {
        public static Dictionary<string, float> rates;
        // GET: Home
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            //string apiUrl = "http://data.fixer.io/api/latest?access_key=88e5742380260e37cd085046a10c3e68&base=eur";

            //using (var client = new HttpClient())
            //{
            //    var uri = new Uri(apiUrl);

            //    var response = await client.GetAsync(uri);

            //    string textResult = await response.Content.ReadAsStringAsync();
            //    JavaScriptSerializer j = new JavaScriptSerializer();
            //    MarketRate a = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
            //    rates = a.rates;      
            //}
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                return View(model.Currencies.ToList());
            }
        }
        //[HttpGet]
        //public ActionResult GetCurrency(float ExchangeAmount,string ExchangeFrom, string ExchangeTo)
        //{
        //        float amount = (ExchangeAmount/rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value) * rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
        //        var result = new { Amount = amount , Rate = (1 / rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value) * rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value};
        //        return Json(result
        //       , JsonRequestBehavior.AllowGet);

        //}
        public ActionResult Filter(string Search,string ExchangeFrom, string ExchangeTo)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var exchangeRates = model.ExchangeRates.Where(z => z.ExchangeFromId == model.Currencies.Where(y => y.Name == ExchangeFrom).FirstOrDefault().Id
                                                 && z.ExchangeToId == model.Currencies.Where(y => y.Name == ExchangeTo).FirstOrDefault().Id)
                                                 .Where(z => z.MoneyChanger.Name.Contains(Search) || z.MoneyChanger.Location.Contains(Search))
                                                 .OrderBy(z=>z.Rate);

                return Json(
                    exchangeRates.Select(z => new {
                        z.Rate,
                        z.MoneyChanger.Name,
                        z.MoneyChanger.Location
                    }).ToList()
                    , JsonRequestBehavior.AllowGet);
            }
               
        }

    }
}
  
