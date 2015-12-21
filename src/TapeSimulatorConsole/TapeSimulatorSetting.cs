
using System;
using System.Xml.Linq;
using ThreeVR.Common;

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

        public readonly bool IsLoadConfigurationCorrectly;

        #endregion Properties

        #region Life Time

        private TapeSimulatorSetting()
        {
            try
            {
                var configElement = XElement.Load("TapeSimulatorConfig.xml");
                XElement essRootElement = configElement.Element("ExtendedStorageServerDetails");
                string host = essRootElement.Element("Host").Value;
                string portNumber = essRootElement.Element("Port").Value;
                string userName = essRootElement.Element("UserName").Value;
                string password = essRootElement.Element("Password").Value;
                password = Runtime.GenerateSha1Hash(password);

                XElement essClientRootElement = configElement.Element("ExtendedStorageClientDetails");

                string clientGuid = essClientRootElement.Element("ClientGuid").Value;
                //string clientDisplayName = essClientRootElement.Element("ClientDisplayName").Value;

                XElement tapeSimulatorElement = configElement.Element("TapeSimulatorDetails");

                int sendTimeCount = int.Parse(tapeSimulatorElement.Element("SendTimesPerMinute").Value);
                string videoFilePath = tapeSimulatorElement.Element("VideoFilePath").Value;
                int channelCount;
                if (!int.TryParse(tapeSimulatorElement.Element("ChannelCount").Value, out channelCount))
                {
                    channelCount = 36;
                }
                //XElement isWriteToDiskElement = tapeSimulatorElement.Element("IsWriteToDisk");
                //var isWriteToDisk = isWriteToDiskElement == null || bool.Parse(isWriteToDiskElement.Value);
                //isWriteToDisk = false;

                Host = host;
                PortNumber = portNumber;
                UserName = userName;
                Password = password;
                ClientGuid = clientGuid;
                ClientDisplayName = "VMS_TapeSimulator001";

                SendTimeCount = sendTimeCount;
                VideoFilePath = videoFilePath;
                ChannelCount = channelCount;
                IsWriteToDisk = true;
                IsLoadConfigurationCorrectly = true;
            }
            catch (Exception ex)
            {
                IsLoadConfigurationCorrectly = false;
                Console.WriteLine("Fail to load TapeSimulatorConfig.xml, please check it.");
            }
        }

        #endregion Life Time
    }
}