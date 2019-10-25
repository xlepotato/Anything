using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anything.Controllers
{
    public class FavouriteController
    {
        public static string SetFavourite(string MoneyChangerName)
        {
            string changes = "";
            if (HttpContext.Current.Session["Username"] != null)
            {
                using (cz2006anythingEntities model = new cz2006anythingEntities())
                {
                    string username = HttpContext.Current.Session["Username"].ToString();
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
            return changes;
        }
    }
}