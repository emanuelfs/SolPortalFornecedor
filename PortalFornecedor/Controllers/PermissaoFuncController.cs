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
    public class PermissaoFuncController : SegurancaController
    {
        // GET: PermissaoFunc
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPorGrupoModulo(String nomeGrupo, String nomeModulo)
        {
            IList<PermissaoFunc> dados = PermissaoFuncDAL.GetPorGrupoModulo(nomeGrupo, nomeModulo);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<PermissaoFuncExcel>(PermissaoFuncDAL.GetParaExcel(), "as permissões", "Nenhuma permissão foi encontrada");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Permissoes-Funcionalidades-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult AtualizarPermissoes(String nomeGrupo, String nomeModulo, String[] funcionalidades)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (funcionalidades == null)
            {
                funcionalidades = new string[0];
            }

            if (PermissaoFuncDAL.AtualizarPermissoes(nomeGrupo, nomeModulo, funcionalidades) == null)
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