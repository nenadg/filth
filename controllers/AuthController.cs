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
        private IFilthConfiguration _configuration;
        
        public AuthController() : this(new FilthConfiguration()) { }
        public AuthController(IFilthConfiguration _iconfiguration)
        {
            _configuration = _iconfiguration;
        }

        public HttpResponseMessage Login(User user)
        {
            var response = new HttpResponseMessage();
            
            if (ModelState.IsValid)
            {
                if (_configuration.ValidateLogin(user))
                {
                    string chunk = String.Format("#{0:X6}", new Random().Next(0x1000000)) + "." + new Random().Next();
                    System.Net.Http.Headers.CookieHeaderValue val = new System.Net.Http.Headers.CookieHeaderValue("FilthSession", chunk);
                    val.MaxAge = TimeSpan.FromMinutes(60);
                    val.Domain = "/";

                    response = new HttpResponseMessage(HttpStatusCode.Moved);
                    response.Headers.Location = new Uri( "/profile");
                    response.Headers.AddCookies(new System.Net.Http.Headers.CookieHeaderValue[] { val });

                } else
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("Nope. Either you entered invalid combination or credentials doesn't exist."));
            }
            else
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            return response;
        }
    }
}
