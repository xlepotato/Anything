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
            if(Session["Username"]!=null)
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
            if(e=="e")
            {
                Session["Locked"] = false;
            }
            return Redirect("/");
        }
    }
}
  
