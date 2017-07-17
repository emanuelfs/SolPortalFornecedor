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
    public class ComponentesController : SegurancaController
    {
        // GET: Componentes
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
            IList<Componente> dados = ComponenteDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = ComponenteDAL.Get(start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel()
        {
            return VerificarDadosExcel<ComponenteExcel>(ComponenteDAL.GetParaExcel(), "os componentes", "Nenhum componente foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, "Componentes-CSC-Brasil.xls");
        }

        [HttpPost]
        public JsonResult GetPorformulario(Int32 ID_FORMULARIO, Int32? ID_STATUS_REGRA)
        {
            string gruposFiltroUsuarioLogado = null;
            if (null != ID_STATUS_REGRA)
            {
                gruposFiltroUsuarioLogado = UsuarioLogado.ObterGruposParaFiltro();
            }

            IList<Componente> dados = ComponenteDAL.GetPorformulario(ID_FORMULARIO, ID_STATUS_REGRA, gruposFiltroUsuarioLogado);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(String NOME, String DESCRICAO, Int32 ORDEM, Int32? TAMANHO, Int32 OBRIGATORIEDADE, Int32 EXIBIR_NO_GRID, Int32 EXIBIR_NO_LANCAMENTO, Int32 EXIBIR_NO_ATENDIMENTO, Int32 EXIBIR_NA_BUSCA_AVANCADA, Int32 CAIXA_ALTA, Int32? ID_MASCARA_COMPONENTE, Int32? ID_VALIDADOR_COMPONENTE, Int32 ID_TIPO_COMPONENTE, Int32? ID_TIPO_VALOR_COMPONENTE, Int32 ID_FORMULARIO, Int32? ID_TIPO_LISTA)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Componente aux = ComponenteDAL.GetPorformularioOrdem(ID_FORMULARIO, ORDEM);
            if (aux != null)
            {
                auxMsgErro = "Já existe um componente com a ordem informada";
            }
            else
            {
                aux = ComponenteDAL.GetPorformularioNome(ID_FORMULARIO, NOME);
                if (aux != null)
                {
                    auxMsgErro = "Já existe um componente com o nome informado";
                }
                else
                {
                    Componente obj = new Componente
                    {
                        NOME = NOME,
                        DESCRICAO = DESCRICAO,
                        ORDEM = ORDEM,
                        TAMANHO = TAMANHO,
                        OBRIGATORIEDADE = OBRIGATORIEDADE,
                        EXIBIR_NO_GRID = EXIBIR_NO_GRID,
                        EXIBIR_NO_LANCAMENTO = EXIBIR_NO_LANCAMENTO,
                        EXIBIR_NO_ATENDIMENTO = EXIBIR_NO_ATENDIMENTO,
                        EXIBIR_NA_BUSCA_AVANCADA = EXIBIR_NA_BUSCA_AVANCADA,
                        CAIXA_ALTA = CAIXA_ALTA,
                        mascaraComponente = ID_MASCARA_COMPONENTE != null ?
                        new MascaraComponente
                        {
                            ID = (Int32)ID_MASCARA_COMPONENTE
                        }
                        : null,
                        validadorComponente = ID_VALIDADOR_COMPONENTE != null ?
                        new ValidadorComponente
                        {
                            ID = (Int32)ID_VALIDADOR_COMPONENTE
                        }
                        : null,
                        tipoComponente = new TipoComponente
                        {
                            ID = ID_TIPO_COMPONENTE
                        },
                        tipoValorComponente = ID_TIPO_VALOR_COMPONENTE != null ?
                        new TipoValorComponente
                        {
                            ID = (Int32)ID_TIPO_VALOR_COMPONENTE
                        }
                        : TipoValorComponenteDAL.GetDefault()
                        ,
                        formulario = new Formulario
                        {
                            ID = ID_FORMULARIO
                        },
                        tipoLista = ID_TIPO_LISTA != null ?
                        new TipoLista
                        {
                            ID = (Int32)ID_TIPO_LISTA
                        }
                        : null
                    };

                    if (ComponenteDAL.Insert(obj) == null)
                    {
                        auxMsgErro = "Falha ao tentar inserir o componente, favor tente novamente";
                    }
                    else
                    {
                        auxMsgSucesso = "Componente inserido com sucesso";
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(Int32 ID, String NOME, String DESCRICAO, Int32 ORDEM, Int32? TAMANHO, Int32 OBRIGATORIEDADE, Int32 EXIBIR_NO_GRID, Int32 EXIBIR_NO_LANCAMENTO, Int32 EXIBIR_NO_ATENDIMENTO, Int32 EXIBIR_NA_BUSCA_AVANCADA, Int32 CAIXA_ALTA, Int32? ID_MASCARA_COMPONENTE, Int32? ID_VALIDADOR_COMPONENTE, Int32 ID_TIPO_COMPONENTE, Int32? ID_TIPO_VALOR_COMPONENTE, Int32 ID_FORMULARIO, Int32? ID_TIPO_LISTA, string somenteLeitura)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Componente aux = ComponenteDAL.GetPorformularioOrdem(ID_FORMULARIO, ORDEM);
            if (aux != null && aux.ID != ID)
            {
                auxMsgErro = "Já existe um componente com a ordem informada";
            }
            else
            {
                aux = ComponenteDAL.GetPorformularioNome(ID_FORMULARIO, NOME);
                if (aux != null && aux.ID != ID)
                {
                    auxMsgErro = "Já existe um componente com o nome informado";
                }
                else
                {
                    if ("S".Equals(somenteLeitura))
                    {
                        Componente obj = new Componente
                        {
                            ID = ID,
                            ORDEM = ORDEM,
                            OBRIGATORIEDADE = OBRIGATORIEDADE,
                            EXIBIR_NO_GRID = EXIBIR_NO_GRID,
                            EXIBIR_NO_LANCAMENTO = EXIBIR_NO_LANCAMENTO,
                            EXIBIR_NO_ATENDIMENTO = EXIBIR_NO_ATENDIMENTO,
                            EXIBIR_NA_BUSCA_AVANCADA = EXIBIR_NA_BUSCA_AVANCADA,
                            CAIXA_ALTA = CAIXA_ALTA
                        };

                        if (ComponenteDAL.UpdateSomenteLeitura(obj) == null)
                        {
                            auxMsgErro = "Falha ao tentar alterar o componente, favor tente novamente";
                        }
                        else
                        {
                            auxMsgSucesso = "Componente alterado com sucesso";
                        }
                    }
                    else
                    {
                        Componente obj = new Componente
                        {
                            ID = ID,
                            NOME = NOME,
                            DESCRICAO = DESCRICAO,
                            ORDEM = ORDEM,
                            TAMANHO = TAMANHO,
                            OBRIGATORIEDADE = OBRIGATORIEDADE,
                            EXIBIR_NO_GRID = EXIBIR_NO_GRID,
                            EXIBIR_NO_LANCAMENTO = EXIBIR_NO_LANCAMENTO,
                            EXIBIR_NO_ATENDIMENTO = EXIBIR_NO_ATENDIMENTO,
                            EXIBIR_NA_BUSCA_AVANCADA = EXIBIR_NA_BUSCA_AVANCADA,
                            CAIXA_ALTA = CAIXA_ALTA,
                            mascaraComponente = ID_MASCARA_COMPONENTE != null ?
                            new MascaraComponente
                            {
                                ID = (Int32)ID_MASCARA_COMPONENTE
                            }
                            : null,
                            validadorComponente = ID_VALIDADOR_COMPONENTE != null ?
                            new ValidadorComponente
                            {
                                ID = (Int32)ID_VALIDADOR_COMPONENTE
                            }
                            : null,
                            tipoComponente = new TipoComponente
                            {
                                ID = ID_TIPO_COMPONENTE
                            },
                            tipoValorComponente = ID_TIPO_VALOR_COMPONENTE != null ?
                            new TipoValorComponente
                            {
                                ID = (Int32)ID_TIPO_VALOR_COMPONENTE
                            }
                            : TipoValorComponenteDAL.GetDefault()
                            ,
                            formulario = new Formulario
                            {
                                ID = ID_FORMULARIO
                            },
                            tipoLista = ID_TIPO_LISTA != null ?
                            new TipoLista
                            {
                                ID = (Int32)ID_TIPO_LISTA
                            }
                            : null
                        };

                        if (ComponenteDAL.Update(obj) == null)
                        {
                            auxMsgErro = "Falha ao tentar alterar o componente, favor tente novamente";
                        }
                        else
                        {
                            auxMsgSucesso = "Componente alterado com sucesso";
                        }
                    }
                }
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(Int32 ID)
        {
            Componente obj = new Componente
            {
                ID = ID
            };

            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;
            if (ComponenteDAL.Delete(obj) == null)
            {
                auxMsgErro = "Falha ao tentar excluir o componente, favor tente novamente";
            }
            else
            {
                auxMsgSucesso = "Componente excluído com sucesso";
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}