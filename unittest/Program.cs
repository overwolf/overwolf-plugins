using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace overwolf.plugins.unittest
{
  class Program
  {
    static SimpleIOPlugin plugn = new SimpleIOPlugin();
    static void Main(string[] args)
    {
      int maxthread =0;
      int io;
      ThreadPool.GetMaxThreads(out maxthread, out io);
      ThreadPool.SetMaxThreads(1, 1);

      //SimpleIOPlugin plugn = new SimpleIOPlugin();
      
      plugn.onFileListenerChanged += plugn_OnFileListenerChanged;
      plugn.getMonitorDPI(65539, new Action<object, object>((x, y) =>
      {

      }));

      plugn.onOutputDebugString += plugn_onOutputDebugString;
      plugn.listenOnProcess("LeagueClientUx", new Action<object, object>((x, y) => {

      }));

      var folder = plugn.PROGRAMFILES + "/overwolf";
      plugn.getLatestFileInDirectory(folder, new Action<object, object>((x, y) =>
      {

      }));


      plugn.getLatestFileInDirectory(folder + "/*.msi", new Action<object, object>((x, y) =>
      {

      }));


      plugn.getTextFile(@"c:\Users\elad.bahar\AppData\Local\Overwolf\Log\OverwolfCEF_13096.log", true, new Action<object, object>((x, y) =>
      {

      }));

      plugn.getBinaryFile(plugn.PROGRAMFILES + "/overwolf/Overwolf.exe.config", -1, new Action<object, object>((x, y) =>
      {

      }));

      plugn.listDirectory(@"c:\Users\elad.bahar\AppData\Local\Overwolf", new Action<object, object>((x, y) =>
      {

      }));

      plugn.getCurrentCursorPosition(new Action<object, object, object, object>((status, reason, x, y) =>
      {
        Trace.WriteLine(reason);
      }));


      plugn.listenOnFile("test", @"e:\League of Legends\Logs\Game - R3d Logs\2017-01-29T13-50-43_r3dlog.txt", false, new Action<object, object, object>((id, status, line) =>
      {
       // Trace.WriteLine(line);
      }));
      Task.Run(() =>
      {
        try
        {
          Trace.WriteLine("left button pressed:" + plugn.isMouseLeftButtonPressed);
          plugn.stopProcesseListen("LeagueClientUx", new Action<object, object>((x, y) => {

          }));
          Thread.Sleep(5000);
          Trace.WriteLine("left button pressed:" + plugn.isMouseLeftButtonPressed);
          plugn.stopFileListen("test");
          //plugn.listenOnFile("test", @"c:\Temp\test.txt", true, new Action<object, object, object>((id, status, line) =>
          //{
          //  Trace.WriteLine(line);
          //}));
        }
        catch (Exception ex)
        {
          //callback(string.Format("error: ", ex.ToString()));
        }
      });

      Console.ReadLine();
    }

    static void plugn_onOutputDebugString(object arg1,object name, object arg2) {
      Console.WriteLine(string.Format("onOutputDebugString pid:{0} text:{1}", arg1, arg2));
    }
    static void plugn_OnFileListenerChanged(object id, object status, object data)
    {
      //Console.WriteLine(string.Format("file updated: id:{0} status:{1} data:{2}", id, status, data));
    }
  }
}
