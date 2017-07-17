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
    public class PermissaoFuncDAL
    {
        public static IList<PermissaoFunc> GetPorGrupoModulo(String nomeGrupo, String nomeModulo)
        {
            IList<PermissaoFunc> objs = new List<PermissaoFunc>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
                FUNCIONALIDADE = 
                (
                	CASE WHEN @nomeModulo IS NULL THEN
                	func.NOME_MODULO + ' - ' + func.NOME + ' (' + func.CAMINHO + ')' ELSE 
                	func.NOME + ' (' + func.CAMINHO + ')' END
                ),
                POSSUI_PERMISSAO = 
                (
                	CASE WHEN EXISTS (
                		SELECT 1 
                			FROM TB_PERMISSAO_FUNCIONALIDADE 
                				WHERE NOME_GRUPO = @nomeGrupo
                				AND CAMINHO_FUNCIONALIDADE = func.CAMINHO
                	) THEN 1 ELSE 0 END
                ),
                func.CAMINHO
                	FROM TB_FUNCIONALIDADE func
                		WHERE (@nomeModulo IS NULL OR func.NOME_MODULO = @nomeModulo)
                            ORDER BY FUNCIONALIDADE";

                comm.CommandText = queryGet;

                con.Open();

                object auxNomeModulo = DBNull.Value;
                if (!string.IsNullOrEmpty(nomeModulo))
                {
                    auxNomeModulo = nomeModulo;
                }

                comm.Parameters.Add(new SqlParameter("nomeGrupo", nomeGrupo));
                comm.Parameters.Add(new SqlParameter("nomeModulo", auxNomeModulo));

                SqlDataReader rd = comm.ExecuteReader();

                PermissaoFunc obj;
                while (rd.Read())
                {
                    obj = new PermissaoFunc
                    {
                        FUNCIONALIDADE = rd.GetString(0),
                        POSSUI_PERMISSAO = rd.GetInt32(1),
                        CAMINHO_FUNCIONALIDADE = rd.GetString(2)
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

        public static IList<PermissaoFuncExcel> GetParaExcel()
        {
            IList<PermissaoFuncExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                tf.CAMINHO,
                tf.NOME,
                tf.NOME_MODULO,
                tpf.NOME_GRUPO
                
                FROM TB_PERMISSAO_FUNCIONALIDADE tpf
                JOIN TB_FUNCIONALIDADE tf ON tf.CAMINHO = tpf.CAMINHO_FUNCIONALIDADE
                
                ORDER BY tpf.NOME_GRUPO, tf.NOME_MODULO, tf.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<PermissaoFuncExcel>();

                PermissaoFuncExcel obj;

                while (rd.Read())
                {
                    obj = new PermissaoFuncExcel
                    {
                        CAMINHO = rd.GetString(0),
                        NOME = rd.GetString(1),
                        MODULO = rd.GetString(2),
                        GRUPO = rd.GetString(3)
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
                    tf.CAMINHO AS CAMINHO_FUNCIONALIDADE,
                    (tf.NOME_MODULO + '/' + tf.NOME) AS NOME_FUNCIONALIDADE
                    	FROM TB_PERMISSAO_FUNCIONALIDADE tpf, TB_FUNCIONALIDADE tf
	                    	WHERE tpf.NOME_GRUPO IN (
                    ");

                    queryGet.Append(usuario.ObterGruposParaFiltro());
                    queryGet.Append(") ");

                    queryGet.Append(@"AND tf.CAMINHO = tpf.CAMINHO_FUNCIONALIDADE) TB
                    GROUP BY TB.CAMINHO_FUNCIONALIDADE, TB.NOME_FUNCIONALIDADE");

                    comm.CommandText = queryGet.ToString();

                    con.Open();

                    SqlDataReader rd = comm.ExecuteReader();

                    String caminhoFunc;
                    String moduloFunc;
                    while (rd.Read())
                    {
                        caminhoFunc = rd.GetString(0);
                        moduloFunc = rd.GetString(1);
                        usuario.AdicionarPermissao(caminhoFunc, moduloFunc);
                    }
                    rd.Close();
                }
                catch (Exception ex)
                {
                    usuario.LimparPermissoes();
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int? AtualizarPermissoes(String nomeGrupo, String nomeModulo, String[] funcionalidades)
        {
            int? nrLinhas;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("atualizarPermissoesFunc");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    comm.CommandText = @"
                    DELETE FROM TB_PERMISSAO_FUNCIONALIDADE
                    WHERE NOME_GRUPO = @NOME_GRUPO
                    AND 
                    (
                        @NOME_MODULO IS NULL
                        OR CAMINHO_FUNCIONALIDADE IN 
                        (
                            SELECT CAMINHO
                            FROM TB_FUNCIONALIDADE
                            WHERE NOME_MODULO = @NOME_MODULO
                        )
                    )";

                    comm.Parameters.Clear();

                    object auxNomeModulo = DBNull.Value;
                    if (!string.IsNullOrEmpty(nomeModulo))
                    {
                        auxNomeModulo = nomeModulo;
                    }

                    comm.Parameters.Add(new SqlParameter("NOME_GRUPO", nomeGrupo));
                    comm.Parameters.Add(new SqlParameter("NOME_MODULO", auxNomeModulo));

                    comm.ExecuteNonQuery();

                    foreach (String funcionalidade in funcionalidades)
                    {
                        comm.CommandText = @"
                        INSERT INTO TB_PERMISSAO_FUNCIONALIDADE
                            (NOME_GRUPO,
                            CAMINHO_FUNCIONALIDADE)
                        VALUES
                            (@NOME_GRUPO,
                            @CAMINHO_FUNCIONALIDADE)";

                        comm.Parameters.Clear();

                        comm.Parameters.Add(new SqlParameter("NOME_GRUPO", nomeGrupo));
                        comm.Parameters.Add(new SqlParameter("CAMINHO_FUNCIONALIDADE", funcionalidade));

                        comm.ExecuteNonQuery();
                    }

                    trans.Commit();
                    nrLinhas = funcionalidades.Length;
                }
                catch
                {
                    nrLinhas = null;
                    trans.Rollback();
                }
                finally
                {
                    con.Close();
                }
            }
            return nrLinhas;
        }
    }
}