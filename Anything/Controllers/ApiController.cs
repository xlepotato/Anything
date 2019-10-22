using Anything.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Anything.Controllers
{
    public class ApiController
    {
        public static string Key = "0170c6349c6aa647d6342170b3cc3cc9";
        //6372ddbcd9efaf6482ba812a3f9879cf
        //0170c6349c6aa647d6342170b3cc3cc9
        public static Dictionary<string, float> rates;
        public static async System.Threading.Tasks.Task SetRates()
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
        }
        public static async System.Threading.Tasks.Task<MarketRate> GetHistoryRateAsync(DateTime thisDate)
        {

            string apiUrl = " http://data.fixer.io/api/" + thisDate.ToString("yyyy-MM-dd") + "?access_key=" + ApiController.Key + "&base=EUR";

            using (var client = new HttpClient())
            {
                var uri = new Uri(apiUrl);
                var response = client.GetAsync(uri).Result;
                string textResult =  await response.Content.ReadAsStringAsync();
                JavaScriptSerializer j = new JavaScriptSerializer();
                MarketRate marketRate = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
                return marketRate;
            }
        }
        public static object GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        {
            float exchangeFrom = rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
            float exchangeTo = rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
            double amount = CalculationController.ConvertCurrency(ExchangeAmount, exchangeFrom, exchangeTo);
            double baseRate = CalculationController.ConvertCurrency(1, exchangeFrom, exchangeTo);
            var result = new { Amount = amount, Rate = baseRate };
            return result;
        }
    }
}