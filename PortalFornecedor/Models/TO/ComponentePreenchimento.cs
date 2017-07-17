using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class ComponentePreenchimento
    {
        public Int32 ID { get; set; }
        public String VALOR_VARCHAR { get; set; }
        public Preenchimento preenchimento { get; set; }
        public Componente componente { get; set; }
        public ItemLista itemLista { get; set; }
    }
}