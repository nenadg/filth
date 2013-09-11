using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using filth.models;
using filth.methods;

namespace filth.controllers
{ 
    public class InstallController : ApiController
    {
        
        private ISetup setup;

        public InstallController() : this(new Setup()) { }
        public InstallController(ISetup _setup)
        {
            setup = _setup;
        }    

        public HttpResponseMessage Server(ServerConfiguration serverConfiguration)
        {
            FilthConfiguration configuration = new FilthConfiguration();

            ConnectionStringState state = configuration.CheckConnection();
            var response = new HttpResponseMessage();

            switch (configuration.CheckConnection())
            {
                case ConnectionStringState.Absent:
                    if (ModelState.IsValid)
                    {
                        configuration.CreateConnectionString(serverConfiguration);
                        
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Location = new Uri(Request.RequestUri.Authority + "/?install=2");                        
                    } else
                        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    break;               
                case ConnectionStringState.Present:
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Location = new Uri(Request.RequestUri.Authority + "/?install=0");
                    break;
                case ConnectionStringState.Invalid:
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("I can't connect to database, either it doesn't exist or your credentials are out of date."));
                    break;
            }

            return response;
            
        }
        
        [HttpPost]
        public HttpResponseMessage Blog(Blog blog)
        {
            if (ModelState.IsValid)
            {
                setup.CreateBlog(blog);

                var response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Headers.Location = new Uri(Request.RequestUri.Authority + "/?install=3");
                return response;
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        [HttpPost]
        public HttpResponseMessage FirstUser(User user)
        {
           
            
            if (ModelState.IsValid)
            {
                // this is all hard coded for now
                Role master = new Role(){ Name = "Masters" };

                setup.CreateUser(user, master);

                var response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Headers.Location = new Uri(Request.RequestUri.Authority + "/?install=4");
                return response;
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        
        protected override void Dispose(bool disposing)
        {
            setup.Dispose();
            base.Dispose(disposing);
        }
         
    }
}
