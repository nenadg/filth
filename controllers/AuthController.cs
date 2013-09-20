using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using filth.models;
using filth.methods;

namespace filth.controllers
{
    public class AuthController : ApiController
    {
        private ISetup setup;

        public AuthController() : this(new Setup()) { }
        public AuthController(ISetup _setup)
        {
            setup = _setup;
        }

        [HttpPost]
        public HttpResponseMessage Login(User user)
        { 
            var response = new HttpResponseMessage();
            
            if (ModelState.IsValid)
            {
                if (setup.ValidateLogin(user))
                {
                    // --- stupid part starts here ---
                    var context = Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
                    string useragent = context.Request.UserAgent;
                    string ip = context.Request.UserHostAddress;

                    Random rand = new Random();
                    Session session = new Session() { User = user, Authorised = true, IP = ip, UserAgent = useragent };

                    // here we'll generate some random color every time user logs in
                    // for the random value of the cookie
                    string color = String.Format("{0:X6}", rand.Next(0x1000000));
                    string secret = setup.GenerateSessionKey(session, color);

                    response = new HttpResponseMessage(HttpStatusCode.OK);               
                    response.Headers.Location = new Uri(Request.RequestUri.Authority + "/profile/index/" + user.Username);
                    
                    if (secret != null)
                    {
                        System.Net.Http.Headers.CookieHeaderValue cookie = new System.Net.Http.Headers.CookieHeaderValue("filth.sid", secret);

                        // for some strange reason IE won't accept cookies containing Domain attribute ?!
                        //cookie.Domain = Request.RequestUri.Host;
                        cookie.Path = "/";
                        cookie.HttpOnly = true; // prevents JavaScript-based cookie theft
 
                        response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { cookie });
                    }
                    // --- stupid part ends here ---
                } else
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("Nope. Either you entered invalid combination or credentials doesn't exist."));
            }
            else
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            setup.Dispose();
            base.Dispose(disposing);
        }
    }
}
