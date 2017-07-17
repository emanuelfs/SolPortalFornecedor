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
    public class ItensListasController : SegurancaController
    {
        // GET: ItensLista
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
            IList<ItemLista> dados = ItemListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = ItemListaDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<ItemListaExcel>(ItemListaDAL.GetParaExcel(), "os itens das listas", "Nenhum item foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Itens-Listas-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetPorTipoLista(Int32 ID_TIPO_LISTA)
        {
            IList<ItemLista> dados = ItemListaDAL.GetPorTipoLista(ID_TIPO_LISTA);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPorTipoListaPai(Int32 ID_TIPO_LISTA, Int32? ID_ITEM_LISTA)
        {
            IList<ItemLista> dados = ItemListaDAL.GetPorTipoListaPai(ID_TIPO_LISTA, ID_ITEM_LISTA);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String NOME, Int32 ID_TIPO_LISTA, Int32? ID_PAI)
        {
            ItemLista obj = new ItemLista
            {
                NOME = NOME,
                tipoLista = new TipoLista
                {
                    ID = ID_TIPO_LISTA
                },
                itemPai = ID_PAI != null ?
                new ItemLista
                {
                    ID = (Int32)ID_PAI
                } : null
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ItemListaDAL.Insert(obj) == null)
            {
                auxMsgErro = "Falha ao tentar inserir o item, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Item inserido com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, Int32 ID, Int32 ID_TIPO_LISTA, Int32? ID_PAI)
        {
            ItemLista obj = new ItemLista
            {
                ID = ID,
                NOME = NOME,
                tipoLista = new TipoLista
                {
                    ID = ID_TIPO_LISTA
                },
                itemPai = ID_PAI != null ?
                new ItemLista
                {
                    ID = (Int32)ID_PAI
                } : null
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ItemListaDAL.Update(obj) == null)
            {
                auxMsgErro = "Falha ao tentar alterar o item, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Item alterado com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            ItemLista obj = new ItemLista
            {
                ID = ID
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ItemListaDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o item, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Item excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}