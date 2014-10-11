using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace PUB.STCT.Server
{
    public static class STCTbll
    {
        public static DataTable Select_BLL(string tablename, string[] columns = null, string conn_str = null,string sql = null)
        {
            return STCTdal.Select(getSelectCommand(tablename, false,columns,sql), STCTdal.getConn(conn_str));
        }
        //public static List<string[]> Select_Reader_BLL(string tablename, bool distinct, string[] columns, string conn_str = null,string sql=null)
        //{
        //    return STCTdal.Select_Reader(getSelectCommand(tablename,distinct,columns,sql),columns.Length, STCTdal.getConn(conn_str));
        //}
        public static List<string>[] Select_Reader_BLL(string tablename, bool distinct, string[] columns, string conn_str = null, string sql = null)
        {
            return STCTdal.Select_Reader(getSelectCommand(tablename, distinct, columns, sql), columns.Length, STCTdal.getConn(conn_str));
        }
        public static string Save_BLL(DataTable changes, string tablename, string[] columns = null, string conn_str = null,string sql = null)
        {
            return STCTdal.Save(changes, getSelectCommand(tablename, false,columns,sql), STCTdal.getConn(conn_str));
        }
        public static string LogSave_BLL(DataTable changes, string logsql, string tablename, string[] columns = null, string conn_str = null, string logconn_str = null, string sql = null)
        {
            return STCTdal.LogSave(changes, getSelectCommand(tablename, false, columns, sql), logsql, STCTdal.getConn(conn_str), STCTdal.getConn(logconn_str));
        }
        private static string getSelectCommand(string tablename, bool distinct, string[] columns = null, string sql = null)
        {
            string cmd = "select ";
            if (distinct)
                cmd += "distinct ";
            if(columns==null)
            {
                cmd += "* from " + tablename;
                return cmd;
            }
            cmd += columns[0];
            for (int i = 1; i < columns.Length; i++)
            {
                cmd += ", " + columns[i] + " ";
            }
            cmd += " from " + tablename;
            if (sql != "" && sql != null)
            {
                cmd += " where " + sql;
            }
            return cmd;
        }
    }
}
