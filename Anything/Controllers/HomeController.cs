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
            //await ApiController.SetRates();
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                return View(model.Currencies.ToList());
            }
        }
        //public ActionResult GetCurrency(float ExchangeAmount, string ExchangeFrom, string ExchangeTo)
        //{
        //    return Json(ApiController.GetCurrency(ExchangeAmount,ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Filter(string Search,string ExchangeFrom, string ExchangeTo)
        {
            return Json(FilterController.Filter(Search,ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);          
        }
        //public async System.Threading.Tasks.Task<ActionResult> GetGraph(string ExchangeFrom, string ExchangeTo)
        //{
        //   return Json(await GraphController.GetGraph(ExchangeFrom,ExchangeTo), JsonRequestBehavior.AllowGet);
        //}              
    }
}
  
