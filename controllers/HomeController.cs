using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Configuration;
using System.Data.Entity;

using filth.models;
using filth.methods;

namespace filth.controllers
{
    public class HomeController : Controller
    {
        

        [HttpGet]
        public ActionResult Index(int? install)
        {
            FilthConfiguration configuration = new FilthConfiguration();
            var state = configuration.CheckConnection();

            if (state == ConnectionStringState.Present || state == ConnectionStringState.Invalid)
                if (install == 2)
                    return View("Install-BlogConfiguration");
                else if (install == 3)
                    return View("Install-AddUser");
                else if (install == 4)
                    return View("Install-Ready");
                else        
                    return View();
            else
            {
                return View("Install-ServerConfiguration");
            }
        }

    } 
}
