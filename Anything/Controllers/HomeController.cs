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
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            if (Session["Locked"] == null)
            {
                Session["Locked"] = true;
            }
            else if ((bool)Session["Locked"] == false)
            {
                //await ApiController.SetRates();
            }
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                return View(model.Currencies.ToList());
            }
        }
        //public ActionResult GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        //{
        //    return Json(ApiController.GetCurrency(ExchangeAmount, ExchangeFrom, ExchangeTo), JsonRequestBehavior.AllowGet);
        //}
        //public async System.Threading.Tasks.Task<ActionResult> GetGraph(string ExchangeFrom, string ExchangeTo)
        //{
        //    return Json(await GraphController.GetGraph(ExchangeFrom, ExchangeTo), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Filter(string Search, string ExchangeFrom, string ExchangeTo, string SortBy, bool IsFavourite)
        {
            return Json(FilterController.Filter(Search, ExchangeFrom, ExchangeTo, SortBy, IsFavourite), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetFavourite(string MoneyChangerName)
        {
            string changes = "";
            if (Session["Username"] != null)
            {
                using (cz2006anythingEntities model = new cz2006anythingEntities())
                {
                    string username = Session["Username"].ToString();
                    var thisFavourite = model.Favourites.Where(z => z.Username == username && z.MoneyChanger.Name == MoneyChangerName).FirstOrDefault();
                    if (thisFavourite == null)
                    {
                        Favourite favourite = new Favourite();
                        favourite.MoneyChanger = model.MoneyChangers.Where(z => z.Name == MoneyChangerName).First();
                        favourite.Username = username;
                        model.Favourites.Add(favourite);
                        changes = "Added";
                    }
                    else
                    {
                        model.Favourites.Remove(thisFavourite);
                        changes = "Deleted";
                    }
                    model.SaveChanges();
                }
            }
            return Json(changes, JsonRequestBehavior.AllowGet);
        }
        [Route("a/b/c/d/{e}")]
        public ActionResult Unlock(string e)
        {
            if (e == "e")
            {
                Session["Locked"] = false;
            }
            return Redirect("/");
        }
        public ActionResult UpdateMoneyChanger(string Name, string Address, string Img, string OpeningHours, int? Tel_No)
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
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateExchangeRates(string moneychanger_name,string currency_code, float? exchange_rate_buy, float? exchange_rate_sell,string last_update_buy,string last_update_sell)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                if (exchange_rate_sell != null && exchange_rate_sell !=0)
                {
                    var selling = model.ExchangeRates.Where(z => z.MoneyChanger.Name == moneychanger_name && z.Currency.Name == currency_code && z.Currency1.Name == "SGD").FirstOrDefault();
                    if (selling == null)
                    {
                        selling = new ExchangeRate();
                        selling.MoneyChanger = model.MoneyChangers.Where(z => z.Name == moneychanger_name).FirstOrDefault();
                        selling.Currency = model.Currencies.Where(z => z.Name == currency_code).FirstOrDefault();
                        selling.Currency1 = model.Currencies.Where(z => z.Name == "SGD").FirstOrDefault();
                        selling.Rate = (float)exchange_rate_sell;
                        if (last_update_sell.ToLower().Contains("year"))
                        {
                            selling.LastUpdated = DateTime.Now.AddYears(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("month"))
                        {
                            selling.LastUpdated = DateTime.Now.AddMonths(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("day"))
                        {
                            selling.LastUpdated = DateTime.Now.AddDays(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("hour"))
                        {
                            selling.LastUpdated = DateTime.Now.AddHours(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("min"))
                        {
                            selling.LastUpdated = DateTime.Now.AddMinutes(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("second"))
                        {
                            selling.LastUpdated = DateTime.Now.AddSeconds(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else
                        {
                            selling.LastUpdated = DateTime.Now.AddYears(-1);
                        }
                        model.ExchangeRates.Add(selling);
                    }
                    else
                    {
                        selling.Rate = (float)exchange_rate_sell;
                        if (last_update_sell.ToLower().Contains("year"))
                        {
                            selling.LastUpdated = DateTime.Now.AddYears(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("month"))
                        {
                            selling.LastUpdated = DateTime.Now.AddMonths(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("day"))
                        {
                            selling.LastUpdated = DateTime.Now.AddDays(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("hour"))
                        {
                            selling.LastUpdated = DateTime.Now.AddHours(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("min"))
                        {
                            selling.LastUpdated = DateTime.Now.AddMinutes(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else if (last_update_sell.ToLower().Contains("second"))
                        {
                            selling.LastUpdated = DateTime.Now.AddSeconds(-Convert.ToInt32(last_update_sell.Split(' ')[0]));
                        }
                        else
                        {
                            selling.LastUpdated = DateTime.Now.AddYears(-1);
                        }
                    }

                }
                if (exchange_rate_buy != null && exchange_rate_buy!=0)
                {
                    var buying = model.ExchangeRates.Where(z => z.MoneyChanger.Name == moneychanger_name && z.Currency1.Name == currency_code && z.Currency.Name == "SGD").FirstOrDefault();
                    if (buying == null)
                    {
                        buying = new ExchangeRate();
                        buying.MoneyChanger = model.MoneyChangers.Where(z => z.Name == moneychanger_name).FirstOrDefault();
                        buying.Currency1 = model.Currencies.Where(z => z.Name == currency_code).FirstOrDefault();
                        buying.Currency = model.Currencies.Where(z => z.Name == "SGD").FirstOrDefault();
                        buying.Rate = (float)exchange_rate_buy;
                        if (last_update_buy.ToLower().Contains("year"))
                        {
                            buying.LastUpdated = DateTime.Now.AddYears(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("month"))
                        {
                            buying.LastUpdated = DateTime.Now.AddMonths(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("day"))
                        {
                            buying.LastUpdated = DateTime.Now.AddDays(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("hour"))
                        {
                            buying.LastUpdated = DateTime.Now.AddHours(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("min"))
                        {
                            buying.LastUpdated = DateTime.Now.AddMinutes(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("second"))
                        {
                            buying.LastUpdated = DateTime.Now.AddSeconds(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else
                        {
                            buying.LastUpdated = DateTime.Now.AddYears(-1);
                        }
                        model.ExchangeRates.Add(buying);
                    }
                    else
                    {
                        buying.Rate = (float)exchange_rate_buy;
                        if (last_update_buy.ToLower().Contains("year"))
                        {
                            buying.LastUpdated = DateTime.Now.AddYears(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("month"))
                        {
                            buying.LastUpdated = DateTime.Now.AddMonths(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("day"))
                        {
                            buying.LastUpdated = DateTime.Now.AddDays(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("hour"))
                        {
                            buying.LastUpdated = DateTime.Now.AddHours(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("min"))
                        {
                            buying.LastUpdated = DateTime.Now.AddMinutes(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else if (last_update_buy.ToLower().Contains("second"))
                        {
                            buying.LastUpdated = DateTime.Now.AddSeconds(-Convert.ToInt32(last_update_buy.Split(' ')[0]));
                        }
                        else
                        {
                            buying.LastUpdated = DateTime.Now.AddYears(-1);
                        }
                    }
                }
                model.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
  
