using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Xml;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using EAS.Explorer.WinUI;

namespace FormDesignerWizard
{
    public partial class Form1 : XtraForm
    {
        const int FormStringCount = 3;
        const int DesignerStringCount = 8;
        const int ColumnMax = 50;
        private string conStr = "";
        private OleDbConnection conn;
        private string tablename = "";
        private List<string> tableNames = new List<string>();
        private List<string>[] columnNames;
        private List<columnInfo> myCloumns = new List<columnInfo>();
        private columnInfo[] infoCloumns = new columnInfo[4];
        private BindingSource myBind = new BindingSource();
        CodeString FormString = new CodeString("FormString",FormStringCount);
        CodeString DesignerString = new CodeString("DesignerString",DesignerStringCount);
        CodeString FormDynamicString = new CodeString("FormDynamicString",FormStringCount);
        CodeString DesignerDynamicString = new CodeString("DesignerDynamicString",DesignerStringCount);
        private BackgroundWorker Thread_initString = new BackgroundWorker();
        private BackgroundWorker Thread_initDynamicString = new BackgroundWorker();
        private BackgroundWorker Thread_generateFile = new BackgroundWorker();
        private BackgroundWorker Thread_initColumn = new BackgroundWorker();
        private FolderBrowserDialog myBrowser = new FolderBrowserDialog();
        private int _initComplete = 0;
        private object initComplete = new object();
        //private int _columnComplete = 0;
        //private object columnComplete = new object();
        private bool initDynamicComplete = false;
        private bool page2PutinComplete = false;
        private int columnCount = 0;
        private int editColumnCount = 0;
        private int filterColumnCount = 0;
        private int infoColumnCount = 0;
        private string columns = "";
        private string t = "    ";
        private int _page1Check=0;
        private List<string>[] InfoColumnName = new List<string>[4];
        private ComboBoxEdit[] InfoColumnComboBox = new ComboBoxEdit[4];
        private int page1Check
        {
            get
            {
                return this._page1Check;
            }
            set
            {
                _page1Check = value;
                if (_page1Check == 0)
                {
                    this.wizardPage1.AllowNext = true;
                }
                else
                    this.wizardPage1.AllowNext = false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
                this.InfoColumnName[i] = new List<string>();
            InitConnect();
            this.checkBox1.Checked = true;
            this.comboBoxEdit2.SelectedIndex = 0;
            this.comboBoxEdit1.Enabled = false;
            this.welcomeWizardPage1.AllowNext = false;
            this.completionWizardPage1.AllowNext = true;
            this.wizardPage1.AllowNext = false;
            this.wizardPage2.AllowNext = false;
            this.page1Check=this.page1Check|2;
            this.Thread_initString.DoWork += (object DWsender, DoWorkEventArgs DWe) => { this.InitStrings(); };
            this.Thread_initString.RunWorkerCompleted += (object RWsender, RunWorkerCompletedEventArgs RWe) => {
                this.page1Check = this.page1Check & (this.page1Check^2);
            };
            this.Thread_initString.RunWorkerAsync();
            this.Thread_initDynamicString.DoWork += (object DWsender, DoWorkEventArgs DWe) => { this.InitDynamicStrings(); };
            this.Thread_initDynamicString.RunWorkerCompleted += (object RWsender, RunWorkerCompletedEventArgs RWe) => { this.initDynamicComplete = true; page2_check(); };
            this.Thread_generateFile.DoWork += (object DWsender, DoWorkEventArgs DWe) => { this.generateFile(); };
            this.Thread_generateFile.RunWorkerCompleted += (object RWsender, RunWorkerCompletedEventArgs RWe) => { this.generateFileFinished(); };
            this.Thread_initColumn.DoWork += (object DWsender, DoWorkEventArgs DWe) => { this.ThreadInitColumn(); };
            this.Thread_initColumn.RunWorkerCompleted += (object RWsender, RunWorkerCompletedEventArgs RWe) =>{ this.initColumnFinished(); };
            Thread temp = new Thread(() =>
            {
                this.InfoColumnComboBox[0] = this.createTimeComboBox;
                this.InfoColumnComboBox[1] = this.createPersonComboBox;
                this.InfoColumnComboBox[2] = this.alterTimeComboBox;
                this.InfoColumnComboBox[3] = this.alterPersonComboBox;
            });
            temp.Start();
        }
        private void InitStrings()
        {
            _initComplete = 0;
            for (int i = 0; i < FormStringCount; i++)
                ThreadPool.QueueUserWorkItem(InitString, new threadstate(FormString,i));
            for (int i = 0; i < DesignerStringCount; i++)
                ThreadPool.QueueUserWorkItem(InitString, new threadstate(DesignerString, i));
            while (this._initComplete != FormStringCount+DesignerStringCount) ;
        }
        private void InitString(object state)
        {
            StreamReader sr = new StreamReader((state as threadstate).data.type+"\\"+(state as threadstate).data.type + ((state as threadstate).number + 1).ToString() + ".txt");
            (state as threadstate).data.strings[(state as threadstate).number] = sr.ReadToEnd();
            lock(initComplete)
            {
                _initComplete++;
            }
        }
        private void InitDynamicStrings()
        {
            columnCount = 0;
            editColumnCount = 0;
            filterColumnCount = 0;
            columns = "";
            string initControls = "\r\n" + t+t + "private void InitControls()" + "\r\n"
                + t+t + "{" + "\r\n";
            string initColumn = "\r\n" + t+t + "private void InitColumn()" + "\r\n"
                + t+t + "{" + "\r\n"
                + t+t+t + "int i = 0, j = 0, k = 0;" + "\r\n"
                + t+t+t + "ColumnInfo temp;" + "\r\n";
            string initSetEditText = "\r\n" + t+t + "private void SetEditText()" + "\r\n"
                + t+t + "{" + "\r\n";
            string initCustomColumnDisplayText = "\r\n" + t+t + "private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)" + "\r\n"
                + t+t + "{" + "\r\n";
            string newEdit = "\r\n";
            string newLayout = "\r\n";
            string declareEdit = "\r\n";
            string declareLayout = "\r\n";
            string initLayout = "\r\n";
            string endInitLayout = "\r\n";
            string newColumn = "\r\n";
            string declareColumn = "\r\n";
            string mainLayoutControl = "\r\n"+ t + t + t + @"//" + "\r\n"
                + t + t + t + @"// MainLayoutControl" + "\r\n"
                + t + t + t + @"//" + "\r\n";
            string mainLayoutControlGroup = "\r\n" + t + t + t + @"//" + "\r\n"
                + t + t + t + @"// MainLayoutControlGroup" + "\r\n"
                + t + t + t + @"//" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.CustomizationFormText = \"MainLayoutControlGroup\";" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.GroupBordersVisible = false;" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {" + "\r\n";
            string mainGridView = "\r\n" + t + t + t + @"//" + "\r\n"
                + t + t + t + @"// MainGridView" + "\r\n"
                + t + t + t + @"//" + "\r\n"
                + t + t + t + "this.MainGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;" + "\r\n"
                + t + t + t + "this.MainGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {" + "\r\n";
            string dataEdit = "\r\n";
            string layoutControlItem = "\r\n";
            string layoutControlGroup = "\r\n";
            string gridColumn = "\r\n";
            bool firstFk = true;
            for (int i = 0; i < myCloumns.Count;i++ )
            {
                if (columnCount != 0)
                    columns += ",";
                columns += "\"" + myCloumns[i].column.ToUpper() + "\"";
                columnCount++;
                gridColumn += t + t + t + @"//" + "\r\n"
                        + t + t + t + @"// gridColumn" + columnCount.ToString() + "\r\n"
                        + t + t + t + @"//" + "\r\n";
                if (myCloumns[i].column_Name != null && myCloumns[i].column_Name.Trim() != "")
                {
                    initColumn += t + t + t + "temp = new ColumnInfo(\"" + myCloumns[i].column.ToUpper() + "\", \"" + myCloumns[i].column_Name + "\", \"" + myCloumns[i].type + "\"," + myCloumns[i].edit_Allow.ToString().ToLower()
                        + ", MaskType." + myCloumns[i].type_Mask + ", EditType." + myCloumns[i].type_Edit + ", FormatType." + myCloumns[i].format_Type + ", \"" + myCloumns[i].format_String + "\", \"" + myCloumns[i].unit + "\", " + myCloumns[i].filter_Allow.ToString().ToLower() + ", FilterType." + myCloumns[i].filter_Type + ", " + myCloumns[i].fk_Allow.ToString().ToLower() + ", \"" + myCloumns[i].fk_Table + "\", \"" + myCloumns[i].fk_ID + "\", \"" + myCloumns[i].fk_Data + "\");" + "\r\n"
                        + t + t + t + "myColumns[i++] = temp;" + "\r\n";
                    gridColumn += t + t + t + "this.gridColumn" + columnCount.ToString() + ".Caption = \"" + myCloumns[i].column_Name + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".FieldName = \"" + myCloumns[i].column.ToUpper() + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".Name = \"gridColumn" + columnCount.ToString() + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".Visible = true;" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".VisibleIndex = " + (columnCount - 1).ToString() + ";" + "\r\n";
                }
                else
                {
                    initColumn += t + t + t + "temp = new ColumnInfo(\"" + myCloumns[i].column.ToUpper() + "\", \"" + myCloumns[i].column.ToUpper() + "\", \"" + myCloumns[i].type + "\"," + myCloumns[i].edit_Allow.ToString().ToLower()
                        + ", MaskType." + myCloumns[i].type_Mask + ", EditType." + myCloumns[i].type_Edit + ", FormatType." + myCloumns[i].format_Type + ", \"" + myCloumns[i].format_String + "\", \"" + myCloumns[i].unit + "\", " + myCloumns[i].filter_Allow.ToString().ToLower() + ", FilterType." + myCloumns[i].filter_Type + ", " + myCloumns[i].fk_Allow.ToString().ToLower() + ", \"" + myCloumns[i].fk_Table + "\", \"" + myCloumns[i].fk_ID + "\", \"" + myCloumns[i].fk_Data + "\");" + "\r\n"
                        + t + t + t + "myColumns[i++] = temp;" + "\r\n";
                    gridColumn += t + t + t + "this.gridColumn" + columnCount.ToString() + ".Caption = \"" + myCloumns[i].column.ToUpper() + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".FieldName = \"" + myCloumns[i].column.ToUpper() + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".Name = \"gridColumn" + columnCount.ToString() + "\";" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".Visible = true;" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".VisibleIndex = " + (columnCount - 1).ToString() + ";" + "\r\n";
                }
                if (myCloumns[i].isPK)
                    gridColumn += t + t + t + "this.gridColumn" + columnCount.ToString() + ".AppearanceHeader.ForeColor = System.Drawing.Color.Blue;" + "\r\n"
                        + t + t + t + "this.gridColumn" + columnCount.ToString() + ".AppearanceHeader.Options.UseForeColor = true;" + "\r\n";
                newColumn += t + t + t + "this.gridColumn" + columnCount.ToString() + " = new DevExpress.XtraGrid.Columns.GridColumn();" + "\r\n";
                declareColumn += t + t + "private DevExpress.XtraGrid.Columns.GridColumn gridColumn" + columnCount.ToString() + ";" + "\r\n";
                if (columnCount!=1)
                    mainGridView += "," + "\r\n";
                mainGridView += t + t + t + "this.gridColumn" + columnCount.ToString();
                if (myCloumns[i].edit_Allow == true)
                {
                    editColumnCount++;
                    initControls += t + t + t + "editItems[" + (editColumnCount - 1).ToString() + "] = this.layoutControlItem" + editColumnCount.ToString() + ";" + "\r\n";
                    initColumn += t + t + t + "editItems[k++].Tag = temp;" + "\r\n";
                    if ((editColumnCount-1) % 50 == 0)
                    {
                        int temp = (editColumnCount-1) / 50 + 1;
                        newLayout += t + t + t + "this.layoutControlGroup" + temp.ToString() + " = new DevExpress.XtraLayout.LayoutControlGroup();" + "\r\n";
                        declareLayout += t + t + "private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup" + temp.ToString() + ";" + "\r\n";
                        initLayout += t + t + t + "((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup" + temp.ToString() + ")).BeginInit();" + "\r\n";
                        endInitLayout += t + t + t + "((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup" + temp.ToString() + ")).EndInit();" + "\r\n";
                        if ((editColumnCount-1) / 50 != 0)
                        {
                            mainLayoutControlGroup += "," + "\r\n";
                            layoutControlGroup += "});" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Location = new System.Drawing.Point(0, " + 1302 * (temp - 2) + ");" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Name = \"layoutControlGroup" + (temp - 1).ToString() + "\";" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Size = new System.Drawing.Size(591, 1302);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".Text = \"layoutControlGroup" + (temp - 1).ToString() + "\";" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + (temp - 1).ToString() + ".TextVisible = false;" + "\r\n";
                        }
                        mainLayoutControlGroup += t + t + t + "this.layoutControlGroup" + temp.ToString();
                        layoutControlGroup += t + t + t + @"//" + "\r\n"
                            + t + t + t + @"// layoutControlGroup" + temp.ToString() + "\r\n"
                            + t + t + t + @"//" + "\r\n"
                            + t + t + t + "this.layoutControlGroup" + temp.ToString() + ".CustomizationFormText = \"layoutControlGroup" + temp.ToString() + "\";" + "\r\n"
                            + t + t + t + "this.layoutControlGroup" + temp.ToString() + ".Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {" + "\r\n";
                    }
                    newLayout += t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + " = new DevExpress.XtraLayout.LayoutControlItem();" + "\r\n";
                    declareLayout += t + t + "private DevExpress.XtraLayout.LayoutControlItem layoutControlItem" + editColumnCount.ToString() + ";" + "\r\n";
                    initLayout += t + t + t + "((System.ComponentModel.ISupportInitialize)(this.layoutControlItem" + editColumnCount.ToString() + ")).BeginInit();" + "\r\n";
                    endInitLayout += t + t + t + "((System.ComponentModel.ISupportInitialize)(this.layoutControlItem" + editColumnCount.ToString() + ")).EndInit();" + "\r\n";
                    mainLayoutControl += t + t + t + "this.MainLayoutControl.Controls.Add(this.dataEdit" + editColumnCount.ToString() + ");" + "\r\n";
                    if (editColumnCount%50!=1)
                        layoutControlGroup += "," + "\r\n";
                    layoutControlGroup += t + t + t + "this.layoutControlItem" + editColumnCount.ToString();
                    dataEdit += t + t + t + @"//" + "\r\n"
                        + t + t + t + @"// dataEdit" + editColumnCount.ToString() + "\r\n"
                        + t + t + t + @"//" + "\r\n";
                    if (myCloumns[i].unit != null && myCloumns[i].unit.Trim() != "")
                        dataEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".ExText = \"" + myCloumns[i].unit + "\";" + "\r\n";
                    if (myCloumns[i].format_Type != "None" && myCloumns[i].format_Type.Trim() != "" && myCloumns[i].format_Type != null && myCloumns[i].format_String != null && myCloumns[i].format_String.Trim() != "")
                        dataEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".FormatType = DevExpress.Utils.FormatType." + myCloumns[i].format_Type + ";" + "\r\n"
                            + t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Formatstring = \"" + myCloumns[i].format_String + "\";" + "\r\n";
                    if (myCloumns[i].type_Mask != "String" && myCloumns[i].type_Mask != null && myCloumns[i].type_Mask.Trim() != "")
                        dataEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Mask = PUB.STCT.Client.MaskType." + myCloumns[i].type_Mask + ";" + "\r\n";
                    dataEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Location = new System.Drawing.Point(147, " + (0 + (editColumnCount-1) * 26+2*(editColumnCount-1)/50).ToString() + ");" + "\r\n"
                        + t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".MaximumSize = new System.Drawing.Size(0, 22);" + "\r\n"
                        + t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".MinimumSize = new System.Drawing.Size(0, 22);" + "\r\n"
                        + t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Name = \"dataEdit" + editColumnCount.ToString() + "\";" + "\r\n"
                        + t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Size = new System.Drawing.Size(542, 22);" + "\r\n";
                    if (myCloumns[i].IsInfoColumn == true)
                        dataEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + ".Enabled = false;" + "\r\n";
                    layoutControlItem += t + t + t + @"//" + "\r\n"
                        + t + t + t + @"// layoutControlItem" + editColumnCount.ToString() + "\r\n"
                        + t + t + t + @"//" + "\r\n";
                    if (myCloumns[i].isPK)
                        layoutControlItem += t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;" + "\r\n"
                            + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".AppearanceItemCaption.Options.UseForeColor = true;" + "\r\n";
                    layoutControlItem += t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".AppearanceItemCaption.Options.UseTextOptions = true;" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Control = this.dataEdit" + editColumnCount.ToString() + ";" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Location = new System.Drawing.Point(0, " + (((editColumnCount - 1) % 50 ) * 26).ToString() + ");" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Name = \"layoutControlItem2\";" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Size = new System.Drawing.Size(572, 26);" + "\r\n"
                        + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".TextSize = new System.Drawing.Size(42, 17);" + "\r\n";
                    if (myCloumns[i].column_Name != null && myCloumns[i].column_Name.Trim() != "")
                        layoutControlItem += t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Text = \"" + myCloumns[i].column_Name + "：\";" + "\r\n"
                            + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".CustomizationFormText = \"" + myCloumns[i].column_Name + "：\";" + "\r\n";
                    else
                        layoutControlItem += t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".Text = \"" + myCloumns[i].column.ToUpper() + "：\";" + "\r\n"
                            + t + t + t + "this.layoutControlItem" + editColumnCount.ToString() + ".CustomizationFormText = \"" + myCloumns[i].column.ToUpper() + "：\";" + "\r\n";

                    switch(myCloumns[i].type_Edit)
                    {
                        case "Text": 
                            if(!myCloumns[i].IsInfoColumn)
                                initSetEditText += t + t + t + "(editItems[" + (editColumnCount - 1).ToString() + "].Control as DataTextEditPro).Text = currentRow[(editItems[" + (editColumnCount - 1).ToString() + "].Tag as ColumnInfo).column].ToString();" + "\r\n";
                            newEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + " = new PUB.STCT.Client.DataTextEditPro();" + "\r\n";
                            declareEdit += t + t + "private PUB.STCT.Client.DataTextEditPro dataEdit" + editColumnCount.ToString() + ";" + "\r\n";
                            break;
                        case "DateTime":
                            if (!myCloumns[i].IsInfoColumn)
                                initSetEditText += t + t + t + "(editItems[" + (editColumnCount - 1).ToString() + "].Control as DataDateEditPro).Text = currentRow[(editItems[" + (editColumnCount - 1).ToString() + "].Tag as ColumnInfo).column].ToString();" + "\r\n";
                            newEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + " = new PUB.STCT.Client.DataDateEditPro();" + "\r\n";
                            declareEdit += t + t + "private PUB.STCT.Client.DataDateEditPro dataEdit" + editColumnCount.ToString() + ";" + "\r\n";
                            break;
                        case "FK":
                            if (!myCloumns[i].IsInfoColumn)
                                initSetEditText += t + t + t + "(editItems[" + (editColumnCount - 1).ToString() + "].Control as DataComboBoxEditPro).Text = currentRow[(editItems[" + (editColumnCount - 1).ToString() + "].Tag as ColumnInfo).column].ToString();" + "\r\n";
                            newEdit += t + t + t + "this.dataEdit" + editColumnCount.ToString() + " = new PUB.STCT.Client.DataComboBoxEditPro();" + "\r\n";
                            declareEdit += t + t + t + "private PUB.STCT.Client.DataComboBoxEditPro dataEdit" + editColumnCount.ToString() + ";" + "\r\n";
                            dataEdit += t + t + "this.dataEdit" + editColumnCount.ToString() + ".Type = PUB.STCT.Client.DataComboBoxEdit.DataComboBoxType.Advance;" + "\r\n";
                            break;
                    }
                }
                if (myCloumns[i].filter_Allow == true)
                {
                    filterColumnCount++;
                    initColumn += t + t + t + "filterColumns[j++] = temp;" + "\r\n";
                }
                if (myCloumns[i].IsInfoColumn ==true)
                {
                    infoColumnCount++;
                    initColumn += t + t + t + "infoColumns[" + myCloumns[i].InfoType + "] = temp;" + "\r\n";
                }
                if(myCloumns[i].fk_Allow==true)
                {
                    initColumn += t + t + t + "temp.getFK(conn);" + "\r\n";
                    if (!firstFk)
                        initCustomColumnDisplayText += t + t + t + "else ";
                    else
                        initCustomColumnDisplayText += t + t + t;
                    initCustomColumnDisplayText += "if (e.Column == gridColumn" + (i + 1).ToString() + ")" + "\r\n"
                        + t + t + t + "{" + "\r\n"
                        + t + t + t + t + "try" + "\r\n"
                        + t + t + t + t + "{" + "\r\n"
                        + t + t + t + t + t + "e.DisplayText = myColumns[" + i.ToString() + "].value_Show[myColumns[" + i.ToString() + "].value_Original.IndexOf(e.Value.ToString())];" + "\r\n"
                        + t + t + t + t + "}" + "\r\n"
                        + t + t + t + t + "catch" + "\r\n"
                        + t + t + t + t + "{" + "\r\n"
                        + t + t + t + t + "}" + "\r\n"
                        + t + t + t + "}" + "\r\n";
                    firstFk = false;
                }
            }
            initControls += t+t + "}" + "\r\n";
            initColumn += t+t + "}" + "\r\n";
            initSetEditText += t+t + "}" + "\r\n";
            initCustomColumnDisplayText += t+t + "}" + "\r\n";
            mainLayoutControl += t + t + t + "this.MainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;" + "\r\n"
                + t + t + t + "this.MainLayoutControl.Location = new System.Drawing.Point(0, 0);" + "\r\n"
                + t + t + t + "this.MainLayoutControl.Name = \"MainLayoutControl\";" + "\r\n"
                + t + t + t + "this.MainLayoutControl.Root = this.MainLayoutControlGroup;" + "\r\n"
                + t + t + t + "this.MainLayoutControl.Size = new System.Drawing.Size(791, 518);" + "\r\n"
                + t + t + t + "this.MainLayoutControl.TabIndex = 0;" + "\r\n"
                + t + t + t + "this.MainLayoutControl.Text = \"layoutControl1\";" + "\r\n";
            mainLayoutControlGroup+="});"+"\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Location = new System.Drawing.Point(0, 0);" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Name = \"MainLayoutControlGroup\";" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(100, 100, 30, 30);" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Size = new System.Drawing.Size(791, 518);" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.Text = \"MainLayoutControlGroup\";" + "\r\n"
                + t + t + t + "this.MainLayoutControlGroup.TextVisible = false;" + "\r\n";
            layoutControlGroup += "});" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Location = new System.Drawing.Point(0, " + 1302 * ((editColumnCount - 1) / 50) + ");" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Name = \"layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + "\";" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Size = new System.Drawing.Size(591, 1302);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".Text = \"layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + "\";" + "\r\n"
                                + t + t + t + "this.layoutControlGroup" + ((editColumnCount - 1) / 50 + 1).ToString() + ".TextVisible = false;" + "\r\n";
            mainGridView += "});" + "\r\n"
                + t + t + t + "this.MainGridView.GridControl = this.MainGridControl;" + "\r\n"
                + t + t + t + "this.MainGridView.Name = \"MainGridView\";" + "\r\n"
                + t + t + t + "this.MainGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;" + "\r\n"
                + t + t + t + "this.MainGridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;" + "\r\n"
                + t + t + t + "this.MainGridView.OptionsBehavior.Editable = false;" + "\r\n"
                + t + t + t + "this.MainGridView.OptionsView.ColumnAutoWidth = false;" + "\r\n"
                + t + t + t + "this.MainGridView.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(this.gridView1_InvalidRowException);" + "\r\n"
                + t + t + t + "this.MainGridView.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);" + "\r\n"
                + t + t + t + "this.MainGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);" + "\r\n";
            this.FormDynamicString.strings[2] = initControls + initColumn + initSetEditText + initCustomColumnDisplayText + t +"}\r\n"+"}\r\n";
            this.DesignerDynamicString.strings[2] = newEdit + newLayout + newColumn;
            this.DesignerDynamicString.strings[3] = initLayout;
            this.DesignerDynamicString.strings[4] = mainLayoutControl + dataEdit + mainLayoutControlGroup + layoutControlGroup + layoutControlItem;
            this.DesignerDynamicString.strings[5] = mainGridView + gridColumn;
            this.DesignerDynamicString.strings[6] = endInitLayout;
            this.DesignerDynamicString.strings[7] = declareLayout + declareColumn + declareEdit + t + "}\r\n" + "}\r\n";
        }
        private void completeFullString()
        {
            this.FormDynamicString.strings[0] = "\r\n" + "namespace " + this.textEdit2.Text + "\r\n"
                + "{" + "\r\n"
                + t + "[Module(\"" + this.textEdit6.Text + "\", \"" + this.textEdit4.Text + "\", \"" + this.textEdit5.Text + "\")]" + "\r\n"
                + t + "public partial class " + this.textEdit3.Text + " : UserControl" + "\r\n"
                + t + "{" + "\r\n"
                + t + t + "const int ColumnCount = " + this.columnCount.ToString() + ";" + "\r\n"
                + t + t + "const int EditColumnCount = " + this.editColumnCount.ToString() + ";" + "\r\n"
                + t + t + "const int FilterColumnCount = " + this.filterColumnCount.ToString() + ";" + "\r\n"
                + t + t + "const int InfoColumnCount = 4;" + "\r\n"
                + t + t + "string MoudleName = \"" + this.textEdit7.Text + "\";" + "\r\n"
                + t + t + "string TableName = \"" + this.tablename + "\";" + "\r\n"
                + t + t + "private string[] Columns = { " + this.columns + "};" + "\r\n"
                + t + t + "string conn;" + "\r\n"
                + "\r\n";
            this.FormDynamicString.strings[1] = "\r\n" + t + t + "public " + this.textEdit3.Text + "()" + "\r\n"
                + t + t + "{" + "\r\n"
                + t + t + t + "InitializeComponent();" + "\r\n";
            if (this.checkBox1.Checked)
                this.FormDynamicString.strings[1] += t + t + t + "this.conn = OraConnstr.get_connstr(\"" + this.comboBoxEdit2.Text + "\");" + "\r\n";
            else
                this.FormDynamicString.strings[1] += t + t + t + "this.conn = \"" + this.conStr + "\";" + "\r\n";
            this.FormDynamicString.strings[1] += t + t + "}" + "\r\n";
            this.DesignerDynamicString.strings[0] = "\r\n" + "namespace " + this.textEdit2.Text + "\r\n" 
                + "{" + "\r\n"
                + t + "partial class " + this.textEdit3.Text + "\r\n"
                + t + "{" + "\r\n";
            this.DesignerDynamicString.strings[1] = "\r\n" + t + t + t + "System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(" + this.textEdit3.Text + "));" + "\r\n";
            string fullFormString = "";
            for (int i = 0; i < FormStringCount; i++)
                fullFormString += this.FormString.strings[i] + this.FormDynamicString.strings[i];
            string fullDesignerString = "";
            for (int i = 0; i < DesignerStringCount; i++)
                fullDesignerString += this.DesignerString.strings[i] + this.DesignerDynamicString.strings[i];
            this.memoEdit1.Text = fullFormString;
            this.memoEdit2.Text = fullDesignerString;
        }
        private void InitConnect()
        {
            this.textEdit1.Properties.Items.Clear();
            if (!File.Exists("ConnectString.xml"))
            {
                XmlDocument doc = new XmlDocument();
                XmlElement Root = doc.CreateElement("document");
                XmlElement Con = doc.CreateElement("con");
                Root.AppendChild(Con);
                XmlElement InfoColumn = doc.CreateElement("InfoColumn");
                XmlElement CreateTimes = doc.CreateElement("CreateTimes");
                XmlElement CreatePersons = doc.CreateElement("CreatePersons");
                XmlElement AlterTimes = doc.CreateElement("AlterTimes");
                XmlElement AlterPersons = doc.CreateElement("AlterPersons");
                InfoColumn.AppendChild(CreateTimes);
                InfoColumn.AppendChild(CreatePersons);
                InfoColumn.AppendChild(AlterTimes);
                InfoColumn.AppendChild(AlterPersons);
                Root.AppendChild(InfoColumn);
                doc.AppendChild(Root);
                doc.Save("ConnectString.xml");
            }
            try
            {
                XmlReader rdr = XmlReader.Create("ConnectString.xml");
                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        if (rdr.Name == "ConnString")
                        {
                            string s = rdr["type"];
                            var temps = new ConnectInfoStringItem(rdr.ReadElementContentAsString());
                            this.textEdit1.Properties.Items.Add(temps);
                            if (s != null && s.Trim() != "")
                            {
                                var tempt = new ConnectInfoTypeItem(s);
                                tempt.str = temps;
                                temps.type = tempt;
                                this.comboBoxEdit2.Properties.Items.Add(tempt);
                            }
                        }
                        else if (rdr.Name == "CreateTime")
                            this.InfoColumnName[0].Add(rdr.ReadElementContentAsString());
                        else if (rdr.Name == "CreatePerson")
                            this.InfoColumnName[1].Add(rdr.ReadElementContentAsString());
                        else if (rdr.Name == "AlterTime")
                            this.InfoColumnName[2].Add(rdr.ReadElementContentAsString());
                        else if (rdr.Name == "AlterPerson")
                            this.InfoColumnName[3].Add(rdr.ReadElementContentAsString());
                    }
                }
                rdr.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void SaveConnect(string constr)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("ConnectString.xml");
            XmlElement newConn = doc.CreateElement("ConnString");
            newConn.InnerText = constr;
            doc.GetElementsByTagName("con")[0].InsertAfter(newConn, doc.GetElementsByTagName("con")[0].LastChild);
            XmlWriterSettings mySettings = new XmlWriterSettings();
            mySettings.Indent = true;
            XmlWriter tr = XmlWriter.Create("ConnectString.xml", mySettings);
            doc.WriteContentTo(tr);
            tr.Close();
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.textEdit1.Text != null && this.textEdit1.Text != "")
            {
                //this.conStr = "Provider = MSDAORA; Data Source = orcl; User ID = Tcat; Password = mao19911024;";
                try
                {
                    selectTable(this.textEdit1.Text);
                    this.comboBoxEdit1.Properties.Items.Clear();
                    this.comboBoxEdit1.Properties.Items.AddRange(tableNames);
                    this.comboBoxEdit1.Enabled = true;
                    var temp = new ConnectInfoStringItem(this.textEdit1.Text);
                    if (this.textEdit1.Properties.Items.IndexOf(temp)==-1)
                    {
                        SaveConnect(this.textEdit1.Text);
                        this.textEdit1.Properties.Items.Add(temp);
                    }
                    this.conStr = this.textEdit1.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private List<string> selectTable(string con)
        {
            var temp = new List<string>();
            conn = new OleDbConnection(con);
            OleDbCommand com = new OleDbCommand("select * from user_tables order by table_name", conn);
            conn.Open();
            OleDbDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                temp.Add(reader[0].ToString());
            }
            conn.Close();
            tableNames = temp;
            columnNames = new List<string>[tableNames.Count];
            return tableNames;
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBoxEdit1.SelectedIndex;
            if (comboBoxEdit1.Text != null && comboBoxEdit1.Text != "")
            {
                if(columnNames[index] == null)
                {
                    needColumns(index);
                }
                this.listBoxControl2.Items.Clear();
                this.listBoxControl1.Items.Clear();
                this.listBoxControl1.Items.AddRange(columnNames[index].ToArray());
                this.tablename = tableNames[index];
            }
            this.WelcomePage_check();
        }
        private void needColumns(int index)
        {
            columnNames[index] = new List<string>();
            OleDbCommand com = new OleDbCommand("select aa.COLUMN_NAME,bb.CONSTRAINT_TYPE from (select column_name from user_tab_col" +
                "umns where table_name='" + tableNames[index].Trim().ToUpper() + "' order by column_id) aa left join (select a.column_name,b.CONSTRAINT_TYPE " +
                "from user_cons_columns a, user_constraints b where a.constraint_name = b.constraint_name and b.constraint_type = 'P' " +
                "and a.table_name = '" + tableNames[index].Trim().ToUpper() + "') bb on aa.column_name = bb.column_name", conn);
            conn.Open();
            OleDbDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {

                if (reader[1] != null)
                {
                    if (reader[1].ToString() == "P")
                    {
                        columnNames[index].Add(reader[0].ToString() + "(PK)");
                        continue;
                    }
                }
                columnNames[index].Add(reader[0].ToString());
            }
            conn.Close();
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (this.listBoxControl1.SelectedItems != null)
            {
                foreach (object item in listBoxControl1.SelectedItems)
                {
                    if (!listBoxControl2.Items.Contains(item))
                        this.listBoxControl2.Items.Add(item);
                }
            }
            this.WelcomePage_check();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (this.listBoxControl2.SelectedItems != null)
            {
                for (int i = this.listBoxControl2.SelectedItems.Count - 1; i > -1; i--)
                    this.listBoxControl2.Items.Remove(this.listBoxControl2.SelectedItems[i]);
            }
            this.WelcomePage_check();
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show(this, "确定要退出吗？", "注意", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
        private class columnInfo
        {
            public event Action NeedUpdate;
            public columnInfo(string columnname,OleDbConnection conn,string tablename)
            {
                if (Regex.IsMatch(columnname, @"\b\S*(PK)\b"))
                {
                    this.column = columnname.Replace("(PK)", "");
                    this.isPK = true;
                }
                else
                {
                    this.column = columnname;
                    this.isPK = false;
                }
                this.edit_Allow = true;
                this.format_Type = "None";
                this.filter_Type = "Text";
                this.type_Edit = "Text";
                this.filter_Allow = true;
                this.fk_Allow = false;
                this.IsInfoColumn = false;
                this.InfoType = -1;
                OleDbCommand com = new OleDbCommand("SELECT b.data_type data_type,a.COMMENTS data_comment FROM user_col_comments a,all_tab_columns b WHERE a.table_name = b.table_name and a.table_name = '" + tablename + "'and a.COLUMN_NAME=b.COLUMN_NAME and b.COLUMN_NAME = '" + this.column + "'", conn);
                OleDbDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    this.type = reader[0].ToString();
                    this.column_Name = reader[1].ToString();
                    if (reader[0].ToString() == "NUMBER")
                    {
                        this.type_Mask = "Number";
                    }
                    else
                    {
                        this.type_Mask = "String";
                    }
                }
                
            }
            public string column { get; private set; }
            public string column_Name { get; set; }
            public bool isPK { get; set; }
            public string type { get; private set; }
            public bool edit_Allow { get; set; }
            public string type_Mask { get; set; }
            private string _type_Edit; 
            public string type_Edit 
            { 
                get
                {
                    return _type_Edit;
                }
                set
                {
                    if (value == "DateTime")
                    {
                        this.format_Type = "DateTime";
                        this.format_String = "yyyyMMddhhmmss";
                        if(NeedUpdate!=null)
                            NeedUpdate();
                    }
                    _type_Edit = value;
                }
            }
            public string format_Type { get; set; }
            public string format_String { get; set; }
            public string unit { get; set; }
            public bool filter_Allow { get; set; }
            public string filter_Type { get; set; }
            private bool _fk_Allow = false;
            public bool fk_Allow 
            { 
                get
                {
                    return this._fk_Allow;
                }
                set
                {
                    if (value)
                    {
                        this.type_Edit = "FK";
                        if (NeedUpdate != null)
                            NeedUpdate();
                    }
                    this._fk_Allow = value;
                }
            }
            public string fk_Table { get; set; }
            private string _fk_ID;
            public string fk_ID
            { 
                get
                {
                    return _fk_ID;
                }
                set
                {
                    if (Regex.IsMatch(value, @"\b\S*(PK)\b"))
                    {
                        _fk_ID = value.Replace("(PK)", "");
                    }
                    else
                    {
                        _fk_ID = value;
                    }

                }
            }
            private string _fk_Data;
            public string fk_Data 
            { 
                get
                {
                    return _fk_Data;
                }
                set
                {
                    if (Regex.IsMatch(value, @"\b\S*(PK)\b"))
                    {
                        _fk_Data = value.Replace("(PK)", "");
                    }
                    else
                    {
                        _fk_Data = value;
                    }
                }
            }
            public bool IsInfoColumn { get; set; }
            public int InfoType { get; set; }
            public override string ToString()
            {
                return this.column;
            }
            public override bool Equals(object obj)
            {
                return this.ToString() == obj.ToString();
            }
            public override int GetHashCode()
            {
                return this.column.GetHashCode();
            }
        }
        private void welcomeWizardPage1_PageCommit(object sender, EventArgs e)
        {
            this.page1Check=this.page1Check|1;
            this.gridControl1.DataSource = null;
            this.myCloumns.Clear();
            this.progressPanel2.Visible = true;           
            this.Thread_initColumn.RunWorkerAsync(); 
        }
        private void ThreadInitColumn()
        {
            int count = this.listBoxControl2.Items.Count;
            for (int j = 0; j <= count / ColumnMax; j++)
            {
                conn.Open();
                for (int i = 0; i < ((count - j * ColumnMax) >= 50 ? 50 : (count - j * ColumnMax)); i++)
                {
                    var temp = new columnInfo(this.listBoxControl2.Items[50 * j + i].ToString(), this.conn, this.tablename);
                    temp.NeedUpdate += () => { this.bandedGridView1.RefreshData(); };
                    this.myCloumns.Add(temp);
                }
                conn.Close();
            }
        }
        private void initColumnFinished()
        {
            myBind.DataSource = myCloumns;
            this.gridControl1.DataSource = myBind;
            this.FKTableComboBox.Items.AddRange(tableNames);
            InitInfoColumn();
            this.page1Check = this.page1Check&(this.page1Check^1);
            this.progressPanel2.Visible = false;
        }
        private void InitInfoColumn()
        {
            for(int i =0;i<4;i++)
            {
                this.InfoColumnComboBox[i].Properties.Items.AddRange(myCloumns);
                if (this.InfoColumnName[i] != null)
                {
                    foreach (var temp in InfoColumnName[i])
                    {
                        int index = this.InfoColumnComboBox[i].Properties.Items.IndexOf(temp);
                        if (index != -1)
                            this.InfoColumnComboBox[i].SelectedIndex = index;
                    }
                }
            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.bandedGridView1.GetFocusedRow() != null)
            {
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if (index > 0)
                {
                    columnInfo temp = this.myCloumns[index];
                    this.myCloumns.Remove(temp);
                    this.myCloumns.Insert(0, temp);
                    this.bandedGridView1.RefreshData();
                    this.myBind.MoveFirst();
                }
            }
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.bandedGridView1.GetFocusedRow() != null)
            {
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if(index>0)
                {
                    columnInfo temp = this.myCloumns[index];
                    this.myCloumns[index] = this.myCloumns[index - 1];
                    this.myCloumns[index - 1] = temp;
                    this.bandedGridView1.RefreshData();
                    this.myBind.MovePrevious();
                }
            }
        }
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.bandedGridView1.GetFocusedRow() != null)
            {
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if (index<this.myCloumns.Count-1)
                {
                    columnInfo temp = this.myCloumns[index];
                    this.myCloumns[index] = this.myCloumns[index + 1];
                    this.myCloumns[index +1] = temp;
                    this.bandedGridView1.RefreshData();
                    this.myBind.MoveNext();
                }
            }
        }
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.bandedGridView1.GetFocusedRow() != null)
            {
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if (index < this.myCloumns.Count - 1)
                {
                    columnInfo temp = this.myCloumns[index];
                    this.myCloumns.Remove(temp);
                    this.myCloumns.Add(temp);
                    this.bandedGridView1.RefreshData();
                    this.myBind.MoveLast();
                }
            }
        }
        private void bandedGridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (this.gridControl1.DataSource == null)
                return;  
            if(e.Column == this.Column)
            {
                if (myCloumns[e.RowHandle].isPK)
                    e.Appearance.ForeColor = Color.Blue;
            }
        }
        private void bandedGridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            if (this.gridControl1.DataSource == null)
                return;  
            if (e.FocusedColumn == FKID)
            {
                FKIDComboBox.Items.Clear();
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if (myCloumns[index].fk_Table != null && myCloumns[index].fk_Table != "")
                {
                    int index2 = tableNames.IndexOf(myCloumns[index].fk_Table);
                    if (columnNames[index2] == null)
                        needColumns(index2);
                    FKIDComboBox.Items.AddRange(columnNames[index2]);
                }
            }
            else if (e.FocusedColumn == FKData)
            {
                FKDataComboBox.Items.Clear();
                int index = this.bandedGridView1.GetFocusedDataSourceRowIndex();
                if (myCloumns[index].fk_Table != null && myCloumns[index].fk_Table != "")
                {
                    int index2 = tableNames.IndexOf(myCloumns[index].fk_Table);
                    if (columnNames[index2] == null)
                        needColumns(index2);
                    FKDataComboBox.Items.AddRange(columnNames[index2]);
                }
            }
        }
        private void bandedGridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (this.gridControl1.DataSource == null)
                return;         
            FKIDComboBox.Items.Clear();
            FKDataComboBox.Items.Clear();
            int index = e.FocusedRowHandle;
            if (myCloumns[index].fk_Table != null && myCloumns[index].fk_Table != "")
            {
                int index2 = tableNames.IndexOf(myCloumns[index].fk_Table);
                if (columnNames[index2] == null)
                    needColumns(index2);
                FKIDComboBox.Items.AddRange(columnNames[index2]);
                FKDataComboBox.Items.AddRange(columnNames[index2]);
            }
        }
        private void WelcomePage_check()
        {
            if (this.listBoxControl2.Items == null || this.listBoxControl2.Items.Count==0)
                this.welcomeWizardPage1.AllowNext = false;
            else
                this.welcomeWizardPage1.AllowNext = true;
        }

        private void page2Putin_check()
        {
            bool result = true;
            foreach(LayoutControlItem item in this.layoutControlGroup1.Items)
            {
                if(item.Control.Text==""||item.Control.Text==null)
                {
                    result = false;
                    break;
                }
            }
            if (result)
                page2PutinComplete = true;
            else
                page2PutinComplete = false;
            this.page2_check();
        }
        private void page2_check()
        {
            if (this.initDynamicComplete && page2PutinComplete)
                this.wizardPage2.AllowNext = true;
            else
                this.wizardPage2.AllowNext = false;
        }
        private void textEdit6_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            this.page2Putin_check();          
        }

        private void textEdit5_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            this.page2Putin_check();         
        }

        private void textEdit4_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            this.page2Putin_check();         
        }

        private void textEdit3_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            this.page2Putin_check();         
        }

        private void textEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            this.page2Putin_check();         
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Guid myGuid = new Guid();
            myGuid = Guid.NewGuid();
            this.textEdit6.Text = myGuid.ToString();
            this.page2Putin_check();
        }
        private class threadstate
        {
            public CodeString data;
            public int number;
            public threadstate(CodeString d,int i)
            {
                this.data = d;
                this.number = i;
            }
        }
        
        private void wizardPage1_PageCommit(object sender, EventArgs e)
        {
            if (this.Thread_initDynamicString.IsBusy)
                this.Thread_initDynamicString.CancelAsync();
            this.initDynamicComplete = false;
            this.Thread_initDynamicString.RunWorkerAsync();
        }

        private void wizardPage2_PageCommit(object sender, EventArgs e)
        {
            this.completeFullString();
            this.wizardControl1.CancelText = "完成";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
        }

        private void completionWizardPage1_PageCommit(object sender, EventArgs e)
        {
            if(myBrowser.ShowDialog()==DialogResult.OK)
            {
                bool con = true;
                string path = myBrowser.SelectedPath + "\\";
                if (File.Exists(path + this.textEdit7.Text + ".cs") || File.Exists(path + this.textEdit7.Text + ".Designer.cs") || File.Exists(path + this.textEdit7.Text + ".resx"))
                {
                    if (MessageBox.Show(this, "目标路径已有同名文件，是否要覆盖？", "注意", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        con = false;
                    }
                }
                if (con)
                {
                    progressPanel1.Visible = true;
                    this.Thread_generateFile.RunWorkerAsync();
                }
                else
                    MessageBox.Show("代码生成已取消");
            }
        }

        private void completionWizardPage1_PageRollback(object sender, EventArgs e)
        {
            this.wizardControl1.CancelText = "取消";
        }
        private void generateFile()
        {
            string path = myBrowser.SelectedPath + "\\";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                File.Copy("Source\\Source.cs", path + this.textEdit7.Text + ".cs", true);
                FileStream fs = new FileStream(path + this.textEdit7.Text + ".cs", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(this.memoEdit1.Text);
                sw.Close();
                fs.Close();
                File.Copy("Source\\Source.Designer.cs", path + this.textEdit7.Text + ".Designer.cs", true);
                fs = new FileStream(path + this.textEdit7.Text + ".Designer.cs", FileMode.Append);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(this.memoEdit2.Text);
                sw.Close();
                fs.Close();
                File.Copy("Source\\Source.resx", path + this.textEdit7.Text + ".resx", true);
                MessageBox.Show("代码生成成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("代码生成失败," + ex.Message);
            }
        }
        private void generateFileFinished()
        {
            this.progressPanel1.Visible = false;
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (this.listBoxControl1.Items != null)
            {
                this.listBoxControl2.Items.Clear();
                foreach(var item in this.listBoxControl1.Items)
                    this.listBoxControl2.Items.Add(item);

                this.WelcomePage_check();
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            this.listBoxControl2.Items.Clear();
            this.WelcomePage_check();
        }

        private void createPersonComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetInfoColumn((sender as ComboBoxEdit).SelectedItem as columnInfo, 1);
            this.CheckInfoColumn();
        }

        private void createTimeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetInfoColumn((sender as ComboBoxEdit).SelectedItem as columnInfo, 0);
            this.CheckInfoColumn();
        }

        private void alterTimeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetInfoColumn((sender as ComboBoxEdit).SelectedItem as columnInfo, 2);
            this.CheckInfoColumn();
        }

        private void alterPersonComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetInfoColumn((sender as ComboBoxEdit).SelectedItem as columnInfo, 3);
            this.CheckInfoColumn();
        }
        private void CheckInfoColumn()
        {
            int c = 0;
            foreach(var temp in InfoColumnComboBox)
            {
                if (temp.SelectedIndex == 0)
                    c++;
            }
            int a = infoCloumns.Distinct().Count();
            int b = infoCloumns.Count();
            if ((c == 0 &&infoCloumns.Distinct().Count()==infoCloumns.Count()) || c == 4)
                this.page1Check = this.page1Check & (this.page1Check ^ 4);
            else
                this.page1Check = this.page1Check | 4;
        }
        private void SetInfoColumn(columnInfo target,int infotype)
        {
            if (infotype < 0 || infotype > 3)
                return;
            if (target != null)
            {
                target.IsInfoColumn = true;
                if (target.edit_Allow)
                    target.edit_Allow = false;
                target.InfoType = infotype;
                if (target.column_Name == null || target.column_Name.Trim() == "" || target.column_Name == "记录创建时刻" || target.column_Name == "记录创建责任者" || target.column_Name == "记录修改时刻" || target.column_Name == "记录修改责任者")
                {
                    switch(infotype)
                    {
                        case 0: target.column_Name = "记录创建时刻"; break;
                        case 1: target.column_Name = "记录创建责任者"; break;
                        case 2: target.column_Name = "记录修改时刻"; break;
                        case 3: target.column_Name = "记录修改责任者"; break;
                    }
                }
            }
            if (this.infoCloumns[infotype] != null)
            {
                if (this.infoCloumns[infotype].column_Name == "记录创建时刻" || this.infoCloumns[infotype].column_Name == "记录创建责任者" || this.infoCloumns[infotype].column_Name == "记录修改时刻" || this.infoCloumns[infotype].column_Name == "记录修改责任者")
                    this.infoCloumns[infotype].column_Name = null;
                this.infoCloumns[infotype].IsInfoColumn = false;
                if (!this.infoCloumns[infotype].edit_Allow)
                    this.infoCloumns[infotype].edit_Allow = true;
                this.infoCloumns[infotype].InfoType = -1;
            }
            this.infoCloumns[infotype] = target;
            this.bandedGridView1.RefreshData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(this.checkBox1.Checked==true)
            {
                this.comboBoxEdit2.Enabled = true;
            }
            else
            {
                this.comboBoxEdit2.Enabled = false;
            }
        }

        private void textEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var temp = (textEdit1.SelectedItem as ConnectInfoStringItem);
            if (temp != null && temp.type != null && temp.type.ToString().Trim() != "")
            {
                this.comboBoxEdit2.SelectedItem = temp;
                this.checkBox1.Checked = true;
            }
            else
                this.checkBox1.Checked = false;
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textEdit1.SelectedItem = (comboBoxEdit2.SelectedItem as ConnectInfoTypeItem).str;
        }

    }
    public class ConnectInfoStringItem
    {
        public string str = "";
        public ConnectInfoTypeItem type;
        public ConnectInfoStringItem(string s)
        {
            this.str = s;
        }
        public override string ToString()
        {
            return this.str;
        }
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
    public class ConnectInfoTypeItem
    {
        public string type = "";
        public ConnectInfoStringItem str;
        public ConnectInfoTypeItem(string s)
        {
            this.type = s;
        }
        public override string ToString()
        {
            return this.type;
        }
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj.ToString());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
    public class CodeString
    {
        public string type = "";
        public string[] strings;
        public CodeString(string ty,int i)
        {
            this.type = ty;
            this.strings = new string[i];
        }
    }
}

