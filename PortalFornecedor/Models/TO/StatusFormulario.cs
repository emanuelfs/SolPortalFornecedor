using System;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class StatusFormulario
    {
        public Int32 ID { get; set; }
        public String NOME { get; set; }
        public Int32 ID_FORMULARIO { get; set; }
        public Int32 INICIAL { get; set; }
        public String TEXTO_INICIAL { get; set; }
        public Int32 RETORNO { get; set; }
        public String TEXTO_RETORNO { get; set; }
        public Int32 ENVIAR_EMAIL { get; set; }
        public String TEXTO_ENVIAR_EMAIL { get; set; }
        public String TITULO_EMAIL { get; set; }
        public String CORPO_EMAIL { get; set; }
        public Formulario formulario { get; set; }
    }
}