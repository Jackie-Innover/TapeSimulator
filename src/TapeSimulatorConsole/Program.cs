using System;

namespace TapeSimulatorConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //TODO: Use a video file size more than 10 MB to test.
            Console.WriteLine("Tape Simulator do not use security protocol to send data to ESS.");
            try
            {
                AsyncWebSocketRequests.Instance.Start(TapeSimulatorSetting.Instance.Uri,
                    TapeSimulatorSetting.Instance.UserName, TapeSimulatorSetting.Instance.Password,
                    TapeSimulatorSetting.Instance.ClientGuid, TapeSimulatorSetting.Instance.ClientDisplayName);
                SendManager.SendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load TapeSimulatorConfig.xml file. Please check it and restart the application." + Environment.NewLine + ex);
            }
            Console.ReadKey();
        }
    }
}