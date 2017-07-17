using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class GrupoDAL
    {
        public static IList<Grupo> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Grupo> objs = new List<Grupo>();

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

						(SELECT COUNT(NOME) FROM TB_GRUPO) 
						AS 'totRegistros', 

						(SELECT COUNT(NOME) 
                            FROM TB_GRUPO
						        WHERE 
                                NOME collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_GRUPO
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

                Grupo obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(2);
                    totRegistrosFiltro = rd.GetInt32(3);

                    obj = new Grupo
                    {
                        NOME = rd.GetString(0)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Grupo
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

        public static IList<Grupo> GetParaPermissao()
        {
            IList<Grupo> objs = new List<Grupo>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				NOME

	            FROM TB_GRUPO

                ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Grupo obj;
                while (rd.Read())
                {
                    obj = new Grupo
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

        public static IList<Grupo> GetParaExcel()
        {
            IList<Grupo> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT *
        
                FROM TB_GRUPO
    
                ORDER BY NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<Grupo>();

                Grupo obj;

                while (rd.Read())
                {
                    obj = new Grupo
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

        public static Grupo GetPorNome(String NOME)
        {
            Grupo result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				NOME

	            FROM TB_GRUPO

                WHERE NOME collate Latin1_General_CI_AI = @NOME collate Latin1_General_CI_AI";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME", NOME));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Grupo
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

        public static int? Insert(Grupo obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_GRUPO 
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

        public static int? Update(Grupo obj, String NOME_ANTERIOR)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_GRUPO SET   
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

        public static int? Delete(Grupo obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_GRUPO
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