using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Extensions
{
   
    public class AxosoftAuthorizeAttribute : AuthorizeAttribute
    {
        private const string DEFAULT_REDIRECT_URL = "~/Login";

        private string _redirectUrl = DEFAULT_REDIRECT_URL;

        public AxosoftAuthorizeAttribute(string redirectUrl = null)
        {
            if(redirectUrl != null)
                _redirectUrl = redirectUrl;
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var isAuthorized = GetIsAuthorized(filterContext.HttpContext.Request.Cookies);


            if (!isAuthorized)
            {
                filterContext.RequestContext.HttpContext.Response.Redirect(_redirectUrl);
            }
        }

        private bool GetIsAuthorized(HttpCookieCollection cookies)
        {
            if (!cookies.AllKeys.Contains(AuthenticationCookie.COOKIE_NAME))
                return false;

            var authCookie = new AuthenticationCookie(cookies[AuthenticationCookie.COOKIE_NAME]);

            return authCookie.IsAuthenticated &&
                new DateTime(authCookie.Timeout, DateTimeKind.Utc) > DateTime.UtcNow;
        }
    }
    
}