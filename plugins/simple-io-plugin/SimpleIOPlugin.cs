using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace overwolf.plugins.simpleio {
  public class SimpleIOPlugin : IDisposable {
    IntPtr _window = IntPtr.Zero;

    public SimpleIOPlugin(int window) {
      _window = new IntPtr(window); ;
    }

    public SimpleIOPlugin() {
    }
    
    #region Events
    public event Action<object, object, object> onFileListenerChanged;
    public event Action<object, object, object, object> onFileListenerChanged2;
    public event Action<object, object, object> onOutputDebugString;
    public event Action<object, object, object> onFolderListenerChanged;
    #endregion Events

    #region IDisposable
    public void Dispose() {
      FileListenerManager.Dispose();
    }
    #endregion

    #region Properties
    public string PROGRAMFILES {
      get { return Constants.kProgramFiles; }
    }

    public string PROGRAMFILESX86 {
      get { return Constants.kProgramFilesX86; }
    }

    public string COMMONFILES {
      get { return Constants.kCommonFiles; }
    }

    public string COMMONFILESX86 {
      get { return Constants.kCommonFilesX86; }
    }

    public string COMMONAPPDATA {
      get { return Constants.kCommonAppDataFiles; }
    }

    public string DESKTOP {
      get { return Constants.kDesktop; }
    }

    public string WINDIR {
      get { return Constants.kWinDir; }
    }

    public string SYSDIR {
      get { return Constants.kSysDir; }
    }

    public string SYSDIRX86 {
      get { return Constants.kSysDirX86; }
    }

    public string MYDOCUMENTS {
      get { return Constants.kMyDocuments; }
    }

    public string MYVIDEOS {
      get { return Constants.kMyVideos; }
    }

    public string MYPICTURES {
      get { return Constants.kMyPictures; }
    }

    public string MYMUSIC {
      get { return Constants.kMyMusic; }
    }

    public string COMMONDOCUMENTS {
      get { return Constants.kCommonDocuments; }
    }

    public string FAVORITES {
      get { return Constants.kFavorites; }
    }

    public string FONTS {
      get { return Constants.kFonts; }
    }

    public string STARTMENU {
      get { return Constants.kStartMenu; }
    }

    public string LOCALAPPDATA {
      get { return Constants.kLocalApplicationData; }
    }

    public bool isMouseLeftButtonPressed {
      get {
        return Convert.ToBoolean(GetKeyState(VK_LBUTTON) & KEY_PRESSED);

      }
    }
    public bool isMouseRightButtonPressed {
      get {
        return Convert.ToBoolean(GetKeyState(VK_RBUTTON) & KEY_PRESSED);
      }
    }

    #endregion

    #region Functions
    public void fileExists(string path, Action<object> callback) {

      if (callback == null)
        return;

      if (string.IsNullOrEmpty(path)) {
        callback(false);
        return;
      }

      Task.Run(() => {
        try {
          path = path.Replace('/', '\\');
          callback(File.Exists(path));
        } catch (Exception ex) {
          callback(string.Format("error: ", ex.ToString()));
        }
      });


    }

    public void isDirectory(string path, Action<object> callback) {
      if (callback == null)
        return;

      if (path == null) {
        callback(false);
        return;
      }

      try {
        Task.Run(() => {
          try {
            path = path.Replace('/', '\\');
            callback(Directory.Exists(path));
          } catch (Exception) {

            callback(false);
          }
        });
      } catch (Exception ex) {
        callback(string.Format("error: ", ex.ToString()));
      }
    }

    public void writeLocalAppDataFile(string path, string content, Action<object, object> callback) {
      if (callback == null)
        return;

      try {
        Task.Run(() => {
          string filePath = "";
          try {
            path = path.Replace('/', '\\');
            if (path.StartsWith("\\")) {
              path = path.Remove(0, 1);
            }
            filePath = Path.Combine(LOCALAPPDATA, path);

            // make sure the folder exists, prior to writing the file
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Directory.Exists) {
              fileInfo.Directory.Create();
            }

            using (FileStream filestream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write)) {
              byte[] info = new UTF8Encoding(false).GetBytes(content);
              filestream.Write(info, 0, info.Length);
            }
            callback(true, "");
          } catch (Exception ex) {
            callback(false, string.Format("unexpected error when trying to write to '{0}' : {1}",
              filePath, ex.ToString()));
          }
        });
      } catch (Exception ex) {
        callback(false, string.Format("error: ", ex.ToString()));
      }
    }

    public void getLatestFileInDirectory(string path, Action<object, object> callback) {
      if (callback == null)
        return;

      try {
        Task.Run(() => {
          try {
            var filePattern = "*";
            var folder = path;

            if (!NormalizePathWithFilePattern(path, out filePattern, out  folder)) {
              callback(false, "folder not found");
              return;
            }

            var lastFile = Directory.GetFiles(folder, filePattern)
                       .OrderByDescending(x => new FileInfo(x).LastWriteTime)
                       .FirstOrDefault();

            if (lastFile == null) {
              callback(false, "no file in directory");
            } else {
              callback(true, new FileInfo(lastFile).Name);
            }

          } catch (Exception ex) {
            callback(false, string.Format("unknown error: ", ex.ToString()));
          }
        });

      } catch (Exception ex) {
        callback(false, string.Format("unknown error: ", ex.ToString()));
      }
    }

    public void getTextFile(string filePath, bool widechars, Action<object, object> callback) {
      if (callback == null)
        return;

      try {
        Task.Run(() => {
          string output = string.Empty;
          try {
            if (!File.Exists(filePath)) {
              callback(false, string.Format("File doesn't exists", filePath));
              return;
            }

            try {
              output = File.ReadAllText(
                filePath, widechars ? Encoding.Default : Encoding.UTF8);
            } catch {
              try {
                string tempFile = Path.GetTempFileName();
                File.Copy(filePath, tempFile, true);

                output = File.ReadAllText(tempFile,
                  widechars ? Encoding.Default : Encoding.UTF8);

                try { File.Delete(tempFile); } catch { }

              } catch (Exception ex) {
                callback(false, "Fail to create temp file " + ex.ToString());
                return;
              }
            }

            callback(true, output);

          } catch (Exception ex) {
            callback(false, string.Format("Exception GetTextFile : {0}", ex.ToString()));
          }
        });
      } catch (Exception ex) {
        callback(false, string.Format("Exception GetTextFile: {0}", ex.ToString()));
      }
    }

    public void getBinaryFile(string filePath, int limit, Action<object, object> callback) {
      if (callback == null)
        return;

      try {
        Task.Run(() => {
          string output = string.Empty;
          try {
            if (!File.Exists(filePath)) {
              callback(false, string.Format("File doesn't exists", filePath));
              return;
            }

            try {
              using (FileStream filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                int readSize = limit <= 0 ?
                  (int)filestream.Length : (Math.Min(limit, (int)filestream.Length));

                if (readSize == 0) {
                  callback(false, "file has no content");
                  return;
                }

                byte[] buffer = new byte[readSize];
                filestream.Read(buffer, 0, readSize);

                StringBuilder result = new StringBuilder(readSize * 2);
                result.Append(buffer[0]);

                for (int i = 1; i < buffer.Length; i++) {
                  result = result.Append(",");
                  result = result.Append(buffer[i]);
                }

                callback(true, result.ToString());
              }
            } catch (Exception ex) {
              callback(false, "Fail to read file " + ex.ToString());
              return;
            }

          } catch (Exception ex) {
            callback(false, string.Format("Exception GetTextFile : {0}", ex.ToString()));
          }
        });
      } catch (Exception ex) {
        callback(false, string.Format("Exception GetTextFile: {0}", ex.ToString()));
      }
    }

    public void listDirectory(string path, Action<object, object> callback) {
      if (callback == null)
        return;

      if (path == null) {
        callback(false, "empty path");
        return;
      }

      try {
        Task.Run(() => {
          try {

            var filePattern = "*";
            var folder = path;
            if (!NormalizePathWithFilePattern(path, out filePattern, out  folder)) {
              callback(false, "folder not found");
              return;
            }

            var fileList = Directory.GetFiles(folder, filePattern)
              .OrderByDescending(x => new FileInfo(x).LastWriteTime);

            StringBuilder resoltJson = new StringBuilder();
            resoltJson.Append("[");
            foreach (string file in fileList) {
              resoltJson.Append(string.Format("{{ \"name\" : \"{0}\" , \"type\": \"file\" }},", new FileInfo(file).Name));
              //resoltJson.Append(string.Format("{ {0} }", file));
            }

            var folderList = Directory.GetDirectories(folder);
            foreach (string subFolder in folderList) {
              resoltJson.Append(string.Format("{{ \"name\" : \"{0}\" , \"type\": \"dir\" }},", new DirectoryInfo(subFolder).Name));
              //resoltJson.Append(string.Format("{ {0} }", file));
            }
            resoltJson.Remove(resoltJson.Length - 1, 1);

            resoltJson.Append("]");
            callback(true, resoltJson.ToString());

          } catch (Exception ex) {
            callback(false, string.Format("listDirectory exception: {0}", ex.ToString()));
          }
        });
      } catch (Exception ex) {
        callback(false, string.Format("listDirectory exception: {0}", ex.ToString()));
      }
    }

    public void listenOnFile(string id, string filename, bool skipToEnd, Action<object, object, object> callback) {
      FileListenerManager.ListenOnFile(id, filename, skipToEnd, callback, OnFileChanged);
    }

    public void stopFileListen(string id) {
      FileListenerManager.stopFileListen(id);
    }

    public void listenOnDirectory(string id, string path, string filter, Action<object, object, object> callback) {
      FileListenerManager.ListenOnDirectory(id, path, filter, callback, OnDirectoryChanged);
    }

    public void iniReadValue(string section, string key, string filePath, Action<object, object> callback) {
      Task.Run(() => {
        IniFile.IniReadValue(section, key, filePath, callback);
      });
    }
    public void iniWriteValue(string section, string key, string value, string filePath, Action<object, object> callback ) {
      Task.Run(() => {
        IniFile.IniWriteValue(section, key, value, filePath, callback);
      });
    }

    public void listenOnProcess(string processname, Action<object, object> callback) {
      OutputDebugStringManager.Callback = OnOutputDebugString;
      Task.Run(() => {
        OutputDebugStringManager.ListenOnProcess(processname, callback);
      });
    }

    public void stopProcesseListen(string processname, Action<object, object> callback) {
      Task.Run(() => {
        OutputDebugStringManager.StopListenOnProcess(processname, callback);
      });
    }

    private void OnFileChanged(object id, object status, object data, bool isNew) {
      if (onFileListenerChanged != null) {
        onFileListenerChanged(id, status, data);
      }

      // new event: |isNew| the line was written after start listen on file
      if (onFileListenerChanged2 != null) {
         onFileListenerChanged2(id, status, data, isNew);
      }
    }

    private void OnDirectoryChanged(object id, object status, object data) {
      if (onFolderListenerChanged != null) {
        onFolderListenerChanged(id, status, data);
      }
    }

    private void OnOutputDebugString(int processId,string processName, string text) {
      if (onOutputDebugString != null) {
        onOutputDebugString(processId, processName,  text);
      }
    }

    public void simulateMouseClick(int x, int y, Action<object> callback) {
      if (_window == IntPtr.Zero) {
        if (callback != null) {
          callback(new { status = "error", error = "window handle undefined" });
        }
        return;
      }

      Task.Run(() => {
        try {
          SendMessage(_window, WM_LBUTTONDOWN, 0, MakeLParam(x, y));
          Thread.Sleep(100);
          SendMessage(_window, WM_LBUTTONUP, 0, MakeLParam(x, y));
          if (callback != null) {
            callback(new { status = "success" });
          }
        } catch (Exception ex) {
          if (callback != null) {
            callback(new { status = "error", error = ex.ToString() });
          }
        }
      });
    }

    #region getCurrentCursorPosition
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
      public int X;
      public int Y;

      public POINT(int x, int y) {
        this.X = x;
        this.Y = y;
      }

      public override string ToString() {
        return "{" + X + "," + Y + "}";
      }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern long GetWindowText(IntPtr hwnd, StringBuilder lpString, long cch);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool ScreenToClient(IntPtr hwnd, ref POINT pt);

    public void getCurrentCursorPosition(Action<object, object, object, object> callback) {
      if (callback == null)
        return;

      try {
        POINT point;
        if (!GetCursorPos(out point)) {
          callback(false, "Failed to GetCursorPos", 0, 0);
          return;
        }

        IntPtr hWnd = WindowFromPoint(point);
        if (hWnd == IntPtr.Zero) {
          callback(false, "Failed to get WindowFromPoint", 0, 0);
          return;
        }

        bool result = ScreenToClient(hWnd, ref point);
        int length = GetWindowTextLength(hWnd);
        StringBuilder windowText = new StringBuilder("", length + 5);
        GetWindowText(hWnd, windowText, length + 2);

        callback(result, windowText.ToString(), point.X, point.Y);
      } catch (Exception ex) {
        callback(false, string.Format("exception: {0}", ex.ToString()), 0, 0);
      }
    }

    #endregion getCurrentCursorPosition

    #region getMonitorDPI
    private enum PROCESS_DPI_AWARENESS {
      PROCESS_DPI_UNAWARE = 0,
      PROCESS_SYSTEM_DPI_AWARE = 1,
      PROCESS_PER_MONITOR_DPI_AWARE = 2
    }

    private enum MONITOR_DPI_TYPE {
      MDT_EFFECTIVE_DPI = 0,
      MDT_ANGULAR_DPI = 1,
      MDT_RAW_DPI = 2,
      MDT_DEFAULT = MDT_EFFECTIVE_DPI
    };

    [DllImport("Shcore.dll")]
    private static extern int GetDpiForMonitor([In]IntPtr hmonitor, [In]MONITOR_DPI_TYPE dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

    [DllImport("SHCore.dll", SetLastError = true)]
    private static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

    [DllImport("SHCore.dll", SetLastError = true)]
    private static extern int GetProcessDpiAwareness(IntPtr hprocess, out PROCESS_DPI_AWARENESS awareness);

    public void getMonitorDPI(double monitorHandle, Action<object, object> callback) {
      if (callback == null)
        return;

      try {
        try {
          Task.Run(() => {
            try {
              PROCESS_DPI_AWARENESS value;
              var result = GetProcessDpiAwareness(IntPtr.Zero, out value);
              if (result != 0) {
                callback(false, "error: no Dpi awareness");
                return;
              }

              if (value != PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE) {
                if (SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE) != 0) {
                  callback(false, "error: setting Dpi awareness");
                  return;
                }
              }

              uint dpiX = 0;
              uint dpiY = 0;
              result = GetDpiForMonitor(new IntPtr((long)monitorHandle),
                MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out dpiX, out dpiY);

              callback(result == 0, (int)dpiX);

            } catch (Exception ex) {
              callback(false, "Unknown error: " + ex.ToString());
            }
          });

        } catch (Exception ex) {
          callback(false, "Unknown error: " + ex.ToString());
        }
      } catch (Exception ex) {
        callback(false, "Unknown error: " + ex.ToString());
      }
    }
    #endregion getMonitorDPI

    #endregion Functions

    #region Private Funcs
    [DllImport("USER32.dll")]
    private static extern short GetKeyState(int nVirtKey);

    private bool NormalizePathWithFilePattern(string path, out string pattern, out string folder) {
      path = path.Replace('/', '\\');
      folder = path;
      pattern = "*";
      if (!Directory.Exists(folder)) {
        folder = Path.GetDirectoryName(path);
        pattern = Path.GetFileName(path);
      }

      if (!Directory.Exists(folder)) {
        return false;
      }

      return true;
    }

    [DllImport("USER32.DLL", EntryPoint = "SendMessage")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    private int MakeLParam(int LoWord, int HiWord) {
      return ((HiWord << 16) | (LoWord & 0xffff));
    }

    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int VK_LBUTTON = 0x01;
    private const int VK_RBUTTON = 0x02;
    private const int KEY_PRESSED = 0x8000;
    #endregion Private Funcs
  }
}
