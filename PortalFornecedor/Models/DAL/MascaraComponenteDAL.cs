using CencosudCSCWEBMVC.Models.TO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CencosudCSCWEBMVC.Models.DAL
{
    public class MascaraComponenteDAL
    {
        public static IList<MascaraComponente> GetParaComponente()
        {
            IList<MascaraComponente> objs = new List<MascaraComponente>();

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

	                FROM TB_MASCARA_COMPONENTE

                    ORDER BY NOME";

                comm.CommandText = queryGet;

                con.Open();

                SqlDataReader rd = comm.ExecuteReader();

                MascaraComponente obj;
                while (rd.Read())
                {
                    obj = new MascaraComponente
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