using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins {
  internal class Utils {
    public static void AddWriteAccessToFile(string serverLogPath) {
      try {
        var fileSecurity = File.GetAccessControl(serverLogPath);
        var fileAccessRule = new FileSystemAccessRule(
          WindowsIdentity.GetCurrent().Name, FileSystemRights.FullControl,
          AccessControlType.Allow);

        fileSecurity.AddAccessRule(fileAccessRule);

        File.SetAccessControl(serverLogPath, fileSecurity);
      } catch {
      }
    }

    public static void MakeFileReadWrite(string serverLogPath) {
      try {
        var attributes = File.GetAttributes(serverLogPath);
        if ((attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly) {
          return;
        }

        attributes = attributes & ~FileAttributes.ReadOnly;
        File.SetAttributes(serverLogPath, attributes);
      } catch {
      }
    }

  }
}
