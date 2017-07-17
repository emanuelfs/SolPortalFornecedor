using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class Preenchimento
    {
        public Int32 ID { get; set; }
        public Int32 ID_FORMULARIO { get; set; }
        public IList<ComponentePreenchimento> componentesPreenchimento { get; set; }

        public Preenchimento()
        {
            this.componentesPreenchimento = new List<ComponentePreenchimento>();
        }
    }
}