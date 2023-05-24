using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace overwolf.plugins.simpleio {
  static class FileListenerManager {
    private struct FileListenTaskObject {
      public Task task;
      public FileListenerWorker worker;
    }

    private static Dictionary<string, FileListenTaskObject> _listenTaskes =
      new Dictionary<string, FileListenTaskObject>();

    public static void ListenOnFile(string id, string filename, bool skipToEnd,
      Action<object, object, object> callback, Action<object, object, object, bool> notifierDelegate) {
      if (callback == null)
        return;

      try {
        filename = filename.Replace('/', '\\');
        if (!File.Exists(filename)) {
          callback(id, false, "file not found: " + filename);
          return;
        }

        StopExistWorker(id);

        var warpper = new FileListenTaskObject();
        warpper.worker = new FileListenerWorker();
        warpper.task = Task.Run(() => {
          try {
            warpper.worker.ListenOnFile(id, filename, skipToEnd, callback, notifierDelegate);

            lock (_listenTaskes) {
              if (_listenTaskes.ContainsKey(id))
                _listenTaskes.Remove(id);
            }
          } catch (Exception) {
            try {
              lock (_listenTaskes) {
                _listenTaskes.Remove(id);
              }
            } catch (Exception) {

            }

          }

        });

        _listenTaskes[id] = warpper;
      } catch (Exception ex) {
        callback(id, false, "listenOnFile error:" + ex.ToString());
      }
    }

    public static void stopFileListen(string id) {
      StopExistWorker(id);
    }

    public static void ListenOnDirectory(
      string id,
      string path,
      string filter,
      FolderListenerTypes[] notifyFilters,
      Action<object, object, object> callback,
      Action<object, object, object> notifierDelegate
    ) {
      if (callback == null) {
        return;
      }

      try {

        var warpper = new FileListenTaskObject();
        warpper.worker = new FileListenerWorker();
        warpper.task = Task.Run(() => {
          try {
            warpper.worker.ListenOnDirectory(id,
                                             path,
                                             filter,
                                             notifyFilters,
                                             callback,
                                             notifierDelegate);

            lock (_listenTaskes) {
              if (_listenTaskes.ContainsKey(id))
                _listenTaskes.Remove(id);
            }
          } catch (Exception) {
            try {
              lock (_listenTaskes) {
                _listenTaskes.Remove(id);
              }
            } catch (Exception) {

            }

          }

        });

        _listenTaskes[id] = warpper;

      } catch (Exception ex) {

        callback(id, false, "listenOnDirectory error:" + ex.ToString());
      }
    }
    public static void StopFolderListen(string id) {
      StopExistWorker(id);
    }

    private static void StopExistWorker(string id) {
      lock (_listenTaskes) {
        if (_listenTaskes.ContainsKey(id)) {
          try {
            _listenTaskes[id].worker.IsCanceled = true;
          } finally {
            try {
              if (_listenTaskes[id].task != null)
                _listenTaskes[id].task.Dispose();
            } catch {
            }

            if (_listenTaskes.ContainsKey(id))
              _listenTaskes.Remove(id);

          }
        }
      }
    }


    public static void Dispose() {

      lock (_listenTaskes) {
        try {
          foreach (var fileListenTaskObject in _listenTaskes) {
            fileListenTaskObject.Value.worker.IsCanceled = true;
          }
        } catch (Exception) {


        }

        _listenTaskes.Clear();
      }

    }
  }

}
