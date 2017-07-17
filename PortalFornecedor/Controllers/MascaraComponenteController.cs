using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class MascaraComponenteController : SegurancaController
    {
        // GET: MascaraComponente
        [HttpPost]
        public JsonResult GetParaComponente()
        {
            IList<MascaraComponente> dados = MascaraComponenteDAL.GetParaComponente();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
    }
}