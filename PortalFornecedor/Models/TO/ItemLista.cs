using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class ItemLista
    {
        public Int32 ID { get; set; }
        public String NOME { get; set; }
        public TipoLista tipoLista { get; set; }
        public ItemLista itemPai { get; set; }
    }
}