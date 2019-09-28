using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anything.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            Session["Username"] = null;
            return View();
        }
        public ActionResult Login(string Username, string Password)
        {
            using (cz2006anythingEntities myEntities = new cz2006anythingEntities())
            {
                string message = "Success";
                var user = myEntities.Users.Where(z => z.Username == Username && z.Password==Password).FirstOrDefault();
             
               if (user == null)
                {
                    message = "Invalid Login Credentials";
                }
               else
                {
                    Session["Username"] = user.Username;
                }
                return Json(message
                , JsonRequestBehavior.AllowGet);
            }
        }
    }
}