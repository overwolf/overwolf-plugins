using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace overwolf.plugins {
  /// <summary>
  /// Create a New INI file to store or load data
  /// </summary>
  internal class IniFile {
    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,
        string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,
             string key, string def, StringBuilder retVal,
        int size, string filePath);

    [DllImport("kernel32.dll")]
    public static extern uint GetLastError();

    static object _sync = new object();

    public static void IniWriteValue(string section, string key,
       string value, string filePath, Action<object, object> callback) {
      try {
        lock (_sync) {
          WritePrivateProfileString(section, key, value, filePath);
          callback(true, "");
        }
      } catch (Exception ex) {
        callback(false, ex.ToString());
      }
    }

    public static void IniReadValue(string section, string key, string filePath, Action<object, object> callback) {
      try {
        lock (_sync) {
          StringBuilder temp = new StringBuilder(255);
          int i = GetPrivateProfileString(section, key, "", temp,
              255, filePath);

          var error = GetLastError();
          callback(i != 0, i != 0 ? temp.ToString() : "invalid key");
        }
      } catch (Exception ex) {
        callback(false, ex.ToString());
      }

    }
  }
}
