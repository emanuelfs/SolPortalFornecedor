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
    public class PermissaoExibicaoDAL
    {
        public static IList<PermissaoExibicao> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<PermissaoExibicao> objs = new List<PermissaoExibicao>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = "ORDER BY tpe.NOME_GRUPO";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME_GRUPO asc":
                            ordenacao = "ORDER BY tpe.NOME_GRUPO";
                            break;
                        case "NOME_FORMULARIO asc":
                            ordenacao = "ORDER BY tf.NOME";
                            break;
                        case "NOME_GRUPO desc":
                            ordenacao = "ORDER BY tpe.NOME_GRUPO DESC";
                            break;
                        case "NOME_FORMULARIO desc":
                            ordenacao = "ORDER BY tf.NOME DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY tpe.NOME_GRUPO
                            ";
                            break;
                    }
                }
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                FROM (
				    SELECT 
                    tpe.NOME_GRUPO,
                    tpe.ID_FORMULARIO,
                    tf.NOME AS NOME_FORMULARIO,

                    (ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))
                    AS 'numeroLinha', 

                    (SELECT COUNT(NOME_GRUPO) FROM TB_PERMISSAO_EXIBICAO) 
				    AS 'totRegistros', 

					(SELECT COUNT(tpe.NOME_GRUPO)
                        FROM TB_PERMISSAO_EXIBICAO tpe
                        JOIN TB_FORMULARIO tf ON tf.ID = tpe.ID_FORMULARIO
					        WHERE
                            tpe.NOME_GRUPO collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            tf.NOME collate Latin1_General_CI_AI like @textoFiltro
                    ) 
					AS 'totRegistrosFiltro'

	                FROM TB_PERMISSAO_EXIBICAO tpe
                    JOIN TB_FORMULARIO tf ON tf.ID = tpe.ID_FORMULARIO
					    WHERE
                        tpe.NOME_GRUPO collate Latin1_General_CI_AI like @textoFiltro
                        OR
                        tf.NOME collate Latin1_General_CI_AI like @textoFiltro) 

				AS todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                PermissaoExibicao obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(4);
                    totRegistrosFiltro = rd.GetInt32(5);

                    obj = new PermissaoExibicao
                    {
                        grupo = new Grupo
                        {
                            NOME = rd.GetString(0)
                        },
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(1),
                            NOME = rd.GetString(2)
                        }
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new PermissaoExibicao
                    {
                        grupo = new Grupo
                        {
                            NOME = rd.GetString(0)
                        },
                        formulario = new Formulario
                        {
                            ID = rd.GetInt32(1),
                            NOME = rd.GetString(2)
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

        public static IList<PermissaoExibicaoExcel> GetParaExcel()
        {
            IList<PermissaoExibicaoExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT
                tpe.NOME_GRUPO,
                NOME_FORMULARIO = tf.NOME
                
                FROM TB_PERMISSAO_EXIBICAO tpe
                JOIN TB_FORMULARIO tf ON tf.ID = tpe.ID_FORMULARIO
                
                ORDER BY tpe.NOME_GRUPO, tf.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<PermissaoExibicaoExcel>();

                PermissaoExibicaoExcel obj;

                while (rd.Read())
                {
                    obj = new PermissaoExibicaoExcel
                    {
                        NOME_GRUPO = rd.GetString(0),
                        NOME_FORMULARIO = rd.GetString(1)
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

        public static int? Insert(PermissaoExibicao obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_PERMISSAO_EXIBICAO 
                (NOME_GRUPO, ID_FORMULARIO) VALUES 
                (@NOME_GRUPO, @ID_FORMULARIO)
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME_GRUPO", obj.grupo.NOME));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));

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

        public static int? Delete(PermissaoExibicao obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_PERMISSAO_EXIBICAO 
                WHERE NOME_GRUPO = @NOME_GRUPO
                AND ID_FORMULARIO = @ID_FORMULARIO
                ";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME_GRUPO", obj.grupo.NOME));
                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.formulario.ID));

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

        public static void AtualizarPermissoesUsuario(Usuario usuario)
        {
            if (usuario != null)
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = Util.CONNECTION_STRING;

                try
                {
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = con;

                    StringBuilder queryGet = new StringBuilder(@"
                    SELECT 
                    tf.NOME
                    	FROM TB_PERMISSAO_EXIBICAO tpe
                        JOIN TB_FORMULARIO tf ON tf.ID = tpe.ID_FORMULARIO
	                    	WHERE tpe.NOME_GRUPO IN (
                    ");
                    queryGet.Append(usuario.ObterGruposParaFiltro());
                    queryGet.Append(@") 
                    ORDER BY tf.NOME");

                    comm.CommandText = queryGet.ToString();

                    con.Open();

                    SqlDataReader rd = comm.ExecuteReader();

                    String nomeFormulario;
                    while (rd.Read())
                    {
                        nomeFormulario = rd.GetString(0);
                        usuario.AdicionarPermissaoExibicao(nomeFormulario);
                    }
                    rd.Close();
                }
                catch (Exception ex)
                {
                    usuario.LimparPermissoesExibicoes();
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}