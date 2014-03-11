using EMBACore.Legacy;
namespace EMBACore.DetailItems
{
    partial class Student_Address
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblFullAddress = new System.Windows.Forms.Label();
            this.txtZipcode = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtDetail = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnAddressType = new DevComponents.DotNetBar.ButtonX();
            this.btnFAddress = new DevComponents.DotNetBar.ButtonItem();
            this.btnPAddress = new DevComponents.DotNetBar.ButtonItem();
            this.btnOAddress = new DevComponents.DotNetBar.ButtonItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLongtitude = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtLatitude = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnMap = new DevComponents.DotNetBar.ButtonX();
            this.btnQueryPoint = new DevComponents.DotNetBar.ButtonX();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDistrict = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtArea = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label8 = new System.Windows.Forms.Label();
            this.lnklblAddress1 = new System.Windows.Forms.LinkLabel();
            this.lnklblAddress2 = new System.Windows.Forms.LinkLabel();
            this.cboCounty = new EMBACore.Legacy.ComboBoxAdv();
            this.cboTown = new EMBACore.Legacy.ComboBoxAdv();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "郵遞區號";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 131);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "其　　它";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(218, 73);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "縣市";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(358, 73);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "鄉鎮市區";
            // 
            // lblFullAddress
            // 
            this.lblFullAddress.AutoSize = true;
            this.lblFullAddress.Location = new System.Drawing.Point(191, 13);
            this.lblFullAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFullAddress.Name = "lblFullAddress";
            this.lblFullAddress.Size = new System.Drawing.Size(0, 17);
            this.lblFullAddress.TabIndex = 5;
            // 
            // txtZipcode
            // 
            // 
            // 
            // 
            this.txtZipcode.Border.Class = "TextBoxBorder";
            this.txtZipcode.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtZipcode.Location = new System.Drawing.Point(119, 67);
            this.txtZipcode.Margin = new System.Windows.Forms.Padding(4);
            this.txtZipcode.Name = "txtZipcode";
            this.txtZipcode.Size = new System.Drawing.Size(84, 25);
            this.txtZipcode.TabIndex = 2;
            this.txtZipcode.TextChanged += new System.EventHandler(this.txtZipcode_TextChanged);
            this.txtZipcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtZipcode_KeyDown);
            this.txtZipcode.Validated += new System.EventHandler(this.txtZipcode_Validated);
            // 
            // txtDetail
            // 
            // 
            // 
            // 
            this.txtDetail.Border.Class = "TextBoxBorder";
            this.txtDetail.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDetail.Location = new System.Drawing.Point(119, 129);
            this.txtDetail.Margin = new System.Windows.Forms.Padding(4);
            this.txtDetail.Name = "txtDetail";
            this.txtDetail.Size = new System.Drawing.Size(363, 25);
            this.txtDetail.TabIndex = 7;
            this.txtDetail.TextChanged += new System.EventHandler(this.txtAddress_TextChanged);
            // 
            // btnAddressType
            // 
            this.btnAddressType.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddressType.AutoExpandOnClick = true;
            this.btnAddressType.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAddressType.Location = new System.Drawing.Point(39, 9);
            this.btnAddressType.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddressType.Name = "btnAddressType";
            this.btnAddressType.Size = new System.Drawing.Size(140, 22);
            this.btnAddressType.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.btnFAddress,
            this.btnPAddress,
            this.btnOAddress});
            this.btnAddressType.TabIndex = 1;
            this.btnAddressType.Text = "聯絡地址";
            // 
            // btnFAddress
            // 
            this.btnFAddress.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnFAddress.GlobalItem = false;
            this.btnFAddress.Name = "btnFAddress";
            this.btnFAddress.Text = "聯絡地址";
            this.btnFAddress.Click += new System.EventHandler(this.btnFAddress_Click);
            // 
            // btnPAddress
            // 
            this.btnPAddress.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPAddress.GlobalItem = false;
            this.btnPAddress.Name = "btnPAddress";
            this.btnPAddress.Text = "住家地址";
            this.btnPAddress.Click += new System.EventHandler(this.btnPAddress_Click);
            // 
            // btnOAddress
            // 
            this.btnOAddress.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOAddress.GlobalItem = false;
            this.btnOAddress.Name = "btnOAddress";
            this.btnOAddress.Text = "公司地址";
            this.btnOAddress.Click += new System.EventHandler(this.btnOAddress_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 165);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "經　　度";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(261, 167);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "緯度";
            // 
            // txtLongtitude
            // 
            // 
            // 
            // 
            this.txtLongtitude.Border.Class = "TextBoxBorder";
            this.txtLongtitude.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtLongtitude.Location = new System.Drawing.Point(119, 162);
            this.txtLongtitude.Margin = new System.Windows.Forms.Padding(4);
            this.txtLongtitude.Name = "txtLongtitude";
            this.txtLongtitude.Size = new System.Drawing.Size(130, 25);
            this.txtLongtitude.TabIndex = 8;
            this.txtLongtitude.TextChanged += new System.EventHandler(this.txtLongtitude_TextChanged);
            // 
            // txtLatitude
            // 
            // 
            // 
            // 
            this.txtLatitude.Border.Class = "TextBoxBorder";
            this.txtLatitude.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtLatitude.Location = new System.Drawing.Point(299, 164);
            this.txtLatitude.Margin = new System.Windows.Forms.Padding(4);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.Size = new System.Drawing.Size(130, 25);
            this.txtLatitude.TabIndex = 9;
            this.txtLatitude.TextChanged += new System.EventHandler(this.txtLatitude_TextChanged);
            // 
            // btnMap
            // 
            this.btnMap.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnMap.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnMap.Location = new System.Drawing.Point(445, 165);
            this.btnMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(70, 22);
            this.btnMap.TabIndex = 10;
            this.btnMap.Text = "觀看地圖";
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // btnQueryPoint
            // 
            this.btnQueryPoint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnQueryPoint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnQueryPoint.Location = new System.Drawing.Point(489, 129);
            this.btnQueryPoint.Name = "btnQueryPoint";
            this.btnQueryPoint.Size = new System.Drawing.Size(26, 23);
            this.btnQueryPoint.TabIndex = 13;
            this.btnQueryPoint.Tooltip = "查詢此地址座標";
            this.btnQueryPoint.Click += new System.EventHandler(this.btnQueryPoint_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 104);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "村　　里";
            // 
            // txtDistrict
            // 
            // 
            // 
            // 
            this.txtDistrict.Border.Class = "TextBoxBorder";
            this.txtDistrict.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDistrict.Location = new System.Drawing.Point(119, 100);
            this.txtDistrict.Margin = new System.Windows.Forms.Padding(4);
            this.txtDistrict.Name = "txtDistrict";
            this.txtDistrict.Size = new System.Drawing.Size(130, 25);
            this.txtDistrict.TabIndex = 5;
            this.txtDistrict.TextChanged += new System.EventHandler(this.txtDistrict_TextChanged);
            // 
            // txtArea
            // 
            // 
            // 
            // 
            this.txtArea.Border.Class = "TextBoxBorder";
            this.txtArea.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtArea.Location = new System.Drawing.Point(302, 100);
            this.txtArea.Margin = new System.Windows.Forms.Padding(4);
            this.txtArea.Name = "txtArea";
            this.txtArea.Size = new System.Drawing.Size(213, 25);
            this.txtArea.TabIndex = 6;
            this.txtArea.WatermarkText = "例如:10鄰";
            this.txtArea.TextChanged += new System.EventHandler(this.txtArea_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(277, 104);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 17);
            this.label8.TabIndex = 16;
            this.label8.Text = "鄰";
            // 
            // lnklblAddress1
            // 
            this.lnklblAddress1.AutoSize = true;
            this.lnklblAddress1.Location = new System.Drawing.Point(337, 41);
            this.lnklblAddress1.Name = "lnklblAddress1";
            this.lnklblAddress1.Size = new System.Drawing.Size(86, 17);
            this.lnklblAddress1.TabIndex = 18;
            this.lnklblAddress1.TabStop = true;
            this.lnklblAddress1.Text = "複製住家地址";
            this.lnklblAddress1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblAddress1_LinkClicked);
            // 
            // lnklblAddress2
            // 
            this.lnklblAddress2.AutoSize = true;
            this.lnklblAddress2.Location = new System.Drawing.Point(435, 41);
            this.lnklblAddress2.Name = "lnklblAddress2";
            this.lnklblAddress2.Size = new System.Drawing.Size(86, 17);
            this.lnklblAddress2.TabIndex = 19;
            this.lnklblAddress2.TabStop = true;
            this.lnklblAddress2.Text = "複製公司地址";
            this.lnklblAddress2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblAddress2_LinkClicked);
            // 
            // cboCounty
            // 
            this.cboCounty.DisplayMember = "Text";
            this.cboCounty.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboCounty.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboCounty.FormattingEnabled = true;
            this.cboCounty.ItemHeight = 16;
            this.cboCounty.Location = new System.Drawing.Point(260, 70);
            this.cboCounty.Margin = new System.Windows.Forms.Padding(4);
            this.cboCounty.Name = "cboCounty";
            this.cboCounty.Size = new System.Drawing.Size(84, 22);
            this.cboCounty.TabIndex = 3;
            this.cboCounty.TextChanged += new System.EventHandler(this.cboCounty_TextChanged);
            // 
            // cboTown
            // 
            this.cboTown.DisplayMember = "Text";
            this.cboTown.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTown.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboTown.FormattingEnabled = true;
            this.cboTown.ItemHeight = 16;
            this.cboTown.Location = new System.Drawing.Point(426, 70);
            this.cboTown.Margin = new System.Windows.Forms.Padding(4);
            this.cboTown.Name = "cboTown";
            this.cboTown.Size = new System.Drawing.Size(89, 22);
            this.cboTown.TabIndex = 4;
            this.cboTown.TextChanged += new System.EventHandler(this.cboTown_TextChanged);
            // 
            // Student_Address
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lnklblAddress2);
            this.Controls.Add(this.lnklblAddress1);
            this.Controls.Add(this.txtArea);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDistrict);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnAddressType);
            this.Controls.Add(this.txtLongtitude);
            this.Controls.Add(this.btnQueryPoint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDetail);
            this.Controls.Add(this.cboCounty);
            this.Controls.Add(this.txtLatitude);
            this.Controls.Add(this.btnMap);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboTown);
            this.Controls.Add(this.txtZipcode);
            this.Controls.Add(this.lblFullAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "Student_Address";
            this.Size = new System.Drawing.Size(550, 205);
            this.Load += new System.EventHandler(this.Student_Address_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblFullAddress;
        private DevComponents.DotNetBar.Controls.TextBoxX txtZipcode;
        private EMBACore.Legacy.ComboBoxAdv cboCounty;
        private EMBACore.Legacy.ComboBoxAdv cboTown;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDetail;
        private DevComponents.DotNetBar.ButtonX btnAddressType;
        private DevComponents.DotNetBar.ButtonItem btnPAddress;
        private DevComponents.DotNetBar.ButtonItem btnFAddress;
        private DevComponents.DotNetBar.ButtonItem btnOAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private DevComponents.DotNetBar.Controls.TextBoxX txtLongtitude;
        private DevComponents.DotNetBar.Controls.TextBoxX txtLatitude;
        private DevComponents.DotNetBar.ButtonX btnMap;
        private DevComponents.DotNetBar.ButtonX btnQueryPoint;
        private System.Windows.Forms.Label label6;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDistrict;
        private DevComponents.DotNetBar.Controls.TextBoxX txtArea;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.LinkLabel lnklblAddress1;
        private System.Windows.Forms.LinkLabel lnklblAddress2;
    }
}
