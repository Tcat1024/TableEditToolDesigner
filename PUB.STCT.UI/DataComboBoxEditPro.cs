using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;

namespace PUB.STCT.Client
{
    [Designer(typeof(EditBoxDesigner))]
    public partial class DataComboBoxEditPro : XtraUserControl,DataBaseEditPro
    {
        private DataComboBoxEdit.DataComboBoxType _Type;
        [DefaultValue(DataComboBoxEdit.DataComboBoxType.Normal)]
        public DataComboBoxEdit.DataComboBoxType Type
        {
            get
            {
                return this.BaseEdit.Type;
            }
            set
            {
                this._Type = value;
                this.BaseEdit.Type = _Type;
            }
        }
        public void SetSource(List<string> original, List<string> show)
        {
            this.BaseEdit.AddRange(original, show);
        }
        public DataComboBoxEditPro()
        {
            InitializeComponent();
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            this.BaseEdit.Font = this.Font;
            this.labelControl.Font = this.Font;
            this.MaximumSize = new Size(this.DefaultMaximumSize.Width, this.BaseEdit.Height);
            this.MinimumSize = new Size(this.DefaultMaximumSize.Width, this.BaseEdit.Height);
            base.SetBoundsCore(x, y, width, this.BaseEdit.Height, specified);
            this.BaseEdit.Width = this.Width - this.labelControl.Width;
            this.labelControl.Location = new Point(this.BaseEdit.Width, (this.Height - this.labelControl.Height) / 2);
        }
        public new string Text
        {
            get
            {
                return this.BaseEdit.Text;
            }
            set
            {
                this.BaseEdit.SelectedIndex = this.BaseEdit.TextOrignal.IndexOf(value);
            }
        }
        private string _DefalutText="";
        [DefaultValue("")]
        public string DefalutText
        {
            get
            {
                return this._DefalutText;
            }
            set
            {
                this._DefalutText = value;
                this.BaseEdit.Text = value;
            }
        }
        private string _ExText = "";
        [DefaultValue("")]
        public string ExText
        {
            get
            {
                return _ExText;
            }
            set
            {
                _ExText = value;
                this.labelControl.Text = value;
                this.Width = this.BaseEdit.Width + this.labelControl.Width;
            }
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                this.BaseEdit.Font = value;
                this.labelControl.Font = value;
                int width = this.Width;
                this.Size = new Size(this.BaseEdit.Width + this.labelControl.Width, this.BaseEdit.Height);
            }
        }
        [DefaultValue("")]
        public string Formatstring
        {
            get
            {
                return this.BaseEdit.Properties.EditFormat.FormatString;
            }
            set
            {
                this.BaseEdit.Properties.EditFormat.FormatString = value;
                this.BaseEdit.Properties.DisplayFormat.FormatString = value;
            }
        }
        [DefaultValue(DevExpress.Utils.FormatType.None)]
        public DevExpress.Utils.FormatType FormatType
        {
            get
            {
                return this.BaseEdit.Properties.EditFormat.FormatType;
            }
            set
            {
                this.BaseEdit.Properties.EditFormat.FormatType = value;
                this.BaseEdit.Properties.DisplayFormat.FormatType = value;
            }
        }
        private MaskType _Mask = MaskType.String;
        [DefaultValue(MaskType.String)]
        public MaskType Mask
        {
            get
            {
                return _Mask;
            }
            set
            {
                this._Mask = value;
                switch (_Mask)
                {
                    case MaskType.DateTime: this.BaseEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(delegate(object sender, KeyPressEventArgs e)
                    {
                        if (!Char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '/' && e.KeyChar != '-' && e.KeyChar != ':' && e.KeyChar != ' ')
                        {
                            e.Handled = true;
                        }
                    }); break;
                    case MaskType.Number: this.BaseEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(delegate(object sender, KeyPressEventArgs e)
                    {
                        if (!Char.IsNumber(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
                        {
                            e.Handled = true;
                        }
                    }); break;
                    default: break;
                }
            }
        }
        public void SetNewBind(string property, BindingSource myBind, string target, bool check, DataSourceUpdateMode myMod)
        {
            this.BaseEdit.DataBindings.Clear();
            this.BaseEdit.DataBindings.Add(property, myBind, target, check, myMod);
        }
        public void SetDefalut()
        {
            this.BaseEdit.Text = this.DefalutText;
        }
    }
}
