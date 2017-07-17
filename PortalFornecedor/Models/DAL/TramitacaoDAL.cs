using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class TramitacaoDAL
    {
        public static int? Insert(Preenchimento obj, Tramitacao tro, Usuario usuarioLogado)
        {
            int? nrLinhas;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("insertTramitacaoFormulario");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    foreach (ComponentePreenchimento compPreenchimento in obj.componentesPreenchimento)
                    {
                        comm.Parameters.Clear();
                        if (0 == compPreenchimento.ID)
                        {
                            comm.CommandText = @"
                                INSERT INTO TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
                                    (ID_COMPONENTE,
                                    VALOR_VARCHAR,
                                    ID_ITEM_LISTA,
                                    ID_PREENCHIMENTO_FORMULARIO,
                                    USER_NAME_RESPONSAVEL)
                                VALUES
                                    (@ID_COMPONENTE,
                                    @VALOR_VARCHAR,
                                    @ID_ITEM_LISTA,
                                    @ID_PREENCHIMENTO_FORMULARIO,
                                    @USER_NAME_RESPONSAVEL)";
                            comm.Parameters.Add(new SqlParameter("ID_COMPONENTE", compPreenchimento.componente.ID));
                            comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", tro.ID_PREENCHIMENTO_FORMULARIO));
                        }
                        else
                        {
                            comm.CommandText = @"
                                UPDATE TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
                                    SET VALOR_VARCHAR = @VALOR_VARCHAR,
                                    ID_ITEM_LISTA = @ID_ITEM_LISTA,
                                    USER_NAME_RESPONSAVEL = @USER_NAME_RESPONSAVEL
                                        WHERE ID = @idCompForm";
                            comm.Parameters.Add(new SqlParameter("idCompForm", compPreenchimento.ID));
                        }

                        object idItemLista = DBNull.Value;
                        if (compPreenchimento.itemLista != null && 0 != compPreenchimento.itemLista.ID)
                        {
                            idItemLista = compPreenchimento.itemLista.ID;
                        }

                        comm.Parameters.Add(new SqlParameter("VALOR_VARCHAR", compPreenchimento.VALOR_VARCHAR));
                        comm.Parameters.Add(new SqlParameter("ID_ITEM_LISTA", idItemLista));
                        comm.Parameters.Add(new SqlParameter("USER_NAME_RESPONSAVEL", usuarioLogado.USER_NAME));

                        if (comm.ExecuteNonQuery() == 0)
                        {
                            throw new Exception();
                        }
                    }

                    comm.CommandText = @"
                    INSERT INTO TB_TRAMITACAO
                        (USER_NAME_RESPONSAVEL,
                        NOME_RESPONSAVEL,
                        DATA_HORA,
                        OBSERVACAO,
                        ID_PREENCHIMENTO_FORMULARIO,
                        ID_STATUS_ORIGEM,
                        ID_STATUS_DESTINO,
                        LOG_ALTERACAO_COMPONENTES)
                    VALUES
                        (@USER_NAME_RESPONSAVEL,
                        @NOME_RESPONSAVEL,
                        GETDATE(),
                        @OBSERVACAO,
                        @ID_PREENCHIMENTO_FORMULARIO,
                        @ID_STATUS_ORIGEM,
                        @ID_STATUS_DESTINO,
                        @LOG_ALTERACAO_COMPONENTES)
                    SELECT SCOPE_IDENTITY();";

                    comm.Parameters.Clear();

                    int? ID_STATUS_ORIGEM = null;
                    if (tro.statusOrigem != null)
                    {
                        ID_STATUS_ORIGEM = tro.statusOrigem.ID;
                    }

                    object auxLogAlteracaoComponentes = DBNull.Value;
                    if (!string.IsNullOrEmpty(tro.LOG_ALTERACAO_COMPONENTES))
                    {
                        auxLogAlteracaoComponentes = tro.LOG_ALTERACAO_COMPONENTES;
                    }

                    comm.Parameters.Add(new SqlParameter("USER_NAME_RESPONSAVEL", usuarioLogado.USER_NAME));
                    comm.Parameters.Add(new SqlParameter("NOME_RESPONSAVEL", usuarioLogado.NOME));
                    comm.Parameters.Add(new SqlParameter("OBSERVACAO", tro.OBSERVACAO));
                    comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", tro.ID_PREENCHIMENTO_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", ID_STATUS_ORIGEM));
                    comm.Parameters.Add(new SqlParameter("ID_STATUS_DESTINO", tro.statusDestino.ID));
                    comm.Parameters.Add(new SqlParameter("LOG_ALTERACAO_COMPONENTES", auxLogAlteracaoComponentes));

                    if (null != tro.arquivos && tro.arquivos.Count > 0)
                    {
                        object auxIdTramitacaoInserida = comm.ExecuteScalar();
                        Int32? idTramitacaoInserida = (auxIdTramitacaoInserida == null || Convert.IsDBNull(auxIdTramitacaoInserida)) ? (Int32?)null : Convert.ToInt32(auxIdTramitacaoInserida);
                        if (idTramitacaoInserida != null)
                        {
                            foreach (ArquivoTramitacao arquivoTramitacao in tro.arquivos)
                            {
                                comm.CommandText = @"
                                    INSERT INTO TB_ARQUIVO_TRAMITACAO
                                        (NOME,
                                        ARQUIVO,
                                        ID_TRAMITACAO)
                                    VALUES
                                        (@NOME,
                                        @ARQUIVO,
                                        @ID_TRAMITACAO)";
                                comm.Parameters.Clear();

                                comm.Parameters.Add(new SqlParameter("NOME", arquivoTramitacao.NOME));
                                comm.Parameters.Add(new SqlParameter("ARQUIVO", arquivoTramitacao.ARQUIVO));
                                comm.Parameters.Add(new SqlParameter("ID_TRAMITACAO", idTramitacaoInserida));
                                if (comm.ExecuteNonQuery() == 0)
                                {
                                    throw new Exception();
                                }
                            }
                            trans.Commit();
                            nrLinhas = 1;
                        }
                        else
                        {
                            nrLinhas = null;
                            trans.Rollback();
                        }
                    }
                    else
                    {
                        nrLinhas = comm.ExecuteNonQuery();
                        if (nrLinhas == 1)
                        {
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                        }
                    }
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

        private static IList<ArquivoTramitacao> MontarListaParaHistorico(string listaArquivos)
        {
            IList<ArquivoTramitacao> arquivos = new List<ArquivoTramitacao>();
            try
            {
                string[] auxListaArquivos = listaArquivos.Split('|');
                string[] auxArquivo;
                foreach (string arquivo in auxListaArquivos)
                {
                    auxArquivo = arquivo.Split(':');
                    arquivos.Add(new ArquivoTramitacao
                    {
                        ID = Convert.ToInt32(auxArquivo[0]),
                        NOME = auxArquivo[1]
                    });
                }
            }
            catch
            {
                arquivos.Clear();
            }
            return arquivos;
        }

        public static IList<Tramitacao> GetPorPreenchimento(Int32 ID_PREENCHIMENTO_FORMULARIO)
        {
            IList<Tramitacao> objs = new List<Tramitacao>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT tt.USER_NAME_RESPONSAVEL, tt.NOME_RESPONSAVEL, CONVERT(VARCHAR,tt.DATA_HORA,103) + ' ' + CONVERT(VARCHAR,tt.DATA_HORA,108) AS DATA_HORA, tt.OBSERVACAO, tsfDestino.NOME AS NOME_STATUS_DESTINO
                    ,ISNULL(STUFF((SELECT '|', ISNULL(CONVERT(VARCHAR, ID), '') + ':' + NOME
                                FROM TB_ARQUIVO_TRAMITACAO
                                WHERE ID_TRAMITACAO = tt.ID
                                ORDER BY NOME
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,1,''), '') AS LISTA_ARQUIVOS,
                    tt.LOG_ALTERACAO_COMPONENTES
	                    FROM TB_TRAMITACAO tt
	                    JOIN TB_STATUS_FORMULARIO tsfDestino ON tsfDestino.ID = tt.ID_STATUS_DESTINO
	                    	WHERE tt.ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
	                    		ORDER BY tt.DATA_HORA";

                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                Tramitacao obj;
                while (rd.Read())
                {
                    obj = new Tramitacao
                    {
                        USER_NAME_RESPONSAVEL = rd.GetString(0),
                        NOME_RESPONSAVEL = rd.GetString(1),
                        DATA_HORA = rd.GetString(2),
                        OBSERVACAO = rd.GetString(3),
                        statusDestino = new StatusFormulario
                        {
                            NOME = rd.GetString(4)
                        },
                        arquivos = MontarListaParaHistorico(rd.GetString(5)),
                        LOG_ALTERACAO_COMPONENTES = rd.IsDBNull(6) ? string.Empty : rd.GetString(6)
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
    }
}