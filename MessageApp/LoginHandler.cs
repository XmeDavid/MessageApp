using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MessageApp
{
    class LoginHandler
    {
        private static string _username;
        private static string _token;
        private static string _server;
        public LoginHandler()
        {

        }
        internal void Login(string username, string password, string server)
        {
            _username = username;
            _token = GetHash(username + "" + password);
            _server = server;
        }

        internal string GetUsername()
        {
            return _username;
        }

        internal string GetToken()
        {
            return _token;
        }

        internal string GetServer()
        {
            return _server;
        }

        public string GetHash(string s)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", String.Empty);
            }
        }
    }
}
