using System.IO;

namespace TapeSimulatorConsole
{
    public static class WebSocketBlockWrite
    {
        /// <summary>
        /// Convert PutFileRequest to bytes
        /// NOTE: Please make sure the parsed order of PutFileRequestToByte() and ByteToPutFileRequest() are same
        /// </summary>
        /// <param name="putFileRequest">The input put file request</param>
        /// <returns>The corresponding bytes for input putFileRequest </returns>
        public static byte[] PutFileRequestToByte(PutFileRequest putFileRequest)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    writer.Write(putFileRequest.ClientGuid);
                    writer.Write(putFileRequest.RequestId);
                    writer.Write((int)putFileRequest.RequestType);
                    writer.Write(putFileRequest.FilePath);
                    writer.Write((int)putFileRequest.Position);
                    writer.Write(putFileRequest.FileData.Length);
                    writer.Write(putFileRequest.FileData);
                    var data = output.ToArray();
                    return data;
                }
            }
        }
    }
}