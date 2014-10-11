namespace PUB.STCT.Client
{
    partial class FilterItem
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterItem));
            this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.comboBoxEdit2 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.comboBoxEdit4 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.ValueEditGroup = new DevExpress.XtraEditors.GroupControl();
            this.DistinctEdit = new DevExpress.XtraEditors.ComboBoxEdit();
            this.TextEdit = new DevExpress.XtraEditors.TextEdit();
            this.DateTimeEdit = new DevExpress.XtraEditors.DateEdit();
            this.FKEdit = new PUB.STCT.Client.DataComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueEditGroup)).BeginInit();
            this.ValueEditGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DistinctEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTimeEdit.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTimeEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FKEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxEdit1
            // 
            this.comboBoxEdit1.Location = new System.Drawing.Point(33, 4);
            this.comboBoxEdit1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxEdit1.Name = "comboBoxEdit1";
            this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.comboBoxEdit1.Size = new System.Drawing.Size(134, 22);
            this.comboBoxEdit1.TabIndex = 1;
            this.comboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit1_SelectedIndexChanged);
            // 
            // comboBoxEdit2
            // 
            this.comboBoxEdit2.Location = new System.Drawing.Point(173, 4);
            this.comboBoxEdit2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxEdit2.Name = "comboBoxEdit2";
            this.comboBoxEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit2.Properties.Items.AddRange(new object[] {
            "=",
            ">=",
            ">",
            "<",
            "<=",
            "<>",
            "like",
            "in",
            "is null",
            "is not null"});
            this.comboBoxEdit2.Properties.NullText = "=";
            this.comboBoxEdit2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.comboBoxEdit2.Size = new System.Drawing.Size(86, 22);
            this.comboBoxEdit2.TabIndex = 2;
            this.comboBoxEdit2.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit2_SelectedIndexChanged);
            // 
            // comboBoxEdit4
            // 
            this.comboBoxEdit4.Location = new System.Drawing.Point(405, 4);
            this.comboBoxEdit4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxEdit4.Name = "comboBoxEdit4";
            this.comboBoxEdit4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit4.Properties.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.comboBoxEdit4.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.comboBoxEdit4.Size = new System.Drawing.Size(49, 22);
            this.comboBoxEdit4.TabIndex = 4;
            this.comboBoxEdit4.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit4_SelectedIndexChanged);
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Location = new System.Drawing.Point(5, 4);
            this.pictureEdit1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Size = new System.Drawing.Size(22, 22);
            this.pictureEdit1.TabIndex = 6;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.InsertGalleryImage("apply_32x32.png", "images/actions/apply_32x32.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/apply_32x32.png"), 0);
            this.imageCollection1.Images.SetKeyName(0, "apply_32x32.png");
            this.imageCollection1.InsertGalleryImage("cancel_32x32.png", "images/actions/cancel_32x32.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/cancel_32x32.png"), 1);
            this.imageCollection1.Images.SetKeyName(1, "cancel_32x32.png");
            // 
            // ValueEditGroup
            // 
            this.ValueEditGroup.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.ValueEditGroup.Controls.Add(this.DistinctEdit);
            this.ValueEditGroup.Controls.Add(this.TextEdit);
            this.ValueEditGroup.Controls.Add(this.DateTimeEdit);
            this.ValueEditGroup.Controls.Add(this.FKEdit);
            this.ValueEditGroup.Location = new System.Drawing.Point(265, 4);
            this.ValueEditGroup.Name = "ValueEditGroup";
            this.ValueEditGroup.ShowCaption = false;
            this.ValueEditGroup.Size = new System.Drawing.Size(134, 22);
            this.ValueEditGroup.TabIndex = 10;
            this.ValueEditGroup.Text = "groupControl1";
            // 
            // DistinctEdit
            // 
            this.DistinctEdit.Location = new System.Drawing.Point(0, 0);
            this.DistinctEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DistinctEdit.Name = "DistinctEdit";
            this.DistinctEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DistinctEdit.Size = new System.Drawing.Size(134, 22);
            this.DistinctEdit.TabIndex = 13;
            // 
            // TextEdit
            // 
            this.TextEdit.Location = new System.Drawing.Point(0, 0);
            this.TextEdit.Name = "TextEdit";
            this.TextEdit.Size = new System.Drawing.Size(134, 22);
            this.TextEdit.TabIndex = 12;
            // 
            // DateTimeEdit
            // 
            this.DateTimeEdit.EditValue = null;
            this.DateTimeEdit.Location = new System.Drawing.Point(0, 0);
            this.DateTimeEdit.Name = "DateTimeEdit";
            this.DateTimeEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateTimeEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DateTimeEdit.Properties.Mask.EditMask = "";
            this.DateTimeEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.DateTimeEdit.Size = new System.Drawing.Size(134, 22);
            this.DateTimeEdit.TabIndex = 11;
            // 
            // FKEdit
            // 
            this.FKEdit.Location = new System.Drawing.Point(0, 0);
            this.FKEdit.Name = "FKEdit";
            this.FKEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.FKEdit.Size = new System.Drawing.Size(134, 22);
            this.FKEdit.TabIndex = 10;
            this.FKEdit.Type = PUB.STCT.Client.DataComboBoxEdit.DataComboBoxType.Advance;
            // 
            // FilterItem
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ValueEditGroup);
            this.Controls.Add(this.pictureEdit1);
            this.Controls.Add(this.comboBoxEdit4);
            this.Controls.Add(this.comboBoxEdit2);
            this.Controls.Add(this.comboBoxEdit1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(460, 30);
            this.MinimumSize = new System.Drawing.Size(460, 30);
            this.Name = "FilterItem";
            this.Size = new System.Drawing.Size(460, 30);
            this.Enter += new System.EventHandler(this.FilterItem_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValueEditGroup)).EndInit();
            this.ValueEditGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DistinctEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TextEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTimeEdit.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DateTimeEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FKEdit.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit2;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit4;
        private DevExpress.XtraEditors.GroupControl ValueEditGroup;
        private DevExpress.XtraEditors.ComboBoxEdit DistinctEdit;
        private DevExpress.XtraEditors.TextEdit TextEdit;
        private DevExpress.XtraEditors.DateEdit DateTimeEdit;
        private DataComboBoxEdit FKEdit;

    }
}
