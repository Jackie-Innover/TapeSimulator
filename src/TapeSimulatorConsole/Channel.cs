﻿using System;
using System.IO;
using System.Threading;

namespace TapeSimulatorConsole
{
    public class Channel
    {
        private const string VideosFolder = "videos";
        private int _fileIndex;
        public int ChannelId { get; set; }
        public string ApplianceGuid { get; set; }

        /// <summary>
        /// The hour that last time to get file name.
        /// </summary>
        private readonly int _hourGetFileNameLastTime = DateTime.Now.Hour;

        public string NextFileName()
        {
            if (_hourGetFileNameLastTime != DateTime.Now.Hour)
            {
                //when change to next hour, reset _fileIndex to 0.
                Interlocked.Add(ref _fileIndex, 0 - _fileIndex);
            }

            DateTime currentDateTime = DateTime.Now;

            string datePart = currentDateTime.ToString("yyyy_MM_dd");
            string hourPart = currentDateTime.Hour.ToString("D2");
            string channelIdPart = ChannelId.ToString("D3");
            string folderPath = Path.Combine(datePart, hourPart, VideosFolder, channelIdPart);

            string timePart = currentDateTime.ToString("HHmmss");
            string fileName = $"V_{ChannelId}_{datePart}_{timePart}_{Interlocked.Increment(ref _fileIndex).ToString("D3")}.avi";
            string fullFileName = Path.Combine(ApplianceGuid, folderPath, fileName);

            return fullFileName;
        }
    }
}