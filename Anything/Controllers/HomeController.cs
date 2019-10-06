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
            if (ExchangeFrom != ExchangeTo)
            {
                int numOfDays = 30;
                HistoricalRates historicalRates = new HistoricalRates();
                historicalRates.Title = ExchangeFrom + " To " + ExchangeTo;
                historicalRates.ShortDate = new List<string>();
                historicalRates.Amount = new List<double>();
                historicalRates.RegressionY = new List<double>();
                DateTime dateNow = DateTime.Now.AddDays(-1);
                DateTime historicalDate = dateNow.AddDays(-numOfDays);
                IQueryable<HistoricalRate> storedHistoricalRates;
                using (cz2006anythingEntities model = new cz2006anythingEntities())
                {
                    model.HistoricalRates.RemoveRange(model.HistoricalRates.Where(z => z.Date < historicalDate));
                    var exchangeFromCurrId = model.Currencies.Where(x => x.Name == ExchangeFrom).FirstOrDefault().Id;
                    var exchangeToCurrId = model.Currencies.Where(x => x.Name == ExchangeTo).FirstOrDefault().Id;
                    storedHistoricalRates = model.HistoricalRates.Where(z => z.ExchangeFromId == exchangeFromCurrId
                                                        && z.ExchangeToId == exchangeToCurrId);
                    DateTime latestStoredHistoricalRateDate = new DateTime();
                    if (storedHistoricalRates != null && storedHistoricalRates.FirstOrDefault() != null)
                    {
                        latestStoredHistoricalRateDate = storedHistoricalRates.OrderByDescending(z => z.Date).FirstOrDefault().Date;
                    }
                    if (!(storedHistoricalRates.FirstOrDefault() == null ||
                         latestStoredHistoricalRateDate < dateNow.AddDays(-numOfDays)))
                    {
                        TimeSpan dateDiff = dateNow.Subtract(latestStoredHistoricalRateDate);
                        numOfDays = dateDiff.Days;
                        historicalDate = dateNow.AddDays(-numOfDays);
                    }
                    for (int i = 1; i <= numOfDays; i++)
                    {
                        DateTime thisDate = historicalDate.AddDays(i);
                        string apiUrl = " http://data.fixer.io/api/" + thisDate.ToString("yyyy-MM-dd") + "?access_key=" + Key + "&base=EUR";

                        using (var client = new HttpClient())
                        {
                            var uri = new Uri(apiUrl);
                            var response = await client.GetAsync(uri);
                            string textResult = await response.Content.ReadAsStringAsync();
                            JavaScriptSerializer j = new JavaScriptSerializer();
                            MarketRate marketRate = (MarketRate)j.Deserialize(textResult, typeof(MarketRate));
                            if (marketRate.success == true)
                            {
                                double exchangeFrom = marketRate.rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
                                double exchangeTo = marketRate.rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;

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

                    float exchangeFromToday = rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
                    float exchangeToToday = rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
                    double baseRate = ConvertCurrency(1, exchangeFromToday, exchangeToToday);

                    List<int> RegressionX = new List<int>();
                    int n = storedHistoricalRates.Count() + 1;
                    for (int i = 0; i < n; i++)
                    {
                        RegressionX.Add(i);
                    }

                    int sumX = RegressionX.Sum();
                    double sumY = 0;


                    List<double> xY = new List<double>();
                    List<double> squareX = new List<double>();
                    for (int i = 0; i < n - 1; i++)
                    {
                        squareX.Add(RegressionX[i] * RegressionX[i]);
                        double y = storedHistoricalRates.OrderByDescending(z => z.Date).Skip(n - i - 2).FirstOrDefault().Rate;
                        sumY += y;
                        xY.Add(RegressionX[i] * y);
                    }
                    squareX.Add(RegressionX[n - 1] * RegressionX[n - 1]);
                    sumY += baseRate;
                    xY.Add(RegressionX[n - 1] * baseRate);

                    double b = (n * xY.Sum() - sumX * sumY) / (n * squareX.Sum() - sumX * sumX);
                    double a = (sumY / n) - b * (sumX / n);
                    int counter = 0;
                    foreach (var x in storedHistoricalRates)
                    {
                        historicalRates.ShortDate.Add(x.Date.ToString("dd MMM"));
                        historicalRates.Amount.Add(x.Rate);
                        historicalRates.RegressionY.Add(a + b * (counter));
                        counter++;
                    }

                    historicalRates.ShortDate.Add(DateTime.Now.ToString("dd MMM"));
                    historicalRates.Amount.Add(baseRate);
                    historicalRates.RegressionY.Add(a + b * (counter));

                }
                return Json(historicalRates, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
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
  
