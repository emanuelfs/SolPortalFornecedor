using CencosudCSCWEBMVC.Models.TO;
using CencosudCSCWEBMVC.Models.TO.Excel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class ComponenteDAL
    {
        public static IList<Componente> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Componente> objs = new List<Componente>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = @"
                    ORDER BY componente.NOME
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME asc":
                            ordenacao = "ORDER BY componente.NOME";
                            break;
                        case "NOME_TIPO_COMPONENTE asc":
                            ordenacao = "ORDER BY tipoComponente.NOME";
                            break;
                        case "NOME_FORMULARIO asc":
                            ordenacao = "ORDER BY formulario.NOME";
                            break;
                        case "NOME_TIPO_LISTA asc":
                            ordenacao = "ORDER BY tipoLista.NOME";
                            break;
                        case "NOME desc":
                            ordenacao = "ORDER BY componente.NOME DESC";
                            break;
                        case "NOME_TIPO_COMPONENTE desc":
                            ordenacao = "ORDER BY tipoComponente.NOME DESC";
                            break;
                        case "NOME_FORMULARIO desc":
                            ordenacao = "ORDER BY formulario.NOME DESC";
                            break;
                        case "NOME_TIPO_LISTA desc":
                            ordenacao = "ORDER BY tipoLista.NOME DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY componente.NOME
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
						componente.ID,
                        componente.NOME,
                        componente.DESCRICAO,
                        componente.ORDEM,
                        componente.OBRIGATORIEDADE,
                        componente.EXIBIR_NO_GRID,
                        componente.EXIBIR_NO_LANCAMENTO,
                        componente.EXIBIR_NO_ATENDIMENTO,
                        componente.EXIBIR_NA_BUSCA_AVANCADA,
                        componente.CAIXA_ALTA,
                        componente.TAMANHO,
                        tipoComponente.ID AS ID_TIPO_COMPONENTE,
                        tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
                        tipoComponente.NOME AS NOME_TIPO_COMPONENTE,
                        tipoValorComponente.ID AS ID_TIPO_VALOR_COMPONENTE,
                        tipoValorComponente.CODIGO AS CODIGO_TIPO_VALOR_COMPONENTE,
                        tipoValorComponente.NOME AS NOME_TIPO_VALOR_COMPONENTE,
                        formulario.ID AS ID_FORMULARIO,
                        formulario.CODIGO AS CODIGO_FORMULARIO,
                        formulario.NOME AS NOME_FORMULARIO,
                        tipoLista.ID AS ID_TIPO_LISTA,
                        ISNULL(CONVERT(VARCHAR(50), tipoLista.NOME), '') AS NOME_TIPO_LISTA,
                        mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                        mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                        ISNULL(CONVERT(VARCHAR, mascaraComponente.NOME), '') AS NOME_MASCARA_COMPONENTE,
                        validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                        validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
                        ISNULL(CONVERT(VARCHAR, validadorComponente.NOME), '') AS NOME_VALIDADOR_COMPONENTE,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID) FROM TB_COMPONENTE) 
						AS 'totRegistros', 

						(SELECT COUNT(componente.ID) 
                            FROM TB_COMPONENTE componente 
                            JOIN TB_TIPO_COMPONENTE tipoComponente
                                ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                            JOIN TB_TIPO_VALOR_COMPONENTE tipoValorComponente
                                ON tipoValorComponente.ID = componente.ID_TIPO_VALOR_COMPONENTE
                            JOIN TB_FORMULARIO formulario
                                ON formulario.ID = componente.ID_FORMULARIO
                            LEFT JOIN TB_TIPO_LISTA tipoLista 
                                ON tipoLista.ID = componente.ID_TIPO_LISTA
                            LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente 
                                ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                            LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente 
                                ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE
						            WHERE 
                                    componente.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    tipoComponente.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    formulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    ISNULL(CONVERT(VARCHAR(50), tipoLista.NOME), '') collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro',

                        ISNULL((SELECT MAX(ID_COMPONENTE)
		            		FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
		            			WHERE ID_COMPONENTE = componente.ID), -1)
		            	AS 'componentePreenchido'

	                	FROM TB_COMPONENTE componente 
                            JOIN TB_TIPO_COMPONENTE tipoComponente
                                ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                            JOIN TB_TIPO_VALOR_COMPONENTE tipoValorComponente
                                ON tipoValorComponente.ID = componente.ID_TIPO_VALOR_COMPONENTE
                            JOIN TB_FORMULARIO formulario
                                ON formulario.ID = componente.ID_FORMULARIO
                            LEFT JOIN TB_TIPO_LISTA tipoLista 
                                ON tipoLista.ID = componente.ID_TIPO_LISTA
                            LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente 
                                ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                            LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente 
                                ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE
						            WHERE 
                                    componente.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    tipoComponente.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    formulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                    OR
                                    ISNULL(CONVERT(VARCHAR(50), tipoLista.NOME), '') collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Componente obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(29);
                    totRegistrosFiltro = rd.GetInt32(30);

                    obj = new Componente
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        DESCRICAO = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                        ORDEM = rd.GetInt32(3),
                        OBRIGATORIEDADE = rd.GetBoolean(4) ? 1 : 0,
                        EXIBIR_NO_GRID = rd.GetBoolean(5) ? 1 : 0,
                        EXIBIR_NO_LANCAMENTO = rd.GetBoolean(6) ? 1 : 0,
                        EXIBIR_NO_ATENDIMENTO = rd.GetBoolean(7) ? 1 : 0,
                        EXIBIR_NA_BUSCA_AVANCADA = rd.GetBoolean(8) ? 1 : 0,
                        CAIXA_ALTA = rd.GetBoolean(9) ? 1 : 0,
                        TAMANHO = rd.IsDBNull(10) ? null : (Int32?)rd.GetInt32(10),
                        tipoComponente = new TipoComponente
                        {
                            ID = rd.GetInt32(11),
                            CODIGO = rd.GetInt32(12),
                            NOME = rd.GetString(13)
                        },
                        tipoValorComponente = new TipoValorComponente
                        {
                            ID = rd.GetInt32(14),
                            CODIGO = rd.GetInt32(15),
                            NOME = rd.GetString(16)
                        },
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(17),
                            CODIGO = rd.GetInt32(18),
                            NOME = rd.GetString(19)
                        },
                        tipoLista = rd.IsDBNull(20) ?
                        new TipoLista
                        {
                            NOME = rd.GetString(21)
                        }
                        : new TipoLista
                        {
                            ID = rd.GetInt32(20),
                            NOME = rd.GetString(21),
                        },
                        mascaraComponente = rd.IsDBNull(22) ?
                        new MascaraComponente
                        {
                            NOME = rd.GetString(24)
                        }
                        : new MascaraComponente
                        {
                            ID = rd.GetInt32(22),
                            CODIGO = rd.GetInt32(23),
                            NOME = rd.GetString(24),
                        },

                        validadorComponente = rd.IsDBNull(25) ?
                        new ValidadorComponente
                        {
                            NOME = rd.GetString(27)
                        }
                        : new ValidadorComponente
                        {
                            ID = rd.GetInt32(25),
                            CODIGO = rd.GetInt32(26),
                            NOME = rd.GetString(27),
                        },
                        componentePreenchido = rd.GetInt32(31) > 0 ? 1 : 0
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Componente
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        DESCRICAO = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                        ORDEM = rd.GetInt32(3),
                        OBRIGATORIEDADE = rd.GetBoolean(4) ? 1 : 0,
                        EXIBIR_NO_GRID = rd.GetBoolean(5) ? 1 : 0,
                        EXIBIR_NO_LANCAMENTO = rd.GetBoolean(6) ? 1 : 0,
                        EXIBIR_NO_ATENDIMENTO = rd.GetBoolean(7) ? 1 : 0,
                        EXIBIR_NA_BUSCA_AVANCADA = rd.GetBoolean(8) ? 1 : 0,
                        CAIXA_ALTA = rd.GetBoolean(9) ? 1 : 0,
                        TAMANHO = rd.IsDBNull(10) ? null : (Int32?)rd.GetInt32(10),
                        tipoComponente = new TipoComponente
                        {
                            ID = rd.GetInt32(11),
                            CODIGO = rd.GetInt32(12),
                            NOME = rd.GetString(13)
                        },
                        tipoValorComponente = new TipoValorComponente
                        {
                            ID = rd.GetInt32(14),
                            CODIGO = rd.GetInt32(15),
                            NOME = rd.GetString(16)
                        },
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(17),
                            CODIGO = rd.GetInt32(18),
                            NOME = rd.GetString(19)
                        },
                        tipoLista = rd.IsDBNull(20) ?
                        new TipoLista
                        {
                            NOME = rd.GetString(21)
                        }
                        : new TipoLista
                        {
                            ID = rd.GetInt32(20),
                            NOME = rd.GetString(21),
                        },
                        mascaraComponente = rd.IsDBNull(22) ?
                        new MascaraComponente
                        {
                            NOME = rd.GetString(24)
                        }
                        : new MascaraComponente
                        {
                            ID = rd.GetInt32(22),
                            CODIGO = rd.GetInt32(23),
                            NOME = rd.GetString(24),
                        },

                        validadorComponente = rd.IsDBNull(25) ?
                        new ValidadorComponente
                        {
                            NOME = rd.GetString(27)
                        }
                        : new ValidadorComponente
                        {
                            ID = rd.GetInt32(25),
                            CODIGO = rd.GetInt32(26),
                            NOME = rd.GetString(27),
                        },
                        componentePreenchido = rd.GetInt32(31) > 0 ? 1 : 0
                    };
                    objs.Add(obj);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                objs.Clear();
            }
            finally
            {
                con.Close();
            }

            return objs;
        }

        public static IList<ComponenteExcel> GetParaExcel()
        {
            IList<ComponenteExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                componente.NOME,
                componente.ORDEM,
                OBRIGATORIO = (CASE WHEN 1 = componente.OBRIGATORIEDADE THEN 'Sim' ELSE 'Não' END),
                EXIBIR_NA_LISTAGEM = (CASE WHEN 1 = componente.EXIBIR_NO_GRID THEN 'Sim' ELSE 'Não' END),
                EXIBIR_NO_LANCAMENTO = (CASE WHEN 1 = componente.EXIBIR_NO_LANCAMENTO THEN 'Sim' ELSE 'Não' END),
                EXIBIR_NO_ATENDIMENTO = (CASE WHEN 1 = componente.EXIBIR_NO_ATENDIMENTO THEN 'Sim' ELSE 'Não' END),
                EXIBIR_NA_BUSCA_AVANCADA = (CASE WHEN 1 = componente.EXIBIR_NA_BUSCA_AVANCADA THEN 'Sim' ELSE 'Não' END),
                CAIXA_ALTA = (CASE WHEN 1 = componente.CAIXA_ALTA THEN 'Sim' ELSE 'Não' END),
                DESCRICAO = ISNULL(componente.DESCRICAO, ''),
                TAMANHO = ISNULL(componente.TAMANHO, 0),
                TIPO = ISNULL(tipoComponente.NOME, ''),
                FORMULARIO = formulario.NOME,
                LISTA = ISNULL(tipoLista.NOME, ''),
                MASCARA = ISNULL(mascaraComponente.NOME, ''),
                VALIDADOR = ISNULL(validadorComponente.NOME, '')
        
                FROM TB_COMPONENTE componente 
                JOIN TB_TIPO_COMPONENTE tipoComponente
                    ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                JOIN TB_FORMULARIO formulario
                    ON formulario.ID = componente.ID_FORMULARIO
                LEFT JOIN TB_TIPO_LISTA tipoLista 
                    ON tipoLista.ID = componente.ID_TIPO_LISTA
                LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente 
                    ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente 
                    ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE
    
                ORDER BY formulario.NOME, componente.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<ComponenteExcel>();

                ComponenteExcel obj;

                while (rd.Read())
                {
                    obj = new ComponenteExcel
                    {
                        NOME = rd.GetString(0),
                        ORDEM = rd.GetInt32(1),
                        OBRIGATORIO = rd.GetString(2),
                        EXIBIR_NA_LISTAGEM = rd.GetString(3),
                        EXIBIR_NO_LANCAMENTO = rd.GetString(4),
                        EXIBIR_NO_ATENDIMENTO = rd.GetString(5),
                        EXIBIR_NA_BUSCA_AVANCADA = rd.GetString(6),
                        CAIXA_ALTA = rd.GetString(7),
                        DESCRICAO = rd.GetString(8),
                        TAMANHO = rd.GetInt32(9),
                        TIPO = rd.GetString(10),
                        FORMULARIO = rd.GetString(11),
                        LISTA = rd.GetString(12),
                        MASCARA = rd.GetString(13),
                        VALIDADOR = rd.GetString(14)
                    };
                    objs.Add(obj);
                }

                rd.Close();
            }
            catch (Exception ex)
            {
                objs = null;
            }
            finally
            {
                con.Close();
            }

            return objs;
        }

        public static IList<Componente> GetPorformulario(Int32 ID_FORMULARIO, Int32? ID_STATUS_REGRA, string gruposFiltroUsuarioLogado)
        {
            IList<Componente> objs = new List<Componente>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder();
                if (ID_STATUS_REGRA != null && null != gruposFiltroUsuarioLogado)
                {
                    queryGet.Append(@"
				    SELECT 
				    componente.ID, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
                    tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
                    tipoLista.ID AS ID_TIPO_LISTA,
                    permissaoComponente.TIPO AS TIPO_PERMISSAO_STATUS

	                FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE

                    LEFT JOIN TB_PERMISSAO_COMPONENTE permissaoComponente
					ON permissaoComponente.NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
					AND permissaoComponente.ID_STATUS = @ID_STATUS_REGRA
					AND permissaoComponente.ID_COMPONENTE = componente.ID

                    WHERE componente.ID_FORMULARIO = @ID_FORMULARIO

                    GROUP BY componente.ID, 
                    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID,
                    mascaraComponente.CODIGO,
                    validadorComponente.ID,
                    validadorComponente.CODIGO,
                    tipoComponente.CODIGO,
                    tipoLista.ID,
                    permissaoComponente.TIPO
                    
                    ORDER BY componente.ORDEM");

                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("ID_STATUS_REGRA", ID_STATUS_REGRA));
                }
                else
                {
                    queryGet.Append(@"
				    SELECT 
				    componente.ID, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
                    tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
                    tipoLista.ID AS ID_TIPO_LISTA

	                FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE

                    WHERE componente.ID_FORMULARIO = @ID_FORMULARIO
                    
                    ORDER BY componente.ORDEM");

                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                }

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Componente obj;
                while (rd.Read())
                {
                    obj = new Componente
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        DESCRICAO = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                        ORDEM = rd.GetInt32(3),
                        OBRIGATORIEDADE = rd.GetBoolean(4) ? 1 : 0,
                        EXIBIR_NO_GRID = rd.GetBoolean(5) ? 1 : 0,
                        EXIBIR_NO_LANCAMENTO = rd.GetBoolean(6) ? 1 : 0,
                        EXIBIR_NO_ATENDIMENTO = rd.GetBoolean(7) ? 1 : 0,
                        EXIBIR_NA_BUSCA_AVANCADA = rd.GetBoolean(8) ? 1 : 0,
                        CAIXA_ALTA = rd.GetBoolean(9) ? 1 : 0,
                        TAMANHO = rd.IsDBNull(10) ? null : (Int32?)rd.GetInt32(10),
                        mascaraComponente = rd.IsDBNull(11) ?
                        new MascaraComponente() : new MascaraComponente
                        {
                            ID = rd.GetInt32(11),
                            CODIGO = rd.GetInt32(12)
                        },
                        validadorComponente = rd.IsDBNull(13) ?
                        new ValidadorComponente() : new ValidadorComponente
                        {
                            ID = rd.GetInt32(13),
                            CODIGO = rd.GetInt32(14)
                        },
                        tipoComponente = new TipoComponente { CODIGO = rd.GetInt32(15) },
                        tipoLista = rd.IsDBNull(16) ?
                        new TipoLista() : new TipoLista { ID = rd.GetInt32(16) },
                        tipoPermissaoStatus =
                        ID_STATUS_REGRA != null 
                        && null != gruposFiltroUsuarioLogado 
                        && !rd.IsDBNull(17) ? rd.GetInt32(17) : (Int32?)null
                    };
                    objs.Add(obj);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                objs.Clear();
            }
            finally
            {
                con.Close();
            }

            return objs;
        }

        public static Componente GetPorformularioOrdem(Int32 ID_FORMULARIO, Int32 ORDEM)
        {
            Componente result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_COMPONENTE

                    WHERE ID_FORMULARIO = @ID_FORMULARIO
                    AND ORDEM = @ORDEM";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                comm.Parameters.Add(new SqlParameter("ORDEM", ORDEM));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Componente
                    {
                        ID = rd.GetInt32(0)
                    };
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                con.Close();
            }

            return result;
        }

        public static Componente GetPorformularioNome(Int32 ID_FORMULARIO, String NOME)
        {
            Componente result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_COMPONENTE

                    WHERE ID_FORMULARIO = @ID_FORMULARIO
                    AND NOME = @NOME";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                comm.Parameters.Add(new SqlParameter("NOME", NOME));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Componente
                    {
                        ID = rd.GetInt32(0)
                    };
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                con.Close();
            }

            return result;
        }

        public static int? Insert(Componente obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                INSERT INTO TB_COMPONENTE 
                    (NOME, 
                    DESCRICAO,
                    ORDEM,
                    OBRIGATORIEDADE,
                    EXIBIR_NO_GRID,
                    EXIBIR_NO_LANCAMENTO,
                    EXIBIR_NO_ATENDIMENTO,
                    EXIBIR_NA_BUSCA_AVANCADA,
                    CAIXA_ALTA,
                    TAMANHO,
                    ID_MASCARA_COMPONENTE,
                    ID_VALIDADOR_COMPONENTE,
                    ID_TIPO_COMPONENTE, 
                    ID_TIPO_VALOR_COMPONENTE, 
                    ID_FORMULARIO, 
                    ID_TIPO_LISTA) 
                    VALUES 
                    (@NOME, 
                    @DESCRICAO,
                    @ORDEM,
                    @OBRIGATORIEDADE,
                    @EXIBIR_NO_GRID,
                    @EXIBIR_NO_LANCAMENTO,
                    @EXIBIR_NO_ATENDIMENTO,
                    @EXIBIR_NA_BUSCA_AVANCADA,
                    @CAIXA_ALTA,
                    @TAMANHO,
                    @ID_MASCARA_COMPONENTE,
                    @ID_VALIDADOR_COMPONENTE,
                    @ID_TIPO_COMPONENTE, 
                    @ID_TIPO_VALOR_COMPONENTE, 
                    @ID_FORMULARIO, 
                    @ID_TIPO_LISTA)";

                con.Open();

                object descricaoComponente = DBNull.Value;
                if (!String.IsNullOrEmpty(obj.DESCRICAO))
                {
                    descricaoComponente = obj.DESCRICAO;
                }
                object tamanhoComponente = DBNull.Value;
                if (obj.TAMANHO != null)
                {
                    tamanhoComponente = obj.TAMANHO;
                }
                object idMascara = DBNull.Value;
                if (obj.mascaraComponente != null)
                {
                    idMascara = obj.mascaraComponente.ID;
                }
                object idValidador = DBNull.Value;
                if (obj.validadorComponente != null)
                {
                    idValidador = obj.validadorComponente.ID;
                }
                object idTipoLista = DBNull.Value;
                if (obj.tipoLista != null)
                {
                    idTipoLista = obj.tipoLista.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("DESCRICAO", descricaoComponente));
                comm.Parameters.Add(new SqlParameter("ORDEM", obj.ORDEM));
                comm.Parameters.Add(new SqlParameter("OBRIGATORIEDADE", obj.OBRIGATORIEDADE));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_GRID", obj.EXIBIR_NO_GRID));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_LANCAMENTO", obj.EXIBIR_NO_LANCAMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_ATENDIMENTO", obj.EXIBIR_NO_ATENDIMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NA_BUSCA_AVANCADA", obj.EXIBIR_NA_BUSCA_AVANCADA));
                comm.Parameters.Add(new SqlParameter("CAIXA_ALTA", obj.CAIXA_ALTA));
                comm.Parameters.Add(new SqlParameter("TAMANHO", tamanhoComponente));
                comm.Parameters.Add(new SqlParameter("ID_MASCARA_COMPONENTE", idMascara));
                comm.Parameters.Add(new SqlParameter("ID_VALIDADOR_COMPONENTE", idValidador));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_COMPONENTE", obj.tipoComponente.ID));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_VALOR_COMPONENTE", obj.tipoValorComponente.ID));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", idTipoLista));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }

        public static int? Update(Componente obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_COMPONENTE SET   
                    NOME = @NOME,
                    DESCRICAO = @DESCRICAO,
                    ORDEM = @ORDEM,
                    OBRIGATORIEDADE = @OBRIGATORIEDADE,
                    EXIBIR_NO_GRID = @EXIBIR_NO_GRID,
                    EXIBIR_NO_LANCAMENTO = @EXIBIR_NO_LANCAMENTO,
                    EXIBIR_NO_ATENDIMENTO = @EXIBIR_NO_ATENDIMENTO,
                    EXIBIR_NA_BUSCA_AVANCADA = @EXIBIR_NA_BUSCA_AVANCADA,
                    CAIXA_ALTA = @CAIXA_ALTA,
                    TAMANHO = @TAMANHO,
                    ID_MASCARA_COMPONENTE = @ID_MASCARA_COMPONENTE,
                    ID_VALIDADOR_COMPONENTE = @ID_VALIDADOR_COMPONENTE,
                    ID_TIPO_COMPONENTE = @ID_TIPO_COMPONENTE,
                    ID_TIPO_VALOR_COMPONENTE = @ID_TIPO_VALOR_COMPONENTE,
                    ID_FORMULARIO = @ID_FORMULARIO,
                    ID_TIPO_LISTA = @ID_TIPO_LISTA
                WHERE ID = @ID";

                con.Open();

                object descricaoComponente = DBNull.Value;
                if (!String.IsNullOrEmpty(obj.DESCRICAO))
                {
                    descricaoComponente = obj.DESCRICAO;
                }
                object tamanhoComponente = DBNull.Value;
                if (obj.TAMANHO != null)
                {
                    tamanhoComponente = obj.TAMANHO;
                }
                object idMascara = DBNull.Value;
                if (obj.mascaraComponente != null)
                {
                    idMascara = obj.mascaraComponente.ID;
                }
                object idValidador = DBNull.Value;
                if (obj.validadorComponente != null)
                {
                    idValidador = obj.validadorComponente.ID;
                }
                object idTipoLista = DBNull.Value;
                if (obj.tipoLista != null)
                {
                    idTipoLista = obj.tipoLista.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("DESCRICAO", descricaoComponente));
                comm.Parameters.Add(new SqlParameter("ORDEM", obj.ORDEM));
                comm.Parameters.Add(new SqlParameter("OBRIGATORIEDADE", obj.OBRIGATORIEDADE));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_GRID", obj.EXIBIR_NO_GRID));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_LANCAMENTO", obj.EXIBIR_NO_LANCAMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_ATENDIMENTO", obj.EXIBIR_NO_ATENDIMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NA_BUSCA_AVANCADA", obj.EXIBIR_NA_BUSCA_AVANCADA));
                comm.Parameters.Add(new SqlParameter("CAIXA_ALTA", obj.CAIXA_ALTA));
                comm.Parameters.Add(new SqlParameter("TAMANHO", tamanhoComponente));
                comm.Parameters.Add(new SqlParameter("ID_MASCARA_COMPONENTE", idMascara));
                comm.Parameters.Add(new SqlParameter("ID_VALIDADOR_COMPONENTE", idValidador));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_COMPONENTE", obj.tipoComponente.ID));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_VALOR_COMPONENTE", obj.tipoValorComponente.ID));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", idTipoLista));
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }

        public static int? UpdateSomenteLeitura(Componente obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_COMPONENTE SET   
                    ORDEM = @ORDEM,
                    OBRIGATORIEDADE = @OBRIGATORIEDADE,
                    EXIBIR_NO_GRID = @EXIBIR_NO_GRID,
                    EXIBIR_NO_LANCAMENTO = @EXIBIR_NO_LANCAMENTO,
                    EXIBIR_NO_ATENDIMENTO = @EXIBIR_NO_ATENDIMENTO,
                    EXIBIR_NA_BUSCA_AVANCADA = @EXIBIR_NA_BUSCA_AVANCADA,
                    CAIXA_ALTA = @CAIXA_ALTA
                WHERE ID = @ID";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ORDEM", obj.ORDEM));
                comm.Parameters.Add(new SqlParameter("OBRIGATORIEDADE", obj.OBRIGATORIEDADE));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_GRID", obj.EXIBIR_NO_GRID));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_LANCAMENTO", obj.EXIBIR_NO_LANCAMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NO_ATENDIMENTO", obj.EXIBIR_NO_ATENDIMENTO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_NA_BUSCA_AVANCADA", obj.EXIBIR_NA_BUSCA_AVANCADA));
                comm.Parameters.Add(new SqlParameter("CAIXA_ALTA", obj.CAIXA_ALTA));
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }

        public static int? Delete(Componente obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_COMPONENTE
                WHERE ID = @ID";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }
    }
}