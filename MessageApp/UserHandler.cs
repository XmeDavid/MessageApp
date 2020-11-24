using System;
using System.Collections.Generic;
using System.Text;

namespace MessageApp
{
    internal class UserHandler
    {
        private static MessageHandler messageHandler;

        public static string RegisterUserTopic { get; set; } = "&rEg1sT3rUseR_";

        private static Dictionary<string, string> users;

        public UserHandler(MessageHandler handler)
        {
            messageHandler = handler;
            if (users == null)
                users = new Dictionary<string, string>();
        }

        public UserHandler( )
        {
            if(users == null)
                users = new Dictionary<string, string>();
        }

        public void RegisterUser(FormattedMessage message)
        {
            if(Add(message.UserToken, message.Message))
            {
                SelfRegister();
                messageHandler.UpdateUsers(GetUsers());
            }
        }

        private bool Add(string key, string value)
        {
            if (!users.ContainsKey(key))
            {
                users.Add(key, value);
                return true;
            }
            return false;
        }

        public void SelfRegister()
        {
            messageHandler.Publish(RegisterUserTopic, new LoginHandler().GetUsername());
        }

        public string GetUsername(string token)
        {
            string username;
            if(users.TryGetValue(token, out username))
                return username;
            return "Unknown user";
        }

        private string[] GetUsers()
        {
            List<string> _users = new List<string>();
            foreach(string s in users.Values)
            {
                _users.Add(s);
            }
            return _users.ToArray();
        }

        public string GetToken(string username)
        {
            string _username;
            foreach (string s  in users.Keys)
            {
                users.TryGetValue(s, out _username);
                if (username == _username)
                {
                    return s;
                }
            }
            return "";
        }
    }
}
