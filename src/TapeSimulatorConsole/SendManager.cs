using System;
using System.Diagnostics;
using System.Threading;
using ThreeVR.Libraries.Common;

namespace TapeSimulatorConsole
{
    public static class SendManager
    {
        private static readonly QueuingThreadPool _threadPool = new QueuingThreadPool("TapeSimulator", 10);
        private static readonly WebSocketHandler webSocketHandler = new WebSocketHandler();
        private static bool _running;

        public static void SendData()
        {
            while (true)
            {
                _running = true;
                _threadPool.AddTask(SendDataToEss);
                Thread.Sleep(60 * 1000);

                while (_running)
                {
                    Console.WriteLine("Warnning: Not able to send all scheduled files in 1 min, still waiting");
                    Thread.Sleep(1000);
                }
            }
        }

        private static void SendDataToEss()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TapeSimulatorSetting.Instance.SendTimeCount; i++)
            {
                Channel channel = ChannelManager.Instance.GetNextChannel();
                string videoFilePath = channel.NextFileName();
                webSocketHandler.PutFile(videoFilePath);
            }

            stopwatch.Stop();
            if (stopwatch.Elapsed.TotalSeconds > 60)
            {
                Console.WriteLine("Warnning: Send over time!!!. Spent time:{0}", stopwatch.Elapsed.TotalSeconds);
            }
            else
            {
                Console.WriteLine("Send done. Spent time {0}", stopwatch.Elapsed.TotalSeconds);
            }
            _running = false;
        }
    }
}