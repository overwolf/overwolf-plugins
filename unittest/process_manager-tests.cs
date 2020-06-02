using System;
using com.overwolf.com.overwolf.procmgr;

namespace overwolf.plugins.unittest {
  class ProcessManagerTests {
    public void Run() {
      Console.WriteLine("Starting");
      var ProcessManager = new ProcessManager();

      bool obsRunning = ProcessManager.isProcessRunning("obs64");
      bool streamLabsRunning = 
        ProcessManager.isProcessRunning("Streamlabs OBS");

      string path = @"notepad.exe";
      string arguments = "";
      object environmentVariables = "{}";
      bool hidden = true;
      var processId = 0;

      ProcessManager.onDataReceivedEvent += (result) => {
        Console.WriteLine("Line: {0}", result);
      };

      ProcessManager.launchProcess(path,
                                   arguments,
                                   environmentVariables,
                                   hidden,
                                   true, // track process
                                   (dynamic result) => {
                                     if (result.GetType().GetProperty("data") != null) {
                                       Console.WriteLine("Process ID: {0}",
                                         result.GetType().GetProperty("data").GetValue(result, null));
                                       processId = result.GetType().GetProperty("data").GetValue(result, null);
                                     }
                                     Console.WriteLine("Process: {0}", result);
                                   });

      string line = Console.ReadLine();
      while (line != "q") {
        ProcessManager.sendTextToProcess(processId, line);
        line = Console.ReadLine();
      }

      Console.WriteLine("Exit");
    }
  }
}
