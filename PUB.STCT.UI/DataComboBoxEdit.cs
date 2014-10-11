using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace PUB.STCT.Client
{
    public class DataComboBoxEdit : ComboBoxEdit
    {
        public List<string> TextOrignal { get; set; }
        public List<string> TextShow { get; set; }
        public DataComboBoxType Type { get; set; }
        public DataComboBoxEdit()
            : base()
        {
            this.Type = DataComboBoxType.Normal;
            this.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.comboBoxEdit_CustomDisplayText);
        }
        public void AddRange(List<string> textorignal, List<string> textshow)
        {
            this.TextOrignal = textorignal;
            this.TextShow = textshow;
            this.Properties.Items.AddRange(textshow);
        }
        public override string Text
        {
            get
            {
                if (this.Type == DataComboBoxType.Advance && this.SelectedIndex >= 0)
                {
                    return this.TextOrignal[this.SelectedIndex];
                }
                else
                    return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        private void comboBoxEdit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (e.Value != null)
                if (this.Type == DataComboBoxType.Advance&&TextShow!=null&&TextOrignal!=null)
                {
                    int i = TextOrignal.IndexOf(e.Value.ToString());
                    e.DisplayText = i >= 0 ? TextShow[i] : e.Value.ToString();
                }
                else
                    e.DisplayText = e.Value.ToString();
        }
        public enum DataComboBoxType
        {
            Normal = 0,
            Advance = 1
        }
    }
}
