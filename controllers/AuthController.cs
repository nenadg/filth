using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using filth.models;
using filth.methods;
using DevOne.Security.Cryptography.BCrypt;

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
                    string chunk = String.Format("#{0:X6}", new Random().Next(0x1000000)) + "." + new Random().Next();
                    System.Net.Http.Headers.CookieHeaderValue val = new System.Net.Http.Headers.CookieHeaderValue("FilthSession", chunk);
                    val.MaxAge = TimeSpan.FromMinutes(60);
                    val.Domain = "/";

                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Location = new Uri(Request.RequestUri.Authority + "/profile");
                    response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { val });

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
