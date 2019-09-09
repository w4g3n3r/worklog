using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Models;
using wl;

namespace web.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        [HttpGet]
        public ActionResult Index(string redirectUrl)
        {
            return View(new Account() { RedirectUrl = redirectUrl });
        }

        //[HttpPost]
        //public ActionResult Index(Account account)
        //{
        //    var ontime = new OnTime(ConfigurationManager.AppSettings["Url"], ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["Secret"]);
        //    if (ontime.Login(account.Username, account.Password))
        //    {
        //        Response.SetCookie(new AuthenticationCookie() 
        //        { 
        //            Username = account.Username, 
        //            Password = account.Password, 
        //            IsAuthenticated = true, 
        //            Timeout = DateTime.UtcNow.AddMinutes(20).Ticks 
        //        }.ToCookie());
        //        Response.Redirect(account.RedirectUrl);
        //    }

        //    ViewBag.AuthenticationFailed = true;

        //    return View(account);
        //}
	}
}