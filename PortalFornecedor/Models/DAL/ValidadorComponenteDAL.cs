using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class ValidadorComponenteDAL
    {
        public static IList<ValidadorComponente> GetParaComponente()
        {
            IList<ValidadorComponente> objs = new List<ValidadorComponente>();

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

	                FROM TB_VALIDADOR_COMPONENTE

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                ValidadorComponente obj;
                while (rd.Read())
                {
                    obj = new ValidadorComponente
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
    }
}