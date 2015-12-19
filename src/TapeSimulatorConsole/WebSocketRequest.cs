
namespace TapeSimulatorConsole
{

    public enum WebSocketRequestType
    {
        GetAvailableDiskSpaceRequest,
        PutFileRequest,
        GetFileRequest,
        DeleteFileRequest,
        DeleteFolderRequest,
        TestConnectionRequest,
        FileExistsRequest,
        ListDirectoryRequest,
        VideoRequest,
        ListFileRequest,
        EraseAllDataRequest,
        IsSupportPlaybackRequest,
        GetStorageFileInfoRequest,
        GetStorageFileInfosRequest,
        LoadFileFromTapeRequest,
        CancelLoadFileFromTapeRequest,
        SendEmailToTapeAdminRequest,
        PutFileProxyRequest
    }

    public class WebSocketRequest
    {
        /// <summary>
        /// Gets or sets the client unique identifier.
        /// </summary>
        /// <value>
        /// The client unique identifier.
        /// </value>
        public string ClientGuid { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public long RequestId { get; set; }

        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public WebSocketRequestType RequestType { get; set; }
    }

    /// <summary>
    /// Thansfer the file to the Extended Storage Server.
    /// </summary>
    public class PutFileRequest : WebSocketRequest
    {
        public PutFileRequest()
        {
            RequestType = WebSocketRequestType.PutFileRequest;
        }
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }
        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public FilePosition Position { get; set; }

        public bool IsWriteToDisk { get; set; }

        public override string ToString()
        {
            return $"RequestId:{RequestId}, RequestType:{RequestType}, FilePath:{FilePath}, Position:{Position}";
        }
    }

    public class PutFileProxyRequest : WebSocketRequest
    {
        public PutFileProxyRequest()
        {
            RequestType = WebSocketRequestType.PutFileProxyRequest;
        }
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        public override string ToString()
        {
            return $"RequestId:{RequestId}, RequestType:{RequestType}, FilePath:{FilePath}";
        }
    }

}
