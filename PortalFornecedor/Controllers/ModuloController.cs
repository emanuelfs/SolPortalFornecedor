using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class ModuloController : SegurancaController
    {
        // GET: Modulo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get()
        {
            int draw = Convert.ToInt32(Request.Form["draw"]);
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string textoFiltro = Request.Form["search[value]"];
            string sortColumn = Request.Form[string.Format("columns[{0}][name]", Request.Form["order[0][column]"])];
            string sortColumnDir = Request.Form["order[0][dir]"];

            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            IList<Modulo> dados = ModuloDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = ModuloDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaFuncionalidade()
        {
            IList<Modulo> dados = ModuloDAL.GetParaFuncionalidade();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<Modulo>(ModuloDAL.GetParaExcel(), "os módulos", "Nenhum módulo foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Modulos-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult Insert(String NOME)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Modulo aux = ModuloDAL.GetPorNome(NOME);
            if (aux != null)
            {
                auxMsgErro = "Já existe um módulo com o nome informado";
            }
            else
            {
                Modulo obj = new Modulo
                {
                    NOME = NOME
                };

                if (ModuloDAL.Insert(obj) == null)
                {
                    auxMsgErro = "Falha ao tentar inserir o módulo, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Módulo inserido com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, String NOME_ANTERIOR)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Modulo aux = ModuloDAL.GetPorNome(NOME);
            if (aux != null
                && !string.IsNullOrEmpty(NOME)
                && !string.IsNullOrEmpty(NOME_ANTERIOR)
                && !NOME.Equals(NOME_ANTERIOR))
            {
                auxMsgErro = "Já existe um módulo com o nome informado";
            }
            else
            {
                Modulo obj = new Modulo
                {
                    NOME = NOME
                };

                if (ModuloDAL.Update(obj, NOME_ANTERIOR) == null)
                {
                    auxMsgErro = "Falha ao tentar alterar o módulo, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Módulo alterado com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(String NOME)
        {
            Modulo obj = new Modulo
            {
                NOME = NOME
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ModuloDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o módulo, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Módulo excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}