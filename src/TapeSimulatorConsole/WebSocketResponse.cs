using System;
using System.Collections.Generic;

namespace TapeSimulatorConsole
{

    public enum WebSocketResponseType
    {
        GetAvailableDiskSpaceResponse,
        PutFileResponse,
        GetFileResponse,
        DeleteFileResponse,
        DeleteFolderResponse,
        TestConnectionResponse,
        AuthenticationResponse,
        //FileExistsResponse,
        //ListDirectoryResponse,
        //ReceiveVideoPkgResponse,
        //ListFileResponse,
        //EraseAllDataResponse,
        //IsSupportPlaybackResponse,
        //GetStorageFileInfoResponse,
        //GetStorageFileInfosResponse,
        //LoadFileFromTapeResponse,
        //LoadFileFromTapeDoneResponse,
        //CancelLoadFileFromTapeResponse,
        //SendEmailToTapeAdminResponse
    }

    public class WebSocketResponse
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public long RequestId { get; set; }
        /// <summary>
        /// Gets or sets the name of the response type.
        /// </summary>
        /// <value>
        /// The name of the response type.
        /// </value>
        public WebSocketResponseType ResponseType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [execute success].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [execute success]; otherwise, <c>false</c>.
        /// </value>
        public bool ExecuteSuccess { get; set; }

        /// <summary>
        /// Gets or sets the exception item.
        /// </summary>
        /// <value>
        /// The exception item.
        /// </value>
        public Exception ExceptionItem { get; set; }
    }

    public class AuthenticationResponse : WebSocketResponse
    {
        public AuthenticationResponse()
        {
            ResponseType = WebSocketResponseType.AuthenticationResponse;
        }

        public Dictionary<WebSocketServerFeatureType, int> ServerFeatures { get; set; }
    }

    public class PutFileResponse : WebSocketResponse
    {
        public PutFileResponse()
        {
            ResponseType = WebSocketResponseType.PutFileResponse;
        }
    }

}
