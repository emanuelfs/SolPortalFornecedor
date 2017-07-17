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
    public class StatusFormularioController : SegurancaController
    {
        // GET: StatusFormulario
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
            IList<StatusFormulario> dados = StatusFormularioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = StatusFormularioDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<StatusFormularioExcel>(StatusFormularioDAL.GetParaExcel(), "os status", "Nenhum status foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Status-Formularios-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetPorformulario(Int32 ID_FORMULARIO, Int32? ID_STATUS_ORIGEM, Char? filtrarStatus)
        {
            string statusFiltroUsuarioLogado = null;
            if ('S' == filtrarStatus)
            {
                statusFiltroUsuarioLogado = UsuarioLogado.ObterStatusParaFiltro();
            }

            IList<StatusFormulario> dados = StatusFormularioDAL.GetPorformulario(ID_FORMULARIO, ID_STATUS_ORIGEM, statusFiltroUsuarioLogado);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCorpoEmailParaAlteracao(Int32 idStatus)
        {
            string corpoEmail = StatusFormularioDAL.GetCorpoEmailParaAlteracao(idStatus);
            return Json(new { corpoEmail = corpoEmail }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String NOME, Int32 ID_FORMULARIO, Int32 INICIAL, Int32 RETORNO, Int32 ENVIAR_EMAIL, String TITULO_EMAIL, String CORPO_EMAIL)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (1 == INICIAL && INICIAL == RETORNO)
            {
                auxMsgErro = "Um status não pode ser inicial e de retorno ao mesmo tempo";
            }
            else
            {
                StatusFormulario aux = null;
                if (1 == INICIAL)
                {
                    aux = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                }
                if (aux != null)
                {
                    auxMsgErro = "Já existe um status inicial para o formulário informado";
                }
                else
                {
                    if (1 == RETORNO)
                    {
                        aux = StatusFormularioDAL.GetRetornoPorFormulario(ID_FORMULARIO);
                    }
                    if (aux != null)
                    {
                        auxMsgErro = "Já existe um status de retorno para o formulário informado";
                    }
                    else
                    {
                        StatusFormulario obj = new StatusFormulario
                        {
                            NOME = NOME,
                            INICIAL = INICIAL,
                            RETORNO = RETORNO,
                            ENVIAR_EMAIL = ENVIAR_EMAIL,
                            TITULO_EMAIL = TITULO_EMAIL,
                            CORPO_EMAIL = CORPO_EMAIL,
                            formulario = new Formulario
                            {
                                ID = ID_FORMULARIO
                            }
                        };

                        if (StatusFormularioDAL.Insert(obj) == null)
                        {
                            auxMsgErro = "Falha ao tentar inserir o status do formulário, favor tente novamente";
                        }
                        else
                        {
                            auxMsgSucesso = "Status do formulário inserido com sucesso";
                        }
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(Int32 ID, String NOME, Int32 ID_FORMULARIO, Int32 INICIAL, Int32 RETORNO, Int32 ENVIAR_EMAIL, String TITULO_EMAIL, String CORPO_EMAIL)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (1 == INICIAL && INICIAL == RETORNO)
            {
                auxMsgErro = "Um status não pode ser inicial e de retorno ao mesmo tempo";
            }
            else
            {
                StatusFormulario aux = null;
                if (1 == INICIAL)
                {
                    aux = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                }
                if (aux != null && aux.ID != ID)
                {
                    auxMsgErro = "Já existe um status inicial para o formulário informado";
                }
                else
                {
                    if (1 == RETORNO)
                    {
                        aux = StatusFormularioDAL.GetRetornoPorFormulario(ID_FORMULARIO);
                    }
                    if (aux != null && aux.ID != ID)
                    {
                        auxMsgErro = "Já existe um status de retorno para o formulário informado";
                    }
                    else
                    {
                        StatusFormulario obj = new StatusFormulario
                        {
                            ID = ID,
                            NOME = NOME,
                            INICIAL = INICIAL,
                            RETORNO = RETORNO,
                            ENVIAR_EMAIL = ENVIAR_EMAIL,
                            TITULO_EMAIL = TITULO_EMAIL,
                            CORPO_EMAIL = CORPO_EMAIL,
                            formulario = new Formulario
                            {
                                ID = ID_FORMULARIO
                            }
                        };

                        if (StatusFormularioDAL.Update(obj) == null)
                        {
                            auxMsgErro = "Falha ao tentar alterar o status do formulario, favor tente novamente";
                        }
                        else
                        {
                            auxMsgSucesso = "Status do formulario alterado com sucesso";
                        }
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            StatusFormulario obj = new StatusFormulario
            {
                ID = ID
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (StatusFormularioDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o status do formulario, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Status do formulario excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}