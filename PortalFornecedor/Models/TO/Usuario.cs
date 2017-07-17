using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class Usuario
    {
        private ISet<String> gruposUsuario { get; }
        private IDictionary<String, String> permissoesUsuario { get; }
        private IDictionary<Int32, String> permissoesStatusUsuario { get; }
        private ISet<String> permissoesComponentesUsuario { get; }
        private ISet<String> permissoesExibicoesUsuario { get; }

        public String USER_NAME { get; set; }
        public String NOME { get; set; }
        public String LOJA { get; set; }

        public Usuario()
        {
            this.gruposUsuario = new HashSet<String>();
            this.permissoesUsuario = new Dictionary<String, String>();
            this.permissoesStatusUsuario = new Dictionary<Int32, String>();
            this.permissoesComponentesUsuario = new HashSet<String>();
            this.permissoesExibicoesUsuario = new HashSet<String>();
        }

        public void AdicionarGrupo(string nomeGrupo)
        {
            if (this.gruposUsuario != null)
            {
                this.gruposUsuario.Add(nomeGrupo);
            }
        }

        public void AdicionarPermissao(string caminhoFunc, string moduloFunc)
        {
            if (this.permissoesUsuario != null)
            {
                this.permissoesUsuario.Add(caminhoFunc, moduloFunc);
            }
        }

        public void AdicionarPermissaoStatus(int idStatus, string nomeStatus)
        {
            if (this.permissoesStatusUsuario != null)
            {
                this.permissoesStatusUsuario.Add(idStatus, nomeStatus);
            }
        }

        public void AdicionarPermissaoComponente(string nomeComponente)
        {
            if (this.permissoesComponentesUsuario != null)
            {
                this.permissoesComponentesUsuario.Add(nomeComponente);
            }
        }

        public void AdicionarPermissaoExibicao(string nomeFormulario)
        {
            if (this.permissoesExibicoesUsuario != null)
            {
                this.permissoesExibicoesUsuario.Add(nomeFormulario);
            }
        }

        public void LimparPermissoes()
        {
            if (this.permissoesUsuario != null)
            {
                this.permissoesUsuario.Clear();
            }
        }

        public void LimparPermissoesStatus()
        {
            if (this.permissoesStatusUsuario != null)
            {
                this.permissoesStatusUsuario.Clear();
            }
        }

        public void LimparPermissoesComponentes()
        {
            if (this.permissoesComponentesUsuario != null)
            {
                this.permissoesComponentesUsuario.Clear();
            }
        }

        public void LimparPermissoesExibicoes()
        {
            if (this.permissoesExibicoesUsuario != null)
            {
                this.permissoesExibicoesUsuario.Clear();
            }
        }

        public bool PossuiPermissao(string caminhoFunc)
        {
            if (this.permissoesUsuario != null)
            {
                return this.permissoesUsuario.ContainsKey(caminhoFunc);
            }
            return false;
        }

        public string ObterGruposParaFiltro()
        {
            StringBuilder result = new StringBuilder();
            if (this.gruposUsuario != null && this.gruposUsuario.Count > 0)
            {
                IEnumerator<String> nomesGrupos = this.gruposUsuario.GetEnumerator();
                nomesGrupos.MoveNext();
                String nomeGrupos = nomesGrupos.Current;
                result.Append(string.Format("'{0}'", nomeGrupos));
                while (nomesGrupos.MoveNext())
                {
                    nomeGrupos = nomesGrupos.Current;
                    result.Append(",").Append(string.Format("'{0}'", nomeGrupos));
                }
            }
            else
            {
                result.Append("''");
            }
            return result.ToString();
        }

        public string ObterStatusParaFiltro()
        {
            StringBuilder result = new StringBuilder();
            if (this.permissoesStatusUsuario != null && this.permissoesStatusUsuario.Count > 0)
            {
                IEnumerator<Int32> idsStatus = this.permissoesStatusUsuario.Keys.GetEnumerator();
                idsStatus.MoveNext();
                Int32 idStatus = idsStatus.Current;
                result.Append(idStatus);
                while (idsStatus.MoveNext())
                {
                    idStatus = idsStatus.Current;
                    result.Append(",").Append(idStatus);
                }
            }
            else
            {
                result.Append("''");
            }
            return result.ToString();
        }

        public string MontarGruposPerfil()
        {
            StringBuilder result = new StringBuilder();
            if (this.gruposUsuario != null && this.gruposUsuario.Count > 0)
            {
                foreach (String nomeGrupo in this.gruposUsuario.OrderBy(nome => nome.ToString()))
                {
                    result.Append(string.Format("<br/>{0}", nomeGrupo));
                }
            }
            else
            {
                result.Append("<br/>Nenhum grupo foi encontrado");
            }
            return result.ToString();
        }

        public string MontarPermissoesPerfil()
        {
            StringBuilder result = new StringBuilder();
            if (this.permissoesUsuario != null && this.permissoesUsuario.Count > 0)
            {
                foreach (String moduloFunc in this.permissoesUsuario.Values.OrderBy(molFunc => molFunc.Split('/')[0])
                    .OrderBy(molFunc => molFunc.Split('/')[1]))
                {
                    result.Append(string.Format("<br/>{0}", moduloFunc));
                }
            }
            else
            {
                result.Append("<br/>Nenhuma permissão foi encontrada");
            }
            return result.ToString();
        }

        public string MontarPermissoesStatusPerfil()
        {
            StringBuilder result = new StringBuilder();
            if (this.permissoesStatusUsuario != null && this.permissoesStatusUsuario.Count > 0)
            {
                foreach (String nomeStatus in this.permissoesStatusUsuario.Values.OrderBy(status => status.Split('/')[0])
                    .OrderBy(status => status.Split('/')[1]))
                {
                    result.Append(string.Format("<br/>{0}", nomeStatus));
                }
            }
            else
            {
                result.Append("<br/>Nenhuma permissão foi encontrada");
            }
            return result.ToString();
        }

        public string MontarPermissoesComponentesPerfil()
        {
            StringBuilder result = new StringBuilder();
            if (this.permissoesComponentesUsuario != null && this.permissoesComponentesUsuario.Count > 0)
            {
                foreach (String nomeComponente in this.permissoesComponentesUsuario.OrderBy(status => status.Split('/')[0])
                    .OrderBy(status => status.Split('/')[1])
                    .OrderBy(status => status.Split(' ')[1]))
                {
                    result.Append(string.Format("<br/>{0}", nomeComponente));
                }
            }
            else
            {
                result.Append("<br/>Nenhuma permissão foi encontrada");
            }
            return result.ToString();
        }

        public string MontarPermissoesExibicoesPerfil()
        {
            StringBuilder result = new StringBuilder();
            if (this.permissoesExibicoesUsuario != null && this.permissoesExibicoesUsuario.Count > 0)
            {
                foreach (String nomeFormulario in this.permissoesExibicoesUsuario.OrderBy(nmFormulario => nmFormulario))
                {
                    result.Append(string.Format("<br/>{0}", nomeFormulario));
                }
            }
            else
            {
                result.Append("<br/>Nenhuma permissão foi encontrada");
            }
            return result.ToString();
        }
    }
}