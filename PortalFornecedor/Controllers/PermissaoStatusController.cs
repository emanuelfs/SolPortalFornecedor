using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using CencosudCSCWEBMVC.Models.TO.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class PermissaoStatusController : SegurancaController
    {
        // GET: PermissaoStatus
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPorGrupoFormulario(String nomeGrupo, Int32? idFormulario)
        {
            IList<PermissaoStatus> dados = PermissaoStatusDAL.GetPorGrupoFormulario(nomeGrupo, idFormulario);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<PermissaoStatusExcel>(PermissaoStatusDAL.GetParaExcel(), "as permissões", "Nenhuma permissão foi encontrada");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Permissoes-Status-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult AtualizarPermissoes(String nomeGrupo, Int32? idFormulario, String[] listaStatus)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (listaStatus == null)
            {
                listaStatus = new string[0];
            }

            if (PermissaoStatusDAL.AtualizarPermissoes(nomeGrupo, idFormulario, listaStatus) == null)
            {
                auxMsgErro = "Falha ao tentar salvar as permissões, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Permissões salvas com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}