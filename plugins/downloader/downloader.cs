using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace com.overwolf.dwnldr {
  public class Downloader {
    private const string kErrorAlreadyDownloading = "already_downloading";
    private const string kErrorNoActiveDownload = "no_active_download";
    private const string kErrorFailedUnknownReason = "failed_unknown_reason";
    private const string kDownloadCanceled = "download_canceled";

    public event Action<object> onDownloadError = null;
    public event Action<object> onDownloadProgress = null;
    public event Action<object> onDownloadComplete = null;
    public event Action<object> onFileExecuted = null;

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
    public void downloadFile(string url, string localFile, string userAgant = null) {
      if (_downloading) {
        FireDownloadErrorEvent(url, kErrorAlreadyDownloading);
        return;
      }

      PrepareLocalFileForDownload(localFile);
      SetServicePointManagerGlobalParams();

      try {
        _url = url;
        _localFile = localFile;
        _previousProgress = -1;

        _webClient = new WebClientGzip();
        if (userAgant != null) { 
          _webClient.Headers[HttpRequestHeader.UserAgent] = userAgant;
        }
        _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
        _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
        _webClient.DownloadFileAsync(new Uri(url), localFile);
        _downloading = true;
      } catch (Exception e) {
        _downloading = false;
        FireDownloadErrorEvent(url, e.Message.ToString());
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// cancells active download
    /// </param>
    public void cancelDownload() {
      if (!_downloading) {
        FireDownloadErrorEvent("", kErrorNoActiveDownload);
      }

      _webClient.CancelAsync();
      _downloading = false;
    }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="path">Path of file to run</param>
      /// download completion
      /// </param>
      public async void ExecuteFileProcess(string path, string[] verifiedThumbprints = null) {
      try {
        bool validated = await ValidateFileSigniture(path, verifiedThumbprints);
        if (!validated) {

          onFileExecuted(new {
            result = false,
            reason = "File validation failed"
          });
          return;
        }

        ProcessStartInfo psi = new ProcessStartInfo() {
          FileName = path,
          Arguments = "",
        };

        Process.Start(psi);

        onFileExecuted(new {
          result = true,
        });

      } catch (Exception ex) {
        onFileExecuted(new {
          result = false,
          reason = ex.Message
        });

      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path of file to run</param>
    /// download completion
    /// </param>
    public async Task<bool> ValidateFileSigniture(string path, string[] verifiedThumbprints = null ) {
      try {
        bool signed = await SignitureHelper.IsSigned(path);
        if (!signed) {
          return false;
        }

        if (verifiedThumbprints == null) {
          return true;
        }

        var signitureHashString = await SignitureHelper.GetCertHashString(path);

        if (!verifiedThumbprints.Contains(signitureHashString)) {
          return false;
        }

        return true;
      } catch (Exception) {
        return false;
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

      if (e.Cancelled) {
        try {
          FireDownloadErrorEvent(_url, kDownloadCanceled);
        }
        catch (Exception) {
          FireDownloadErrorEvent(_url, kErrorFailedUnknownReason);
        }
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

    /// <summary>
    /// Sets the global |ServicePointManager| to support new TLS protocols
    /// </summary>
    private static void SetServicePointManagerGlobalParams() {
      // Wrapping this with a try...catch... since we've seen that
      // |ServicePointManager| might not always exist in memory
      try {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                               SecurityProtocolType.Tls11 |
                                               SecurityProtocolType.Tls12 |
                                               SecurityProtocolType.Ssl3;
      } catch (Exception) {
        // NOTE(twolf): Suppressing errors - because if we really need to
        // support these protocols for the file being downloaded, the error
        // will return to the caller of |download|
      }
    }

  }
}
