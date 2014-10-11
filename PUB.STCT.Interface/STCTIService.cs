using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace PUB.STCT.Interface
{
    public interface STCTIService
    {
        DataTable ST_Select(string tablename, string[] columns = null, string conn_str = null,string sql = null);
        string ST_Save(DataTable changes, string tablename, string[] columns = null, string conn_str = null, string sql = null);
        List<string>[] ST_Select_Reader(string tablename, bool distinct, string[] columns = null, string conn_str = null, string sql = null);
        string ST_LogSave(DataTable changes, string logsql, string tablename, string[] columns = null, string conn_str = null, string logconn_str = null, string sql = null);
    }
}
