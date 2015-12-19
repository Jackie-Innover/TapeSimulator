using System.Collections.Generic;
using System.Xml.Linq;

// ReSharper disable PossibleNullReferenceException

namespace TapeSimulatorConsole
{
    public sealed class TapeSimulatorSetting
    {
        #region Properties

        public static readonly TapeSimulatorSetting Instance = new TapeSimulatorSetting();
        public string Host { get; }
        public string PortNumber { get; }
        public string UserName { get; }
        public string Password { get; }

        public string ClientGuid { get; }
        public string ClientDisplayName { get; }

        public int SendTimeCount { get; }
        public string VideoFilePath { get; }
        public int ChannelCount { get; }
        public bool IsWriteToDisk { get; }
        public string Uri => $"ws://{Host}:{PortNumber}";

        public List<KeyValuePair<string, string>> Cookies
        {
            get
            {
                List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(WebSocketConst.UserNameKey, UserName),
                    new KeyValuePair<string, string>(WebSocketConst.PasswordKey, Password),
                    new KeyValuePair<string, string>(WebSocketConst.ClientGuidKey, ClientGuid),
                    new KeyValuePair<string, string>(WebSocketConst.ClientHostNameKey, ClientDisplayName),
                    new KeyValuePair<string, string>(WebSocketConst.WebSocketClientVersion, "2")
                };
                return cookies;
            }
        }

        #endregion Properties

        #region Life Time

        private TapeSimulatorSetting()
        {
            var configElement = XElement.Load("TapeSimulatorConfig.xml");
            XElement essRootElement = configElement.Element("ExtendedStorageServerDetails");
            string host = essRootElement.Element("Host").Value;
            string portNumber = essRootElement.Element("Port").Value;
            string userName = essRootElement.Element("UserName").Value;
            string password = essRootElement.Element("Password").Value;
            XElement essClientRootElement = configElement.Element("ExtendedStorageClientDetails");

            string clientGuid = essClientRootElement.Element("ClientGuid").Value;
            string clientDisplayName = essClientRootElement.Element("ClientDisplayName").Value;
            int sendTimeCount = int.Parse(configElement.Element("SendTimesPerMinute").Value);
            string videoFilePath = configElement.Element("VideoFilePath").Value;
            int channelCount;
            if (!int.TryParse(configElement.Element("ChannelCount").Value, out channelCount))
            {
                channelCount = 36;
            }
            bool isWriteToDisk = bool.Parse(configElement.Element("IsWriteToDisk").Value);

            Host = host;
            PortNumber = portNumber;
            UserName = userName;
            Password = password;
            ClientGuid = clientGuid;
            ClientDisplayName = clientDisplayName;

            SendTimeCount = sendTimeCount;
            VideoFilePath = videoFilePath;
            ChannelCount = channelCount;
            IsWriteToDisk = isWriteToDisk;
        }

        #endregion Life Time
    }
}