using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public static class Util
    {
        private const int TOT_DADOS_ENVIO_EMAIL = 9;
        public static string CONNECTION_STRING = WebConfigurationManager.ConnectionStrings["PortalFornecedor"].ConnectionString;
        public const int CODIGO_TIPO_VALOR_COMPONENTE_DEFAULT = 1;

        public static bool VerificarEnvioEmailStatus(Int32 ID_STATUS, ref StatusFormulario statusEnvioEmail, ref string[] dadosEnvioEmail, ref String msgErro)
        {
            statusEnvioEmail = StatusFormularioDAL.GetParaEnvioEmail(ID_STATUS);
            if (null == statusEnvioEmail)
            {
                msgErro = "não foi possível verificar o envio de email, favor entrar em contato com o suporte";
                return false;
            }
            if (!string.IsNullOrEmpty(statusEnvioEmail.TITULO_EMAIL))
            {
                String msgPadrao = "não foi possível obter os parâmetros para o envio de email, favor entrar em contato com o suporte";
                Parametro auxDadosEnvioEmail = ParametroDAL.GetPorNome(ParametroDAL.DADOS_ENVIO_EMAIL);
                if (null == auxDadosEnvioEmail || string.IsNullOrEmpty(auxDadosEnvioEmail.VALOR))
                {
                    msgErro = msgPadrao;
                    return false;
                }
                dadosEnvioEmail = auxDadosEnvioEmail.VALOR.Split(';');
                if (null == dadosEnvioEmail || Util.TOT_DADOS_ENVIO_EMAIL != dadosEnvioEmail.Length)
                {
                    msgErro = msgPadrao;
                    return false;
                }
                msgErro = null;
                return true;
            }
            msgErro = null;
            return false;
        }

        public static String ObterEmailUsuario(String userName)
        {
            try
            {
                Parametro dadosLDAP = ParametroDAL.GetPorNome(ParametroDAL.DADOS_CONEXAO_ACTIVE_DIRECTORY);
                string[] auxDadosLDAP = dadosLDAP.VALOR.Split(';');

                Parametro propriedadesAD = ParametroDAL.GetPorNome(ParametroDAL.PROPRIEDADES_USUARIO_ACTIVE_DIRECTORY);
                string[] auxPropriedadesAD = propriedadesAD.VALOR.Split(';');

                string LDAP = auxDadosLDAP[0];
                string user_LDAP = auxDadosLDAP[1];
                string pass_LDAP = auxDadosLDAP[2];

                string emailAD = auxPropriedadesAD[3];

                DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}", LDAP), user_LDAP, pass_LDAP);

                DirectorySearcher dePesquisa = new DirectorySearcher(de);
                dePesquisa.PropertiesToLoad.Add(emailAD);

                dePesquisa.SearchRoot = de;
                dePesquisa.Filter = string.Format("(&(objectCategory=user)(objectClass=person)(sAMAccountName={0}))", userName);

                SearchResultCollection resultado = dePesquisa.FindAll();

                SearchResult res = resultado[0];

                return res.Properties[emailAD][0].ToString();
            }
            catch
            {
                return null;
            }
        }

        public static IDictionary<string, string> MontarListaCoringasEmail(string NOME_CRIADOR, string ID_BUSCA, string NOME_FORMULARIO, string DADOS_ATENDIMENTO)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            result.Add("#NOME_CRIADOR", NOME_CRIADOR);
            result.Add("#ID_BUSCA", ID_BUSCA);
            result.Add("#NOME_FORMULARIO", NOME_FORMULARIO);
            result.Add("#DADOS_ATENDIMENTO", DADOS_ATENDIMENTO);
            return result;
        }

        public static String EnviarEmail(String emailDestino, String tituloEmail, String corpoEmail, string[] dadosEnvioEmail, IDictionary<string, string> coringasEmail)
        {
            String msgErroPadrao = "Falha ao tentar enviar o email: ";
            try
            {
                if (string.IsNullOrEmpty(emailDestino)
                    || string.IsNullOrEmpty(tituloEmail)
                    || string.IsNullOrEmpty(corpoEmail)
                    || null == dadosEnvioEmail
                    || dadosEnvioEmail.Length != TOT_DADOS_ENVIO_EMAIL)
                {
                    return string.Format("{0}{1}", msgErroPadrao, " dados não informados, favor entrar em contato com o suporte");
                }
                string host = dadosEnvioEmail[0];
                int porta = Convert.ToInt32(dadosEnvioEmail[1]);
                string userNameEmail = dadosEnvioEmail[2];
                string senhaEmail = dadosEnvioEmail[3];
                string emailFrom = dadosEnvioEmail[4];
                string emailDestinoCopia = dadosEnvioEmail[5];
                string emailDestinoCopiaOculta = dadosEnvioEmail[6];
                string permitirSsl = dadosEnvioEmail[7];
                string encodingEmail = dadosEnvioEmail[8];

                if (null != coringasEmail && coringasEmail.Count > 0)
                {
                    foreach (string chave in coringasEmail.Keys)
                    {
                        if ("#DADOS_ATENDIMENTO".Equals(chave))
                        {
                            if (string.IsNullOrEmpty(coringasEmail[chave]))
                            {
                                corpoEmail = corpoEmail.Replace(chave, "Nenhuma observação informada.");
                            }
                            else
                            {
                                corpoEmail = corpoEmail.Replace(chave, coringasEmail[chave]);
                            }
                        }
                        else
                        {
                            corpoEmail = corpoEmail.Replace(chave, coringasEmail[chave]);
                        }
                    }
                }
                corpoEmail = corpoEmail.Replace("\n", "<br/>").Replace("-b-", "<b>").Replace("-/b-", "</b>");

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new System.Net.Mail.MailAddress(emailFrom);
                mailMessage.To.Add(emailDestino);
                mailMessage.Subject = tituloEmail;
                mailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding(encodingEmail);
                mailMessage.Body = corpoEmail;
                mailMessage.BodyEncoding = System.Text.Encoding.GetEncoding(encodingEmail);
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = System.Net.Mail.MailPriority.High;

                if (!string.IsNullOrEmpty(emailDestinoCopia))
                {
                    mailMessage.CC.Add(emailDestinoCopia);
                }

                if (!string.IsNullOrEmpty(emailDestinoCopiaOculta))
                {
                    mailMessage.Bcc.Add(emailDestinoCopiaOculta);
                }

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(host, porta);
                smtp.Credentials = new System.Net.NetworkCredential(userNameEmail, senhaEmail);

                smtp.EnableSsl = "S".Equals(permitirSsl);

                smtp.Send(mailMessage);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("{0}{1}", msgErroPadrao, " favor entrar em contato com o suporte");
            }
        }
    }
}