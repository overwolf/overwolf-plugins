using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;

namespace com.overwolf.dwnldr {
  public class Downloader {
    private const string kErrorAlreadyDownloading = "already_downloading";
    private const string kErrorFailedUnknownReason = "failed_unknown_reason";

    public event Action<object> onDownloadError = null;
    public event Action<object> onDownloadProgress = null;
    public event Action<object> onDownloadComplete = null;  

    // Allow only 1 download per instance    
    private bool _downloading = false;

    // Store this so we give context upon callbacks
    private string _url;
    private string _localFile;
    private int _previousProgress;
    private WebClientGzip _webClient;
    
    /// Public Methods
    public Downloader() {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">URL to download from</param>
    /// <param name="localFile">Destination file on local disk</param>
    /// download completion
    /// </param>
    public void downloadFile(string url, string localFile) {
      if (_downloading) {
        FireDownloadErrorEvent(url, kErrorAlreadyDownloading);
        return;
      }

      PrepareLocalFileForDownload(localFile);      

      try {
        _url = url;
        _localFile = localFile;
        _previousProgress = -1;

        _webClient = new WebClientGzip();
        _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
        _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
        _webClient.DownloadFileAsync(new Uri(url), localFile);
        _downloading = true;
      } catch (Exception e) {
        FireDownloadErrorEvent(url, e.Message.ToString());
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reason"></param>
    private void FireDownloadErrorEvent(string url, string reason) {
      if (onDownloadError == null) {
        return;
      }

      ThreadPool.QueueUserWorkItem(_ => {
        onDownloadError(new {
          url = url,
          reason = reason
        });
      });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDownloadFileCompleted(object sender, 
                                         AsyncCompletedEventArgs e) {

      _downloading = false;

      if (onDownloadComplete == null) {
        return;
      }

      if (e.Error != null) {
        try {
          FireDownloadErrorEvent(_url, e.Error.Message.ToString());
        } catch (Exception) {
          FireDownloadErrorEvent(_url, kErrorFailedUnknownReason);
        }
        return;
      }

      onDownloadComplete(new {
        url = _url,
        localFile = _localFile,
        md5 = CalculateMD5(_localFile)
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDownloadProgressChanged(
      object sender, 
      DownloadProgressChangedEventArgs e) {

      if (onDownloadProgress == null) {
        return;
      }

      if (_previousProgress == e.ProgressPercentage) {
        return;
      }

      _previousProgress = e.ProgressPercentage;

      try {
        onDownloadProgress(new {
          url = _url,
          progress = e.ProgressPercentage
        });
      } catch (Exception) {
      }
    }

    /// <summary>
    /// Assure the file doesn't exist and the full folder path is created
    /// </summary>
    /// <param name="localFile"></param>
    private void PrepareLocalFileForDownload(string localFile) {
      try {
        // Make sure the file doesn't already exist - otherwise we'll fail 
        // downloading
        File.Delete(localFile);
      } catch (Exception) {
        // File probably doesn't exist
      }

      try {
        var localFileInfo = new FileInfo(localFile);
        Directory.CreateDirectory(localFileInfo.DirectoryName);
      } catch (Exception) {

      }
    }

    /// <summary>
    /// https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    static string CalculateMD5(string filename) {
      try {
        using (var md5 = MD5.Create()) {
          using (var stream = File.OpenRead(filename)) {
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash)
                               .Replace("-", "")
                               .ToLowerInvariant();
          }
        }
      } catch(Exception) {
        return String.Empty;
      }
    }

  }
}
