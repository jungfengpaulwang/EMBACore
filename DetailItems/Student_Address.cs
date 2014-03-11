using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using K12.Data;
using FISCA.Presentation;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using Campus.Windows;
using Campus;
using FISCA.Permission;
using Campus.Configuration;
using System.Threading.Tasks;

namespace EMBACore.DetailItems
{
    [FCode("ischool.EMBA.Student.Detail0070", "通訊資料")]
    internal partial class Student_Address : FISCA.Presentation.DetailContent
    {
        private AddressRecord _StudAddressRec;
        private ChangeListen _DataListener_Permanent;
        private ChangeListen _DataListener_Mailing;
        private ChangeListen _DataListener_Company;
        private ChangeListen _DataListener_Other;
        private AddressType _address_type;

        private dynamic CountyTownList;
        private Task CountyTownDownloadTask = null;

        private enum AddressType
        {
            Permanent,
            Mailing,
            Company,
            Other
        }

        private ErrorTip _errors;
        private ErrorTip _warnings;

        private BackgroundWorker _getCountyBackgroundWorker;
        private bool isBGBusy = false;
        private BackgroundWorker BGWorker;

        //Town -> ZipCode
        private Dictionary<string, string> _zip_code_mapping = new Dictionary<string, string>();

        private bool _isInitialized = false;
        public Student_Address()
        {
            InitializeComponent();
            Group = "地址資料";
        }

        void AddressPalmerwormItem_Disposed(object sender, EventArgs e)
        {
            Address.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate);
        }

