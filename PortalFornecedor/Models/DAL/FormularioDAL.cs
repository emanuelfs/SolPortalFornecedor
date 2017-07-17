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
    public class FormularioDAL
    {
        private static String MontarFiltroExibirTodos(string gruposFiltroUsuarioLogado)
        {
            if (string.IsNullOrEmpty(gruposFiltroUsuarioLogado))
            {
                return "tf.EXIBIR_TODOS";
            }
            StringBuilder result = new StringBuilder(@"CONVERT(BIT,
            (
                CASE WHEN 
                tf.EXIBIR_TODOS = 1 OR
                (
                    SELECT COUNT(ID_FORMULARIO)
                        FROM TB_PERMISSAO_EXIBICAO
                            WHERE ID_FORMULARIO = tf.ID
                            AND NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
                ) > 0 THEN 1 ELSE 0 END
            ))");
            return result.ToString();
        }

        public static IList<Formulario> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Formulario> objs = new List<Formulario>();

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
						ID,
                        CODIGO,
                        NOME,
                        SIGLA,
                        ATIVO,
                        TEXTO_ATIVO = (CASE WHEN 1 = ATIVO THEN 'Sim' ELSE 'Não' END),
                        OBRIGATORIEDADE_ANEXO,
                        TEXTO_OBRIGATORIEDADE_ANEXO = (CASE WHEN 1 = OBRIGATORIEDADE_ANEXO THEN 'Sim' ELSE 'Não' END),
                        EXIBIR_TODOS,
                        TEXTO_EXIBIR_TODOS = (CASE WHEN 1 = EXIBIR_TODOS THEN 'Sim' ELSE 'Não' END),

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID) FROM TB_FORMULARIO) 
						AS 'totRegistros', 

						(SELECT COUNT(ID) FROM TB_FORMULARIO 
						    WHERE 
                            NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            CODIGO like @textoFiltro
                            OR
                            SIGLA collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN ATIVO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN OBRIGATORIEDADE_ANEXO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN EXIBIR_TODOS = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_FORMULARIO
							WHERE 
                            NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            CODIGO like @textoFiltro
                            OR
                            SIGLA collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN ATIVO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN OBRIGATORIEDADE_ANEXO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                            OR (CASE WHEN EXIBIR_TODOS = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Formulario obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(11);
                    totRegistrosFiltro = rd.GetInt32(12);

                    obj = new Formulario
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2),
                        SIGLA = rd.GetString(3),
                        ATIVO = rd.GetBoolean(4) ? 1 : 0,
                        TEXTO_ATIVO = rd.GetString(5),
                        OBRIGATORIEDADE_ANEXO = rd.GetBoolean(6) ? 1 : 0,
                        TEXTO_OBRIGATORIEDADE_ANEXO = rd.GetString(7),
                        EXIBIR_TODOS = rd.GetBoolean(8) ? 1 : 0,
                        TEXTO_EXIBIR_TODOS = rd.GetString(9)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Formulario
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2),
                        SIGLA = rd.GetString(3),
                        ATIVO = rd.GetBoolean(4) ? 1 : 0,
                        TEXTO_ATIVO = rd.GetString(5),
                        OBRIGATORIEDADE_ANEXO = rd.GetBoolean(6) ? 1 : 0,
                        TEXTO_OBRIGATORIEDADE_ANEXO = rd.GetString(7),
                        EXIBIR_TODOS = rd.GetBoolean(8) ? 1 : 0,
                        TEXTO_EXIBIR_TODOS = rd.GetString(9)
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

        public static IList<FormularioExcel> GetParaExcel()
        {
            IList<FormularioExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                CODIGO,
                NOME,
                SIGLA,
                TEXTO_ATIVO = (CASE WHEN 1 = ATIVO THEN 'Sim' ELSE 'Não' END),
                TEXTO_OBRIGATORIEDADE_ANEXO = (CASE WHEN 1 = OBRIGATORIEDADE_ANEXO THEN 'Sim' ELSE 'Não' END),
                TEXTO_EXIBIR_TODOS = (CASE WHEN 1 = EXIBIR_TODOS THEN 'Sim' ELSE 'Não' END)
        
                FROM TB_FORMULARIO
    
                ORDER BY NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<FormularioExcel>();

                FormularioExcel obj;

                while (rd.Read())
                {
                    obj = new FormularioExcel
                    {
                        CODIGO = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        SIGLA = rd.GetString(2),
                        ATIVO = rd.GetString(3),
                        ANEXO_OBRIGATORIO = rd.GetString(4),
                        EXIBIR_TODOS = rd.GetString(5),
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

        public static IList<Formulario> GetParaComponente()
        {
            IList<Formulario> objs = new List<Formulario>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    CODIGO, 
				    NOME,
				    SIGLA

	                FROM TB_FORMULARIO

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Formulario obj;
                while (rd.Read())
                {
                    obj = new Formulario
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2),
                        SIGLA = rd.GetString(3)
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

        public static IList<Formulario> GetParaHome(bool somenteAtivos, string statusFiltroUsuarioLogado, string gruposFiltroUsuarioLogado)
        {
            IList<Formulario> objs = new List<Formulario>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder(@"
				SELECT 
				tf.ID, 
				tf.CODIGO, 
				tf.NOME,
				tf.SIGLA,
                tf.OBRIGATORIEDADE_ANEXO,
                EXIBIR_TODOS = ").Append(MontarFiltroExibirTodos(gruposFiltroUsuarioLogado)).Append(@"

	            FROM TB_FORMULARIO tf");

                if (somenteAtivos)
                {
                    queryGet.Append(" WHERE tf.ATIVO = 1");
                }
                if (!string.IsNullOrEmpty(statusFiltroUsuarioLogado))
                {
                    if (somenteAtivos)
                    {
                        queryGet.Append(" AND EXISTS");
                    }
                    else
                    {
                        queryGet.Append(" WHERE EXISTS");
                    }
                    queryGet.Append(@"
                    (
                        SELECT ID 
				        FROM TB_STATUS_FORMULARIO 
				    	WHERE ID_FORMULARIO = tf.ID
				    	AND ID IN (").Append(statusFiltroUsuarioLogado).Append(@")
                    )");
                }
                queryGet.Append(" ORDER BY tf.NOME");

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Formulario obj;
                while (rd.Read())
                {
                    obj = new Formulario
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2),
                        SIGLA = rd.GetString(3),
                        OBRIGATORIEDADE_ANEXO = rd.GetBoolean(4) ? 1 : 0,
                        EXIBIR_TODOS = rd.GetBoolean(5) ? 1 : 0
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

        public static Formulario GetPorCodigo(Int32 CODIGO)
        {
            Formulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_FORMULARIO

                    WHERE CODIGO = @CODIGO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("CODIGO", CODIGO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Formulario
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

        public static Formulario GetPorSigla(String SIGLA)
        {
            Formulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_FORMULARIO

                    WHERE SIGLA = @SIGLA";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("SIGLA", SIGLA));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Formulario
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

        public static Formulario GetPorAtivoPorId(Int32 ID)
        {
            Formulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID,
                    OBRIGATORIEDADE_ANEXO

	                FROM TB_FORMULARIO

                    WHERE ID = @ID
                    AND ATIVO = 1";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", ID));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Formulario
                    {
                        ID = rd.GetInt32(0),
                        OBRIGATORIEDADE_ANEXO = rd.GetBoolean(1) ? 1 : 0
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

        public static int? Insert(Formulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_FORMULARIO 
                    (NOME, CODIGO, SIGLA, OBRIGATORIEDADE_ANEXO, EXIBIR_TODOS, ATIVO) VALUES 
                    (@NOME, @CODIGO, @SIGLA, @OBRIGATORIEDADE_ANEXO, @EXIBIR_TODOS, 0)";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("CODIGO", obj.CODIGO));
                comm.Parameters.Add(new SqlParameter("SIGLA", obj.SIGLA));
                comm.Parameters.Add(new SqlParameter("OBRIGATORIEDADE_ANEXO", obj.OBRIGATORIEDADE_ANEXO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_TODOS", obj.EXIBIR_TODOS));

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

        public static int? Update(Formulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_FORMULARIO SET   
                    NOME = @NOME,
                    CODIGO = @CODIGO,
                    SIGLA = @SIGLA,
                    OBRIGATORIEDADE_ANEXO = @OBRIGATORIEDADE_ANEXO,
                    EXIBIR_TODOS = @EXIBIR_TODOS,
                    ATIVO = @ATIVO
                WHERE ID = @ID";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("CODIGO", obj.CODIGO));
                comm.Parameters.Add(new SqlParameter("SIGLA", obj.SIGLA));
                comm.Parameters.Add(new SqlParameter("ATIVO", obj.ATIVO));
                comm.Parameters.Add(new SqlParameter("OBRIGATORIEDADE_ANEXO", obj.OBRIGATORIEDADE_ANEXO));
                comm.Parameters.Add(new SqlParameter("EXIBIR_TODOS", obj.EXIBIR_TODOS));
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

        public static int? Delete(Formulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_FORMULARIO
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