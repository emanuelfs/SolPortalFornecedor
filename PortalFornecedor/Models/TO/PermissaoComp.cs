using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class PermissaoComp
    {
        public Grupo grupo { get; set; }
        public StatusFormulario status { get; set; }
        public Componente componente { get; set; }
        public Int32 TIPO { get; set; }
        public String TEXTO_TIPO { get; set; }
    }
}