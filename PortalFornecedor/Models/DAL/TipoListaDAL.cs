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
    public class TipoListaDAL
    {
        public static IList<TipoLista> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<TipoLista> objs = new List<TipoLista>();

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
                    ORDER BY tipoLista.NOME
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME asc":
                            ordenacao = "ORDER BY tipoLista.NOME";
                            break;
                        case "NOME_PAI asc":
                            ordenacao = "ORDER BY listaPai.NOME";
                            break;
                        case "NOME desc":
                            ordenacao = "ORDER BY tipoLista.NOME DESC";
                            break;
                        case "NOME_PAI desc":
                            ordenacao = "ORDER BY listaPai.NOME DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY tipoLista.NOME
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
						tipoLista.ID,
                        tipoLista.NOME,
						listaPai.ID AS ID_PAI,
                        listaPai.NOME AS NOME_PAI,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID) FROM TB_TIPO_LISTA) 
						AS 'totRegistros', 

						(SELECT COUNT(tipoLista.ID) 
                            FROM TB_TIPO_LISTA tipoLista
                            LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
						        WHERE 
                                tipoLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                ISNULL(CONVERT(VARCHAR, listaPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_TIPO_LISTA tipoLista
                        LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
						    WHERE 
                            tipoLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            ISNULL(CONVERT(VARCHAR, listaPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                TipoLista obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(5);
                    totRegistrosFiltro = rd.GetInt32(6);

                    obj = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        listaPai = rd.IsDBNull(2) ?
                        new TipoLista
                        {
                            NOME = string.Empty
                        }
                        : new TipoLista
                        {
                            ID = rd.GetInt32(2),
                            NOME = rd.GetString(3),
                        }
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        listaPai = rd.IsDBNull(2) ?
                        new TipoLista
                        {
                            NOME = string.Empty
                        }
                        : new TipoLista
                        {
                            ID = rd.GetInt32(2),
                            NOME = rd.GetString(3),
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

        public static IList<TipoListaExcel> GetParaExcel()
        {
            IList<TipoListaExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                tipoLista.NOME,
                LISTA_VINCULADA = listaPai.NOME
        
                FROM TB_TIPO_LISTA tipoLista
                LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
    
                ORDER BY tipoLista.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<TipoListaExcel>();

                TipoListaExcel obj;

                while (rd.Read())
                {
                    obj = new TipoListaExcel
                    {
                        NOME = rd.GetString(0),
                        LISTA_VINCULADA = rd.IsDBNull(1) ? string.Empty : rd.GetString(1)
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

        public static IList<TipoLista> GetParaItem()
        {
            IList<TipoLista> objs = new List<TipoLista>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    NOME

	                FROM TB_TIPO_LISTA

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                TipoLista obj;
                while (rd.Read())
                {
                    obj = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1)
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

        public static IList<TipoLista> GetParaSubLista(Int32 ID)
        {
            IList<TipoLista> objs = new List<TipoLista>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    NOME

	                FROM TB_TIPO_LISTA
                    WHERE ID != @ID

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", ID));

                SqlDataReader rd = comm.ExecuteReader();

                TipoLista obj;
                while (rd.Read())
                {
                    obj = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1)
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

        public static IList<TipoLista> GetSublistasPai(Int32 ID_PAI)
        {
            IList<TipoLista> objs = new List<TipoLista>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    NOME

	                FROM TB_TIPO_LISTA
                    WHERE ID_PAI = @ID_PAI

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));

                SqlDataReader rd = comm.ExecuteReader();

                TipoLista obj;
                while (rd.Read())
                {
                    obj = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1)
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

        public static TipoLista GetPorId(Int32 ID)
        {
            TipoLista result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID,
                    ID_PAI

	                FROM TB_TIPO_LISTA

                    WHERE ID = @ID";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", ID));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new TipoLista
                    {
                        ID = rd.GetInt32(0),
                        listaPai = rd.IsDBNull(1) ? new TipoLista() :
                        new TipoLista
                        {
                            ID = rd.GetInt32(1)
                        }
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

        public static int? Insert(TipoLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_TIPO_LISTA 
                    (NOME, ID_PAI) VALUES 
                    (@NOME, @ID_PAI)";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (null != obj.listaPai)
                {
                    ID_PAI = obj.listaPai.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));

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

        public static int? Update(TipoLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_TIPO_LISTA SET   
                    NOME = @NOME,
                    ID_PAI = @ID_PAI
                WHERE ID = @ID";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (null != obj.listaPai)
                {
                    ID_PAI = obj.listaPai.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));
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

        public static int? Delete(TipoLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_TIPO_LISTA
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