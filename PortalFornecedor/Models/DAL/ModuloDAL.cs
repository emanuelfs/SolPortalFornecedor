using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class ModuloDAL
    {
        public static IList<Modulo> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Modulo> objs = new List<Modulo>();

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

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(NOME) FROM TB_MODULO) 
						AS 'totRegistros', 

						(SELECT COUNT(NOME) 
                            FROM TB_MODULO
						        WHERE 
                                NOME collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_MODULO
						    WHERE 
                            NOME collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Modulo obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(2);
                    totRegistrosFiltro = rd.GetInt32(3);

                    obj = new Modulo
                    {
                        NOME = rd.GetString(0)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Modulo
                    {
                        NOME = rd.GetString(0)
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

        public static IList<Modulo> GetParaFuncionalidade()
        {
            IList<Modulo> objs = new List<Modulo>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				NOME

	            FROM TB_MODULO

                ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Modulo obj;
                while (rd.Read())
                {
                    obj = new Modulo
                    {
                        NOME = rd.GetString(0)
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

        public static IList<Modulo> GetParaExcel()
        {
            IList<Modulo> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT *
        
                FROM TB_MODULO
    
                ORDER BY NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<Modulo>();

                Modulo obj;

                while (rd.Read())
                {
                    obj = new Modulo
                    {
                        NOME = rd.GetString(0)
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

        public static Modulo GetPorNome(String NOME)
        {
            Modulo result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				NOME

	            FROM TB_MODULO

                WHERE NOME collate Latin1_General_CI_AI = @NOME collate Latin1_General_CI_AI";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", NOME));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Modulo
                    {
                        NOME = rd.GetString(0)
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

        public static int? Insert(Modulo obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_MODULO 
                    (NOME) VALUES 
                    (@NOME)";

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

        public static int? Update(Modulo obj, String NOME_ANTERIOR)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_MODULO SET   
                    NOME = @NOME
                WHERE NOME = @NOME_ANTERIOR";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
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

        public static int? Delete(Modulo obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_MODULO
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