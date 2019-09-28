using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anything.Controllers
{
    public class SignUpController : Controller
    {
        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register(string Username, string Password, int MobileNumber)
        {
            using (cz2006anythingEntities myEntities = new cz2006anythingEntities())
            {
                string message = "Success";
                var username = myEntities.Users.Where(z => z.Username == Username).FirstOrDefault();
                var mobilenumber = myEntities.Users.Where(z => z.MobileNumber == MobileNumber).FirstOrDefault();
                if(username == null && mobilenumber == null)
                {
                    User user = new User();
                    user.Username = Username;
                    user.Password = Password;
                    user.MobileNumber = MobileNumber;

                    myEntities.Users.Add(user);
                    myEntities.SaveChanges();
                }
                else if(username != null)
                {
                    message = "Username already exists";
                }
                else if (mobilenumber != null)
                {
                    message = "Mobile Number already exists";
                }
                return Json(message
                , JsonRequestBehavior.AllowGet);
            }
        }
    }
}