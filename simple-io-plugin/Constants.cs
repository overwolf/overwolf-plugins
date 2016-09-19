using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins {
  static class Constants {
    public static string kProgramFiles =
      Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

    public static string kProgramFilesX86 =
      Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

    public static string kCommonFiles =
      Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

    public static string kCommonFilesX86 =
      Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);

    public static string kCommonAppDataFiles =
      Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

    public static string kDesktop =
      Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    public static string kWinDir =
      Environment.GetFolderPath(Environment.SpecialFolder.Windows);

    public static string kSysDir =
      Environment.GetFolderPath(Environment.SpecialFolder.System);

    public static string kSysDirX86 =
      Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);

    public static string kMyDocuments =
      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public static string kMyVideos =
      Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

    public static string kMyPictures =
      Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    public static string kMyMusic =
      Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

    public static string kCommonDocuments =
      Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

    public static string kFavorites =
      Environment.GetFolderPath(Environment.SpecialFolder.Favorites);

    public static string kFonts =
      Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

    public static string kStartMenu =
      Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);

    public static string kLocalApplicationData =
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
  }
}
