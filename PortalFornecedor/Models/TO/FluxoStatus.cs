using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class FluxoStatus
    {
        public StatusFormulario statusOrigem { get; set; }
        public StatusFormulario statusDestino { get; set; }
        public Int32 TOT_DESTINOS_ORIGEM { get; set; }//Para a lógica do relatório
        public Int32 TOT_DESTINOS_DESTINO { get; set; }//Para a lógica do relatório
    }
}