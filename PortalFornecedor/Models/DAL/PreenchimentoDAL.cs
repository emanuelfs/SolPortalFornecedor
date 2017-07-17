using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class PreenchimentoDAL
    {
        private static object MontarRegistro(SqlDataReader rd, string[] colunasForm, int indice, int indiceNumeroLinha)
        {
            dynamic result = new ExpandoObject();
            IDictionary<string, object> colunasResult = result;
            string nomeColuna;
            object valorColuna;
            while (indice < indiceNumeroLinha)
            {
                nomeColuna = colunasForm[indice];
                valorColuna = rd.IsDBNull(indice) ? string.Empty : rd.GetValue(indice);
                colunasResult.Add(nomeColuna, valorColuna);
                indice++;
            }
            return result;
        }

        public static IList<string> GetColunasDinamicas(Int32 ID_FORMULARIO, string[] colunasFixas, bool somenteOcultas)
        {
            IList<string> objs = colunasFixas != null ? new List<string>(colunasFixas) : new List<string>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet;
                if (somenteOcultas)
                {
                    queryGet = new StringBuilder(@"
                            SELECT STUFF((SELECT ';', NOME + '|' + CONVERT(VARCHAR, EXIBIR_NO_LANCAMENTO) + '|' + CONVERT(VARCHAR, EXIBIR_NO_ATENDIMENTO) + '|' + CONVERT(VARCHAR, EXIBIR_NA_BUSCA_AVANCADA)
                                FROM TB_COMPONENTE
                                WHERE ID_FORMULARIO = @idFormulario
                                AND EXIBIR_NO_GRID = 0
                                ORDER BY ORDEM
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,1,'') AS NOME_COMPONENTES");
                }
                else
                {
                    queryGet = new StringBuilder(@"
                            SELECT STUFF((SELECT ';', NOME + '|' + CONVERT(VARCHAR, EXIBIR_NO_LANCAMENTO) + '|' + CONVERT(VARCHAR, EXIBIR_NO_ATENDIMENTO) + '|' + CONVERT(VARCHAR, EXIBIR_NA_BUSCA_AVANCADA)
                                FROM TB_COMPONENTE
                                WHERE ID_FORMULARIO = @idFormulario
                                ORDER BY ORDEM
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,1,'') AS NOME_COMPONENTES");
                }

                comm.Parameters.Add(new SqlParameter("idFormulario", ID_FORMULARIO));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read() && !rd.IsDBNull(0))
                {
                    string[] auxObjs = rd.GetString(0).Split(';');
                    ((List<string>)objs).InsertRange(objs.Count, auxObjs);
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

        public static IList<object> Get(Int32 ID_FORMULARIO, string[] colunasForm, string dataInicialFiltro, string dataFinalFiltro, string statusFormFiltro, int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir, string userNameCriador, string statusFiltroUsuarioLogado, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder colunasFormConca = new StringBuilder();
                StringBuilder colunasSelect = new StringBuilder("TB_DINAMICA.ID,TB_DINAMICA.ID_STATUS_ATUAL,TB_DINAMICA.NOME_RESPONSAVEL,TB_DINAMICA.[Criação],TB_DINAMICA.[Identificador],TB_DINAMICA.[Status],TB_DINAMICA.[Loja],");
                StringBuilder colunasFiltro = new StringBuilder();
                int totColunasFixas = 7;//ID, ID_STATUS_ATUAL, NOME_RESPONSAVEL, DATA_CRIACAO, IDENTIFICADOR, NOME_STATUS_ATUAL e LOJA_CRIADOR
                if (colunasForm.Length > totColunasFixas)
                {
                    string colunaDinamica = string.Format("[{0}]", colunasForm[totColunasFixas]);
                    colunasFormConca.Append(string.Format("MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[totColunasFixas], colunaDinamica));
                    colunasSelect.Append(string.Format("TB_DINAMICA.{0},", colunaDinamica));
                    colunasFiltro.Append(string.Format("TB_DINAMICA.{0} collate Latin1_General_CI_AI like @textoFiltro", colunaDinamica));
                    for (int i = totColunasFixas + 1; i < colunasForm.Length; i++)
                    {
                        colunaDinamica = string.Format("[{0}]", colunasForm[i]);
                        colunasFormConca.Append(string.Format(",MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[i], colunaDinamica));
                        colunasSelect.Append(string.Format("TB_DINAMICA.{0},", colunaDinamica));
                        colunasFiltro.Append(string.Format(" OR TB_DINAMICA.{0} collate Latin1_General_CI_AI like @textoFiltro", colunaDinamica));
                    }
                    //Adicionando colunas fixas que aparecem no grid ao filtro
                    colunasFiltro.Append(" OR TB_DINAMICA.[Criação] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Identificador] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Status] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Loja] collate Latin1_General_CI_AI like @textoFiltro");
                    //Adicionando colunas fixas que aparecem no grid ao filtro

                    string ordenacao;
                    if (string.IsNullOrEmpty(sortColumn))
                    {
                        ordenacao = @"
                        ORDER BY TB_DINAMICA.[Criação]
                        ";
                    }
                    else
                    {
                        //ordenacao = string.Format("ORDER BY ABS(TB_DINAMICA.{0}) {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                        ordenacao = string.Format("ORDER BY TB_DINAMICA.{0} {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                    }

                    string dataInicio = string.Empty;
                    string dataFim = string.Empty;
                    if (!string.IsNullOrEmpty(dataInicialFiltro))
                    {
                        string[] auxDataInicialFiltro = dataInicialFiltro.Split('/');
                        if (auxDataInicialFiltro.Length == 3)
                        {
                            dataInicio = string.Format("{0}-{1}-{2} 00:00:00", auxDataInicialFiltro[2], auxDataInicialFiltro[1], auxDataInicialFiltro[0]);
                        }
                    }
                    if (!string.IsNullOrEmpty(dataFinalFiltro))
                    {
                        string[] auxDataFinalFiltro = dataFinalFiltro.Split('/');
                        if (auxDataFinalFiltro.Length == 3)
                        {
                            dataFim = string.Format("{0}-{1}-{2} 23:59:59", auxDataFinalFiltro[2], auxDataFinalFiltro[1], auxDataFinalFiltro[0]);
                        }
                    }

                    StringBuilder filtroStatus = new StringBuilder();
                    if (!string.IsNullOrEmpty(statusFormFiltro))
                    {
                        if (!"T".Equals(statusFormFiltro))
                        {
                            filtroStatus.Append("AND tt.ID_STATUS_DESTINO = @statusForm");
                            comm.Parameters.Add(new SqlParameter("statusForm", statusFormFiltro));
                        }
                        else
                        {
                            filtroStatus.Append("AND tt.ID_STATUS_DESTINO IN (").Append(statusFiltroUsuarioLogado).Append(")");
                        }
                    }
                    filtroStatus.Append(@"
                    AND tt.ID IN 
                    (
			            SELECT MAX(ID)
				            FROM TB_TRAMITACAO
					            WHERE ID_PREENCHIMENTO_FORMULARIO = tpf.ID
		            )
                    ");

                    StringBuilder filtroIdBusca = new StringBuilder();
                    if (!string.IsNullOrEmpty(idBuscaPreenchimento))
                    {
                        filtroIdBusca.Append("AND tpf.ID_BUSCA collate Latin1_General_CI_AI like @idBuscaPreenchimento");
                        comm.Parameters.Add(new SqlParameter("idBuscaPreenchimento", string.Format("%{0}%", idBuscaPreenchimento)));
                    }

                    StringBuilder filtroCamposDinamicos = new StringBuilder();
                    if (camposDinamicosFiltro != null && camposDinamicosFiltro.Length > 0)
                    {
                        int totDinamicosAdicionados = 0;
                        StringBuilder auxCamposDinamicosFiltro = new StringBuilder();
                        AdicionarColunaBuscaAvancada(camposDinamicosFiltro, 0, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        for (int i = 1; i < camposDinamicosFiltro.Length; i++)
                        {
                            AdicionarColunaBuscaAvancada(camposDinamicosFiltro, i, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        }
                        if (totDinamicosAdicionados > 0)
                        {
                            auxCamposDinamicosFiltro.Append(")");
                            filtroCamposDinamicos.Append(@"
                            AND EXISTS 
                            (
                                SELECT tcpf.ID_PREENCHIMENTO_FORMULARIO
                                    FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf, TB_COMPONENTE tc
                                        WHERE tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                                        AND tc.ID = tcpf.ID_COMPONENTE
                                        ").Append(auxCamposDinamicosFiltro.ToString()).Append(@"
                                            GROUP BY tcpf.ID_PREENCHIMENTO_FORMULARIO
                                            HAVING COUNT(tc.ID) = @totDinamicosAdicionados
                            )
                            ");
                            comm.Parameters.Add(new SqlParameter("@totDinamicosAdicionados", totDinamicosAdicionados));
                        }
                    }

                    #region TabelaDinamica
                    StringBuilder filtroSomenteMeus = new StringBuilder();
                    if (null != userNameCriador)
                    {
                        filtroSomenteMeus.Append("AND tpf.USER_NAME_CRIADOR = @userNameCriador");
                    }

                    StringBuilder fromTabelaDinamica = new StringBuilder(@"
                        FROM TB_PREENCHIMENTO_FORMULARIO tpf
						JOIN TB_TRAMITACAO tt ON tt.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                        JOIN TB_STATUS_FORMULARIO ts ON ts.ID = tt.ID_STATUS_DESTINO
						LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf ON tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
						LEFT JOIN TB_COMPONENTE tc ON tc.ID = tcpf.ID_COMPONENTE
				            WHERE tpf.DATA_HORA BETWEEN @dataInicio AND @dataFim
                            AND tpf.ID_FORMULARIO = @idFormulario
                            ").Append(filtroStatus.ToString()).Append(@"
                            ").Append(filtroSomenteMeus.ToString()).Append(@"
                            ").Append(filtroIdBusca.ToString()).Append(@"
                            ").Append(filtroCamposDinamicos.ToString());

                    StringBuilder tabelaDinamica = new StringBuilder(@"
                    (
                        SELECT ID_PREENCHIMENTO_FORMULARIO AS ID,ID_STATUS_ATUAL,NOME_RESPONSAVEL,[Criação],[Identificador],[Status],[Loja],").Append(colunasFormConca).Append(@" from 
				        (
				        SELECT 
				        	tpf.ID AS ID_PREENCHIMENTO_FORMULARIO, 
                            tt.ID_STATUS_DESTINO AS ID_STATUS_ATUAL,
                            tpf.NOME_RESPONSAVEL,
                            CONVERT(VARCHAR,tpf.DATA_HORA,103) AS [Criação],
                            tpf.ID_BUSCA AS [Identificador],
                            ts.NOME AS [Status],
                            tpf.LOJA_CRIADOR AS [Loja],
				        	tcpf.VALOR_VARCHAR,
				        	tc.NOME AS NOME_COMPONENTE_,
							tcpf.VALOR_VARCHAR AS VALOR_COMPONENTE_
				        	").Append(fromTabelaDinamica.ToString()).Append(@"
				        ) tb
				        GROUP BY ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL, NOME_RESPONSAVEL, [Criação], [Identificador], [Status], [Loja]
                    ) AS TB_DINAMICA
                    ");
                    #endregion

                    con.Open();

                    #region QueryGetTotRegistros
                    StringBuilder queryGetTotRegistros = new StringBuilder(@"
                    SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                        FROM (
                            SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
                            GROUP BY tpf.ID
                        ) tbTotRegistrosFiltro
                    ");
                    #endregion

                    comm.CommandText = queryGetTotRegistros.ToString();

                    comm.Parameters.Add(new SqlParameter("idFormulario", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("dataInicio", dataInicio));
                    comm.Parameters.Add(new SqlParameter("dataFim", dataFim));

                    if (null != userNameCriador)
                    {
                        comm.Parameters.Add(new SqlParameter("userNameCriador", userNameCriador));
                    }

                    totRegistros = (int)comm.ExecuteScalar();

                    #region QueryGetTotRegistrosFiltro
                    StringBuilder colunasFixasGridFiltro = new StringBuilder(@"OR CONVERT(VARCHAR,tpf.DATA_HORA,103) collate Latin1_General_CI_AI like @textoFiltro
                    OR tpf.ID_BUSCA collate Latin1_General_CI_AI like @textoFiltro
                    OR ts.NOME collate Latin1_General_CI_AI like @textoFiltro
                    OR tpf.LOJA_CRIADOR collate Latin1_General_CI_AI like @textoFiltro");

                    StringBuilder queryGetTotRegistrosFiltro = new StringBuilder(@"
                    SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                        FROM (
                            SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
                            AND (tcpf.VALOR_VARCHAR collate Latin1_General_CI_AI like @textoFiltro
                            ").Append(colunasFixasGridFiltro.ToString()).Append(@")
                            GROUP BY tpf.ID
                        ) tbTotRegistrosFiltro
                    ");
                    #endregion

                    comm.CommandText = queryGetTotRegistrosFiltro.ToString();

                    comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                    totRegistrosFiltro = (int)comm.ExecuteScalar();

                    #region QueryGet
                    StringBuilder queryGet = new StringBuilder(@"
                    SELECT TOP (@pageSize) *
	                    FROM (
				    		SELECT 
				    		").Append(colunasSelect.ToString()).Append(@"

				    		(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
				    		AS 'numeroLinha'

	                    	FROM ").Append(tabelaDinamica.ToString()).Append(@"
				    			WHERE 
                                ").Append(colunasFiltro.ToString()).Append(@") 
				    as todasLinhas
                    WHERE todasLinhas.numeroLinha > (@start)");
                    #endregion

                    comm.CommandText = queryGet.ToString();

                    comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                    comm.Parameters.Add(new SqlParameter("start", start));

                    SqlDataReader rd = comm.ExecuteReader();

                    int indiceNumeroLinha = Convert.ToInt32(colunasForm.Length);
                    while (rd.Read())
                    {
                        objs.Add(MontarRegistro(rd, colunasForm, 0, indiceNumeroLinha));
                    }

                    rd.Close();
                }
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

        public static IList<object> GetParaExcel(Int32 ID_FORMULARIO, string[] colunasForm, string dataInicialFiltro, string dataFinalFiltro, string statusFormFiltro, string userNameCriador, string statusFiltroUsuarioLogado, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder colunasFormConca = new StringBuilder();
                StringBuilder colunasSelect = new StringBuilder("TB_DINAMICA.ID,TB_DINAMICA.ID_STATUS_ATUAL,TB_DINAMICA.NOME_RESPONSAVEL,TB_DINAMICA.[Criação],TB_DINAMICA.[Identificador],TB_DINAMICA.[Status],TB_DINAMICA.[Loja]");
                StringBuilder colunasFiltro = new StringBuilder();
                int totColunasFixas = 7;//ID, ID_STATUS_ATUAL, NOME_RESPONSAVEL, DATA_CRIACAO, IDENTIFICADOR, NOME_STATUS_ATUAL e LOJA_CRIADOR
                if (colunasForm.Length > totColunasFixas)
                {
                    string colunaDinamica = string.Format("[{0}]", colunasForm[totColunasFixas]);
                    colunasFormConca.Append(string.Format("MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[totColunasFixas], colunaDinamica));
                    colunasSelect.Append(string.Format(",TB_DINAMICA.{0}", colunaDinamica));
                    for (int i = totColunasFixas + 1; i < colunasForm.Length; i++)
                    {
                        colunaDinamica = string.Format("[{0}]", colunasForm[i]);
                        colunasFormConca.Append(string.Format(",MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[i], colunaDinamica));
                        colunasSelect.Append(string.Format(",TB_DINAMICA.{0}", colunaDinamica));
                    }

                    string dataInicio = string.Empty;
                    string dataFim = string.Empty;
                    if (!string.IsNullOrEmpty(dataInicialFiltro))
                    {
                        string[] auxDataInicialFiltro = dataInicialFiltro.Split('/');
                        if (auxDataInicialFiltro.Length == 3)
                        {
                            dataInicio = string.Format("{0}-{1}-{2} 00:00:00", auxDataInicialFiltro[2], auxDataInicialFiltro[1], auxDataInicialFiltro[0]);
                        }
                    }
                    if (!string.IsNullOrEmpty(dataFinalFiltro))
                    {
                        string[] auxDataFinalFiltro = dataFinalFiltro.Split('/');
                        if (auxDataFinalFiltro.Length == 3)
                        {
                            dataFim = string.Format("{0}-{1}-{2} 23:59:59", auxDataFinalFiltro[2], auxDataFinalFiltro[1], auxDataFinalFiltro[0]);
                        }
                    }

                    StringBuilder filtroStatus = new StringBuilder();
                    if (!string.IsNullOrEmpty(statusFormFiltro))
                    {
                        if (!"T".Equals(statusFormFiltro))
                        {
                            filtroStatus.Append("AND tt.ID_STATUS_DESTINO = @statusForm");
                            comm.Parameters.Add(new SqlParameter("statusForm", statusFormFiltro));
                        }
                        else
                        {
                            filtroStatus.Append("AND tt.ID_STATUS_DESTINO IN (").Append(statusFiltroUsuarioLogado).Append(")");
                        }
                    }
                    filtroStatus.Append(@"
                    AND tt.ID IN 
                    (
			            SELECT MAX(ID)
				            FROM TB_TRAMITACAO
					            WHERE ID_PREENCHIMENTO_FORMULARIO = tpf.ID
		            )
                    ");

                    #region TabelaDinamica
                    StringBuilder filtroSomenteMeus = new StringBuilder();
                    if (null != userNameCriador)
                    {
                        filtroSomenteMeus.Append("AND tpf.USER_NAME_CRIADOR = @userNameCriador");
                    }
                    StringBuilder filtroIdBusca = new StringBuilder();
                    if (!string.IsNullOrEmpty(idBuscaPreenchimento))
                    {
                        filtroIdBusca.Append("AND tpf.ID_BUSCA collate Latin1_General_CI_AI like @idBuscaPreenchimento");
                        comm.Parameters.Add(new SqlParameter("idBuscaPreenchimento", string.Format("%{0}%", idBuscaPreenchimento)));
                    }
                    StringBuilder filtroCamposDinamicos = new StringBuilder();
                    if (camposDinamicosFiltro != null && camposDinamicosFiltro.Length > 0)
                    {
                        int totDinamicosAdicionados = 0;
                        StringBuilder auxCamposDinamicosFiltro = new StringBuilder();
                        AdicionarColunaBuscaAvancada(camposDinamicosFiltro, 0, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        for (int i = 1; i < camposDinamicosFiltro.Length; i++)
                        {
                            AdicionarColunaBuscaAvancada(camposDinamicosFiltro, i, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        }
                        if (totDinamicosAdicionados > 0)
                        {
                            auxCamposDinamicosFiltro.Append(")");
                            filtroCamposDinamicos.Append(@"
                            AND EXISTS 
                            (
                                SELECT tcpf.ID_PREENCHIMENTO_FORMULARIO
                                    FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf, TB_COMPONENTE tc
                                        WHERE tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                                        AND tc.ID = tcpf.ID_COMPONENTE
                                        ").Append(auxCamposDinamicosFiltro.ToString()).Append(@"
                                            GROUP BY tcpf.ID_PREENCHIMENTO_FORMULARIO
                                            HAVING COUNT(tc.ID) = @totDinamicosAdicionados
                            )
                            ");
                            comm.Parameters.Add(new SqlParameter("@totDinamicosAdicionados", totDinamicosAdicionados));
                        }
                    }

                    StringBuilder fromTabelaDinamica = new StringBuilder(@"
                        FROM TB_PREENCHIMENTO_FORMULARIO tpf
						JOIN TB_TRAMITACAO tt ON tt.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                        JOIN TB_STATUS_FORMULARIO ts ON ts.ID = tt.ID_STATUS_DESTINO 
						LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf ON tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
						LEFT JOIN TB_COMPONENTE tc ON tc.ID = tcpf.ID_COMPONENTE
				            WHERE tpf.DATA_HORA BETWEEN @dataInicio AND @dataFim
                            AND tpf.ID_FORMULARIO = @idFormulario
                            ").Append(filtroStatus.ToString()).Append(@"
                            ").Append(filtroSomenteMeus.ToString()).Append(@"
                            ").Append(filtroIdBusca.ToString()).Append(@"
                            ").Append(filtroCamposDinamicos.ToString());

                    StringBuilder tabelaDinamica = new StringBuilder(@"
                    (
                        SELECT ID_PREENCHIMENTO_FORMULARIO AS ID,ID_STATUS_ATUAL,NOME_RESPONSAVEL,[Criação],[Identificador],[Status],[Loja],").Append(colunasFormConca).Append(@" from 
				        (
				        SELECT 
				        	tpf.ID AS ID_PREENCHIMENTO_FORMULARIO, 
                            tt.ID_STATUS_DESTINO AS ID_STATUS_ATUAL,
				        	tpf.NOME_RESPONSAVEL,
                            CONVERT(VARCHAR,tpf.DATA_HORA,103) AS [Criação],
                            tpf.ID_BUSCA AS [Identificador],
                            ts.NOME AS [Status],
                            tpf.LOJA_CRIADOR AS [Loja],
				        	tcpf.VALOR_VARCHAR,
				        	tc.NOME AS NOME_COMPONENTE_,
							tcpf.VALOR_VARCHAR AS VALOR_COMPONENTE_
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
				        ) tb
				        GROUP BY ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL, NOME_RESPONSAVEL, [Criação], [Identificador], [Status], [Loja]
                    ) AS TB_DINAMICA
                    ");
                    #endregion

                    con.Open();

                    comm.Parameters.Add(new SqlParameter("idFormulario", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("dataInicio", dataInicio));
                    comm.Parameters.Add(new SqlParameter("dataFim", dataFim));

                    if (null != userNameCriador)
                    {
                        comm.Parameters.Add(new SqlParameter("userNameCriador", userNameCriador));
                    }

                    #region QueryGet
                    StringBuilder queryGet = new StringBuilder(@"
				    SELECT ").Append(colunasSelect.ToString()).Append(@"
	                FROM ").Append(tabelaDinamica.ToString());
                    #endregion

                    comm.CommandText = queryGet.ToString();

                    SqlDataReader rd = comm.ExecuteReader();

                    int indiceNumeroLinha = Convert.ToInt32(colunasForm.Length);
                    while (rd.Read())
                    {
                        objs.Add(MontarRegistro(rd, colunasForm, 3, indiceNumeroLinha));
                    }

                    rd.Close();
                }
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

        public static IList<object> GetParaExcelGenerica(int[] formularios, string dataInicialFiltro, string dataFinalFiltro, string idBuscaPreenchimento, string gruposFiltroUsuarioLogado)
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder colunasFormConca = new StringBuilder();
                StringBuilder colunasSelect = new StringBuilder("TB_DINAMICA.ID_FORMULARIO,TB_DINAMICA.ID,TB_DINAMICA.ID_STATUS_ATUAL,TB_DINAMICA.NOME_RESPONSAVEL,TB_DINAMICA.[Formulário],TB_DINAMICA.[Criação],TB_DINAMICA.[Identificador],TB_DINAMICA.[Status],TB_DINAMICA.[Loja]");

                string dataInicio = string.Empty;
                string dataFim = string.Empty;
                if (!string.IsNullOrEmpty(dataInicialFiltro))
                {
                    string[] auxDataInicialFiltro = dataInicialFiltro.Split('/');
                    if (auxDataInicialFiltro.Length == 3)
                    {
                        dataInicio = string.Format("{0}-{1}-{2} 00:00:00", auxDataInicialFiltro[2], auxDataInicialFiltro[1], auxDataInicialFiltro[0]);
                    }
                }
                if (!string.IsNullOrEmpty(dataFinalFiltro))
                {
                    string[] auxDataFinalFiltro = dataFinalFiltro.Split('/');
                    if (auxDataFinalFiltro.Length == 3)
                    {
                        dataFim = string.Format("{0}-{1}-{2} 23:59:59", auxDataFinalFiltro[2], auxDataFinalFiltro[1], auxDataFinalFiltro[0]);
                    }
                }

                StringBuilder filtroFormularios = new StringBuilder();
                if (formularios != null && formularios.Length > 0)
                {
                    Int32 idFormulario = formularios[0];
                    filtroFormularios.Append(idFormulario);
                    for (int i = 1; i < formularios.Length; i++)
                    {
                        idFormulario = formularios[i];
                        filtroFormularios.Append(",").Append(idFormulario);
                    }
                }
                else
                {
                    filtroFormularios.Append("''");
                }

                StringBuilder filtroPermissaoExibicao = new StringBuilder();
                filtroPermissaoExibicao.Append(@"
                AND 
                (
                    (1 = tf.EXIBIR_TODOS) 
                    OR
                    (EXISTS
                        (
                            SELECT 1 
                                FROM TB_PERMISSAO_EXIBICAO 
                                    WHERE ID_FORMULARIO = tf.ID 
                                    AND NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
                        )
                    )
                )
                ");

                StringBuilder filtroStatus = new StringBuilder();
                filtroStatus.Append(@"
                AND tt.ID IN 
                (
			        SELECT MAX(ID)
				        FROM TB_TRAMITACAO
				            WHERE ID_PREENCHIMENTO_FORMULARIO = tpf.ID
		        )
                ");

                StringBuilder filtroIdBusca = new StringBuilder();
                if (!string.IsNullOrEmpty(idBuscaPreenchimento))
                {
                    filtroIdBusca.Append("AND tpf.ID_BUSCA collate Latin1_General_CI_AI like @idBuscaPreenchimento");
                    comm.Parameters.Add(new SqlParameter("idBuscaPreenchimento", string.Format("%{0}%", idBuscaPreenchimento)));
                }

                #region TabelaDinamica
                StringBuilder fromTabelaDinamica = new StringBuilder(@"
                FROM TB_FORMULARIO tf
				JOIN TB_PREENCHIMENTO_FORMULARIO tpf ON tpf.ID_FORMULARIO = tf.ID
				JOIN TB_TRAMITACAO tt ON tt.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                JOIN TB_STATUS_FORMULARIO ts ON ts.ID = tt.ID_STATUS_DESTINO 
				LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf ON tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
				LEFT JOIN TB_COMPONENTE tc ON tc.ID = tcpf.ID_COMPONENTE
				    WHERE tpf.DATA_HORA BETWEEN @dataInicio AND @dataFim
                    AND tpf.ID_FORMULARIO IN (").Append(filtroFormularios.ToString()).Append(@")
                    ").Append(filtroPermissaoExibicao.ToString()).Append(@"
                    ").Append(filtroStatus.ToString()).Append(@"
                    ").Append(filtroIdBusca.ToString());

                StringBuilder tabelaDinamica = new StringBuilder(@"
                (
                    SELECT ID_FORMULARIO, [Formulário], ID_PREENCHIMENTO_FORMULARIO AS ID,ID_STATUS_ATUAL,NOME_RESPONSAVEL,[Criação],[Identificador],[Status],[Loja] from 
				    (
				    SELECT 
                        tf.ID AS ID_FORMULARIO,
						tf.NOME AS [Formulário],
				    	tpf.ID AS ID_PREENCHIMENTO_FORMULARIO, 
                        tt.ID_STATUS_DESTINO AS ID_STATUS_ATUAL,
				    	tpf.NOME_RESPONSAVEL,
                        CONVERT(VARCHAR,tpf.DATA_HORA,103) AS [Criação],
                        tpf.ID_BUSCA AS [Identificador],
                        ts.NOME AS [Status],
                        tpf.LOJA_CRIADOR AS [Loja],
				    	tcpf.VALOR_VARCHAR,
				    	tc.NOME AS NOME_COMPONENTE_,
					tcpf.VALOR_VARCHAR AS VALOR_COMPONENTE_
                        ").Append(fromTabelaDinamica.ToString()).Append(@"
				    ) tb
				    GROUP BY ID_FORMULARIO, [Formulário], ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL, NOME_RESPONSAVEL, [Criação], [Identificador], [Status], [Loja]
                ) AS TB_DINAMICA
                ");
                #endregion

                con.Open();

                comm.Parameters.Add(new SqlParameter("dataInicio", dataInicio));
                comm.Parameters.Add(new SqlParameter("dataFim", dataFim));

                #region QueryGet
                StringBuilder queryGet = new StringBuilder(@"
				    SELECT ").Append(colunasSelect.ToString()).Append(@"
	                FROM ").Append(tabelaDinamica.ToString()).Append(@"
                    ORDER BY TB_DINAMICA.[Formulário]");
                #endregion

                comm.CommandText = queryGet.ToString();

                SqlDataReader rd = comm.ExecuteReader();

                int indiceNumeroLinha = 9;
                string[] colunasForm = "ID_FORMULARIO;ID;ID_STATUS_ATUAL;NOME_RESPONSAVEL;Formulário;Criação;Identificador;Status;Loja".Split(';');
                while (rd.Read())
                {
                    objs.Add(MontarRegistro(rd, colunasForm, 4, indiceNumeroLinha));
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

        private static void AdicionarColunaBuscaAvancada(string[] camposDinamicosFiltro, int indice, ref StringBuilder auxCamposDinamicosFiltro, ref SqlCommand comm, ref int totDinamicosAdicionados)
        {
            string[] nomeValorCampo = camposDinamicosFiltro[indice].Split(';');
            string valorCampo = nomeValorCampo[1];
            if (!string.IsNullOrEmpty(valorCampo))
            {
                string nomeCampo = nomeValorCampo[0];

                if (totDinamicosAdicionados == 0)
                {
                    auxCamposDinamicosFiltro.Append("AND (");
                }
                else
                {
                    auxCamposDinamicosFiltro.Append(" OR ");
                }
                auxCamposDinamicosFiltro.Append(string.Format("(tc.NOME = @campoDinamico{0} AND tcpf.VALOR_VARCHAR collate Latin1_General_CI_AI like @textoFiltroDinamico{1})", indice, indice));

                comm.Parameters.Add(new SqlParameter(string.Format("@campoDinamico{0}", indice), nomeCampo));
                comm.Parameters.Add(new SqlParameter(string.Format("@textoFiltroDinamico{0}", indice), string.Format("%{0}%", valorCampo)));

                totDinamicosAdicionados++;
            }
        }

        public static IList<object> GetBuscaAvancada(Int32 ID_FORMULARIO, string[] colunasForm, string dataInicialFiltro, string dataFinalFiltro, string statusFormFiltro, int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir, string userNameCriador, string[] camposDinamicosFiltro, string idBuscaPreenchimento)
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder colunasFormConca = new StringBuilder();
                StringBuilder colunasSelect = new StringBuilder("TB_DINAMICA.ID,TB_DINAMICA.ID_STATUS_ATUAL,TB_DINAMICA.NOME_RESPONSAVEL,TB_DINAMICA.[Criação],TB_DINAMICA.[Identificador],TB_DINAMICA.[Status],TB_DINAMICA.[Loja],");
                StringBuilder colunasFiltro = new StringBuilder();
                int totColunasFixas = 7;//ID, ID_STATUS_ATUAL, NOME_RESPONSAVEL, DATA_CRIACAO, IDENTIFICADOR, NOME_STATUS_ATUAL e LOJA_CRIADOR
                if (colunasForm.Length > totColunasFixas)
                {
                    string colunaDinamica = string.Format("[{0}]", colunasForm[totColunasFixas]);
                    colunasFormConca.Append(string.Format("MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[totColunasFixas], colunaDinamica));
                    colunasSelect.Append(string.Format("TB_DINAMICA.{0},", colunaDinamica));
                    colunasFiltro.Append(string.Format("TB_DINAMICA.{0} collate Latin1_General_CI_AI like @textoFiltro", colunaDinamica));
                    for (int i = totColunasFixas + 1; i < colunasForm.Length; i++)
                    {
                        colunaDinamica = string.Format("[{0}]", colunasForm[i]);
                        colunasFormConca.Append(string.Format(",MAX(CASE WHEN NOME_COMPONENTE_ = '{0}' THEN VALOR_COMPONENTE_ END) AS {1}", colunasForm[i], colunaDinamica));
                        colunasSelect.Append(string.Format("TB_DINAMICA.{0},", colunaDinamica));
                        colunasFiltro.Append(string.Format(" OR TB_DINAMICA.{0} collate Latin1_General_CI_AI like @textoFiltro", colunaDinamica));
                    }
                    //Adicionando colunas fixas que aparecem no grid ao filtro
                    colunasFiltro.Append(" OR TB_DINAMICA.[Criação] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Identificador] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Status] collate Latin1_General_CI_AI like @textoFiltro");
                    colunasFiltro.Append(" OR TB_DINAMICA.[Loja] collate Latin1_General_CI_AI like @textoFiltro");
                    //Adicionando colunas fixas que aparecem no grid ao filtro

                    string ordenacao;
                    if (string.IsNullOrEmpty(sortColumn))
                    {
                        ordenacao = @"
                        ORDER BY TB_DINAMICA.[Criação]
                        ";
                    }
                    else
                    {
                        //ordenacao = string.Format("ORDER BY ABS(TB_DINAMICA.{0}) {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                        ordenacao = string.Format("ORDER BY TB_DINAMICA.{0} {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                    }

                    string dataInicio = string.Empty;
                    string dataFim = string.Empty;
                    if (!string.IsNullOrEmpty(dataInicialFiltro))
                    {
                        string[] auxDataInicialFiltro = dataInicialFiltro.Split('/');
                        if (auxDataInicialFiltro.Length == 3)
                        {
                            dataInicio = string.Format("{0}-{1}-{2} 00:00:00", auxDataInicialFiltro[2], auxDataInicialFiltro[1], auxDataInicialFiltro[0]);
                        }
                    }
                    if (!string.IsNullOrEmpty(dataFinalFiltro))
                    {
                        string[] auxDataFinalFiltro = dataFinalFiltro.Split('/');
                        if (auxDataFinalFiltro.Length == 3)
                        {
                            dataFim = string.Format("{0}-{1}-{2} 23:59:59", auxDataFinalFiltro[2], auxDataFinalFiltro[1], auxDataFinalFiltro[0]);
                        }
                    }
                    StringBuilder filtroStatus = new StringBuilder();
                    if (!string.IsNullOrEmpty(statusFormFiltro) && !"T".Equals(statusFormFiltro))
                    {
                        filtroStatus.Append("AND tt.ID_STATUS_DESTINO = @statusForm");
                        comm.Parameters.Add(new SqlParameter("statusForm", statusFormFiltro));
                    }
                    filtroStatus.Append(@"
                    AND tt.ID IN 
                    (
			            SELECT MAX(ID)
				            FROM TB_TRAMITACAO
					            WHERE ID_PREENCHIMENTO_FORMULARIO = tpf.ID
		            )
                    ");
                    StringBuilder filtroSomenteMeus = new StringBuilder();
                    if (null != userNameCriador)
                    {
                        filtroSomenteMeus.Append("AND tpf.USER_NAME_CRIADOR = @userNameCriador");
                        comm.Parameters.Add(new SqlParameter("userNameCriador", userNameCriador));
                    }
                    StringBuilder filtroIdBusca = new StringBuilder();
                    if (!string.IsNullOrEmpty(idBuscaPreenchimento))
                    {
                        filtroIdBusca.Append("AND tpf.ID_BUSCA collate Latin1_General_CI_AI like @idBuscaPreenchimento");
                        comm.Parameters.Add(new SqlParameter("idBuscaPreenchimento", string.Format("%{0}%", idBuscaPreenchimento)));
                    }
                    StringBuilder filtroCamposDinamicos = new StringBuilder();
                    if (camposDinamicosFiltro != null && camposDinamicosFiltro.Length > 0)
                    {
                        int totDinamicosAdicionados = 0;
                        StringBuilder auxCamposDinamicosFiltro = new StringBuilder();
                        AdicionarColunaBuscaAvancada(camposDinamicosFiltro, 0, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        for (int i = 1; i < camposDinamicosFiltro.Length; i++)
                        {
                            AdicionarColunaBuscaAvancada(camposDinamicosFiltro, i, ref auxCamposDinamicosFiltro, ref comm, ref totDinamicosAdicionados);
                        }
                        if (totDinamicosAdicionados > 0)
                        {
                            auxCamposDinamicosFiltro.Append(")");
                            filtroCamposDinamicos.Append(@"
                            AND EXISTS 
                            (
                                SELECT tcpf.ID_PREENCHIMENTO_FORMULARIO
                                    FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf, TB_COMPONENTE tc
                                        WHERE tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                                        AND tc.ID = tcpf.ID_COMPONENTE
                                        ").Append(auxCamposDinamicosFiltro.ToString()).Append(@"
                                            GROUP BY tcpf.ID_PREENCHIMENTO_FORMULARIO
                                            HAVING COUNT(tc.ID) = @totDinamicosAdicionados
                            )
                            ");
                            comm.Parameters.Add(new SqlParameter("@totDinamicosAdicionados", totDinamicosAdicionados));
                        }
                    }

                    #region TabelaDinamica
                    StringBuilder fromTabelaDinamica = new StringBuilder(@"
                        FROM TB_PREENCHIMENTO_FORMULARIO tpf
						JOIN TB_TRAMITACAO tt ON tt.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                        JOIN TB_STATUS_FORMULARIO ts ON ts.ID = tt.ID_STATUS_DESTINO 
						LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf ON tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
						LEFT JOIN TB_COMPONENTE tc ON tc.ID = tcpf.ID_COMPONENTE
				            WHERE tpf.DATA_HORA BETWEEN @dataInicio AND @dataFim
                            AND tpf.ID_FORMULARIO = @idFormulario
                            ").Append(filtroStatus.ToString()).Append(@"
                            ").Append(filtroSomenteMeus.ToString()).Append(@"
                            ").Append(filtroIdBusca.ToString()).Append(@"
                            ").Append(filtroCamposDinamicos.ToString());

                    StringBuilder tabelaDinamica = new StringBuilder(@"
                    (
                        SELECT ID_PREENCHIMENTO_FORMULARIO AS ID,ID_STATUS_ATUAL,NOME_RESPONSAVEL,[Criação],[Identificador],[Status],[Loja],").Append(colunasFormConca).Append(@" from 
				        (
				        SELECT 
				        	tpf.ID AS ID_PREENCHIMENTO_FORMULARIO, 
                            tt.ID_STATUS_DESTINO AS ID_STATUS_ATUAL,
				        	tpf.NOME_RESPONSAVEL,
                            CONVERT(VARCHAR,tpf.DATA_HORA,103) AS [Criação],
                            tpf.ID_BUSCA AS [Identificador],
                            ts.NOME AS [Status],
                            tpf.LOJA_CRIADOR AS [Loja],
				        	tcpf.VALOR_VARCHAR,
				        	tc.NOME AS NOME_COMPONENTE_,
							tcpf.VALOR_VARCHAR AS VALOR_COMPONENTE_
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
				        ) tb
				        GROUP BY ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL, NOME_RESPONSAVEL, [Criação], [Identificador], [Status], [Loja]
                    ) AS TB_DINAMICA
                    ");
                    #endregion

                    con.Open();

                    #region QueryGetTotRegistros
                    StringBuilder queryGetTotRegistros = new StringBuilder(@"
                    SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                        FROM (
                            SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
                            GROUP BY tpf.ID
                        ) tbTotRegistrosFiltro
                    ");
                    #endregion

                    comm.CommandText = queryGetTotRegistros.ToString();

                    comm.Parameters.Add(new SqlParameter("idFormulario", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("dataInicio", dataInicio));
                    comm.Parameters.Add(new SqlParameter("dataFim", dataFim));

                    totRegistros = (int)comm.ExecuteScalar();

                    #region QueryGetTotRegistrosFiltro
                    StringBuilder colunasFixasGridFiltro = new StringBuilder(@"OR CONVERT(VARCHAR,tpf.DATA_HORA,103) collate Latin1_General_CI_AI like @textoFiltro
                    OR tpf.ID_BUSCA collate Latin1_General_CI_AI like @textoFiltro
                    OR ts.NOME collate Latin1_General_CI_AI like @textoFiltro
                    OR tpf.LOJA_CRIADOR collate Latin1_General_CI_AI like @textoFiltro");

                    StringBuilder queryGetTotRegistrosFiltro = new StringBuilder(@"
                    SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                        FROM (
                            SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                            ").Append(fromTabelaDinamica.ToString()).Append(@"
                            AND (tcpf.VALOR_VARCHAR collate Latin1_General_CI_AI like @textoFiltro
                            ").Append(colunasFixasGridFiltro.ToString()).Append(@")
                            GROUP BY tpf.ID
                        ) tbTotRegistrosFiltro
                    ");
                    #endregion

                    comm.CommandText = queryGetTotRegistrosFiltro.ToString();

                    comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                    totRegistrosFiltro = (int)comm.ExecuteScalar();

                    #region QueryGet
                    StringBuilder queryGet = new StringBuilder(@"
                    SELECT TOP (@pageSize) *
	                    FROM (
				    		SELECT 
				    		").Append(colunasSelect.ToString()).Append(@"

				    		(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
				    		AS 'numeroLinha'

	                    	FROM ").Append(tabelaDinamica.ToString()).Append(@"
				    			WHERE 
                                ").Append(colunasFiltro.ToString()).Append(@") 
				    as todasLinhas
                    WHERE todasLinhas.numeroLinha > (@start)");
                    #endregion

                    comm.CommandText = queryGet.ToString();

                    comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                    comm.Parameters.Add(new SqlParameter("start", start));

                    SqlDataReader rd = comm.ExecuteReader();

                    int indiceNumeroLinha = Convert.ToInt32(colunasForm.Length);
                    while (rd.Read())
                    {
                        objs.Add(MontarRegistro(rd, colunasForm, 0, indiceNumeroLinha));
                    }

                    rd.Close();
                }
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

        public static IList<object> GetBuscaAvancadaGenerica(int[] formularios, string dataInicialFiltro, string dataFinalFiltro, int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir, string idBuscaPreenchimento, string gruposFiltroUsuarioLogado)
        {
            IList<object> objs = new List<object>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder colunasSelect = new StringBuilder("TB_DINAMICA.ID_FORMULARIO,TB_DINAMICA.[Formulário],TB_DINAMICA.ID,TB_DINAMICA.ID_STATUS_ATUAL,TB_DINAMICA.NOME_RESPONSAVEL,TB_DINAMICA.[Criação],TB_DINAMICA.[Identificador],TB_DINAMICA.[Status],TB_DINAMICA.[Loja],TB_DINAMICA.[Responsável],");
                StringBuilder colunasFiltro = new StringBuilder();

                //Adicionando colunas fixas que aparecem no grid ao filtro
                colunasFiltro.Append(" TB_DINAMICA.[Formulário] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.[Criação] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.[Identificador] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.[Status] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.[Loja] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.[Responsável] collate Latin1_General_CI_AI like @textoFiltro");
                colunasFiltro.Append(" OR TB_DINAMICA.VALORES_COMPONENTES collate Latin1_General_CI_AI like @textoFiltro");
                //Adicionando colunas fixas que aparecem no grid ao filtro

                string ordenacao;
                if (string.IsNullOrEmpty(sortColumn))
                {
                    ordenacao = @"
                    ORDER BY TB_DINAMICA.[Criação]
                    ";
                }
                else
                {
                    //ordenacao = string.Format("ORDER BY ABS(TB_DINAMICA.{0}) {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                    ordenacao = string.Format("ORDER BY TB_DINAMICA.{0} {1}", string.Format("[{0}]", sortColumn), sortColumnDir);
                }

                string dataInicio = string.Empty;
                string dataFim = string.Empty;
                if (!string.IsNullOrEmpty(dataInicialFiltro))
                {
                    string[] auxDataInicialFiltro = dataInicialFiltro.Split('/');
                    if (auxDataInicialFiltro.Length == 3)
                    {
                        dataInicio = string.Format("{0}-{1}-{2} 00:00:00", auxDataInicialFiltro[2], auxDataInicialFiltro[1], auxDataInicialFiltro[0]);
                    }
                }
                if (!string.IsNullOrEmpty(dataFinalFiltro))
                {
                    string[] auxDataFinalFiltro = dataFinalFiltro.Split('/');
                    if (auxDataFinalFiltro.Length == 3)
                    {
                        dataFim = string.Format("{0}-{1}-{2} 23:59:59", auxDataFinalFiltro[2], auxDataFinalFiltro[1], auxDataFinalFiltro[0]);
                    }
                }

                StringBuilder filtroFormularios = new StringBuilder();
                if (formularios != null && formularios.Length > 0)
                {
                    Int32 idFormulario = formularios[0];
                    filtroFormularios.Append(idFormulario);
                    for (int i = 1; i < formularios.Length; i++)
                    {
                        idFormulario = formularios[i];
                        filtroFormularios.Append(",").Append(idFormulario);
                    }
                }
                else
                {
                    filtroFormularios.Append("''");
                }

                StringBuilder filtroPermissaoExibicao = new StringBuilder();
                filtroPermissaoExibicao.Append(@"
                AND 
                (
                    (1 = tf.EXIBIR_TODOS) 
                    OR
                    (EXISTS
                        (
                            SELECT 1 
                                FROM TB_PERMISSAO_EXIBICAO 
                                    WHERE ID_FORMULARIO = tf.ID 
                                    AND NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
                        )
                    )
                )
                ");

                StringBuilder filtroStatus = new StringBuilder();
                filtroStatus.Append(@"
                AND tt.ID IN 
                (
	                SELECT MAX(ID)
		                FROM TB_TRAMITACAO
		                    WHERE ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                )
                ");

                StringBuilder filtroIdBusca = new StringBuilder();
                if (!string.IsNullOrEmpty(idBuscaPreenchimento))
                {
                    filtroIdBusca.Append("AND tpf.ID_BUSCA collate Latin1_General_CI_AI like @idBuscaPreenchimento");
                    comm.Parameters.Add(new SqlParameter("idBuscaPreenchimento", string.Format("%{0}%", idBuscaPreenchimento)));
                }

                #region TabelaDinamica
                StringBuilder fromTabelaDinamica = new StringBuilder(@"
                FROM TB_FORMULARIO tf
		        JOIN TB_PREENCHIMENTO_FORMULARIO tpf ON tpf.ID_FORMULARIO = tf.ID
		        JOIN TB_TRAMITACAO tt ON tt.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
                JOIN TB_STATUS_FORMULARIO ts ON ts.ID = tt.ID_STATUS_DESTINO 
		        LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO tcpf ON tcpf.ID_PREENCHIMENTO_FORMULARIO = tpf.ID
		        LEFT JOIN TB_COMPONENTE tc ON tc.ID = tcpf.ID_COMPONENTE
		            WHERE tpf.DATA_HORA BETWEEN @dataInicio AND @dataFim
                    AND tpf.ID_FORMULARIO IN (").Append(filtroFormularios.ToString()).Append(@")
                    ").Append(filtroPermissaoExibicao.ToString()).Append(@"
                    ").Append(filtroStatus.ToString()).Append(@"
                    ").Append(filtroIdBusca.ToString());

                StringBuilder tabelaDinamica = new StringBuilder(@"
                (
                    SELECT ID_FORMULARIO, [Formulário], ID_PREENCHIMENTO_FORMULARIO AS ID,ID_STATUS_ATUAL,NOME_RESPONSAVEL,[Criação],[Identificador],[Status],[Loja],[Responsável] 
                    ,STUFF((SELECT ' ', VALOR_VARCHAR
                                FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
                                WHERE ID_PREENCHIMENTO_FORMULARIO = tb.ID_PREENCHIMENTO_FORMULARIO
                        FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)') 
                    ,1,1,'') AS VALORES_COMPONENTES
                    FROM 
		            (
		            SELECT 
                        tf.ID AS ID_FORMULARIO,
		        		tf.NOME AS [Formulário],
		            	tpf.ID AS ID_PREENCHIMENTO_FORMULARIO, 
                        tt.ID_STATUS_DESTINO AS ID_STATUS_ATUAL,
		            	tpf.NOME_RESPONSAVEL,
                        CONVERT(VARCHAR,tpf.DATA_HORA,103) AS [Criação],
                        tpf.ID_BUSCA AS [Identificador],
                        ts.NOME AS [Status],
                        tpf.LOJA_CRIADOR AS [Loja],
                        tpf.NOME_CRIADOR AS [Responsável],
		            	tcpf.VALOR_VARCHAR,
		            	tc.NOME AS NOME_COMPONENTE_,
		        	tcpf.VALOR_VARCHAR AS VALOR_COMPONENTE_
                        ").Append(fromTabelaDinamica.ToString()).Append(@"
		            ) tb
		            GROUP BY ID_FORMULARIO, [Formulário], ID_PREENCHIMENTO_FORMULARIO, ID_STATUS_ATUAL, NOME_RESPONSAVEL, [Criação], [Identificador], [Status], [Loja], [Responsável]
                ) AS TB_DINAMICA
                ");
                #endregion

                con.Open();

                #region QueryGetTotRegistros
                StringBuilder queryGetTotRegistros = new StringBuilder(@"
                SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                    FROM (
                        SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                        ").Append(fromTabelaDinamica.ToString()).Append(@"
                        GROUP BY tpf.ID
                    ) tbTotRegistrosFiltro
                ");
                #endregion

                comm.CommandText = queryGetTotRegistros.ToString();

                comm.Parameters.Add(new SqlParameter("dataInicio", dataInicio));
                comm.Parameters.Add(new SqlParameter("dataFim", dataFim));

                totRegistros = (int)comm.ExecuteScalar();

                #region QueryGetTotRegistrosFiltro
                StringBuilder colunasFixasGridFiltro = new StringBuilder(@"
                OR tf.NOME collate Latin1_General_CI_AI like @textoFiltro
                OR CONVERT(VARCHAR,tpf.DATA_HORA,103) collate Latin1_General_CI_AI like @textoFiltro
                OR tpf.ID_BUSCA collate Latin1_General_CI_AI like @textoFiltro
                OR ts.NOME collate Latin1_General_CI_AI like @textoFiltro
                OR tpf.LOJA_CRIADOR collate Latin1_General_CI_AI like @textoFiltro
				OR tpf.NOME_CRIADOR collate Latin1_General_CI_AI like @textoFiltro");

                StringBuilder queryGetTotRegistrosFiltro = new StringBuilder(@"
                SELECT COUNT(tbTotRegistrosFiltro.ID_PREENCHIMENTO_FORMULARIO) 
                    FROM (
                        SELECT tpf.ID AS ID_PREENCHIMENTO_FORMULARIO
                        ").Append(fromTabelaDinamica.ToString()).Append(@"
                        AND (tcpf.VALOR_VARCHAR collate Latin1_General_CI_AI like @textoFiltro
                        ").Append(colunasFixasGridFiltro.ToString()).Append(@")
                        GROUP BY tpf.ID
                    ) tbTotRegistrosFiltro
                ");
                #endregion

                comm.CommandText = queryGetTotRegistrosFiltro.ToString();

                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                totRegistrosFiltro = (int)comm.ExecuteScalar();

                #region QueryGet
                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
                    FROM (
		        		SELECT 
		        		").Append(colunasSelect.ToString()).Append(@"

		        		(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
		        		AS 'numeroLinha'

                    	FROM ").Append(tabelaDinamica.ToString()).Append(@"
		        			WHERE 
                            ").Append(colunasFiltro.ToString()).Append(@") 
		        as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");
                #endregion

                comm.CommandText = queryGet.ToString();

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));

                SqlDataReader rd = comm.ExecuteReader();

                int indiceNumeroLinha = 10;
                string[] colunasForm = "ID_FORMULARIO;Formulário;ID;ID_STATUS_ATUAL;NOME_RESPONSAVEL;Criação;Identificador;Status;Loja;Responsável".Split(';');
                while (rd.Read())
                {
                    objs.Add(MontarRegistro(rd, colunasForm, 0, indiceNumeroLinha));
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

        public static IList<ComponentePreenchimento> GetPorPreenchimento(Int32 ID_FORMULARIO, Int32 ID_PREENCHIMENTO_FORMULARIO, Int32? ID_STATUS_REGRA, string gruposFiltroUsuarioLogado)
        {
            IList<ComponentePreenchimento> objs = new List<ComponentePreenchimento>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                StringBuilder queryGet = new StringBuilder();
                if (ID_STATUS_REGRA != null && null != gruposFiltroUsuarioLogado)
                {
                    queryGet.Append(@"
				    SELECT TB.* FROM (SELECT
	                NULL AS ID,
	                NULL AS VALOR_VARCHAR,
                    componente.ID AS ID_COMPONENTE, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
	                tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
	                tipoLista.ID AS ID_TIPO_LISTA,
	                NULL AS ID_ITEM_LISTA,
                    permissaoComponente.TIPO AS TIPO_PERMISSAO_STATUS
	
                    FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE

                    LEFT JOIN TB_PERMISSAO_COMPONENTE permissaoComponente
					ON permissaoComponente.NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
					AND permissaoComponente.ID_STATUS = @ID_STATUS_REGRA
					AND permissaoComponente.ID_COMPONENTE = componente.ID
		
                    WHERE componente.ID_FORMULARIO = @ID_FORMULARIO
		            AND componente.ID NOT IN 
		            (
		            	SELECT ID_COMPONENTE 
		            		FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
		            			WHERE ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
		            )

                    UNION

                    SELECT 
                    componenteForm.ID,
                    componenteForm.VALOR_VARCHAR,
				    componente.ID AS ID_COMPONENTE, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
                    tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
                    tipoLista.ID AS ID_TIPO_LISTA,
                    itemLista.ID AS ID_ITEM_LISTA,
                    permissaoComponente.TIPO AS TIPO_PERMISSAO_STATUS

	                FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO componenteForm ON componenteForm.ID_COMPONENTE = componente.ID
                    LEFT JOIN TB_ITEM_LISTA itemLista ON itemLista.ID = componenteForm.ID_ITEM_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE

                    LEFT JOIN TB_PERMISSAO_COMPONENTE permissaoComponente
					ON permissaoComponente.NOME_GRUPO IN (").Append(gruposFiltroUsuarioLogado).Append(@")
					AND permissaoComponente.ID_STATUS = @ID_STATUS_REGRA
					AND permissaoComponente.ID_COMPONENTE = componente.ID

                    WHERE componenteForm.ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
                    ) TB
                    
                    ORDER BY TB.ORDEM");

                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("ID_STATUS_REGRA", ID_STATUS_REGRA));
                }
                else
                {
                    queryGet.Append(@"
				    SELECT TB.* FROM (SELECT
	                NULL AS ID,
	                NULL AS VALOR_VARCHAR,
                    componente.ID AS ID_COMPONENTE, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
	                tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
	                tipoLista.ID AS ID_TIPO_LISTA,
	                NULL AS ID_ITEM_LISTA
	
                    FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE
		
                    WHERE componente.ID_FORMULARIO = @ID_FORMULARIO
		            AND componente.ID NOT IN 
		            (
		            	SELECT ID_COMPONENTE 
		            		FROM TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
		            			WHERE ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
		            )

                    UNION

                    SELECT 
                    componenteForm.ID,
                    componenteForm.VALOR_VARCHAR,
				    componente.ID AS ID_COMPONENTE, 
				    componente.NOME,
                    componente.DESCRICAO,
                    componente.ORDEM,
                    componente.OBRIGATORIEDADE,
                    componente.EXIBIR_NO_GRID,
                    componente.EXIBIR_NO_LANCAMENTO,
                    componente.EXIBIR_NO_ATENDIMENTO,
                    componente.EXIBIR_NA_BUSCA_AVANCADA,
                    componente.CAIXA_ALTA,
                    componente.TAMANHO,
                    mascaraComponente.ID AS ID_MASCARA_COMPONENTE,
                    mascaraComponente.CODIGO AS CODIGO_MASCARA_COMPONENTE,
                    validadorComponente.ID AS ID_VALIDADOR_COMPONENTE,
                    validadorComponente.CODIGO AS CODIGO_VALIDADOR_COMPONENTE,
                    tipoComponente.CODIGO AS CODIGO_TIPO_COMPONENTE,
                    tipoLista.ID AS ID_TIPO_LISTA,
                    itemLista.ID AS ID_ITEM_LISTA

	                FROM TB_COMPONENTE componente 
                    JOIN TB_TIPO_COMPONENTE tipoComponente ON tipoComponente.ID = componente.ID_TIPO_COMPONENTE
                    LEFT JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = componente.ID_TIPO_LISTA
                    LEFT JOIN TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO componenteForm ON componenteForm.ID_COMPONENTE = componente.ID
                    LEFT JOIN TB_ITEM_LISTA itemLista ON itemLista.ID = componenteForm.ID_ITEM_LISTA
                    LEFT JOIN TB_MASCARA_COMPONENTE mascaraComponente ON mascaraComponente.ID = componente.ID_MASCARA_COMPONENTE
                    LEFT JOIN TB_VALIDADOR_COMPONENTE validadorComponente ON validadorComponente.ID = componente.ID_VALIDADOR_COMPONENTE

                    WHERE componenteForm.ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
                    ) TB
                    
                    ORDER BY TB.ORDEM");

                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));
                }

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ComponentePreenchimento obj;
                while (rd.Read())
                {
                    obj = new ComponentePreenchimento
                    {
                        ID = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                        VALOR_VARCHAR = rd.IsDBNull(1) ? null : rd.GetString(1),
                        componente = new Componente
                        {
                            ID = rd.GetInt32(2),
                            NOME = rd.GetString(3),
                            DESCRICAO = rd.IsDBNull(4) ? string.Empty : rd.GetString(4),
                            ORDEM = rd.GetInt32(5),
                            OBRIGATORIEDADE = rd.GetBoolean(6) ? 1 : 0,
                            EXIBIR_NO_GRID = rd.GetBoolean(7) ? 1 : 0,
                            EXIBIR_NO_LANCAMENTO = rd.GetBoolean(8) ? 1 : 0,
                            EXIBIR_NO_ATENDIMENTO = rd.GetBoolean(9) ? 1 : 0,
                            EXIBIR_NA_BUSCA_AVANCADA = rd.GetBoolean(10) ? 1 : 0,
                            CAIXA_ALTA = rd.GetBoolean(11) ? 1 : 0,
                            TAMANHO = rd.IsDBNull(12) ? null : (Int32?)rd.GetInt32(12),
                            mascaraComponente = rd.IsDBNull(13) ?
                            new MascaraComponente() : new MascaraComponente
                            {
                                ID = rd.GetInt32(13),
                                CODIGO = rd.GetInt32(14)
                            },
                            validadorComponente = rd.IsDBNull(15) ?
                            new ValidadorComponente() : new ValidadorComponente
                            {
                                ID = rd.GetInt32(15),
                                CODIGO = rd.GetInt32(16)
                            },
                            tipoComponente = new TipoComponente { CODIGO = rd.GetInt32(17) },
                            tipoLista = rd.IsDBNull(18) ?
                            new TipoLista() : new TipoLista { ID = rd.GetInt32(18) },
                            tipoPermissaoStatus =
                            ID_STATUS_REGRA != null
                            && null != gruposFiltroUsuarioLogado
                            && !rd.IsDBNull(20) ? rd.GetInt32(20) : (Int32?)null
                        },
                        itemLista = rd.IsDBNull(19) ?
                        new ItemLista() : new ItemLista { ID = rd.GetInt32(19) }
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

        private static void AdicionarTextoMsgResult(ref StringBuilder auxResult, string texto, string ID_BUSCA_FORMULARIO)
        {
            texto = string.Format(texto, string.Format("<b>{0}</b>", ID_BUSCA_FORMULARIO));
            if (string.IsNullOrEmpty(auxResult.ToString()))
            {
                auxResult.Append(texto);
            }
            else
            {
                auxResult.Append(string.Format("<br/>{0}", texto));
            }
        }

        public static String Atender(IList<Tramitacao> tramitacoes, StatusFormulario statusEnvioEmail, string[] dadosEnvioEmail, bool enviarEmail, Usuario usuarioLogado)
        {
            if (null == tramitacoes || tramitacoes.Count == 0)
            {
                return "Nenhum registro foi selecionado";
            }
            String msgResult = null;
            String emailDestino;
            String nomeCriador;
            String nomeFormulario;
            String idBusca;
            IDictionary<string, string> coringasEmail;
            String msgErroEnvioEmail;
            StringBuilder auxResult = new StringBuilder();
            String textoPadraoFalha = "Falha ao tentar atender o formulário {0}, favor tente novamente";
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("insertTramitacoesPreenchimentos");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    int? ID_STATUS_ORIGEM;
                    bool podeAtender;
                    bool? formularioLivre;
                    Int32? idStatusAtual;
                    SqlParameter parResult;
                    foreach (Tramitacao obj in tramitacoes)
                    {
                        try
                        {
                            ID_STATUS_ORIGEM = null;
                            if (obj.statusOrigem != null)
                            {
                                ID_STATUS_ORIGEM = obj.statusOrigem.ID;
                            }

                            podeAtender = true;

                            #region VerificarBloqueioFormulario
                            try
                            {
                                Parametro limiteTempoFormAberto = ParametroDAL.GetPorNome(ParametroDAL.MINUTOS_BLOQUEIO_FORMULARIO);

                                comm.CommandType = System.Data.CommandType.StoredProcedure;
                                comm.CommandText = "PROC_VERIFICAR_BLOQUEIO_FORM";
                                comm.Parameters.Clear();

                                parResult = new SqlParameter("result", System.Data.SqlDbType.Bit);
                                parResult.Direction = System.Data.ParameterDirection.Output;

                                comm.Parameters.Add(new SqlParameter("idFormulario", obj.ID_PREENCHIMENTO_FORMULARIO));
                                comm.Parameters.Add(new SqlParameter("userNameUsuarioLogado", usuarioLogado.USER_NAME));//"ATENDIMENTO"
                                comm.Parameters.Add(new SqlParameter("nomeUsuarioLogado", usuarioLogado.NOME));
                                comm.Parameters.Add(new SqlParameter("bloquearForm", false));
                                comm.Parameters.Add(new SqlParameter("limiteTempoFormAberto", Convert.ToInt32(limiteTempoFormAberto.VALOR)));
                                comm.Parameters.Add(parResult);

                                comm.ExecuteNonQuery();

                                formularioLivre = (bool?)parResult.Value;
                            }
                            catch (Exception ex)
                            {
                                formularioLivre = null;
                            }

                            if (null == formularioLivre)
                            {
                                AdicionarTextoMsgResult(ref auxResult, "Falha ao tentar verificar a trava lógica do formulário {0}, favor tente novamente", obj.ID_BUSCA_FORMULARIO);
                                podeAtender = false;
                            }
                            else if (formularioLivre == false)
                            {
                                AdicionarTextoMsgResult(ref auxResult, "O formulário {0} se encontra aberto por outro usuário, favor tente novamente mais tarde", obj.ID_BUSCA_FORMULARIO);
                                podeAtender = false;
                            }
                            #endregion

                            comm.CommandType = System.Data.CommandType.Text;

                            #region VerificarStatusAtualFormulario
                            if (true == formularioLivre)
                            {
                                try
                                {
                                    comm.CommandText = @"
                                    SELECT ID_STATUS_DESTINO
                                    FROM TB_TRAMITACAO
                                    WHERE ID IN 
                                    (
                                        SELECT MAX(ID)
                                            FROM TB_TRAMITACAO
                                                WHERE ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
                                    )";
                                    comm.Parameters.Clear();

                                    comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", obj.ID_PREENCHIMENTO_FORMULARIO));

                                    SqlDataReader rd = comm.ExecuteReader();

                                    if (rd.Read())
                                    {
                                        idStatusAtual = rd.GetInt32(0);
                                    }
                                    else
                                    {
                                        idStatusAtual = 0;
                                    }
                                    rd.Close();
                                }
                                catch (Exception ex)
                                {
                                    idStatusAtual = null;
                                }

                                if (idStatusAtual == null)
                                {
                                    AdicionarTextoMsgResult(ref auxResult, "Falha ao tentar verificar o status atual do formulário {0}, favor tente novamente", obj.ID_BUSCA_FORMULARIO);
                                    podeAtender = false;
                                }
                                else if (idStatusAtual != ID_STATUS_ORIGEM)
                                {
                                    AdicionarTextoMsgResult(ref auxResult, "Operação não permitida: o status do formulário {0} foi alterado, favor atualizar a consulta", obj.ID_BUSCA_FORMULARIO);
                                    podeAtender = false;
                                }
                            }
                            #endregion

                            if (podeAtender)
                            {
                                comm.CommandText = @"
                                INSERT INTO TB_TRAMITACAO
                                    (USER_NAME_RESPONSAVEL,
                                    NOME_RESPONSAVEL,
                                    DATA_HORA,
                                    OBSERVACAO,
                                    ID_PREENCHIMENTO_FORMULARIO,
                                    ID_STATUS_ORIGEM,
                                    ID_STATUS_DESTINO)
                                VALUES
                                    (@USER_NAME_RESPONSAVEL,
                                    @NOME_RESPONSAVEL,
                                    GETDATE(),
                                    @OBSERVACAO,
                                    @ID_PREENCHIMENTO_FORMULARIO,
                                    @ID_STATUS_ORIGEM,
                                    @ID_STATUS_DESTINO)";
                                comm.Parameters.Clear();

                                comm.Parameters.Add(new SqlParameter("USER_NAME_RESPONSAVEL", usuarioLogado.USER_NAME));
                                comm.Parameters.Add(new SqlParameter("NOME_RESPONSAVEL", usuarioLogado.NOME));
                                comm.Parameters.Add(new SqlParameter("OBSERVACAO", obj.OBSERVACAO));
                                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", obj.ID_PREENCHIMENTO_FORMULARIO));
                                comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", ID_STATUS_ORIGEM));
                                comm.Parameters.Add(new SqlParameter("ID_STATUS_DESTINO", obj.statusDestino.ID));

                                if (comm.ExecuteNonQuery() == 0)
                                {
                                    AdicionarTextoMsgResult(ref auxResult, textoPadraoFalha, obj.ID_BUSCA_FORMULARIO);
                                }
                                else if (enviarEmail)
                                {
                                    emailDestino = Util.ObterEmailUsuario(PreenchimentoDAL.ObterUserNameCriador(obj.ID_PREENCHIMENTO_FORMULARIO));
                                    if (string.IsNullOrEmpty(emailDestino))
                                    {
                                        AdicionarTextoMsgResult(ref auxResult, "Falha ao tentar enviar o email: endereço de destino não encontrado (formulário {0})", obj.ID_BUSCA_FORMULARIO);
                                    }
                                    else
                                    {
                                        nomeCriador = string.Empty;
                                        idBusca = string.Empty;
                                        nomeFormulario = string.Empty;
                                        PreenchimentoDAL.ObterDadosParaCoringasEmail(obj.ID_PREENCHIMENTO_FORMULARIO, ref nomeCriador, ref idBusca, ref nomeFormulario);
                                        coringasEmail = Util.MontarListaCoringasEmail(nomeCriador, idBusca, nomeFormulario, obj.OBSERVACAO);
                                        msgErroEnvioEmail = Util.EnviarEmail(emailDestino, statusEnvioEmail.TITULO_EMAIL, statusEnvioEmail.CORPO_EMAIL, dadosEnvioEmail, coringasEmail);
                                        if (!string.IsNullOrEmpty(msgErroEnvioEmail))
                                        {
                                            AdicionarTextoMsgResult(ref auxResult, string.Format("{0} {1}", msgErroEnvioEmail, "(formulário {0})"), obj.ID_BUSCA_FORMULARIO);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            AdicionarTextoMsgResult(ref auxResult, textoPadraoFalha, obj.ID_BUSCA_FORMULARIO);
                        }
                    }

                    trans.Commit();
                    msgResult = auxResult.ToString();
                }
                catch
                {
                    msgResult = null;
                    trans.Rollback();
                }
                finally
                {
                    con.Close();
                }
            }
            return msgResult;
        }

        public static string Insert(Preenchimento obj, Tramitacao tro, Usuario usuarioLogado, ref Int32? idPreenchimentoInserido)
        {
            int? nrLinhas;
            string idBusca = null;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("insertPreenchimentoFormulario");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    comm.CommandText = string.Format("SELECT {0}.dbo.FUNC_GERAR_ID_BUSCA()", con.Database);

                    SqlDataReader rd = comm.ExecuteReader();
                    if (rd.Read())
                    {
                        idBusca = rd.GetString(0);
                    }
                    rd.Close();
                    if (string.IsNullOrEmpty(idBusca))
                    {
                        throw new Exception();
                    }

                    comm.CommandText = @"
                    INSERT INTO TB_PREENCHIMENTO_FORMULARIO
                        (ID_BUSCA,
                        ID_FORMULARIO,
                        USER_NAME_CRIADOR,
                        NOME_CRIADOR,
                        LOJA_CRIADOR,
                        DATA_HORA)
                    VALUES
                        (@ID_BUSCA,
                        @ID_FORMULARIO,
                        @USER_NAME_CRIADOR,
                        @NOME_CRIADOR,
                        @LOJA_CRIADOR,
                        GETDATE())
                    SELECT SCOPE_IDENTITY();";

                    comm.Parameters.Add(new SqlParameter("ID_BUSCA", idBusca));
                    comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", obj.ID_FORMULARIO));
                    comm.Parameters.Add(new SqlParameter("USER_NAME_CRIADOR", usuarioLogado.USER_NAME));
                    comm.Parameters.Add(new SqlParameter("NOME_CRIADOR", usuarioLogado.NOME));
                    comm.Parameters.Add(new SqlParameter("LOJA_CRIADOR", usuarioLogado.LOJA));

                    object auxIdPreenchimentoInserido = comm.ExecuteScalar();
                    idPreenchimentoInserido = (auxIdPreenchimentoInserido == null || Convert.IsDBNull(auxIdPreenchimentoInserido)) ? (Int32?)null : Convert.ToInt32(auxIdPreenchimentoInserido);
                    if (idPreenchimentoInserido != null)
                    {
                        foreach (ComponentePreenchimento compPreenchimento in obj.componentesPreenchimento)
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
                            comm.Parameters.Clear();

                            object idItemLista = DBNull.Value;
                            if (compPreenchimento.itemLista != null && 0 != compPreenchimento.itemLista.ID)
                            {
                                idItemLista = compPreenchimento.itemLista.ID;
                            }

                            comm.Parameters.Add(new SqlParameter("ID_COMPONENTE", compPreenchimento.componente.ID));
                            comm.Parameters.Add(new SqlParameter("VALOR_VARCHAR", compPreenchimento.VALOR_VARCHAR));
                            comm.Parameters.Add(new SqlParameter("ID_ITEM_LISTA", idItemLista));
                            comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", idPreenchimentoInserido));
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
                            ID_STATUS_DESTINO)
                        VALUES
                            (@USER_NAME_RESPONSAVEL,
                            @NOME_RESPONSAVEL,
                            GETDATE(),
                            @OBSERVACAO,
                            @ID_PREENCHIMENTO_FORMULARIO,
                            @ID_STATUS_DESTINO)
                        SELECT SCOPE_IDENTITY();";

                        comm.Parameters.Clear();
                        comm.Parameters.Add(new SqlParameter("USER_NAME_RESPONSAVEL", usuarioLogado.USER_NAME));
                        comm.Parameters.Add(new SqlParameter("NOME_RESPONSAVEL", usuarioLogado.NOME));
                        comm.Parameters.Add(new SqlParameter("OBSERVACAO", tro.OBSERVACAO));
                        comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", idPreenchimentoInserido));
                        comm.Parameters.Add(new SqlParameter("ID_STATUS_DESTINO", tro.statusDestino.ID));

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
                    else
                    {
                        nrLinhas = null;
                        trans.Rollback();
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
            return nrLinhas != null && nrLinhas > 0 ? idBusca : null;
        }

        public static int? Update(Preenchimento obj, Tramitacao tro, Usuario usuarioLogado)
        {
            int? nrLinhas;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("updatePreenchimentoFormulario");

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
                            comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", obj.ID));
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
                            ID_STATUS_DESTINO,
                            LOG_ALTERACAO_COMPONENTES)
                        VALUES
                            (@USER_NAME_RESPONSAVEL,
                            @NOME_RESPONSAVEL,
                            GETDATE(),
                            @OBSERVACAO,
                            @ID_PREENCHIMENTO_FORMULARIO,
                            @ID_STATUS_DESTINO,
                            @LOG_ALTERACAO_COMPONENTES)
                        SELECT SCOPE_IDENTITY();";

                    comm.Parameters.Clear();

                    object auxLogAlteracaoComponentes = DBNull.Value;
                    if (!string.IsNullOrEmpty(tro.LOG_ALTERACAO_COMPONENTES))
                    {
                        auxLogAlteracaoComponentes = tro.LOG_ALTERACAO_COMPONENTES;
                    }

                    comm.Parameters.Add(new SqlParameter("USER_NAME_RESPONSAVEL", usuarioLogado.USER_NAME));
                    comm.Parameters.Add(new SqlParameter("NOME_RESPONSAVEL", usuarioLogado.NOME));
                    comm.Parameters.Add(new SqlParameter("OBSERVACAO", tro.OBSERVACAO));
                    comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", obj.ID));
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

        public static int? Delete(Preenchimento obj)
        {
            int? nrLinhas;
            using (SqlConnection con = new SqlConnection(Util.CONNECTION_STRING))
            {
                con.Open();

                SqlCommand comm = con.CreateCommand();
                SqlTransaction trans = con.BeginTransaction("deletePreenchimentoFormulario");

                comm.Connection = con;
                comm.Transaction = trans;

                try
                {
                    comm.CommandText = @"
                    DELETE TBR_COMPONENTE_PREENCHIMENTO_FORMULARIO
                    WHERE ID_PREENCHIMENTO_FORMULARIO = @idPreenchimentoForm";
                    comm.Parameters.Add(new SqlParameter("idPreenchimentoForm", obj.ID));
                    comm.ExecuteNonQuery();

                    comm.CommandText = @"
                    DELETE TB_ARQUIVO_TRAMITACAO
                    WHERE ID_TRAMITACAO IN 
                    (SELECT ID FROM TB_TRAMITACAO
                    WHERE ID_PREENCHIMENTO_FORMULARIO = @idPreenchimentoForm)";
                    comm.Parameters.Clear();
                    comm.Parameters.Add(new SqlParameter("idPreenchimentoForm", obj.ID));
                    comm.ExecuteNonQuery();

                    comm.CommandText = @"
                    DELETE TB_TRAMITACAO
                    WHERE ID_PREENCHIMENTO_FORMULARIO = @idPreenchimentoForm";
                    comm.Parameters.Clear();
                    comm.Parameters.Add(new SqlParameter("idPreenchimentoForm", obj.ID));
                    comm.ExecuteNonQuery();

                    comm.CommandText = @"
                    DELETE TB_PREENCHIMENTO_FORMULARIO
                    WHERE ID = @ID";
                    comm.Parameters.Clear();
                    comm.Parameters.Add(new SqlParameter("ID", obj.ID));
                    nrLinhas = comm.ExecuteNonQuery();

                    trans.Commit();
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

        public static bool? VerificarBloqueio(Int32 ID_PREENCHIMENTO_FORMULARIO, String userNameUsuarioLogado, String nomeUsuarioLogado, bool bloquearForm)
        {
            bool? result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                Parametro limiteTempoFormAberto = ParametroDAL.GetPorNome(ParametroDAL.MINUTOS_BLOQUEIO_FORMULARIO);

                SqlCommand comm = new SqlCommand();
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection = con;

                comm.CommandText = "PROC_VERIFICAR_BLOQUEIO_FORM";

                con.Open();

                SqlParameter parResult = new SqlParameter("result", System.Data.SqlDbType.Bit);
                parResult.Direction = System.Data.ParameterDirection.Output;

                comm.Parameters.Add(new SqlParameter("idFormulario", ID_PREENCHIMENTO_FORMULARIO));
                comm.Parameters.Add(new SqlParameter("userNameUsuarioLogado", userNameUsuarioLogado));
                comm.Parameters.Add(new SqlParameter("nomeUsuarioLogado", nomeUsuarioLogado));
                comm.Parameters.Add(new SqlParameter("bloquearForm", bloquearForm ? 1 : 0));
                comm.Parameters.Add(new SqlParameter("limiteTempoFormAberto", Convert.ToInt32(limiteTempoFormAberto.VALOR)));
                comm.Parameters.Add(parResult);

                comm.ExecuteNonQuery();

                result = (bool?)parResult.Value;
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

        public static Int32? ObterStatusAtual(Int32 ID_PREENCHIMENTO_FORMULARIO)
        {
            Int32? result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT ID_STATUS_DESTINO
                FROM TB_TRAMITACAO
                WHERE ID IN 
                (
                    SELECT MAX(ID)
                        FROM TB_TRAMITACAO
                            WHERE ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO
                )";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = rd.GetInt32(0);
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

        public static String ObterUserNameCriador(Int32 ID_PREENCHIMENTO_FORMULARIO)
        {
            String result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT USER_NAME_CRIADOR

                FROM TB_PREENCHIMENTO_FORMULARIO

                WHERE ID = @ID_PREENCHIMENTO_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));

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

        public static void ObterDadosParaCoringasEmail(Int32 ID_PREENCHIMENTO_FORMULARIO, ref String nomeCriador, ref String idBusca, ref String nomeFormulario)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT tpf.NOME_CRIADOR, tpf.ID_BUSCA, tf.NOME

                FROM TB_PREENCHIMENTO_FORMULARIO tpf JOIN TB_FORMULARIO tf ON tf.ID = tpf.ID_FORMULARIO

                WHERE tpf.ID = @ID_PREENCHIMENTO_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", ID_PREENCHIMENTO_FORMULARIO));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    nomeCriador = rd.GetString(0);
                    idBusca = rd.GetString(1);
                    nomeFormulario = rd.GetString(2);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                nomeCriador = string.Empty;
                idBusca = string.Empty;
                nomeFormulario = string.Empty;
            }
            finally
            {
                con.Close();
            }
        }
    }
}