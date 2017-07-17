using System;

namespace CencosudCSCWEBMVC.Models.TO.Excel
{
    public class ComponenteExcel
    {
        public String NOME { get; set; }
        public Int32 ORDEM { get; set; }
        public String OBRIGATORIO { get; set; }
        public String EXIBIR_NA_LISTAGEM { get; set; }
        public String EXIBIR_NO_LANCAMENTO { get; set; }
        public String EXIBIR_NO_ATENDIMENTO { get; set; }
        public String EXIBIR_NA_BUSCA_AVANCADA { get; set; }
        public String CAIXA_ALTA { get; set; }
        public String DESCRICAO { get; set; }
        public Int32 TAMANHO { get; set; }
        public String TIPO { get; set; }
        public String FORMULARIO { get; set; }
        public String LISTA { get; set; }
        public String MASCARA { get; set; }
        public String VALIDADOR { get; set; }
    }
}