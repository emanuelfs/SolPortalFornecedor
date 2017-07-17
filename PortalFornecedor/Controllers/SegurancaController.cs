using CencosudCSCWEBMVC.Models.DAL;
using CencosudCSCWEBMVC.Models.TO;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class SegurancaController : Controller
    {
        private const string USUARIO_LOGADO = "USUARIO_LOGADO";
        private const string CONTROLLER_SEGURANCA = "Seguranca";
        private const string LOGIN_SEGURANCA = "Login";
        private const string ACESSO_NEGADO_SEGURANCA = "AcessoNegado";

        protected const string MSG_ERRO_FORMULARIO_INATIVO = "O formulário não se encontra mais ativo";

        public static Usuario UsuarioLogado
        {
            get
            {
                if (System.Web.HttpContext.Current.Session[USUARIO_LOGADO] != null)
                {
                    return (Usuario)System.Web.HttpContext.Current.Session[USUARIO_LOGADO];
                }
                return null;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ActionDescriptor descriptor = filterContext.ActionDescriptor;
            string controllerName = descriptor.ControllerDescriptor.ControllerName;
            string actionName = descriptor.ActionName;

            if (Session[USUARIO_LOGADO] == null)
            {
                if (!CONTROLLER_SEGURANCA.Equals(controllerName)
                    || !LOGIN_SEGURANCA.Equals(actionName))
                {
                    filterContext.Result = new RedirectResult(string.Format("~/{0}/{1}", CONTROLLER_SEGURANCA, LOGIN_SEGURANCA));
                }
            }
            else
            {
                if ("INDEX".Equals(actionName.ToUpper())
                    && !UsuarioLogado.PossuiPermissao(string.Format("{0}/{1}", controllerName, actionName))
                    && (!CONTROLLER_SEGURANCA.Equals(controllerName)
                        || !ACESSO_NEGADO_SEGURANCA.Equals(actionName)))
                {
                    filterContext.Result = new RedirectResult(string.Format("~/{0}/{1}", CONTROLLER_SEGURANCA, ACESSO_NEGADO_SEGURANCA));
                }
            }
        }

        private Usuario MontarUsuarioLogin(string userName)
        {
            Usuario result;

            try
            {
                Parametro dadosLDAP = ParametroDAL.GetPorNome(ParametroDAL.DADOS_CONEXAO_ACTIVE_DIRECTORY);
                string[] auxDadosLDAP = dadosLDAP.VALOR.Split(';');

                Parametro propriedadesAD = ParametroDAL.GetPorNome(ParametroDAL.PROPRIEDADES_USUARIO_ACTIVE_DIRECTORY);
                string[] auxPropriedadesAD = propriedadesAD.VALOR.Split(';');

                string LDAP = auxDadosLDAP[0];
                string user_LDAP = auxDadosLDAP[1];
                string pass_LDAP = auxDadosLDAP[2];

                string nomeAD = auxPropriedadesAD[0];
                string lojaAD = auxPropriedadesAD[1];
                string gruposAD = auxPropriedadesAD[2];

                DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}", LDAP), user_LDAP, pass_LDAP);

                DirectorySearcher dePesquisa = new DirectorySearcher(de);
                dePesquisa.PropertiesToLoad.Add(nomeAD);
                dePesquisa.PropertiesToLoad.Add(lojaAD);
                dePesquisa.PropertiesToLoad.Add(gruposAD);

                dePesquisa.SearchRoot = de;
                dePesquisa.Filter = string.Format("(&(objectCategory=user)(objectClass=person)(sAMAccountName={0}))", userName);

                SearchResultCollection resultado = dePesquisa.FindAll();

                SearchResult res = resultado[0];
                result = new Usuario
                {
                    USER_NAME = userName,
                    NOME = res.Properties[nomeAD][0].ToString(),
                    LOJA = res.Properties[lojaAD][0].ToString()
                };
                if (result.NOME.Length > 50)
                {
                    result.NOME = new StringBuilder(result.NOME.Substring(0, 49).Trim()).Append(".").ToString();
                }
                int totGrupos = res.Properties[gruposAD].Count;
                String nomeGrupo;
                int indiceIgual;
                int indiceVirgula;
                for (int i = 0; i < totGrupos; i++)
                {
                    nomeGrupo = res.Properties[gruposAD][i].ToString();
                    indiceIgual = nomeGrupo.IndexOf("=", 1);
                    indiceVirgula = nomeGrupo.IndexOf(",", 1);
                    if (-1 == indiceIgual)
                    {
                        continue;
                    }
                    nomeGrupo = nomeGrupo.Substring((indiceIgual + 1), (indiceVirgula - indiceIgual) - 1);
                    result.AdicionarGrupo(nomeGrupo);
                }

                PermissaoFuncDAL.AtualizarPermissoesUsuario(result);
                PermissaoStatusDAL.AtualizarPermissoesUsuario(result);
                PermissaoCompDAL.AtualizarPermissoesUsuario(result);
                PermissaoExibicaoDAL.AtualizarPermissoesUsuario(result);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public ActionResult Login()
        {
            try
            {
                string userNameCompleto = HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(userNameCompleto))
                {
                    string[] auxUserName = userNameCompleto.Split('\\');
                    string dominio = auxUserName[0];
                    dominio = "CENCOSUD";
                    Parametro auxDominio = ParametroDAL.GetPorNome(ParametroDAL.DOMINIO_AUTENTICACAO_ACTIVE_DIRECTORY);
                    if (null != auxDominio && dominio.Equals(auxDominio.VALOR))
                    {
                        string userName = auxUserName[1];
                        Usuario usuarioLogado = MontarUsuarioLogin(userName);
                        //LOCAL
                        usuarioLogado = new Usuario
                        {
                            USER_NAME = "FIXO",
                            NOME = "Fixo da Silva",
                            LOJA = "Loja Fixa"
                        };
                        usuarioLogado.AdicionarGrupo("GGG300_Adm_Portal_CSC");
                        usuarioLogado.AdicionarGrupo("Administrador");
                        usuarioLogado.AdicionarGrupo("Mesa");
                        usuarioLogado.AdicionarGrupo("Externo");
                        PermissaoFuncDAL.AtualizarPermissoesUsuario(usuarioLogado);
                        PermissaoStatusDAL.AtualizarPermissoesUsuario(usuarioLogado);
                        PermissaoCompDAL.AtualizarPermissoesUsuario(usuarioLogado);
                        PermissaoExibicaoDAL.AtualizarPermissoesUsuario(usuarioLogado);
                        //LOCAL
                        if (null != usuarioLogado)
                        {
                            Session[USUARIO_LOGADO] = usuarioLogado;
                            return View("BemVindo");
                        }
                        else
                        {
                            return View("AcessoNegado");
                        }
                    }
                    else
                    {
                        return View("AcessoNegado");
                    }
                }
                else
                {
                    return View("AcessoNegado");
                }
            }
            catch
            {
                return View("AcessoNegado");
            }
        }

        public ActionResult AcessoNegado()
        {
            return View();
        }

        public ActionResult BemVindo()
        {
            return View();
        }

        public ActionResult PerfilUsuario()
        {
            return View();
        }

        protected bool FormularioAtivo(Int32 ID_FORMULARIO)
        {
            return FormularioDAL.GetPorAtivoPorId(ID_FORMULARIO) != null;
        }

        protected bool FormularioAtivo(Int32 ID_FORMULARIO, ref Int32? OBRIGATORIEDADE_ANEXO)
        {
            Formulario result = FormularioDAL.GetPorAtivoPorId(ID_FORMULARIO);
            if (result != null)
            {
                OBRIGATORIEDADE_ANEXO = result.OBRIGATORIEDADE_ANEXO;
            }
            return result != null;
        }

        protected JsonResult VerificarStatusAtualFormulario(Int32 ID_PREENCHIMENTO_FORMULARIO, Int32? ID_STATUS_ATUAL)
        {
            JsonResult result = null;

            if (null != ID_STATUS_ATUAL)
            {
                Int32? idStatusAtual = PreenchimentoDAL.ObterStatusAtual(ID_PREENCHIMENTO_FORMULARIO);
                if (idStatusAtual == null)
                {
                    result = Json(new { msgErro = "Falha ao tentar verificar o status atual do formulário, favor tente novamente" }, JsonRequestBehavior.AllowGet);
                }
                else if (idStatusAtual != ID_STATUS_ATUAL)
                {
                    result = Json(new { msgErro = "Operação não permitida: o status do formulário foi alterado, favor atualizar a consulta" }, JsonRequestBehavior.AllowGet);
                }
            }

            return result;
        }

        protected JsonResult VerificarDadosExcel<T>(IList<T> dados, String dadosPlural, String msgErroNenhumDado)
        {
            try
            {
                if (dados != null)
                {
                    if (dados.Count <= 0)
                    {
                        return Json(new { msgErro = msgErroNenhumDado });
                    }
                    return Json(new { dados = dados }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { msgErro = string.Format("Falha ao tentar obter {0}, favor tente novamente", dadosPlural) });
            }
            catch
            {
                return Json(new { msgErro = string.Format("Falha ao tentar obter {0}, favor entrar em contato com o suporte", dadosPlural) });
            }
        }

        protected FileResult DownloadExcel(DataTable dados, String nomeArquivo)
        {
            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(dados);

            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms, false);
            ms.Position = 0;

            return File(ms, "application/vnd.ms-excel", nomeArquivo);
        }

        [HttpPost]
        public JsonResult VerificarBloqueioFormulario(Int32 ID_PREENCHIMENTO_FORMULARIO, Char? realizarBloqueio)
        {
            JsonResult result = null;

            bool? formularioLivre = true;
            if (null != realizarBloqueio)
            {
                formularioLivre = PreenchimentoDAL.VerificarBloqueio(ID_PREENCHIMENTO_FORMULARIO, UsuarioLogado.USER_NAME, UsuarioLogado.NOME, 'S' == realizarBloqueio);
            }

            if (null == formularioLivre)
            {
                result = Json(new { msgErro = "Falha ao tentar verificar a trava lógica do formulário, favor tente novamente" }, JsonRequestBehavior.AllowGet);
            }
            else if (formularioLivre == false)
            {
                result = Json(new { msgErro = "O formulário se encontra aberto por outro usuário, favor tente novamente mais tarde" }, JsonRequestBehavior.AllowGet);
            }

            return result;
        }
    }
}