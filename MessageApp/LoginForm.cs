using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MessageApp
{
    public partial class LoginForm : Form
    {
        private MessageAppForm messageAppForm;
        private LoginHandler loginHandler;
        public LoginForm()
        {
            InitializeComponent();
            AcceptButton = btnLogin;
        }
        #region Events

        private void btnLogin_Click(object sender, EventArgs e)
        {
            OpenMessageAppForm();
            throw new ExecutionEngineException();
        }


        #endregion

        private void OpenMessageAppForm()
        {
            loginHandler = new LoginHandler();
            loginHandler.Login(txtUsername.Text,txtPassword.Text,txtBoxServerIP.Text);
            messageAppForm = new MessageAppForm();
            AddOwnedForm(messageAppForm);
            messageAppForm.Owner = this;
            this.Hide();
            messageAppForm.ShowDialog();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form f in OwnedForms)
                f.Close();
        }
    }
}
