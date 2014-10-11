using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Transactions;

namespace PUB.STCT.Server
{
    public static class STCTdal
    {
        public static OleDbDataAdapter getAdp(string cmd,OleDbConnection conn)
        {
            OleDbDataAdapter adp = new OleDbDataAdapter(cmd, conn);
            adp.MissingSchemaAction = MissingSchemaAction.Add;
            return adp;
        }
        public static OleDbConnection getConn(string conn_str = null)
        {
            return(new OleDbConnection(conn_str == null ? "Provider=MSDAORA;Data Source = orcl;User ID=Tcat;Password=mao19911024;" : conn_str));
        }
        public static DataTable Select(string cmd,OleDbConnection conn)
        {
            DataTable data = new DataTable();
            getAdp(cmd,conn).Fill(data);
            return data;
        }
        //public static List<string[]> Select_Reader(string cmd,int columncount,OleDbConnection conn)
        //{
        //    List<string[]> data = new List<string[]>();
        //    conn.Open();
        //    OleDbCommand command = new OleDbCommand(cmd,conn);
        //    OleDbDataReader reader = command.ExecuteReader();
        //    while(reader.Read())
        //    {
        //        string[] temp = new string[columncount];
        //        for (int i = 0; i < columncount;i++ )
        //        {
        //            temp[i] = reader[i].ToString();
        //        }
        //        data.Add(temp);
        //    }
        //    conn.Close();
        //    return data;
        //}
        public static List<string>[] Select_Reader(string cmd,int columncount, OleDbConnection conn)
        {
            List<string>[] data = new List<string>[columncount];
            for (int i = 0; i < columncount; i++)
            {
                data[i] = new List<string>();
            }
            conn.Open();
            OleDbCommand command = new OleDbCommand(cmd, conn);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < columncount; i++)
                {
                    data[i].Add(reader[i].ToString());
                }
            }
            conn.Close();
            return data;
        }
        public static string Save(DataTable changes,string cmd, OleDbConnection conn)
        {
            if (changes != null)
            {
                try
                {
                    OleDbDataAdapter adp = getAdp(cmd,conn);
                    OleDbCommandBuilder cmb = new OleDbCommandBuilder(adp);
                    adp.Update(changes);
                    return "";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
            return "";
        }
        public static string LogSave(DataTable changes, string cmd,string logcmd, OleDbConnection conn, OleDbConnection logconn)
        {
            using (TransactionScope tra = new TransactionScope())
            {
                if (changes != null)
                {
                    try
                    {
                        logconn.Open();
                        OleDbCommand logCmd = new OleDbCommand(logcmd, logconn);
                        logCmd.ExecuteNonQuery();
                        logconn.Close();
                        OleDbDataAdapter adp = getAdp(cmd, conn);
                        OleDbCommandBuilder cmb = new OleDbCommandBuilder(adp);
                        adp.Update(changes);
                        tra.Complete();
                        return "";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    finally
                    {
                        if(logconn.State== ConnectionState.Open)
                            logconn.Close();
                    }
                }
            }
            return "";
        }
    }
}
