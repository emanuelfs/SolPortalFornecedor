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
    public class PermissaoStatusDAL
    {
        public static IList<PermissaoStatus> GetPorGrupoFormulario(String nomeGrupo, Int32? idFormulario)
        {
            IList<PermissaoStatus> objs = new List<PermissaoStatus>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT 
				ID_STATUS = stts.ID,
                NOME_STATUS = 
                (
                	CASE WHEN @idFormulario IS NULL THEN
                	stts.NOME + ' (' + formulario.NOME + ')' ELSE 
                	stts.NOME END
                ),
                POSSUI_PERMISSAO = 
                (
                	CASE WHEN EXISTS (
                		SELECT 1 
                			FROM TB_PERMISSAO_STATUS 
                				WHERE NOME_GRUPO = @nomeGrupo
                				AND ID_STATUS = stts.ID
                	) THEN 1 ELSE 0 END
                )
                FROM TB_STATUS_FORMULARIO stts
				JOIN TB_FORMULARIO formulario ON formulario.ID = stts.ID_FORMULARIO
                	WHERE (@idFormulario IS NULL OR stts.ID_FORMULARIO = @idFormulario)
                        ORDER BY NOME_STATUS";

                comm.CommandText = queryGet;

                con.Open();

                object auxIdFormulario = DBNull.Value;
                if (null != idFormulario)
                {
                    auxIdFormulario = idFormulario;
                }

                comm.Parameters.Add(new SqlParameter("nomeGrupo", nomeGrupo));
                comm.Parameters.Add(new SqlParameter("idFormulario", auxIdFormulario));

                SqlDataReader rd = comm.ExecuteReader();

                PermissaoStatus obj;
                while (rd.Read())
                {
                    obj = new PermissaoStatus
                    {
                        ID_STATUS = rd.GetInt32(0),
                        NOME_STATUS = rd.GetString(1),
                        POSSUI_PERMISSAO = rd.GetInt32(2)
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

        public static IList<PermissaoStatusExcel> GetParaExcel()
        {
            IList<PermissaoStatusExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT
                tps.NOME_GRUPO,
                NOME_FORMULARIO = tf.NOME,
                NOME_STATUS = tsf.NOME
                
                FROM TB_PERMISSAO_STATUS tps
                JOIN TB_STATUS_FORMULARIO tsf ON tsf.ID = tps.ID_STATUS
                JOIN TB_FORMULARIO tf ON tf.ID = tsf.ID_FORMULARIO
                
                ORDER BY tps.NOME_GRUPO, tf.NOME, tsf.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<PermissaoStatusExcel>();

                PermissaoStatusExcel obj;

                while (rd.Read())
                {
                    obj = new PermissaoStatusExcel
                    {
                        NOME_GRUPO = rd.GetString(0),
                        NOME_FORMULARIO = rd.GetString(1),
                        NOME_STATUS = rd.GetString(2)
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
                    tsf.ID AS ID_STATUS,
                    (tf.NOME + '/' + tsf.NOME) AS NOME_STATUS
                    	FROM TB_PERMISSAO_STATUS tps, TB_STATUS_FORMULARIO tsf, TB_FORMULARIO tf
	                    	WHERE tps.NOME_GRUPO IN (
                    ");

                    queryGet.Append(usuario.ObterGruposParaFiltro());
                    queryGet.Append(") ");

                    queryGet.Append(@"AND tsf.ID = tps.ID_STATUS
					AND tf.ID = tsf.ID_FORMULARIO) TB
                    GROUP BY TB.ID_STATUS, TB.NOME_STATUS");

                    comm.CommandText = queryGet.ToString();

                    con.Open();

                    SqlDataReader rd = comm.ExecuteReader();

                    int idStatus;
                    String nomeStatus;
                    while (rd.Read())
                    {
                        idStatus = rd.GetInt32(0);
                        nomeStatus = rd.GetString(1);
                        usuario.AdicionarPermissaoStatus(idStatus, nomeStatus);
                    }
                    rd.Close();
                }
                catch (Exception ex)
                {
                    usuario.LimparPermissoesStatus();
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int? AtualizarPermissoes(String nomeGrupo, Int32? idFormulario, String[] listaStatus)
        {
            int? nrLinhas;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("atualizarPermissoesStatus");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    comm.CommandText = @"
                    DELETE FROM TB_PERMISSAO_STATUS
                    WHERE NOME_GRUPO = @NOME_GRUPO
                    AND 
                    (
                        @ID_FORMULARIO IS NULL
                        OR ID_STATUS IN 
                        (
                            SELECT ID
                            FROM TB_STATUS_FORMULARIO
                            WHERE ID_FORMULARIO = @ID_FORMULARIO
                        )
                    )";

                    comm.Parameters.Clear();

                    object auxIdFormulario = DBNull.Value;
                    if (null != idFormulario)
                    {
                        auxIdFormulario = idFormulario;
                    }

                    comm.Parameters.Add(new SqlParameter("NOME_GRUPO", nomeGrupo));
                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", auxIdFormulario));

                    comm.ExecuteNonQuery();

                    foreach (String status in listaStatus)
                    {
                        comm.CommandText = @"
                        INSERT INTO TB_PERMISSAO_STATUS
                            (NOME_GRUPO,
                            ID_STATUS)
                        VALUES
                            (@NOME_GRUPO,
                            @ID_STATUS)";

                        comm.Parameters.Clear();

                        comm.Parameters.Add(new SqlParameter("NOME_GRUPO", nomeGrupo));
                        comm.Parameters.Add(new SqlParameter("ID_STATUS", status));

                        comm.ExecuteNonQuery();
                    }

                    trans.Commit();
                    nrLinhas = listaStatus.Length;
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