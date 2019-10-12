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
    public  class GraphController
    {
        public static async System.Threading.Tasks.Task<HomeHistoricalRates> GetGraph(string ExchangeFrom, string ExchangeTo)
        {
            HomeHistoricalRates historicalRates = new HomeHistoricalRates();
            if (ExchangeFrom != ExchangeTo)
            {
                int numOfDays = 30;             
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
                        string apiUrl = " http://data.fixer.io/api/" + thisDate.ToString("yyyy-MM-dd") + "?access_key=" + HomeController.Key + "&base=EUR";

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
                                newHistoricalRate.Rate = HomeController.ConvertCurrency(1, exchangeFrom, exchangeTo);
                                newHistoricalRate.Date = thisDate;

                                model.HistoricalRates.Add(newHistoricalRate);
                            }

                        }
                    }

                    model.SaveChanges();

                    float exchangeFromToday = HomeController.rates.Where(z => z.Key == ExchangeFrom).FirstOrDefault().Value;
                    float exchangeToToday = HomeController.rates.Where(z => z.Key == ExchangeTo).FirstOrDefault().Value;
                    double baseRate = HomeController.ConvertCurrency(1, exchangeFromToday, exchangeToToday);

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
            }
            return historicalRates;
        }
    }
}