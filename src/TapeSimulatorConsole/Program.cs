using System;

namespace TapeSimulatorConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                bool isLoadConfigurationFileCorrectly = TapeSimulatorSetting.Instance.IsLoadConfigurationCorrectly;

                if (!isLoadConfigurationFileCorrectly)
                {
                    Console.WriteLine("Please check configuration, and restart this application.");
                    Console.ReadKey();
                    return;
                }

                AsyncWebSocketRequests.Instance.Start(TapeSimulatorSetting.Instance.Uri,
                    TapeSimulatorSetting.Instance.UserName, TapeSimulatorSetting.Instance.Password,
                    TapeSimulatorSetting.Instance.ClientGuid, TapeSimulatorSetting.Instance.ClientDisplayName);
                SendManager.SendData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to start application. Please check and restart the application." + Environment.NewLine + ex);
            }
            Console.ReadKey();
        }
    }
}