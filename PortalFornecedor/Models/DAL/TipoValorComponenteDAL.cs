using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class TipoValorComponenteDAL
    {
        public static IList<TipoValorComponente> GetParaComponente()
        {
            IList<TipoValorComponente> objs = new List<TipoValorComponente>();

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    CODIGO, 
				    NOME

	                FROM TB_TIPO_VALOR_COMPONENTE

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                TipoValorComponente obj;
                while (rd.Read())
                {
                    obj = new TipoValorComponente
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2)
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

        public static TipoValorComponente GetDefault()
        {
            TipoValorComponente result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ID, 
				    CODIGO, 
				    NOME

	                FROM TB_TIPO_VALOR_COMPONENTE

                    WHERE CODIGO = @CODIGO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("CODIGO", Util.CODIGO_TIPO_VALOR_COMPONENTE_DEFAULT));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = new TipoValorComponente
                    {
                        ID = rd.GetInt32(0),
                        CODIGO = rd.GetInt32(1),
                        NOME = rd.GetString(2)
                    };
                }
                rd.Close();
            }
            finally
            {
                con.Close();
            }

            return result;
        }
    }
}