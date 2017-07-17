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
    public class FuncionalidadeDAL
    {
        public static IList<Funcionalidade> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<Funcionalidade> objs = new List<Funcionalidade>();

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
                        NOME_MODULO,
                        CAMINHO,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(CAMINHO) FROM TB_FUNCIONALIDADE) 
						AS 'totRegistros', 

						(SELECT COUNT(CAMINHO) 
                            FROM TB_FUNCIONALIDADE
						        WHERE 
                                NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                NOME_MODULO collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                CAMINHO collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_FUNCIONALIDADE
						    WHERE 
                            NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            NOME_MODULO collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            CAMINHO collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Funcionalidade obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new Funcionalidade
                    {
                        NOME = rd.GetString(0),
                        modulo = new Modulo
                        {
                            NOME = rd.GetString(1)
                        },
                        CAMINHO = rd.GetString(2)
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new Funcionalidade
                    {
                        NOME = rd.GetString(0),
                        modulo = new Modulo
                        {
                            NOME = rd.GetString(1)
                        },
                        CAMINHO = rd.GetString(2)
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

        public static IList<FuncionalidadeExcel> GetParaExcel()
        {
            IList<FuncionalidadeExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                NOME,
                CAMINHO,
                NOME_MODULO
        
                FROM TB_FUNCIONALIDADE
    
                ORDER BY NOME_MODULO, NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<FuncionalidadeExcel>();

                FuncionalidadeExcel obj;

                while (rd.Read())
                {
                    obj = new FuncionalidadeExcel
                    {
                        NOME = rd.GetString(0),
                        CAMINHO = rd.GetString(1),
                        MODULO = rd.GetString(2)
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

        public static Funcionalidade GetPorCaminho(String CAMINHO)
        {
            Funcionalidade result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				CAMINHO

	            FROM TB_FUNCIONALIDADE

                WHERE CAMINHO collate Latin1_General_CI_AI = @CAMINHO collate Latin1_General_CI_AI";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("CAMINHO", CAMINHO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new Funcionalidade
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

        public static int? Insert(Funcionalidade obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_FUNCIONALIDADE 
                    (CAMINHO, NOME, NOME_MODULO) VALUES 
                    (@CAMINHO, @NOME, @NOME_MODULO)";

                con.Open();

                comm.Parameters.Add(new SqlParameter("CAMINHO", obj.CAMINHO));
                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("NOME_MODULO", obj.modulo.NOME));

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

        public static int? Update(Funcionalidade obj, String CAMINHO_ANTERIOR)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_FUNCIONALIDADE SET   
                    CAMINHO = @CAMINHO, 
                    NOME = @NOME, 
                    NOME_MODULO = @NOME_MODULO
                WHERE CAMINHO = @CAMINHO_ANTERIOR";

                con.Open();

                comm.Parameters.Add(new SqlParameter("CAMINHO", obj.CAMINHO));
                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("NOME_MODULO", obj.modulo.NOME));
                comm.Parameters.Add(new SqlParameter("CAMINHO_ANTERIOR", CAMINHO_ANTERIOR));

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

        public static int? Delete(Funcionalidade obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_FUNCIONALIDADE
                WHERE CAMINHO = @CAMINHO";

                con.Open();

                comm.Parameters.Add(new SqlParameter("CAMINHO", obj.CAMINHO));

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