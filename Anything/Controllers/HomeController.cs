﻿using Anything.Models;
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
            historicalRates.Title = ExchangeFrom + " To " + ExchangeTo;
            historicalRates.ShortDate = new List<string>();
            historicalRates.Amount = new List<double>();
            DateTime dateNow = DateTime.Now;
            DateTime historicalDate = dateNow.AddDays(-numOfDays);
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var exchangeFromCurrId = model.Currencies.Where(x => x.Name == ExchangeFrom).FirstOrDefault().Id;
                var exchangeToCurrId = model.Currencies.Where(x => x.Name == ExchangeTo).FirstOrDefault().Id;
                var storedHistoricalRates = model.HistoricalRates.Where(z => z.ExchangeFromId == exchangeFromCurrId
                                                    && z.ExchangeToId == exchangeToCurrId);
                DateTime latestStoredHistoricalRateDate = new DateTime();
                if (storedHistoricalRates != null && storedHistoricalRates.FirstOrDefault() !=null)
                {               
                    latestStoredHistoricalRateDate = storedHistoricalRates.OrderByDescending(z => z.Date).FirstOrDefault().Date;
                }
                if (!(storedHistoricalRates.FirstOrDefault() == null ||
                     latestStoredHistoricalRateDate < dateNow.AddDays(-numOfDays)))
                {                  
                    TimeSpan dateDiff = dateNow.Subtract(latestStoredHistoricalRateDate);
                    numOfDays = dateDiff.Days;
                    historicalDate = dateNow.AddDays(-numOfDays);
                    if(historicalDate.ToString("yyyy-MM-dd") == latestStoredHistoricalRateDate.ToString("yyyy-MM-dd"))
                    {
                        numOfDays = 0;
                    }
                }
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
                        if (a.success == true)
                        {
                            double exchangeFrom = a.rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
                            double exchangeTo = a.rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;

                            HistoricalRate newHistoricalRate = new HistoricalRate();
                            newHistoricalRate.ExchangeFromId = exchangeFromCurrId;
                            newHistoricalRate.ExchangeToId = exchangeToCurrId;
                            newHistoricalRate.Rate = ConvertCurrency(1, exchangeFrom, exchangeTo);
                            newHistoricalRate.Date = thisDate;

                            model.HistoricalRates.Add(newHistoricalRate);
                        }

                    }
                }
                model.SaveChanges();
                foreach(var x in model.HistoricalRates)
                {
                    historicalRates.ShortDate.Add(x.Date.ToString("dd MMM"));
                    historicalRates.Amount.Add(x.Rate);
                }
            }
            return Json(historicalRates, JsonRequestBehavior.AllowGet);
        }
        private double ConvertCurrency(double ExchangeAmount, double ExchangeFrom, double ExchangeTo)
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
  
