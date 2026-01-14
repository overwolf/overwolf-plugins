using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Management;

namespace overwolf.plugins.simpleio {
  public class BsodInfo {
    public DateTime CheckedFromUtc { get; set; }
    public bool HasBugcheckEvent { get; set; }
    public bool HasNewMinidump { get; set; }
    public List<BsodEvent> Events { get; set; } = new List<BsodEvent>();
    public List<FileInfo> Minidumps { get; set; } = new List<FileInfo>();
    public List<EventBrief> KernelPower41Events { get; set; } = new List<EventBrief>();
  }

  public class BsodEvent {
    public DateTime? TimeCreatedUtc { get; set; }
    public string ProviderName { get; set; }
    public int EventId { get; set; }
    public string BugcheckCode { get; set; }
    public string Message { get; set; }
  }

  public class EventBrief {
    public DateTime TimeCreatedUtc { get; set; }
    public int EventId { get; set; }
    public string ProviderName { get; set; }
    public string Message { get; set; }
  }


  public class bsodDetector {
    public static bool BsodSinceLastBoot(int hoursBack) {
      var boot = GetLastBootTimeUtc();
      DateTime sinceUtc = DateTime.UtcNow.AddHours(-hoursBack);
      if (boot <= sinceUtc) {
        return false;
      }

      var info = new BsodInfo { CheckedFromUtc = sinceUtc };

      // 1) Event ID 1001 (BugCheck) — two possible providers
      var EventBrief = GetKernelPower41Since(sinceUtc);
      if (EventBrief.Count > 0) {
        info.HasBugcheckEvent = true;
      }

      // 3) New minidumps since boot
      var dumps = GetDumpsSince(sinceUtc);
      if (dumps.Count > 0) {
        info.HasNewMinidump = true;
      }

      // “BSOD likely” if we saw a BugCheck event OR a new minidump
      return info.HasNewMinidump || info.HasBugcheckEvent;
    }

    public static DateTime GetLastBootTimeUtc() {
      // WMI is the most reliable for last boot time across Windows versions
      using (var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem"))
        foreach (ManagementObject mo in searcher.Get()) {
          // CIM_DATETIME -> parse to DateTime
          var lastBoot = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
          return lastBoot.ToUniversalTime();
        }
      // Fallback (rare)
      return DateTime.UtcNow.AddMinutes(-30);
    }

    public static List<BsodEvent> GetBugChecksSince(DateTime sinceUtc) {
      var results = new List<BsodEvent>();
      // Two common providers for Event ID 1001
      string[] providers =
      {
            "Microsoft-Windows-WER-SystemErrorReporting",
            "BugCheck" // legacy name on some systems
        };

      foreach (var provider in providers) {
        // We’ll filter time in code for simplicity/robustness
        string xpath = "*[System[Provider[@Name='" + provider + "'] and (EventID=1001)]]";
        var q = new EventLogQuery("System", PathType.LogName, xpath) { ReverseDirection = true };
        using (var reader = new EventLogReader(q))
          for (EventRecord rec = reader.ReadEvent(); rec != null; rec = reader.ReadEvent()) {
            if (rec.TimeCreated is DateTime t && t.ToUniversalTime() >= sinceUtc) {
              results.Add(ParseBugCheck(rec));
            }
            else {
              // Logs are reverse (newest first); once we’re before 'sinceUtc' we can stop
              break;
            }
          }
      }
      return results;
    }

    public static List<EventBrief> GetKernelPower41Since(DateTime sinceUtc) {
      var results = new List<EventBrief>();
      string xpath = "*[System[Provider[@Name='Microsoft-Windows-Kernel-Power'] and (EventID=41)]]";
      var q = new EventLogQuery("System", PathType.LogName, xpath) { ReverseDirection = true };
      using (var reader = new EventLogReader(q))
        for (EventRecord rec = reader.ReadEvent(); rec != null; rec = reader.ReadEvent()) {
          if (rec.TimeCreated is DateTime t && t.ToUniversalTime() >= sinceUtc) {
            results.Add(new EventBrief {
              TimeCreatedUtc = t.ToUniversalTime(),
              EventId = rec.Id,
              ProviderName = rec.ProviderName ?? "Kernel-Power",
              Message = SafeFormat(rec.FormatDescription())
            });
          }
          else break;
        }
      return results;
    }


    public static List<FileInfo> GetDumpsSince(DateTime sinceUtc) {
      var list = new List<FileInfo>();

      var windowsFolder = 
          Environment.GetFolderPath(Environment.SpecialFolder.Windows);

      var dumpDir = new DirectoryInfo(Path.Combine(windowsFolder, "Minidump"));
      if (dumpDir.Exists) {
        foreach (var file in dumpDir.GetFiles("*.dmp")) {
          if (file.LastWriteTimeUtc >= sinceUtc)
            list.Add(file);
        }
      }

      var fullDumpFile = new FileInfo(Path.Combine(windowsFolder, "memory.dmp"));
      if (fullDumpFile.Exists && fullDumpFile.LastWriteTimeUtc >= sinceUtc) {
        list.Add(fullDumpFile);
      }

      return list;
    }

    private static BsodEvent ParseBugCheck(EventRecord rec) {
      string msg = SafeFormat(rec.FormatDescription());
      // BugCheck code usually appears like: "The computer has rebooted from a bugcheck.  The bugcheck was: 0x0000009f (0x..., ...)."
      // Try to pull the 0x code for convenience:
      string code = null;
      if (!string.IsNullOrEmpty(msg)) {
        // crude extract
        int idx = msg.IndexOf("0x", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) {
          int end = idx;
          while (end < msg.Length && !char.IsWhiteSpace(msg[end])) end++;
          code = msg.Substring(idx, end - idx);
        }
      }

      return new BsodEvent {
        TimeCreatedUtc = rec.TimeCreated?.ToUniversalTime(),
        ProviderName = rec.ProviderName,
        EventId = rec.Id,
        BugcheckCode = code,
        Message = msg
      };
    }

    private static string SafeFormat(string s) =>
       string.IsNullOrWhiteSpace(s) ? "" : s.Trim();


  }
}
