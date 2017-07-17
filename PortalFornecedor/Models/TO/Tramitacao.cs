using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class Tramitacao
    {
        public Int32 ID { get; set; }
        public String USER_NAME_RESPONSAVEL { get; set; }
        public String NOME_RESPONSAVEL { get; set; }
        public String DATA_HORA { get; set; }
        public String OBSERVACAO { get; set; }
        public Int32 ID_PREENCHIMENTO_FORMULARIO { get; set; }
        public String ID_BUSCA_FORMULARIO { get; set; }
        public StatusFormulario statusOrigem { get; set; }
        public StatusFormulario statusDestino { get; set; }
        public IList<ArquivoTramitacao> arquivos { get; set; }
        public String LOG_ALTERACAO_COMPONENTES { get; set; }
    }
}