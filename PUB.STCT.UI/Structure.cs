using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EAS.Services;
using EAS.Modularization;
using DevExpress.XtraEditors;
using PUB.STCT.Interface;

namespace PUB.STCT.Client
{
    public class ColumnInfo
    {
        public override string ToString()
        {
            return this.column_Name;
        }
        public ColumnInfo(string column, string column_Name,string type, bool edit_Allow, MaskType edit_Mask, EditType edit_Type, FormatType format_Type, string format_String, string unit, bool filter_Allow,
            FilterType filter_Type,bool fk_Allow,string fk_Table,string fk_ID,string fk_Data)
        {
            this.column = column;
            this.column_Name = column_Name;
            this.type = type;
            this.edit_Allow = edit_Allow;
            this.edit_Mask = edit_Mask;
            this.edit_Type = edit_Type;
            this.format_Type = format_Type;
            this.format_String = format_String;
            this.unit = unit;
            this.filter_Allow = filter_Allow;
            this.filter_Type = filter_Type;
            this.fk_Allow = fk_Allow;
            this.fk_Table = fk_Table;
            this.fk_ID = fk_ID;
            this.fk_Data = fk_Data;
        }
        public void getFK(string con)
        {
            List<string>[] temp = ServiceContainer.GetService<STCTIService>().ST_Select_Reader(this.fk_Table, true, new string[] { fk_ID, fk_Data }, con);
            value_Original = temp[0];
            value_Show =temp[1];
        }
        public string column { get; private set; }
        public string column_Name { get; set; }
        public string type { get; private set; }
        public bool edit_Allow { get; set; }
        public MaskType edit_Mask { get; set; }
        public EditType edit_Type { get; set; }
        public FormatType format_Type { get; set; }
        public string format_String { get; set; }
        public string unit { get; set; }
        public bool filter_Allow { get; set; }
        public FilterType filter_Type { get; set; }
        public bool fk_Allow { get; set; }
        public string fk_Table { get; set; }
        public string fk_ID{ get; set; }
        public string fk_Data { get; set; }
        public List<string> value_Original
        {
            get;
            set;
        }
        public List<string> value_Show
        {
            get;
            set;
        }
    }
    public enum FilterType
    {
        Text,
        DateTime,
        Distinct,
        FK
    }
    public enum MaskType
    {
        DateTime,
        String,
        Number,

    }
    public enum EditType
    {
        Text,
        DateTime,
        FK
    }
    public enum FormatType
    {
        None,
        Numeric,
        DateTime,
        Custom
    }
    public interface DataBaseEditPro
    {
        void SetSource(List<string> original, List<string> show);
        void SetNewBind(string property,BindingSource myBind,string target,bool check,DataSourceUpdateMode myMod);
        void SetDefalut();
    }
}
