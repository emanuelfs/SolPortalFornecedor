using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using CencosudCSCWEBMVC.Models.TO.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class FluxoStatusController : SegurancaController
    {
        // GET: FluxoStatus
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
            IList<FluxoStatus> dados = FluxoStatusDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = FluxoStatusDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<FluxoStatusExcel>(FluxoStatusDAL.GetParaExcel(), "os fluxos", "Nenhum fluxo foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Fluxos-Formularios-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetPorFormulario(Int32 ID_FORMULARIO)
        {
            string auxMsgErro = string.Empty;

            IList<FluxoStatus> dados = FluxoStatusDAL.GetPorFormulario(ID_FORMULARIO);
            if (dados == null)
            {
                auxMsgErro = "Falha ao tentar obter os fluxos, favor tente novamente";
            }

            return Json(new { data = dados, msgErro = auxMsgErro });
        }

        [HttpPost]
        public JsonResult Insert(Int32 ID_FORMULARIO, Int32 ID_STATUS_ORIGEM, Int32 ID_STATUS_DESTINO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (ID_STATUS_ORIGEM == ID_STATUS_DESTINO)
            {
                auxMsgErro = "Favor informar o status de origem diferente do status de destino";
            }
            else
            {
                StatusFormulario statusInicial = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                if (statusInicial != null && statusInicial.ID == ID_STATUS_ORIGEM)
                {
                    FluxoStatus fluxoStatusInicial = FluxoStatusDAL.GetPorOrigem(ID_STATUS_ORIGEM);
                    if (fluxoStatusInicial != null)
                    {
                        auxMsgErro = "Já existe um fluxo inicial para o formulário informado";
                    }
                }
            }

            if (string.IsNullOrEmpty(auxMsgErro))
            {
                FluxoStatus obj = new FluxoStatus
                {
                    statusOrigem = new StatusFormulario
                    {
                        ID = ID_STATUS_ORIGEM
                    },
                    statusDestino = new StatusFormulario
                    {
                        ID = ID_STATUS_DESTINO
                    }
                };

                StringBuilder msgErro = new StringBuilder();
                if (FluxoStatusDAL.Insert(obj, ref msgErro) == null)
                {
                    if (string.IsNullOrEmpty(msgErro.ToString()))
                    {
                        auxMsgErro = "Falha ao tentar inserir o fluxo do formulário, favor tente novamente";
                    }
                    else
                    {
                        auxMsgErro = msgErro.ToString();
                    }
                }
                else
                {
                    auxMsgSucesso = "Fluxo do formulário inserido com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID_STATUS_ORIGEM, Int32 ID_STATUS_DESTINO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            FluxoStatus obj = new FluxoStatus
            {
                statusOrigem = new StatusFormulario
                {
                    ID = ID_STATUS_ORIGEM
                },
                statusDestino = new StatusFormulario
                {
                    ID = ID_STATUS_DESTINO
                }
            };

            if (FluxoStatusDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o fluxo do formulario, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Fluxo do formulario excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}