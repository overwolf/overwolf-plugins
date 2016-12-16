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
    public FileListenerWorker()
    {
      IsCanceled = false;
    }
    public bool IsCanceled { get; set; }
    public void ListenOnFile(string id, string filename, bool skipToEnd, Action<object, object, object> callback,
      Action<object, object, object> notifierDelegate)
    {
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


          if (!skipToEnd)
          {
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
              notifierDelegate(id, true, line);
            }
          }

          //start at the end of the file
          long lastMaxOffset = reader.BaseStream.Length;

          while (!IsCanceled)
          {
            System.Threading.Thread.Sleep(100);

            //if the file size has not changed, idle
            if (reader.BaseStream.Length == lastMaxOffset)
              continue;

            if (lastMaxOffset > reader.BaseStream.Length)
            {
             // lastMaxOffset = reader.BaseStream.Position;
              notifierDelegate(id, false, "truncated");
              return;
            }

            //seek to the last max offset
            reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

            //read out of the file until the EOF
            string line = "";
            while (!IsCanceled && (line = reader.ReadLine()) != null)
            {
              notifierDelegate(id, true, line);
            }

            //update the last max offset
            lastMaxOffset = reader.BaseStream.Position;
          }
        }

        if (notifierDelegate != null)
          notifierDelegate(id, false, "Listener Terminated");
      }
      catch (Exception ex)
      {
        if (callback != null)
          callback(id, false, "Terminated with Unknown error " + ex.ToString());

        if (notifierDelegate != null)
          notifierDelegate(id, false, "Terminated with Unknown error " + ex.ToString());
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
