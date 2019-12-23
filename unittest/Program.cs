using System;

namespace overwolf.plugins.unittest { 
  class Program {
    static void Main(string[] args) {
      DownloaderTests downloaderTests = new DownloaderTests();
      downloaderTests.Run();

      Console.ReadLine();

    }
  }
}
