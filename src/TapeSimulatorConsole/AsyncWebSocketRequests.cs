using System;
using System.Threading;

namespace TapeSimulatorConsole
{
    /// <summary>
    /// Create multiple WebSocket sessions to asynchronize requests
    /// </summary>
    public class AsyncWebSocketRequests
    {
        private static volatile AsyncWebSocketRequests _instance;
        private readonly SingleElementTransfer _nextElement;
        private readonly Thread[] _threads;
        private string _uri;
        private string _userName;
        private string _password;
        private string _applianceGuid;
        private string _applianceDisplayName;

        private const int WebSocketSessionCount = 10;

        private AsyncWebSocketRequests()
        {
            _nextElement = new SingleElementTransfer();
            _threads = new Thread[WebSocketSessionCount];
        }

        public static AsyncWebSocketRequests Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(AsyncWebSocketRequests))
                    {
                        if (_instance == null)
                        {
                            return _instance = new AsyncWebSocketRequests();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Add(WebSocketRequest request)
        {
            _nextElement.Put(request);
        }

        private void Run()
        {
            var handler = new WebSocketClient(_uri, _userName, _password, _applianceGuid, _applianceDisplayName);
            while (true)
            {
                try
                {
                    var request = (WebSocketRequest)_nextElement.Get();
                    if (handler.Status == WebSessionStatus.Inactive)
                    {
                        handler.Login();
                    }
                    handler.SendRequest(request, true);
                }
                catch (ThreadAbortException)
                {
                    //Close the websocket connection
                    handler.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in batch WebSocket pipeline AysncWebSocket.", e);
                }
            }
        }

        public void Start(string uri, string userName, string password, string applianceGuid, string applianceDisplayName)
        {
            _uri = uri;
            _userName = userName;
            _password = password;
            _applianceGuid = applianceGuid;
            _applianceDisplayName = applianceDisplayName;

            if (_instance != null)
            {
                lock (typeof(AsyncWebSocketRequests))
                {
                    if (_instance != null)
                    {
                        for (int threadNumber = 0; threadNumber < WebSocketSessionCount; threadNumber++)
                        {
                            _threads[threadNumber] = new Thread(Run)
                            {
                                Name = $"WebSocket Client {threadNumber + 1}"
                            };
                            _threads[threadNumber].Start();
                        }
                    }
                }
            }
        }

        public void Shutdown()
        {
            if (_instance != null)
            {
                lock (typeof(AsyncWebSocketRequests))
                {
                    if (_instance != null && _threads != null)
                    {
                        foreach (Thread thread in _threads)
                        {
                            thread.Abort();
                        }
                        _instance = null;
                    }
                }
            }
        }
    }
}