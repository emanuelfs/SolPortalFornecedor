using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO.Excel
{
    public class StatusFormularioExcel
    {
        public String NOME { get; set; }
        public String FORMULARIO { get; set; }
        public String INICIAL { get; set; }
        public String RETORNO { get; set; }
        public String ENVIAR_EMAIL { get; set; }
        public String TITULO_EMAIL { get; set; }
        public String CORPO_EMAIL { get; set; }
    }
}