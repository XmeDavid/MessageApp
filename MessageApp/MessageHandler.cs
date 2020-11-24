using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MessageApp
{

    class MessageHandler
    {
        private MqttClient broker;
        private string[] topics = { UserHandler.RegisterUserTopic };
        private byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
        private MessageAppForm form;
        private UserHandler userHandler;

        public MessageHandler(MessageAppForm _form)
        {
            form = _form;
            if (!Connect())
                form.Message("Unable to connect to broker!");
        }

        private bool Connect()
        {
            try
            {
                if (userHandler == null)
                    userHandler = new UserHandler(this);
                LoginHandler login = new LoginHandler();
                broker = new MqttClient(login.GetServer());
                broker.Connect(login.GetToken()); //TODO Make sure token is the correct one
                broker.MqttMsgPublishReceived += Broker_MqttMsgPublishReceived;
                AddTopic(login.GetToken());
                Subscribe();
                userHandler.SelfRegister();
                return true;
            }catch(Exception e)
            {
                form.Message(e.Message);
                form.Close();
                return false;
            }
        }

        private void Subscribe()
        {
            if(IsConnected())
                broker.Subscribe(topics, qosLevels);
        }

        private void UpdateMessage(FormattedMessage message)
        {
            form.UpdateMessage(message);
        }

        private FormattedMessage GetMessage(byte[] msg)
        {
            string json = Encoding.UTF8.GetString(msg);
            return JsonConvert.DeserializeObject<FormattedMessage>(json);
        }

        internal string GetTokenFromUsername(string s)
        {
            return userHandler.GetToken(s);
        }

        public void Publish(string topic, string message)
        {
            try{ 
                if (IsConnected())
                {
                    string _token = new LoginHandler().GetToken();
                    FormattedMessage _message = new FormattedMessage(topic, _token, message, new TimeStamp());
                    if(topic != topics[0] && _token != topic)
                        UpdateMessage(_message);
                    string json = JsonConvert.SerializeObject(_message);
                    broker.Publish(topic,Encoding.UTF8.GetBytes(json));
                }
            }
            catch(uPLibrary.Networking.M2Mqtt.Exceptions.MqttClientException)
            {
                form.Message("Unable to send message");
            }
        }

        internal void UpdateUsers(string[] users)
        {
            form.UpdateUsers(users);
        }

        internal void AddTopic(string newTopic)
        {
            List<string> _topics = new List<string>();
            foreach (string s in topics)
            {
                _topics.Add(s);
            }
            _topics.Add(newTopic);
            List<byte> _qosLevels = new List<byte>();
            foreach (byte b in qosLevels)
            {
                _qosLevels.Add(b);
            }
            _qosLevels.Add(qosLevels[0]);
            topics = _topics.ToArray();
            qosLevels = _qosLevels.ToArray();
            _topics = null;
            _qosLevels = null;
        }

        internal string[] GetTopics()
        {
            return topics;
        }

        private void Broker_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            FormattedMessage message = GetMessage(e.Message);
            if (e.Topic == topics[0])
            {
                userHandler.RegisterUser(message);
            }
            else
            {
                UpdateMessage(message);
            }
        }

        

        public bool IsConnected()
        {
            if(broker != null)
                return broker.IsConnected;
            return false;
        }
    }

    public class FormattedMessage
    {

        public FormattedMessage()
        {
        }

        public FormattedMessage(string topic, string user, string message, TimeStamp time)
        {
            Topic = topic;
            UserToken = user;
            Message = message;
            Time = time;
        }

        public string Topic { get; set; }
        public string UserToken { get; set; }
        public string Message { get; set; }
        public TimeStamp Time { get; set; }

        public override string ToString()
        {
            return $"[{Time}][{new UserHandler().GetUsername(UserToken)}][{new UserHandler().GetUsername(Topic)}] {Message}";
        }
    }

    public class TimeStamp
    {
        private int day;
        private int month;
        private int year;
        private int hour;
        private int minute;
        private int second;

        public TimeStamp()
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            day = DateTime.Now.Day;
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;
            second = DateTime.Now.Second;
        }

        public override string ToString()
        {
            UpdateTime();
            return $"{hour}:{minute}:{second}";
        }
    }
}
