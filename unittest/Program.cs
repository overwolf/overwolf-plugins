using System;

namespace overwolf.plugins.unittest { 
  class Program {
    static void Main(string[] args) {
      //DownloaderTests downloaderTests = new DownloaderTests();
      //downloaderTests.Run();
      ProcessManagerTests processManagerTests = new ProcessManagerTests();
      processManagerTests.Run();

      Console.ReadLine();

    }
  }
}
