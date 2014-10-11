using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PUB.STCT.Client
{
    public partial class FilterItem : XtraUserControl 
    {
        private List<string>[] _columnValue;
        private bool _isChecked = false;
        private BaseEdit valueEdit;
        private ColumnInfo[] myColumns;
        private FilterType currentType;
        private ColumnInfo currentColumn;
        public FilterItem(ColumnInfo[] myColumns,List<string>[] columnvalue)
        {
            InitializeComponent();
            this.DateTimeEdit.Tag = FilterType.DateTime;
            this.TextEdit.Tag = FilterType.Text;
            this.DistinctEdit.Tag = FilterType.Distinct;
            this.FKEdit.Tag = FilterType.FK;
            this.myColumns = myColumns;
            this.comboBoxEdit1.Properties.Items.Clear();
            this.comboBoxEdit1.Properties.Items.AddRange(myColumns);
            this.columnValue = columnvalue;
            this.isChecked = false;
        }
        public bool isChecked
        {
            get
            {
                return _isChecked;
            }
            private set
            {
                _isChecked = value;
                if(value)
                    this.pictureEdit1.Image = this.imageCollection1.Images[0];
                else
                    this.pictureEdit1.Image = this.imageCollection1.Images[1];
                if (CheckedChanged!=null)
                    CheckedChanged(this);
            }
        }
        public List<string>[] columnValue
        {
            get
            {
                return _columnValue;
            }
            set
            {
                _columnValue = value;
            }
        }
        public delegate void NewItemEvent();
        public delegate void SelectItemEvent(object sender);
        public delegate void CheckItemEvent(object sender);
        public delegate void NeedValuesEvent(object sender,NeedValueEventArgs e);
        public event NewItemEvent WantNewItem;
        public event SelectItemEvent ItemSelected;
        public event CheckItemEvent CheckedChanged;
        public event NeedValuesEvent NeedValues;
        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit2.Text == "is null" || comboBoxEdit2.Text == "is not null")
            {
                this.DistinctEdit.Text = "";
                this.DistinctEdit.Enabled = false;
            }
            else
            {
                this.DistinctEdit.Enabled = true;
            }
            check();
        }
        public void SetDefalutWhenAdd()
        {
            if (this.comboBoxEdit4.Text=="")
            {
                comboBoxEdit4.SelectedIndexChanged -= comboBoxEdit4_SelectedIndexChanged;
                this.comboBoxEdit4.Text = "AND";
            }
        }
        public void SetDefalutWhenDelete()
        {
            comboBoxEdit4.SelectedIndexChanged -= comboBoxEdit4_SelectedIndexChanged;
            comboBoxEdit4.SelectedIndexChanged += comboBoxEdit4_SelectedIndexChanged;
            this.comboBoxEdit4.Text = "";
        }
        private void comboBoxEdit4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WantNewItem != null && comboBoxEdit4.Text != "")
            {
                comboBoxEdit4.SelectedIndexChanged -= comboBoxEdit4_SelectedIndexChanged;
                WantNewItem();
            }
        }
        private void check()
        {
            if (comboBoxEdit1.Text != "" && (valueEdit.Text != "" || comboBoxEdit2.Text == "is null" || comboBoxEdit2.Text == "is not null"))
                this.isChecked = true;
            else
                this.isChecked = false;
        }
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColumn = myColumns[comboBoxEdit1.SelectedIndex];
            var type = currentColumn.filter_Type;
            if (valueEdit != null)
            {
                valueEdit.Text = null;
                valueEdit.EditValueChanged -= valueEditChanged;
            }
            valueEdit = setType(type);
            valueEdit.EditValueChanged += valueEditChanged;
            switch(type)
            {
                case FilterType.Distinct:
                    {
                        (valueEdit as ComboBoxEdit).Properties.Items.Clear();
                        if (columnValue[comboBoxEdit1.SelectedIndex] == null && NeedValues != null)
                            NeedValues(sender, new NeedValueEventArgs(comboBoxEdit1.SelectedIndex));
                        (valueEdit as ComboBoxEdit).Properties.Items.AddRange(columnValue[comboBoxEdit1.SelectedIndex]);
                        break;
                    }
                case FilterType.FK:
                    {
                        (valueEdit as DataComboBoxEdit).Properties.Items.Clear();
                        (valueEdit as DataComboBoxEdit).AddRange(currentColumn.value_Original, currentColumn.value_Show);
                        break;
                    }
                case FilterType.Text:
                case FilterType.DateTime:
                    {
                        switch (currentColumn.format_Type)
                        {
                            case FormatType.Custom:
                                {
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                                    break;
                                }
                            case FormatType.DateTime:
                                {
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                                    break;
                                }
                            case FormatType.None:
                                {
                                    break;
                                }
                            case FormatType.Numeric:
                                {
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatString = currentColumn.format_String;
                                    (valueEdit as BaseEdit).Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                                    break;
                                }
                            default: break;
                        }
                        break;
                    }
                default:break;
            }
            check();
        }

        private void valueEditChanged(object sender, EventArgs e)
        {
            check();
        }

        private void FilterItem_Enter(object sender, EventArgs e)
        {
            if(ItemSelected!=null)
                ItemSelected(sender);
        }
        public string getSql()
        {
            return " " + myColumns[comboBoxEdit1.SelectedIndex].column + this.comboBoxEdit2.Text + "'" + valueEdit.Text + "' " + this.comboBoxEdit4.Text; ;
        }
        private BaseEdit setType(FilterType target)
        {
            this.currentType = target;
            BaseEdit data = null;
            foreach(Control mycl in ValueEditGroup.Controls)
            {
                if (Enum.Equals(mycl.Tag, target))
                {
                    mycl.Visible = true;
                    data = (mycl as BaseEdit);
                }
                else
                    mycl.Visible = false;
            }
            return data;
        }
    }
    public class NeedValueEventArgs:EventArgs
    {
        public NeedValueEventArgs(int i)
        {
            this.Index = i;
        }
        public int Index { get; private set; }
    }
}
