namespace EMBACore.Views
{
    partial class Student_GradeClassView
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.ATree = new DevComponents.AdvTree.AdvTree();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            ((System.ComponentModel.ISupportInitialize)(this.ATree)).BeginInit();
            this.SuspendLayout();
            // 
            // advTree1
            // 
            this.ATree.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.ATree.AllowDrop = true;
            this.ATree.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.ATree.BackgroundStyle.Class = "TreeBorderKey";
            this.ATree.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ATree.Columns.Add(this.columnHeader1);
            this.ATree.ColumnsVisible = false;
            this.ATree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ATree.DragDropEnabled = false;
            this.ATree.ExpandWidth = 16;
            this.ATree.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.ATree.Location = new System.Drawing.Point(0, 0);
            this.ATree.Name = "advTree1";
            this.ATree.NodesConnector = this.nodeConnector1;
            this.ATree.NodeStyle = this.elementStyle1;
            this.ATree.PathSeparator = ";";
            this.ATree.Size = new System.Drawing.Size(338, 514);
            this.ATree.Styles.Add(this.elementStyle1);
            this.ATree.TabIndex = 1;
            this.ATree.Text = "advTree1";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Width.Relative = 100;
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Class = "";
            this.elementStyle1.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // StudentGradeClassView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.ATree);
            this.Name = "StudentGradeClassView";
            this.Size = new System.Drawing.Size(338, 514);
            ((System.ComponentModel.ISupportInitialize)(this.ATree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree ATree;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
    }
}
