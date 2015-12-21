using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;

namespace TapeSimulatorConsole
{
    public class WebSocketClient
    {
        private readonly string _password;
        private readonly string _uri;
        private readonly string _userName;
        private readonly string _applianceGuid;
        private readonly string _applianceDisplayName;
        private volatile AutoResetEvent _waitConnectionOpenedEvent;
        public volatile WebSessionStatus Status;
        private static long _sendDataLengthCount;

        private int _loginAttemps;
        private volatile WebSocket _webSocket;
        private static long _getPutResponseCnt;
        private static long _getPutRequestCnt;

        public static readonly Timer MonitorSendDataCounTimer = new Timer(state =>
          {
              Console.WriteLine("Send data speed: {0} MB/s", (_sendDataLengthCount * 1.0 / 1024 / 1024).ToString("F2"));
              Interlocked.Add(ref _sendDataLengthCount, 0 - _sendDataLengthCount);

          }, null, Timeout.Infinite, Timeout.Infinite);

        public WebSocketClient(string uri, string userName, string password, string applianceGuid, string applianceDisplayName)
        {
            //ServicePointManager.ServerCertificateValidationCallback = OnValidationCallback;
            _waitConnectionOpenedEvent = new AutoResetEvent(false);
            Status = WebSessionStatus.Inactive;
            _uri = uri;
            _userName = userName;
            _password = password;
            _applianceGuid = applianceGuid;
            _applianceDisplayName = applianceDisplayName;
            Login();
        }

        public static void ResetSendDataCount()
        {
            Interlocked.Add(ref _sendDataLengthCount, 0 - _sendDataLengthCount);
        }

        public void Login()
        {
            try
            {
                if (Status != WebSessionStatus.Active)
                {
                    Console.WriteLine("Try to login webscoket server now");
                    _waitConnectionOpenedEvent.Reset();
                    Status = WebSessionStatus.Logging;
                    Interlocked.Increment(ref _loginAttemps);
                    var cookies = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(WebSocketConst.UserNameKey, _userName),
                        new KeyValuePair<string, string>(WebSocketConst.PasswordKey, _password),
                        new KeyValuePair<string, string>(WebSocketConst.ClientGuidKey, _applianceGuid),
                        new KeyValuePair<string, string>(WebSocketConst.ClientHostNameKey, _applianceDisplayName),
                        new KeyValuePair<string, string>(WebSocketConst.WebSocketClientVersion, "2")};

                    _webSocket = new WebSocket(_uri, cookies: cookies)
                    {
                        AllowUnstrustedCertificate = false,
                        EnableAutoSendPing = false
                    };
                    _webSocket.Closed += WebSocket_Closed;
                    _webSocket.Error += WebSocket_Error;
                    _webSocket.MessageReceived += webSocket_MessageReceived;
                    //_webSocket.DataReceived += websocket_DataReceived;
                    _webSocket.Opened += WebSocket_Opened;
                    _webSocket.Open();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void webSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var response = JsonConvert.DeserializeObject<WebSocketResponse>(e.Message);
            if (response.ResponseType == WebSocketResponseType.PutFileResponse)
            {
                Interlocked.Increment(ref _getPutResponseCnt);
            }
            switch (response.ResponseType)
            {
                case WebSocketResponseType.AuthenticationResponse:
                    {
                        if (response.ExecuteSuccess)
                        {
                            Status = WebSessionStatus.Active;
                            Console.WriteLine("WebSocket Connection is opened" + Environment.NewLine);
                        }
                        else
                        {
                            Status = WebSessionStatus.Inactive;
                            Interlocked.Increment(ref _loginAttemps);
                            Console.WriteLine("Exception during authentication: " + response.ExceptionItem);
                        }
                        _waitConnectionOpenedEvent.Set();
                        break;
                    }
                case WebSocketResponseType.PutFileResponse:
                    {
                        var putFileResponse = JsonConvert.DeserializeObject<PutFileResponse>(e.Message);
                        WebSocketNotificationHandler.ReceiveResponse(response.RequestId, putFileResponse.ExecuteSuccess);
                        break;
                    }
                default:
                    Console.WriteLine("Unrecognized WebSocket response");
                    break;
            }
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            Status = WebSessionStatus.Active;
        }

        private void WebSocket_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            Status = WebSessionStatus.Inactive;
        }

        public bool SendRequest(WebSocketRequest request, bool isAsync = false)
        {
            request.ClientGuid = _applianceGuid;
            if (Status == WebSessionStatus.Logging)
            {
                _waitConnectionOpenedEvent.WaitOne(TimeSpan.FromSeconds(20));
            }
            if (Status != WebSessionStatus.Active)
            {
                Console.WriteLine("Inactive connection. Refuse this request " + request);
                if (isAsync)
                {
                    WebSocketNotificationHandler.ReceiveResponse(request.RequestId, false);
                }
                return false;
            }

            if (request.RequestType == WebSocketRequestType.PutFileProxyRequest)
            {
                PutFileProxyRequest putFileProxyRequest = (PutFileProxyRequest)request;
                foreach (NeedSendData needSendData in NeedSendDataManager.Instance.NeedSendDatas)
                {
                    PutFileRequest putFileRequest = new PutFileRequest
                    {
                        RequestId = RequestHelper.NextRequestId(),
                        FilePath = putFileProxyRequest.FilePath,
                        FileData = needSendData.FileData,
                        Position = needSendData.Position,
                        IsWriteToDisk = TapeSimulatorSetting.Instance.IsWriteToDisk
                    };

                    bool isSuccess = (bool)WebSocketHandler.SendRequestReturnResult(this, putFileRequest);
                    Interlocked.Add(ref _sendDataLengthCount, needSendData.FileData.Length);
                    if (!isSuccess)
                    {
                        WebSocketNotificationHandler.ReceiveResponse(putFileProxyRequest.RequestId, false);
                        return false;
                    }
                }

                WebSocketNotificationHandler.ReceiveResponse(putFileProxyRequest.RequestId, true);
                return true;
            }
            //Send file bytes directly to ESS Server without Json because frequent Json deserialization costs high
            // CPU in ESS Server.
            if (request.RequestType == WebSocketRequestType.PutFileRequest)
            {
                Interlocked.Increment(ref _getPutRequestCnt);
                PutFileRequest putFileRequest = (PutFileRequest)request;
                var data = WebSocketBlockWrite.PutFileRequestToByte(putFileRequest);
                var arrayList = new List<ArraySegment<byte>>();
                var array = new ArraySegment<byte>(data);
                arrayList.Add(array);
                _webSocket.Send(arrayList);
            }
            return true;
        }

        public void Dispose()
        {
            try
            {
                _webSocket.Close();
                Status = WebSessionStatus.Inactive;
            }
            catch
            {
                //Ignore this exception because we don't care about it.
            }
        }
    }
}