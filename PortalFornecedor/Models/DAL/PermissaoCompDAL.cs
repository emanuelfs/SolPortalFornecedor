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
    public class PermissaoCompDAL
    {
        public static IList<PermissaoComp> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<PermissaoComp> objs = new List<PermissaoComp>();

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
                    ORDER BY tpc.NOME_GRUPO
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME_GRUPO asc":
                            ordenacao = "ORDER BY tpc.NOME_GRUPO";
                            break;
                        case "NOME_FORMULARIO asc":
                            ordenacao = "ORDER BY tf.NOME";
                            break;
                        case "NOME_STATUS asc":
                            ordenacao = "ORDER BY tsf.NOME";
                            break;
                        case "NOME_COMPONENTE asc":
                            ordenacao = "ORDER BY tc.NOME";
                            break;
                        case "TEXTO_TIPO asc":
                            ordenacao = "ORDER BY CASE WHEN tpc.TIPO = 1 THEN 'Oculto' ELSE 'Escrita' END";
                            break;
                        case "NOME_GRUPO desc":
                            ordenacao = "ORDER BY tpc.NOME_GRUPO DESC";
                            break;
                        case "NOME_FORMULARIO desc":
                            ordenacao = "ORDER BY tf.NOME DESC";
                            break;
                        case "NOME_STATUS desc":
                            ordenacao = "ORDER BY tsf.NOME DESC";
                            break;
                        case "NOME_COMPONENTE desc":
                            ordenacao = "ORDER BY tc.NOME DESC";
                            break;
                        case "TEXTO_TIPO desc":
                            ordenacao = "ORDER BY CASE WHEN tpc.TIPO = 1 THEN 'Oculto' ELSE 'Escrita' END DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY tpc.NOME_GRUPO
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
                        tpc.NOME_GRUPO,
                        tf.ID AS ID_FORMULARIO,
                        tf.NOME AS NOME_FORMULARIO,
                        tsf.ID AS ID_STATUS,
                        tsf.NOME AS NOME_STATUS,
                        tc.ID AS ID_COMPONENTE,
                        tc.NOME AS NOME_COMPONENTE,
                        tpc.TIPO,
                        TEXTO_TIPO = (CASE WHEN tpc.TIPO = 1 THEN 'Oculto' ELSE 'Escrita' END),

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(*) FROM TB_PERMISSAO_COMPONENTE) 
						AS 'totRegistros', 

						(SELECT COUNT(tpc.ID_COMPONENTE) 
                            FROM TB_PERMISSAO_COMPONENTE tpc
                            JOIN TB_STATUS_FORMULARIO tsf ON tsf.ID = tpc.ID_STATUS
                            JOIN TB_COMPONENTE tc ON tc.ID = tpc.ID_COMPONENTE
                            JOIN TB_FORMULARIO tf ON tf.ID = tsf.ID_FORMULARIO
						        WHERE
                                tpc.NOME_GRUPO collate Latin1_General_CI_AI like @textoFiltro
                                OR 
                                tf.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                tsf.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                tc.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR 
                                (CASE WHEN tpc.TIPO = 1 THEN 'Oculto' ELSE 'Escrita' END) collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_PERMISSAO_COMPONENTE tpc
                        JOIN TB_STATUS_FORMULARIO tsf ON tsf.ID = tpc.ID_STATUS
                        JOIN TB_COMPONENTE tc ON tc.ID = tpc.ID_COMPONENTE
                        JOIN TB_FORMULARIO tf ON tf.ID = tsf.ID_FORMULARIO
						    WHERE
                            tpc.NOME_GRUPO collate Latin1_General_CI_AI like @textoFiltro
                            OR 
                            tf.NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            tsf.NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR
                            tc.NOME collate Latin1_General_CI_AI like @textoFiltro
                            OR 
                            (CASE WHEN tpc.TIPO = 1 THEN 'Oculto' ELSE 'Escrita' END) collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                PermissaoComp obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(10);
                    totRegistrosFiltro = rd.GetInt32(11);

                    obj = new PermissaoComp
                    {
                        grupo = new Grupo
                        {
                            NOME = rd.GetString(0)
                        },
                        status = new StatusFormulario
                        {
                            formulario = new Formulario
                            {
                                ID = rd.GetInt32(1),
                                NOME = rd.GetString(2)
                            },
                            ID = rd.GetInt32(3),
                            NOME = rd.GetString(4)
                        },
                        componente = new Componente
                        {
                            ID = rd.GetInt32(5),
                            NOME = rd.GetString(6)
                        },
                        TIPO = rd.GetInt32(7),
                        TEXTO_TIPO = rd.GetString(8)
                    };

                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new PermissaoComp
                    {
                        grupo = new Grupo
                        {
                            NOME = rd.GetString(0)
                        },
                        status = new StatusFormulario
                        {
                            formulario = new Formulario
                            {
                                ID = rd.GetInt32(1),
                                NOME = rd.GetString(2)
                            },
                            ID = rd.GetInt32(3),
                            NOME = rd.GetString(4)
                        },
                        componente = new Componente
                        {
                            ID = rd.GetInt32(5),
                            NOME = rd.GetString(6)
                        },
                        TIPO = rd.GetInt32(7),
                        TEXTO_TIPO = rd.GetString(8)
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

        public static IList<PermissaoCompExcel> GetParaExcel()
        {
            IList<PermissaoCompExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT
                tpc.NOME_GRUPO,
                NOME_FORMULARIO = tf.NOME,
				NOME_COMPONENTE = tc.NOME,
                NOME_STATUS = tsf.NOME
                
                FROM TB_PERMISSAO_COMPONENTE tpc
				JOIN TB_COMPONENTE tc ON tc.ID = tpc.ID_COMPONENTE
                JOIN TB_STATUS_FORMULARIO tsf ON tsf.ID = tpc.ID_STATUS
                JOIN TB_FORMULARIO tf ON tf.ID = tsf.ID_FORMULARIO
                
                ORDER BY tpc.NOME_GRUPO, tf.NOME, tc.NOME, tsf.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<PermissaoCompExcel>();

                PermissaoCompExcel obj;

                while (rd.Read())
                {
                    obj = new PermissaoCompExcel
                    {
                        NOME_GRUPO = rd.GetString(0),
                        NOME_FORMULARIO = rd.GetString(1),
                        NOME_COMPONENTE = rd.GetString(2),
                        NOME_STATUS = rd.GetString(3)
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

        public static int? Insert(PermissaoComp obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_PERMISSAO_COMPONENTE 
                    (NOME_GRUPO, ID_STATUS, ID_COMPONENTE, TIPO) VALUES 
                    (@NOME_GRUPO, @ID_STATUS, @ID_COMPONENTE, @TIPO)";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME_GRUPO", obj.grupo.NOME));
                comm.Parameters.Add(new SqlParameter("ID_STATUS", obj.status.ID));
                comm.Parameters.Add(new SqlParameter("ID_COMPONENTE", obj.componente.ID));
                comm.Parameters.Add(new SqlParameter("TIPO", obj.TIPO));

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

        public static int? Update(PermissaoComp obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_PERMISSAO_COMPONENTE SET   
                    TIPO = @TIPO
                WHERE NOME_GRUPO = @NOME_GRUPO
                AND ID_STATUS = @ID_STATUS
                AND ID_COMPONENTE = @ID_COMPONENTE";

                con.Open();

                comm.Parameters.Add(new SqlParameter("TIPO", obj.TIPO));
                comm.Parameters.Add(new SqlParameter("NOME_GRUPO", obj.grupo.NOME));
                comm.Parameters.Add(new SqlParameter("ID_STATUS", obj.status.ID));
                comm.Parameters.Add(new SqlParameter("ID_COMPONENTE", obj.componente.ID));

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

        public static int? Delete(PermissaoComp obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_PERMISSAO_COMPONENTE
                WHERE NOME_GRUPO = @NOME_GRUPO
                AND ID_STATUS = @ID_STATUS
                AND ID_COMPONENTE = @ID_COMPONENTE";

                con.Open();

                comm.Parameters.Add(new SqlParameter("NOME_GRUPO", obj.grupo.NOME));
                comm.Parameters.Add(new SqlParameter("ID_STATUS", obj.status.ID));
                comm.Parameters.Add(new SqlParameter("ID_COMPONENTE", obj.componente.ID));

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
                    SELECT TB.* FROM (
                    SELECT 
                    (tf.NOME + '/' + tc.NOME + ' (' + tsf.NOME + ')') AS NOME_COMPONENTE
                    	FROM TB_PERMISSAO_COMPONENTE tpc, TB_COMPONENTE tc, TB_FORMULARIO tf, TB_STATUS_FORMULARIO tsf
	                    	WHERE tpc.NOME_GRUPO IN (
                    ");

                    queryGet.Append(usuario.ObterGruposParaFiltro());
                    queryGet.Append(") ");

                    queryGet.Append(@"AND tc.ID = tpc.ID_COMPONENTE
					AND tsf.ID = tpc.ID_STATUS
					AND tf.ID = tc.ID_FORMULARIO) TB
                    GROUP BY TB.NOME_COMPONENTE");

                    comm.CommandText = queryGet.ToString();

                    con.Open();

                    SqlDataReader rd = comm.ExecuteReader();

                    String nomeComponente;
                    while (rd.Read())
                    {
                        nomeComponente = rd.GetString(0);
                        usuario.AdicionarPermissaoComponente(nomeComponente);
                    }
                    rd.Close();
                }
                catch (Exception ex)
                {
                    usuario.LimparPermissoesComponentes();
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}