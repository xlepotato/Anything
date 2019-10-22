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
                var MoneyChanger = model.MoneyChangers.Where(z => z.Name.Contains(MoneyChangerName)).OrderBy(z => z.Name.Length).FirstOrDefault();
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
    }
}