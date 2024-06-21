using Proyecto26;
using RSG;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public class FileTooLargeException : IOException
    {
        public FileTooLargeException(string message) : base(message) { }
    }

    /// <summary>
    /// Handles uploading a list of files to their respective URLs sequentially.
    /// If any web request fails, then the upload process will abort and the 'exception' property will be populated.
    /// Should be used with a coroutine (yield on the object instance).
    /// </summary>
    public class FileUploader : IEnumerator
    {
        private abstract class WebRequestInfo
        {
            public string filePath;
            public string url;
            public long maxFileSizeBytes;

            public long fileSizeBytes { get; private set; }

            public void ThrowExceptionOnInvalidFile()
            {
                // Accessing length property on FileInfo will throw NotFoundException if file doesn't exist.
                CacheFileInfo();

                string fileName = Path.GetFileName(filePath);
                if (maxFileSizeBytes > 0 && fileSizeBytes > maxFileSizeBytes)
                    throw new FileTooLargeException($"File size of '{fileName}' ({fileSizeBytes:N0} byte(s)) exceeds limit of {maxFileSizeBytes:N0} byte(s)");
            }

            public abstract IPromise SendWebRequest();

            public void CacheFileInfo()
            {
                FileInfo file = new(filePath);
                fileSizeBytes = file.Length;
            }
        }

        private class WebRequestInfo<T> : WebRequestInfo
        {
            public Func<WebRequestArgs, IPromise<T>> promiseGenerator;

            public override IPromise SendWebRequest()
            {
                // File could have been modified while request was queued. Revalidate before sending.
                ThrowExceptionOnInvalidFile();

                // Sends a web request by executing the generator function.
                IPromise<T> requestPromise = promiseGenerator?.Invoke(new WebRequestArgs() {
                    url = url,
                    fileBytes = File.ReadAllBytes(filePath)
                });

                // Convert generic promise to non-generic type for progress callbacks.
                Promise proxyPromise = new();
                requestPromise.Progress(proxyPromise.ReportProgress);
                requestPromise.Then(_ => proxyPromise.Resolve());
                requestPromise.Catch(proxyPromise.Reject);
                return proxyPromise;
            }
        }

        public struct WebRequestArgs
        {
            public string url;
            public byte[] fileBytes;
        }

        public object Current => null;

        public bool progressBarEnabled { get; set; }
        public string progressBarTitleOverride { get; set; }
        public Exception exception { get; private set; }

        private Queue<WebRequestInfo> _queuedWebRequests = new();
        private int _numEnqueuedRequests = 0;
        private int _numUploadedRequests = 0;
        private long _numEnqueuedBytes = 0;
        private long _numUploadedBytes = 0;
        private double _firstRequestTime = 0.0;
        private WebRequestInfo _currentRequestInfo = null;

        public FileUploader()
        {
            Reset();
        }

        public void EnqueueWebRequest<T>(string filePath, string url, Func<WebRequestArgs, IPromise<T>> requestPromiseGenerator, long maxFileSizeBytes = 0)
        {
            // Can't enqueue more requests if one already failed.
            if (exception != null)
                return;

            try
            {
                if (requestPromiseGenerator == null)
                    throw new ArgumentException($"{nameof(requestPromiseGenerator)} cannot be null");

                WebRequestInfo<T> info = new() {
                    filePath = filePath,
                    url = url,
                    promiseGenerator = requestPromiseGenerator,
                    maxFileSizeBytes = maxFileSizeBytes
                };
                info.ThrowExceptionOnInvalidFile();
                _queuedWebRequests.Enqueue(info);

                _numEnqueuedRequests++;
                _numEnqueuedBytes += info.fileSizeBytes;
            }
            catch (Exception ex)
            {
                Abort(ex);
            }
        }

        public bool MoveNext()
        {
            if (exception != null)
                return false;

            if (_currentRequestInfo != null)
                return true;

            if (_queuedWebRequests.Count == 0)
                return false;

            try
            {
                WebRequestInfo info = _queuedWebRequests.Dequeue();
                _currentRequestInfo = info;

                if (_firstRequestTime <= 0.0)
                    _firstRequestTime = EditorApplication.timeSinceStartup;

                IPromise requestPromise = info.SendWebRequest();
                requestPromise
                    .Then(() => {
                        _numUploadedRequests++;
                        _numUploadedBytes += _currentRequestInfo.fileSizeBytes;
                    })
                    .Catch(ex => Abort(ex))
                    .Finally(() => _currentRequestInfo = null);

                if (progressBarEnabled)
                    requestPromise.Progress(p => UpdateProgressBar(p));

                return true;
            }
            catch (Exception ex)
            {
                Abort(ex);
                return false;
            }
        }

        /// <summary>
        /// Resets state of uploader for reuse.
        /// </summary>
        public void Reset()
        {
            _queuedWebRequests.Clear();
            _numEnqueuedRequests = 0;
            _numUploadedRequests = 0;
            _numEnqueuedBytes = 0;
            _numUploadedBytes = 0;
            _firstRequestTime = 0.0;
            progressBarEnabled = !Application.isBatchMode;
            progressBarTitleOverride = null;
            exception = null;
        }

        private void UpdateProgressBar(float progress)
        {
            if (exception != null || _currentRequestInfo == null || _numEnqueuedBytes == 0)
            {
                UnityEditor.EditorUtility.ClearProgressBar();
                return;
            }

            progress = Mathf.Clamp01(progress);
            long cumulativeUploadedBytes = _numUploadedBytes + (long)(_currentRequestInfo.fileSizeBytes * progress);
            float totalProgress = Mathf.Clamp01(cumulativeUploadedBytes / (float)_numEnqueuedBytes);

            int secondsElapsed = Mathf.Max(0, (int)(EditorApplication.timeSinceStartup - _firstRequestTime));
            string fileName = EditorUtility.TruncateFromMiddle(Path.GetFileName(_currentRequestInfo.filePath), maxLength: 50);
            UnityEditor.EditorUtility.DisplayProgressBar(
                (!string.IsNullOrEmpty(progressBarTitleOverride) ? progressBarTitleOverride : "Uploading files") + $" ({secondsElapsed} sec)",
                $"{cumulativeUploadedBytes / 1024:N0}kb / {_numEnqueuedBytes / 1024:N0}kb ({(totalProgress * 100f):F0}%) - {fileName} ({_numUploadedRequests + 1} of {_numEnqueuedRequests})",
                totalProgress
            );
        }

        /// <summary>
        /// Avoid throwing uncaught exceptions since there's no way to distinguish between an editor coroutine that finished successfully from one
        /// that stopped due to an uncaught exception. There's no way to know if an editor coroutine is running either, so the coroutine's parent
        /// function may end up waiting indefinitely.
        /// Use this function to safely stop running and store the exception object for the parent coroutine to reference.
        /// This won't be necessary anymore once unhandled exceptions can be handled during the build process.
        /// </summary>
        private void Abort(Exception ex)
        {
            bool alreadyAborted = exception != null;
            Reset();

            // Don't report exceptions after the first one that aborted the upload.
            if (!alreadyAborted && ex != null)
            {
                exception = ex;
                Debug.LogException(ex);
                UnityEditor.EditorUtility.ClearProgressBar();
            }
        }
    }
}
