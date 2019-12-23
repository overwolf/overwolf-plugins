using com.overwolf.dwnldr;
using System;
using System.IO;

namespace overwolf.plugins.unittest {
  public class DownloaderTests {
    private Downloader _downloader = new Downloader();

    // CHANGE THESE!!!
    private const string kRemoteFile = 
      @"https://download.overwolf.com/install/Download?Channel=web_dl_btn";
    private const string kLocalFile = @"d:\temp\setup\setup.exe";

    public void Run() {
      _downloader.onDownloadComplete += OnDownloadComplete;
      _downloader.onDownloadError += OnDownloadError;
      _downloader.onDownloadProgress += OnDownloadProgress;

      _downloader.downloadFile(kRemoteFile, kLocalFile);
    }

    private void OnDownloadProgress(object obj) {
      // obj is a json object
      Console.WriteLine("Progress: " + obj);
    }

    private void OnDownloadError(object obj) {
      // obj is a json object
      Console.WriteLine("Error: " + obj);
    }

    private void OnDownloadComplete(object obj) {
      // obj is a json object
      Console.WriteLine("Completed: " + obj);
    }
  }
}
