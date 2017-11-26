using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins {
  static class Constants {
    public static string kProgramFiles =
      GetFolderPathSafe(Environment.SpecialFolder.ProgramFiles);

    public static string kProgramFilesX86 =
      GetFolderPathSafe(Environment.SpecialFolder.ProgramFilesX86);

    public static string kCommonFiles =
      GetFolderPathSafe(Environment.SpecialFolder.CommonProgramFiles);

    public static string kCommonFilesX86 =
      GetFolderPathSafe(Environment.SpecialFolder.CommonProgramFilesX86);

    public static string kCommonAppDataFiles =
      GetFolderPathSafe(Environment.SpecialFolder.CommonApplicationData);

    public static string kDesktop =
      GetFolderPathSafe(Environment.SpecialFolder.Desktop);

    public static string kWinDir =
      GetFolderPathSafe(Environment.SpecialFolder.Windows);

    public static string kSysDir =
      GetFolderPathSafe(Environment.SpecialFolder.System);

    public static string kSysDirX86 =
      GetFolderPathSafe(Environment.SpecialFolder.SystemX86);

    public static string kMyDocuments =
      GetFolderPathSafe(Environment.SpecialFolder.MyDocuments);

    public static string kMyVideos =
      GetFolderPathSafe(Environment.SpecialFolder.MyVideos);

    public static string kMyPictures =
      GetFolderPathSafe(Environment.SpecialFolder.MyPictures);

    public static string kMyMusic =
      GetFolderPathSafe(Environment.SpecialFolder.MyMusic);

    public static string kCommonDocuments =
     GetFolderPathSafe(Environment.SpecialFolder.CommonDocuments);

    public static string kFavorites =
     GetFolderPathSafe(Environment.SpecialFolder.Favorites);

    public static string kFonts =
     GetFolderPathSafe(Environment.SpecialFolder.Fonts);

    public static string kStartMenu =
     GetFolderPathSafe(Environment.SpecialFolder.StartMenu);

    public static string kLocalApplicationData =
     GetFolderPathSafe(Environment.SpecialFolder.LocalApplicationData);

    private static string GetFolderPathSafe(Environment.SpecialFolder folder) {
      try {
        return Environment.GetFolderPath(folder);
      } catch {
        return string.Empty;
      }
    }
  }
}
