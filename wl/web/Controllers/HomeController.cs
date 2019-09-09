using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Extensions;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [AxosoftAuthorize]
        public ActionResult Index()
        {
            return View();
        }
	}
}