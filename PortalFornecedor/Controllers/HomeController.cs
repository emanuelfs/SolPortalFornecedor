using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class HomeController : SegurancaController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}