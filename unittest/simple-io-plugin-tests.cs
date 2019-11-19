using overwolf.plugins.simpleio;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace overwolf.plugins.unittest {
  class SimpleIOPluginTest {
    private SimpleIOPlugin _plugn = new SimpleIOPlugin();

    public void Run() {
      int maxthread = 0;
      int io;
      ThreadPool.GetMaxThreads(out maxthread, out io);
      ThreadPool.SetMaxThreads(1, 1);

      //SimpleIOPlugin plugn = new SimpleIOPlugin();

      _plugn.onFileListenerChanged += plugn_OnFileListenerChanged;
      _plugn.onFileListenerChanged2 += plugn_onFileListenerChanged2;
      _plugn.getMonitorDPI(65539, new Action<object, object>((x, y) => {

      }));

      _plugn.onOutputDebugString += plugn_onOutputDebugString;
      _plugn.listenOnProcess("LeagueClientUx", new Action<object, object>((x, y) => {

      }));

      var folder = _plugn.PROGRAMFILES + "/overwolf";
      _plugn.getLatestFileInDirectory(folder, new Action<object, object>((x, y) => {

      }));


      _plugn.getLatestFileInDirectory(folder + "/*.msi", new Action<object, object>((x, y) => {

      }));


      _plugn.getTextFile(@"c:\Users\elad.bahar\AppData\Local\Overwolf\Log\OverwolfCEF_13096.log", true, new Action<object, object>((x, y) => {

      }));

      _plugn.getBinaryFile(_plugn.PROGRAMFILES + "/overwolf/Overwolf.exe.config", -1, new Action<object, object>((x, y) => {

      }));

      _plugn.listDirectory(@"c:\Users\elad.bahar\AppData\Local\Overwolf", new Action<object, object>((x, y) => {

      }));

      _plugn.getCurrentCursorPosition(new Action<object, object, object, object>((status, reason, x, y) => {
        Trace.WriteLine(reason);
      }));

      _plugn.iniReadValue("Power", "ConsolePrinting", @"C:\Users\elad.bahar\AppData\Local\Blizzard\Hearthstone\log.config",
        new Action<object, object>((status, result) => {
          Trace.WriteLine("iniReadValue: " + status + ", : " + result);
        }));


      //   plugn.iniWriterValue("Power", "LogLevel","2", @"C:\Users\elad.bahar\AppData\Local\Blizzard\Hearthstone\log.config",
      //     new Action<object, object>((status, result) => {
      //       Trace.WriteLine("iniReadValue: " + status + ", : " + result);
      //     }));

      //   plugn.iniWriterValue("Power", "LogLevel", "1", @"C:\Users\elad.bahar\AppData\Local\Blizzard\Hearthstone\log.config",
      //       new Action<object, object>((status, result) => {
      //  Trace.WriteLine("iniReadValue: " + status + ", : " + result);
      //}));


      _plugn.listenOnFile("test", @"e:\temp\python.log", false, new Action<object, object, object>((id, status, line) => {
        // Trace.WriteLine(line);
      }));
      Task.Run(() => {
        try {
          Trace.WriteLine("left button pressed:" + _plugn.isMouseLeftButtonPressed);
          _plugn.stopProcesseListen("LeagueClientUx", new Action<object, object>((x, y) => {

          }));
          Thread.Sleep(5000);
          Trace.WriteLine("left button pressed:" + _plugn.isMouseLeftButtonPressed);
          _plugn.stopFileListen("test");
          //plugn.listenOnFile("test", @"c:\Temp\test.txt", true, new Action<object, object, object>((id, status, line) =>
          //{
          //  Trace.WriteLine(line);
          //}));
        } catch (Exception) {
          //callback(string.Format("error: ", ex.ToString()));
        }
      });

      Console.ReadLine();
    }

    void plugn_onFileListenerChanged2(object arg1, object arg2, object arg3, object arg4) {

    }

    void plugn_onOutputDebugString(object arg1, object name, object arg2) {
      Console.WriteLine(string.Format("onOutputDebugString pid:{0} text:{1}", arg1, arg2));
    }
    void plugn_OnFileListenerChanged(object id, object status, object data) {
      //Console.WriteLine(string.Format("file updated: id:{0} status:{1} data:{2}", id, status, data));
    }
  }
}
