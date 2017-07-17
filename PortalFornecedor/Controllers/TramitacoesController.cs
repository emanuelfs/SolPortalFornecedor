using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class TramitacoesController : SegurancaController
    {
        // GET: Tramitacoes
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPorPreenchimento(Int32 ID_PREENCHIMENTO_FORMULARIO)
        {
            IList<Tramitacao> dados = TramitacaoDAL.GetPorPreenchimento(ID_PREENCHIMENTO_FORMULARIO);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }
    }
}