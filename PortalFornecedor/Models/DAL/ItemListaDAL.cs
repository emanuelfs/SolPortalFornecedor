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
    public class ItemListaDAL
    {
        public static IList<ItemLista> Get(int start, int pageSize, ref int totRegistros, string textoFiltro, ref int totRegistrosFiltro, string sortColumn, string sortColumnDir)
        {
            IList<ItemLista> objs = new List<ItemLista>();

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
                    ORDER BY itemLista.NOME
                    ";
                }
                else
                {
                    string auxOrdenacao = string.Format("{0} {1}", sortColumn, sortColumnDir);
                    switch (auxOrdenacao)
                    {
                        case "NOME asc":
                            ordenacao = "ORDER BY itemLista.NOME";
                            break;
                        case "NOME_TIPO_LISTA asc":
                            ordenacao = "ORDER BY tipoLista.NOME";
                            break;
                        case "NOME_PAI asc":
                            ordenacao = "ORDER BY itemPai.NOME";
                            break;
                        case "NOME_TIPO_LISTA_PAI asc":
                            ordenacao = "ORDER BY listaPai.NOME";
                            break;
                        case "NOME desc":
                            ordenacao = "ORDER BY itemLista.NOME DESC";
                            break;
                        case "NOME_TIPO_LISTA desc":
                            ordenacao = "ORDER BY tipoLista.NOME DESC";
                            break;
                        case "NOME_PAI desc":
                            ordenacao = "ORDER BY itemPai.NOME DESC";
                            break;
                        case "NOME_TIPO_LISTA_PAI desc":
                            ordenacao = "ORDER BY listaPai.NOME DESC";
                            break;
                        default:
                            ordenacao = @"
                            ORDER BY itemLista.NOME
                            ";
                            break;
                    }
                }

                StringBuilder queryGet = new StringBuilder(@"
                SELECT TOP (@pageSize) *
	                FROM (
						SELECT 
						itemLista.ID,
                        itemLista.NOME,
                        itemPai.ID AS ID_PAI,
                        ISNULL(CONVERT(VARCHAR(50), itemPai.NOME), '') AS NOME_PAI,
                        tipoLista.ID AS ID_TIPO_LISTA,
                        tipoLista.NOME AS NOME_TIPO_LISTA,
                        listaPai.ID AS ID_TIPO_LISTA_PAI,
                        ISNULL(CONVERT(VARCHAR(50), listaPai.NOME), '') AS NOME_TIPO_LISTA_PAI,

						(ROW_NUMBER() OVER (").Append(ordenacao).Append(@"))  
						AS 'numeroLinha', 

						(SELECT COUNT(ID) FROM TB_ITEM_LISTA) 
						AS 'totRegistros', 

						(SELECT COUNT(itemLista.ID) 
                            FROM TB_ITEM_LISTA itemLista 
                            JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = itemLista.ID_TIPO_LISTA
                            LEFT JOIN TB_ITEM_LISTA itemPai ON itemPai.ID = itemLista.ID_PAI
                            LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
						        WHERE 
                                itemLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                tipoLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                ISNULL(CONVERT(VARCHAR(50), itemPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                ISNULL(CONVERT(VARCHAR(50), listaPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro
                        ) 
						AS 'totRegistrosFiltro'

	                	FROM TB_ITEM_LISTA itemLista 
                            JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = itemLista.ID_TIPO_LISTA
                            LEFT JOIN TB_ITEM_LISTA itemPai ON itemPai.ID = itemLista.ID_PAI
                            LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
						        WHERE 
                                itemLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                tipoLista.NOME collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                ISNULL(CONVERT(VARCHAR(50), itemPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro
                                OR
                                ISNULL(CONVERT(VARCHAR(50), listaPai.NOME), '') collate Latin1_General_CI_AI like @textoFiltro)
				as todasLinhas
                WHERE todasLinhas.numeroLinha > (@start)");

                comm.Parameters.Add(new SqlParameter("pageSize", pageSize));
                comm.Parameters.Add(new SqlParameter("start", start));
                comm.Parameters.Add(new SqlParameter("textoFiltro", string.Format("%{0}%", textoFiltro)));

                comm.CommandText = queryGet.ToString();

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ItemLista obj;

                if (rd.Read())
                {
                    totRegistros = rd.GetInt32(9);
                    totRegistrosFiltro = rd.GetInt32(10);

                    obj = new ItemLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        itemPai = !rd.IsDBNull(2) ?
                        new ItemLista
                        {
                            ID = rd.GetInt32(2),
                            NOME = rd.GetString(3)
                        } :
                        new ItemLista
                        {
                            NOME = rd.GetString(3)
                        },
                        tipoLista = new TipoLista
                        {
                            ID = rd.GetInt32(4),
                            NOME = rd.GetString(5),
                            listaPai = !rd.IsDBNull(6) ?
                            new TipoLista
                            {
                                ID = rd.GetInt32(6),
                                NOME = rd.GetString(7)
                            } :
                            new TipoLista
                            {
                                NOME = rd.GetString(7)
                            }
                        }
                    };
                    objs.Add(obj);
                }
                while (rd.Read())
                {
                    obj = new ItemLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        itemPai = !rd.IsDBNull(2) ?
                        new ItemLista
                        {
                            ID = rd.GetInt32(2),
                            NOME = rd.GetString(3)
                        } :
                        new ItemLista
                        {
                            NOME = rd.GetString(3)
                        },
                        tipoLista = new TipoLista
                        {
                            ID = rd.GetInt32(4),
                            NOME = rd.GetString(5),
                            listaPai = !rd.IsDBNull(6) ?
                            new TipoLista
                            {
                                ID = rd.GetInt32(6),
                                NOME = rd.GetString(7)
                            } :
                            new TipoLista
                            {
                                NOME = rd.GetString(7)
                            }
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

        public static IList<ItemListaExcel> GetParaExcel()
        {
            IList<ItemListaExcel> objs = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                comm.CommandText = @"
                SELECT 
                itemLista.NOME,
                LISTA = tipoLista.NOME,
                ITEM_VINCULADO = ISNULL(CONVERT(VARCHAR(50), itemPai.NOME), ''),
                LISTA_VINCULADA = ISNULL(CONVERT(VARCHAR(50), listaPai.NOME), '')
        
                FROM TB_ITEM_LISTA itemLista 
                JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = itemLista.ID_TIPO_LISTA
                LEFT JOIN TB_ITEM_LISTA itemPai ON itemPai.ID = itemLista.ID_PAI
                LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI
    
                ORDER BY tipoLista.NOME, itemLista.NOME
                ";

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                objs = new List<ItemListaExcel>();

                ItemListaExcel obj;

                while (rd.Read())
                {
                    obj = new ItemListaExcel
                    {
                        NOME = rd.GetString(0),
                        LISTA = rd.GetString(1),
                        ITEM_VINCULADO = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                        LISTA_VINCULADA = rd.IsDBNull(3) ? string.Empty : rd.GetString(3)
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

        public static IList<ItemLista> GetPorTipoLista(Int32 ID_TIPO_LISTA)
        {
            IList<ItemLista> objs = new List<ItemLista>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    itemLista.ID, 
				    itemLista.NOME,
                    itemPai.ID AS ID_PAI,
                    listaPai.ID AS ID_TIPO_LISTA_PAI

	                FROM TB_ITEM_LISTA itemLista
                    JOIN TB_TIPO_LISTA tipoLista ON tipoLista.ID = itemLista.ID_TIPO_LISTA
                    LEFT JOIN TB_ITEM_LISTA itemPai ON itemPai.ID = itemLista.ID_PAI
                    LEFT JOIN TB_TIPO_LISTA listaPai ON listaPai.ID = tipoLista.ID_PAI

                    WHERE itemLista.ID_TIPO_LISTA = @ID_TIPO_LISTA

                    ORDER BY itemLista.NOME";

                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", ID_TIPO_LISTA));

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ItemLista obj;
                while (rd.Read())
                {
                    obj = new ItemLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1),
                        itemPai = !rd.IsDBNull(2) && !rd.IsDBNull(3) ?
                        new ItemLista
                        {
                            ID = rd.GetInt32(2)
                        } : null,
                        tipoLista = !rd.IsDBNull(3) ?
                        new TipoLista
                        {
                            listaPai = new TipoLista
                            {
                                ID = rd.GetInt32(3)
                            }
                        } : null
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

        public static IList<ItemLista> GetPorTipoListaPai(Int32 ID_TIPO_LISTA, Int32? ID_ITEM_LISTA)
        {
            IList<ItemLista> objs = new List<ItemLista>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT
                itemLista.ID,
                NOME_ITEM_LISTA = itemLista.NOME + ' (' + tipoListaPai.NOME + ')'
                
                FROM TB_TIPO_LISTA tipoLista
				JOIN TB_TIPO_LISTA tipoListaPai ON tipoListaPai.ID = tipoLista.ID_PAI
                JOIN TB_ITEM_LISTA itemLista ON itemLista.ID_TIPO_LISTA = tipoListaPai.ID
                
                WHERE tipoLista.ID = @ID_TIPO_LISTA
                AND (@ID_ITEM_LISTA IS NULL OR itemLista.ID != @ID_ITEM_LISTA)

                ORDER BY NOME_ITEM_LISTA";

                object idItemLista = DBNull.Value;
                if (null != ID_ITEM_LISTA)
                {
                    idItemLista = ID_ITEM_LISTA;
                }

                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", ID_TIPO_LISTA));
                comm.Parameters.Add(new SqlParameter("ID_ITEM_LISTA", idItemLista));

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ItemLista obj;
                while (rd.Read())
                {
                    obj = new ItemLista
                    {
                        ID = rd.GetInt32(0),
                        NOME = rd.GetString(1)
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

        public static int? Insert(ItemLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                INSERT INTO TB_ITEM_LISTA 
                    (NOME, ID_TIPO_LISTA, ID_PAI) VALUES 
                    (@NOME, @ID_TIPO_LISTA, @ID_PAI)";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (obj.itemPai != null)
                {
                    ID_PAI = obj.itemPai.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", obj.tipoLista.ID));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));

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

        public static int? Update(ItemLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                UPDATE TB_ITEM_LISTA SET   
                    NOME = @NOME,
                    ID_TIPO_LISTA = @ID_TIPO_LISTA,
                    ID_PAI = @ID_PAI
                WHERE ID = @ID";

                con.Open();

                object ID_PAI = DBNull.Value;
                if (obj.itemPai != null)
                {
                    ID_PAI = obj.itemPai.ID;
                }

                comm.Parameters.Add(new SqlParameter("NOME", obj.NOME));
                comm.Parameters.Add(new SqlParameter("ID_TIPO_LISTA", obj.tipoLista.ID));
                comm.Parameters.Add(new SqlParameter("ID_PAI", ID_PAI));
                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

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

        public static int? Delete(ItemLista obj)
        {
            int? nrLinhas;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;
                comm.CommandText = @"
                DELETE TB_ITEM_LISTA
                WHERE ID = @ID";

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID", obj.ID));

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