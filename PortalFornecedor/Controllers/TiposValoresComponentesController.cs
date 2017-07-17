using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class TiposValoresComponentesController : SegurancaController
    {
        // GET: TiposValoresComponentesController
        [HttpPost]
        public JsonResult GetParaComponente()
        {
            IList<TipoValorComponente> dados = TipoValorComponenteDAL.GetParaComponente();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
    }
}