using CencosudCSCWEBMVC.Models.TO;
using CencosudCSCWEBMVC.Models.TO.Excel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class FluxoStatusDAL
    {
        public static IList<FluxoStatus> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<FluxoStatus> objs = new List<FluxoStatus>();

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
                    ORDER BY statusOrigem.NOME
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME_STATUS_ORIGEM asc":
                            ordenacao = "ORDER BY statusOrigem.NOME";
                            break;
                        case "NOME_STATUS_DESTINO asc":
                            ordenacao = "ORDER BY statusDestino.NOME";
                            break;
                        case "NOME_FORMULARIO asc":
                            ordenacao = "ORDER BY formulario.NOME";
                            break;
                        case "NOME_STATUS_ORIGEM desc":
                            ordenacao = "ORDER BY statusOrigem.NOME DESC";
                            break;
                        case "NOME_STATUS_DESTINO desc":
                            ordenacao = "ORDER BY statusDestino.NOME DESC";
                            break;
                        case "NOME_FORMULARIO desc":
                            ordenacao = "ORDER BY formulario.NOME DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY statusOrigem.NOME
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
						statusOrigem.ID AS ID_STATUS_ORIGEM,
                        statusOrigem.NOME AS NOME_STATUS_ORIGEM,
                        statusOrigem.INICIAL AS INICIAL_STATUS_ORIGEM,
                        statusDestino.ID AS ID_STATUS_DESTINO,
                        statusDestino.NOME AS NOME_STATUS_DESTINO,
                        statusDestino.INICIAL AS INICIAL_STATUS_DESTINO,
                        formulario.ID AS ID_FORMULARIO,
                        formulario.NOME AS NOME_FORMULARIO,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID_STATUS_ORIGEM) FROM TB_FLUXO_STATUS) 
						AS 'totRegistros', 

						(SELECT COUNT(fluxoStatus.ID_STATUS_ORIGEM) 
                            FROM TB_FLUXO_STATUS fluxoStatus 
                            JOIN TB_STATUS_FORMULARIO statusOrigem ON statusOrigem.ID = fluxoStatus.ID_STATUS_ORIGEM
                            JOIN TB_STATUS_FORMULARIO statusDestino ON statusDestino.ID = fluxoStatus.ID_STATUS_DESTINO
                            JOIN TB_FORMULARIO formulario ON formulario.ID = statusOrigem.ID_FORMULARIO
						        WHERE 
                                statusOrigem.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                statusDestino.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                formulario.NOME collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_FLUXO_STATUS fluxoStatus 
                            JOIN TB_STATUS_FORMULARIO statusOrigem ON statusOrigem.ID = fluxoStatus.ID_STATUS_ORIGEM
                            JOIN TB_STATUS_FORMULARIO statusDestino ON statusDestino.ID = fluxoStatus.ID_STATUS_DESTINO
                            JOIN TB_FORMULARIO formulario ON formulario.ID = statusOrigem.ID_FORMULARIO
						        WHERE 
                                statusOrigem.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                statusDestino.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                formulario.NOME collate Latin1_General_CI_AI like @textoFiltro) 
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                FluxoStatus obj;
                Formulario formulario;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(9);
                    totRegistrosFiltro = rd.GetInt32(10);

                    obj = new FluxoStatus
                    {
                        statusOrigem = new StatusFormulario
                        {
                            ID = rd.GetInt32(0),
                            NOME = rd.GetString(1),
                            INICIAL = rd.GetBoolean(2) ? 1 : 0
                        },
                        statusDestino = new StatusFormulario
                        {
                            ID = rd.GetInt32(3),
                            NOME = rd.GetString(4),
                            INICIAL = rd.GetBoolean(5) ? 1 : 0
                        }
                    };
                    formulario = new Formulario
                    {
                        ID = rd.GetInt32(6),
                        NOME = rd.GetString(7)
                    };
                    obj.statusOrigem.formulario = formulario;
                    obj.statusDestino.formulario = formulario;

                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new FluxoStatus
                    {
                        statusOrigem = new StatusFormulario
                        {
                            ID = rd.GetInt32(0),
                            NOME = rd.GetString(1),
                            INICIAL = rd.GetBoolean(2) ? 1 : 0
                        },
                        statusDestino = new StatusFormulario
                        {
                            ID = rd.GetInt32(3),
                            NOME = rd.GetString(4),
                            INICIAL = rd.GetBoolean(5) ? 1 : 0
                        }
                    };
                    formulario = new Formulario
                    {
                        ID = rd.GetInt32(6),
                        NOME = rd.GetString(7)
                    };
                    obj.statusOrigem.formulario = formulario;
                    obj.statusDestino.formulario = formulario;

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

        public static IList<FluxoStatusExcel> GetParaExcel()
        {
            IList<FluxoStatusExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                FORMULARIO = formulario.NOME,
                ORIGEM = statusOrigem.NOME,
                DESTINO = statusDestino.NOME
        
                FROM TB_FLUXO_STATUS fluxoStatus 
                JOIN TB_STATUS_FORMULARIO statusOrigem ON statusOrigem.ID = fluxoStatus.ID_STATUS_ORIGEM
                JOIN TB_STATUS_FORMULARIO statusDestino ON statusDestino.ID = fluxoStatus.ID_STATUS_DESTINO
                JOIN TB_FORMULARIO formulario ON formulario.ID = statusOrigem.ID_FORMULARIO
    
                ORDER BY formulario.NOME, statusOrigem.NOME, statusDestino.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<FluxoStatusExcel>();

                FluxoStatusExcel obj;

                while (rd.Read())
                {
                    obj = new FluxoStatusExcel
                    {
                        FORMULARIO = rd.GetString(0),
                        ORIGEM = rd.GetString(1),
                        DESTINO = rd.IsDBNull(2) ? string.Empty : rd.GetString(2)
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

        public static IList<FluxoStatus> GetPorFormulario(Int32 ID_FORMULARIO)
        {
            IList<FluxoStatus> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.Parameters.Add(new SqlParameter("ID_FORMULARIO", ID_FORMULARIO));

                comm.CommandText = @"
                SELECT 
                ID_STATUS_ORIGEM = tsfOrigem.ID,
                NOME_STATUS_ORIGEM = tsfOrigem.NOME,
                tsfOrigem.INICIAL,
                ID_STATUS_DESTINO = tsfDestino.ID,
                NOME_STATUS_DESTINO = tsfDestino.NOME,
                TOT_DESTINOS_ORIGEM = (SELECT COUNT(ID_STATUS_DESTINO) FROM TB_FLUXO_STATUS WHERE ID_STATUS_ORIGEM = tsfOrigem.ID),
                TOT_DESTINOS_DESTINO = (SELECT COUNT(ID_STATUS_DESTINO) FROM TB_FLUXO_STATUS WHERE ID_STATUS_ORIGEM = tsfDestino.ID)

                FROM TB_FLUXO_STATUS tfs
                JOIN TB_STATUS_FORMULARIO tsfOrigem ON tsfOrigem.ID = tfs.ID_STATUS_ORIGEM
                JOIN TB_STATUS_FORMULARIO tsfDestino ON tsfDestino.ID = tfs.ID_STATUS_DESTINO
                
                WHERE tsfOrigem.ID_FORMULARIO = @ID_FORMULARIO
                AND tsfDestino.ID_FORMULARIO = @ID_FORMULARIO
                	
                ORDER BY INICIAL DESC, ID_STATUS_ORIGEM, ID_STATUS_DESTINO
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<FluxoStatus>();
                FluxoStatus obj;
                while (rd.Read())
                {
                    obj = new FluxoStatus
                    {
                        statusOrigem = new StatusFormulario
                        {
                            ID = rd.GetInt32(0),
                            NOME = rd.GetString(1),
                            INICIAL = rd.GetBoolean(2) ? 1 : 0
                        },
                        statusDestino = new StatusFormulario
                        {
                            ID = rd.GetInt32(3),
                            NOME = rd.GetString(4)
                        },
                        TOT_DESTINOS_ORIGEM = rd.GetInt32(5),
                        TOT_DESTINOS_DESTINO = rd.GetInt32(6)
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

        public static FluxoStatus GetPorOrigem(Int32 ID_STATUS_ORIGEM)
        {
            FluxoStatus result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID_STATUS_ORIGEM

	                FROM TB_FLUXO_STATUS

                    WHERE ID_STATUS_ORIGEM = @ID_STATUS_ORIGEM";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", ID_STATUS_ORIGEM));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new FluxoStatus
                    {
                        statusOrigem = new StatusFormulario
                        {
                            ID = rd.GetInt32(0)
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

        public static int? Insert(FluxoStatus obj, ref StringBuilder msgErro)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                INSERT INTO TB_FLUXO_STATUS 
                    (ID_STATUS_ORIGEM,                    
                    ID_STATUS_DESTINO) 
                    VALUES 
                    (@ID_STATUS_ORIGEM,
                     @ID_STATUS_DESTINO)";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", obj.statusOrigem.ID));
                comm.Parameters.Add(new SqlParameter("ID_STATUS_DESTINO", obj.statusDestino.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (SqlException sex)
            {
                if (2627 == sex.Number)
                {
                    msgErro.Append("Já existe um fluxo com os status de origem e destino informados");
                }
                nrLinhas = null;
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

        public static int? Delete(FluxoStatus obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_FLUXO_STATUS
                WHERE ID_STATUS_ORIGEM = @ID_STATUS_ORIGEM
                AND ID_STATUS_DESTINO = @ID_STATUS_DESTINO";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_STATUS_ORIGEM", obj.statusOrigem.ID));
                comm.Parameters.Add(new SqlParameter("ID_STATUS_DESTINO", obj.statusDestino.ID));

                nrLinhas = comm.ExecuteNonQuery();
            }
            catch (Exception)
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