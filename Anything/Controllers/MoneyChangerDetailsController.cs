using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anything.Controllers
{
    public class MoneyChangerDetailsController : Controller
    {
        [Route("MoneyChangers/{MoneyChangerName}")]
        public ActionResult Details(string MoneyChangerName)
        {
            using (cz2006anythingEntities model = new cz2006anythingEntities())
            {
                var MoneyChanger = model.MoneyChangers.Where(z => z.Name.Replace(".","").Contains(MoneyChangerName)).OrderBy(z => z.Name.Length).FirstOrDefault();
                if (MoneyChanger != null)
                {
                    var x = MoneyChanger.ExchangeRates;
                    var y = MoneyChanger.Favourites;
                }
                return View(MoneyChanger);
            }
        }
        public ActionResult SetFavourite(string MoneyChangerName)
        {
            return Json(FavouriteController.SetFavourite(MoneyChangerName), JsonRequestBehavior.AllowGet);
        }
    }
}