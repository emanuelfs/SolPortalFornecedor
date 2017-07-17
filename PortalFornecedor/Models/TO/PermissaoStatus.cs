using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class PermissaoStatus
    {
        public Int32 ID_STATUS { get; set; }
        //Para consulta e CRUD
        public String NOME_STATUS { get; set; }
        public Int32 POSSUI_PERMISSAO { get; set; }
        //Para consulta e CRUD
    }
}