using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GrupoLTM.WebSmart.Infrastructure.Helper
{
    public static class SqlHelper
    {
        static string InserePrefixo(string prefixo, string valor)
        {
            return prefixo + valor;
        }

        public static DataTable ExecutarProcedure(string strProcedure, Dictionary<string, object> parameters, string strRetorno, string strConn)
        {
            strRetorno = "";
            DataTable dta = new DataTable();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 99999999;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }
            }

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dta);
                sda.Dispose();
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }
            return dta;
        }

        public static DataTable ExecutarProcedure(string strProcedure, ArrayList arrayParameters, string strRetorno, string strConn)
        {
            strRetorno = "";
            DataTable dta = new DataTable();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 99999999;

            string[] strParamaters = new string[2];

            for (int i = 0; i < arrayParameters.Count; i++)
            {
                strParamaters = (string[])arrayParameters[i];
                if (strParamaters[1].ToString() != "")
                    cmd.Parameters.AddWithValue(strParamaters[0].ToString(), strParamaters[1].ToString());
            }

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dta);
                sda.Dispose();
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }
            return dta;
        }

        public static DataTable ExecutarProcedure(string strProcedure, string strRetorno, string strConn)
        {
            strRetorno = "";
            DataTable dta = new DataTable();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 99999999;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dta);
                sda.Dispose();
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }
            return dta;
        }

        public static DataSet ExecutarProcedureDataSet(string strProcedure, Dictionary<string, object> parameters, string strRetorno, string strConn)
        {
            strRetorno = "";
            DataSet ds = new DataSet();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 999999;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);

                sda.Dispose();
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }

            return ds;
        }

        public static DataSet ExecutarProcedureDataSet(string strProcedure, Dictionary<string, object> parameters, string strRetorno, string strConn, out object outputSaida, object outputEntrada = null)
        {
            outputSaida = null;
            strRetorno = "";
            DataSet ds = new DataSet();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 99999999;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }

            if (outputEntrada != null)
                cmd.Parameters["@" + outputEntrada].Direction = ParameterDirection.Output;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);

                if (outputEntrada != null)
                    outputSaida = Convert.ToInt32(cmd.Parameters["@" + outputEntrada].Value);

                sda.Dispose();
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }

            return ds;
        }

        public static void ExecuteNonQuery(string strProcedure, Dictionary<string, object> parameters, string strConn)
        {
            SqlConnection cnnBase = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand(strProcedure, cnnBase);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }
            }

            try
            {
                cnnBase.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                cnnBase.Close();
                cmd.Dispose();
            }
        }

        static string message = "";
        private static ArrayList InfoMessages = new ArrayList();

        public static DataSet ExecutarQueryDataSet(string strProcedure, out string strRetorno, string strConn)
        {
            strRetorno = "";
            message = "";
            InfoMessages.Clear();

            DataSet ds = new DataSet();

            SqlConnection con = new SqlConnection(strConn);

            con.InfoMessage += new SqlInfoMessageEventHandler(MessageEventHandler);

            SqlCommand cmd = new SqlCommand(strProcedure, con);
            cmd.CommandType = CommandType.Text;
            //cmd.CommandTimeout = 99999999;

            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);
                sda.Dispose();

                //con.Open();
                //cmd.ExecuteNonQuery();
                //con.Close();

                foreach (string _message in InfoMessages)
                {
                    message += "</br>" + _message;
                }

                strRetorno = message;
            }
            catch (Exception exc)
            {
                strRetorno = exc.ToString();
            }
            finally
            {
                cmd.Dispose();
                con.Close();
            }

            return ds;
        }

        public static string DatatableToJson(DataTable dt)
        {
            string texto = JsonConvert.SerializeObject(dt);

            return texto;
        }

        public static void MessageEventHandler(object sender, SqlInfoMessageEventArgs e)
        {
            InfoMessages.Add(e.Message);
        }
    }
}
