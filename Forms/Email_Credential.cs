using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace EMBACore.Forms
{
    /// <summary>
    /// 處理當要發送Email 時，使用者要輸入 SMTP 的認證資訊。
    /// </summary>
    public partial class Email_Credential : BaseForm
    {
        internal delegate void InputCredentialHandler(object sender, EmailCredentialEventArgs args) ;
        internal event InputCredentialHandler AfterInputCredential ;

        public Email_Credential()
        {
            InitializeComponent();
        }

        private void Email_Credential_Load(object sender, EventArgs e)
        {
            UDT.AbsenceConfiguration conf = UDT.AbsenceConfiguration.GetEmailSenderInfo();
            this.txtUserID.Text = conf.Content;
            toolTip1.SetToolTip(this.txtCC, "如有多位收件者，請以『,』分隔！");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool isOk = true ;

            //Validate UserID
            if (string.IsNullOrWhiteSpace(this.txtUserID.Text))
            {
                this.errorProvider1.SetError(this.txtUserID, "請輸入帳號");
               isOk = false  ;
            }
            else
                this.errorProvider1.SetError(this.txtUserID, "");

            //Validate Password
            if (string.IsNullOrWhiteSpace(this.txtPassword.Text))
            {
                if (MessageBox.Show("確定密碼是空白嗎？","注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes) {
                    isOk = false ;
                }
            }


            if (isOk)
            {
                if (this.AfterInputCredential != null)
                {
                    EmailCredentialEventArgs arg = new EmailCredentialEventArgs(this.txtUserID.Text, this.txtPassword.Text, this.txtCC.Text);
                    this.AfterInputCredential(this, arg);
                }
                this.Close();
            }
        }
    }

    internal class EmailCredentialEventArgs : EventArgs
    {
        public EmailCredentialEventArgs(string userID, string pwd, string cc)
            : base()
        {
            this.UserID = userID;
            this.Password = pwd ;
            this.CC = cc;
        }

        public string UserID { get; private set; }
        public string Password { get; private set; }
        public string CC { get; private set; }
    }
}
