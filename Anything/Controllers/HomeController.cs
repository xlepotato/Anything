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
           
            return Json(FavouriteController.SetFavourite(MoneyChangerName), JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateMoneyChanger(string Name, string Address, string Img, string OpeningHours, int? Tel_No)
        {
            ApiController.UpdateMoneyChanger(Name, Address, Img, OpeningHours, Tel_No);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateExchangeRates(string moneychanger_name,string currency_code, float? exchange_rate_buy, float? exchange_rate_sell,string last_update_buy,string last_update_sell)
        {
            ApiController.UpdateExchangeRates(moneychanger_name, currency_code, exchange_rate_buy, exchange_rate_sell, last_update_buy, last_update_sell);
            return Json("", JsonRequestBehavior.AllowGet);
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
    }
}
  
