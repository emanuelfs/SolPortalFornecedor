using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class ArquivoTramitacao
    {
        public Int32 ID { get; set; }
        public string NOME { get; set; }
        public byte[] ARQUIVO { get; set; }
    }
}