        void JHAddress_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!BGWorker.IsBusy)
                        BGWorker.RunWorkerAsync();
                }
            }
        }

        void _DataListener_Other_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _DataListener_Mailing_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _DataListener_Permanent_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _DataListener_Company_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            //// 當輸入沒有 鄰自動補
            //if (!string.IsNullOrEmpty(txtArea.Text))
            //{
            //    if (txtArea.Text.IndexOf("鄰") == -1)
            //        txtArea.Text += "鄰";
            //}

            // 檢查畫面儲存相對應
            if (_address_type == AddressType.Permanent)
            {
                _StudAddressRec.Permanent.ZipCode = txtZipcode.Text;
                _StudAddressRec.Permanent.County = cboCounty.Text;
                _StudAddressRec.Permanent.Town = cboTown.Text;
                _StudAddressRec.Permanent.District = txtDistrict.Text;
                _StudAddressRec.Permanent.Area = txtArea.Text;
                _StudAddressRec.Permanent.Detail = txtDetail.Text;
                _StudAddressRec.Permanent.Longitude = txtLongtitude.Text;
                _StudAddressRec.Permanent.Latitude = txtLatitude.Text;

            }

            if (_address_type == AddressType.Mailing)
            {
                _StudAddressRec.Mailing.ZipCode = txtZipcode.Text;
                _StudAddressRec.Mailing.County = cboCounty.Text;
                _StudAddressRec.Mailing.Town = cboTown.Text;
                _StudAddressRec.Mailing.District = txtDistrict.Text;
                _StudAddressRec.Mailing.Area = txtArea.Text;
                _StudAddressRec.Mailing.Detail = txtDetail.Text;
                _StudAddressRec.Mailing.Longitude = txtLongtitude.Text;
                _StudAddressRec.Mailing.Latitude = txtLatitude.Text;

            }

            if (_address_type == AddressType.Other)
            {
                _StudAddressRec.Address1.ZipCode = txtZipcode.Text;
                _StudAddressRec.Address1.County = cboCounty.Text;
                _StudAddressRec.Address1.Town = cboTown.Text;
                _StudAddressRec.Address1.District = txtDistrict.Text;
                _StudAddressRec.Address1.Area = txtArea.Text;
                _StudAddressRec.Address1.Detail = txtDetail.Text;
                _StudAddressRec.Address1.Longitude = txtLongtitude.Text;
                _StudAddressRec.Address1.Latitude = txtLatitude.Text;
            }
            _errors.Clear();
            Address.Update(_StudAddressRec);
            StudentRecord studRec = Student.SelectByID(PrimaryKey);
            BindDataToForm();
        }

        // 當讀取學生資訊完成時
        void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBGBusy)
            {
                isBGBusy = false;
                BGWorker.RunWorkerAsync();
                return;
            }
            Initialize();
            BindDataToForm();

        }

        // 透過DAL讀取學生地址資訊
        void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudAddressRec = Address.SelectByStudentID(PrimaryKey);
            if (CountyTownDownloadTask != null && !CountyTownDownloadTask.IsCompleted)
                CountyTownDownloadTask.Wait();
        }

        // 取得地址對應中文名稱
        private string GetAddressTypeTitle()
        {
            if (_address_type == AddressType.Permanent)
                return "住家地址";  //原 戶籍地址
            else if (_address_type == AddressType.Mailing)
                return "聯絡地址"; //原 通訊地址
            else if (_address_type == AddressType.Company)
                return "公司地址";
            else if (_address_type == AddressType.Other)
                return "公司地址"; //原 其它地址
            else
                return string.Empty;
        }

        // 填入地址資訊到畫面
        public void BindDataToForm()
        {
            DataListenerPause();

            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;

            // 判斷不同 Bind 到相對應
            if (_address_type == AddressType.Permanent)
            {
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;
                _DataListener_Permanent.Reset();
                _DataListener_Permanent.ResumeListen();
            }

            if (_address_type == AddressType.Mailing)
            {
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;
                _DataListener_Mailing.Reset();
                _DataListener_Mailing.ResumeListen();
            }

            if (_address_type == AddressType.Other)
            {
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;
                _DataListener_Other.Reset();
                _DataListener_Other.ResumeListen();
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (BGWorker.IsBusy)
                isBGBusy = true;
            else
                BGWorker.RunWorkerAsync();
        }

        public DetailContent GetContent()
        {
            return new Student_Address();
        }

        void _getCountyBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> result = new List<string>();

            if (CountyTownDownloadTask != null && !CountyTownDownloadTask.IsCompleted)
                CountyTownDownloadTask.Wait();

            HashSet<string> counties = new HashSet<string>();
            foreach (dynamic each in CountyTownList.Town.Each())
                counties.Add(each["@County"]);

            e.Result = new List<string>(counties);
        }

        void _getCountyBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<string> countyList = e.Result as List<string>;
            //countyList.Sort();
            foreach (string county in countyList)
                cboCounty.AddItem(county);
        }

        private void Initialize()
        {
            _errors.Clear();
            _warnings.Clear();

            if (_isInitialized) return;

            _isInitialized = true;
        }

        private void cboCounty_TextChanged(object sender, EventArgs e)
        {
            cboTown.SelectedItem = null;
            cboTown.Items.Clear();
            if (cboCounty.GetText() != "")
            {
                List<dynamic> townList = GetTownList(cboCounty.Text);// Framework.Feature.Config.GetTownList(cboCounty.Text);
                _zip_code_mapping = new Dictionary<string, string>();
                foreach (dynamic each in townList)
                {
                    string name = each["@Name"];

                    if (!_zip_code_mapping.ContainsKey(name))
                        _zip_code_mapping.Add(name, each["@Code"]);

                    cboTown.AddItem(name);
                }
            }

            ShowFullAddress();
        }

        private void cboTown_TextChanged(object sender, EventArgs e)
        {
            if (_date_updating) return;
            CheckTownChange();
        }

        private void CheckTownChange()
        {
            string value = cboTown.GetText();
            if (!string.IsNullOrEmpty(value))
            {
                if (_zip_code_mapping.ContainsKey(value))
                    txtZipcode.Text = _zip_code_mapping[value];
            }
            ShowFullAddress();
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void ShowFullAddress()
        {
            string fullAddress = "";
            if (txtZipcode.Text != "")
                fullAddress += "[" + txtZipcode.Text + "]";
            fullAddress += cboCounty.GetText();
            fullAddress += cboTown.GetText();
            fullAddress += txtDistrict.Text;
            fullAddress += txtArea.Text;
            fullAddress += txtDetail.Text;
            this.lblFullAddress.Text = fullAddress;
        }

        private void txtLongtitude_TextChanged(object sender, EventArgs e)
        {
            decimal d;
            if (!string.IsNullOrEmpty(txtLongtitude.Text) && !decimal.TryParse(txtLongtitude.Text, out d))
            {
                _errors.SetError(txtLongtitude, "經度必須為數字形態。");
                return;
            }
            else
                _errors.SetError(txtLongtitude, string.Empty);
        }

        private void txtLatitude_TextChanged(object sender, EventArgs e)
        {
            decimal d;
            if (!string.IsNullOrEmpty(txtLatitude.Text) && !decimal.TryParse(txtLatitude.Text, out d))
            {
                _errors.SetError(txtLatitude, "緯度必須為數字形態。");
                return;
            }
            else
                _errors.SetError(txtLatitude, string.Empty);
        }

        private void btnPAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("資料錯誤，請修正資料");
                return;
            }

            _address_type = AddressType.Permanent;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Permanent.Reset();
            _DataListener_Permanent.ResumeListen();
        }

        private void DataListenerPause()
        {
            _DataListener_Other.SuspendListen();
            _DataListener_Mailing.SuspendListen();
            _DataListener_Permanent.SuspendListen();
        }

        private void btnFAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("資料錯誤，請修正資料");
                return;
            }

            _address_type = AddressType.Mailing;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Mailing.Reset();
            _DataListener_Mailing.ResumeListen();
        }

        private void btnOAddress_Click(object sender, EventArgs e)
        {
            if (_errors.HasError)
            {
                MsgBox.Show("資料錯誤，請修正資料");
                return;
            }

            _address_type = AddressType.Other;
            DataListenerPause();
            DisplayAddress(GetCurrentAddress());
            _DataListener_Other.Reset();
            _DataListener_Other.ResumeListen();
        }

        private void txtZipcode_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void txtZipcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (_date_updating) return;

            if (e.KeyCode == Keys.Enter)
                CheckZipCode();
        }

        private void txtZipcode_Validated(object sender, EventArgs e)
        {
            if (_date_updating) return;

            if (!_errors.ContainError(txtZipcode))
                CheckZipCode();
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            EMBACore.Forms.MapForm.ShowMap(txtLatitude.Text, txtLongtitude.Text, txtDetail.Text);
        }

        private void CheckZipCode()
        {
            dynamic ctPair = FindTownByZipCode(txtZipcode.Text);
            if (ctPair["@Code"] == string.Empty)
                _warnings.SetError(txtZipcode, "查無此郵遞區號對應縣市鄉鎮資料。");
            else
            {
                _warnings.SetError(txtZipcode, string.Empty);

                string county = ctPair["@County"];
                string town = ctPair["@Name"];

                cboCounty.SetComboBoxText(county);
                cboTown.SetComboBoxText(town);
            }
        }

        private bool IsValid()
        {
            return !_errors.HasError;
        }

        private K12.Data.AddressItem GetCurrentAddress()
        {
            if (_address_type == AddressType.Permanent)
                return _StudAddressRec.Permanent;

            else if (_address_type == AddressType.Mailing)
                return _StudAddressRec.Mailing;

            else if (_address_type == AddressType.Other)
                return _StudAddressRec.Address1;
            else
                throw new ArgumentException("沒有此種 Address Type。");
        }

        private bool _date_updating = false;
        private void DisplayAddress(K12.Data.AddressItem addr)
        {
            _date_updating = true;
            btnAddressType.Text = GetAddressTypeTitle();
            if (btnAddressType.Text == "住家地址")
            {
                lnklblAddress1.Text = "複製聯絡地址";
                lnklblAddress2.Text = "複製公司地址";
            }

            if (btnAddressType.Text == "聯絡地址")
            {
                lnklblAddress1.Text = "複製住家地址";
                lnklblAddress2.Text = "複製公司地址";
            }

            if (btnAddressType.Text == "公司地址")
            {
                lnklblAddress1.Text = "複製住家地址";
                lnklblAddress2.Text = "複製聯絡地址";
            }
            cboCounty.SetComboBoxText(addr.County);
            cboTown.SetComboBoxText(addr.Town);
            txtDistrict.Text = addr.District;
            txtArea.Text = addr.Area;
            txtDetail.Text = addr.Detail;
            txtLongtitude.Text = addr.Longitude;
            txtLatitude.Text = addr.Latitude;
            txtZipcode.Text = addr.ZipCode;

            _date_updating = false;
        }



        private void btnQueryPoint_Click(object sender, EventArgs e)
        {
            try
            {
                DSXmlHelper h = new DSXmlHelper("Request");
                string address = cboCounty.GetText() + cboTown.GetText() + txtDistrict.Text + txtArea.Text + txtDetail.Text;
                h.AddText(".", address);

                DSResponse rsp = FISCA.Authentication.DSAServices.CallService("SmartSchool.Common.QueryCoordinates", new DSRequest(h));
                h = rsp.GetContent();
                if (h.GetElement("Error") != null)
                    MsgBox.Show("無法查詢此地址座標相關資訊");
                else
                {
                    string latitude = h.GetText("Latitude");
                    string longitude = h.GetText("Longitude");

                    if (!string.IsNullOrEmpty(txtLatitude.Text) || !string.IsNullOrEmpty(txtLongtitude.Text))
                    {
                        string msg = "已查詢出此地址座標為：\n\n(" + longitude + "," + latitude + ")\n\n要取代目前座標設定嗎？";
                        if (MsgBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            txtLatitude.Text = latitude;
                            txtLongtitude.Text = longitude;
                        }
                    }
                    else
                    {
                        txtLatitude.Text = latitude;
                        txtLongtitude.Text = longitude;
                    }
                }
            }
            catch (Exception)
            {
                MsgBox.Show("查詢座標資訊錯誤。");
                //Diagnostic.FeedbackError(ex, "地址資料項目的查詢座標資訊。");
            }
        }

        private void txtDistrict_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            ShowFullAddress();
        }

        private void lnklblAddress1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string sourceAddressType = this.lnklblAddress1.Text.Replace("複製", "");

            //if (btnAddressType.Text == "聯絡地址")
            if (sourceAddressType == "聯絡地址")
            {
                //複製聯絡地址
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;
            }

            if (sourceAddressType == "住家地址")
            {

                // 複製戶籍地址
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;

            }

            if (sourceAddressType == "公司地址")
            {
                // 複製公司地址
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;
            }

        }

        private void lnklblAddress2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string sourceAddressType = this.lnklblAddress2.Text.Replace("複製", "");

            //if (btnAddressType.Text == "聯絡地址")
            if (sourceAddressType == "聯絡地址")
            {
                //複製聯絡地址
                txtZipcode.Text = _StudAddressRec.Mailing.ZipCode;
                cboCounty.Text = _StudAddressRec.Mailing.County;
                cboTown.Text = _StudAddressRec.Mailing.Town;
                txtDistrict.Text = _StudAddressRec.Mailing.District;
                txtArea.Text = _StudAddressRec.Mailing.Area;
                txtDetail.Text = _StudAddressRec.Mailing.Detail;
                txtLongtitude.Text = _StudAddressRec.Mailing.Longitude;
                txtLatitude.Text = _StudAddressRec.Mailing.Latitude;

            }

            if (sourceAddressType == "公司地址")
            {
                // 複製其它地址
                txtZipcode.Text = _StudAddressRec.Address1.ZipCode;
                cboCounty.Text = _StudAddressRec.Address1.County;
                cboTown.Text = _StudAddressRec.Address1.Town;
                txtDistrict.Text = _StudAddressRec.Address1.District;
                txtArea.Text = _StudAddressRec.Address1.Area;
                txtDetail.Text = _StudAddressRec.Address1.Detail;
                txtLongtitude.Text = _StudAddressRec.Address1.Longitude;
                txtLatitude.Text = _StudAddressRec.Address1.Latitude;

            }

            if (sourceAddressType == "住家地址")
            {
                // 複製戶籍地址
                txtZipcode.Text = _StudAddressRec.Permanent.ZipCode;
                cboCounty.Text = _StudAddressRec.Permanent.County;
                cboTown.Text = _StudAddressRec.Permanent.Town;
                txtDistrict.Text = _StudAddressRec.Permanent.District;
                txtArea.Text = _StudAddressRec.Permanent.Area;
                txtDetail.Text = _StudAddressRec.Permanent.Detail;
                txtLongtitude.Text = _StudAddressRec.Permanent.Longitude;
                txtLatitude.Text = _StudAddressRec.Permanent.Latitude;
            }
        }

        private void Student_Address_Load(object sender, EventArgs e)
        {
            _errors = new ErrorTip();
            _warnings = new ErrorTip();

            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += new DoWorkEventHandler(BGWorker_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWorker_RunWorkerCompleted);

            _DataListener_Permanent = new ChangeListen();
            _DataListener_Mailing = new ChangeListen();
            _DataListener_Other = new ChangeListen();

            _DataListener_Permanent.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Permanent_StatusChanged);
            _DataListener_Mailing.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Mailing_StatusChanged);
            _DataListener_Other.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Other_StatusChanged);

            CountyTownDownloadTask = Task.Factory.StartNew(() =>
            {
                //XmlElement 可以轉型成 XmlObject。
                CountyTownList = (XmlObject)Config.App["鄉鎮市區列表"].PreviousData;
            });

            // 加入戶籍 Listener Data
            _DataListener_Permanent.Add(new TextBoxSource(txtZipcode));
            _DataListener_Permanent.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Permanent.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Permanent.Add(new TextBoxSource(txtDistrict));
            _DataListener_Permanent.Add(new TextBoxSource(txtArea));
            _DataListener_Permanent.Add(new TextBoxSource(txtDetail));
            _DataListener_Permanent.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Permanent.Add(new TextBoxSource(txtLatitude));

            // 加入通訊 Listener Data
            _DataListener_Mailing.Add(new TextBoxSource(txtZipcode));
            _DataListener_Mailing.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Mailing.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Mailing.Add(new TextBoxSource(txtDistrict));
            _DataListener_Mailing.Add(new TextBoxSource(txtArea));
            _DataListener_Mailing.Add(new TextBoxSource(txtDetail));
            _DataListener_Mailing.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Mailing.Add(new TextBoxSource(txtLatitude));

            // 加入其它 Listener Data
            _DataListener_Other.Add(new TextBoxSource(txtZipcode));
            _DataListener_Other.Add(new ComboBoxSource(cboCounty, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Other.Add(new ComboBoxSource(cboTown, ComboBoxSource.ListenAttribute.Text));
            _DataListener_Other.Add(new TextBoxSource(txtDistrict));
            _DataListener_Other.Add(new TextBoxSource(txtArea));
            _DataListener_Other.Add(new TextBoxSource(txtDetail));
            _DataListener_Other.Add(new TextBoxSource(txtLongtitude));
            _DataListener_Other.Add(new TextBoxSource(txtLatitude));

            Address.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHAddress_AfterUpdate);

            //_address_type = AddressType.Permanent;
            _address_type = AddressType.Mailing;

            _getCountyBackgroundWorker = new BackgroundWorker();
            _getCountyBackgroundWorker.DoWork += new DoWorkEventHandler(_getCountyBackgroundWorker_DoWork);
            _getCountyBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_getCountyBackgroundWorker_RunWorkerCompleted);
            _getCountyBackgroundWorker.RunWorkerAsync();

            if (UserAcl.Current["Content0050"].Editable)
                return;

            if (UserAcl.Current["Content0050"].Viewable)
                this.Enabled = false;

            Disposed += new EventHandler(AddressPalmerwormItem_Disposed);
        }

        private List<dynamic> GetTownList(string county)
        {
            List<dynamic> towns = new List<dynamic>();
            foreach (dynamic town in CountyTownList.Town.Each())
            {
                if (town["@County"] == county)
                    towns.Add(town);
            }
            return towns;
        }

        private dynamic FindTownByZipCode(string zipcode)
        {
            foreach (dynamic town in CountyTownList.Town.Each())
            {
                if (town["@Code"] == zipcode)
                    return town;
            }
            return new XmlObject("Town");
        }

    }
}
