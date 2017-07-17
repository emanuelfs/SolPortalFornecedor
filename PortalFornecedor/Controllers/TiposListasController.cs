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
    public class TiposListasController : SegurancaController
    {
        private void AlterarTipoLista(Int32 ID, String NOME, Int32? ID_PAI, ref string auxMsgErro, ref string auxMsgSucesso)
        {
            TipoLista obj = new TipoLista
            {
                ID = ID,
                NOME = NOME,
                listaPai = ID_PAI == null ? null : new TipoLista
                {
                    ID = (Int32)ID_PAI
                }
            };

            if (TipoListaDAL.Update(obj) == null)
            {
                auxMsgErro = "Falha ao tentar alterar a lista, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Tipo de lista alterado com sucesso";
            }
        }

        // GET: TiposListas
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
            IList<TipoLista> dados = TipoListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = TipoListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<TipoListaExcel>(TipoListaDAL.GetParaExcel(), "as listas", "Nenhuma lista foi encontrada");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Listas-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetParaItem()
        {
            IList<TipoLista> dados = TipoListaDAL.GetParaItem();

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaSubLista(Int32 ID)
        {
            IList<TipoLista> dados = TipoListaDAL.GetParaSubLista(ID);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String NOME, Int32? ID_PAI)
        {
            TipoLista obj = new TipoLista
            {
                NOME = NOME,
                listaPai = ID_PAI == null ? null : new TipoLista
                {
                    ID = (Int32)ID_PAI
                }
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (TipoListaDAL.Insert(obj) == null)
            {
                auxMsgErro = "Falha ao tentar inserir a lista, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Tipo de lista inserido com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, Int32 ID, Int32? ID_PAI)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            string msgPadraoFalha = "Falha ao tentar alterar a lista, favor tente novamente";

            if (ID_PAI == null)
            {
                AlterarTipoLista(ID, NOME, ID_PAI, ref auxMsgErro, ref auxMsgSucesso);
            }
            else
            {
                if (ID != ID_PAI)
                {
                    TipoLista tipoLista = TipoListaDAL.GetPorId(ID);
                    if (tipoLista == null)
                    {
                        auxMsgErro = msgPadraoFalha;
                    }
                    else
                    {
                        if (tipoLista.listaPai.ID == ID_PAI)
                        {
                            AlterarTipoLista(ID, NOME, ID_PAI, ref auxMsgErro, ref auxMsgSucesso);
                        }
                        else
                        {
                            IList<TipoLista> sublistasPai = TipoListaDAL.GetSublistasPai(ID);
                            if (sublistasPai == null)
                            {
                                auxMsgErro = msgPadraoFalha;
                            }
                            else
                            {
                                if (sublistasPai.Count == 0)
                                {
                                    AlterarTipoLista(ID, NOME, ID_PAI, ref auxMsgErro, ref auxMsgSucesso);
                                }
                                else
                                {
                                    auxMsgErro = "A lista não pode ser alterado pois já possui ao menos uma sublista";
                                }
                            }
                        }
                    }
                }
                else
                {
                    auxMsgErro = "Uma lista não pode ser sublista dela mesma";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            TipoLista obj = new TipoLista
            {
                ID = ID
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (TipoListaDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir a lista, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Tipo de lista excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}