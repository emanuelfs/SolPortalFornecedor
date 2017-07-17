using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class TipoLista
    {
        public Int32 ID { get; set; }
        public String NOME { get; set; }
        public TipoLista listaPai { get; set; }
    }
}