using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using EAS.Services;
using PUB.STCT.Interface;

namespace PUB.STCT.Server
{
    [ServiceObject("涟钢服务")]
    [ServiceBind(typeof(STCTIService))]
    public class STCTService:ServiceObject,STCTIService
    {
        DataSet myData = new DataSet();
        public DataTable ST_Select(string tablename, string[] columns = null, string conn_str = null,string sql = null)
        {
            return STCTbll.Select_BLL(tablename, columns, conn_str,sql);
        }
        //public List<string[]> ST_Select_Reader(string tablename,bool distinct, string[] columns = null, string conn_str = null,string sql = null)
        //{
        //    return STCTbll.Select_Reader_BLL(tablename, distinct,columns, conn_str,sql);
        //}
        public List<string>[] ST_Select_Reader(string tablename, bool distinct, string[] columns = null, string conn_str = null, string sql = null)
        {
            return STCTbll.Select_Reader_BLL(tablename, distinct, columns, conn_str, sql);
        }
        public string ST_Save(DataTable changes, string tablename, string[] columns = null, string conn_str = null, string sql = null)
        {
            return STCTbll.Save_BLL(changes, tablename, columns, conn_str,sql);
        }
        public string ST_LogSave(DataTable changes,string logsql,string tablename, string[] columns = null, string conn_str = null,string logconn_str=null, string sql = null )
        {
            return STCTbll.LogSave_BLL(changes, logsql,tablename, columns, conn_str,logconn_str, sql);
        }
    }
}
