using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class PermissaoFunc
    {
        public String CAMINHO_FUNCIONALIDADE { get; set; }
        //Para consulta e CRUD
        public String FUNCIONALIDADE { get; set; }
        public Int32 POSSUI_PERMISSAO { get; set; }
        //Para consulta e CRUD
    }
}