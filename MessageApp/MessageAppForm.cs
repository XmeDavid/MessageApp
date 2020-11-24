using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MessageApp
{
    public partial class MessageAppForm : Form
    {
        private MessageHandler messageHandler;
        public MessageAppForm()
        {
            InitializeComponent();
            AcceptButton = btnPublish;
            messageHandler = new MessageHandler(this);
        }

        private void btnPublish_Click(object sender, EventArgs e)
        {
            messageHandler.Publish(messageHandler.GetTokenFromUsername(GetSelectedUser()),txtBoxMessage.Text);
        }

        public void UpdateMessage(FormattedMessage message)
        {
            Invoke((MethodInvoker)delegate {
                listBoxMessages.Items.Add(message);
            });
        }

        public void UpdateUsers(string[] users)
        {
            Invoke((MethodInvoker)delegate {
                Random rand = new Random();
                listBoxUsers.Items.Clear();
                foreach (string s in users)
                {
                    if (listBoxUsers.Items.Contains(s))
                    {
                        listBoxUsers.Items.Add(s + "#" + rand.Next(1000));
                        continue;
                    }
                    listBoxUsers.Items.Add(s);
                }
            });
        }

        public string GetSelectedUser()
        {
            try
            {
                if(listBoxUsers.SelectedItem != null)
                    return listBoxUsers.SelectedItem.ToString();
                throw new ArgumentNullException("No user selected - Bouncing the message...");
            }
            catch(Exception e)
            {
                Message(e.Message);
                return listBoxUsers.Items[0].ToString();
            }
        }

        public void Message(string s)
        {
            MessageBox.Show(s);
        }
    }
}
