namespace PUB.STCT.Client
{
    partial class DataTextEditPro
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
            this.labelControl = new DevExpress.XtraEditors.LabelControl();
            this.BaseEdit = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.BaseEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl
            // 
            this.labelControl.Location = new System.Drawing.Point(84, 3);
            this.labelControl.Margin = new System.Windows.Forms.Padding(0);
            this.labelControl.Name = "labelControl";
            this.labelControl.Size = new System.Drawing.Size(0, 16);
            this.labelControl.TabIndex = 1;
            // 
            // BaseEdit
            // 
            this.BaseEdit.Location = new System.Drawing.Point(0, 0);
            this.BaseEdit.Margin = new System.Windows.Forms.Padding(0);
            this.BaseEdit.Name = "BaseEdit";
            this.BaseEdit.Size = new System.Drawing.Size(84, 22);
            this.BaseEdit.TabIndex = 0;
            // 
            // DataTextEditPro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelControl);
            this.Controls.Add(this.BaseEdit);
            this.Name = "DataTextEditPro";
            this.Size = new System.Drawing.Size(176, 94);
            ((System.ComponentModel.ISupportInitialize)(this.BaseEdit.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl;
        private DevExpress.XtraEditors.TextEdit BaseEdit;
    }
}
