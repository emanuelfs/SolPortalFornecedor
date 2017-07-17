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
    public class ParametroController : SegurancaController
    {
        // GET: Parametro
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
            IList<Parametro> dados = ParametroDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = ParametroDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<ParametroExcel>(ParametroDAL.GetParaExcel(), "os parâmetros", "Nenhum parâmetro foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Parametros-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetParametrosParaUpload(Int32? ID_PREENCHIMENTO_FORMULARIO)
        {
            object auxLimiteArquivosFormulario = null;
            object auxLimiteTamanhoArquivoUpload = null;
            object auxTiposArquivoFormulario = null;
            object auxLimiteNomeArquivoUpload = null;

            Int32? totArquivosPreenchimento = 0;
            if (null != ID_PREENCHIMENTO_FORMULARIO)
            {
                totArquivosPreenchimento = ArquivoTramitacaoDAL.ObterTotArquivoPreenchimento(ID_PREENCHIMENTO_FORMULARIO);
            }
            if (totArquivosPreenchimento != null)
            {
                Parametro limiteArquivos = ParametroDAL.GetPorNome(ParametroDAL.LIMITE_ARQUIVOS_FORMULARIO);
                Int32 valorLimiteArquivos;
                if (limiteArquivos != null && Int32.TryParse(limiteArquivos.VALOR, out valorLimiteArquivos))
                {
                    if (valorLimiteArquivos > totArquivosPreenchimento)
                    {
                        auxLimiteArquivosFormulario = valorLimiteArquivos - totArquivosPreenchimento;
                    }
                    else
                    {
                        auxLimiteArquivosFormulario = 0;
                    }
                }
            }

            Parametro limiteTamanho = ParametroDAL.GetPorNome(ParametroDAL.LIMITE_TAMANHO_ARQUIVO_UPLOAD);
            Int32 valorLimiteTamanho;
            if (limiteTamanho != null && Int32.TryParse(limiteTamanho.VALOR, out valorLimiteTamanho))
            {
                auxLimiteTamanhoArquivoUpload = valorLimiteTamanho;
            }

            Parametro tiposArquivo = ParametroDAL.GetPorNome(ParametroDAL.TIPOS_ARQUIVO_FORMULARIO);
            if (tiposArquivo != null)
            {
                auxTiposArquivoFormulario = tiposArquivo.VALOR;
            }

            Parametro limiteTamanhoNome = ParametroDAL.GetPorNome(ParametroDAL.LIMITE_NOME_ARQUIVO_TRAMITACAO);
            Int32 valorLimiteTamanhoNome;
            if (limiteTamanhoNome != null && Int32.TryParse(limiteTamanhoNome.VALOR, out valorLimiteTamanhoNome))
            {
                auxLimiteNomeArquivoUpload = valorLimiteTamanhoNome;
            }

            return Json(new { limiteArquivosFormulario = auxLimiteArquivosFormulario, limiteTamanhoArquivoUpload = auxLimiteTamanhoArquivoUpload, tiposArquivoFormulario = auxTiposArquivoFormulario, limiteNomeArquivoUpload = auxLimiteNomeArquivoUpload });
        }

        [HttpPost]
        public JsonResult Insert(String NOME, String VALOR, String DESCRICAO)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Parametro obj = new Parametro
            {
                NOME = NOME,
                VALOR = VALOR,
                DESCRICAO = DESCRICAO
            };

            StringBuilder msgErro = new StringBuilder();
            if (ParametroDAL.Insert(obj, ref msgErro) == null)
            {
                if (string.IsNullOrEmpty(msgErro.ToString()))
                {
                    auxMsgErro = "Falha ao tentar inserir o parâmetro, favor tente novamente";
                }
                else
                {
                    auxMsgErro = msgErro.ToString();
                }
            }
            else
            {
                auxMsgSucesso = "Parâmetro inserido com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(String NOME, String VALOR, String DESCRICAO, String NOME_ANTERIOR)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Parametro aux = ParametroDAL.GetPorNome(NOME);
            if (aux != null
                && !string.IsNullOrEmpty(NOME)
                && !string.IsNullOrEmpty(NOME_ANTERIOR)
                && !NOME.Equals(NOME_ANTERIOR))
            {
                auxMsgErro = "Já existe um parâmetro com o nome informado";
            }
            else
            {
                Parametro obj = new Parametro
                {
                    NOME = NOME,
                    VALOR = VALOR,
                    DESCRICAO = DESCRICAO
                };

                if (ParametroDAL.Update(obj, NOME_ANTERIOR) == null)
                {
                    auxMsgErro = "Falha ao tentar alterar o parâmetro, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Parâmetro alterado com sucesso";
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(String NOME)
        {
            Parametro obj = new Parametro
            {
                NOME = NOME
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ParametroDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o parâmetro, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Parâmetro excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}