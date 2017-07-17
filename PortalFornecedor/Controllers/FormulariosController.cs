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
    public class FormulariosController : SegurancaController
    {
        // GET: Formularios
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
            IList<Formulario> dados = FormularioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = FormularioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<FormularioExcel>(FormularioDAL.GetParaExcel(), "os formulários", "Nenhum formulário foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Formularios-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetParaComponente()
        {
            IList<Formulario> dados = FormularioDAL.GetParaComponente();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaHome(Char somenteAtivos, Char filtrarStatus)
        {
            string statusFiltroUsuarioLogado = null;
            if ('S' == filtrarStatus)
            {
                statusFiltroUsuarioLogado = UsuarioLogado.ObterStatusParaFiltro();
            }

            string gruposFiltroUsuarioLogado = null;
            if ('N' == somenteAtivos && 'S' == filtrarStatus)
            {
                gruposFiltroUsuarioLogado = UsuarioLogado.ObterGruposParaFiltro();
            }

            IList<Formulario> dados = FormularioDAL.GetParaHome('S' == somenteAtivos, statusFiltroUsuarioLogado, gruposFiltroUsuarioLogado);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String NOME, Int32 CODIGO, String SIGLA, Int32 OBRIGATORIEDADE_ANEXO, Int32 EXIBIR_TODOS)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Formulario aux = FormularioDAL.GetPorCodigo(CODIGO);
            if (aux != null)
            {
                auxMsgErro = "Já existe um formulário com o código informado";
            }
            else
            {
                aux = FormularioDAL.GetPorSigla(SIGLA);
                if (aux != null)
                {
                    auxMsgErro = "Já existe um formulário com a sigla informada";
                }
                else
                {
                    Formulario obj = new Formulario
                    {
                        NOME = NOME,
                        CODIGO = CODIGO,
                        SIGLA = SIGLA,
                        OBRIGATORIEDADE_ANEXO = OBRIGATORIEDADE_ANEXO,
                        EXIBIR_TODOS = EXIBIR_TODOS
                    };

                    if (FormularioDAL.Insert(obj) == null)
                    {
                        auxMsgErro = "Falha ao tentar inserir o formulário, favor tente novamente";
                    }
                    else
                    {
                        auxMsgSucesso = "Formulário inserido com sucesso";
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, Int32 CODIGO, String SIGLA, Int32 OBRIGATORIEDADE_ANEXO, Int32 EXIBIR_TODOS, Int32 ATIVO, Int32 ID)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Formulario aux = FormularioDAL.GetPorCodigo(CODIGO);
            if (aux != null && aux.ID != ID)
            {
                auxMsgErro = "Já existe um formulário com o código informado";
            }
            else
            {
                aux = FormularioDAL.GetPorSigla(SIGLA);
                if (aux != null && aux.ID != ID)
                {
                    auxMsgErro = "Já existe um formulário com a sigla informada";
                }
                else
                {
                    Formulario obj = new Formulario
                    {
                        ID = ID,
                        NOME = NOME,
                        CODIGO = CODIGO,
                        SIGLA = SIGLA,
                        OBRIGATORIEDADE_ANEXO = OBRIGATORIEDADE_ANEXO,
                        EXIBIR_TODOS = EXIBIR_TODOS,
                        ATIVO = ATIVO
                    };

                    if (FormularioDAL.Update(obj) == null)
                    {
                        auxMsgErro = "Falha ao tentar alterar o formulário, favor tente novamente";
                    }
                    else
                    {
                        auxMsgSucesso = "Formulário alterado com sucesso";
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            Formulario obj = new Formulario
            {
                ID = ID
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (FormularioDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o formulário, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Formulário excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}