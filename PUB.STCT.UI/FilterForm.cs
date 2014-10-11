using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using EAS.Services;
using EAS.Modularization;
using PUB.STCT.Interface;

namespace PUB.STCT.Client
{
    public partial class FilterForm : Form
    {

        FilterItem lastItem;
        FilterItem selectedItem;
        List<string>[] FilterColumnsValue;
        ColumnInfo[] myColumns;
        string conn;
        string TableName;
       /// <summary>
       /// ////////////////////////////////////////
       /// </summary>
        public string SQLresult = "";
        /// <summary>
        /// //////////////////////////////////////////
        /// </summary>
        private Mod thisMod;
        private enum Mod
        {
            Advance,
            Normal
        }
        public FilterForm(ColumnInfo[] myColumns,string tablename, string con)
        {
            InitializeComponent();
            this.myColumns = myColumns;
            ChangeMod(Mod.Normal);
            TableName = tablename;
            conn = con;
            FilterColumnsValue = new List<string>[myColumns.Length];
            this.listBoxControl1.Items.AddRange(myColumns);
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            this.AddNewItem();
        }
        private void AddNewItem()
        {
            FilterItem current = new FilterItem(myColumns,FilterColumnsValue);
            FilterItem_Checked(current);
            current.WantNewItem += AddNewItem;
            current.ItemSelected += FilterItem_Selected;
            current.CheckedChanged += FilterItem_Checked;
            current.NeedValues+=FilterItem_NeedValues;
            this.flowLayoutPanel1.Controls.Add(current);
            current.Focus();
            this.xtraScrollableControl1.VerticalScroll.Value = this.xtraScrollableControl1.VerticalScroll.Maximum;
            if(lastItem!=null)
                lastItem.SetDefalutWhenAdd();
            lastItem = current;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                int index = flowLayoutPanel1.Controls.GetChildIndex(selectedItem);
                flowLayoutPanel1.Controls.Remove(selectedItem);
                int count = flowLayoutPanel1.Controls.Count ;
                if (count > 0 && index < count)
                {
                    (flowLayoutPanel1.Controls[count - 1] as FilterItem).SetDefalutWhenDelete();
                    flowLayoutPanel1.Controls[index].Focus();
                }
                else if (flowLayoutPanel1.Controls.Count > 0)
                {
                    (flowLayoutPanel1.Controls[count - 1] as FilterItem).SetDefalutWhenDelete();
                    flowLayoutPanel1.Controls[index - 1].Focus();
                }
                else
                    selectedItem = null;
                FilterItem_Checked();
            }
        }
        private void FilterItem_Selected(object sender)
        {
            if(selectedItem!=null)
                selectedItem.Appearance.BackColor = System.Drawing.SystemColors.Control;
            selectedItem = sender as FilterItem;
            selectedItem.Appearance.BackColor = System.Drawing.SystemColors.Highlight;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            selectedItem = null;
            this.flowLayoutPanel1.Controls.Clear();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (thisMod == Mod.Normal)
            {
                getSql();
            }
            else
            {
                SQLresult = memoEdit1.Text.Replace("/r"," ");
            }
            this.DialogResult = DialogResult.Yes; 
        }
        private void FilterItem_Checked(object sender)
        {
            if (thisMod == Mod.Normal)
            {
                if (!(sender as FilterItem).isChecked)
                {
                    this.OKButton.Enabled = false;
                    this.CreateSQLButton.Enabled = false;
                    return;
                }
                if (this.flowLayoutPanel1.Controls.Count == 0)
                {
                    this.OKButton.Enabled = true;
                    this.CreateSQLButton.Enabled = true;
                    return;
                }
                foreach (FilterItem current in flowLayoutPanel1.Controls)
                {
                    if (!current.isChecked)
                    {
                        this.OKButton.Enabled = false;
                        this.CreateSQLButton.Enabled = false;
                        return;
                    }
                }
                this.OKButton.Enabled = true;
                this.CreateSQLButton.Enabled = true;
            }
            else
            {
                if (!(sender as FilterItem).isChecked)
                {
                    this.CreateSQLButton.Enabled = false;
                    return;
                }
                if (this.flowLayoutPanel1.Controls.Count == 0)
                {
                    this.CreateSQLButton.Enabled = true;
                    return;
                }
                foreach (FilterItem current in flowLayoutPanel1.Controls)
                {
                    if (!current.isChecked)
                    {
                        this.CreateSQLButton.Enabled = false;
                        return;
                    }
                }
                this.CreateSQLButton.Enabled = true;
            }
        }
        private void FilterItem_Checked()
        {
            if (thisMod == Mod.Normal)
            {
                if (this.flowLayoutPanel1.Controls.Count == 0)
                {
                    this.OKButton.Enabled = true;
                    this.CreateSQLButton.Enabled = true;
                    return;
                }
                foreach (FilterItem current in flowLayoutPanel1.Controls)
                {
                    if (!current.isChecked)
                    {
                        this.OKButton.Enabled = false;
                        this.CreateSQLButton.Enabled = false;
                        return;
                    }
                }
                this.OKButton.Enabled = true;
                this.CreateSQLButton.Enabled = true;
            }
            else
            {
                if (this.flowLayoutPanel1.Controls.Count == 0)
                {
                    this.CreateSQLButton.Enabled = true;
                    return;
                }
                foreach (FilterItem current in flowLayoutPanel1.Controls)
                {
                    if (!current.isChecked)
                    {
                        this.CreateSQLButton.Enabled = false;
                        return;
                    }
                }
                this.CreateSQLButton.Enabled = true;
            }
        }
        private void FilterItem_NeedValues(object sender,NeedValueEventArgs e)
        {
            FilterColumnsValue[e.Index] = getDistinctValues(e.Index);
        }
        private List<string> getDistinctValues(int index)
        {
            List<string> datatemp = ServiceContainer.GetService<STCTIService>().ST_Select_Reader(TableName,true,new string[]{myColumns[index].column},conn)[0];
            return datatemp;
        }
        private void getSql()
        {
            SQLresult = "";
            if (this.flowLayoutPanel1.Controls.Count == 0)
                return;
            else
            {
                foreach (FilterItem current in flowLayoutPanel1.Controls)
                {
                    SQLresult += current.getSql();
                }
            }
        }

