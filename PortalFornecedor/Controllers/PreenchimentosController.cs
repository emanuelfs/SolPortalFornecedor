using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class PreenchimentosController : SegurancaController
    {
        private void AnexarArquivosTramitacao(string arquivosTramitacao, Tramitacao tro)
        {
            if (!string.IsNullOrEmpty(arquivosTramitacao))
            {
                JArray auxArquivos = JsonConvert.DeserializeObject(arquivosTramitacao) as JArray;
                JObject auxArquivo;
                string nomeArquivo;
                string auxBytesArquivo;
                byte[] bytesArquivo;
                ArquivoTramitacao arquivoTramitacao;
                IList<ArquivoTramitacao> arquivos = new List<ArquivoTramitacao>();
                for (int i = 0; i < auxArquivos.Count; i++)
                {
                    auxArquivo = auxArquivos[i] as JObject;
                    nomeArquivo = auxArquivo.GetValue("nomeArquivo").ToString();
                    auxBytesArquivo = auxArquivo.GetValue("bytesArquivo").ToString();
                    auxBytesArquivo = auxBytesArquivo.Contains(",") ? auxBytesArquivo.Split(',')[1] : auxBytesArquivo.Split(':')[1];
                    bytesArquivo = Convert.FromBase64String(auxBytesArquivo);
                    arquivoTramitacao = new ArquivoTramitacao
                    {
                        NOME = nomeArquivo,
                        ARQUIVO = bytesArquivo
                    };
                    arquivos.Add(arquivoTramitacao);
                }
                tro.arquivos = arquivos;
            }
        }

        // GET: Preenchimentos
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetColunasDinamicas(int ID_FORMULARIO, char somenteAtivos)
        {
            if ('N' == somenteAtivos || FormularioAtivo(ID_FORMULARIO))
            {
                string[] colunasFixas = "ID;ID_STATUS_ATUAL;NOME_RESPONSAVEL;Criação;Identificador;Status;Loja".Split(';');
                string[] colunasFixasExibidasGrid = "Criação;Identificador;Status;Loja".Split(';');
                string[] colunasFixasExibidasBusca = "".Split(';');
                IList<string> colunasOcultas = PreenchimentoDAL.GetColunasDinamicas(ID_FORMULARIO, colunasFixas, true);
                IList<string> auxDados = PreenchimentoDAL.GetColunasDinamicas(ID_FORMULARIO, colunasFixas, false);
                IList<object> dados = new List<object>();
                string[] auxColuna;
                string coluna;
                foreach (string dado in auxDados)
                {
                    auxColuna = dado.Split('|');
                    coluna = auxColuna[0];
                    dados.Add(new
                    {
                        nomeColuna = coluna,
                        exibirColuna = !colunasOcultas.Contains(dado) || colunasFixasExibidasGrid.Contains(coluna),
                        exibirEmLancamento = (auxColuna.Length > 1 && "1".Equals(auxColuna[1])) || colunasFixasExibidasBusca.Contains(coluna),
                        exibirEmAtendimento = (auxColuna.Length > 2 && "1".Equals(auxColuna[2])) || colunasFixasExibidasBusca.Contains(coluna),
                        exibirEmBuscaAvancada = (auxColuna.Length > 3 && "1".Equals(auxColuna[3])) || colunasFixasExibidasBusca.Contains(coluna)
                    });
                }

                return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msgErro = MSG_ERRO_FORMULARIO_INATIVO }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Get(string requestModel, string idFormulario, string[] colunasFormulario, string dataInicialFiltro, string dataFinalFiltro, string statusForm, string tipoLancamentosFiltro, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            JArray auxModel = JsonConvert.DeserializeObject(requestModel) as JArray;

            JObject auxDraw = auxModel[0] as JObject;
            JObject auxStart = auxModel[3] as JObject;
            JObject auxLength = auxModel[4] as JObject;

            int draw = Convert.ToInt32(auxDraw.GetValue("value"));
            int start = Convert.ToInt32(auxStart.GetValue("value"));
            int length = Convert.ToInt32(auxLength.GetValue("value"));

            JObject auxSearch = auxModel[5] as JObject;
            JObject auxColumns = auxModel[1] as JObject;
            JObject auxOrder = auxModel[2] as JObject;

            string textoFiltro = Convert.ToString(((JObject)auxSearch.GetValue("value")).GetValue("value"));
            int indiceSortColumn = 0;
            try
            {
                indiceSortColumn = Convert.ToInt32(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("column"));
            }
            catch { }
            string sortColumn = Convert.ToString(((JObject)((JArray)auxColumns.GetValue("value"))[indiceSortColumn]).GetValue("name"));
            string sortColumnDir = "asc";
            try
            {
                sortColumnDir = Convert.ToString(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("dir"));
            }
            catch { }

            Int32 ID_FORMULARIO = Convert.ToInt32(idFormulario);
            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            string userNameCriador = null;
            if (null != tipoLancamentosFiltro && "M".Equals(tipoLancamentosFiltro))
            {
                try
                {
                    userNameCriador = UsuarioLogado.USER_NAME;
                }
                catch
                {
                    userNameCriador = string.Empty;
                }
            }
            String statusFiltroUsuarioLogado = "T".Equals(statusForm) ? UsuarioLogado.ObterStatusParaFiltro() : "''";
            IList<object> dados = PreenchimentoDAL.Get(ID_FORMULARIO, colunasFormulario, dataInicialFiltro, dataFinalFiltro, statusForm, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, userNameCriador, statusFiltroUsuarioLogado, camposDinamicosFiltro, idBuscaPreenchimento);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = PreenchimentoDAL.Get(ID_FORMULARIO, colunasFormulario, dataInicialFiltro, dataFinalFiltro, statusForm, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, userNameCriador, statusFiltroUsuarioLogado, camposDinamicosFiltro, idBuscaPreenchimento);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetParaExcel(string idFormulario, string[] colunasFormulario, string dataInicialFiltro, string dataFinalFiltro, string statusForm, string tipoLancamentosFiltro, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            Int32 ID_FORMULARIO = Convert.ToInt32(idFormulario);
            string userNameCriador = null;
            if (null != tipoLancamentosFiltro && "M".Equals(tipoLancamentosFiltro))
            {
                try
                {
                    userNameCriador = UsuarioLogado.USER_NAME;
                }
                catch
                {
                    userNameCriador = string.Empty;
                }
            }
            String statusFiltroUsuarioLogado = "T".Equals(statusForm) ? UsuarioLogado.ObterStatusParaFiltro() : "''";
            return VerificarDadosExcel<object>(PreenchimentoDAL.GetParaExcel(ID_FORMULARIO, colunasFormulario, dataInicialFiltro, dataFinalFiltro, statusForm, userNameCriador, statusFiltroUsuarioLogado, camposDinamicosFiltro, idBuscaPreenchimento), "os lançamentos", "Nenhum lançamento foi encontrado");
        }

        [HttpPost]
        public JsonResult GetParaExcelGenerica(int[] formulariosFiltro, string dataInicialFiltro, string dataFinalFiltro, string idBuscaPreenchimento)
        {
            string gruposFiltroUsuarioLogado = UsuarioLogado.ObterGruposParaFiltro();
            return VerificarDadosExcel<object>(PreenchimentoDAL.GetParaExcelGenerica(formulariosFiltro, dataInicialFiltro, dataFinalFiltro, idBuscaPreenchimento, gruposFiltroUsuarioLogado), "os lançamentos", "Nenhum lançamento foi encontrado");
        }

        [HttpPost]
        public FileResult ExportarExcel(string dados, string nomeTabela)
        {
            DataTable auxDados = JsonConvert.DeserializeObject<DataTable>(dados);
            auxDados.TableName = nomeTabela;
            return DownloadExcel(auxDados, string.Format("{0}-CSC-Brasil.xls", nomeTabela));
        }

        [HttpPost]
        public JsonResult GetBuscaAvancada(string requestModel, string idFormulario, string[] colunasFormulario, string dataInicialFiltro, string dataFinalFiltro, string statusForm, string tipoLancamentosFiltro, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            JArray auxModel = JsonConvert.DeserializeObject(requestModel) as JArray;

            JObject auxDraw = auxModel[0] as JObject;
            JObject auxStart = auxModel[3] as JObject;
            JObject auxLength = auxModel[4] as JObject;

            int draw = Convert.ToInt32(auxDraw.GetValue("value"));
            int start = Convert.ToInt32(auxStart.GetValue("value"));
            int length = Convert.ToInt32(auxLength.GetValue("value"));

            JObject auxSearch = auxModel[5] as JObject;
            JObject auxColumns = auxModel[1] as JObject;
            JObject auxOrder = auxModel[2] as JObject;

            string textoFiltro = Convert.ToString(((JObject)auxSearch.GetValue("value")).GetValue("value"));
            int indiceSortColumn = 0;
            try
            {
                indiceSortColumn = Convert.ToInt32(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("column"));
            }
            catch { }
            string sortColumn = Convert.ToString(((JObject)((JArray)auxColumns.GetValue("value"))[indiceSortColumn]).GetValue("name"));
            string sortColumnDir = "asc";
            try
            {
                sortColumnDir = Convert.ToString(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("dir"));
            }
            catch { }

            Int32 ID_FORMULARIO = Convert.ToInt32(idFormulario);
            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            string userNameCriador = null;
            if (null != tipoLancamentosFiltro && "M".Equals(tipoLancamentosFiltro))
            {
                try
                {
                    userNameCriador = UsuarioLogado.USER_NAME;
                }
                catch
                {
                    userNameCriador = string.Empty;
                }
            }
            IList<object> dados = PreenchimentoDAL.GetBuscaAvancada(ID_FORMULARIO, colunasFormulario, dataInicialFiltro, dataFinalFiltro, statusForm, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, userNameCriador, camposDinamicosFiltro, idBuscaPreenchimento);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = PreenchimentoDAL.GetBuscaAvancada(ID_FORMULARIO, colunasFormulario, dataInicialFiltro, dataFinalFiltro, statusForm, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, userNameCriador, camposDinamicosFiltro, idBuscaPreenchimento);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBuscaAvancadaGenerica(string requestModel, string dataInicialFiltro, string dataFinalFiltro, int[] formulariosFiltro, string idBuscaPreenchimento)
        {
            JArray auxModel = JsonConvert.DeserializeObject(requestModel) as JArray;

            JObject auxDraw = auxModel[0] as JObject;
            JObject auxStart = auxModel[3] as JObject;
            JObject auxLength = auxModel[4] as JObject;

            int draw = Convert.ToInt32(auxDraw.GetValue("value"));
            int start = Convert.ToInt32(auxStart.GetValue("value"));
            int length = Convert.ToInt32(auxLength.GetValue("value"));

            JObject auxSearch = auxModel[5] as JObject;
            JObject auxColumns = auxModel[1] as JObject;
            JObject auxOrder = auxModel[2] as JObject;

            string textoFiltro = Convert.ToString(((JObject)auxSearch.GetValue("value")).GetValue("value"));
            int indiceSortColumn = 0;
            try
            {
                indiceSortColumn = Convert.ToInt32(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("column"));
            }
            catch { }
            string sortColumn = Convert.ToString(((JObject)((JArray)auxColumns.GetValue("value"))[indiceSortColumn]).GetValue("name"));
            string sortColumnDir = "asc";
            try
            {
                sortColumnDir = Convert.ToString(((JObject)((JArray)auxOrder.GetValue("value"))[0]).GetValue("dir"));
            }
            catch { }

            string gruposFiltroUsuarioLogado = UsuarioLogado.ObterGruposParaFiltro();

            int totRegistros = 0;
            int totRegistrosFiltro = 0;
            IList<object> dados = PreenchimentoDAL.GetBuscaAvancadaGenerica(formulariosFiltro, dataInicialFiltro, dataFinalFiltro, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, idBuscaPreenchimento, gruposFiltroUsuarioLogado);
            if (start > 0 && dados.Count == 0)
            {
                start -= length;
                dados = PreenchimentoDAL.GetBuscaAvancadaGenerica(formulariosFiltro, dataInicialFiltro, dataFinalFiltro, start, length, ref totRegistros, textoFiltro, ref totRegistrosFiltro, sortColumn, sortColumnDir, idBuscaPreenchimento, gruposFiltroUsuarioLogado);
                return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados, voltarPagina = 'S' }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { draw = draw, recordsFiltered = totRegistrosFiltro, recordsTotal = totRegistros, data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPorPreenchimento(Int32 ID_FORMULARIO, Int32 ID_PREENCHIMENTO_FORMULARIO, Int32? ID_STATUS_REGRA, Char? realizarBloqueio, Int32? ID_STATUS_ATUAL)
        {
            JsonResult bloqueioForm = VerificarBloqueioFormulario(ID_PREENCHIMENTO_FORMULARIO, realizarBloqueio);
            if (bloqueioForm != null)
            {
                return bloqueioForm;
            }

            JsonResult statusAtualForm = VerificarStatusAtualFormulario(ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL);
            if (statusAtualForm != null)
            {
                return statusAtualForm;
            }

            string gruposFiltroUsuarioLogado = null;
            if (null != ID_STATUS_REGRA)
            {
                gruposFiltroUsuarioLogado = UsuarioLogado.ObterGruposParaFiltro();
            }

            IList<ComponentePreenchimento> dados = PreenchimentoDAL.GetPorPreenchimento(ID_FORMULARIO, ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_REGRA, gruposFiltroUsuarioLogado);

            return Json(new { data = dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(int ID_FORMULARIO, string LISTA_COMPONENTES, string OBSERVACAO, string arquivosTramitacao)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            Int32? OBRIGATORIEDADE_ANEXO = null;

            if (FormularioAtivo(ID_FORMULARIO, ref OBRIGATORIEDADE_ANEXO))
            {
                StatusFormulario statusInicial = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                if (null == statusInicial)
                {
                    auxMsgErro = "Falha ao tentar inserir o formulário: nenhum status inicial foi encontrado";
                }
                else
                {
                    if (OBRIGATORIEDADE_ANEXO == 1
                        && (string.IsNullOrEmpty(arquivosTramitacao)
                        || "[]".Equals(arquivosTramitacao)))
                    {
                        auxMsgErro = "Favor anexar pelo menos 1 arquivo";
                    }
                    else
                    {
                        string msgErroVerificacaoEmail = null;
                        StatusFormulario statusEnvioEmail = null;
                        string[] dadosEnvioEmail = null;
                        bool enviarEmail = Util.VerificarEnvioEmailStatus(statusInicial.ID, ref statusEnvioEmail, ref dadosEnvioEmail, ref msgErroVerificacaoEmail);
                        if (string.IsNullOrEmpty(msgErroVerificacaoEmail))
                        {
                            IList<ComponentePreenchimento> listaComponentes = JsonConvert.DeserializeObject<List<ComponentePreenchimento>>(LISTA_COMPONENTES);

                            Preenchimento obj = new Preenchimento
                            {
                                ID_FORMULARIO = ID_FORMULARIO,
                                componentesPreenchimento = listaComponentes
                            };

                            Tramitacao tro = new Tramitacao
                            {
                                OBSERVACAO = OBSERVACAO,
                                statusDestino = new StatusFormulario
                                {
                                    ID = statusInicial.ID
                                }
                            };

                            AnexarArquivosTramitacao(arquivosTramitacao, tro);

                            Int32? idPreenchimentoInserido = null;
                            string idBusca = PreenchimentoDAL.Insert(obj, tro, UsuarioLogado, ref idPreenchimentoInserido);
                            if (string.IsNullOrEmpty(idBusca))
                            {
                                auxMsgErro = "Falha ao tentar inserir o formulário, favor tente novamente";
                            }
                            else
                            {
                                auxMsgSucesso = string.Format("Formulário <b>{0}</b> inserido com sucesso", idBusca);
                                if (enviarEmail)
                                {
                                    String emailDestino = Util.ObterEmailUsuario(PreenchimentoDAL.ObterUserNameCriador((Int32)idPreenchimentoInserido));
                                    if (string.IsNullOrEmpty(emailDestino))
                                    {
                                        auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, "Falha ao tentar enviar o email: endereço de destino não encontrado");
                                    }
                                    else
                                    {
                                        String nomeCriador = string.Empty;
                                        String nomeFormulario = string.Empty;
                                        PreenchimentoDAL.ObterDadosParaCoringasEmail((int)idPreenchimentoInserido, ref nomeCriador, ref idBusca, ref nomeFormulario);
                                        IDictionary<string, string> coringasEmail = Util.MontarListaCoringasEmail(nomeCriador, idBusca, nomeFormulario, OBSERVACAO);
                                        String msgErroEnvioEmail = Util.EnviarEmail(emailDestino, statusEnvioEmail.TITULO_EMAIL, statusEnvioEmail.CORPO_EMAIL, dadosEnvioEmail, coringasEmail);
                                        if (!string.IsNullOrEmpty(msgErroEnvioEmail))
                                        {
                                            auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, msgErroEnvioEmail);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            auxMsgErro = string.Format("Falha ao tentar inserir o formulário: {0}", msgErroVerificacaoEmail);
                        }
                    }
                }
            }
            else
            {
                auxMsgErro = MSG_ERRO_FORMULARIO_INATIVO;
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Update(int ID_PREENCHIMENTO, int ID_FORMULARIO, string LISTA_COMPONENTES, string OBSERVACAO, string arquivosTramitacao, Char realizarBloqueio, Int32 ID_STATUS_ATUAL, string logAlteracaoComponentes)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (FormularioAtivo(ID_FORMULARIO))
            {
                JsonResult bloqueioForm = VerificarBloqueioFormulario(ID_PREENCHIMENTO, realizarBloqueio);
                if (bloqueioForm != null)
                {
                    return bloqueioForm;
                }

                JsonResult statusAtualForm = VerificarStatusAtualFormulario(ID_PREENCHIMENTO, ID_STATUS_ATUAL);
                if (statusAtualForm != null)
                {
                    return statusAtualForm;
                }

                StatusFormulario statusInicial = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                if (null == statusInicial)
                {
                    auxMsgErro = "Falha ao tentar alterar o formulário: nenhum status inicial foi encontrado";
                }
                else
                {
                    Int32? idStatusDestino = statusInicial.ID;
                    StatusFormulario statusRetorno = StatusFormularioDAL.GetRetornoPorFormulario(ID_FORMULARIO);
                    if (statusRetorno != null && ID_STATUS_ATUAL == statusRetorno.ID)
                    {
                        StatusFormulario statusEmAtendimento = StatusFormularioDAL.GetEmAtendimentoPorFormulario(ID_FORMULARIO, statusInicial.ID);
                        if (statusEmAtendimento == null)
                        {
                            idStatusDestino = null;
                            auxMsgErro = "Falha ao tentar alterar o formulário: nenhum status de atendimento foi encontrado";
                        }
                        else
                        {
                            idStatusDestino = statusEmAtendimento.ID;
                        }
                    }
                    if (null != idStatusDestino)
                    {
                        string msgErroVerificacaoEmail = null;
                        StatusFormulario statusEnvioEmail = null;
                        string[] dadosEnvioEmail = null;
                        bool enviarEmail = Util.VerificarEnvioEmailStatus((Int32)idStatusDestino, ref statusEnvioEmail, ref dadosEnvioEmail, ref msgErroVerificacaoEmail);
                        if (string.IsNullOrEmpty(msgErroVerificacaoEmail))
                        {
                            IList<ComponentePreenchimento> listaComponentes = JsonConvert.DeserializeObject<List<ComponentePreenchimento>>(LISTA_COMPONENTES);

                            Preenchimento obj = new Preenchimento
                            {
                                ID = ID_PREENCHIMENTO,
                                ID_FORMULARIO = ID_FORMULARIO,
                                componentesPreenchimento = listaComponentes
                            };

                            Tramitacao tro = new Tramitacao
                            {
                                OBSERVACAO = OBSERVACAO,
                                statusDestino = new StatusFormulario
                                {
                                    ID = (Int32)idStatusDestino
                                },
                                LOG_ALTERACAO_COMPONENTES = logAlteracaoComponentes
                            };

                            AnexarArquivosTramitacao(arquivosTramitacao, tro);

                            if (PreenchimentoDAL.Update(obj, tro, UsuarioLogado) == null)
                            {
                                auxMsgErro = "Falha ao tentar alterar o formulário, favor tente novamente";
                            }
                            else
                            {
                                auxMsgSucesso = "Formulário alterado com sucesso";
                                if (enviarEmail)
                                {
                                    String emailDestino = Util.ObterEmailUsuario(PreenchimentoDAL.ObterUserNameCriador(ID_PREENCHIMENTO));
                                    if (string.IsNullOrEmpty(emailDestino))
                                    {
                                        auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, "Falha ao tentar enviar o email: endereço de destino não encontrado");
                                    }
                                    else
                                    {
                                        String nomeCriador = string.Empty;
                                        String idBusca = string.Empty;
                                        String nomeFormulario = string.Empty;
                                        PreenchimentoDAL.ObterDadosParaCoringasEmail(ID_PREENCHIMENTO, ref nomeCriador, ref idBusca, ref nomeFormulario);
                                        IDictionary<string, string> coringasEmail = Util.MontarListaCoringasEmail(nomeCriador, idBusca, nomeFormulario, OBSERVACAO);
                                        String msgErroEnvioEmail = Util.EnviarEmail(emailDestino, statusEnvioEmail.TITULO_EMAIL, statusEnvioEmail.CORPO_EMAIL, dadosEnvioEmail, coringasEmail);
                                        if (!string.IsNullOrEmpty(msgErroEnvioEmail))
                                        {
                                            auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, msgErroEnvioEmail);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            auxMsgErro = string.Format("Falha ao tentar alterar o formulário: {0}", msgErroVerificacaoEmail);
                        }
                    }
                }
            }
            else
            {
                auxMsgErro = MSG_ERRO_FORMULARIO_INATIVO;
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Tramitar(int ID_FORMULARIO, string LISTA_COMPONENTES, int ID_PREENCHIMENTO_FORMULARIO, int ID_STATUS_ORIGEM, int ID_STATUS_DESTINO, string OBSERVACAO, string arquivosTramitacao, Char realizarBloqueio, string logAlteracaoComponentes)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (FormularioAtivo(ID_FORMULARIO))
            {
                JsonResult bloqueioForm = VerificarBloqueioFormulario(ID_PREENCHIMENTO_FORMULARIO, realizarBloqueio);
                if (bloqueioForm != null)
                {
                    return bloqueioForm;
                }

                JsonResult statusAtualForm = VerificarStatusAtualFormulario(ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ORIGEM);
                if (statusAtualForm != null)
                {
                    return statusAtualForm;
                }

                string msgErroVerificacaoEmail = null;
                StatusFormulario statusEnvioEmail = null;
                string[] dadosEnvioEmail = null;
                bool enviarEmail = Util.VerificarEnvioEmailStatus(ID_STATUS_DESTINO, ref statusEnvioEmail, ref dadosEnvioEmail, ref msgErroVerificacaoEmail);
                if (string.IsNullOrEmpty(msgErroVerificacaoEmail))
                {
                    IList<ComponentePreenchimento> listaComponentes = JsonConvert.DeserializeObject<List<ComponentePreenchimento>>(LISTA_COMPONENTES);

                    Preenchimento obj = new Preenchimento
                    {
                        componentesPreenchimento = listaComponentes
                    };

                    Tramitacao tro = new Tramitacao
                    {
                        ID_PREENCHIMENTO_FORMULARIO = ID_PREENCHIMENTO_FORMULARIO,
                        statusOrigem = new StatusFormulario
                        {
                            ID = ID_STATUS_ORIGEM
                        },
                        statusDestino = new StatusFormulario
                        {
                            ID = ID_STATUS_DESTINO
                        },
                        OBSERVACAO = OBSERVACAO,
                        LOG_ALTERACAO_COMPONENTES = logAlteracaoComponentes
                    };

                    AnexarArquivosTramitacao(arquivosTramitacao, tro);

                    if (TramitacaoDAL.Insert(obj, tro, UsuarioLogado) == null)
                    {
                        auxMsgErro = "Falha ao tentar tramitar o formulário, favor tente novamente";
                    }
                    else
                    {
                        auxMsgSucesso = "Formulário tramitado com sucesso";
                        if (enviarEmail)
                        {
                            String emailDestino = Util.ObterEmailUsuario(PreenchimentoDAL.ObterUserNameCriador(ID_PREENCHIMENTO_FORMULARIO));
                            if (string.IsNullOrEmpty(emailDestino))
                            {
                                auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, "Falha ao tentar enviar o email: endereço de destino não encontrado");
                            }
                            else
                            {
                                String nomeCriador = string.Empty;
                                String idBusca = string.Empty;
                                String nomeFormulario = string.Empty;
                                PreenchimentoDAL.ObterDadosParaCoringasEmail(ID_PREENCHIMENTO_FORMULARIO, ref nomeCriador, ref idBusca, ref nomeFormulario);
                                IDictionary<string, string> coringasEmail = Util.MontarListaCoringasEmail(nomeCriador, idBusca, nomeFormulario, OBSERVACAO);
                                String msgErroEnvioEmail = Util.EnviarEmail(emailDestino, statusEnvioEmail.TITULO_EMAIL, statusEnvioEmail.CORPO_EMAIL, dadosEnvioEmail, coringasEmail);
                                if (!string.IsNullOrEmpty(msgErroEnvioEmail))
                                {
                                    auxMsgSucesso = string.Format("{0}, porém com a seguinte falha:<br/><br/>{1}", auxMsgSucesso, msgErroEnvioEmail);
                                }
                            }
                        }
                    }
                }
                else
                {
                    auxMsgErro = string.Format("Falha ao tentar tramitar o formulário: {0}", msgErroVerificacaoEmail);
                }
            }
            else
            {
                auxMsgErro = MSG_ERRO_FORMULARIO_INATIVO;
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Atender(int ID_FORMULARIO, string[] idsPreenchimentos)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (FormularioAtivo(ID_FORMULARIO))
            {
                if (idsPreenchimentos != null && idsPreenchimentos.Length > 0)
                {
                    string textoSingPluri = idsPreenchimentos.Length == 1 ? "o" : "os";
                    StatusFormulario statusInicial = StatusFormularioDAL.GetInicialPorFormulario(ID_FORMULARIO);
                    if (null != statusInicial)
                    {
                        StatusFormulario statusEmAtendimento = StatusFormularioDAL.GetEmAtendimentoPorFormulario(ID_FORMULARIO, statusInicial.ID);
                        if (statusEmAtendimento != null)
                        {
                            string msgErroVerificacaoEmail = null;
                            StatusFormulario statusEnvioEmail = null;
                            string[] dadosEnvioEmail = null;
                            bool enviarEmail = Util.VerificarEnvioEmailStatus(statusEmAtendimento.ID, ref statusEnvioEmail, ref dadosEnvioEmail, ref msgErroVerificacaoEmail);
                            if (string.IsNullOrEmpty(msgErroVerificacaoEmail))
                            {
                                int ID_STATUS_ORIGEM = statusInicial.ID;
                                int ID_STATUS_DESTINO = statusEmAtendimento.ID;
                                string OBSERVACAO = "Atendimento iniciado";
                                IList<Tramitacao> tramitacoes = new List<Tramitacao>();
                                Tramitacao obj;
                                int idPreenchimentoFormulario;
                                string idBuscaFormulario;
                                string[] auxIdComIdBusca;
                                foreach (string idComIdBusca in idsPreenchimentos)
                                {
                                    auxIdComIdBusca = idComIdBusca.Split('_');
                                    idPreenchimentoFormulario = Convert.ToInt32(auxIdComIdBusca[0]);
                                    idBuscaFormulario = auxIdComIdBusca[1];
                                    obj = new Tramitacao
                                    {
                                        ID_PREENCHIMENTO_FORMULARIO = idPreenchimentoFormulario,
                                        ID_BUSCA_FORMULARIO = idBuscaFormulario,
                                        statusOrigem = new StatusFormulario
                                        {
                                            ID = ID_STATUS_ORIGEM
                                        },
                                        statusDestino = new StatusFormulario
                                        {
                                            ID = ID_STATUS_DESTINO
                                        },
                                        OBSERVACAO = OBSERVACAO
                                    };
                                    tramitacoes.Add(obj);
                                }

                                string resultAtender = PreenchimentoDAL.Atender(tramitacoes, statusEnvioEmail, dadosEnvioEmail, enviarEmail, UsuarioLogado);
                                if (resultAtender == null)
                                {
                                    auxMsgErro = string.Format("Falha ao tentar atender {0} formulári{1}, favor tente novamente", textoSingPluri, textoSingPluri);
                                }
                                else if (!string.IsNullOrEmpty(resultAtender))
                                {
                                    if (resultAtender.Contains("<br/>"))
                                    {
                                        auxMsgSucesso = string.Format("Operação realizada com sucesso, porém com as seguintes falhas:<br/><br/>{0}", resultAtender);
                                    }
                                    else
                                    {
                                        auxMsgSucesso = string.Format("Operação realizada com sucesso, porém com a seguinte falha:<br/><br/>{0}", resultAtender);
                                    }
                                }
                                else
                                {
                                    auxMsgSucesso = string.Format("Formulári{0} atendendid{1} com sucesso", textoSingPluri, textoSingPluri);
                                }
                            }
                            else
                            {
                                auxMsgErro = string.Format("Falha ao tentar atender {0} formulári{1}: {2}", textoSingPluri, textoSingPluri, msgErroVerificacaoEmail);
                            }
                        }
                        else
                        {
                            auxMsgErro = string.Format("Falha ao tentar atender {0} formulári{1}: nenhum status de atendimento foi encontrado", textoSingPluri, textoSingPluri);
                        }
                    }
                    else
                    {
                        auxMsgErro = string.Format("Falha ao tentar atender {0} formulári{1}: nenhum status inicial foi encontrado", textoSingPluri, textoSingPluri);
                    }
                }
                else
                {
                    auxMsgErro = "Falha ao tentar atender: nenhum registro foi selecionado";
                }
            }
            else
            {
                auxMsgErro = MSG_ERRO_FORMULARIO_INATIVO;
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }

        [HttpPost]
        public JsonResult Delete(int ID_FORMULARIO, Int32 ID, Char realizarBloqueio, Int32 ID_STATUS_ATUAL)
        {
            string auxMsgErro = string.Empty;
            string auxMsgSucesso = string.Empty;

            if (FormularioAtivo(ID_FORMULARIO))
            {
                JsonResult bloqueioForm = VerificarBloqueioFormulario(ID, realizarBloqueio);
                if (bloqueioForm != null)
                {
                    return bloqueioForm;
                }

                JsonResult statusAtualForm = VerificarStatusAtualFormulario(ID, ID_STATUS_ATUAL);
                if (statusAtualForm != null)
                {
                    return statusAtualForm;
                }

                Preenchimento obj = new Preenchimento
                {
                    ID = ID
                };

                if (PreenchimentoDAL.Delete(obj) == null)
                {
                    auxMsgErro = "Falha ao tentar excluir o formulário, favor tente novamente";
                }
                else
                {
                    auxMsgSucesso = "Formulário excluído com sucesso";
                }
            }
            else
            {
                auxMsgErro = MSG_ERRO_FORMULARIO_INATIVO;
            }

            return Json(new { msgErro = auxMsgErro, msgSucesso = auxMsgSucesso });
        }
    }
}