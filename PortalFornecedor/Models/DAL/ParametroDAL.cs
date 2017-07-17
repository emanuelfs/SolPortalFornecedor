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
    public class ParametroDAL
    {
        public const string MINUTOS_BLOQUEIO_FORMULARIO = "MINUTOS-BLOQUEIO-FORMULARIO";
        public const string LIMITE_ARQUIVOS_FORMULARIO = "LIMITE-ARQUIVOS-FORMULARIO";
        public const string LIMITE_TAMANHO_ARQUIVO_UPLOAD = "LIMITE-TAMANHO-ARQUIVO-UPLOAD";
        public const string TIPOS_ARQUIVO_FORMULARIO = "TIPOS-ARQUIVO-FORMULARIO";
        public const string DADOS_CONEXAO_ACTIVE_DIRECTORY = "DADOS-CONEXAO-ACTIVE-DIRECTORY";
        public const string DOMINIO_AUTENTICACAO_ACTIVE_DIRECTORY = "DOMINIO-AUTENTICACAO-ACTIVE-DIRECTORY";
        public const string PROPRIEDADES_USUARIO_ACTIVE_DIRECTORY = "PROPRIEDADES-USUARIO-ACTIVE-DIRECTORY";
        public const string DADOS_ENVIO_EMAIL = "DADOS-ENVIO-EMAIL";
        public const string LIMITE_NOME_ARQUIVO_TRAMITACAO = "LIMITE_NOME_ARQUIVO_TRAMITACAO";

        public static IList<Parametro> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Parametro> objs = new List<Parametro>();

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
                    ORDER BY NOME
                    ";
                }
                else
                {
                    ordenacao = string.Format("ORDER BY {0} {1}", sortColumn, sortColumnDir);
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
                        NOME,
                        VALOR,
                        DESCRICAO,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(NOME) FROM TB_PARAMETRO) 
						AS 'totRegistros', 

						(SELECT COUNT(NOME) 
                            FROM TB_PARAMETRO
						        WHERE 
                                NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                VALOR collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                DESCRICAO collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_PARAMETRO
						    WHERE 
                            NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            VALOR collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            DESCRICAO collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Parametro obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new Parametro
                    {
                        NOME = rd.GetString(0),
                        VALOR = rd.GetString(1),
                        DESCRICAO = rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Parametro
                    {
                        NOME = rd.GetString(0),
                        VALOR = rd.GetString(1),
                        DESCRICAO = rd.GetString(2)
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

        public static IList<ParametroExcel> GetParaExcel()
        {
            IList<ParametroExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                NOME,
                VALOR,
                DESCRICAO
        
                FROM TB_PARAMETRO
    
                ORDER BY NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<ParametroExcel>();

                ParametroExcel obj;

                while (rd.Read())
                {
                    obj = new ParametroExcel
                    {
                        NOME = rd.GetString(0),
                        VALOR = rd.GetString(1),
                        DESCRICAO = rd.GetString(2)
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

        public static Parametro GetPorNome(String NOME)
        {
            Parametro result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				NOME,
                VALOR

	            FROM TB_PARAMETRO

                WHERE NOME collate Latin1_General_CI_AI = @NOME collate Latin1_General_CI_AI";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", NOME));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Parametro
                    {
                        NOME = rd.GetString(0),
                        VALOR = rd.GetString(1)
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

        public static int? Insert(Parametro obj, ref StringBuilder msgErro)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_PARAMETRO 
                    (NOME, VALOR, DESCRICAO) VALUES 
                    (@NOME, @VALOR, @DESCRICAO)";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("VALOR", obj.VALOR));
                comm.Parameters.Add(new SqlParameter("DESCRICAO", obj.DESCRICAO));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (SqlException sex)
            {
                if (2627 == sex.Number)
                {
                    msgErro.Append("Já existe um parâmetro com o nome informado");
                }
                nrLinhas = null;
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

        public static int? Update(Parametro obj, String NOME_ANTERIOR)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_PARAMETRO SET   
                    NOME = @NOME,
                    VALOR = @VALOR,
                    DESCRICAO = @DESCRICAO
                WHERE NOME = @NOME_ANTERIOR";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("VALOR", obj.VALOR));
                comm.Parameters.Add(new SqlParameter("DESCRICAO", obj.DESCRICAO));
                comm.Parameters.Add(new SqlParameter("NOME_ANTERIOR", NOME_ANTERIOR));

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

        public static int? Delete(Parametro obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_PARAMETRO
                WHERE NOME = @NOME";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));

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