        private void CreateSQLButton_Click(object sender, EventArgs e)
        {
            getSql();
            this.memoEdit1.Text = SQLresult;
        }

        private void AdvanceButton_Click(object sender, EventArgs e)
        {
            if (thisMod == Mod.Normal)
            {
                ChangeMod(Mod.Advance);
                AdvanceButton.Text = "收起<<";
            }
            else
            {
                ChangeMod(Mod.Normal);
                AdvanceButton.Text = "高级>>";
            }
        }
        private void ChangeMod(Mod target)
        {
            if(target == Mod.Normal)
            {
                thisMod = Mod.Normal;
                groupAdvance.Hide();
                this.Size = new Size(635, 386);
                SQLresult = "";
                xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                xtraTabControl1.Size = new System.Drawing.Size(619,343);
                FilterItem_Checked();
            }
            else
            {
                thisMod = Mod.Advance;
                groupAdvance.Show();
                OKButton.Enabled = true;
                xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                xtraTabControl1.Size = new System.Drawing.Size(619, 315);
                this.Size = new Size(635, 568);
            }
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index =this.listBoxControl1.SelectedIndex; 
            switch(myColumns[index].filter_Type)
            {
                case FilterType.FK:
                    {
                        if (FilterColumnsValue[index] == null)
                            FilterColumnsValue[index] = myColumns[index].value_Original;
                        this.listBoxControl2.Items.Clear();
                        this.listBoxControl2.DataSource = null;
                        this.listBoxControl2.DataSource = myColumns[index].value_Show;
                        break;
                    }
                default:
                    {
                        if (FilterColumnsValue[index] == null)
                            FilterColumnsValue[index] = getDistinctValues(index);
                        this.listBoxControl2.Items.Clear();
                        this.listBoxControl2.DataSource = null;
                        this.listBoxControl2.DataSource = FilterColumnsValue[index];
                        break;
                    }
            }
        }

        private void simpleButton_Click(object sender, EventArgs e)
        {
            this.memoEdit1.Text += " " + (sender as SimpleButton).Text+" ";
        }

        private void listBoxControl1_DoubleClick(object sender, EventArgs e)
        {
            if((sender as ListBoxControl).SelectedIndex != -1)
            {
                this.memoEdit1.Text += " " + myColumns[(sender as ListBoxControl).SelectedIndex].column + " ";
            }
        }

        private void listBoxControl2_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as ListBoxControl).SelectedIndex != -1)
            {
                this.memoEdit1.Text += " " + FilterColumnsValue[listBoxControl1.SelectedIndex][(sender as ListBoxControl).SelectedIndex] + " ";
            }
        }

    }
}
