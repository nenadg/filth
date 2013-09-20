using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using filth.models;

namespace filth.methods
{
    public class FilthAuthorizeAttribute : AuthorizeAttribute
    {
        public Role AccessRole { get; set; }
        private ISetup setup = new Setup();

        protected override bool AuthorizeCore(HttpContextBase httpContext )
        {
            try
            {
                HttpCookie cookie = httpContext.Request.Cookies.Get("filth.sid");

                if (cookie != null && cookie.Expires >= DateTime.Now)
                {
                    // remove session key from database
                    setup.RemoveSessionKey(cookie.Value);
                    return false;
                }

                string value = httpContext.Server.UrlDecode(cookie.Value);   

                User user = setup.ValidateUser(value);

                if (user != null)
                {
                    if (this.AccessRole != null)
                        if (user.Roles.Contains(this.AccessRole))
                            return true;
                        else
                            return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "errors",
                                action = "unauthorised"
                                
                            })
                        );
        }

    }

}