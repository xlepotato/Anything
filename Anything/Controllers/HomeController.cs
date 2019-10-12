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
        public static string Key = "8bd5338c9b5589de70fe9aa036b0167f";
        // GET: Home

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            string apiUrl = "http://data.fixer.io/api/latest?access_key=" + Key + "&base=eur";

            using (var client = new HttpClient())
            {
                var uri = new Uri(apiUrl);

                var response = await client.GetAsync(uri);

                string textResult = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer j = new JavaScriptSerializer();
                MarketRate a = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
                rates = a.rates;
            }
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                return View(model.Currencies.ToList());
            }
        }
        [HttpGet]
        public ActionResult GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        {
            float exchangeFrom = rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
            float exchangeTo = rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
            double amount = ConvertCurrency(ExchangeAmount, exchangeFrom, exchangeTo);
            double baseRate = ConvertCurrency(1, exchangeFrom, exchangeTo);
            var result = new { Amount = amount, Rate = baseRate };
            return Json(result
           , JsonRequestBehavior.AllowGet);

        }

        public ActionResult Filter(string Search,string ExchangeFrom, string ExchangeTo)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var exchangeRates = model.ExchangeRates.Where(z => z.ExchangeFromId == model.Currencies.Where(y => y.Name == ExchangeFrom).FirstOrDefault().Id
                                                 && z.ExchangeToId == model.Currencies.Where(y => y.Name == ExchangeTo).FirstOrDefault().Id)
                                                 .Where(z => z.MoneyChanger.Name.Contains(Search) || z.MoneyChanger.Location.Contains(Search))
                                                 .OrderByDescending(z=>z.Rate);


                return Json(
                    exchangeRates.AsEnumerable().Select(z => new {
                        z.Rate,
                        z.MoneyChanger.Name,
                        z.MoneyChanger.Location,
                        LastUpdated = CalculateDate(z.LastUpdated)
                    }).ToList()
                    , JsonRequestBehavior.AllowGet);
            }
               
        }
        public string CalculateDate(DateTime date)
        {
            var timeDiff = DateTime.Now - date;
            if(timeDiff.Days!=0)
            {
                return timeDiff.Days+" days ago";
            }
            else if (timeDiff.Hours!=0)
            {
                return timeDiff.Hours + " hours ago";
            }
            else if(timeDiff.Minutes!=0)
            {
                return timeDiff.Minutes + " minutes ago";
            }
            else if(timeDiff.Seconds!=0)
            {
                return timeDiff.Seconds + " seconds ago";
            }
            return "timespan error";
        }
        public async System.Threading.Tasks.Task<ActionResult> GetGraph(string ExchangeFrom, string ExchangeTo)
        {
            return Json(GraphController.GetGraph(ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);
        }
        public static double ConvertCurrency(double ExchangeAmount, double ExchangeFrom, double ExchangeTo)
        {
            double amount = (ExchangeAmount / ExchangeFrom) * ExchangeTo;
            return amount;
        }
        [Route("{MoneyChangerName}")]
        public ActionResult Details(string MoneyChangerName)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var MoneyChanger = model.MoneyChangers.Where(z => z.Name.Contains(MoneyChangerName)).OrderBy(z=>z.Name.Length).FirstOrDefault();
                if(MoneyChanger!=null)
                {
                    var x = MoneyChanger.ExchangeRates;
                }
                return View(MoneyChanger);
            }         
        }
    }
}
  
