using CencosudCSCWEBMVC.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CencosudCSCWEBMVC.Controllers
{
    public class ArquivoTramitacaoController : SegurancaController
    {
        // GET: ArquivoTramitacao
        public ActionResult Erro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Visualizar(int idArquivo, string nomeArquivo)
        {
            try
            {
                byte[] bytesArquivo = ArquivoTramitacaoDAL.ObterBytes(idArquivo);
                string[] auxExtensaoArquivo = nomeArquivo.Split('.');
                string extensaoArquivo = auxExtensaoArquivo[auxExtensaoArquivo.Length - 1];
                Response.AppendHeader("Content-Disposition", "inline; filename=" + nomeArquivo);
                return File(bytesArquivo, string.Format("application/{0}", extensaoArquivo));
            }
            catch
            {
                return View("Erro");
            }
        }
    }
}