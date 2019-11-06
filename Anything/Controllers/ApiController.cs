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
        //6fe677e438e04557167fe3dde5f986b9
        //5a60f47c79eb8b98e65baff7df338657
        //df8d8bc6dacb1da60fae09d862320c02
        //8485c16a5044ee39dc818d00b8f4b67e
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
        public static void UpdateMoneyChanger(string Name, string Address, string Img, string OpeningHours, int? Tel_No)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var thisMC = model.MoneyChangers.Where(z => z.Name == Name).FirstOrDefault();
                if (thisMC == null)
                {
                    thisMC = new MoneyChanger();
                    var arr = Address.Split(' ');
                    thisMC.Name = Name;
                    if (arr.Count() > 0)
                    {
                        string PostalCode = arr[arr.Count() - 1];
                        thisMC.Location = Address.Replace(", " + PostalCode, "");
                        thisMC.PostalCode = PostalCode;
                    }
                    thisMC.OpeningHours = OpeningHours;
                    thisMC.Photo = Img;
                    thisMC.ContactNumber = Tel_No;
                    model.MoneyChangers.Add(thisMC);
                }
                else
                {
                    var arr = Address.Split(' ');
                    if (arr.Count() > 0)
                    {
                        string PostalCode = arr[arr.Count() - 1];
                        thisMC.Location = Address.Replace(", " + PostalCode, "");
                        thisMC.PostalCode = PostalCode;
                    }
                    thisMC.OpeningHours = OpeningHours;
                    thisMC.Photo = Img;
                    thisMC.ContactNumber = Tel_No;
                }
                model.SaveChanges();
            }
        }
        public static void UpdateExchangeRates(string moneychanger_name, string currency_code, float? exchange_rate_buy, float? exchange_rate_sell, string last_update_buy, string last_update_sell)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var selling = model.ExchangeRates.Where(z => z.MoneyChanger.Name == moneychanger_name && z.Currency.Name == currency_code && z.Currency1.Name == "SGD").FirstOrDefault();
                if (exchange_rate_sell != null && exchange_rate_sell != 0)
                {                   
                    if (selling == null)
                    {
                        selling = new ExchangeRate();
                        selling.MoneyChanger = model.MoneyChangers.Where(z => z.Name == moneychanger_name).FirstOrDefault();
                        selling.Currency = model.Currencies.Where(z => z.Name == currency_code).FirstOrDefault();
                        selling.Currency1 = model.Currencies.Where(z => z.Name == "SGD").FirstOrDefault();
                        selling.Rate = (float)exchange_rate_sell;
                        selling.LastUpdated = CalculationController.SetDate(last_update_sell);
                        model.ExchangeRates.Add(selling);
                    }
                    else
                    {
                        selling.Rate = (float)exchange_rate_sell;
                        selling.LastUpdated = CalculationController.SetDate(last_update_sell);
                    }

                }
                else
                {
                    if (selling != null)
                    {
                        model.ExchangeRates.Remove(selling);
                    }
                }
                var buying = model.ExchangeRates.Where(z => z.MoneyChanger.Name == moneychanger_name && z.Currency1.Name == currency_code && z.Currency.Name == "SGD").FirstOrDefault();
                if (exchange_rate_buy != null && exchange_rate_buy != 0)
                {
                    exchange_rate_buy = 1 / exchange_rate_buy;                   
                    if (buying == null)
                    {
                        buying = new ExchangeRate();
                        buying.MoneyChanger = model.MoneyChangers.Where(z => z.Name == moneychanger_name).FirstOrDefault();
                        buying.Currency1 = model.Currencies.Where(z => z.Name == currency_code).FirstOrDefault();
                        buying.Currency = model.Currencies.Where(z => z.Name == "SGD").FirstOrDefault();
                        buying.Rate = (float)exchange_rate_buy;
                        buying.LastUpdated = CalculationController.SetDate(last_update_buy);
                        model.ExchangeRates.Add(buying);
                    }
                    else
                    {
                        buying.Rate = (float)exchange_rate_buy;
                        buying.LastUpdated = CalculationController.SetDate(last_update_buy);
                    }
                }
                else
                {
                    if (buying != null)
                    {
                        model.ExchangeRates.Remove(buying);
                    }
                }
                model.SaveChanges();
            }
        }
    }
}