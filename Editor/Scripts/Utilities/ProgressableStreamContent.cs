using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Modified version of Jeff Hansen <jeff@jeffijoe.com>'s ProgressableStreamContent class.
    /// https://github.com/jeffijoe/httpclientgoodies.net/blob/master/src/Jeffijoe.HttpClientGoodies/ProgressableStreamContent.cs
    /// </summary>
    public class ProgressableStreamContent : StreamContent
    {
        private const int DefaultBufferSize = 4096;
        public delegate void ReportUploadProgress(long transferred, long total, float progress);

        private readonly int _bufferSize;
        private readonly ReportUploadProgress _progress;
        private readonly Stream _streamToWrite;
        private bool _contentConsumed;

        public ProgressableStreamContent(Stream streamToWrite, ReportUploadProgress progress)
            : this(streamToWrite, DefaultBufferSize, progress) { }

        public ProgressableStreamContent(Stream streamToWrite, int bufferSize, ReportUploadProgress progress)
            : base(streamToWrite, bufferSize)
        {
            _streamToWrite = streamToWrite;
            _bufferSize = bufferSize;
            _progress = progress;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _streamToWrite.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            PrepareContent();

            var buffer = new byte[_bufferSize];
            var size = _streamToWrite.Length;
            var uploaded = 0;

            using (_streamToWrite)
            {
                while (true)
                {
                    try
                    {
                        var length = _streamToWrite.Read(buffer, 0, buffer.Length);
                        if (length <= 0)
                            break;

                        uploaded += length;
                        _progress?.Invoke(uploaded, size, (float)uploaded / size);
                        await stream.WriteAsync(buffer, 0, length);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _streamToWrite.Length;
            return true;
        }

        private void PrepareContent()
        {
            if (_contentConsumed)
            {
                // If the content needs to be written to a target stream a 2nd time, then the stream must support
                // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                // stream (e.g. a NetworkStream).
                if (_streamToWrite.CanSeek)
                {
                    _streamToWrite.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException("The stream has already been read.");
                }
            }

            _contentConsumed = true;
        }
    }
}