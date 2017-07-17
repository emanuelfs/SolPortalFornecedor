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
    public class PermissaoCompController : SegurancaController
    {
        // GET: PermissaoComp
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
            IList<PermissaoComp> dados = PermissaoCompDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = PermissaoCompDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<PermissaoCompExcel>(PermissaoCompDAL.GetParaExcel(), "as permissões", "Nenhuma permissão foi encontrada");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Permissoes-Componentes-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult Insert(String NOME_GRUPO, Int32 ID_STATUS, Int32 ID_COMPONENTE, Int32 TIPO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            PermissaoComp obj = new PermissaoComp
            {
                grupo = new Grupo
                {
                    NOME = NOME_GRUPO
                },
                status = new StatusFormulario
                {
                    ID = ID_STATUS
                },
                componente = new Componente
                {
                    ID = ID_COMPONENTE
                },
                TIPO = TIPO,
            };

            if (PermissaoCompDAL.Insert(obj) == null)
            {
                auxMsgErro = "Falha ao tentar inserir a permissão, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Permissão inserida com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME_GRUPO, Int32 ID_STATUS, Int32 ID_COMPONENTE, Int32 TIPO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            PermissaoComp obj = new PermissaoComp
            {
                grupo = new Grupo
                {
                    NOME = NOME_GRUPO
                },
                status = new StatusFormulario
                {
                    ID = ID_STATUS
                },
                componente = new Componente
                {
                    ID = ID_COMPONENTE
                },
                TIPO = TIPO,
            };

            if (PermissaoCompDAL.Update(obj) == null)
            {
                auxMsgErro = "Falha ao tentar alterar a permissão, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Permissão alterada com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(String NOME_GRUPO, Int32 ID_STATUS, Int32 ID_COMPONENTE)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            PermissaoComp obj = new PermissaoComp
            {
                grupo = new Grupo
                {
                    NOME = NOME_GRUPO
                },
                status = new StatusFormulario
                {
                    ID = ID_STATUS
                },
                componente = new Componente
                {
                    ID = ID_COMPONENTE
                }
            };

            if (PermissaoCompDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir a permissão, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Permissão excluída com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}