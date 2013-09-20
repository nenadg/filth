using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using filth.methods;
using filth.models;

namespace filth.controllers
{
    public class ProfileController : Controller
    {
        private ISetup setup;
        private IRepository<Profile> profile;

        public ProfileController() : this(new Setup(), new Repository<Profile>()) { }
        public ProfileController(ISetup _setup, IRepository<Profile> _profile)
        {
            setup = _setup;
            profile = _profile;
        }

        [FilthAuthorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
