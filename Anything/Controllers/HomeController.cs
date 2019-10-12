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
        public ActionResult GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        {
            float exchangeFrom = rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
            float exchangeTo = rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
            double amount = CalculationController.ConvertCurrency(ExchangeAmount, exchangeFrom, exchangeTo);
            double baseRate = CalculationController.ConvertCurrency(1, exchangeFrom, exchangeTo);
            var result = new { Amount = amount, Rate = baseRate };
            return Json(result
           , JsonRequestBehavior.AllowGet);

        }

        public ActionResult Filter(string Search,string ExchangeFrom, string ExchangeTo)
        {
            return Json(FilterController.Filter(Search,ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);          
        }
       
        public ActionResult GetGraph(string ExchangeFrom, string ExchangeTo)
        {
            return Json(GraphController.GetGraph(ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);
        }
        
        
    }
}
  
