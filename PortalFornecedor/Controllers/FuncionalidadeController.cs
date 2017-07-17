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
    public class FuncionalidadeController : SegurancaController
    {
        // GET: Funcionalidade
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
            IList<Funcionalidade> dados = FuncionalidadeDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = FuncionalidadeDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<FuncionalidadeExcel>(FuncionalidadeDAL.GetParaExcel(), "as funcionalidades", "Nenhuma funcionalidade foi encontrada");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Funcionalidades-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult Insert(String NOME, String NOME_MODULO, String CAMINHO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Funcionalidade aux = FuncionalidadeDAL.GetPorCaminho(CAMINHO);
            if (aux != null)
            {
                auxMsgErro = "Já existe uma funcionalidade com o caminho informado";
            }
            else
            {
                Funcionalidade obj = new Funcionalidade
                {
                    NOME = NOME,
                    modulo = new Modulo
                    {
                        NOME = NOME_MODULO
                    },
                    CAMINHO = CAMINHO
                };

                if (FuncionalidadeDAL.Insert(obj) == null)
                {
                    auxMsgErro = "Falha ao tentar inserir a funcionalidade, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Funcionalidade inserida com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, String NOME_MODULO, String CAMINHO, String CAMINHO_ANTERIOR)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Funcionalidade aux = FuncionalidadeDAL.GetPorCaminho(CAMINHO);
            if (aux != null
                && !string.IsNullOrEmpty(CAMINHO)
                && !string.IsNullOrEmpty(CAMINHO_ANTERIOR)
                && !CAMINHO.Equals(CAMINHO_ANTERIOR))
            {
                auxMsgErro = "Já existe uma funcionalidade com o caminho informado";
            }
            else
            {
                Funcionalidade obj = new Funcionalidade
                {
                    NOME = NOME,
                    modulo = new Modulo
                    {
                        NOME = NOME_MODULO
                    },
                    CAMINHO = CAMINHO
                };

                if (FuncionalidadeDAL.Update(obj, CAMINHO_ANTERIOR) == null)
                {
                    auxMsgErro = "Falha ao tentar alterar a funcionalidade, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Funcionalidade alterada com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(String CAMINHO)
        {
            Funcionalidade obj = new Funcionalidade
            {
                CAMINHO = CAMINHO
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (FuncionalidadeDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir a funcionalidade, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Funcionalidade excluída com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}