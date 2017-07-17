using System;

namespace CencosudCSCWEBMVC.Models.TO
{
    public class Componente
    {
        public Int32 ID { get; set; }
        public String NOME { get; set; }
        public Int32 ORDEM { get; set; }
        public Int32 OBRIGATORIEDADE { get; set; }
        public Int32 EXIBIR_NO_GRID { get; set; }
        public Int32 EXIBIR_NO_LANCAMENTO { get; set; }
        public Int32 EXIBIR_NO_ATENDIMENTO { get; set; }
        public Int32 EXIBIR_NA_BUSCA_AVANCADA { get; set; }
        public Int32 CAIXA_ALTA { get; set; }
        public String DESCRICAO { get; set; }
        public Int32? TAMANHO { get; set; }
        public TipoComponente tipoComponente { get; set; }
        public TipoValorComponente tipoValorComponente { get; set; }
        public Formulario formulario { get; set; }
        public TipoLista tipoLista { get; set; }
        public MascaraComponente mascaraComponente { get; set; }
        public ValidadorComponente validadorComponente { get; set; }
        public Int32 componentePreenchido { get; set; } 
        public Int32? tipoPermissaoStatus { get; set; }
    }
}