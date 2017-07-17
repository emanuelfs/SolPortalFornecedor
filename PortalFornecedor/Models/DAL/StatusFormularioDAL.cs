using CencosudCSCWEBMVC.Models.TO;
using CencosudCSCWEBMVC.Models.TO.Excel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class StatusFormularioDAL
    {
        public static IList<StatusFormulario> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<StatusFormulario> objs = new List<StatusFormulario>();

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
                    ORDER BY statusFormulario.NOME
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME asc":
                            ordenacao = "ORDER BY statusFormulario.NOME";
                            break;
                        case "NOME_FORMULARIO asc":
                            ordenacao = "ORDER BY formulario.NOME";
                            break;
                        case "TEXTO_INICIAL asc":
                            ordenacao = "ORDER BY CASE WHEN INICIAL = 1 THEN 'Sim' ELSE 'Não' END";
                            break;
                        case "TEXTO_RETORNO asc":
                            ordenacao = "ORDER BY CASE WHEN RETORNO = 1 THEN 'Sim' ELSE 'Não' END";
                            break;
                        case "TEXTO_ENVIAR_EMAIL asc":
                            ordenacao = "ORDER BY CASE WHEN ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END";
                            break;
                        case "NOME desc":
                            ordenacao = "ORDER BY statusFormulario.NOME DESC";
                            break;
                        case "NOME_FORMULARIO desc":
                            ordenacao = "ORDER BY formulario.NOME DESC";
                            break;
                        case "TEXTO_INICIAL desc":
                            ordenacao = "ORDER BY CASE WHEN INICIAL = 1 THEN 'Sim' ELSE 'Não' END DESC";
                            break;
                        case "TEXTO_RETORNO desc":
                            ordenacao = "ORDER BY CASE WHEN RETORNO = 1 THEN 'Sim' ELSE 'Não' END DESC";
                            break;
                        case "TEXTO_ENVIAR_EMAIL desc":
                            ordenacao = "ORDER BY CASE WHEN ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY statusFormulario.NOME
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
						statusFormulario.ID,
                        statusFormulario.NOME,
                        statusFormulario.INICIAL,
                        TEXTO_INICIAL = (CASE WHEN statusFormulario.INICIAL = 1 THEN 'Sim' ELSE 'Não' END),
                        statusFormulario.RETORNO,
                        TEXTO_RETORNO = (CASE WHEN statusFormulario.RETORNO = 1 THEN 'Sim' ELSE 'Não' END),
                        statusFormulario.ENVIAR_EMAIL,
                        TEXTO_ENVIAR_EMAIL = (CASE WHEN statusFormulario.ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END),
                        statusFormulario.TITULO_EMAIL,
                        statusFormulario.CORPO_EMAIL,
                        formulario.ID AS ID_FORMULARIO,
                        formulario.NOME AS NOME_FORMULARIO,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID) FROM TB_STATUS_FORMULARIO) 
						AS 'totRegistros', 

						(SELECT COUNT(statusFormulario.ID) 
                            FROM TB_STATUS_FORMULARIO statusFormulario JOIN TB_FORMULARIO formulario 
                            ON formulario.ID = statusFormulario.ID_FORMULARIO
						        WHERE 
                                statusFormulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                formulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN INICIAL = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN RETORNO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_STATUS_FORMULARIO statusFormulario JOIN TB_FORMULARIO formulario 
                            ON formulario.ID = statusFormulario.ID_FORMULARIO
						        WHERE 
                                statusFormulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                formulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN INICIAL = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN RETORNO = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro
                                OR (CASE WHEN ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END) collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                StatusFormulario obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(13);
                    totRegistrosFiltro = rd.GetInt32(14);

                    obj = new StatusFormulario
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        INICIAL = rd.GetBoolean(2) ? 1 : 0,
                        TEXTO_INICIAL = rd.GetString(3),
                        RETORNO = rd.GetBoolean(4) ? 1 : 0,
                        TEXTO_RETORNO = rd.GetString(5),
                        ENVIAR_EMAIL = rd.GetBoolean(6) ? 1 : 0,
                        TEXTO_ENVIAR_EMAIL = rd.GetString(7),
                        TITULO_EMAIL = rd.IsDBNull(8) ? string.Empty : rd.GetString(8),
                        CORPO_EMAIL = rd.IsDBNull(9) ? string.Empty : rd.GetString(9),
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(10),
                            NOME = rd.GetString(11),
                        }
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new StatusFormulario
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        INICIAL = rd.GetBoolean(2) ? 1 : 0,
                        TEXTO_INICIAL = rd.GetString(3),
                        RETORNO = rd.GetBoolean(4) ? 1 : 0,
                        TEXTO_RETORNO = rd.GetString(5),
                        ENVIAR_EMAIL = rd.GetBoolean(6) ? 1 : 0,
                        TEXTO_ENVIAR_EMAIL = rd.GetString(7),
                        TITULO_EMAIL = rd.IsDBNull(8) ? string.Empty : rd.GetString(8),
                        CORPO_EMAIL = rd.IsDBNull(9) ? string.Empty : rd.GetString(9),
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(10),
                            NOME = rd.GetString(11),
                        }
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

        public static IList<StatusFormularioExcel> GetParaExcel()
        {
            IList<StatusFormularioExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                statusFormulario.NOME,
                NOME_FORMULARIO = formulario.NOME,
                TEXTO_INICIAL = (CASE WHEN statusFormulario.INICIAL = 1 THEN 'Sim' ELSE 'Não' END),
                TEXTO_RETORNO = (CASE WHEN statusFormulario.RETORNO = 1 THEN 'Sim' ELSE 'Não' END),
                TEXTO_ENVIAR_EMAIL = (CASE WHEN statusFormulario.ENVIAR_EMAIL = 1 THEN 'Sim' ELSE 'Não' END),
                statusFormulario.TITULO_EMAIL,
                statusFormulario.CORPO_EMAIL
        
                FROM TB_STATUS_FORMULARIO statusFormulario 
                JOIN TB_FORMULARIO formulario ON formulario.ID = statusFormulario.ID_FORMULARIO
    
                ORDER BY formulario.NOME, statusFormulario.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<StatusFormularioExcel>();

                StatusFormularioExcel obj;

                while (rd.Read())
                {
                    obj = new StatusFormularioExcel
                    {
                        NOME = rd.GetString(0),
                        FORMULARIO = rd.GetString(1),
                        INICIAL = rd.GetString(2),
                        RETORNO = rd.GetString(3),
                        ENVIAR_EMAIL = rd.GetString(4),
                        TITULO_EMAIL = rd.IsDBNull(5) ? string.Empty : rd.GetString(5),
                        CORPO_EMAIL = rd.IsDBNull(6) ? string.Empty : rd.GetString(6),
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

        public static IList<StatusFormulario> GetPorformulario(Int32 ID_FORMULARIO, Int32? ID_STATUS_ORIGEM, String statusFiltroUsuarioLogado)
        {
            IList<StatusFormulario> objs = new List<StatusFormulario>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder();
                if (ID_STATUS_ORIGEM == null)
                {
                    queryGet.Append(@"
				    SELECT 
				    ID, 
				    NOME,
                    INICIAL,
                    RETORNO,
                    ENVIAR_EMAIL
	                FROM TB_STATUS_FORMULARIO
                    WHERE ID_FORMULARIO = @ID_FORMULARIO");
                    if (statusFiltroUsuarioLogado != null)
                    {
                        queryGet.Append(@"
                        AND ID IN (").Append(statusFiltroUsuarioLogado).Append(")");
                    }
                    queryGet.Append(@"
                    ORDER BY NOME");
                }
                else
                {
                    queryGet.Append(@"
				    SELECT 
				    tsf.ID, 
				    tsf.NOME,
                    tsf.INICIAL,
                    tsf.RETORNO,
                    tsf.ENVIAR_EMAIL
	                FROM TB_FLUXO_STATUS tfs, TB_STATUS_FORMULARIO tsf
                    WHERE tfs.ID_STATUS_ORIGEM = @ID_STATUS_ORIGEM
	                AND tsf.ID = tfs.ID_STATUS_DESTINO
                    AND tsf.ID_FORMULARIO = @ID_FORMULARIO");
                    if (statusFiltroUsuarioLogado != null)
                    {
                        queryGet.Append(@"
                        AND tsf.ID IN (").Append(statusFiltroUsuarioLogado).Append(")");
                    }
                    queryGet.Append(@"
                    ORDER BY tsf.NOME");

                    comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", ID_STATUS_ORIGEM));
                }

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                StatusFormulario obj;
                while (rd.Read())
                {
                    obj = new StatusFormulario
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        INICIAL = rd.GetBoolean(2) ? 1 : 0,
                        RETORNO = rd.GetBoolean(3) ? 1 : 0,
                        ENVIAR_EMAIL = rd.GetBoolean(4) ? 1 : 0
                    };
                    objs.Add(obj);
                }
                rd.Close();
            }
            catch (Exception)
            {
                objs.Clear();
            }
            finally
            {
                con.Close();
            }

            return objs;
        }

        public static StatusFormulario GetInicialPorFormulario(Int32 ID_FORMULARIO)
        {
            StatusFormulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_STATUS_FORMULARIO

                    WHERE INICIAL = 1
                    AND ID_FORMULARIO = @ID_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new StatusFormulario
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

        public static StatusFormulario GetRetornoPorFormulario(Int32 ID_FORMULARIO)
        {
            StatusFormulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID

	                FROM TB_STATUS_FORMULARIO

                    WHERE RETORNO = 1
                    AND ID_FORMULARIO = @ID_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new StatusFormulario
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

        public static StatusFormulario GetEmAtendimentoPorFormulario(Int32 ID_FORMULARIO, Int32 ID_STATUS_INICIAL)
        {
            StatusFormulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT MAX(tsf.ID) AS ID
	                    FROM TB_FLUXO_STATUS tfs, TB_STATUS_FORMULARIO tsf
	                    	WHERE tfs.ID_STATUS_ORIGEM = @ID_STATUS_INICIAL
	                    	AND tsf.ID = tfs.ID_STATUS_DESTINO
                            AND tsf.ID_FORMULARIO = @ID_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                comm.Parameters.Add(new SqlParameter("ID_STATUS_INICIAL", ID_STATUS_INICIAL));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new StatusFormulario
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

        public static StatusFormulario GetParaEnvioEmail(Int32 ID)
        {
            StatusFormulario result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				TITULO_EMAIL,
                CORPO_EMAIL

	            FROM TB_STATUS_FORMULARIO

                WHERE ID = @ID
                AND ENVIAR_EMAIL = 1";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", ID));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new StatusFormulario
                    {
                        TITULO_EMAIL = rd.GetString(0),
                        CORPO_EMAIL = rd.GetString(1)
                    };
                }
                else
                {
                    result = new StatusFormulario
                    {
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

        public static String GetCorpoEmailParaAlteracao(Int32 ID)
        {
            String result = string.Empty;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
                CORPO_EMAIL

	            FROM TB_STATUS_FORMULARIO

                WHERE ID = @ID
                AND ENVIAR_EMAIL = 1";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", ID));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = rd.GetString(0);
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

        public static int? Insert(StatusFormulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                INSERT INTO TB_STATUS_FORMULARIO 
                    (NOME,                    
                    ID_FORMULARIO,
                    INICIAL,
                    RETORNO,
                    ENVIAR_EMAIL,
                    TITULO_EMAIL,
                    CORPO_EMAIL) 
                    VALUES 
                    (@NOME,
                     @ID_FORMULARIO,
                     @INICIAL,
                     @RETORNO,
                     @ENVIAR_EMAIL,
                     @TITULO_EMAIL,
                     @CORPO_EMAIL)";

                con.Open();

                object auxTituloEmail = DBNull.Value;
                if (null != obj.TITULO_EMAIL)
                {
                    auxTituloEmail = obj.TITULO_EMAIL;
                }
                object auxCorpoEmail = DBNull.Value;
                if (obj.CORPO_EMAIL != null)
                {
                    auxCorpoEmail = obj.CORPO_EMAIL;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));
                comm.Parameters.Add(new SqlParameter("INICIAL", obj.INICIAL));
                comm.Parameters.Add(new SqlParameter("RETORNO", obj.RETORNO));
                comm.Parameters.Add(new SqlParameter("ENVIAR_EMAIL", obj.ENVIAR_EMAIL));
                comm.Parameters.Add(new SqlParameter("TITULO_EMAIL", auxTituloEmail));
                comm.Parameters.Add(new SqlParameter("CORPO_EMAIL", auxCorpoEmail));

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

        public static int? Update(StatusFormulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_STATUS_FORMULARIO SET   
                    NOME = @NOME,
                    ID_FORMULARIO = @ID_FORMULARIO,
                    INICIAL = @INICIAL,
                    RETORNO = @RETORNO,
                    ENVIAR_EMAIL = @ENVIAR_EMAIL,
                    TITULO_EMAIL = @TITULO_EMAIL,
                    CORPO_EMAIL = @CORPO_EMAIL
                WHERE ID = @ID";

                con.Open();

                object auxTituloEmail = DBNull.Value;
                if (null != obj.TITULO_EMAIL)
                {
                    auxTituloEmail = obj.TITULO_EMAIL;
                }
                object auxCorpoEmail = DBNull.Value;
                if (obj.CORPO_EMAIL != null)
                {
                    auxCorpoEmail = obj.CORPO_EMAIL;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));
                comm.Parameters.Add(new SqlParameter("INICIAL", obj.INICIAL));
                comm.Parameters.Add(new SqlParameter("RETORNO", obj.RETORNO));
                comm.Parameters.Add(new SqlParameter("ENVIAR_EMAIL", obj.ENVIAR_EMAIL));
                comm.Parameters.Add(new SqlParameter("TITULO_EMAIL", auxTituloEmail));
                comm.Parameters.Add(new SqlParameter("CORPO_EMAIL", auxCorpoEmail));
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                nrLinhas = null;
            }
            finally
            {
                con.Close();
            }
            return nrLinhas;
        }

        public static int? Delete(StatusFormulario obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_STATUS_FORMULARIO
                WHERE ID = @ID";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception)
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