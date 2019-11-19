using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace overwolf.plugins.simpleio {
  static internal class OutputDebugStringManager {

    private class ProcessData {
     public int pid;
     public int count;
     public string name;
    }

    private static Dictionary<int, ProcessData> _monitorProcess = 
      new Dictionary<int, ProcessData>();
    internal static Action<int, string, string> Callback = null;

    static OutputDebugStringManager() {
      DebugMonitor.OnOutputDebugString += OnOutputDebugString;
    }


    public static void StopListenOnProcess(string processName, Action<object, object> callback) {
      try {
        lock (_monitorProcess) {
          var data = _monitorProcess.Values.Where(x => x.name == processName).FirstOrDefault();
          if (data == null) {
            if (callback != null)
              callback(false, new { error = "process not found", name = processName });
            return;
          }

          if ((--data.count) <= 0) {
            _monitorProcess.Remove(data.pid);
          }

          if (_monitorProcess.Count == 0) {
            DebugMonitor.Stop();
          }
          if (callback != null)
            callback(true, new { });
        }
      } catch (Exception ex) {
        if (callback != null)
          callback(false, new { error = ex.ToString(), name = processName });
      }
    }

    public static void ListenOnProcess(string processName, Action<object, object> callback) {
      try {
      lock (_monitorProcess) {

        var data = _monitorProcess.Values.Where(x=> x. name == processName).FirstOrDefault();
        if (data != null) {
          data.count++;
          callback(true, new { });
          return;
        }


        using (var process = Process.GetProcessesByName(processName).FirstOrDefault()) {
          if (process == null) {
            callback(false, new { error = "process not found", name = processName });
            return;
          }

          _monitorProcess[process.Id] = new ProcessData() {
            pid = process.Id,
            name = processName,
            count = 1
          };

          if (!DebugMonitor.IsRunning()) {
            DebugMonitor.Start();
          }

          callback(true, new {pid  = process.Id});
        }    
      }
      } catch (Exception ex) {
        callback(false, new { error = ex.ToString(), name = processName });
      }
    }

    private static void OnOutputDebugString(int pid, string text)
    {
      try {
        if (Callback == null)
          return;

        lock (_monitorProcess) {
          if (!_monitorProcess.ContainsKey(pid))
            return;

          Callback(pid,_monitorProcess[pid].name, text);

        }
      } catch (Exception ex) {
        Trace.WriteLine("overwolf.plugins process OutputDebugString error: " + ex.ToString());
      }
    }
  }
}
