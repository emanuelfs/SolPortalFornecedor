using System;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class Formulario
    {
        public Int32 ID { get; set; }
        public Int32 CODIGO { get; set; }
        public String NOME { get; set; }
        public String SIGLA { get; set; }
        public Int32 OBRIGATORIEDADE_ANEXO { get; set; }
        public String TEXTO_OBRIGATORIEDADE_ANEXO { get; set; }
        public Int32 EXIBIR_TODOS { get; set; }
        public String TEXTO_EXIBIR_TODOS { get; set; }
        public Int32 ATIVO { get; set; }
        public String TEXTO_ATIVO { get; set; }
    }
}