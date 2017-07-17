using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class ValidadorComponenteController : SegurancaController
    {
        // GET: ValidadorComponente
        [HttpPost]
        public JsonResult GetParaComponente()
        {
            IList<ValidadorComponente> dados = ValidadorComponenteDAL.GetParaComponente();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
    }
}