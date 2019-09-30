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
        public static string Key = "88e5742380260e37cd085046a10c3e68";
        // GET: Home

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            //string apiUrl = "http://data.fixer.io/api/latest?access_key="+Key+"&base=eur";

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
        //public ActionResult GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        //{
        //    float exchangeFrom = rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
        //    float exchangeTo = rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
        //    float amount = ConvertCurrency(ExchangeAmount, exchangeFrom, exchangeTo );
        //    float baseRate = ConvertCurrency(1, exchangeFrom, exchangeTo);
        //    var result = new { Amount = amount, Rate = baseRate};
        //    return Json(result
        //   , JsonRequestBehavior.AllowGet);

        //}

        public ActionResult Filter(string Search,string ExchangeFrom, string ExchangeTo)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var exchangeRates = model.ExchangeRates.Where(z => z.ExchangeFromId == model.Currencies.Where(y => y.Name == ExchangeFrom).FirstOrDefault().Id
                                                 && z.ExchangeToId == model.Currencies.Where(y => y.Name == ExchangeTo).FirstOrDefault().Id)
                                                 .Where(z => z.MoneyChanger.Name.Contains(Search) || z.MoneyChanger.Location.Contains(Search))
                                                 .OrderByDescending(z=>z.Rate);

                return Json(
                    exchangeRates.Select(z => new {
                        z.Rate,
                        z.MoneyChanger.Name,
                        z.MoneyChanger.Location
                    }).ToList()
                    , JsonRequestBehavior.AllowGet);
            }
               
        }
        public async System.Threading.Tasks.Task<ActionResult> GetGraph(string ExchangeFrom, string ExchangeTo)
        {
            ExchangeFrom = "SGD";
            ExchangeTo = "MYR";
            int numOfDays = 30;
            HistoricalRates historicalRates = new HistoricalRates();
            historicalRates.ShortDate = new List<string>();
            historicalRates.Amount = new List<float>();
            DateTime historicalDate = DateTime.Now.AddDays(-numOfDays+1);
            for (int i = 0; i < numOfDays; i++)
            {
                DateTime thisDate = historicalDate.AddDays(i);
                string apiUrl = " http://data.fixer.io/api/" + thisDate.ToString("yyyy-MM-dd") + "?access_key=" + Key + "&base=EUR";

                using (var client = new HttpClient())
                {
                    var uri = new Uri(apiUrl);
                    var response = await client.GetAsync(uri);
                    string textResult = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer j = new JavaScriptSerializer();
                    MarketRate a = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
                    float exchangeFrom = a.rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
                    float exchangeTo = a.rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
               
                    
                    historicalRates.ShortDate.Add(thisDate.ToString("dd MMM"));
                    historicalRates.Amount.Add(ConvertCurrency(1, exchangeFrom, exchangeTo));
        
                }
            }
            return Json(historicalRates, JsonRequestBehavior.AllowGet);
        }
        private float ConvertCurrency(float ExchangeAmount, float ExchangeFrom, float ExchangeTo)
        {
            float amount = (ExchangeAmount / ExchangeFrom) * ExchangeTo;
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
  
