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
            string apiUrl = "http://data.fixer.io/api/latest?access_key=e4e42612bf86976d190d8d29b20fcc32&base=eur";

            using (var client = new HttpClient())
            {
                var uri = new Uri(apiUrl);

                var response = await client.GetAsync(uri);

                string textResult = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer j = new JavaScriptSerializer();
                MarketRate a = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
                rates = a.rates;
                var currencies = a.rates.Keys.ToList();
                return View(currencies);
            }
        }
        [HttpGet]
        public ActionResult GetCurrency(float ExchangeAmount,string ExchangeFrom, string ExchangeTo)
        {
                float amount = (ExchangeAmount/rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value) * rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
                var result = new { Amount = amount , Rate = (1 / rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value) * rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value};
                return Json(result
               , JsonRequestBehavior.AllowGet);
            
        }

       
        }
    }
  
