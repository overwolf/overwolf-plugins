using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins
{
  internal class FileListenerWorker
  {
    string _fileName;
    public FileListenerWorker()
    {
      IsCanceled = false;
    }
    public bool IsCanceled { get; set; }
    public void ListenOnFile(string id, string filename, bool skipToEnd, Action<object, object, object> callback,
      Action<object, object, object, bool> notifierDelegate)
    {
      _fileName = filename;
      try
      {
        if (!CanReadFile(filename))
        {
          callback(id, false, "Can't access file");
          return;
        }
       
        using (StreamReader reader = new StreamReader(new FileStream(filename,
          FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
          callback(id, true, "");
          callback = null;

          long lastFilePosition = 0;
          // 
          if (!skipToEnd) {
            int lineCount = GetLineCount(filename);
            string line = "";
            int lineIndex = 0;
            while ((line = reader.ReadLine()) != null) {
              notifierDelegate(id, true, line, (lineIndex++) >= lineCount);
            }
            lastFilePosition = reader.BaseStream.Position;
          } else {
            //start at the end of the file
            lastFilePosition = reader.BaseStream.Seek(0, SeekOrigin.End);
          }          

          while (!IsCanceled)
          {
            System.Threading.Thread.Sleep(100);

            //if the file size has not changed, idle
            if (reader.BaseStream.Length == lastFilePosition)
              continue;

            if (lastFilePosition > reader.BaseStream.Length)
            {
             // lastMaxOffset = reader.BaseStream.Position;
              notifierDelegate(id, false, "truncated", false);
              return;
            }

            //seek to the last max offset
            reader.BaseStream.Seek(lastFilePosition, SeekOrigin.Begin);

            //read out of the file until the EOF
            string line = "";
            while (!IsCanceled && (line = reader.ReadLine()) != null)
            {
              notifierDelegate(id, true, line, true);
            }

            //update the last max offset
            lastFilePosition = reader.BaseStream.Position;
          }
        }

        if (notifierDelegate != null)
          notifierDelegate(id, false, "Listener Terminated", false);
      }
      catch (Exception ex)
      {
        if (callback != null)
          callback(id, false, "Terminated with Unknown error " + ex.ToString());

        if (notifierDelegate != null)
          notifierDelegate(id, false, "Terminated with Unknown error " + ex.ToString(), false);
      }
     
    }

    private int GetLineCount(string filename) {
      try {
        return File.ReadLines(filename).Count();
      } catch {
        return int.MaxValue;
      }
    }

    private bool CanReadFile(string filename)
    {
      try
      {
        using (FileStream fileStream = (new FileStream(filename,
          FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
          return true;
        }
      }
      catch (Exception)
      {
        Utils.MakeFileReadWrite(filename);
        Utils.AddWriteAccessToFile(filename);

        try
        {
          using (FileStream fileStream = (new FileStream(filename,
            FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
          {
            return true;
          }
        }
        catch
        {
          return false;
        }
      }
    }
  }
}
