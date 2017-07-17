using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class ArquivoTramitacaoDAL
    {
        public static byte[] ObterBytes(int idArquivo)
        {
            byte[] result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				    SELECT 
				    ARQUIVO

	                FROM TB_ARQUIVO_TRAMITACAO

                    WHERE ID = @idArquivo";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("idArquivo", idArquivo));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = rd.GetValue(0) as byte[];
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

        public static Int32? ObterTotArquivoPreenchimento(Int32? idPreenchimentoFormulario)
        {
            Int32? result = null;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = Util.CONNECTION_STRING;

            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = con;

                string queryGet = @"
				SELECT COUNT(tat.ID)
	                FROM TB_TRAMITACAO tt
	                JOIN TB_ARQUIVO_TRAMITACAO tat ON tat.ID_TRAMITACAO = tt.ID
	                	WHERE tt.ID_PREENCHIMENTO_FORMULARIO = @ID_PREENCHIMENTO_FORMULARIO";

                comm.CommandText = queryGet;

                con.Open();

                comm.Parameters.Add(new SqlParameter("ID_PREENCHIMENTO_FORMULARIO", idPreenchimentoFormulario));

                SqlDataReader rd = comm.ExecuteReader();

                if (rd.Read())
                {
                    result = rd.GetValue(0) as Int32?;
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
    }
}