using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins.simpleio {
  internal class RegistryManager {

    // ------------------------------------------------------------------------
    public static Task<RegistryKey> CurrentUserOpenKey(string key, bool writable = false) {
      return Task.Run(() => {
        try {
          return Registry.CurrentUser.OpenSubKey(key, writable);
        } catch {
          return null;
        }
      });
    }

    // ------------------------------------------------------------------------
    public static Task<RegistryKey> LocalMachineOpenKey(string key, bool writable = false) {
      return Task.Run(() => {
        try {
          return Registry.LocalMachine.OpenSubKey(key, writable);
        } catch {
          return null;
        }
      });
    }

    // ------------------------------------------------------------------------
    public static Task<RegistryKey> ClassesRootOpenKey(string key, bool writable = false) {
      return Task.Run(() => {
        try {
          return Registry.ClassesRoot.OpenSubKey(key, writable);
        } catch {
          return null;
        }
      });
    }


  }
}
