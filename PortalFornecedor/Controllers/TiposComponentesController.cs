using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class TiposComponentesController : SegurancaController
    {
        // GET: TiposComponentes
        [HttpPost]
        public JsonResult GetParaComponente()
        {
            IList<TipoComponente> dados = TipoComponenteDAL.GetParaComponente();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
    }
}